using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CustomNpcs.Npcs;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;
using System.Diagnostics;
using System.Reflection;
using Corruption.PluginSupport;
using PythonTS.Models;

namespace CustomNpcs.Invasions
{
    /// <summary>
    ///     Represents an invasion manager. This class is a singleton.
    /// </summary>
    public sealed class InvasionManager : CustomTypeManager<InvasionDefinition>, IDisposable
    {
		private static readonly Color InvasionTextColor = new Color(175, 25, 255);
		       
        private readonly CustomNpcsPlugin _plugin;
        private readonly Random _random = new Random();
		private string _currentMiniboss;
		private bool currentMinibossKilled;
        private int _currentPoints;
        private int _currentWaveIndex;
        private DateTime _lastProgressUpdate;
        private int _requiredPoints;

        public static new List<InvasionDefinition> Definitions { get; set; }


        internal InvasionManager(CustomNpcsPlugin plugin)
        {
            _plugin = plugin;

			BasePath = "npcs";
			ConfigPath = Path.Combine(BasePath, "invasions.json");

			if (!File.Exists(ConfigPath))
			{
                List<InvasionDefinition> obj = new()
                {
                    new InvasionDefinition()
                };
                var text = JsonConvert.SerializeObject(obj);
				File.WriteAllText(ConfigPath, text);
			}
			AssemblyNamePrefix = "Invasion_";

			LoadDefinitions();
			
            GeneralHooks.ReloadEvent += OnReload;
            // Register OnGameUpdate with priority 1 to guarantee that InvasionManager runs before NpcManager.
            ServerApi.Hooks.GameUpdate.Register(_plugin, OnGameUpdate, 1);
            ServerApi.Hooks.NpcKilled.Register(_plugin, OnNpcKilled);
        }

        public override void LoadDefinitions()
        {
            CustomNpcsPlugin.Instance.LogPrint($"Loading Invasions...", TraceLevel.Info);
            Definitions = InvasionDefinition.LoadAll(ConfigPath);
            foreach (var def in Definitions)
            {
                def.CreateModules();
            }
        }

        /// <summary>
        ///     Gets the invasion manager instance.
        /// </summary>
        public static InvasionManager Instance { get; internal set; }

        /// <summary>
        ///     Gets the current invasion, or <c>null</c> if there is none.
        /// </summary>
        public InvasionDefinition CurrentInvasion { get; private set; }

        /// <summary>
        ///     Disposes the invasion manager.
        /// </summary>
        public void Dispose()
        {
            GeneralHooks.ReloadEvent -= OnReload;
            ServerApi.Hooks.GameUpdate.Deregister(_plugin, OnGameUpdate);
            ServerApi.Hooks.NpcKilled.Deregister(_plugin, OnNpcKilled);

            CurrentInvasion = null;

			ClearDefinitions();
        }
		        
        /// <summary>
        ///     Starts the specified invasion.
        /// </summary>
        /// <param name="invasion">The invasion, or <c>null</c> to stop the current invasion.</param>
        public void StartInvasion(InvasionDefinition invasion)
        {
			EndInvasion();

			CurrentInvasion = invasion;
            if (CurrentInvasion != null)
            {
				try
				{

					invasion.Script.ExecuteMethod("OnInvasionStart");
				}
				catch(Exception ex)
				{
					Utils.LogScriptRuntimeError(ex);
				}
				
				CurrentInvasion.HasStarted = true;
				_currentWaveIndex = 0;
                StartCurrentWave();
            }
        }

		public void EndInvasion()
		{
			if(CurrentInvasion!=null)
			{
				TryEndPreviousWave();
								
				try
				{
					CurrentInvasion.Script.ExecuteMethod("OnInvasionEnd");
				}
				catch( Exception ex )
				{
					Utils.LogScriptRuntimeError(ex);
				}
				
				TSPlayer.All.SendMessage(CurrentInvasion.CompletedMessage, new Color(175, 75, 225));
				CurrentInvasion.HasStarted = false;//...probably not needed
				CurrentInvasion = null;
			}
		}
		
/*		protected override IEnumerable<EnsuredMethodSignature> GetEnsuredMethodSignatures()
		{
			var sigs = new List<EnsuredMethodSignature>()
			{
				new EnsuredMethodSignature("OnWaveStart")
					.AddParameter("waveIndex",typeof(int))
					.AddParameter("waveDefinition",typeof(WaveDefinition)),

				new EnsuredMethodSignature("OnWaveEnd")
					.AddParameter("waveIndex",typeof(int))
					.AddParameter("waveDefinition",typeof(WaveDefinition)),

				new EnsuredMethodSignature("OnWaveUpdate")
					.AddParameter("waveIndex",typeof(int))
					.AddParameter("waveDefinition",typeof(WaveDefinition))
					.AddParameter("currentPoints",typeof(int)),
			};

			return sigs;
		}*/

/*		protected override void LoadDefinitions()
		{
			CustomNpcsPlugin.Instance.LogPrint($"Loading CustomInvasions...", TraceLevel.Info);
			base.LoadDefinitions();
		}*/
		
		private void NotifyRelevantPlayers()
        {
            foreach (var player in TShock.Players.Where(p => p != null && p.Active && ShouldSpawnInvasionNpcs(p)))
            {
                player.SendData(PacketTypes.ReportInvasionProgress, "", _currentPoints, _requiredPoints, 0,
                    _currentWaveIndex + 1);
            }
        }

        private void OnGameUpdate(EventArgs args)
        {
            if (CurrentInvasion == null || CurrentInvasion.HasStarted == false)
				return;
            						
			var activePlayers = TShock.Players.Where(p => p?.Active == true);

			if(activePlayers.Count()<1)
			{
				CustomNpcsPlugin.Instance.LogPrint("There no more active players, ending the current invasion.", TraceLevel.Info);
				EndInvasion();
				return;
			}

            Utils.TrySpawnForEachPlayer(TrySpawnInvasionNpc);

			// Prevent other NPCs from spawning for relevant players.
			foreach( var player in activePlayers )
            {
				if(ShouldSpawnInvasionNpcs(player))
					player.TPlayer.nearbyActiveNPCs = 10000;
            }

            if(_currentPoints >= _requiredPoints &&
				( _currentMiniboss == null || currentMinibossKilled == true ))
            {
				if (++_currentWaveIndex == CurrentInvasion.Waves.Count)
                {
					EndInvasion();
                    return;
                }
				else
				{
					TryEndPreviousWave();
					StartCurrentWave();
				}
            }

            var now = DateTime.UtcNow;
            if (now - _lastProgressUpdate > TimeSpan.FromSeconds(1))
            {
                NotifyRelevantPlayers();
                _lastProgressUpdate = now;
            }

			try
			{
                CurrentInvasion.Script.ExecuteMethod("OnUpdate");
            }
			catch( Exception ex )
			{
				Utils.LogScriptRuntimeError(ex);
			}
		}

		private void OnNpcKilled(NpcKilledEventArgs args)
        {
            if (CurrentInvasion == null)
				return;
            
            var npc = args.npc;
            var customNpc = NpcManager.Instance?.GetCustomNpc(npc);
            var npcNameOrType = customNpc?.Definition.Identifier ?? npc.netID.ToString();
            if (npcNameOrType.Equals(_currentMiniboss, StringComparison.OrdinalIgnoreCase))
            {
				try
				{
					CurrentInvasion.Script.ExecuteMethod("OnBossDefeated");
                }
				catch(Exception ex)
				{
					Utils.LogScriptRuntimeError(ex);
					_currentMiniboss = null;
				}

				currentMinibossKilled = true;
	        }
            else if (CurrentInvasion.NpcPointValues.TryGetValue(npcNameOrType, out var points))
            {
                _currentPoints += points;
                _currentPoints = Math.Min(_currentPoints, _requiredPoints);
                NotifyRelevantPlayers();

				if(_currentWaveIndex>=0 && _currentWaveIndex<CurrentInvasion.Waves.Count )
				{
					var wave = CurrentInvasion.Waves[_currentWaveIndex];
					if( wave != null )
					{
						try
						{
							ScriptArguments[] Args = new ScriptArguments[]
							{
                                new ScriptArguments("waveIndex", _currentWaveIndex),
                                new ScriptArguments("waveDefinition", wave),
                                new ScriptArguments("currentPoints", _currentPoints),
                            };
							CurrentInvasion.Script.ExecuteMethod("OnWaveEnd", Args);
						}
						catch( Exception ex )
						{
							Utils.LogScriptRuntimeError(ex);
						}
					}
				}
			}
        }

        private void OnReload(ReloadEventArgs args)
        {
            CurrentInvasion = null;
			
			Definitions.Clear();

			LoadDefinitions();

			args.Player.SendSuccessMessage("[CustomNpcs] Reloaded invasions!");
        }

        private bool ShouldSpawnInvasionNpcs(TSPlayer player)
        {
            var playerPosition = player.TPlayer.position;
            return !CurrentInvasion.AtSpawnOnly || Main.spawnTileX * 16.0 - 3000 < playerPosition.X &&
                   playerPosition.X < Main.spawnTileX * 16.0 + 3000 &&
                   playerPosition.Y < Main.worldSurface * 16.0 + NPC.sHeight;
        }

        private void StartCurrentWave()
        {
			var wave = CurrentInvasion.Waves[_currentWaveIndex];
			TSPlayer.All.SendMessage(wave.StartMessage, InvasionTextColor);
			_currentPoints = 0;
			_currentMiniboss = wave.Miniboss;
			currentMinibossKilled = false;
			_requiredPoints = wave.PointsRequired * ( CurrentInvasion.ScaleByPlayers ? TShock.Utils.GetActivePlayerCount() : 1 );

			if(wave!=null)
			{
				try
				{
					ScriptArguments[] args = new ScriptArguments[]
					{
						new ScriptArguments("waveIndex", _currentWaveIndex),
						new ScriptArguments("wave", wave)
					};
					CurrentInvasion.Script.ExecuteMethod("OnWaveEnd", args);
				}
				catch( Exception ex )
				{
					Utils.LogScriptRuntimeError(ex);
				}
			}
		}

		private void TryEndPreviousWave()
		{
			//run end event for previous wave, if there was a previous wave
			var previousWaveIndex = _currentWaveIndex - 1;
			if( previousWaveIndex >= 0 )
			{
				var previousWave = CurrentInvasion.Waves[previousWaveIndex];

				try
				{
					ScriptArguments[] args = new ScriptArguments[]
					{
						new ScriptArguments("previousWaveIndex", previousWaveIndex),
						new ScriptArguments("previousWave", previousWave)
					};
					CurrentInvasion.Script.ExecuteMethod("OnWaveEnd", args);

                }
				catch( Exception ex )
				{
					Utils.LogScriptRuntimeError(ex);
				}
			}
		}

        private void TrySpawnInvasionNpc(TSPlayer player, int tileX, int tileY)
        {
            if (!ShouldSpawnInvasionNpcs(player))
				return;
            
            var currentWave = CurrentInvasion.Waves[_currentWaveIndex];
            if (player.TPlayer.nearbyActiveNPCs >= currentWave.MaxSpawns || _random.Next(currentWave.SpawnRate) != 0)
				return;
            
            if (_currentPoints >= _requiredPoints && _currentMiniboss != null)
            {
				//only spawn mini boss if a current miniboss doesnt exist.
                foreach (var npc in Main.npc.Where(n => n?.active == true))
                {
                    var customNpc = NpcManager.Instance?.GetCustomNpc(npc);
                    var npcNameOrType = customNpc?.Definition.Identifier ?? npc.netID.ToString();
                    if (npcNameOrType.Equals(_currentMiniboss, StringComparison.OrdinalIgnoreCase))
                    {
                        return;
                    }
                }

				Utils.SpawnVanillaOrCustomNpc(_currentMiniboss, tileX, tileY);
            }
            else
            {
                var randomNpcNameOrType = Utils.PickRandomWeightedKey(currentWave.NpcWeights);
                Utils.SpawnVanillaOrCustomNpc(randomNpcNameOrType, tileX, tileY);
            }
        }
    }
}

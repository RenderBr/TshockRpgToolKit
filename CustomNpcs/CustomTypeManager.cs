using Corruption.PluginSupport;
using Newtonsoft.Json;
using PythonTS;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace CustomNpcs
{
    /// <summary>
    /// Base implementation for types that will manage custom type overrides of Terraria types.
    /// </summary>
    /// <typeparam name="TCustomType"></typeparam>
    public abstract class CustomTypeManager<TCustomType> where TCustomType : Definition
    {
        public static string BasePath { get; protected set; }
        public string ConfigPath { get; protected set; }

        /// <summary>
        /// Gets the IList of custom definitions managed by this instance.
        /// </summary>
        public virtual List<IDefinition> Definitions { get; protected set; } = new List<IDefinition>();

        //for fast access, instead of always doing a linear search through our definitions...
        private Dictionary<string, TCustomType> definitionMap { get; set; } = new Dictionary<string, TCustomType>();

        /// <summary>
        /// Gets or sets the Assembly name prefix to be applied during the next compile.
        /// </summary>
        /// <remarks> This is cached and applied to each ModuleManager right before compilation.</remarks>
        public string AssemblyNamePrefix { get; protected set; } = "";

        /*/// <summary>
		/// Loads in json definition files, and attempts to compile and link to any related scripts.
		/// </summary>
		protected virtual void LoadDefinitions()
		{
			if(!File.Exists(ConfigPath))
			{
				CustomNpcsPlugin.Instance.LogPrint($"Unable to find definition file, creating default file at {ConfigPath}.", TraceLevel.Warning);
				SaveDefaultFile(ConfigPath);	
			}

			var include = DefinitionInclude.Load<TCustomType>(ConfigPath);
			Definitions = DefinitionInclude.Flatten<TCustomType>(include);
			
			var rootResult = new ValidationResult(ConfigPath);
			rootResult.Source = ConfigPath;

            var usedNames = new HashSet<string>();
            foreach (var def in Definitions)
            {
                var name = "";
                var result = def.Validate();

                if (!string.IsNullOrWhiteSpace(def.Name))
                    name = $" - '{def.Name}'";


                if (!usedNames.Contains(def.Name))
                {
                    Debug.WriteLine(def.Name);
                    rootResult.Children.Add(result);
                    usedNames.Add(def.Name);
                }
                else
                {
                    // Log or handle the duplicate name as needed.
                    CustomNpcsPlugin.Instance.LogPrint($"Duplicate name found for definition {def.Name}. Skipping this definition.", TraceLevel.Warning);
                    continue;
                }

                // Log or handle the missing name as needed.
                if (string.IsNullOrWhiteSpace(def.Name))
                {
                    CustomNpcsPlugin.Instance.LogPrint($"A definition with no name was found. Skipping this definition.", TraceLevel.Warning);
                }

        }

            Debug.WriteLine(rootResult);
			CustomNpcsPlugin.Instance.LogPrint(rootResult);

			CustomNpcsPlugin.Instance.LogPrint("**************************");

			//Definitions = DefinitionLoader.LoadFromFile<TCustomType>(ConfigPath);

			CustomNpcsPlugin.Instance.LogPrint("Compiling scripts...", TraceLevel.Info);

			//get script files paths
			var booScripts = Definitions.Where(d => !string.IsNullOrWhiteSpace(d.ScriptPath))
										 .Select(d => Path.Combine(BasePath, d.ScriptPath))
										 .ToList();

			var newModuleManager = new BooModuleManager(CustomNpcsPlugin.Instance,
													ScriptHelpers.GetReferences(),
													ScriptHelpers.GetDefaultImports(),
													GetEnsuredMethodSignatures());

			newModuleManager.AssemblyNamePrefix = AssemblyNamePrefix;

			foreach( var f in booScripts )
				newModuleManager.Add(f);
			
			Dictionary<string, CompilerContext> results = null;

			if( ModuleManager != null )
				results = newModuleManager.IncrementalCompile(ModuleManager);
			else
				results = newModuleManager.Compile();
			
			ModuleManager = newModuleManager;

			var scriptedDefinitions = Definitions.Where(d => !string.IsNullOrWhiteSpace(d.ScriptPath));

			foreach(var def in scriptedDefinitions)
			{
				var fileName = Path.Combine(BasePath, def.ScriptPath);

				//if newly compile assembly, examine the context, and try to link to the new assembly
				if( results.TryGetValue(fileName, out var context) )
				{
					var scriptAssembly = context.GeneratedAssembly;

					if( scriptAssembly != null )
					{
						var result = def.LinkToScriptAssembly(scriptAssembly);

						//if(!result)
						//	//	CustomNpcsPlugin.Instance.LogPrint($"Failed to link {kvp.Key}.", TraceLevel.Info);
					}
				}
				else
				{
					var scriptAssembly = ModuleManager[fileName];

					if(scriptAssembly!=null)
					{
						var result = def.LinkToScriptAssembly(scriptAssembly);

						//if(!result)
						//	//	CustomNpcsPlugin.Instance.LogPrint($"Failed to link {kvp.Key}.", TraceLevel.Info);
					}
				}
			}

			definitionMap = new Dictionary<string, TCustomType>();

            var usedNames1 = new HashSet<string>();
            foreach (var def in Definitions)
            {
                var defName = def.Name.ToLowerInvariant();
                if (!usedNames1.Contains(defName))
                {
                    definitionMap.Add(defName, def);
                    usedNames1.Add(defName);
                }
            }			
		}*/

        public void ClearDefinitions()
        {
            Definitions?.Clear();
            definitionMap.Clear();
        }

        /// <summary>
		/// Loads in json definition files, and attempts to compile and link to any related scripts.
		/// </summary>
		public virtual void LoadDefinitions()
        {
            if (!File.Exists(ConfigPath))
            {
                CustomNpcsPlugin.Instance.LogPrint($"Unable to find definition file, creating default file at {ConfigPath}.", TraceLevel.Warning);
                SaveDefaultFile(ConfigPath);

            }

            var rootResult = new ValidationResult(ConfigPath);
            rootResult.Source = ConfigPath;

            CustomNpcsPlugin.Instance.LogPrint(rootResult);

            List<IScript> defs = Script.Modules.Where(x => x.FilePath.Contains(BasePath)).ToList();
            /*            foreach (var def in defs)
                            Definitions.Add(new CustomDefinition(def.FilePath));

                        foreach (var f in Definitions)
                            Script.AddModuleDefault(f.FilePath);*/

        }

        /// <summary>
        ///     Finds the definition with the specified name.
        /// </summary>
        /// <param name="name">The name, which must not be <c>null</c>.</param>
        /// <returns>The definition, or <c>null</c> if it does not exist.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="name" /> is <c>null</c>.</exception>
        public virtual IDefinition FindDefinition(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            var lowerName = name.ToLowerInvariant();

            return Definitions.FirstOrDefault(d => d.Identifier.ToLowerInvariant() == lowerName, null);
        }

        /// <summary>
        /// Creates and writes default definition data to a file. Used when no file can be found.
        /// </summary>
        /// <param name="filePath"></param>
        internal virtual void SaveDefaultFile(string filePath)
        {
            var array = new TCustomType[0];
            var json = JsonConvert.SerializeObject(array);
            var defaultNpcs = @"
            [
                {
                    ""CustomID"": ""Example"",
                    ""BaseType"": 352,
                    ""ScriptPath"": null,
                    ""BaseOverride"": {
                                ""AiStyle"": null,
                      ""BuffImmunities"": [],
                      ""Defense"": 50,
                      ""HasNoCollision"": true,
                      ""HasNoGravity"": true,
                      ""IsBoss"": false,
                      ""IsImmortal"": false,
                      ""IsImmuneToLava"": false,
                      ""IsTrapImmune"": false,
                      ""KnockbackMultiplier"": 1.0,
                      ""MaxHp"": 300,
                      ""Name"": ""Lovely Example"",
                      ""NpcSlots"": null,
                      ""Value"": null,
                      ""BehindTiles"": null,
                      ""DontTakeDamageFromHostiles"": null
                    },
                    ""Loot"": {
                      ""TallyKills"": false,
                      ""IsOverride"": true,
                      ""Entries"": [
                        {
                          ""Name"": ""Dirt Block"",
                          ""MinStackSize"": 0,
                          ""MaxStackSize"": 0,
                          ""Prefix"": 0,
                          ""Chance"": 0.0
                        }
                      ]
                    },
                    ""Spawning"": {
                      ""ShouldSpawn"": true,
                      ""ShouldReplace"": false,
                      ""SpawnRateOverride"": 1900
                    }
                }
            ]";
            var defaultProj = @"
            [
                {
                ""CustomID"": ""ExampleProj"",
                ""Name"": ""Example"",
                ""ScriptPath"": null,
                ""BaseType"": 0,
                ""BaseOverride"": {
                                ""AiStyle"": 0,
                    ""Ai"": null,
                    ""Damage"": 6,
                    ""KnockBack"": 300,
                    ""Friendly"": false,
                    ""Hostile"": false,
                    ""MaxPenetrate"": 8,
                    ""TimeLeft"": 800,
                    ""Magic"": true,
                    ""Light"": 20.0,
                    ""Melee"": false,
                    ""ColdDamage"": true,
                    ""TileCollide"": true,
                    ""IgnoreWater"": true
                    }
                }
            ]";
            var defaultInvasions = @"
            [
                {
                ""Name"": ""New Invasion"",
                ""ScriptPath"": null,
                ""NpcPointValues"": { },
                ""CompletedMessage"": ""The invasion has ended!"",
                ""AtSpawnOnly"": false,
                ""ScaleByPlayers"": false,
                ""Waves"": [
                      {
                        ""Name"": ""New Wave 1"",
                        ""NpcWeights"": {
                          ""example"": 6
                        },
                        ""PointsRequired"": 20,
                        ""MaxSpawns"": 10,
                        ""SpawnRate"": 20,
                        ""StartMessage"": ""The wave has started!"",
                        ""Miniboss"": null
                      },
                      {
                        ""Name"": ""MidBoss"",
                        ""NpcWeights"": {
                          ""example"": 8
                        },
                        ""PointsRequired"": 40,
                        ""MaxSpawns"": 1,
                        ""SpawnRate"": 20,
                        ""StartMessage"": ""The wave has started!"",
                        ""Miniboss"": null
                      },
                      {
                        ""Name"": ""Boss"",
                        ""NpcWeights"": {
                          ""example"": 5
                        },
                        ""PointsRequired"": 60,
                        ""MaxSpawns"": 10,
                        ""SpawnRate"": 20,
                        ""StartMessage"": ""The wave has started!"",
                        ""Miniboss"": null
                      }
                    ]
                }
            ]";
            if (filePath.Contains("npcs.json"))
            {
                json = defaultNpcs;
            }
            else if (filePath.Contains("projectiles.json"))
            {
                json = defaultProj;
            }
            else if (filePath.Contains("invasions.json"))
            {
                json = defaultInvasions;
            }

            File.WriteAllText(filePath, json);
        }
    }
}

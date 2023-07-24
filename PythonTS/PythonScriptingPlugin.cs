using PythonTS.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;

namespace PythonTS
{
    [ApiVersion(2, 1)]
    public sealed class PythonScriptingPlugin : TerrariaPlugin
    {
        public override string Author => "Average";
        public override string Description => "Python scripting for TShock.";
        public override string Name => "PythonTS";
        public override Version Version => Assembly.GetExecutingAssembly().GetName().Version;
        public static string DataDirectory { get; set; } = "scripting";
        internal static string ConfigPath => Path.Combine(DataDirectory, "config.json");

        public static List<IScript> Modules { get; set; } = new();

        public static PythonScriptingPlugin Instance { get; private set; }

        public PythonScriptingPlugin(Main game) : base(game)
        {
            Instance = this;
        }

        private Script ServerStart;
        private Script PlayerJoin;
        private Script PlayerLeave;

        public override void Initialize()
        {
            GeneralHooks.ReloadEvent += OnReload;

            ServerApi.Hooks.GamePostInitialize.Register(this, OnPostInitialize);
            ServerApi.Hooks.NetGreetPlayer.Register(this, OnServerJoin);
            ServerApi.Hooks.ServerLeave.Register(this, OnServerLeave);

            Commands.ChatCommands.Add(new Command("pythonts.control", CommandLoad, "py", "python"));
            Commands.ChatCommands.Add(new Command("pythonts.control", RefreshAll, "pyref", "pyr", "pyreload"));

            onLoad();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                GeneralHooks.ReloadEvent -= OnReload;
                ServerApi.Hooks.NetGreetPlayer.Deregister(this, OnServerJoin);
                ServerApi.Hooks.ServerLeave.Deregister(this, OnServerLeave);
            }

            base.Dispose(disposing);
        }

        private void onLoad()
        {
            if (!Directory.Exists(DataDirectory))
                Directory.CreateDirectory(DataDirectory);

            try
            {
                foreach (string filePath in Directory.GetFiles(DataDirectory))
                {
                    Script file = new(filePath);
                    Modules.Add(file);
                }
                LoadDefaultScripts();
            }
            catch (Exception ex)
            {
                Executor.LogPrint(ex.ToString(), TraceLevel.Error);
            }
        }

        private void OnPostInitialize(EventArgs args) => ServerStart?.Execute();

        private void OnReload(ReloadEventArgs e) => onLoad();

        private void OnServerJoin(GreetPlayerEventArgs args)
        {
            var player = TShock.Players[args.Who];
            ScriptArguments[] arg = new ScriptArguments[]
            {
                new("Player", player)
            };
            PlayerJoin?.Execute(arg);
        }

        private void OnServerLeave(LeaveEventArgs args)
        {
            var player = new TSPlayer(args.Who);
            ScriptArguments[] arg = new ScriptArguments[]
            {
                new("Player", player)
            };
            PlayerLeave?.Execute(arg);
        }

        private void LoadDefaultScripts()
        {
            ServerStart = Script.FindDefault("ServerStart");
            PlayerJoin = Script.FindDefault("PlayerJoin");
            PlayerLeave = Script.FindDefault("PlayerLeave");
        }

        internal void RunScript(Script script)
        {
            if (script == null)
                throw new ArgumentNullException("script");

            script.Execute();
        }

        private void RefreshAll(CommandArgs args) => Modules.ForEach(x => { x.Reload(); });
        private void CommandLoad(CommandArgs args)
        {
            var player = args.Player;

            if (args.Parameters.Count < 2)
            {
                player.SendErrorMessage("Not enough parameters.");
                player.SendErrorMessage($"Format is: {Commands.Specifier}py run <script>");
                return;
            }

            var sub = args.Parameters[0];
            var filePath = args.Parameters[1];

            if (sub != "run")
            {
                player.SendErrorMessage($"Unknown sub command '{sub}'.");
                return;
            }

            var fullFilePath = Path.Combine(DataDirectory, filePath);

            if (!File.Exists(fullFilePath))
            {
                player.SendErrorMessage($"Unable to find script '{filePath}'.");
                return;
            }

            Task.Run(() =>
            {
                try
                {
                    IScript? script = Modules.FirstOrDefault(x => x.FilePath == filePath, null);
                    if (script is null)
                    {
                        player.SendErrorMessage($"Unable to find script '{filePath}'.");
                        return;
                    }

                    if (script.Enabled)
                        script.Reload();
                    else
                        script.Execute();
                }
                catch
                {
                    player?.SendErrorMessage("Script failed. Check logs for error information.");
                }
            });
        }
    }
}

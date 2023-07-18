using PythonTS.Models;
using System.Diagnostics;
using System.Reflection;
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
        public static string DataDirectory { get; set; } = @"scripting";
        internal static string ConfigPath => Path.Combine(DataDirectory, @"config.json");

        public static List<IScript> Modules { get; set; } = new();

        public static PythonScriptingPlugin Instance { get; private set; }

        public PythonScriptingPlugin(Main game) : base(game)
        {
            Instance = this;
        }

        Script ServerStart;
        Script PlayerJoin;
        Script PlayerLeave;

        public override void Initialize()
        {
            GeneralHooks.ReloadEvent += OnReload;

            ServerApi.Hooks.GamePostInitialize.Register(this, OnPostInitialize);
            ServerApi.Hooks.GameUpdate.Register(this, OnGameUpdate);
            ServerApi.Hooks.NetGreetPlayer.Register(this, OnServerJoin);
            ServerApi.Hooks.ServerLeave.Register(this, OnServerLeave);

            Commands.ChatCommands.Add(new Command("pythonts.control", CommandLoad, "py", "python"));
            /*
			Commands.ChatCommands.Add(new Command("boots.control", CommandLoad, "boo")
			{
				HelpText = $"Syntax: {Commands.Specifier}boo run <script>"
			});
			*/
            onLoad();

        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                //JsonConfig.Save(this, Config.Instance, ConfigPath);

                GeneralHooks.ReloadEvent -= OnReload;
                //	PlayerHooks.PlayerChat -= OnPlayerChat;
                ServerApi.Hooks.GameUpdate.Deregister(this, OnGameUpdate);
                ServerApi.Hooks.NetGreetPlayer.Deregister(this, OnServerJoin);
                ServerApi.Hooks.ServerLeave.Deregister(this, OnServerLeave);
            }

            base.Dispose(disposing);
        }

        private void onLoad()
        {
            if (!Directory.Exists(DataDirectory))
                    Directory.CreateDirectory(DataDirectory);
            //Config.Instance = JsonConfig.LoadOrCreate<Config>(this, ConfigPath);
            try
            {
                foreach (string _f in Directory.GetFiles(DataDirectory))
                {
                    Script file = new(_f);
                    Modules.Add(file);
                }
                LoadDefaultScripts();

                /*                ScheduledScripts = new ConcurrentDictionary<string, Script>();
                                LoadScheduledScripts();*/
            }
            catch (Exception ex)
            {
                Extensions.LogPrint(ex.ToString(), TraceLevel.Error);
            }
        }

        private void OnPostInitialize(EventArgs args)
        {
            ServerStart?.Execute();

        }

        private void OnReload(ReloadEventArgs e)
        {
            onLoad();
        }

        private void OnServerJoin(GreetPlayerEventArgs args)
        {
            Debug.Print("OnServerJoin");
            var player = TShock.Players[args.Who];

            // if you are reading off the readme for help on deducting correct arguments
            // passed through to the respective script, each argument set will look something like this
            // in Python, these arguments are available via the string, ex. "Player",
            // this is a direct link to the player object, which is a TSPlayer class,
            // thus in Python you can use any public methods from the TSPlayer class
            ScriptArguments[] arg = new ScriptArguments[]
            {
                new("Player", player)

            };
            PlayerJoin?.Execute(arg);
        }

        private void OnServerLeave(LeaveEventArgs args)
        {
            Debug.Print("OnServerLeave");

            var player = new TSPlayer(args.Who);
            ScriptArguments[] arg = new ScriptArguments[]
            {
                new("Player", player)
            };
            PlayerLeave?.Execute(arg);
        }

        private void OnGameUpdate(EventArgs args)
        {
            // TODO: Add scheduled tasks
        }

        /// <summary>
        /// Scans the scripts directory for convention based filenames, and attempts to compile and cache them.
        /// </summary>
        internal void LoadDefaultScripts()
        {
            ServerStart = Script.FindDefault("ServerStart");
            PlayerJoin = Script.FindDefault("PlayerJoin");
            PlayerLeave = Script.FindDefault("PlayerLeave");
        }

        /// <summary>
        /// Runs a precompiled script, passing in the optional string arguments.
        /// </summary>
        /// <param name="script"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        internal void RunScript(Script script)
        {
            if (script == null)
                throw new ArgumentNullException("script");

            script.Execute();
        }

        private void CommandLoad(CommandArgs args)
        {
            var player = args.Player;

            if (args.Parameters.Count < 2)
            {
                player.SendErrorMessage($"Not enough parameters.");
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
                        player.SendErrorMessage($"Unable to find script '{filePath}'.");

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

            //player.SendErrorMessage($"{Commands.Specifier}boo run <script>");
        }
    }
}

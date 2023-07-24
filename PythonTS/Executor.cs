using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using PythonTS.Models;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using TerrariaApi.Server;
using TShockAPI;

namespace PythonTS
{
    public class Executor
    {
        public ScriptEngine Engine { get; set; }
        public ScriptScope Scope { get; set; }
        public ObjectOperations Operations { get; set; }

        public Executor()
        {
            Engine = Python.CreateEngine();
            Scope = Engine.CreateScope();
            Operations = Engine.CreateOperations();
        }

        public dynamic? Execute(Script source, ScriptArguments[] args = null)
        {
            try
            {
                InjectDependencies(source, out string contents, args);
                var result = Engine.Execute(contents, Scope);
                Engine.Runtime.IO.RedirectToConsole();
                return result;
            }
            catch (Exception e)
            {
                var eo = Engine.GetService<ExceptionOperations>();
                LogPrint(eo.FormatException(e), TraceLevel.Error);
            }
            return null;
        }

        public static void LogPrint(string message, TraceLevel level = TraceLevel.Error) => ServerApi.LogWriter.PluginWriteLine(PythonScriptingPlugin.Instance, message, level);

        public object GetVariable(string varName) => Scope.GetVariable(varName);

        public dynamic? ExecuteMethod(Script source, string methodName, ScriptArguments[] args = null)
        {
            try
            {
                InjectDependencies(source, out string contents, args);
                Engine.Execute(contents, Scope);
                Engine.Runtime.IO.RedirectToConsole();

                if (!Scope.ContainsVariable(methodName))
                    return null;

                return Operations.Invoke(Scope.GetVariable(methodName));
            }
            catch (Exception e)
            {
                var eo = Engine.GetService<ExceptionOperations>();
                LogPrint(eo.FormatException(e), TraceLevel.Error);
            }
            return null;
        }

        public void Disable()
        {
            Engine.Runtime.Shutdown();
        }

        public void InjectDependencies(Script source, out string contents, ScriptArguments[] args = null)
        {
            if (args != null)
            {
                foreach (var e in args)
                {
                    Scope.SetVariable(e.VariableName, e.Value);
                }
            }

            var builder = new StringBuilder();
            builder.AppendLine("import clr");
            builder.AppendLine("clr.AddReference(\"Corruption\")");
            builder.AppendLine("clr.AddReference(\"CustomNpcs\")");
            builder.AppendLine("clr.AddReference(\"OTAPI\")");
            builder.AppendLine("from Microsoft.Xna.Framework import *");
            builder.AppendLine("from Corruption.PlayerFunctions import *");
            builder.AppendLine("from Corruption.AreaFunctions import *");
            builder.AppendLine("from Corruption.ChestFunctions import *");
            builder.AppendLine("from Corruption.CommandFunctions import *");
            builder.AppendLine("from Corruption.EmoteFunctions import *");
            builder.AppendLine("from Corruption.ItemFunctions import *");
            builder.AppendLine("from Corruption.MiscFunctions import *");
            builder.AppendLine("from Corruption.NpcFunctions import *");
            builder.AppendLine("from Corruption.ProjectileFunctions import *");
            builder.AppendLine("from Corruption.SignFunctions import *");
            builder.AppendLine("from Corruption.TileFunctions import *");
            builder.AppendLine("from Corruption.TimeFunctions import *");
            builder.AppendLine("from CustomNpcs.NpcFunctions import *");
            builder.AppendLine(source.Contents);

            Scope.SetVariable("TSPlayer.All", TSPlayer.All);
            Scope.SetVariable("TSPlayer.Server", TSPlayer.Server);
            Scope.SetVariable("TSPlayers", TShock.Players);
            Scope.SetVariable("TSUtils", TShock.Utils);
            Scope.SetVariable("Main", Terraria.Main.instance);

            ICollection<string> searchPaths = Engine.GetSearchPaths();
            searchPaths.Add(TShock.SavePath + Path.DirectorySeparatorChar + "bin");
            searchPaths.Add(TShock.SavePath + Path.DirectorySeparatorChar + "ServerPlugins");
            Engine.SetSearchPaths(searchPaths);

            contents = builder.ToString();
        }

    }
}

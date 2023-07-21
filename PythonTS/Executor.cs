using IronPython;
using IronPython.Hosting;
using IronPython.Runtime.Operations;
using IronPython.Runtime.Types;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
//using Corruption;
using Microsoft.Xna.Framework;
using NuGet.Packaging.Licenses;
using PythonTS.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO.Streams;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
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
		
		public dynamic Execute(Script source, ScriptArguments[] args = null)
		{
			try
			{
				if(args != null)
				{
					foreach(ScriptArguments e in args)
					{
						Scope.SetVariable(e.VariableName, e.Value);
                    }
                }
                // add some nice globals
                var contents = "import clr" + Environment.NewLine +
                    "clr.AddReference(\"Corruption\")" + Environment.NewLine +
                    "from Corruption.PlayerFunctions import *" + Environment.NewLine +
                    "from Corruption.AreaFunctions import *" + Environment.NewLine +
                    "from Corruption.ChestFunctions import *" + Environment.NewLine +
                    "from Corruption.CommandFunctions import *" + Environment.NewLine +
                    "from Corruption.EmoteFunctions import *" + Environment.NewLine +
                    "from Corruption.ItemFunctions import *" + Environment.NewLine +
                    "from Corruption.MiscFunctions import *" + Environment.NewLine +
                    "from Corruption.NpcFunctions import *" + Environment.NewLine +
                    "from Corruption.ProjectileFunctions import *" + Environment.NewLine +
                    "from Corruption.SignFunctions import *" + Environment.NewLine +
                    "from Corruption.TileFunctions import *" + Environment.NewLine +
                    "from Corruption.TimeFunctions import *" + Environment.NewLine +
                    "" + Environment.NewLine + source.Contents;

                Scope.SetVariable("TSPlayer.All", TSPlayer.All);
                Scope.SetVariable("TSPlayer.Server", TSPlayer.Server);
                Scope.SetVariable("TSPlayers", TShock.Players);
                Scope.SetVariable("TSUtils", TShock.Utils);
                Scope.SetVariable("Main", Terraria.Main.instance);

/*                Scope.SetVariable("pf", DynamicHelpers.GetPythonTypeFromType(typeof(Corruption.PlayerFunctions)));
                Scope.SetVariable("af", DynamicHelpers.GetPythonTypeFromType(typeof(Corruption.AreaFunctions)));
                Scope.SetVariable("cf", DynamicHelpers.GetPythonTypeFromType(typeof(Corruption.ChestFunctions)));
                Scope.SetVariable("com", DynamicHelpers.GetPythonTypeFromType(typeof(Corruption.CommandFunctions)));
                Scope.SetVariable("ef", DynamicHelpers.GetPythonTypeFromType(typeof(Corruption.EmoteFunctions)));
                Scope.SetVariable("if", DynamicHelpers.GetPythonTypeFromType(typeof(Corruption.ItemFunctions)));
                Scope.SetVariable("mf", DynamicHelpers.GetPythonTypeFromType(typeof(Corruption.MiscFunctions)));
                Scope.SetVariable("nf", DynamicHelpers.GetPythonTypeFromType(typeof(Corruption.NpcFunctions)));
                Scope.SetVariable("nf", DynamicHelpers.GetPythonTypeFromType(typeof(Corruption.NpcFunctions)));
                Scope.SetVariable("proj", DynamicHelpers.GetPythonTypeFromType(typeof(Corruption.ProjectileFunctions)));
                Scope.SetVariable("sign", DynamicHelpers.GetPythonTypeFromType(typeof(Corruption.SignFunctions)));
                Scope.SetVariable("tf", DynamicHelpers.GetPythonTypeFromType(typeof(Corruption.TileFunctions)));
                Scope.SetVariable("time", DynamicHelpers.GetPythonTypeFromType(typeof(Corruption.TimeFunctions)));
*/


                ICollection<string> searchPaths = Engine.GetSearchPaths();
				searchPaths.Add(TShock.SavePath + Path.DirectorySeparatorChar + "bin");
				searchPaths.Add(TShock.SavePath + Path.DirectorySeparatorChar + "ServerPlugins");
                Engine.SetSearchPaths(searchPaths);
                var d = Engine.Execute(contents, Scope);

                Engine.Runtime.IO.RedirectToConsole();
                return d;
            }
			catch(Exception e)
			{
                var eo = Engine.GetService<ExceptionOperations>();
				Extensions.LogPrint(eo.FormatException(e), TraceLevel.Error);

            }
			return null;
        }

        public object GetVariable(string varName) => Scope.GetVariable(varName);

        public dynamic ExecuteMethod(Script source, string methodName, ScriptArguments[] args = null)
        {
            try
            {
                if (args != null)
                {
                    foreach (ScriptArguments e in args)
                    {
                        Scope.SetVariable(e.VariableName, e.Value);
                    }
                }
                ICollection<string> searchPaths = Engine.GetSearchPaths();
                searchPaths.Add(TShock.SavePath + Path.DirectorySeparatorChar + "bin");
                searchPaths.Add(TShock.SavePath + Path.DirectorySeparatorChar + "ServerPlugins");

                Engine.Execute(source.Contents, Scope);
                Engine.Runtime.IO.RedirectToConsole();
                return Operations.Invoke(Scope.GetVariable(methodName));

            }
            catch (Exception e)
            {
                var eo = Engine.GetService<ExceptionOperations>();
                Extensions.LogPrint(eo.FormatException(e), TraceLevel.Error);

            }
            return null;
        }

        public void Disable()
		{
			Engine.Runtime.Shutdown();
		}


	}
}

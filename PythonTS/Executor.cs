using IronPython;
using IronPython.Hosting;
using IronPython.Runtime.Operations;
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
                Scope.SetVariable("TSPlayer.All", TSPlayer.All);
                Scope.SetVariable("TSPlayer.Server", TSPlayer.Server);
                Scope.SetVariable("TSPlayers", TShock.Players);
                Scope.SetVariable("TSUtils", TShock.Utils);
                Scope.SetVariable("Main", Terraria.Main.instance);

                ICollection<string> searchPaths = Engine.GetSearchPaths();
				searchPaths.Add(TShock.SavePath + Path.DirectorySeparatorChar + "bin");
				searchPaths.Add(TShock.SavePath + Path.DirectorySeparatorChar + "ServerPlugins");
                var d = Engine.Execute(source.Contents, Scope);
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

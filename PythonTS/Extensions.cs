using Microsoft.Scripting.Hosting;
using System.Diagnostics;
using TerrariaApi.Server;

namespace PythonTS
{
    public static class Extensions
    {
        public static void LogPrint(string message, TraceLevel level = TraceLevel.Error) => ServerApi.LogWriter.PluginWriteLine(PythonScriptingPlugin.Instance, message, level);
    }
}

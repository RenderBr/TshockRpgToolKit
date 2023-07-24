using PythonTS.Models;

namespace PythonTS
{
    /// <summary>
    /// Lightweight wrapper of a filepath, its update time, and whether it exists at all. 
    /// </summary>
    public class Script : IScript, IEquatable<Script>
    {
        public static List<IScript> Modules => PythonScriptingPlugin.Modules;
        public Executor Executor { get; set; }
        public string Contents => File.ReadAllText(FilePath);
        public string Name => Path.GetFileName(FilePath);
        public string FilePath { get; set; }
        public bool Exists => File.Exists(FilePath);
        public bool Enabled { get; set; }
        public bool Enforced { get; set; }
        public DateTime LastUpdated => Exists ? File.GetLastWriteTime(FilePath) : DateTime.Now;

        public Script(string filePath = @"")
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("filePath may be null or whitespace.");

            FilePath = filePath;
            Enabled = false;
            Executor = new();
        }

        public Executor Execute(ScriptArguments[] args = null)
        {
            Executor.Execute(this, args);
            Enabled = true;
            return Executor;
        }

        public Executor ExecuteMethod(string methodName, ScriptArguments[] args = null)
        {
            Executor.ExecuteMethod(this, methodName, args);
            Enabled = true;
            return Executor;
        }

        public Executor Reload(ScriptArguments[] args = null)
        {
            Executor.Disable();
            Executor.Execute(this, args);
            return Executor;
        }

        public void Disable()
        {
            Executor.Disable();
            Enabled = false;
        }

        public override string ToString() => $"{FilePath}";

        public bool Equals(Script other) => FilePath == other.FilePath;

        public override bool Equals(object obj)
        {
            if (obj is Script)
                return Equals(obj as Script);
            else
                return false;
        }

        public static Script? Find(string name) => (Script)PythonScriptingPlugin.Modules.FirstOrDefault(x => x?.Name == name, null);

        public static Script FindDefault(string name)
        {
            var e = PythonScriptingPlugin.Modules.FirstOrDefault(x => x.Name.Contains($"{name}.py"));
            if (e == null)
            {
                Console.WriteLine($"{name}.py not found, creating...");

                if (!File.Exists(Path.Combine(PythonScriptingPlugin.DataDirectory, $"{name}.py")))
                    File.Create(Path.Combine(PythonScriptingPlugin.DataDirectory, $"{name}.py"));

                e = Find(name);
            }
            return (Script)e;
        }

        public static Script FindDefault(string name, bool enforced)
        {
            var e = Modules.FirstOrDefault(x => x.Name.Contains($"{name}.py"));
            if (e == null)
            {
                Console.WriteLine($"{name}.py not found, creating...");

                if (!File.Exists(Path.Combine(PythonScriptingPlugin.DataDirectory, $"{name}.py")))
                    File.Create(Path.Combine(PythonScriptingPlugin.DataDirectory, $"{name}.py"));

                e = Find(name);
            }
            e.Enforced = enforced;
            return (Script)e;
        }

        public static Script AddModule(string filepath)
        {
            if (File.Exists(filepath))
            {
                Script file = new(filepath);
                Modules.Add(file);
                return (Script)Find(file.Name);
            }
            return null;
        }

        public static Script AddModuleDefault(string filepath)
        {

            if (File.Exists(filepath))
            {
                Script file = new(filepath);
                Modules.Add(file);
                return (Script)FindModule(filepath);
            }
            else
            {
                var e = File.Create(filepath);
                Script file = new(filepath);
                Modules.Add(file);
                return (Script)FindModule(filepath);
            }

        }

        public static Script? FindModule(string filepath) => (Script)PythonScriptingPlugin.Modules.FirstOrDefault(x => x?.FilePath == filepath, null);

        public override int GetHashCode() => FilePath.GetHashCode();
    }
}

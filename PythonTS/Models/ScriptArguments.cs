namespace PythonTS.Models
{
    public class ScriptArguments
    {
        public string VariableName { get; set; }
        public object Value { get; set; }

        public ScriptArguments(string variableName, object value)
        {
            VariableName = variableName;
            Value = value;
        }
    }
}

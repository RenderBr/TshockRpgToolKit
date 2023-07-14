using Corruption.PluginSupport;
using CustomNpcs.Invasions;
using CustomNpcs.Npcs;
using CustomNpcs.Projectiles;
using Newtonsoft.Json;
using PythonTS;
using System.Reflection;


namespace CustomNpcs
{
    public interface IDefinition
    {
        public abstract string Identifier { get; set; } //only marking this as abstract so that derived classes override it, so we can apply json attributes. not sure if they'll work otherwise.
        public abstract string ScriptPath { get; set; }

        [JsonIgnore]
        public FilePosition FilePosition { get; set; }

        /// <summary>
        /// Runs validation for the definition, to check for and report on any error or warning conditions.
        /// </summary>
        /// <returns>A ValidationResult.</returns>
        public virtual ValidationResult Validate()
        {
            var result = new ValidationResult();
            return result;
        }

        /// <summary>
        /// Helper method that creates a string with the format FILENAME LINENUMBER,LINE {'DEFINITION.NAME'}.
        /// </summary>
        /// <param name="definition"></param>
        /// <returns></returns>
        internal static string CreateValidationSourceString(IDefinition definition)
        {
            var namePart = definition.Identifier != null ? $" '{definition.Identifier}'" : "";
            var result = $"{definition.FilePosition}{namePart}";

            return result;
        }

        /// <summary>
        /// Allows a derived definition class to control the linking process to an Assembly generated from a script. 
        /// </summary>
        /// <param name="assembly">Generated Assembly for ScriptPath.</param>
        public abstract void CreateModules();

        public virtual void OnDispose()
        {
        }
        public NpcDefinition NpcDefinition { get; }
        public ProjectileDefinition ProjDefinition { get; }
        public InvasionDefinition InvasionDefinition { get; }


    }
}

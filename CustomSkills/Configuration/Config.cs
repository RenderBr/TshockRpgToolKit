using Corruption.PluginSupport;
using Newtonsoft.Json;

namespace CustomSkills
{
    /// <summary>
    /// Custom Skills Configuration.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class Config : JsonConfig
    {
        /// <summary>
        /// Gets the Config instance.
        /// </summary>
        public static Config Instance { get; internal set; } = new Config();

        [JsonProperty(Order = 0)]
        public DatabaseConfig DatabaseConfig { get; set; } = new DatabaseConfig("sqlite", "Data Source=skills/db.sqlite");

        /// <summary>
        /// Gets or sets the file path to the json file containing skill definitions. Relative to 'skills' folder.
        /// </summary>
        [JsonProperty(Order = 1)]
        public string DefinitionFilepath { get; set; } = "skills.json";

        [JsonProperty(Order = 2)]
        public string ScriptPath { get; set; } = "scripts";

        /// <summary>
        /// Gets or sets whether to create a new Definition file, if DefinitionFilepath does not exist.
        /// </summary>
        [JsonProperty(Order = 3)]
        public bool AutoCreateDefinitionFile { get; set; } = true;

        public override ValidationResult Validate()
        {
            var result = new ValidationResult();

            if (string.IsNullOrWhiteSpace(DefinitionFilepath))
                result.Warnings.Add(new ValidationWarning("DefinitionFilepath is null or empty. No skills will be loaded."));

            return result;
        }
    }
}

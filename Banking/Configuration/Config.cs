using Corruption.PluginSupport;
using Newtonsoft.Json;

namespace Banking.Configuration
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Config : JsonConfig
    {
        public static Config Instance { get; internal set; }

        [JsonProperty(Order = 0)]
        public DatabaseConfig Database { get; set; } = new DatabaseConfig("sqlite", "Data Source=banking/db.sqlite");

        [JsonProperty(Order = 1)]
        public string ScriptPath { get; set; } = "scripts";

        [JsonProperty(Order = 2)]
        public string CurrencyPath { get; set; } = "currencies";

        [JsonProperty(Order = 3)]
        public VotingConfig Voting { get; set; } = new VotingConfig();

        public override ValidationResult Validate()
        {
            var result = new ValidationResult();

            if (Database == null)
                result.Errors.Add(new ValidationError($"Database config is null."));

            if (Voting == null)
                result.Warnings.Add(new ValidationWarning($"Voting config is null."));

            return result;
        }
    }
}

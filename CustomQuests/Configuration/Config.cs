using Corruption.PluginSupport;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CustomQuests.Configuration
{
    /// <summary>
    ///     Represents the configuration.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class Config : JsonConfig
    {
        [JsonProperty(Order = 0)]
        public DatabaseConfig Database { get; set; } = new DatabaseConfig("sqlite", "Data Source=quests/db.sqlite");

        [JsonProperty("DefaultQuests", Order = 1)]
        private readonly List<string> _defaultQuestNames = new();

        [JsonProperty("ScriptPath", Order = 2)]
        public string ScriptPath { get; set; } = "quests";
        /// <summary>
        ///     Gets a read-only view of the default quest names.
        /// </summary>
        public ReadOnlyCollection<string> DefaultQuestNames => _defaultQuestNames.AsReadOnly();

        /// <summary>
        ///     Gets the save period.
        /// </summary>
        [JsonProperty]
        public TimeSpan SavePeriod { get; private set; } = TimeSpan.FromMinutes(10.0);

        public override ValidationResult Validate()
        {
            var result = new ValidationResult();

            if (Database == null)
                result.Errors.Add(new ValidationError($"Database config is null."));

            if (_defaultQuestNames.Count < 1)
                result.Warnings.Add(new ValidationWarning("There are no DefaultQuestNames configured."));

            if (SavePeriod < TimeSpan.FromSeconds(20.0))
                result.Warnings.Add(new ValidationWarning("SavePeriod is less than 20 seconds. This may severely impact server performance."));

            return result;
        }
    }
}

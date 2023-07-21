using Corruption.PluginSupport;
using Newtonsoft.Json;
using PythonTS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;

namespace CustomNpcs.Npcs
{
    /// <summary>
    ///     Represents an NPC definition.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class NpcDefinition : Definition, IDisposable
    {
        //internal string originalName; //we need to capture the npc's original name before applying our custom name to it, so the exposed lua function
        //NameContains() works...

        /// <summary>
        ///     Gets the internal name.
        /// </summary>
        [JsonProperty("CustomID", Order = 0)]
        public override string Identifier { get; set; } = "NewNpcDefinition";

        /// <summary>
        ///     Gets the base type.
        /// </summary>
        [JsonProperty(Order = 1)]
        public int BaseType { get; set; }

        [JsonProperty(Order = 2)]
        public override string ScriptPath { get; set; }

        [JsonProperty("BaseOverride", Order = 3)]
        public BaseOverrideDefinition _baseOverride = new();

        [JsonProperty("Loot", Order = 4)]
        public LootDefinition _loot = new();

        [JsonProperty("Spawning", Order = 5)]
        public SpawningDefinition _spawning = new();

        /// <summary>
        ///     Gets the loot entries.
        /// </summary>
        public List<LootEntryDefinition> LootEntries => _loot.Entries;


        public Script Script { get; private set; }


        /// <summary>
        ///     Gets a value indicating whether the NPC should aggressively update due to unsynced changes with clients.
        /// </summary>
        public bool ShouldAggressivelyUpdate =>
            _baseOverride.AiStyle != null || _baseOverride.BuffImmunities != null ||
            _baseOverride.IsImmuneToLava != null || _baseOverride.HasNoCollision != null ||
            _baseOverride.HasNoGravity != null;

        /// <summary>
        ///     Gets a value indicating whether loot should be overriden.
        /// </summary>
        public bool ShouldOverrideLoot => _loot.IsOverride;

        /// <summary>
        ///     Gets a value indicating whether the NPC should spawn.
        /// </summary>
        public bool ShouldReplace => _spawning.ShouldReplace;

        /// <summary>
        ///     Gets a value indicating whether the NPC should spawn.
        /// </summary>
        public bool ShouldSpawn => _spawning.ShouldSpawn;

        /// <summary>
        ///     Gets an optional value that overrides the global spawnrate, if present.
        /// </summary>
        public int? SpawnRateOverride => _spawning.SpawnRateOverride;

        /// <summary>
        ///     Gets a value indicating whether the NPC should have kills tallied.
        /// </summary>
        public bool ShouldTallyKills => _loot.TallyKills;

        /// <summary>
        ///     Gets a value indicating whether the NPC should update on hit.
        /// </summary>
        public bool ShouldUpdateOnHit =>
            _baseOverride.Defense != null || _baseOverride.IsImmortal != null ||
            _baseOverride.KnockbackMultiplier != null;

        /// <summary>
        ///     Disposes the definition.
        /// </summary>
        public void Dispose()
        {
            Script = null;
        }

        public static List<NpcDefinition> LoadAll(string filepath)
        {
            var json = File.ReadAllText(filepath);
            var list = JsonConvert.DeserializeObject<List<NpcDefinition>>(json);
            return list;
        }

        /// <summary>
        ///     Applies the definition to the specified NPC.
        /// </summary>
        /// <param name="npc">The NPC, which must not be <c>null</c>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="npc" /> is <c>null</c>.</exception>
        public void ApplyTo(NPC npc)
        {
            if (npc == null)
            {
                throw new ArgumentNullException(nameof(npc));
            }

            // Set NPC to use four life bytes.
            Main.npcLifeBytes[BaseType] = 4;

            if (npc.netID != BaseType)
            {
                npc.SetDefaults(BaseType);
            }

            npc.aiStyle = _baseOverride.AiStyle ?? npc.aiStyle;
            if (_baseOverride.BuffImmunities != null)
            {
                for (var i = 0; i < Terraria.ID.BuffID.Count; ++i)
                {
                    npc.buffImmune[i] = false;
                }
                foreach (var i in _baseOverride.BuffImmunities)
                {
                    npc.buffImmune[i] = true;
                }
            }

            npc.defense = npc.defDefense = _baseOverride.Defense ?? npc.defense;
            npc.noGravity = _baseOverride.HasNoGravity ?? npc.noGravity;
            npc.noTileCollide = _baseOverride.HasNoCollision ?? npc.noTileCollide;
            npc.behindTiles = _baseOverride.BehindTiles ?? npc.behindTiles;
            npc.boss = _baseOverride.IsBoss ?? npc.boss;
            npc.immortal = _baseOverride.IsImmortal ?? npc.immortal;
            npc.lavaImmune = _baseOverride.IsImmuneToLava ?? npc.lavaImmune;
            npc.trapImmune = _baseOverride.IsTrapImmune ?? npc.trapImmune;
            npc.dontTakeDamageFromHostiles = _baseOverride.DontTakeDamageFromHostiles ?? npc.dontTakeDamageFromHostiles;
            npc.knockBackResist = _baseOverride.KnockbackMultiplier ?? npc.knockBackResist;
            // Don't set npc.lifeMax so that the correct life is always sent to clients.
            npc.life = _baseOverride.MaxHp ?? npc.life;
            npc._givenName = _baseOverride.Name ?? npc._givenName;
            npc.npcSlots = _baseOverride.NpcSlots ?? npc.npcSlots;
            npc.value = _baseOverride.Value ?? npc.value;

            //the following are not settable
            //npc.HasGivenName
            //npc.HasValidTarget
            //npc.HasPlayerTarget
            //npc.HasNPCTarget
        }

        public override void CreateModules()
        {
            if (string.IsNullOrWhiteSpace(ScriptPath))
                return;

            Console.WriteLine($"Loading NPC scripts for {Identifier}...");
            var path = $"{NpcManager.BasePath}/{Identifier}/";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            var prefix = $"{path}";

            Script = Script.AddModuleDefault(prefix + $"{Identifier}.py");

            return;
        }

        /*		public ValidationResult Validate()
                {
                    List<string> result = new();

                    if( string.IsNullOrWhiteSpace(NewName) )
                        result.Add($"{nameof(NewName)} is null or whitespace.");

                    if( int.TryParse(NewName, out _) )
                        result.Add($"{nameof(NewName)} cannot be a number.");

                    if( BaseType < -65 )
                        result.Add($"{nameof(BaseType)} is too small. Value must be greater than -65.");

                    if( BaseType >= Main.maxNPCs) //Was MaxNpcTypes (1.4)
                        result.Add($"{nameof(BaseType)} is too large. Value must be less than {Main.maxNPCs}.");

                    if( ScriptPath != null && !File.Exists(Path.Combine("npcs", ScriptPath)) )
                        result.Add($"{nameof(ScriptPath)} points to invalid script file, '{ScriptPath}'.");

                    //BaseOverride
                    if (_baseOverride != null)
                    {
                        var baseResult = _baseOverride.Validate();
                        baseResult.Source = result.Source;
                        result.Children.Add(baseResult);
                    }
                    else
                        result.Errors.Add(new ValidationError("BaseOverride is null."));

                    //Loot
                    if ( _loot != null )
                    {
                        var lootResult = _loot.Validate();
                        lootResult.Source = result.Source;
                        result.Children.Add(lootResult);
                    }
                    else
                        result.Errors.Add( new ValidationError("Loot is null."));

                    //Spawning
                    if( _spawning != null )
                    {
                        var spawnResult = _spawning.Validate();
                        spawnResult.Source = result.Source;
                        result.Children.Add(spawnResult);
                    }	
                    else
                        result.Errors.Add( new ValidationError("Spawning is null."));

                    //var source = "";

                    //if (!string.IsNullOrWhiteSpace(Name))
                    //	source =  $"{FilePath} {LineNumber},{LinePosition}: in '{Name}'";
                    //else
                    //	source = $"{FilePath} {LineNumber},{LinePosition}:";

                    //result.Children.Add(baseResult);
                    //baseResult.SetSources(source);

                    //result.SetSources(source);

                    return result;
                }
        */
        public static NpcDefinition Find(string name) => NpcManager.Definitions.FirstOrDefault(x => x.Identifier == name);

        [JsonObject(MemberSerialization.OptIn)]
        public sealed class BaseOverrideDefinition : IValidator
        {
            [JsonProperty]
            public int? AiStyle { get; set; }

            [JsonProperty]
            public int[] BuffImmunities { get; set; }

            [JsonProperty]
            public int? Defense { get; set; }

            [JsonProperty]
            public bool? HasNoCollision { get; set; }

            [JsonProperty]
            public bool? HasNoGravity { get; set; }

            [JsonProperty]
            public bool? IsBoss { get; set; }

            [JsonProperty]
            public bool? IsImmortal { get; set; }

            [JsonProperty]
            public bool? IsImmuneToLava { get; set; }

            [JsonProperty]
            public bool? IsTrapImmune { get; set; }

            [JsonProperty]
            public float? KnockbackMultiplier { get; set; }

            [JsonProperty]
            public int? MaxHp { get; set; }

            [JsonProperty]
            public string Name { get; set; }

            [JsonProperty]
            public float? NpcSlots { get; set; }

            [JsonProperty]
            public float? Value { get; set; }

            [JsonProperty]
            public bool? BehindTiles { get; set; }

            [JsonProperty]
            public bool? DontTakeDamageFromHostiles { get; set; }

            public ValidationResult Validate()
            {
                var result = new ValidationResult(this);

                if (BuffImmunities != null && BuffImmunities.Any(i => i <= 0 || i >= Terraria.ID.BuffID.Count))
                    result.Errors.Add(new ValidationError($"{nameof(BuffImmunities)} must contain valid buff types."));

                if (KnockbackMultiplier < 0)
                    result.Errors.Add(new ValidationError($"{nameof(KnockbackMultiplier)} must be non-negative."));

                if (MaxHp < 0)
                    result.Errors.Add(new ValidationError($"{nameof(MaxHp)} must be non-negative."));

                if (Value < 0)
                    result.Errors.Add(new ValidationError($"{nameof(Value)} must be non-negative."));

                return result;
            }
        }

        [JsonObject(MemberSerialization.OptIn)]
        public sealed class LootDefinition : IValidator
        {
            [JsonProperty(Order = 2)]
            public List<LootEntryDefinition> Entries { get; set; } = new List<LootEntryDefinition>();

            [JsonProperty(Order = 1)]
            public bool IsOverride { get; set; }

            [JsonProperty(Order = 0)]
            public bool TallyKills { get; set; }

            public ValidationResult Validate()
            {
                var result = new ValidationResult(this);

                if (Entries != null)
                {
                    foreach (var entry in Entries)
                    {
                        var res = entry.Validate();
                        result.Children.Add(res);
                    }
                }
                else
                {
                    result.Errors.Add(new ValidationError($"{nameof(Entries)} is null."));
                }

                return result;
            }
        }

        [JsonObject(MemberSerialization.OptIn)]
        public sealed class SpawningDefinition : IValidator
        {
            [JsonProperty(Order = 1)]
            public bool ShouldReplace { get; set; }

            [JsonProperty(Order = 0)]
            public bool ShouldSpawn { get; set; }

            [JsonProperty(Order = 2)]
            public int? SpawnRateOverride { get; set; }

            public ValidationResult Validate()
            {
                var result = new ValidationResult(this);
                return result;
            }
        }
    }
}

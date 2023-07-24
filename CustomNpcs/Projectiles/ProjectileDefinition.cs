using Corruption.PluginSupport;
using CustomNpcs.Npcs;
using Newtonsoft.Json;
using PythonTS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;

namespace CustomNpcs.Projectiles
{
    [JsonObject(MemberSerialization.OptIn)]
    public class ProjectileDefinition : Definition, IDisposable
    {

        [JsonProperty("CustomID", Order = 0)]
        public override string Identifier { get; set; } = "NewProjectileDefinition";

        [JsonProperty(Order = 1)]
        public override string ScriptPath { get; set; }

        [JsonProperty(Order = 2)]
        public int BaseType { get; set; }

        [JsonProperty("BaseOverride", Order = 3)]
        public BaseOverrideDefinition BaseOverride { get; set; } = new BaseOverrideDefinition();

        /// <summary>
        ///     Gets a function that is invoked when the projectile AI is spawned.
        /// </summary>
        public Script Script { get; private set; }

        public static List<ProjectileDefinition> LoadAll(string filepath)
        {
            var json = File.ReadAllText(filepath);
            var list = JsonConvert.DeserializeObject<List<ProjectileDefinition>>(json);
            return list;
        }

        public static ProjectileDefinition Find(string name) => ProjectileManager.Definitions.FirstOrDefault(x => x.Identifier == name);


        public void Dispose()
        {
            Script = null;
        }

        public void ApplyTo(Projectile projectile)
        {
            if (projectile == null)
            {
                throw new ArgumentNullException(nameof(projectile));
            }

            //projectile.type = 0;
            projectile.aiStyle = BaseOverride.AiStyle ?? projectile.aiStyle;
            if (BaseOverride.Ai != null)
            {
                //const int maxAis = 2;
                for (var i = 0; i < projectile.ai.Length; i++)
                {
                    if (i < BaseOverride.Ai.Length)
                    {
                        projectile.ai[i] = BaseOverride.Ai[i];
                    }
                }
            }

            //projectile.Name = BaseOverride.Name ?? projectile.Name; //this is readonly
            projectile.damage = BaseOverride.Damage ?? projectile.damage;
            projectile.knockBack = BaseOverride.KnockBack ?? projectile.knockBack;
            projectile.friendly = BaseOverride.Friendly ?? projectile.friendly;
            projectile.hostile = BaseOverride.Hostile ?? projectile.hostile;
            projectile.maxPenetrate = BaseOverride.MaxPenetrate ?? projectile.maxPenetrate;
            projectile.timeLeft = BaseOverride.TimeLeft ?? projectile.timeLeft;
            //projectile.width = BaseOverride.Width ?? projectile.width;
            //projectile.height = BaseOverride.Height ?? projectile.height;
            projectile.magic = BaseOverride.Magic ?? projectile.magic;
            projectile.light = BaseOverride.Light ?? projectile.light;
            //projectile.thrown = BaseOverride.Thrown ?? projectile.thrown;
            projectile.melee = BaseOverride.Melee ?? projectile.melee;
            projectile.coldDamage = BaseOverride.ColdDamage ?? projectile.coldDamage;
            projectile.tileCollide = BaseOverride.TileCollide ?? projectile.tileCollide;
            projectile.ignoreWater = BaseOverride.IgnoreWater ?? projectile.ignoreWater;
            //projectile.wet = baseOverride.Wet ?? projectile.wet;
            //projectile.bobber = BaseOverride.Bobber ?? projectile.bobber;
            //projectile.counterweight = BaseOverride.Counterweight ?? projectile.counterweight;
            //projectile.hide = false;
            //projectile.honeyWet = false;
            //projectile.miscText = "test";
            //projectile.noEnchantments = false;
            //projectile.rotation = 0f;
            //projectile.scale = 1.0f;
            //projectile.sentry = false;
            //projectile.spriteDirection = 1;
            //projectile.velocity = new Vector2(1, 1);
            //projectile.wet = false;
            //projectile.wetCount = 0;
            //projectile.melee = false;
            //projectile.oldVelocity;
            //projectile.velocity;
            //projectile.oldPosition;
            //projectile.position;
            //projectile.numHits = 0;
            //projectile.counterweight = false;
            //projectile.bobber = false;
            //projectile.alpha = 1;
            //projectile.direction = 0;
        }

        //internal bool LinkToScript(Assembly assembly)
        public override void CreateModules()
        {
            if (string.IsNullOrWhiteSpace(ScriptPath))
                return;


            Console.WriteLine($"Loading Projectile scripts for {Identifier}...");
            var path = $"{NpcManager.BasePath}/";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            var prefix = $"{path}";

            Script = Script.AddModuleDefault(prefix + $"{Identifier}.py");

            return;
        }

        /*		public override ValidationResult Validate()
                {
                    var result = new ValidationResult(DefinitionBase.CreateValidationSourceString(this));

                    if (string.IsNullOrWhiteSpace(Name))
                        result.Errors.Add(new ValidationError($"{nameof(Name)} is null or whitespace."));

                    if (int.TryParse(Name, out _))
                        result.Errors.Add(new ValidationError($"{nameof(Name)} cannot be a number."));

                    //if (BaseType < -65)
                    //{
                    //	throw new FormatException($"{nameof(BaseType)} is too small.");
                    //}
                    if (BaseType >= Main.maxProjectiles)
                        result.Errors.Add(new ValidationError($"{nameof(BaseType)} is greater than {Main.maxProjectiles}.")); //was max projectile types

                    if (ScriptPath != null && !File.Exists(Path.Combine("npcs", ScriptPath)))
                        result.Errors.Add(new ValidationError($"{nameof(ScriptPath)} points to invalid script file, '{ScriptPath}'."));

                    if (BaseOverride != null)
                    {
                        var baseResult = BaseOverride.Validate();
                        baseResult.Source = result.Source;
                        result.Children.Add(baseResult);
                    }
                    else
                        result.Errors.Add( new ValidationError($"{nameof(BaseOverride)} is null."));

                    return result;
                }*/

        [JsonObject(MemberSerialization.OptIn)]
        public sealed class BaseOverrideDefinition : IValidator
        {
            [JsonProperty]
            public int? AiStyle { get; set; }

            [JsonProperty]
            public float[] Ai { get; set; }

            [JsonProperty]
            public int? Damage { get; set; }

            [JsonProperty]
            public int? KnockBack { get; set; }

            [JsonProperty]
            public bool? Friendly { get; set; }

            [JsonProperty]
            public bool? Hostile { get; set; }

            [JsonProperty]
            public int? MaxPenetrate { get; set; }

            [JsonProperty]
            public int? TimeLeft { get; set; }

            //[JsonProperty]
            //public int? Width { get; set;}

            //[JsonProperty]
            //public int? Height { get; set;}

            [JsonProperty]
            public bool? Magic { get; set; }

            [JsonProperty]
            public float? Light { get; set; }

            //[JsonProperty]
            //public bool? Thrown { get; set; }

            [JsonProperty]
            public bool? Melee { get; set; }

            [JsonProperty]
            public bool? ColdDamage { get; set; }

            [JsonProperty]
            public bool? TileCollide { get; set; }

            [JsonProperty]
            public bool? IgnoreWater { get; set; }

            public ValidationResult Validate()
            {
                var result = new ValidationResult();
                return result;
            }

            /* 	[JsonProperty]
				public bool? Wet { get; set; } */

            //[JsonProperty]
            //public bool? Bobber { get; set; }

            //[JsonProperty]
            //public bool? Counterweight { get; set; }
        }
    }
}



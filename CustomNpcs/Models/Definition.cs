using Corruption.PluginSupport;
using CustomNpcs.Invasions;
using CustomNpcs.Npcs;
using CustomNpcs.Projectiles;
using Newtonsoft.Json;
using PythonTS;
using System.Collections.Generic;

namespace CustomNpcs
{
    public abstract class Definition : IDefinition
    {
        public abstract string Identifier { get; set; } //only marking this as abstract so that derived classes override it, so we can apply json attributes. not sure if they'll work otherwise.
        public abstract string ScriptPath { get; set; }
        public List<Script> LinkedScripts { get; set; } = new();

        public NpcDefinition NpcDefinition => (NpcDefinition)this;

        public ProjectileDefinition ProjDefinition => (ProjectileDefinition)this;

        public InvasionDefinition InvasionDefinition => (InvasionDefinition)this;

        [JsonIgnore]
        public FilePosition FilePosition { get; set; }

        public abstract void CreateModules();

    }

    public class CustomDefinition : Definition
    {

        public override string Identifier { get; set; }
        public override string ScriptPath { get; set; }

        public override void CreateModules()
        {
        }
    }

}

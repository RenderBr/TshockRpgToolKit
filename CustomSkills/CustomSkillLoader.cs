using Corruption.PluginSupport;
using Newtonsoft.Json;
using PythonTS;
using System.Collections.Generic;
using System.IO;

namespace CustomSkills
{
    /// <summary>
    /// Loads and stores CustomSkillDefinitions.
    /// </summary>
    internal class CustomSkillDefinitionLoader
    {
        internal const string DefaultCategoryName = "uncategorized";

        internal Dictionary<string, CustomSkillCategory> Categories { get; private set; }
        internal Dictionary<string, CustomSkillDefinition> TriggeredDefinitions { get; private set; }
        //internal Dictionary<string, CustomSkillDefinition> TriggerWordsToDefinitions { get; private set; }

        internal CustomSkillDefinitionLoader()
        {
            Categories = new Dictionary<string, CustomSkillCategory>()
            {
                { DefaultCategoryName, new CustomSkillCategory(DefaultCategoryName) }
            };
        }

        private CustomSkillDefinitionLoader(List<CustomSkillCategory> customSkillCategories) : this()
        {
            foreach (var srcCategory in customSkillCategories)
            {
                if (Categories.TryGetValue(srcCategory.Name, out var dstCategory))
                {
                    //copy to existing
                    foreach (var kvp in srcCategory)
                        dstCategory.Add(kvp.Key, kvp.Value);
                }
                else
                {
                    //just add incoming category
                    Categories.Add(srcCategory.Name, srcCategory);
                }
            }

            //TriggerWordsToDefinitions = MapTriggerWords();
            TriggeredDefinitions = MapTriggeredDefinitions();
            LoadScripts();
        }

        private Dictionary<string, CustomSkillDefinition> MapTriggeredDefinitions()
        {
            var result = new Dictionary<string, CustomSkillDefinition>();

            foreach (var cat in Categories.Values)
            {
                foreach (var skill in cat.Values)
                {
                    if (skill.HasTriggerWords)
                    {
                        result[skill.Name] = skill;
                    }
                }
            }

            return result;
        }

        //private Dictionary<string,CustomSkillDefinition> MapTriggerWords()
        //{
        //	var result = new Dictionary<string, CustomSkillDefinition>();

        //	foreach(var cat in Categories.Values)
        //	{
        //		foreach(var skill in cat.Values)
        //		{
        //			if(skill.HasTriggerWords)
        //			{
        //				foreach(var word in skill.TriggerWords)
        //				{
        //					if(!string.IsNullOrWhiteSpace(word))
        //						result[word] = skill;
        //				}
        //			}
        //		}
        //	}

        //	return result;
        //}

        internal CustomSkillCategory TryGetCategory(string categoryName = null)
        {
            categoryName = string.IsNullOrWhiteSpace(categoryName) ? DefaultCategoryName : categoryName;

            Categories.TryGetValue(categoryName, out var category);

            return category;
        }

        internal CustomSkillDefinition TryGetDefinition(string skillName, string categoryName = null)
        {
            var category = TryGetCategory(categoryName);

            //find skill def
            category.TryGetValue(skillName, out var skillDefinition);

            return skillDefinition;
        }

        internal static CustomSkillDefinitionLoader Load(string filePath, bool createIfNeeded = true)
        {
            DefinitionFile<List<CustomSkillCategory>> fileDef = null;

            if (!File.Exists(filePath))
            {
                if (createIfNeeded)
                {
                    fileDef = CreateDefaultDataDefinition();

                    var json = JsonConvert.SerializeObject(fileDef, Formatting.Indented);
                    File.WriteAllText(filePath, json);
                }
            }
            else
            {
                var json = File.ReadAllText(filePath);
                fileDef = JsonConvert.DeserializeObject<DefinitionFile<List<CustomSkillCategory>>>(json);
            }

            //convert file def into a customskillmanager...
            var mgr = new CustomSkillDefinitionLoader(fileDef.Data);

            return mgr;
        }

        private HashSet<string> GetScriptPaths()
        {
            var result = new HashSet<string>();

            foreach (var cat in Categories.Values)
            {
                foreach (var skill in cat.Values)
                {
                    foreach (var level in skill.Levels)
                    {
                        if (!string.IsNullOrWhiteSpace(level.ScriptPath))
                        {
                            result.Add(CustomSkillsPlugin.Instance.PluginRelativePath(level.ScriptPath));
                        }
                    }
                }
            }

            return result;
        }

        private void LoadScripts()
        {
            var scriptPaths = GetScriptPaths();

            foreach (var cat in Categories.Values)
            {
                foreach (var skill in cat.Values)
                {
                    foreach (var level in skill.Levels)
                    {
                        var path = CustomSkillsPlugin.ScriptsDirectory + "/" + skill.Name + "/";
                        if (!Directory.Exists(path))
                            Directory.CreateDirectory(path);

                        var prefix = $"{path}{skill.Name}_";
                        if (level.Script == null)
                            level.Script = Script.AddModuleDefault(prefix + $"Level{skill.Levels.IndexOf(level)}.py");

                    }
                }
            }
        }

        private static DefinitionFile<List<CustomSkillCategory>> CreateDefaultDataDefinition()
        {
            var fileDef = new DefinitionFile<List<CustomSkillCategory>>()
            {
                Version = 0.1f,
                Metadata = new Dictionary<string, object>()
                {
                    { "Authors", "Autogenerated by CustomSkills plugin." },
                    { "Remarks", "This file format is under active development. Do not rely on any properties being available in future versions." }
                },
                Data = new List<CustomSkillCategory>()
                {
                    new CustomSkillCategory(DefaultCategoryName)
                    {
                        { "TestSkill", new CustomSkillDefinition()
                            {
                                Name = "TestSkill",
                                Description = "This skill is just a placeholder!",
                                NotifyUserOnCooldown = true,
                                Levels = new List<CustomSkillLevelDefinition>()
                                {
                                    new CustomSkillLevelDefinition()
                                    {
                                        ScriptPath = "script.py",
                                        CanInterrupt = true,
                                        CanCasterMove = true,
                                        UsesToLevelUp = 0
                                    }
                                }
                            }
                        }
                    }
                }
            };

            return fileDef;
        }
    }
}

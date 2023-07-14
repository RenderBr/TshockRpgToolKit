using Corruption.PluginSupport;
using Newtonsoft.Json;
using PythonTS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomQuests.Quests
{
	public class QuestLoader : IEnumerable<QuestInfo>
	{
		List<QuestInfo> questInfoList;//maintains ordering, enables correct serialization/deserialization to json...
		Dictionary<string, QuestInfo> questInfos;

		public HashSet<string> InvalidQuests { get; private set; } //records quests that are broken in someway, and should not be ran/listed.

		public int Count => questInfoList.Count;
		public QuestInfo this[int index] => questInfoList[index];
		public QuestInfo this[string name]
		{
			get
			{
				questInfos.TryGetValue(name, out var qi);
				return qi;
			}
		}
		
		public QuestLoader()
		{
			questInfos = new Dictionary<string, QuestInfo>();
			InvalidQuests = new HashSet<string>();
		}

		public void Clear()
		{
			InvalidQuests.Clear();
			questInfos.Clear();
		}

		public void LoadQuests(string fileName)
		{
			CustomQuestsPlugin.Instance.LogPrint($"Loading quest info from {fileName}...", TraceLevel.Info);

			if(!Directory.Exists(CustomQuestsPlugin.QuestsPath))
                Directory.CreateDirectory(CustomQuestsPlugin.QuestsPath);

            try
			{
				if( File.Exists(fileName) )
				{
					var json = File.ReadAllText(fileName);
					questInfoList = JsonConvert.DeserializeObject<List<QuestInfo>>(json);
				}
				else
				{
					questInfoList = new List<QuestInfo>();
					var json = JsonConvert.SerializeObject(questInfoList);
					File.WriteAllText(fileName, json);
				}
			}
			catch(Exception ex)
			{
				CustomQuestsPlugin.Instance.LogPrint(ex.ToString());
				questInfoList = new List<QuestInfo>();
			}
			
			Clear();

			//validation
			var rootResult = new ValidationResult(fileName);

			foreach (var qi in questInfoList)
			{
				var result = qi.Validate();
				rootResult.Children.Add(result);

                qi.LinkedScript = Script.AddModuleDefault(Path.Combine($"{CustomQuestsPlugin.QuestsPath}/", qi.ScriptPath ?? $"{qi.Name}.py"));

                //only add quest info if its valid
                if (result.Errors.Count==0)
					questInfos.Add(qi.Name, qi);
				
			}

			CustomQuestsPlugin.Instance.LogPrint(rootResult);
			
			Debug.Print("Found the following quest infos:");
			foreach( var qi in questInfoList )
				Debug.Print($"Quest: {qi.Name},  {qi.FriendlyName} - {qi.Description}");
		}
		
		public Quest CreateInstance(QuestInfo questInfo, Party party)
		{
			if( questInfo == null )
				throw new ArgumentNullException(nameof(questInfo));

			if( party == null )
				throw new ArgumentNullException(nameof(party));

			//check party
			//...

			//check quest
			//...

			//check ...
			//...

			if( !string.IsNullOrWhiteSpace(questInfo.ScriptPath) )
			{
				var scriptPath = Path.Combine("quests", questInfo.ScriptPath ?? $"{questInfo.Name}.py");
                var scriptAssembly = questInfoList.FirstOrDefault(qi => qi.Name == questInfo.Name)?.LinkedScript;

                if ( scriptAssembly != null )
				{

					Quest quest = new();
                    quest.Script = scriptAssembly;

                    //set these before, or various quest specific functions will get null ref's from within the quest.
                    quest.QuestInfo = questInfo;
					quest.party = party;

					return quest;
				}
				else
				{
					CustomQuestsPlugin.Instance.LogPrint($"Cannot load quest '{questInfo.Name}', no assembly exists. ( Did compilation fail? ) ", TraceLevel.Error);
					CustomQuestsPlugin.Instance.QuestLoader.InvalidQuests.Add(questInfo.Name);
				}
			}

			return null;
		}

		public bool Contains(string questName)
		{
			return questInfos.ContainsKey(questName);
		}
		
		public bool IsQuestInvalid(string questName)
		{
			return InvalidQuests.Contains(questName);
		}
		
		public IEnumerator<QuestInfo> GetEnumerator()
		{
			return questInfoList.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return questInfoList.GetEnumerator();
		}
	}
}

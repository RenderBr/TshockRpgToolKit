using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Banking;
using Corruption.PluginSupport;
using Leveling.Levels;
using Leveling.Sessions;
using Newtonsoft.Json;
using PythonTS;
using TerrariaApi.Server;
using TShockAPI;

namespace Leveling.Classes
{
    /// <summary>
    ///     Represents a class definition.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class ClassDefinition : IValidator
    {
		internal FilePosition FilePosition { get; set; } = new FilePosition();

		/// <summary>
		///     Gets the name.
		/// </summary>
		[JsonProperty("Name", Order = 0)]
		public string Name { get; internal set; } = "ranger";

		/// <summary>
		///     Gets the display name.
		/// </summary>
		[JsonProperty("DisplayName", Order = 1)]
		public string DisplayName { get; internal set; }

		/// <summary>
		///     Gets the list of prerequisite levels.
		/// </summary>
		[JsonProperty("PrerequisiteLevels", Order = 3)]
		public IList<string> PrerequisiteLevelNames { get; internal set; } = new List<string>();

		/// <summary>
		///     Gets the list of prerequisite permissions.
		/// </summary>
		[JsonProperty(Order = 4)]
		public IList<string> PrerequisitePermissions { get; internal set; } = new List<string>();
		
		/// <summary>
		///		Gets or sets the Currency cost to enter this class.
		/// </summary>
		[JsonProperty(Order = 6, PropertyName = "Cost")]
		public string CostString { get; set; } = "1s";

		/// <summary>
		/// The parsed equivalent of CostString.
		/// </summary>
		public decimal Cost { get; set; }

		/// <summary>
		/// The Currency used to to enter this class. This is determined from CostString.
		/// </summary>
		public CurrencyDefinition CostCurrency { get; set; }

		/// <summary>
		///     Gets a value indicating whether to allow switching the class after mastery.
		/// </summary>
		[JsonProperty(Order = 7)]
		public bool AllowSwitching { get; internal set; } = true;

		/// <summary>
		///     Gets a value indicating whether to allow switching the class before mastery.
		/// </summary>
		[JsonProperty(Order = 8)]
		public bool AllowSwitchingBeforeMastery { get; internal set; }

		/// <summary>
		///     Gets the EXP multiplier override.
		/// </summary>
		[JsonProperty(Order = 9)]
		public double? ExpMultiplierOverride { get; internal set; }

		/// <summary>
		///     Gets the death penalty multiplier override.
		/// </summary>
		[JsonProperty(Order = 10)]
		public double? DeathPenaltyMultiplierOverride { get; internal set; }

		/// <summary>
		///		Gets the list of commands to execute on first change to a class.
		/// </summary>
		[JsonProperty("CommandsOnClassChangeOnce", Order = 11)]
		public IList<string> CommandsOnClassChangeOnce { get; internal set; } = new List<string>();
		
		/// <summary>
		///     Gets the list of level definitions.
		/// </summary>
		[JsonProperty("Levels", Order = 12)]
		public IList<LevelDefinition> LevelDefinitions { get; internal set; } = new List<LevelDefinition>()
		{
			new LevelDefinition()
			{
                Name = "Level 1",
                DisplayName = "Level 1",
                ExpRequired = 10,
            }
		};

		/// <summary>
		/// Gets or sets the Currency used for Leveling purposes.
		/// </summary>
		public CurrencyDefinition LevelingCurrency { get; set; }

		/// <summary>
		///     Gets the mapping of NPC names to EXP rewards.
		/// </summary>
		[JsonProperty("NpcToExpReward", Order = 13)]
		public Dictionary<string, string> NpcNameToExpReward = new Dictionary<string, string>();

		/// <summary>
		///		Gets a mapping of NPC names to preparsed EXP values.
		/// </summary>
		internal Dictionary<string, decimal> ParsedNpcNameToExpValues { get; set; } = new Dictionary<string, decimal>();
				
		//--- new stuff
		public string DisplayInfo { get; internal set; }

		//not sure how these should/would work
		//public Action<object> OnMaximumCurrency;
		//public Action<object> OnNegativeCurrency;
		
		//player, currentclass, currentLevelIndex
		public Script OnLevelUp;

		//player, currentclass, currentLevelIndex
		public Script OnLevelDown;

		//player, currentclass, oldclass
		public Script OnClassChange;
		
		//player, currentclass
		public Script OnClassMastered;
		
		public override string ToString()
		{
			return $"[ClassDefinition '{Name}']";
		}

		public static List<ClassDefinition> Load(string directoryPath)
		{
			LevelingPlugin.Instance.LogPrint("Loading Classes...");

			var results = new List<ClassDefinition>();
			var filesAndDefs = new List<Tuple<string, ClassDefinition>>();//needed by LoadScripts.
			var classFiles = Directory.EnumerateFiles(directoryPath, "*.json", SearchOption.AllDirectories);
			
			foreach( var file in classFiles )
			{
				try
				{
					var json = File.ReadAllText(file);
					var def = JsonConvert.DeserializeObject<ClassDefinition>(json);
					var result = def.Validate();
					result.Source = file;

					def.FilePosition = new FilePosition(file);

					LevelingPlugin.Instance.LogPrint(result);

					if(result.Errors.Count<1)
					{
						if (def.Initialize())
						{
							results.Add(def);

							var fd = new Tuple<string, ClassDefinition>(file, def);
							filesAndDefs.Add(fd);
						}
					}
				}
				catch(Exception ex)
				{
					LevelingPlugin.Instance.LogPrint($"{file}", TraceLevel.Error);
					LevelingPlugin.Instance.LogPrint(ex.ToString(), TraceLevel.Error);
				}
			}
			
			//additional checks...

			//if default class file does not exist, we're in an error state
			if( results.Select(cd => cd.Name)
						.FirstOrDefault(n => n == Config.Instance.DefaultClassName) == null )
			{
				LevelingPlugin.Instance.LogPrint($"A class matching the DefaultClassName '{Config.Instance.DefaultClassName}' was not found. ", TraceLevel.Error);
			}
			
			return results;
		}
		
		public bool Initialize()
		{
			ValidateAndFix();
			ResolveClassCurrency();
			ResolveLevelingCurrency();
			PreParseRewardValues();
			GenerateScripts();

            return true;//in future, we can check whether the class is actually valid, or needs to be rejected.
		}

		/// <summary>
		/// Class specific filename formatting for errors, warnings and exceptions.
		/// </summary>
		/// <returns></returns>
		private string GetFileStringForError()
		{
			var result = "";

			if (!string.IsNullOrWhiteSpace(FilePosition.FilePath))
				result = $"{FilePosition.FilePath}: ";

			return result;
		}

		/// <summary>
		/// Attempts to determine the Currency's and Values for the class cost.
		/// </summary>
		private bool ResolveClassCurrency()
		{
			var currencyMgr = BankingPlugin.Instance.Bank.CurrencyManager;

			if( currencyMgr.TryFindCurrencyFromString(CostString, out var costCurrency) )
			{
				if(costCurrency.GetCurrencyConverter().TryParse(CostString, out var costValue))
				{
					Cost = costValue;
					CostCurrency = costCurrency;
					return true;
				}
			}

			var fileInfo = GetFileStringForError();
			
			LevelingPlugin.Instance.LogPrint($"{fileInfo}Could not determine currency or value for switching to class '{Name}'.", TraceLevel.Warning);//not an error, in the strict sense of the word.
			LevelingPlugin.Instance.LogPrint($"{fileInfo}Ensure that the 'Cost' property has a properly formatted currency string set.", TraceLevel.Info);

			return false;
		}

		/// <summary>
		/// Attempts to determine the Currency's and Values for the levels within the class.
		/// </summary>
		private bool ResolveLevelingCurrency()
		{
			//determine leveling currency
			var currencyMgr = BankingPlugin.Instance.Bank.CurrencyManager;
			var fileInfo = GetFileStringForError();
			
			foreach( var lvl in LevelDefinitions )
			{
				if( string.IsNullOrWhiteSpace(lvl.Currency.Type) )
					continue;//no currency value was set

				if( currencyMgr.GetCurrencyByName(lvl.Currency.Type, out var lvlCurrency) )
				{
					if( lvlCurrency.GetCurrencyConverter().TryParse(lvl.Currency.Cost, out var requiredValue) )
					{
						if( LevelingCurrency == null )
						{
							//no leveling currency has been set yet...
							LevelingCurrency = lvlCurrency;
							lvl.ExpRequired = (long)requiredValue;
						}
						else
						{
							//a leveling currency has been set, so we should flag any currency's that do not match the set currency.
							if(lvlCurrency==LevelingCurrency)
								lvl.ExpRequired = (long)requiredValue;
							else
							{
								LevelingPlugin.Instance.LogPrint($"{fileInfo}Currency '{lvlCurrency.InternalName}' used in 'CurrencyRequired' in level '{lvl.Name}' in class '{Name}' does not match previously set currency '{LevelingCurrency.InternalName}'. Falling back to 'ExpLevel'.", TraceLevel.Error);
							}
						}
					}
					else
					{
						LevelingPlugin.Instance.LogPrint($"{fileInfo}Couldn't parse 'CurrencyRequired' in level '{lvl.Name}' in class '{Name}'. Using 'ExpLevel' instead.", TraceLevel.Error);
					}
				}
				else
				{
					LevelingPlugin.Instance.LogPrint($"{fileInfo}Couldn't determine currency type in level '{lvl.Name}' in class '{Name}'. Using 'ExpLevel' instead.", TraceLevel.Error);
					LevelingPlugin.Instance.LogPrint($"{fileInfo}Ensure that the 'CurrencyRequired' property has a properly formatted currency string set, or that 'ExpLevel' is set instead.", TraceLevel.Info);
				}
			}

			if(LevelingCurrency==null)
			{
				LevelingPlugin.Instance.LogPrint($"{fileInfo}Could not determine a LevelingCurrency for class '{Name}'. Members of this class will be unable to change levels.", TraceLevel.Error);
				LevelingPlugin.Instance.LogPrint($"{fileInfo}Ensure that at least one Level has a 'CurrencyRequired' property with a properly formatted currency string set.", TraceLevel.Info);
			}

			return true;//just pass it for now, future iterations can handle this better.
		}

		private void PreParseRewardValues()
		{
			ParsedNpcNameToExpValues.Clear();

			foreach( var kvp in NpcNameToExpReward )
			{
				decimal unitValue;

				//if( currency.GetCurrencyConverter().TryParse(kvp.Value, out unitValue) )
				//{
				//	ParsedNpcNameToExpValues.Add(kvp.Key, unitValue);
				//}
				//else
				//{
				//	Debug.Print($"Failed to parse Npc reward value '{kvp.Key}' for class '{Name}'. Setting value to 0.");
				//}
			}
		}
		
		/// <summary>
		/// Checks that the ClassDefinition is valid, and it not, attempts to bring it into a valid state.
		/// </summary>
		private void ValidateAndFix()
		{
			var levelNames = new HashSet<string>();
			var duplicateLevelDefinitions = new List<LevelDefinition>();
			var fileInfo = GetFileStringForError();

			foreach ( var def in LevelDefinitions )
			{
				if(!levelNames.Add(def.Name))
				{
					LevelingPlugin.Instance.LogPrint($"{fileInfo}Class '{Name}' already has a Level named '{def.Name}'. Disabling duplicate level.", TraceLevel.Error);
					duplicateLevelDefinitions.Add(def);
				}
			}

			foreach(var dupDef in duplicateLevelDefinitions)
				LevelDefinitions.Remove(dupDef);
		}

		private bool GenerateScripts()
		{

			//try to link to callbacks...

			//...but these are disabled since I have no idea how these should work
			//def.OnMaximumCurrency = linker["OnMaximumCurrency"]?.TryCreateDelegate<Action<object>>();
			//def.OnNegativeCurrency = linker["OnNegativeCurrency"]?.TryCreateDelegate<Action<object>>();
			if(!Directory.Exists(LevelingPlugin.ClassPath))
                Directory.CreateDirectory(LevelingPlugin.ClassPath);

            var path = Path.Combine(LevelingPlugin.ClassPath, Name);
			if (!Directory.Exists(path))
				Directory.CreateDirectory(path);

            var prefix = $"{path}/{Name}_";
			OnLevelUp = Script.AddModuleDefault(prefix+"OnLevelUp.py");
            OnLevelDown = Script.AddModuleDefault(prefix + "OnLevelDown.py");
            OnClassChange = Script.AddModuleDefault(prefix + "OnClassChange.py");
            OnClassMastered = Script.AddModuleDefault(prefix + "OnClassMastered.py");

            return true;
		}

		public ValidationResult Validate()
		{
			var result = new ValidationResult(); //dont set source, so that errors and warnings fallback to Parent Source after we call Validate(). 

			if (string.IsNullOrWhiteSpace(Name))
				result.Errors.Add(new ValidationError($"{nameof(Name)} is null or whitespace."));

			if (string.IsNullOrWhiteSpace(DisplayName))
				result.Errors.Add(new ValidationError($"{nameof(DisplayName)} is null or whitespace."));

			if ((LevelDefinitions != null && LevelDefinitions.Count>0))
			{
				var i = 0;

				foreach(var levelDef in LevelDefinitions)
				{
					var levelResult = levelDef.Validate();
					levelResult.Source = $"Level[{i++}] {levelResult.Source?.ToString()}";//insert level index, in case name strings arent set.
					result.Children.Add(levelResult);
				}
			}
			else
				result.Errors.Add(new ValidationError($"{nameof(LevelDefinitions)} is null or empty."));

			return result;
		}
	}
}

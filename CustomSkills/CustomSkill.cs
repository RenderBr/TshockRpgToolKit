﻿using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;

namespace CustomSkills
{
	/// <summary>
	/// Represents a CustomSkill, "in action".
	/// </summary>
	internal class CustomSkill
	{
		internal TSPlayer Player { get; set; }
		internal CustomSkillDefinition Definition { get; set; }
		internal int LevelIndex { get; set; }
		internal CustomSkillLevelDefinition LevelDefinition => Definition.Levels[LevelIndex];
		internal SkillPhase Phase { get; set; }
		DateTime ChargeStartTime;
		DateTime CooldownStartTime;
		
		internal CustomSkill()
		{
		}

		internal CustomSkill(TSPlayer player, CustomSkillDefinition skillDefinition, int levelIndex)
		{
			Player = player;
			Definition = skillDefinition;
			LevelIndex = levelIndex;

			if(levelIndex < 0 || levelIndex >= skillDefinition.Levels.Count)
				throw new ArgumentOutOfRangeException($"{nameof(levelIndex)}");
		}
		
		internal void Update()
		{
			if(!Player.ConnectionAlive)
			{
				Phase = SkillPhase.Cancelled;
				return;
			}

			switch(Phase)
			{
				case SkillPhase.Casting:
					RunOnCast();
					break;

				case SkillPhase.Charging:
					RunOnCharging();
					break;

				case SkillPhase.Firing:
					RunOnFiring();
					break;

				case SkillPhase.Cooldown:
					RunCooldown();
					break;
			}
		}

		void RunOnCast()
		{
			try
			{
				var levelDef = LevelDefinition;

				Debug.Print($"Casting {Definition.Name}.");

				levelDef.OnCast?.Invoke(Player);
				//only fires one time

				ChargeStartTime = DateTime.Now;
				Phase = SkillPhase.Charging;
			}
			catch(Exception ex)
			{
				Phase = SkillPhase.Failed;
				throw ex;
			}
		}

		void RunOnCharging()
		{
			try
			{
				var levelDef = LevelDefinition;

				var elapsed = DateTime.Now - ChargeStartTime;
				var completed = (float)(elapsed.TotalMilliseconds / levelDef.ChargingDuration.TotalMilliseconds) * 100.0f;
				completed = Math.Min(completed, 100.0f);
				
				Debug.Print($"Charging {Definition.Name}. ({completed}%)");

				levelDef.OnCharge?.Invoke(Player,completed);

				//can fire continuously...
				//if(DateTime.Now - ChargeStartTime >= levelDef.ChargingDuration)
				//	Phase = SkillPhase.Firing;
				if(completed >= 100.0f)
					Phase = SkillPhase.Firing;
			}
			catch(Exception ex)
			{
				Phase = SkillPhase.Failed;
				throw ex;
			}
		}

		void RunOnFiring()
		{
			try
			{
				var levelDef = LevelDefinition;

				Debug.Print($"Firing {Definition.Name}.");

				levelDef.OnFire?.Invoke(Player);
				//only fires once, but should spark something that can continue for some time afterwards.
				//check if we moved up a level..

				CooldownStartTime = DateTime.Now;
				Phase = SkillPhase.Cooldown;
			}
			catch(Exception ex)
			{
				Phase = SkillPhase.Failed;
				throw ex;
			}
		}

		void RunCooldown()
		{
			try
			{
				var levelDef = LevelDefinition;

				Debug.Print($"Cooling down {Definition.Name}.");

				if(DateTime.Now - CooldownStartTime >= levelDef.CastingCooldown)
					Phase = SkillPhase.Completed;
			}
			catch(Exception ex)
			{
				Phase = SkillPhase.Failed;
				throw ex;
			}
		}
	}
}

# TShockRpgToolkit

Welcome to TShockRpgToolkit! This toolkit provides a brand new scripting implementation called PythonTS, which aims to replace BooTS. With the utilization of IronPython, it offers seamless integration with user-defined scripts and the plugins that can be utilized with them.

## Features

TShockRpgToolKit offers extensive customization options, allowing you to modify the functionality of various events. This is achieved through PythonTS. In this README, we will provide a brief overview of how you can add functionality to each plugin using our scripting module.

## Usage

To get started with TShockRpgToolkit, please follow these steps:

1. Install TShockRpgToolkit by [downloading the latest release](https://github.com/TShockRpgToolkit/releases), and pasting in TShock's root.
2. Ensure you have IronPython and its dependencies dropped in the bin folder. These are found in the release.
3. Once installed, launch TShock. Everything should run without any modification.

## PythonTS Documentation
PythonTS by default generates a few default scripts, these can be found in the `scripting` folder.

By default, the following scripts can be found in there:

  - PlayerJoin.py
  - ServerStart.py
  - ServerLeave.py

Each of these scripts is attached to the appropriate TShock hooks and they will execute when any of these events are executed. For example, when a player joins, it will execute PlayerJoin.py.

To get the latest arguments that are passed through to the Python scripts, if one is missing or not working as previously, please check the source code for when the event fires. 

For example, if you navigate here: [PythonScriptingPlugin.cs](https://github.com/RenderBr/TshockRpgToolKit/blob/e178699ab82c78a37e5cc56f2b1684121b3bdc74/PythonTS/PythonScriptingPlugin.cs#L114)
You will notice the line:
```cs
ScriptArguments[] arg = new ScriptArguments[]
{
    new("Player", player)
};
```
ScriptArguments[] Arrays are typically passed through to it's respective Python script dynamically. This means each object is accessible in its respective script by the first parameter in each ScriptArgument.

To sum it up, you can utilize the object "player" in Python via something like this:
```py
# by default, the arguments passed are already accessible, meaning the user does not have to define them

# we can access "Player" as such:
Player.SendInfoMessage("Welcome to the server!")

# if we don't like the argument name we can change it like so:
global p = Player

# the object will work the same way, meaning we can access it's methods as such:
p.KillPlayer()
p.SendErrorMessage("haha ur dead!")
```

The function of this script will execute as follows:
- The player is greeted with a welcome message
- We then declare a new reference to the Player object defined as 'p'
- Using our newly declared variable, we kill the player and send a message

Now that you understand how arguments are given to Python scripts, looking at [Executor.cs](https://github.com/RenderBr/TshockRpgToolKit/blob/e178699ab82c78a37e5cc56f2b1684121b3bdc74/PythonTS/Executor.cs#L48), you will notice this line:
```cs
Scope.SetVariable("TSPlayer.All", TSPlayer.All);
Scope.SetVariable("TSPlayer.Server", TSPlayer.Server);
Scope.SetVariable("TSPlayers", TShock.Players);
Scope.SetVariable("TSUtils", TShock.Utils);
Scope.SetVariable("Main", Terraria.Main.instance);

ICollection<string> searchPaths = Engine.GetSearchPaths();
searchPaths.Add(TShock.SavePath + Path.DirectorySeparatorChar + "bin");
searchPaths.Add(TShock.SavePath + Path.DirectorySeparatorChar + "ServerPlugins");
```
These are loaded slightly differently, not passed through via arguments but strictly set. 

In **EVERY** script from PythonTS these objects are accessible:
  - TSPlayer.All
  - TSPlayer.Server
  - TSPlayers (TShock.Players array)
  - TSUtils (TShock Utility Manager)
  - Main (access to the Terraria instance, tiles, entities, etc.)

As well as this, everything in ServerPlugins and the bin folder are added as available import assemblies. Use the [IronPython docs](https://ironpython.net/documentation/dotnet/dotnet.html#id31) for a guide on how to import and utilize these.

 Now, we're just scraping the surface. For each of the plugins included in this kit, there are special arguments given to each script event type, just like with PlayerJoin.py.

 **If a script is not mentioned here, it does not have any special arguments passed.**

 ## Banking Integration

 ### OnAccountDeposit.py
 | Variable           | Value           |
|-----------------------|-----------------|
| Bank Account ([Class](https://github.com/RenderBr/TshockRpgToolKit/blob/v1.4.9/Banking/BankAccount.cs))          | `bankAccount`   |
| New Balance (decimal)          | `newBalance`    |
| Previous Balance (decimal)     | `previousBalance`|

 ### OnAccountWithdraw.py
 | Variable           | Value           |
|-----------------------|-----------------|
| Bank Account ([Class](https://github.com/RenderBr/TshockRpgToolKit/blob/v1.4.9/Banking/BankAccount.cs))          | `bankAccount`   |
| New Balance (decimal)          | `newBalance`    |
| Previous Balance (decimal)     | `previousBalance`|

 ### OnPreReward.py
| Variable                                   | Value           |
|-----------------------------------------------|-----------------|
| Player Name (string)                     | `playerName`    |
| Reward ([Class](https://github.com/RenderBr/TshockRpgToolKit/blob/v1.4.9/Banking/Rewards/Reward.cs))                           | `reward`        |
| Currency ([Class](https://github.com/RenderBr/TshockRpgToolKit/blob/v1.4.9/Banking/Currency/CurrencyDefinition.cs))                         | `currency`      |
| Value (decimal)                            | `value`         |

 ## CustomNPCs Integration

#### OnWaveUpdate.py
| Description                           | Value                 |
|---------------------------------------|-----------------------|
| Wave Index (int)                | `waveIndex`   |
| Wave Definition ([Class](https://github.com/RenderBr/TshockRpgToolKit/blob/v1.4.9/CustomNpcs/Invasions/WaveDefinition.cs))           | `waveDefinition`                |
| Current Points (int)            | `currentPoints`      |

#### OnWaveStart.py
| Description                      | Value                      |
|----------------------------------|----------------------------|
| Wave Index (int)          | `"waveIndex"`              |
| Wave ([Class](https://github.com/RenderBr/TshockRpgToolKit/blob/v1.4.9/CustomNpcs/Invasions/WaveDefinition.cs))                | `wave`                     |

#### OnWaveEnd.py

| Description                          | Value                      |
|--------------------------------------|----------------------------|
| Previous Wave Index (int)     | `previousWaveIndex`        |
| Previous Wave ([Class](https://github.com/RenderBr/TshockRpgToolKit/blob/v1.4.9/CustomNpcs/Invasions/WaveDefinition.cs))           | `previousWave`             |

#### OnSpawn.py, OnTransformed.py, OnAiUpdate.py, OnKilled.py
| Description                         | Value                |
|-------------------------------------|----------------------|
| Custom NPC ([Class](https://github.com/RenderBr/TshockRpgToolKit/blob/v1.4.9/CustomNpcs/Npcs/CustomNpc.cs))              | `customNpc`          |

#### OnCollision.py
| Description                         | Value                |
|-------------------------------------|----------------------|
| Custom NPC ([Class](https://github.com/RenderBr/TshockRpgToolKit/blob/v1.4.9/CustomNpcs/Npcs/CustomNpc.cs))              | `customNpc`          |
| Player (TSPlayer)                  | `player`             |

#### OnTileCollision.py
| Description                            | Value                  |
|----------------------------------------|------------------------|
| Custom NPC ([Class](https://github.com/RenderBr/TshockRpgToolKit/blob/v1.4.9/CustomNpcs/Npcs/CustomNpc.cs))              | `customNpc`          |
| Tile Collisions (List<Point>)            | `tileCollisions`       |

#### OnStrike.py
| Description                            | Value                  |
|----------------------------------------|------------------------|
| Custom NPC ([Class](https://github.com/RenderBr/TshockRpgToolKit/blob/v1.4.9/CustomNpcs/Npcs/CustomNpc.cs))              | `customNpc`          |
| Player (TSPlayer)                     | `player`               |
| Damage (int)                     | `damage`          |
| Knockback (float)                  | `knockback`       |
| Critical (bool)                   | `critical`        |

#### OnCheckReplace.py
| Description                           | Value                |
|---------------------------------------|----------------------|
| NPC (NPC)                       | `npc`                |

#### OnCheckSpawn.py
| Description                          | Value                |
|--------------------------------------|----------------------|
| Player (TSPlayer)                  | `player`             |
| Tile X (int)                   | `tileX`              |
| Tile Y (int)                   | `tileY`              |

#### OnSpawn.py, OnGameUpdate.py, OnAiUpdate.py, OnKilled.py (projectile)
| Description                              | Value                     |
|------------------------------------------|---------------------------|
| Custom Projectile ([Class](https://github.com/RenderBr/TshockRpgToolKit/blob/v1.4.9/CustomNpcs/Projectiles/CustomProjectile.cs))           | `customProjectile`        |

#### OnCollision.py (projectile)
| Description                              | Value                     |
|------------------------------------------|---------------------------|
| Custom Projectile ([Class](https://github.com/RenderBr/TshockRpgToolKit/blob/v1.4.9/CustomNpcs/Projectiles/CustomProjectile.cs))           | `customProjectile`        |
| Player (TSPlayer)                      | `player`                  |

#### OnTileCollision.py (projectile)
| Description                            | Value                  |
|----------------------------------------|------------------------|
| Custom Projectile ([Class](https://github.com/RenderBr/TshockRpgToolKit/blob/v1.4.9/CustomNpcs/Projectiles/CustomProjectile.cs))           | `customProjectile`        |
| Tile Collisions (List<Point>)            | `tileCollisions`       |

 ## CustomQuests Integration

#### YourQuest.py
This script will be determined by what you choose for it. The quest will execute the PrimaryMethodName set in the Quest info.
| Description                            | Value                  |
|----------------------------------------|------------------------|
| Quest ([Class](https://github.com/RenderBr/TshockRpgToolKit/blob/v1.4.9/CustomQuests/Quests/Quest.cs))           | `quest`        |

Quest classes have a few useful methods I thought I should outline here:
  ```py
  quest.QuestInfo # retrieves the QuestInfo class

  quest.Complete() # marks the quest as done for all party members

  quest.party # retrieves the party, something like this can be done with it:
  quest.party.SendMessage() # sends a message to every member in the party
  ```
There are more methods you can utilize. Please refer to them at [Quest.cs](https://github.com/RenderBr/TshockRpgToolKit/blob/v1.4.9/CustomQuests/Quests/Quest.cs) and [Quest.Dsl.cs](https://github.com/RenderBr/TshockRpgToolKit/blob/v1.4.9/CustomQuests/Quests/Quest.Dsl.cs)

 ## CustomSkills Integration

#### OnCast.py, OnCharge.py, OnFire.py, OnCancelled.py
| Description                            | Value                  |
|----------------------------------------|------------------------|
| Player (TSPlayer)           | `Player`        |
| Skillstate ([Class](https://github.com/RenderBr/TshockRpgToolKit/blob/v1.4.9/CustomSkills/SkillState.cs))           | `SkillState`        |

#### OnLevelUp.py
| Description                            | Value                  |
|----------------------------------------|------------------------|
| Player (TSPlayer)           | `Player`        |

 ## Leveling Integration
#### OnLevelUp.py, OnLevelDown.py, 
| Description                            | Value                  |
|----------------------------------------|------------------------|
| Player (TSPlayer)           | `player`        |
| Class ([Class](https://github.com/RenderBr/TshockRpgToolKit/blob/v1.4.9/Leveling/Classes/Class.cs))           | `class`        |
| Level Index (int)           | `levelIndex`        |

#### OnClassChange.py
| Description                            | Value                  |
|----------------------------------------|------------------------|
| Player (TSPlayer)           | `player`        |
| Class ([Class](https://github.com/RenderBr/TshockRpgToolKit/blob/v1.4.9/Leveling/Classes/Class.cs))           | `class`        |
| Previous Class ([Class](https://github.com/RenderBr/TshockRpgToolKit/blob/v1.4.9/Leveling/Classes/Class.cs))           | `previousClass`        |

#### OnClassMastered.py
| Description                            | Value                  |
|----------------------------------------|------------------------|
| Player (TSPlayer)           | `player`        |
| Class ([Class](https://github.com/RenderBr/TshockRpgToolKit/blob/v1.4.9/Leveling/Classes/Class.cs))           | `class`        |


## Contributing

We welcome contributions from the community to improve TShockRpgToolkit. If you have any bug fixes, feature enhancements, or suggestions, please feel free to open an issue or submit a pull request.

## License

TShockRpgToolkit is released under the [MIT License](LICENSE).

---

We hope you enjoy using TShockRpgToolkit and find it useful for customizing your TShock server! If you have any questions or need assistance, please don't hesitate to reach out.

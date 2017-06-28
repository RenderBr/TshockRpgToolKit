﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CustomQuests.Quests;
using JetBrains.Annotations;
using NLua;
using TShockAPI;

namespace CustomQuests.Sessions
{
    /// <summary>
    ///     Holds session information.
    /// </summary>
    public sealed class Session : IDisposable
    {
        private readonly TSPlayer _player;

        private Quest _currentQuest;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Session" /> class with the specified player and session information.
        /// </summary>
        /// <param name="player">The player, which must not be <c>null</c>.</param>
        /// <param name="sessionInfo">The session information, which must not be <c>null</c>.</param>
        /// <exception cref="ArgumentNullException">
        ///     Either <paramref name="player" /> or <paramref name="sessionInfo" /> is <c>null</c>.
        /// </exception>
        public Session([NotNull] TSPlayer player, SessionInfo sessionInfo)
        {
            _player = player ?? throw new ArgumentNullException(nameof(player));
            SessionInfo = sessionInfo ?? throw new ArgumentNullException(nameof(sessionInfo));
        }

        /// <summary>
        ///     Gets a read-only view of the available quest names.
        /// </summary>
        [ItemNotNull]
        [NotNull]
        public IEnumerable<string> AvailableQuestNames => SessionInfo.AvailableQuestNames;

        /// <summary>
        ///     Gets a read-only view of the completed quest names.
        /// </summary>
        [ItemNotNull]
        [NotNull]
        public IEnumerable<string> CompletedQuestNames => SessionInfo.CompletedQuestNames;

        /// <summary>
        ///     Gets or sets the current Lua instance.
        /// </summary>
        [CanBeNull]
        public Lua CurrentLua { get; private set; }

        /// <summary>
        ///     Gets or sets the current quest.
        /// </summary>
        [CanBeNull]
        public Quest CurrentQuest
        {
            get => _currentQuest;
            set
            {
                _currentQuest = value;
                CurrentQuestInfo = _currentQuest?.QuestInfo;
                SessionInfo.CurrentQuestInfo = CurrentQuestInfo;
            }
        }

        /// <summary>
        ///     Gets the current quest info.
        /// </summary>
        [CanBeNull]
        public QuestInfo CurrentQuestInfo { get; private set; }

        /// <summary>
        ///     Gets the current quest name.
        /// </summary>
        [CanBeNull]
        public string CurrentQuestName => CurrentQuestInfo?.Name;

        /// <summary>
        ///     Gets or sets a value indicating whether the session is aborting the quest.
        /// </summary>
        public bool IsAborting { get; set; }

        /// <summary>
        ///     Gets or sets the party.
        /// </summary>
        [CanBeNull]
        public Party Party { get; set; }

        /// <summary>
        ///     Gets the session information.
        /// </summary>
        [NotNull]
        public SessionInfo SessionInfo { get; }

        /// <summary>
        ///     Disposes the session.
        /// </summary>
        public void Dispose()
        {
            CurrentQuest?.Dispose();
            CurrentQuest = null;
            CurrentLua?.Dispose();
            CurrentLua = null;
        }

        /// <summary>
        ///     Determines whether the session can see the specified quest.
        /// </summary>
        /// <param name="questInfo">The quest information, which must not be <c>null</c>.</param>
        /// <returns><c>true</c> if the session can see the quest; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="questInfo" /> is <c>null</c>.</exception>
        public bool CanSeeQuest([NotNull] QuestInfo questInfo)
        {
            if (questInfo == null)
            {
                throw new ArgumentNullException(nameof(questInfo));
            }

            return questInfo.RequiredRegionName == null ||
                   TShock.Regions.InAreaRegion(_player.TileX, _player.TileY)
                       .Any(r => r.Name == questInfo.RequiredRegionName);
        }

        /// <summary>
        ///     Gets the quest state.
        /// </summary>
        /// <returns>The state.</returns>
        [LuaGlobal]
        [UsedImplicitly]
        public string GetQuestState() => SessionInfo.CurrentQuestState;

        /// <summary>
        ///     Loads the quest with the specified info.
        /// </summary>
        /// <param name="questInfo">The quest info, which must not be <c>null</c>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="questInfo" /> is <c>null</c>.</exception>
        public void LoadQuest([NotNull] QuestInfo questInfo)
        {
            if (questInfo == null)
            {
                throw new ArgumentNullException(nameof(questInfo));
            }

            var quest = new Quest(questInfo);
            var lua = new Lua {["party"] = Party ?? new Party(_player.Name, _player)};
            lua.LoadCLRPackage();
            lua.DoString("import('System')");
            lua.DoString("import('CustomQuests', 'CustomQuests.Triggers')");
            lua.DoString("import('OTAPI', 'Microsoft.Xna.Framework')");
            lua.DoString("import('OTAPI', 'Terraria')");
            LuaRegistrationHelper.TaggedInstanceMethods(lua, quest);
            LuaRegistrationHelper.TaggedInstanceMethods(lua, this);
            LuaRegistrationHelper.TaggedStaticMethods(lua, typeof(QuestFunctions));

            var path = Path.Combine("quests", $"{questInfo.Name}.lua");
            lua.DoFile(path);
            CurrentQuest = quest;
            CurrentQuestInfo = questInfo;
            CurrentLua = lua;
        }

        /// <summary>
        ///     Revokes the quest with the specified name.
        /// </summary>
        /// <param name="name">The quest name, which must not be <c>null</c>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="name" /> is <c>null</c>.</exception>
        public void RevokeQuest([NotNull] string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            SessionInfo.AvailableQuestNames.Remove(name);
            SessionInfo.CompletedQuestNames.Remove(name);
        }

        /// <summary>
        ///     Sets the quest state.
        /// </summary>
        /// <param name="state">The state.</param>
        [LuaGlobal]
        [UsedImplicitly]
        public void SetQuestState([CanBeNull] string state)
        {
            SessionInfo.CurrentQuestState = state;
        }

        /// <summary>
        ///     Unlocks the quest with the specified name.
        /// </summary>
        /// <param name="name">The quest name, which must not be <c>null</c>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="name" /> is <c>null</c>.</exception>
        public void UnlockQuest([NotNull] string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            SessionInfo.AvailableQuestNames.Add(name);
        }

        /// <summary>
        ///     Updates the current quest.
        /// </summary>
        public void UpdateQuest()
        {
            if (CurrentQuest == null)
            {
                return;
            }

            if (IsAborting)
            {
                IsAborting = false;
                Dispose();
                SetQuestState(null);
                return;
            }

            if (Party == null || _player == Party.Leader)
            {
                CurrentQuest.Update();
            }
            if (CurrentQuest.IsEnded)
            {
                if (CurrentQuest.IsSuccessful)
                {
                    var repeatedQuests = SessionInfo.RepeatedQuestNames;
                    // ReSharper disable once PossibleNullReferenceException
                    if (CurrentQuestInfo.MaxRepeats >= 0)
                    {
                        // ReSharper disable once AssignNullToNotNullAttribute
                        if (repeatedQuests.TryGetValue(CurrentQuestName, out var repeats))
                        {
                            repeatedQuests[CurrentQuestName] = repeats + 1;
                        }
                        else
                        {
                            repeatedQuests[CurrentQuestName] = 1;
                        }
                        if (repeatedQuests[CurrentQuestName] > CurrentQuestInfo.MaxRepeats)
                        {
                            SessionInfo.AvailableQuestNames.Remove(CurrentQuestName);
                            SessionInfo.CompletedQuestNames.Add(CurrentQuestName);
                            repeatedQuests.Remove(CurrentQuestName);
                        }
                    }
                    _player.SendSuccessMessage("Quest completed!");
                }
                else
                {
                    _player.SendErrorMessage("Quest failed.");
                }

                Dispose();
            }
        }
    }
}

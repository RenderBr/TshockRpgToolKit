using Banking.Configuration;
using Banking.Database;
using Corruption.PluginSupport;
using PythonTS;
using PythonTS.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using TShockAPI;

namespace Banking
{
    /// <summary>
    /// The Bank is the central root of BankAccount information for players, and also coordinates persistance to disk.
    /// Currency information can be retrieved through the CurrencyManager.
    /// </summary>
    public class Bank
    {
        internal IDatabase Database;

        /// <summary>
        /// Gets the CurrencyManager.
        /// </summary>
        public CurrencyManager CurrencyManager { get; private set; }

        private Dictionary<string, PlayerBankAccountMap> playerAccountMaps;

        /// <summary>
        /// Gets all BankAccounts linked to a player.
        /// </summary>
        /// <param name="playerName">Name of player.</param>
        /// <returns>PlayerBankAccountMap.</returns>
        public PlayerBankAccountMap this[string playerName]
        {
            get
            {
                playerAccountMaps.TryGetValue(playerName, out var accountMap);
                return accountMap;
            }
        }

        //scripting points
        //private void ScriptHook(string playerName, CurrencyDefinition currency, ref decimal value, Reward reward)
        internal Script OnPreReward;
        private Script OnAccountDeposit;
        private Script OnAccountWithdraw;

        /// <summary>
        /// Raised when a deposit into a BankAccount succeeds.
        /// </summary>
        public event EventHandler<BalanceChangedEventArgs> AccountDeposit;

        /// <summary>
        /// Raised when a withdraw from a BankAccount succeeds.
        /// </summary>
        public event EventHandler<BalanceChangedEventArgs> AccountWithdraw;

        internal Bank()
        {
            CurrencyManager = new CurrencyManager();
            playerAccountMaps = new Dictionary<string, PlayerBankAccountMap>();
        }

        internal void InvokeAccountDeposit(BankAccount bankAccount, ref decimal newBalance, ref decimal previousBalance)
        {
            if (AccountDeposit != null || OnAccountDeposit != null)
            {
                var args = new ScriptArguments[]
                {
                    new ScriptArguments("bankAccount", bankAccount),
                    new ScriptArguments("newBalance", newBalance),
                    new ScriptArguments("previousBalance", previousBalance)
                };
                var eventArgs = new BalanceChangedEventArgs(bankAccount, ref newBalance, ref previousBalance);

                AccountDeposit?.Invoke(this, eventArgs);

                try
                {
                    OnAccountDeposit.Execute(args);
                }
                catch (Exception ex)
                {
                    BankingPlugin.Instance.LogPrint(ex.ToString(), TraceLevel.Error);
                }
            }
        }

        internal void InvokeAccountWithdraw(BankAccount bankAccount, ref decimal newBalance, ref decimal previousBalance)
        {
            if (AccountWithdraw != null || OnAccountWithdraw != null)
            {
                var args = new ScriptArguments[]
                {
                    new ScriptArguments("bankAccount", bankAccount),
                    new ScriptArguments("newBalance", newBalance),
                    new ScriptArguments("previousBalance", previousBalance)
                };
                var eventArgs = new BalanceChangedEventArgs(bankAccount, ref newBalance, ref previousBalance);
                AccountWithdraw?.Invoke(this, eventArgs);

                try
                {
                    OnAccountWithdraw.Execute(args);
                }
                catch (Exception ex)
                {
                    BankingPlugin.Instance.LogPrint(ex.ToString(), TraceLevel.Error);
                }
            }
        }

        /// <summary>
        /// Recreates all BankAccounts using the configured Database.
        /// </summary>
        public void Load()
        {
            Debug.Print("BankAccountManager.Load!");
            CurrencyManager = new CurrencyManager(BankingPlugin.DataDirectory);

            playerAccountMaps.Clear();

            if (!string.IsNullOrWhiteSpace(Config.Instance.ScriptPath))
                LoadScripts(Path.Combine(BankingPlugin.DataDirectory, Config.Instance.ScriptPath));

            var cfg = Config.Instance.Database;

            //Database = new SqliteDatabase(Config.Instance.Database.ConnectionString);
            //Database = DatabaseFactory.LoadOrCreateDatabase("mysql", "Server=localhost;Database=db_banking;Uid=xxx;Pwd=xxx;");
            //Database = DatabaseFactory.LoadOrCreateDatabase("redis", "localhost:6379");
            Database = DatabaseFactory.LoadOrCreateDatabase(cfg.DatabaseType, cfg.ConnectionString);

            var accounts = Database.Load();

            foreach (var acc in accounts)
            {
                PlayerBankAccountMap playerAccounts = null;

                if (!playerAccountMaps.TryGetValue(acc.OwnerName, out playerAccounts))
                {
                    playerAccounts = new PlayerBankAccountMap(acc.OwnerName);
                    playerAccountMaps.Add(acc.OwnerName, playerAccounts);
                }

                playerAccounts.Add(acc.Name, acc);
            }

            EnsureBankAccountsExist(TSPlayer.Server.Name);

            foreach (var player in TShock.Players)
            {
                if (player?.IsLoggedIn == true) // && player?.Active == true)
                {
                    EnsureBankAccountsExist(player.Name);
                }
            }
        }

        private void LoadScripts(string scriptPath)
        {
            string prefix = $"{BankingPlugin.DataDirectory}{Path.DirectorySeparatorChar}{Config.Instance.ScriptPath}{Path.DirectorySeparatorChar}";
            if (!Directory.Exists(prefix))
                Directory.CreateDirectory(prefix);

            OnPreReward = Script.AddModuleDefault(prefix + "OnPreReward.py");
            OnAccountDeposit = Script.AddModuleDefault(prefix + "OnAccountDeposit.py");
            OnAccountWithdraw = Script.AddModuleDefault(prefix + "OnAccountWithdraw.py");
        }

        /// <summary>
        /// For future expansion.
        /// </summary>
        public void Save()
        {
            Debug.Print("BankAccountManager.Save!");
            //Database.Save(bankAccounts.Values.ToArray());
            throw new NotImplementedException("This method is for future expansion.");
        }

        /// <summary>
        /// Ensures that a player has BankAccounts for each configured Currency.
        /// </summary>
        /// <param name="playerName"></param>
        internal void EnsureBankAccountsExist(string playerName)
        {
            if (!playerAccountMaps.TryGetValue(playerName, out var playerAccountMap))
            {
                Debug.Print($"Creating bank account map for user {playerName}...");
                playerAccountMap = new PlayerBankAccountMap(playerName);
                playerAccountMaps.Add(playerName, playerAccountMap);
            }

            var currencyNames = CurrencyManager.Select(v => v.InternalName);
            playerAccountMap.EnsureBankAccountNamesExist(currencyNames);

            //lets set/reset a default mapping from currency's to bank accounts for reward purposes. Anything subscribed to EnsuringPlayerAccounts event
            //will have a chance to change the mapping again.
            foreach (var name in currencyNames)
                playerAccountMap.SetAccountNameOverride(name, name);
        }

        /// <summary>
        /// Tries to get the named BankAccount for the given player, if it exists.
        /// </summary>
        /// <param name="playerName">Player name.</param>
        /// <param name="accountName">BankAccount name.</param>
        /// <returns>BankAccount if found, null otherwise.</returns>
        public BankAccount GetBankAccount(string playerName, string accountName)
        {
            BankAccount bankAccount = null;

            if (playerAccountMaps.TryGetValue(playerName, out var accountMap))
                bankAccount = accountMap.TryGetBankAccount(accountName);

            return bankAccount;
        }

        /// <summary>
        /// Calculates the sum of all BankAccounts linked to the player.
        /// </summary>
        /// <param name="playerName">Name of player.</param>
        /// <returns>Sum of all account balances.</returns>
        public decimal GetTotalBalance(string playerName)
        {
            var sum = BankingPlugin.Instance.GetAllBankAccountsForPlayer(playerName)
                        .Sum(ba => ba.Balance);

            return sum;
        }
    }
}

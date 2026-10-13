using System;
using System.Collections.Generic;
using CPM.Account;
using CodeStage.AntiCheat.Storage;
using Cysharp.Threading.Tasks;
using UnityEngine;
using _Car_Parking.Scripts.PrefsKeys;

namespace _Car_Parking.Scripts.Infrastructure.Start
{
	public class AlphaWipe
	{
		public const bool WipeOldAccounts = true;

		private const string OldPlayerIdKey = "PlayerID";

		private const string OldAccountId = "accountID";

		private const string OldEmail = "userEmail";

		private const string NameKey = "PlayerName";

		private const string FriendIdKey = "FriendID";

		private const string FriendNoKey = "FriendNo";

		private const string FriendNameKey = "FriendName";

		private const string FriendAccountIdKey = "FriendAccountID";

		public const string NeedToSyncMoneyWithDatabase = "NeedToSyncMoneyWithDatabase";

		private IAccountController _accountController;

		public AlphaWipe(IAccountController accountController)
		{
			_accountController = accountController;
		}

		public void AlphaVersionWipeCheck()
		{
			if (ObscuredPrefs.GetString("userEmail", "0") != "0")
			{
				Debug.Log("Old email found. Old account found. Sign out. Reset data.");
				_accountController.ResetData(reloadScene: false);
			}
			else if (ObscuredPrefs.GetString("accountID", "0") != "0")
			{
				Debug.Log("Old account id found. Old Anon account found. Reset data.");
				SaveOldDataAndReset();
				ObscuredPrefs.SetInt("NeedToSyncMoneyWithDatabase", 1);
			}
			else if (ObscuredPrefs.GetString("PlayerID", "0") != "0")
			{
				Debug.Log("Old player id found. Reset data.");
				SaveOldDataAndReset();
			}
		}

		private void SaveOldDataAndReset()
		{
			string text = ObscuredPrefs.GetString("PlayerID", "0");
			string value = ObscuredPrefs.GetString("PlayerName", "0");
			string text2 = ObscuredPrefs.GetString("accountID", "0");
			List<PrefFriendItem> list = new List<PrefFriendItem>();
			for (int i = 0; i < 120; i++)
			{
				if (PlayerPrefs.GetInt("FriendNo" + i, 0) != 0)
				{
					string text3 = PlayerPrefs.GetString("FriendID" + i, "0");
					if (!(text3 == "0"))
					{
						string name = PlayerPrefs.GetString("FriendName" + i, "");
						string accountID = PlayerPrefs.GetString("FriendAccountID" + i, "");
						PrefFriendItem item = new PrefFriendItem
						{
							Name = name,
							accountID = accountID,
							id = text3
						};
						list.Add(item);
					}
				}
			}
			_accountController.ResetData(reloadScene: false);
			if (text != "0")
			{
				ManagerKeysForPrefs.LocalId = text;
			}
			ObscuredPrefs.SetString("PlayerName", value);
			if (text2 != "0")
			{
				ManagerKeysForPrefs.AccountId = text2;
			}
			for (int j = 0; j < list.Count; j++)
			{
				PrefFriendItem prefFriendItem = list[j];
				PlayerPrefs.SetString("FriendID" + j, prefFriendItem.id);
				PlayerPrefs.SetString("FriendName" + j, prefFriendItem.Name);
				PlayerPrefs.SetString("FriendAccountID" + j, prefFriendItem.accountID);
				PlayerPrefs.SetInt("FriendNo" + j, 1);
			}
		}

		public static async UniTaskVoid TrySyncMoneyWithDatabase(CurrencyManagerController currencyManagerController)
		{
			try
			{
				if (ObscuredPrefs.HasKey("NeedToSyncMoneyWithDatabase"))
				{
					Debug.Log("Need to sync money with database. Call function.");
					await currencyManagerController.SyncMoneyFromDatabase();
					ObscuredPrefs.DeleteKey("NeedToSyncMoneyWithDatabase");
				}
			}
			catch (Exception arg)
			{
				Debug.LogError($"TrySyncMoneyWithDatabase: {arg}");
			}
		}
	}
}

using System;
using CodeStage.AntiCheat.ObscuredTypes;
using CodeStage.AntiCheat.Storage;
using UnityEngine;
using UnityEngine.Scripting;
using _Car_Parking.Scripts.PrefsKeys;

namespace CPM.PlayerData
{
	[Preserve]
	public class PlayerDataStorage
	{
		private ObscuredString playerName = "";

		private ObscuredString playerID = "";

		private ObscuredString accountID = "";

		public string PlayerName
		{
			get
			{
				return playerName;
			}
			set
			{
				playerName = value;
			}
		}

		public ObscuredString AccountID
		{
			get
			{
				return accountID;
			}
			set
			{
				if (accountID != value)
				{
					Debug.Log($"Account ID set:{value}");
					accountID = value;
					OnAccountIdUpdatedEvent?.Invoke(accountID);
				}
			}
		}

		public bool HasAccountID
		{
			get
			{
				if (AccountID != (ObscuredString)"0" && AccountID != (ObscuredString)"1" && AccountID != (ObscuredString)"")
				{
					return !string.IsNullOrEmpty(AccountID);
				}
				return false;
			}
		}

		public ObscuredString PlayerID
		{
			get
			{
				return playerID;
			}
			set
			{
				playerID = value;
				ManagerKeysForPrefs.LocalId = value;
				Debug.Log($"Player ID:{PlayerID}");
			}
		}

		public bool IsBanned => ObscuredPrefs.GetInt("Banned") == 1;

		public event Action<ObscuredString> OnAccountIdUpdatedEvent;

		public void ResetValues()
		{
			playerID = "";
			AccountID = "1";
			playerName = "";
		}

		public void SetBasicPlayerData(string playerName, string localID)
		{
			PlayerName = playerName;
			ObscuredPrefs.SetString("PlayerName", playerName);
			PlayerID = localID;
		}
	}
}

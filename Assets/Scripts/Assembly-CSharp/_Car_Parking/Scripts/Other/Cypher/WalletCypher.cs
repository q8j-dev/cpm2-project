using System;
using CPM.PlayerData;
using UnityEngine;
using VContainer;

namespace _Car_Parking.Scripts.Other.Cypher
{
	public class WalletCypher
	{
		private static PlayerDataStorage _playerDataStorage;

		[Inject]
		public WalletCypher(PlayerDataStorage playerDataStorage)
		{
			_playerDataStorage = playerDataStorage;
		}

		public static string GetKey1()
		{
			return _playerDataStorage.AccountID;
		}

		public static string GetKey2()
		{
			return _playerDataStorage.AccountID;
		}

		public static int[] GetCoinsResult(string[] data)
		{
			try
			{
				string s = Decode(data[0]);
				string s2 = Decode(data[1]);
				return new int[2]
				{
					int.Parse(s),
					int.Parse(s2)
				};
			}
			catch (Exception arg)
			{
				Debug.LogError($"GetCoinsResult:{arg}");
				return new int[2];
			}
		}

		public static int[] GetCoinsMoneyResult(string[] data)
		{
			try
			{
				string s = Decode(data[0]);
				string s2 = Decode(data[1]);
				string s3 = Decode(data[2]);
				return new int[3]
				{
					int.Parse(s),
					int.Parse(s2),
					int.Parse(s3)
				};
			}
			catch (Exception arg)
			{
				Debug.LogError($"GetCoinsResult:{arg}");
				return new int[3];
			}
		}

		public static int[] BuyCoinsResult(string[] data)
		{
			try
			{
				string s = Decode(data[0]);
				string s2 = Decode(data[1]);
				return new int[2]
				{
					int.Parse(s),
					int.Parse(s2)
				};
			}
			catch (Exception message)
			{
				Debug.LogError(message);
				return new int[2];
			}
		}

		public static int[] TrySpendCoinsResult(string[] data, int obscuredInt)
		{
			try
			{
				string s = Decode(data[0], obscuredInt);
				string s2 = Decode(data[1], obscuredInt);
				return new int[2]
				{
					int.Parse(s),
					int.Parse(s2)
				};
			}
			catch (Exception message)
			{
				Debug.LogError(message);
				return new int[2];
			}
		}

		public static string Encode(string message, string addKey)
		{
			return CypherMediator.Encode(message, GetKey1(), GetKey2());
		}

		public static string Decode(string encryptedString)
		{
			return CypherMediator.Decode(encryptedString, GetKey1(), GetKey2());
		}

		public static string Decode(string encryptedString, int value)
		{
			return CypherMediator.Decode(encryptedString, GetKey1(), GetKey2());
		}

		public static string EncodeWithCustomToken(string message, string key)
		{
			return CypherMediator.Encode(message, key, key);
		}

		public static string DecodeWithCustomToken(string data, string[] key)
		{
			try
			{
				return CypherMediator.Decode(data, key[0], key[1]);
			}
			catch (Exception arg)
			{
				Debug.LogError($"GetCoinsResult:{arg}");
				return "";
			}
		}

		public static string DecodeWithOtherUID(string data, int otherActorNumber)
		{
			if (!V.CoreGameManager || V.CoreGameManager.FreeDriveProvider == null || !V.CoreGameManager.FreeDriveProvider.MultiPlayer)
			{
				return "";
			}
			try
			{
				MultiPersonControl multiPersonControl = V.CoreGameManager.FreeDriveProvider.MultiPlayer.GetMultiPersonControl(otherActorNumber);
				if (!multiPersonControl)
				{
					return "";
				}
				if (string.IsNullOrEmpty(multiPersonControl.ThisPlayerAccountID))
				{
					return "";
				}
				return DecodeWithCustomToken(data, new string[2] { multiPersonControl.ThisPlayerAccountID, multiPersonControl.ThisPlayerAccountID });
			}
			catch (Exception arg)
			{
				Debug.LogError($"GetCoinsResult:{arg}");
				return "";
			}
		}
	}
}

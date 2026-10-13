using System;
using CPM.PlayerData;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.Other.Cypher;
using _Car_Parking.Scripts.Wallet.Cars;

namespace _Car_Parking.Scripts.Wallet.Cypher
{
	public class BuyCarCypher
	{
		private const string NoAccountKey = "1gjtrfd3";

		private static PlayerDataStorage _playerDataStorage;

		[Inject]
		public BuyCarCypher(PlayerDataStorage playerDataStorage)
		{
			_playerDataStorage = playerDataStorage;
		}

		public static string GetKey1()
		{
			if (!(_playerDataStorage.AccountID == (ObscuredString)"") && !(_playerDataStorage.AccountID == (ObscuredString)"1") && !(_playerDataStorage.AccountID == (ObscuredString)"0"))
			{
				return _playerDataStorage.AccountID;
			}
			return "bgfmk5kv";
		}

		public static string GetKey2()
		{
			return GetKey1();
		}

		public static CarBuyResponse GetBuyCarResult(CarBuyResponseRaw data)
		{
			try
			{
				if (data == null)
				{
					return null;
				}
				CarBuyResponse carBuyResponse = new CarBuyResponse();
				if (data.M != null)
				{
					string s = Decode(data.M);
					carBuyResponse.NewMoney = int.Parse(s);
				}
				if (data.C != null)
				{
					string s2 = Decode(data.C);
					carBuyResponse.NewCoins = int.Parse(s2);
				}
				if (data.CI != null)
				{
					carBuyResponse.CarInstanceID = Decode(data.CI);
				}
				return carBuyResponse;
			}
			catch (Exception arg)
			{
				Debug.LogError($"GetBuyCarResult:{arg}");
				return new CarBuyResponse();
			}
		}

		public static string Encode(string message)
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
	}
}

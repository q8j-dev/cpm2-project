using System;
using System.Globalization;
using CPM.PlayerData;
using CodeStage.AntiCheat.Storage;
using Photon.Pun;
using _Car_Parking.Scripts.Multiplayer.InterestGroupsImpl;
using _Car_Parking.Scripts.PrefsKeys;

namespace _Car_Parking.Scripts.Multiplayer
{
	public static class PersonMultiplayerDataSending
	{
		public static event Action<MultiPersonControl> OnPersonDataApplied;

		public static event Action<MultiPersonControl> OnPersonDataRemoved;

		public static void RemovePersonData(MultiPersonControl multiPersonControl)
		{
			OnPersonDataRemoved?.Invoke(multiPersonControl);
		}

		public static object[] SetDataForSend(int ownCarViewId, string equipmentData, PlayerDataStorage playerDataStorage)
		{
			object[] obj = new object[6]
			{
				playerDataStorage.PlayerName,
				ownCarViewId,
				(string)playerDataStorage.PlayerID,
				(ManagerKeysForPrefs.AccountId == "0") ? "" : ManagerKeysForPrefs.AccountId,
				equipmentData,
				null
			};
			int day = DateTime.Now.Day;
			obj[5] = (ObscuredPrefs.GetInt($"rank{day}") << 8) | InterestGroupManager.PersonInterestGroup;
			return obj;
		}

		public static void ApplyPersonMultiplayerData(MultiPersonControl multiPersonControl)
		{
			object[] instantiationData = multiPersonControl.GetComponent<PhotonView>().InstantiationData;
			multiPersonControl.ThisPlayerName = (string)instantiationData[0];
			multiPersonControl.OwnCarViewID = (int)instantiationData[1];
			multiPersonControl.ThisPlayerID = (string)instantiationData[2];
			multiPersonControl.ThisPlayerAccountID = (string)instantiationData[3];
			(int, int[]) tuple = DecodeEquipments((string)instantiationData[4]);
			multiPersonControl.Gender = tuple.Item1;
			multiPersonControl.equipments = tuple.Item2;
			int num = (int)instantiationData[5];
			multiPersonControl.InterestGroup = (byte)(num % 256);
			int num2 = num >> 8;
			if (num2 < 7)
			{
				multiPersonControl.rank = num2;
			}
			else
			{
				multiPersonControl.validateRank().Forget();
			}
			OnPersonDataApplied?.Invoke(multiPersonControl);
		}

		private static (int, int[]) DecodeEquipments(string input)
		{
			string[] array = input.Split("?"[0]);
			int[] array2 = new int[11];
			for (int i = 1; i < array.Length; i++)
			{
				array2[i - 1] = int.Parse(array[i], CultureInfo.InvariantCulture);
			}
			return (int.Parse(array[0], CultureInfo.InvariantCulture), array2);
		}
	}
}

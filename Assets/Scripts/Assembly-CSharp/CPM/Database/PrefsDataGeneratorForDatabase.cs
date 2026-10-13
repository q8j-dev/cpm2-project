using System.Collections.Generic;
using CPM.Database.Constructors;
using CodeStage.AntiCheat.Storage;
using UnityEngine;
using _Car_Parking.Scripts.Animations.Emotes.Locker;
using _Car_Parking.Scripts.CarSlots;
using _Car_Parking.Scripts.CommonUtility;
using _Car_Parking.Scripts.DataStorageService;
using _Car_Parking.Scripts.DataStorageService.PlayerData;
using _Car_Parking.Scripts.Database;
using _Car_Parking.Scripts.Database.Account;
using _Car_Parking.Scripts.Other.Helpers;
using _Car_Parking.Scripts.Police;

namespace CPM.Database
{
	public class PrefsDataGeneratorForDatabase
	{
		private const int LevelsMaxCount = 110;

		public AdditionalPlayerProgress GenerateOtherData(PlayerDatas.FriendsUtility friendsUtility, IPoliceSystem policeSystem, ISlotService slotService)
		{
			float[] array = new float[110];
			for (int i = 0; i < 110; i++)
			{
				array[i] = Mathf.Round(ObscuredPrefs.GetFloat("bestResultID" + i) * 100f) * 0.01f;
			}
			return new AdditionalPlayerProgress(new List<float>(array), AnimationShop.GetBoughtAnimationsByPrefs(), EmoteLockerPrefs.GetPreset(), GenerateFlags(), HomesController.GenerateOpenHomes(), GenerateEggs(), slotService.SlotCollection.SlotCountPrefs, AdditionalDataSaver.GenerateMain(), AdditionalDataSaver.GeneratePurchased(), AdditionalDataSaver.GenerateCamera(), policeSystem.BoughtBodyKit.GetDataForDataBase(), policeSystem.BoughtSiren.GetDataForDataBase());
		}

		private Dictionary<int, int> GenerateFlags()
		{
			return FlagConstructor.GetFlagsData();
		}

		private List<long> GenerateEggs()
		{
			List<int> list = new List<int>();
			for (int i = 0; i < 64; i++)
			{
				int item = ObscuredPrefs.GetInt("egg" + i);
				list.Add(item);
			}
			long bitMaskFromList = BitMaskToListConverter.GetBitMaskFromList(list);
			List<int> list2 = new List<int>();
			for (int j = 64; j < 100; j++)
			{
				int item2 = ObscuredPrefs.GetInt("egg" + j);
				list2.Add(item2);
			}
			long bitMaskFromList2 = BitMaskToListConverter.GetBitMaskFromList(list2);
			return new List<long> { bitMaskFromList, bitMaskFromList2 };
		}

		public static void SavePrefsData(AdditionalPlayerProgress additionalPlayerProgress, PlayerDatas.FriendsUtility friendsUtility, IPoliceSystem policeSystem, ISlotService slotService)
		{
			friendsUtility.DeleteAllFriends();
			friendsUtility.RefillFriendsList();
			if (additionalPlayerProgress == null)
			{
				return;
			}
			_ = additionalPlayerProgress.Animations;
			AnimationShop.SetBoughtAnimationsByPrefs(additionalPlayerProgress.Animations);
			if (additionalPlayerProgress.SelectedAnimations != null)
			{
				EmoteLockerPrefs.SetPreset(additionalPlayerProgress.SelectedAnimations);
			}
			else
			{
				EmoteLockerPrefs.DeletePreset();
			}
			LevelProgressionHelper.DeleteAllLevelProgressionData();
			if (additionalPlayerProgress.LevelsDoneTime != null && additionalPlayerProgress.LevelsDoneTime.Count != 0)
			{
				float[] array = additionalPlayerProgress.LevelsDoneTime.ToArray();
				for (int i = 0; i < Mathf.Min(array.Length, 110); i++)
				{
					if (array[i] != 0f)
					{
						ObscuredPrefs.SetFloat("bestResultID" + i, array[i]);
					}
				}
			}
			AdditionalDataSaver.SaveMainAndOther(additionalPlayerProgress);
			HomesController.SetHomes(additionalPlayerProgress.Homes);
			List<long> eggs = additionalPlayerProgress.Eggs;
			if (eggs != null && eggs.Count == 2)
			{
				List<int> listFromBitMask = BitMaskToListConverter.GetListFromBitMask(eggs[0]);
				for (int j = 0; j < listFromBitMask.Count; j++)
				{
					if (listFromBitMask[j] == 1)
					{
						ObscuredPrefs.SetInt("egg" + j, 1);
					}
				}
				List<int> listFromBitMask2 = BitMaskToListConverter.GetListFromBitMask(eggs[1]);
				for (int k = 0; k < listFromBitMask2.Count; k++)
				{
					if (listFromBitMask2[k] == 1)
					{
						ObscuredPrefs.SetInt("egg" + (64 + k), 1);
					}
				}
			}
			FlagConstructor.SaveFlagsFromDatabase(additionalPlayerProgress.Flags);
			policeSystem.BoughtBodyKit.LoadDataFromDataBase(additionalPlayerProgress.PoliceBodyKits);
			policeSystem.BoughtSiren.LoadDataFromDataBase(additionalPlayerProgress.PoliceSounds);
		}

		public static void SaveFavouriteVinylsDataToPrefs(List<int> favouriteVinyls)
		{
			foreach (int favouriteVinyl in favouriteVinyls)
			{
				PlayerPrefs.SetInt("FavouriteVinyl" + favouriteVinyl, 1);
			}
		}

		public static void SaveFavouriteWheelsDataToPrefs(List<int> favouriteWheels)
		{
			foreach (int favouriteWheel in favouriteWheels)
			{
				PlayerPrefs.SetInt("FavouriteWheel" + favouriteWheel, 1);
			}
		}

		public static void SetFriends(FriendsCollection additionalPlayerProgress, PlayerDatas.FriendsUtility utilityLocal)
		{
			int num = 0;
			if (additionalPlayerProgress.FriendsID == null || additionalPlayerProgress.FriendsID.Count == 0)
			{
				return;
			}
			foreach (PrefFriendItem item in additionalPlayerProgress.FriendsID)
			{
				PrefFriendData prefFriendData = new PrefFriendData();
				prefFriendData.Name = item.Name;
				prefFriendData.accountID = item.accountID;
				prefFriendData.id = item.id;
				prefFriendData.OrderNumber = num;
				utilityLocal.AddFriend(prefFriendData);
				num++;
			}
		}

		public static FriendsCollection GetFriendItems(PlayerDatas.FriendsUtility friendsUtility)
		{
			FriendsCollection friendsCollection = new FriendsCollection();
			List<PrefFriendItem> list = new List<PrefFriendItem>();
			foreach (PrefFriendData friend in friendsUtility.Friends)
			{
				PrefFriendItem prefFriendItem = new PrefFriendItem();
				prefFriendItem.id = friend.id;
				prefFriendItem.Name = friend.Name;
				prefFriendItem.accountID = friend.accountID;
				list.Add(prefFriendItem);
			}
			friendsCollection.FriendsID = list;
			return friendsCollection;
		}
	}
}

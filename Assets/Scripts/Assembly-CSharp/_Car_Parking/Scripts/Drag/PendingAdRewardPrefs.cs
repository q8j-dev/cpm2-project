using System;
using CPM.Ads.Policy;
using UnityEngine;

namespace _Car_Parking.Scripts.Drag
{
	public static class PendingAdRewardPrefs
	{
		private const string KeyPrefix = "PendingAdReward_";

		private const char Separator = ':';

		private const string AdIdSuffix = "_AdId";

		private const string EngineKey = "PendingAdReward_EngineTimerReduce_Engine";

		public static void Save(AdPlacement placement, int slotId, int carId)
		{
			PlayerPrefs.SetString("PendingAdReward_" + placement, $"{slotId}{':'}{carId}");
			PlayerPrefs.Save();
		}

		public static bool TryGet(AdPlacement placement, out int slotId, out int carId)
		{
			slotId = -1;
			carId = -1;
			string text = PlayerPrefs.GetString("PendingAdReward_" + placement, string.Empty);
			if (string.IsNullOrEmpty(text))
			{
				return false;
			}
			string[] array = text.Split(':');
			if (array.Length == 2 && int.TryParse(array[0], out slotId))
			{
				return int.TryParse(array[1], out carId);
			}
			return false;
		}

		public static void Clear(AdPlacement placement)
		{
			PlayerPrefs.DeleteKey("PendingAdReward_" + placement);
			ClearAdId(placement);
		}

		public static void SaveAdId(AdPlacement placement, string adId)
		{
			if (!string.IsNullOrEmpty(adId))
			{
				PlayerPrefs.SetString("PendingAdReward_" + placement.ToString() + "_AdId", adId);
				PlayerPrefs.Save();
			}
		}

		public static string GetAdId(AdPlacement placement)
		{
			string text = PlayerPrefs.GetString("PendingAdReward_" + placement.ToString() + "_AdId", string.Empty);
			if (!string.IsNullOrEmpty(text))
			{
				return text;
			}
			return null;
		}

		public static void ClearAdId(AdPlacement placement)
		{
			PlayerPrefs.DeleteKey("PendingAdReward_" + placement.ToString() + "_AdId");
		}

		public static void SaveEngine(int engineId)
		{
			PlayerPrefs.SetInt("PendingAdReward_EngineTimerReduce_Engine", engineId);
			PlayerPrefs.Save();
		}

		public static bool TryGetEngine(out int engineId)
		{
			engineId = PlayerPrefs.GetInt("PendingAdReward_EngineTimerReduce_Engine", -1);
			return engineId > 0;
		}

		public static void ClearEngine()
		{
			PlayerPrefs.DeleteKey("PendingAdReward_EngineTimerReduce_Engine");
			ClearAdId(AdPlacement.EngineTimerReduce);
		}

		public static void SaveRaceMode(RacingRewardMode mode)
		{
			if (mode == RacingRewardMode.None)
			{
				Clear(AdPlacement.Competition);
				return;
			}
			PlayerPrefs.SetString("PendingAdReward_" + AdPlacement.Competition, mode.ToString());
			PlayerPrefs.Save();
		}

		public static RacingRewardMode GetRaceMode()
		{
			string value = PlayerPrefs.GetString("PendingAdReward_" + AdPlacement.Competition, string.Empty);
			if (string.IsNullOrEmpty(value))
			{
				return RacingRewardMode.None;
			}
			if (!Enum.TryParse<RacingRewardMode>(value, out var result) || !Enum.IsDefined(typeof(RacingRewardMode), result))
			{
				return RacingRewardMode.None;
			}
			return result;
		}
	}
}

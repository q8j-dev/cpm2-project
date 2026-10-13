using System;
using System.Collections.Generic;
using CPM.Database;
using CPM.Scripts.VinylDrawer;
using CodeStage.AntiCheat.Storage;
using DataEntities.Vinyl;
using Newtonsoft.Json;

namespace _Car_Parking.Scripts.VinylsAi
{
	public static class VinylsCensorSaver
	{
		public static void OnVinylUpdated(this CarInfo carInfo)
		{
			List<VinylData> list = new List<VinylData>();
			foreach (VinylsType value in Enum.GetValues(typeof(VinylsType)))
			{
				List<VinylData> allVinyls = new List<VinylData>();
				if (carInfo.TryGetCarVinyls(value, out allVinyls))
				{
					list.AddRange(allVinyls);
				}
			}
			if (list.Count > 0)
			{
				int hashCode = JsonConvert.SerializeObject(list).GetHashCode();
				if (ObscuredPrefs.GetInt(GetHashPrefsKey(carInfo)) != hashCode)
				{
					ObscuredPrefs.SetInt(GetHashPrefsKey(carInfo), hashCode);
					carInfo.SetNeedToCheckAdultVinyls(value: true);
				}
			}
			else
			{
				carInfo.SetNeedToCheckAdultVinyls(value: false);
			}
		}

		public static void SetNeedToCheckAdultVinyls(this CarInfo carInfo, bool value)
		{
			ObscuredPrefs.SetBool(GetCheckAdultVinylsPrefsKey(carInfo), value);
		}

		public static bool GetNeedToCheckAdultVinyls(this CarInfo carInfo)
		{
			return ObscuredPrefs.GetBool(GetCheckAdultVinylsPrefsKey(carInfo), defaultValue: true);
		}

		public static bool GetHaveAdultVinyls(this CarInfo carInfo)
		{
			return ObscuredPrefs.GetBool(GetHaveAdultVinylsPrefsKey(carInfo));
		}

		public static void SetHaveAdultVinyls(this CarInfo carInfo, bool value)
		{
			ObscuredPrefs.SetBool(GetHaveAdultVinylsPrefsKey(carInfo), value);
		}

		public static void ClearLocalData(string carInstanceId)
		{
			ObscuredPrefs.DeleteKey(GetCheckAdultVinylsPrefsKey(carInstanceId));
			ObscuredPrefs.DeleteKey(GetHaveAdultVinylsPrefsKey(carInstanceId));
			ObscuredPrefs.DeleteKey(GetHashPrefsKey(carInstanceId));
		}

		private static string GetCheckAdultVinylsPrefsKey(CarInfo carInfo)
		{
			return GetCheckAdultVinylsPrefsKey(carInfo.CarInstanceId);
		}

		private static string GetHaveAdultVinylsPrefsKey(CarInfo carInfo)
		{
			return GetHaveAdultVinylsPrefsKey(carInfo.CarInstanceId);
		}

		private static string GetHashPrefsKey(CarInfo carInfo)
		{
			return GetHashPrefsKey(carInfo.CarInstanceId);
		}

		private static string GetCheckAdultVinylsPrefsKey(string carInstanceId)
		{
			return carInstanceId + "CheckAdultVinyls";
		}

		private static string GetHaveAdultVinylsPrefsKey(string carInstanceId)
		{
			return carInstanceId + "HaveAdultVinyls";
		}

		private static string GetHashPrefsKey(string carInstanceId)
		{
			return carInstanceId + "HashVinyls";
		}
	}
}

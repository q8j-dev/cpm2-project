using System.Collections.Generic;
using System.Linq;
using CodeStage.AntiCheat.Storage;
using UnityEngine;
using _Car_Parking.Scripts.CommonUtility;

namespace _Car_Parking.Scripts.Other.FsoStrobeLights
{
	[CreateAssetMenu(fileName = "FsoStrobeData", menuName = "Scriptable/TuningLists/FsoStrobeLightsData", order = 2)]
	public class FsoStrobeLightsConfig : ScriptableObject
	{
		private const string WasBoughtPrefsKey = "WasFsoStrobeBought";

		private const string ToSetPrefsKey = "FsoToSet";

		private const string PositionToSetPrefsKey = "PosFsoToSet";

		private const string ColorLeftToSetPrefsKey = "ColorFsoToSet";

		private const string ColorRightToSetPrefsKey = "ColorFsoToSetRight";

		public const string FsoConfigPath = "Configs/FsoStrobeLights/FsoStrobeData";

		public static int[] DefaultFsoData = new int[8] { -1, 0, 255, 255, 255, 255, 255, 255 };

		public List<OneFsoItemData> data = new List<OneFsoItemData>();

		public int[] paidFso;

		public GameObject itemPrefab;

		public int defaultPrice = 100;

		[Header("Visual")]
		public float FsoColorHdrIntencity = 3f;

		public static bool GetFsoBoughtPrefs(int itemId)
		{
			return ObscuredPrefs.GetBool("WasFsoStrobeBought" + itemId);
		}

		public static void SetFsoBoughtPrefs(int itemId, bool value)
		{
			ObscuredPrefs.SetBool("WasFsoStrobeBought" + itemId, value);
		}

		public static FsoStrobeLightsConfig LoadConfigFromResources()
		{
			return (FsoStrobeLightsConfig)Resources.Load("Configs/FsoStrobeLights/FsoStrobeData");
		}

		public static List<int> GetBoughtFso()
		{
			List<int> list = new List<int>();
			foreach (OneFsoItemData datum in LoadConfigFromResources().data)
			{
				if (GetFsoBoughtPrefs(datum.index))
				{
					list.Add(datum.index);
				}
			}
			return list;
		}

		public static void SetBoughtFso(List<int> boughtFso)
		{
			if (boughtFso == null || boughtFso.Count == 0)
			{
				return;
			}
			foreach (int item in boughtFso)
			{
				SetFsoBoughtPrefs(item, value: true);
			}
		}

		public int GetPrice(int lastSelectedIndex)
		{
			OneFsoItemData oneFsoItemData = data.Find((OneFsoItemData x) => x.index == lastSelectedIndex);
			if (oneFsoItemData == null)
			{
				return defaultPrice;
			}
			return oneFsoItemData.price;
		}

		public static int LegacyGetFsoPrefs(int carID)
		{
			return ObscuredPrefs.GetInt("FsoToSet" + carID, -1);
		}

		public static int LegacyGetFsoPositionPrefs(int carID)
		{
			return ObscuredPrefs.GetInt("PosFsoToSet" + carID);
		}

		public static Color LegacyGetFsoColorPrefsLeft(int carId)
		{
			return ObscuredPrefs.GetColor("ColorFsoToSet" + carId, Color.white);
		}

		public static Color LegacyGetFsoColorPrefsRight(int carId)
		{
			return ObscuredPrefs.GetColor("ColorFsoToSetRight" + carId, LegacyGetFsoColorPrefsLeft(carId));
		}

		public static int[] ResizeFsoData(int[] fsoData)
		{
			List<int> list = fsoData.ToList();
			if (fsoData.Length > 2)
			{
				list.Resize2(8, 255);
			}
			else
			{
				list.Resize2(8, 0);
			}
			return list.ToArray();
		}

		public static void SetFsoColorPrefsLegacy(int carId, Color colorLeft, Color colorRight)
		{
			Color32 value = colorLeft;
			Color32 value2 = colorRight;
			ObscuredPrefs.SetColor("ColorFsoToSet" + carId, value);
			ObscuredPrefs.SetColor("ColorFsoToSetRight" + carId, value2);
		}

		public static void SetFsoPositionPrefsLegacy(int carID, int value)
		{
			ObscuredPrefs.SetInt("PosFsoToSet" + carID, value);
		}

		public static void SetFsoPrefsLegacy(int carID, int value)
		{
			ObscuredPrefs.SetInt("FsoToSet" + carID, value);
		}
	}
}

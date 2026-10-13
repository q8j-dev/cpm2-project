using System.Collections.Generic;
using CodeStage.AntiCheat.Storage;
using Flags;
using Newtonsoft.Json;
using UnityEngine;
using _Car_Parking.Scripts.Database.CarFactories.Constructors;

namespace CPM.Database.Constructors
{
	public class FlagConstructor
	{
		private static FlagsDataSO _flagsDataSo;

		public const string FlagsTuningPath = "Configs/VisualTuning/FlagsData";

		public static Dictionary<int, int> FlagsBought = new Dictionary<int, int>();

		public GameObject Flag { get; set; }

		public Cloth FlagCloth { get; set; }

		public void SetFlag(VehicleData vehicleData, int flagId)
		{
			if (!_flagsDataSo)
			{
				_flagsDataSo = Resources.Load<FlagsDataSO>("Configs/VisualTuning/FlagsData");
			}
			if (vehicleData.flagPoint == null)
			{
				return;
			}
			if (flagId == 0)
			{
				vehicleData.flagPoint.gameObject.SetActive(value: false);
				return;
			}
			if (Flag == null)
			{
				GameObject original = Resources.Load<GameObject>("Flags/flag");
				Flag = Object.Instantiate(original, vehicleData.flagPoint);
				FlagCloth = Flag.GetComponentInChildren<Cloth>();
				if (vehicleData.IsMineCar)
				{
					Flag.AddComponent<FlagPhysic>().Initialize(vehicleData.GetComponent<Rigidbody>(), FlagCloth);
				}
			}
			Material material = Flag.transform.GetChild(1).GetComponent<SkinnedMeshRenderer>().material;
			material.mainTexture = _flagsDataSo.textures[flagId];
			material.SetFloat("_OcclusionStrength", 0.5f);
			vehicleData.flagPoint.gameObject.SetActive(value: true);
		}

		public static int GetCoinPriceForFlag()
		{
			return 100;
		}

		public static Dictionary<int, int> GetFlagsData()
		{
			string text = ObscuredPrefs.GetString("flags");
			FlagsBought = ((text == "") ? new Dictionary<int, int>() : JsonConvert.DeserializeObject<Dictionary<int, int>>(text));
			return FlagsBought;
		}

		public static void AddFlag(int flagID)
		{
			if (flagID != -1)
			{
				if (FlagsBought == null)
				{
					GetFlagsData();
				}
				if (!FlagsBought.ContainsKey(flagID))
				{
					FlagsBought[flagID] = 0;
				}
				FlagsBought[flagID]++;
			}
		}

		public static List<FlagData> getFlagIDs()
		{
			List<FlagData> result = new List<FlagData>();
			ObscuredPrefs.GetString("flags");
			return result;
		}

		public static void SaveFlagsFromDatabase(Dictionary<int, int> flagsBought)
		{
			string value = ((flagsBought == null) ? "" : JsonConvert.SerializeObject(flagsBought, Formatting.Indented));
			ObscuredPrefs.SetString("flags", value);
		}

		public static void SaveBoughtFlags()
		{
			string value = JsonConvert.SerializeObject(FlagsBought);
			ObscuredPrefs.SetString("flags", value);
		}

		public static void Clear()
		{
			if (FlagsBought != null)
			{
				FlagsBought.Clear();
				FlagsBought = null;
			}
		}
	}
}

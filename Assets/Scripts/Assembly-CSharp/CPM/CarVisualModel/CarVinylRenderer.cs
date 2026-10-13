using UnityEngine;
using _Car_Parking.Scripts.EngineTuning;

namespace CPM.CarVisualModel
{
	public static class CarVinylRenderer
	{
		public static bool TryLoadDefaultPreset(int carId, out OneVinylsPreset preset)
		{
			AllVinylPreset allVinylPreset = Resources.Load<AllVinylPreset>($"LiveryPresets/PresetLiveryCar{carId}");
			int defaultStyle = -1;
			VehicleStockTuningData vehicleStockTuningData = Resources.Load<VehicleStockTuningData>(SellCarValues.GetVehicleStockTuningDataPath(carId));
			if ((bool)vehicleStockTuningData)
			{
				defaultStyle = vehicleStockTuningData.DefaultStyle;
			}
			else
			{
				_ = (bool)allVinylPreset;
			}
			preset = SelectDefaultPreset(allVinylPreset, defaultStyle);
			return preset != null;
		}

		public static OneVinylsPreset SelectDefaultPreset(AllVinylPreset all, int defaultStyle)
		{
			if (defaultStyle < 0 || !all || all.allPresetList == null || all.allPresetList.Count == 0)
			{
				return null;
			}
			int index = Mathf.Clamp(defaultStyle, 0, all.allPresetList.Count - 1);
			return all.allPresetList[index];
		}

		public static int BodyVinylCount(OneVinylsPreset preset)
		{
			return (preset?.vynilOneCarList?.allVynils?.Count).GetValueOrDefault();
		}

		public static int WindowVinylCount(OneVinylsPreset preset)
		{
			return (preset?.windowVynilOneCarList?.allVynils?.Count).GetValueOrDefault();
		}

		public static bool HasCarVinyls(int carId)
		{
			if (TryLoadDefaultPreset(carId, out var preset))
			{
				return BodyVinylCount(preset) > 0;
			}
			return false;
		}

		public static bool HasWindowVinyls(int carId)
		{
			if (TryLoadDefaultPreset(carId, out var preset))
			{
				return WindowVinylCount(preset) > 0;
			}
			return false;
		}
	}
}

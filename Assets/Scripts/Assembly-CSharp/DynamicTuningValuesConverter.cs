using System;
using CPMEngine;
using UnityEngine;
using _Car_Parking.Scripts.Multiplayer.MultiplayerData.Other;

public static class DynamicTuningValuesConverter
{
	private const int CountBeforeGears = 27;

	public static object[] ToArray(DynamicTuningSettings data)
	{
		object[] array = new object[27 + data.Gears.Length];
		array[0] = data.Boost;
		array[1] = data.TorqueSplit;
		array[2] = data.FinalDrive;
		array[3] = data.LaunchTime;
		array[4] = data.LaunchRpm;
		array[5] = data.FrontAccelBias;
		array[6] = data.FrontBrakeBias;
		array[7] = data.FrontMaxBias;
		array[8] = data.FrontTorqueBiasRatio;
		array[9] = data.RearAccelBias;
		array[10] = data.RearBrakeBias;
		array[11] = data.RearMaxBias;
		array[12] = data.RearTorqueBiasRatio;
		array[13] = data.RedlineRpm;
		array[14] = data.IdleRpm;
		array[15] = data.MixtureRatio;
		array[16] = (int)data.FuelType;
		array[17] = data.MaxSteeringAngle;
		array[18] = data.FrontLsdLock;
		array[19] = data.RearLsdLock;
		array[20] = data.MinFrontSuspensionHeight;
		array[21] = data.MaxFrontSuspensionHeight;
		array[22] = data.MinRearSuspensionHeight;
		array[23] = data.MaxRearSuspensionHeight;
		array[24] = data.HeightAdjustSpeed;
		array[25] = data.CurrentFrontSuspensionHeight;
		array[26] = data.CurrentRearSuspensionHeight;
		AddGearsAsArray(array, data.Gears, 27);
		return array;
	}

	public static DynamicTuningSettings Get(object dataObject)
	{
		DynamicTuningSettings dynamicTuningSettings = new DynamicTuningSettings();
		object[] array = ConverterUtils.TryGetObjectArray(dataObject);
		dynamicTuningSettings.Boost = Convert.ToSingle(array[0]);
		dynamicTuningSettings.TorqueSplit = Convert.ToSingle(array[1]);
		dynamicTuningSettings.FinalDrive = Convert.ToSingle(array[2]);
		dynamicTuningSettings.LaunchTime = Convert.ToSingle(array[3]);
		dynamicTuningSettings.LaunchRpm = Convert.ToSingle(array[4]);
		dynamicTuningSettings.FrontAccelBias = Convert.ToSingle(array[5]);
		dynamicTuningSettings.FrontBrakeBias = Convert.ToSingle(array[6]);
		dynamicTuningSettings.FrontMaxBias = Convert.ToSingle(array[7]);
		dynamicTuningSettings.FrontTorqueBiasRatio = Convert.ToSingle(array[8]);
		dynamicTuningSettings.RearAccelBias = Convert.ToSingle(array[9]);
		dynamicTuningSettings.RearBrakeBias = Convert.ToSingle(array[10]);
		dynamicTuningSettings.RearMaxBias = Convert.ToSingle(array[11]);
		dynamicTuningSettings.RearTorqueBiasRatio = Convert.ToSingle(array[12]);
		dynamicTuningSettings.RedlineRpm = Convert.ToSingle(array[13]);
		dynamicTuningSettings.IdleRpm = Convert.ToSingle(array[14]);
		dynamicTuningSettings.MixtureRatio = Convert.ToSingle(array[15]);
		dynamicTuningSettings.FuelType = (FuelType)Convert.ToInt32(array[16]);
		dynamicTuningSettings.MaxSteeringAngle = Convert.ToInt32(array[17]);
		dynamicTuningSettings.FrontLsdLock = Convert.ToSingle(array[18]);
		dynamicTuningSettings.RearLsdLock = Convert.ToSingle(array[19]);
		dynamicTuningSettings.MinFrontSuspensionHeight = Convert.ToSingle(array[20]);
		dynamicTuningSettings.MaxFrontSuspensionHeight = Convert.ToSingle(array[21]);
		dynamicTuningSettings.MinRearSuspensionHeight = Convert.ToSingle(array[22]);
		dynamicTuningSettings.MaxRearSuspensionHeight = Convert.ToSingle(array[23]);
		dynamicTuningSettings.HeightAdjustSpeed = Convert.ToSingle(array[24]);
		dynamicTuningSettings.CurrentFrontSuspensionHeight = Convert.ToSingle(array[25]);
		dynamicTuningSettings.CurrentRearSuspensionHeight = Convert.ToSingle(array[26]);
		dynamicTuningSettings.Gears = GetGearsFromArray(array, 27);
		return dynamicTuningSettings;
	}

	private static void AddGearsAsArray(object[] data, float[] gears, int countBeforeGears)
	{
		for (int i = 0; i < gears.Length; i++)
		{
			data[countBeforeGears + i] = gears[i];
		}
	}

	private static float[] GetGearsFromArray(object[] data, int countBeforeGears)
	{
		try
		{
			float[] array = new float[data.Length - countBeforeGears];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = Convert.ToSingle(data[countBeforeGears + i]);
			}
			return array;
		}
		catch (Exception ex)
		{
			Debug.LogError("Can't parse gears, so set default: " + ex.Message);
			return new float[14]
			{
				-1f, -1f, -1f, -1f, -1f, -1f, -1f, -1f, -1f, -1f,
				-1f, -1f, -1f, -1f
			};
		}
	}
}

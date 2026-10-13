using System;
using CPM.Transmission;
using CPMEngine;
using UnityEngine;
using _Car_Parking.Scripts.Drag.Engine;

public static class CarResultConverter
{
	public static object[] ToArray(VehicleDataResult data)
	{
		return new object[41]
		{
			data.Torque,
			FrontWheelDataConverter.ToArray(data.FrontWheel),
			RearWheelDataConverter.ToArray(data.RearWheel),
			MaintenanceDataConverter.ToArray(data.MaintenanceData),
			(int)data.GearBoxType,
			(int)data.TurboType,
			data.CheckEngine,
			data.IsBroken,
			data.Displacement,
			data.IdleRpm,
			data.RedlineRpm,
			data.MaxRpm,
			data.EngineInertia,
			data.EngineBrakeInertia,
			(int)data.Kind,
			data.GearRatios,
			LSDValuesConverter.ToArray(data.FrontLsd),
			LSDValuesConverter.ToArray(data.RearLsd),
			data.GearShiftDelay,
			data.EndRatio,
			data.GearboxInertia,
			data.TorqueSplit,
			data.MaxClutchF,
			data.MaxSteeringAngle,
			data.HaveLc,
			data.LcRpm,
			data.LcTime,
			data.TurboPressure,
			data.MinTurboRpm,
			data.MaxTurboRpm,
			data.FuelConsumptionAtCostantSpeed,
			data.FuelConsumptionSpeed,
			data.Weight,
			data.EngineId,
			(int)data.EngineType,
			data.ExhaustType,
			data.BlowOffType,
			data.SuperChargerType,
			data.BurbleType,
			(int)data.AutoModes,
			data.LowMaxGear
		};
	}

	public static VehicleDataResult Get(object dataObject)
	{
		VehicleDataResult result = default;
		object[] array = (object[])dataObject;
		bool logged = false;
		result.Torque = Ref<TorqueCurveSample[]>(array, 0, ref logged);
		result.FrontWheel = FrontWheelDataConverter.Get(array[1]);
		result.RearWheel = RearWheelDataConverter.Get(array[2]);
		result.MaintenanceData = MaintenanceDataConverter.Get(array[3]);
		result.GearBoxType = Val<GearboxType>(array, 4, ref logged);
		result.TurboType = Val<TurboType>(array, 5, ref logged);
		result.CheckEngine = Val<bool>(array, 6, ref logged);
		result.IsBroken = Val<bool>(array, 7, ref logged);
		result.Displacement = Val<float>(array, 8, ref logged);
		result.IdleRpm = Val<float>(array, 9, ref logged);
		result.RedlineRpm = Val<float>(array, 10, ref logged);
		result.MaxRpm = Val<float>(array, 11, ref logged);
		result.EngineInertia = Val<float>(array, 12, ref logged);
		result.EngineBrakeInertia = Val<float>(array, 13, ref logged);
		result.Kind = ReadKind(array, 14, ref logged);
		result.GearRatios = Ref<float[]>(array, 15, ref logged);
		result.FrontLsd = LSDValuesConverter.Get(array[16]);
		result.RearLsd = LSDValuesConverter.Get(array[17]);
		result.GearShiftDelay = Val<float>(array, 18, ref logged);
		result.EndRatio = Val<float>(array, 19, ref logged);
		result.GearboxInertia = Val<float>(array, 20, ref logged);
		result.TorqueSplit = Val<float>(array, 21, ref logged);
		result.MaxClutchF = Val<float>(array, 22, ref logged);
		result.MaxSteeringAngle = Val<float>(array, 23, ref logged);
		result.HaveLc = Val<bool>(array, 24, ref logged);
		result.LcRpm = Val<float>(array, 25, ref logged);
		result.LcTime = Val<float>(array, 26, ref logged);
		result.TurboPressure = Val<int>(array, 27, ref logged);
		result.MinTurboRpm = Val<float>(array, 28, ref logged);
		result.MaxTurboRpm = Val<float>(array, 29, ref logged);
		result.FuelConsumptionAtCostantSpeed = Val<float>(array, 30, ref logged);
		result.FuelConsumptionSpeed = Val<float>(array, 31, ref logged);
		result.Weight = Val<float>(array, 32, ref logged);
		result.EngineId = Val<int>(array, 33, ref logged);
		result.EngineType = Val<int>(array, 34, ref logged);
		result.ExhaustType = Val<int>(array, 35, ref logged);
		result.BlowOffType = Val<int>(array, 36, ref logged);
		result.SuperChargerType = Val<int>(array, 37, ref logged);
		result.BurbleType = Val<int>(array, 38, ref logged);
		if (array.Length > 40)
		{
			result.AutoModes = Val<AutoShifterModes>(array, 39, ref logged);
			result.LowMaxGear = Val<int>(array, 40, ref logged);
		}
		return result;
	}

	private static GearboxKind ReadKind(object[] data, int index, ref bool logged)
	{
		object obj = ((index < data.Length) ? data[index] : null);
		if (obj is bool)
		{
			if (!(bool)obj)
			{
				return GearboxKind.AT;
			}
			return GearboxKind.MT;
		}
		return Val<GearboxKind>(data, index, ref logged);
	}

	private static T Val<T>(object[] data, int index, ref bool logged) where T : struct
	{
		try
		{
			return (T)data[index];
		}
		catch (Exception e)
		{
			LogMismatch(data, index, typeof(T), e, ref logged);
			return default;
		}
	}

	private static T Ref<T>(object[] data, int index, ref bool logged) where T : class
	{
		try
		{
			return (T)data[index];
		}
		catch (Exception e)
		{
			LogMismatch(data, index, typeof(T), e, ref logged);
			return null;
		}
	}

	private static void LogMismatch(object[] data, int index, Type expected, Exception e, ref bool logged)
	{
		if (!logged)
		{
			logged = true;
			object obj = ((index < data.Length) ? data[index] : null);
			string arg = ((obj != null) ? obj.GetType().Name : "null");
			Debug.LogError($"[CarResultConverter] Layout mismatch at data[{index}]: expected {expected.Name}, got {arg} " + $"(len={data.Length}). Likely an older-build peer; falling back to defaults. {e.Message}");
		}
	}
}

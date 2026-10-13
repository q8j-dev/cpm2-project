using System;
using System.Collections.Generic;
using CPM.Database;
using _Car_Parking.Scripts.Car.CarSetter;
using _Car_Parking.Scripts.CommonUtility.Contracts;
using _Car_Parking.Scripts.Drag.Engine;
using _Car_Parking.Scripts.EngineTuning;
using _Car_Parking.Scripts.EngineTuning.DataTypes;

public static class PhotonConvertersGeneratorTypes
{
	public static readonly List<Type> TypeToGenerate = new List<Type>
	{
		typeof(VehicleDataResult),
		typeof(FrontWheelData),
		typeof(RearWheelData),
		typeof(CarPaintColor),
		typeof(InstalledBodyKits),
		typeof(Colors),
		typeof(AdditionalData),
		typeof(FsoData),
		typeof(MultiplayerDataSending.CarVinyls),
		typeof(SteerSuspensionData),
		typeof(SuspensionData),
		typeof(BrakesData),
		typeof(MaintenanceInfo),
		typeof(CarTuningData),
		typeof(DynamicTuningSettings),
		typeof(EngineKitData),
		typeof(PartData),
		typeof(LSDValues)
	};
}

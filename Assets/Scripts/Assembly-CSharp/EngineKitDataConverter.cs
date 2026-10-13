using _Car_Parking.Scripts.EngineTuning;
using _Car_Parking.Scripts.Multiplayer.MultiplayerData.Other;

public static class EngineKitDataConverter
{
	public static object[] ToArray(EngineKitData data)
	{
		return new object[25]
		{
			PartDataConverter.ToArray(data.EngineId),
			PartDataConverter.ToArray(data.Block),
			PartDataConverter.ToArray(data.CylinderHead),
			PartDataConverter.ToArray(data.IntakeManifold),
			PartDataConverter.ToArray(data.AirFilter),
			PartDataConverter.ToArray(data.Charger),
			PartDataConverter.ToArray(data.ExhaustHeader),
			PartDataConverter.ToArray(data.ExhaustPipe),
			PartDataConverter.ToArray(data.Crankshaft),
			PartDataConverter.ToArray(data.ConnectionRod),
			PartDataConverter.ToArray(data.Pistons),
			PartDataConverter.ToArray(data.Flywheel),
			PartDataConverter.ToArray(data.Camshaft1),
			PartDataConverter.ToArray(data.Camshaft2),
			PartDataConverter.ToArray(data.FuelRail),
			PartDataConverter.ToArray(data.FuelInjectors),
			PartDataConverter.ToArray(data.ThrottleValve),
			PartDataConverter.ToArray(data.FuelPump),
			PartDataConverter.ToArray(data.Radiator),
			PartDataConverter.ToArray(data.Intercooler),
			PartDataConverter.ToArray(data.Clutch),
			PartDataConverter.ToArray(data.RotorHousing),
			PartDataConverter.ToArray(data.Rotors),
			PartDataConverter.ToArray(data.RotorTrackingGear),
			PartDataConverter.ToArray(data.EccentricShaft)
		};
	}

	public static EngineKitData Get(object dataObject)
	{
		EngineKitData engineKitData = new EngineKitData();
		object[] array = ConverterUtils.TryGetObjectArray(dataObject);
		engineKitData.EngineId = PartDataConverter.Get(array[0]);
		engineKitData.Block = PartDataConverter.Get(array[1]);
		engineKitData.CylinderHead = PartDataConverter.Get(array[2]);
		engineKitData.IntakeManifold = PartDataConverter.Get(array[3]);
		engineKitData.AirFilter = PartDataConverter.Get(array[4]);
		engineKitData.Charger = PartDataConverter.Get(array[5]);
		engineKitData.ExhaustHeader = PartDataConverter.Get(array[6]);
		engineKitData.ExhaustPipe = PartDataConverter.Get(array[7]);
		engineKitData.Crankshaft = PartDataConverter.Get(array[8]);
		engineKitData.ConnectionRod = PartDataConverter.Get(array[9]);
		engineKitData.Pistons = PartDataConverter.Get(array[10]);
		engineKitData.Flywheel = PartDataConverter.Get(array[11]);
		engineKitData.Camshaft1 = PartDataConverter.Get(array[12]);
		engineKitData.Camshaft2 = PartDataConverter.Get(array[13]);
		engineKitData.FuelRail = PartDataConverter.Get(array[14]);
		engineKitData.FuelInjectors = PartDataConverter.Get(array[15]);
		engineKitData.ThrottleValve = PartDataConverter.Get(array[16]);
		engineKitData.FuelPump = PartDataConverter.Get(array[17]);
		engineKitData.Radiator = PartDataConverter.Get(array[18]);
		engineKitData.Intercooler = PartDataConverter.Get(array[19]);
		engineKitData.Clutch = PartDataConverter.Get(array[20]);
		engineKitData.RotorHousing = PartDataConverter.Get(array[21]);
		engineKitData.Rotors = PartDataConverter.Get(array[22]);
		engineKitData.RotorTrackingGear = PartDataConverter.Get(array[23]);
		engineKitData.EccentricShaft = PartDataConverter.Get(array[24]);
		return engineKitData;
	}
}

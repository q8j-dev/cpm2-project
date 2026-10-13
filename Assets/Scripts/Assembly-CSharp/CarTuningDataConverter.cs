using _Car_Parking.Scripts.EngineTuning;
using _Car_Parking.Scripts.Multiplayer.MultiplayerData.Other;

public static class CarTuningDataConverter
{
	public static object[] ToArray(CarTuningData data)
	{
		return new object[6]
		{
			EngineKitDataConverter.ToArray(data.EngineKitData),
			PartDataConverter.ToArray(data.ECU),
			PartDataConverter.ToArray(data.GearBox),
			PartDataConverter.ToArray(data.FrontLSD),
			PartDataConverter.ToArray(data.RearLSD),
			DynamicTuningValuesConverter.ToArray(data.DynamicTuningSettings)
		};
	}

	public static CarTuningData Get(object dataObject)
	{
		CarTuningData carTuningData = new CarTuningData();
		object[] array = ConverterUtils.TryGetObjectArray(dataObject);
		carTuningData.EngineKitData = EngineKitDataConverter.Get(array[0]);
		carTuningData.ECU = PartDataConverter.Get(array[1]);
		carTuningData.GearBox = PartDataConverter.Get(array[2]);
		carTuningData.FrontLSD = PartDataConverter.Get(array[3]);
		carTuningData.RearLSD = PartDataConverter.Get(array[4]);
		carTuningData.DynamicTuningSettings = DynamicTuningValuesConverter.Get(array[5]);
		return carTuningData;
	}
}

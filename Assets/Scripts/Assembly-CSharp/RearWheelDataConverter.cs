using CPMEngine;

public static class RearWheelDataConverter
{
	public static object[] ToArray(RearWheelData data)
	{
		return new object[12]
		{
			SuspensionDataConverter.ToArray(data.SuspensionData),
			BrakesDataConverter.ToArray(data.BrakesData),
			data.Pressure,
			(int)data.tyreType,
			data.RimModelId,
			(int)data.wheelSize,
			(int)data.tyreWidth,
			(int)data.tyreProfile,
			(int)data.rimWidth,
			data.SpacerWidth,
			CarPaintColorConverter.ToArray(data.Color1),
			CarPaintColorConverter.ToArray(data.Color2)
		};
	}

	public static RearWheelData Get(object dataObject)
	{
		RearWheelData rearWheelData = new RearWheelData();
		object[] array = (object[])dataObject;
		rearWheelData.SuspensionData = SuspensionDataConverter.Get(array[0]);
		rearWheelData.BrakesData = BrakesDataConverter.Get(array[1]);
		rearWheelData.Pressure = (float)array[2];
		rearWheelData.tyreType = (TyreType)array[3];
		rearWheelData.RimModelId = (int)array[4];
		rearWheelData.wheelSize = (WheelDiameter)array[5];
		rearWheelData.tyreWidth = (TyreWidth)array[6];
		rearWheelData.tyreProfile = (TyreProfile)array[7];
		rearWheelData.rimWidth = (RimWidth)array[8];
		rearWheelData.SpacerWidth = (float)array[9];
		rearWheelData.Color1 = CarPaintColorConverter.Get(array[10]);
		rearWheelData.Color2 = CarPaintColorConverter.Get(array[11]);
		WheelData.Sanitize(rearWheelData);
		return rearWheelData;
	}
}

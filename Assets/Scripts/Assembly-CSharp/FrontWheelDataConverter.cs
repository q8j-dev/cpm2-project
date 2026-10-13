using CPMEngine;

public static class FrontWheelDataConverter
{
	public static object[] ToArray(FrontWheelData data)
	{
		return new object[12]
		{
			SteerSuspensionDataConverter.ToArray(data.SuspensionData),
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

	public static FrontWheelData Get(object dataObject)
	{
		FrontWheelData frontWheelData = new FrontWheelData();
		object[] array = (object[])dataObject;
		frontWheelData.SuspensionData = SteerSuspensionDataConverter.Get(array[0]);
		frontWheelData.BrakesData = BrakesDataConverter.Get(array[1]);
		frontWheelData.Pressure = (float)array[2];
		frontWheelData.tyreType = (TyreType)array[3];
		frontWheelData.RimModelId = (int)array[4];
		frontWheelData.wheelSize = (WheelDiameter)array[5];
		frontWheelData.tyreWidth = (TyreWidth)array[6];
		frontWheelData.tyreProfile = (TyreProfile)array[7];
		frontWheelData.rimWidth = (RimWidth)array[8];
		frontWheelData.SpacerWidth = (float)array[9];
		frontWheelData.Color1 = CarPaintColorConverter.Get(array[10]);
		frontWheelData.Color2 = CarPaintColorConverter.Get(array[11]);
		WheelData.Sanitize(frontWheelData);
		return frontWheelData;
	}
}

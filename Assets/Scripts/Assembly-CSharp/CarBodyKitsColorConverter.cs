using CPM.Database;
using _Car_Parking.Scripts.Drag;

public static class CarBodyKitsColorConverter
{
	public static object[] ToArray(BodyKitsColors data)
	{
		object[] array = new object[data.Colors.Count * 2];
		for (int i = 0; i < data.Colors.Count; i++)
		{
			array[i * 2] = (int)data.Colors[i].BodyKitName;
			array[i * 2 + 1] = CarPaintColorConverter.ToArray(data.Colors[i].Color);
		}
		return array;
	}

	public static BodyKitsColors Get(object dataObject)
	{
		if (dataObject == null)
		{
			return new BodyKitsColors();
		}
		BodyKitsColors bodyKitsColors = new BodyKitsColors();
		object[] array = (object[])dataObject;
		for (int i = 0; i < array.Length; i += 2)
		{
			BodyKitColor bodyKitColor = new BodyKitColor();
			bodyKitColor.BodyKitName = (BodyKitType)array[i];
			bodyKitColor.Color = CarPaintColorConverter.Get(array[i + 1]);
			bodyKitsColors.Colors.Add(bodyKitColor);
		}
		return bodyKitsColors;
	}
}

using UnityEngine;
using _Car_Parking.Scripts.Car.CarSetter;
using _Car_Parking.Scripts.Drag;

public static class CarPaintColorConverter
{
	public static object[] ToArray(CarPaintColor data)
	{
		return new object[4]
		{
			data.Color,
			(int)data.CarMatType,
			data.IridescenceThickness,
			data.IridescencePower
		};
	}

	public static CarPaintColor Get(object dataObject)
	{
		CarPaintColor result = default;
		object[] array = (object[])dataObject;
		result.Color = (Color)array[0];
		result.CarMatType = (CarMatType)array[1];
		result.IridescenceThickness = (float)array[2];
		result.IridescencePower = (float)array[3];
		return result;
	}
}

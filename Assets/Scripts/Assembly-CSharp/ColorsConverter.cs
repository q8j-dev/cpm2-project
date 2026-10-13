using System;
using CPM.Database;
using UnityEngine;

public static class ColorsConverter
{
	private enum ColorType
	{
		CarColor = 0,
		DirtColor = 1,
		DirtAmount = 2,
		WindowColor = 3,
		WindowSpecularColor = 4,
		DiodColor = 5,
		SmokeColor = 6,
		BodyKits = 7
	}

	public static object[] ToArray(Colors data)
	{
		object[] array = new object[Enum.GetValues(typeof(ColorType)).Length];
		array[0] = CarPaintColorConverter.ToArray(data.CarColor);
		array[1] = data.DirtColor;
		array[2] = data.DirtAmount;
		array[3] = data.WindowColor;
		array[4] = data.WindowSpecularColor;
		array[5] = data.DiodColor;
		array[6] = data.SmokeColor;
		array[7] = CarBodyKitsColorConverter.ToArray(data.BodyKits);
		return array;
	}

	public static Colors Get(object dataObject)
	{
		Colors colors = new Colors();
		object[] array = (object[])dataObject;
		colors.CarColor = CarPaintColorConverter.Get(array[0]);
		colors.DirtColor = (Color)array[1];
		colors.DirtAmount = (float)array[2];
		colors.WindowColor = (ColorSerializableWithAlpha)array[3];
		colors.WindowSpecularColor = (Color)array[4];
		colors.DiodColor = (Color)array[5];
		colors.SmokeColor = (Color)array[6];
		colors.BodyKits = CarBodyKitsColorConverter.Get(array[7]);
		return colors;
	}
}

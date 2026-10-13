using System;
using CPM.Database;
using UnityEngine;
using _Car_Parking.Scripts.Multiplayer.MultiplayerData;

public static class AdditionalDataConverter
{
	private enum AdditionalDataType
	{
		FlagId = 0,
		AccessoryId = 1,
		IsPolice = 2,
		WheelText = 3,
		WheelTextFont = 4,
		WheelTextFontStyle = 5,
		WheelTextColor = 6,
		Fso = 7,
		Police = 8,
		Ps = 9
	}

	public static object[] ToArray(AdditionalData data)
	{
		object[] array = new object[Enum.GetValues(typeof(AdditionalDataType)).Length];
		array[0] = data.FlagId;
		array[1] = data.AccessoryId;
		array[2] = data.IsPolice;
		array[3] = data.WheelText;
		array[4] = data.WheelTextFont;
		array[5] = data.WheelTextFontStyle;
		array[6] = data.WheelTextColor;
		array[7] = FsoDataConverter.ToArray(data.Fso);
		array[8] = PoliceDataConverter.ToArray(data.Police);
		array[9] = PsDataConverter.ToArray(data.AirSuspension);
		return array;
	}

	public static AdditionalData Get(object dataObject)
	{
		AdditionalData additionalData = new AdditionalData();
		object[] array = (object[])dataObject;
		additionalData.FlagId = (int)array[0];
		additionalData.AccessoryId = (int)array[1];
		additionalData.IsPolice = (bool)array[2];
		additionalData.WheelText = (string)array[3];
		additionalData.WheelTextFont = (int)array[4];
		additionalData.WheelTextFontStyle = (int)array[5];
		additionalData.WheelTextColor = (Color)array[6];
		additionalData.Fso = FsoDataConverter.Get(array[7]);
		if (array.Length > 8)
		{
			additionalData.Police = PoliceDataConverter.Get(array[8]);
		}
		if (array.Length > 9)
		{
			additionalData.AirSuspension = PsDataConverter.Get(array[9]);
		}
		return additionalData;
	}
}

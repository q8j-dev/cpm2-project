using System;
using _Car_Parking.Scripts.EngineTuning;
using _Car_Parking.Scripts.Multiplayer.MultiplayerData.Other;

public static class PartDataConverter
{
	public static object[] ToArray(PartData data)
	{
		if (data == null)
		{
			return null;
		}
		object[] obj = new object[2] { data.ModelId, null };
		float wear = data.Wear;
		obj[1] = wear;
		return obj;
	}

	public static PartData Get(object dataObject)
	{
		if (dataObject == null)
		{
			return null;
		}
		PartData partData = new PartData();
		object[] array = ConverterUtils.TryGetObjectArray(dataObject);
		partData.ModelId = Convert.ToInt32(array[0]);
		partData.Wear = Convert.ToSingle(array[1]);
		return partData.CheckPartData();
	}
}

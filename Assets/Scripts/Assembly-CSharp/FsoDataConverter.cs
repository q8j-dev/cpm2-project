using CPM.Database;
using UnityEngine;

public static class FsoDataConverter
{
	public static object[] ToArray(FsoData data)
	{
		return new object[4] { data.ModelIndex, data.PositionIndex, data.LeftColor, data.RightColor };
	}

	public static FsoData Get(object dataObject)
	{
		FsoData result = default;
		object[] array = (object[])dataObject;
		result.ModelIndex = (int)array[0];
		result.PositionIndex = (int)array[1];
		result.LeftColor = (Color)array[2];
		result.RightColor = (Color)array[3];
		return result;
	}
}

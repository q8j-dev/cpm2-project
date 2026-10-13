using CPMEngine;
using _Car_Parking.Scripts.Drag.Engine;

public static class LSDValuesConverter
{
	public static object[] ToArray(LSDValues data)
	{
		return new object[6]
		{
			(int)data.Type,
			data.AccelBias,
			data.BrakeBias,
			data.MaxBias,
			data.TorqueBiasRatio,
			data.LockPercent
		};
	}

	public static LSDValues Get(object dataObject)
	{
		LSDValues lSDValues = new LSDValues();
		object[] array = (object[])dataObject;
		lSDValues.Type = (LSDType)array[0];
		lSDValues.AccelBias = (float)array[1];
		lSDValues.BrakeBias = (float)array[2];
		lSDValues.MaxBias = (float)array[3];
		lSDValues.TorqueBiasRatio = (float)array[4];
		lSDValues.LockPercent = (float)array[5];
		return lSDValues;
	}
}

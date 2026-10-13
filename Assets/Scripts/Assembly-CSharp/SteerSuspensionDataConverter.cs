public static class SteerSuspensionDataConverter
{
	public static object[] ToArray(SteerSuspensionData data)
	{
		return new object[10] { data.Caster, data.Ackerman, data.MaxSteerAngle, data.SpringLength, data.SpringRate, data.Bump, data.Rebound, data.Camber, data.ToeIn, data.AntiSwayBarForce };
	}

	public static SteerSuspensionData Get(object dataObject)
	{
		SteerSuspensionData steerSuspensionData = new SteerSuspensionData();
		object[] array = (object[])dataObject;
		steerSuspensionData.Caster = (float)array[0];
		steerSuspensionData.Ackerman = (float)array[1];
		steerSuspensionData.MaxSteerAngle = (int)array[2];
		steerSuspensionData.SpringLength = (float)array[3];
		steerSuspensionData.SpringRate = (float)array[4];
		steerSuspensionData.Bump = (float)array[5];
		steerSuspensionData.Rebound = (float)array[6];
		steerSuspensionData.Camber = (float)array[7];
		steerSuspensionData.ToeIn = (float)array[8];
		steerSuspensionData.AntiSwayBarForce = (float)array[9];
		return steerSuspensionData;
	}
}

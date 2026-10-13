public static class SuspensionDataConverter
{
	public static object[] ToArray(SuspensionData data)
	{
		return new object[7] { data.SpringLength, data.SpringRate, data.Bump, data.Rebound, data.Camber, data.ToeIn, data.AntiSwayBarForce };
	}

	public static SuspensionData Get(object dataObject)
	{
		SuspensionData suspensionData = new SuspensionData();
		object[] array = (object[])dataObject;
		suspensionData.SpringLength = (float)array[0];
		suspensionData.SpringRate = (float)array[1];
		suspensionData.Bump = (float)array[2];
		suspensionData.Rebound = (float)array[3];
		suspensionData.Camber = (float)array[4];
		suspensionData.ToeIn = (float)array[5];
		suspensionData.AntiSwayBarForce = (float)array[6];
		return suspensionData;
	}
}

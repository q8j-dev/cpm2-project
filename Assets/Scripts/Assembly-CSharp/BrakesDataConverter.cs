using CPMEngine;

public static class BrakesDataConverter
{
	public static object[] ToArray(BrakesData data)
	{
		return new object[5]
		{
			data.Diameter,
			(int)data.DiscType,
			data.Pistons,
			data.PistonDiameter,
			data.CaliperMeshIndex
		};
	}

	public static BrakesData Get(object dataObject)
	{
		BrakesData brakesData = new BrakesData();
		object[] array = (object[])dataObject;
		brakesData.Diameter = (float)array[0];
		brakesData.DiscType = (BrakeDiscType)array[1];
		brakesData.Pistons = (int)array[2];
		brakesData.PistonDiameter = (float)array[3];
		brakesData.CaliperMeshIndex = (int)array[4];
		return brakesData;
	}
}

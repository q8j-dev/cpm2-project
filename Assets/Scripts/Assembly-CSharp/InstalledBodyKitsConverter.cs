using CPM.Database;

public static class InstalledBodyKitsConverter
{
	public static object[] ToArray(InstalledBodyKits data)
	{
		if (data == null)
		{
			data = new InstalledBodyKits();
		}
		return new object[10] { data.SpoilerId, data.FrontBumperId, data.RearBumperId, data.SkirtId, data.HoodId, data.HoodAirIntakeId, data.RoofAirIntakeId, data.FenderId, data.Trims, data.InteriorId };
	}

	public static InstalledBodyKits Get(object dataObject)
	{
		InstalledBodyKits installedBodyKits = new InstalledBodyKits();
		object[] array = (object[])dataObject;
		installedBodyKits.SpoilerId = (int)array[0];
		installedBodyKits.FrontBumperId = (int)array[1];
		installedBodyKits.RearBumperId = (int)array[2];
		installedBodyKits.SkirtId = (int)array[3];
		installedBodyKits.HoodId = (int)array[4];
		installedBodyKits.HoodAirIntakeId = (int)array[5];
		installedBodyKits.RoofAirIntakeId = (int)array[6];
		installedBodyKits.FenderId = (int)array[7];
		if (array.Length >= 9)
		{
			installedBodyKits.Trims = (int[])array[8];
		}
		if (array.Length >= 10)
		{
			installedBodyKits.InteriorId = (int)array[9];
		}
		return installedBodyKits;
	}
}

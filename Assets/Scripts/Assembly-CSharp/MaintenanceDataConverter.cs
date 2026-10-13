using _Car_Parking.Scripts.EngineTuning.DataTypes;

public static class MaintenanceDataConverter
{
	public static object[] ToArray(MaintenanceInfo info)
	{
		return new object[6] { info.FuelInTank, info.OilCapacity, info.OilLife, info.FrontTyreHealth, info.RearTyreHealth, info.Mileage };
	}

	public static MaintenanceInfo Get(object dataObject)
	{
		MaintenanceInfo result = default;
		object[] array = (object[])dataObject;
		result.FuelInTank = (float)array[0];
		result.OilCapacity = (float)array[1];
		result.OilLife = (float)array[2];
		result.FrontTyreHealth = (float[])array[3];
		result.RearTyreHealth = (float[])array[4];
		result.Mileage = (float)array[5];
		return result;
	}
}

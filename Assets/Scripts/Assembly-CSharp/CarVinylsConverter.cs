using _Car_Parking.Scripts.CommonUtility.Contracts;

public static class CarVinylsConverter
{
	public enum VinylsTypeEnum
	{
		FrontPlate = 0,
		RearPlate = 1,
		BodyVinyls = 2,
		FrontPlateVinyls = 3,
		RearPlateVinyls = 4,
		WindowVinyls = 5
	}

	public static object[] ToArray(MultiplayerDataSending.CarVinyls data)
	{
		return new object[6] { data.FrontPlateIndex, data.RearPlateIndex, data.CarBodyVinyls, data.FrontPlateVinyls, data.RearPlateVinyls, data.WindowVinyls };
	}

	public static MultiplayerDataSending.CarVinyls Get(object dataObject)
	{
		MultiplayerDataSending.CarVinyls result = default;
		object[] array = (object[])dataObject;
		result.FrontPlateIndex = (int)array[0];
		result.RearPlateIndex = (int)array[1];
		result.CarBodyVinyls = (object[])array[2];
		result.FrontPlateVinyls = (object[])array[3];
		result.RearPlateVinyls = (object[])array[4];
		result.WindowVinyls = (object[])array[5];
		return result;
	}
}

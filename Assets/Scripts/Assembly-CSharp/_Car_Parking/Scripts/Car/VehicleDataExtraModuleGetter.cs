using System.Collections.Generic;

namespace _Car_Parking.Scripts.Car
{
	public static class VehicleDataExtraModuleGetter
	{
		public static List<VehicleDataExtraModule> GetExtraModules(VehicleData vehicleData)
		{
			List<VehicleDataExtraModule> list = new List<VehicleDataExtraModule>();
			switch (vehicleData.CarId)
			{
			case 216:
				list.Add(new ExtraModuleDisplaySounds216(vehicleData));
				break;
			case 162:
				list.Add(new ExtraModuleDisplaySounds162(vehicleData));
				break;
			}
			return list;
		}
	}
}

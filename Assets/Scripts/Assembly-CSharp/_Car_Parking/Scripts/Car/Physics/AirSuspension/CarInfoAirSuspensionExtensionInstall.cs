using System;
using CPM.Database;

namespace _Car_Parking.Scripts.Car.Physics.AirSuspension
{
	public static class CarInfoAirSuspensionExtensionInstall
	{
		public static event Action<PsData, bool> OnInstallUpdated;

		public static void Set(this PsData airSuspensionData, bool isInstall)
		{
			airSuspensionData.Installed = isInstall;
			OnInstallUpdated?.Invoke(airSuspensionData, isInstall);
		}
	}
}

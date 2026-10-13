using System;
using CPM.Database;

namespace _Car_Parking.Scripts.Police
{
	public static class CarInfoPoliceExtensionInstall
	{
		public static event Action<PoliceData, bool> OnInstallUpdated;

		public static void Set(this PoliceData policeData, IPoliceSystem policeSystem, int carID, bool isInstall)
		{
			policeData.InstalledBodyKits.InitDefaultIfNeeds(policeSystem, carID);
			policeData.Installed = isInstall;
			OnInstallUpdated?.Invoke(policeData, isInstall);
		}
	}
}

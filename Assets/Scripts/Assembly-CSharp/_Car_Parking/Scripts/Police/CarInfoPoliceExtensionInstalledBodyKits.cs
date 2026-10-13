using System;
using System.Collections.Generic;
using CPM.Database;

namespace _Car_Parking.Scripts.Police
{
	public static class CarInfoPoliceExtensionInstalledBodyKits
	{
		public static event Action<PoliceData.InstalledBodyKitsData, PoliceBodyKitType, int> OnInstalledBodyKitUpdated;

		public static int GetIndex(this PoliceData.InstalledBodyKitsData installedBodyKitsData, PoliceBodyKitType policeBodyKitType)
		{
			return policeBodyKitType switch
			{
				PoliceBodyKitType.TopLight => installedBodyKitsData.TopLight, 
				PoliceBodyKitType.Roobar => installedBodyKitsData.Roobar, 
				PoliceBodyKitType.FrontInterior => installedBodyKitsData.FrontInterior, 
				PoliceBodyKitType.RearInterior => installedBodyKitsData.RearInterior, 
				PoliceBodyKitType.Skirt => installedBodyKitsData.Skirt, 
				_ => -1, 
			};
		}

		public static void SetIndex(this PoliceData.InstalledBodyKitsData installedBodyKitsData, PoliceCarBodyKitsConfig.BodyKitData bodyKitData)
		{
			installedBodyKitsData.SetIndex(bodyKitData.BodyKitType, bodyKitData.Index);
		}

		public static void SetIndex(this PoliceData.InstalledBodyKitsData installedBodyKitsData, PoliceBodyKitType policeBodyKitType, int index)
		{
			switch (policeBodyKitType)
			{
			case PoliceBodyKitType.TopLight:
				installedBodyKitsData.TopLight = index;
				break;
			case PoliceBodyKitType.Roobar:
				installedBodyKitsData.Roobar = index;
				break;
			case PoliceBodyKitType.FrontInterior:
				installedBodyKitsData.FrontInterior = index;
				break;
			case PoliceBodyKitType.RearInterior:
				installedBodyKitsData.RearInterior = index;
				break;
			case PoliceBodyKitType.Skirt:
				installedBodyKitsData.Skirt = index;
				break;
			}
			OnInstalledBodyKitUpdated?.Invoke(installedBodyKitsData, policeBodyKitType, index);
		}

		public static void InitDefaultIfNeeds(this PoliceData.InstalledBodyKitsData installedBodyKitsData, IPoliceSystem policeSystem, int carID)
		{
			PoliceCarBodyKitsConfig config = policeSystem.PoliceSystemConfig.PoliceCarConfigs.GetConfig(carID);
			if (installedBodyKitsData.GetIndex(PoliceBodyKitType.TopLight) < 0 && (bool)config)
			{
				int index = 0;
				List<PoliceCarBodyKitsConfig.BodyKitData> bodyKits = config.GetBodyKits(PoliceBodyKitType.TopLight);
				if (bodyKits.Count > 0)
				{
					index = bodyKits[0].Index;
				}
				policeSystem.BoughtBodyKit.AddBought(PoliceBodyKitType.TopLight, index);
			}
		}

		public static bool HaveAnyInstalledBodyKit(this PoliceData.InstalledBodyKitsData installedBodyKitsData)
		{
			foreach (PoliceBodyKitType value in Enum.GetValues(typeof(PoliceBodyKitType)))
			{
				if (installedBodyKitsData.GetIndex(value) >= 0)
				{
					return true;
				}
			}
			return false;
		}
	}
}

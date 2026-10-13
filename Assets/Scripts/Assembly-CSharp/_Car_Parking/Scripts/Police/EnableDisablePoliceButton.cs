using System.Collections.Generic;

namespace _Car_Parking.Scripts.Police
{
	public class EnableDisablePoliceButton : PoliceButtonBase
	{
		protected void UpdateSystem(bool value)
		{
			if (!V.Car || !V.Car.TryGetComponent<VehicleData>(out var component))
			{
				return;
			}
			component.CarInfoProvider.CarInfo.AdditionalData.Police.InstalledBodyKits.InitDefaultIfNeeds(policeSystem, component.CarInfoProvider.CarInfo.CarID);
			if (value && !component.CarInfoProvider.CarInfo.AdditionalData.Police.InstalledBodyKits.HaveAnyInstalledBodyKit())
			{
				List<int> boughtIndexes = policeSystem.BoughtBodyKit.GetBoughtIndexes(PoliceBodyKitType.TopLight);
				if (boughtIndexes != null && boughtIndexes.Count > 0)
				{
					component.CarInfoProvider.CarInfo.AdditionalData.Police.InstalledBodyKits.SetIndex(PoliceBodyKitType.TopLight, boughtIndexes[0]);
				}
			}
			component.CarInfoProvider.CarInfo.AdditionalData.Police.Set(policeSystem, component.CarInfoProvider.CarInfo.CarID, value);
		}
	}
}

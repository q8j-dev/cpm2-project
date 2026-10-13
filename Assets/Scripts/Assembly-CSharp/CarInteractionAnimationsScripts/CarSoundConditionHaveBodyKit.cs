using UnityEngine;

namespace CarInteractionAnimationsScripts
{
	public class CarSoundConditionHaveBodyKit : CarSoundCondition
	{
		public string[] bodykits;

		public override bool CanPlaySound(VehicleData vehicleData)
		{
			if (!vehicleData || bodykits == null || bodykits.Length == 0)
			{
				return true;
			}
			foreach (GameObject allInstalledBodyKit in vehicleData.BodyKits.AllInstalledBodyKits)
			{
				if (!allInstalledBodyKit)
				{
					continue;
				}
				string[] array = bodykits;
				foreach (string value in array)
				{
					if (allInstalledBodyKit.name.Contains(value))
					{
						return true;
					}
				}
			}
			return false;
		}
	}
}

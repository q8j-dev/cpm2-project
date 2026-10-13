using System;
using CPM.Database;

namespace _Car_Parking.Scripts.Police
{
	public static class CarInfoPoliceExtensionSirenSound
	{
		public static event Action<PoliceData.SirenSoundData, int> OnSirenSoundInstallIndexUpdated;

		public static void Set(this PoliceData.SirenSoundData sirenSoundData, int index)
		{
			sirenSoundData.InstalledIndex = index;
			OnSirenSoundInstallIndexUpdated?.Invoke(sirenSoundData, index);
		}
	}
}

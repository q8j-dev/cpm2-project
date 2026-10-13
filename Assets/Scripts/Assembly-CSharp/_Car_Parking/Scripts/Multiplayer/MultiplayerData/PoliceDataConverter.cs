using System;
using CPM.Database;

namespace _Car_Parking.Scripts.Multiplayer.MultiplayerData
{
	public static class PoliceDataConverter
	{
		private enum PoliceDataType
		{
			Installed = 0,
			InstalledTopLight = 1,
			InstalledRoobar = 2,
			InstalledFrontInterior = 3,
			InstalledRearInterior = 4,
			InstalledSkirt = 5,
			SirenSound = 6,
			Bought = 7
		}

		public static object[] ToArray(PoliceData data)
		{
			object[] array = new object[Enum.GetValues(typeof(PoliceDataType)).Length];
			array[0] = data.Installed;
			array[1] = data.InstalledBodyKits.TopLight;
			array[2] = data.InstalledBodyKits.Roobar;
			array[3] = data.InstalledBodyKits.FrontInterior;
			array[4] = data.InstalledBodyKits.RearInterior;
			array[5] = data.InstalledBodyKits.Skirt;
			array[6] = data.SirenSound.InstalledIndex;
			array[7] = data.Bought;
			return array;
		}

		public static PoliceData Get(object dataObject)
		{
			PoliceData policeData = new PoliceData();
			object[] array = (object[])dataObject;
			policeData.Installed = (bool)array[0];
			policeData.InstalledBodyKits.TopLight = (int)array[1];
			policeData.InstalledBodyKits.Roobar = (int)array[2];
			policeData.InstalledBodyKits.FrontInterior = (int)array[3];
			policeData.InstalledBodyKits.RearInterior = (int)array[4];
			policeData.InstalledBodyKits.Skirt = (int)array[5];
			policeData.SirenSound.InstalledIndex = (int)array[6];
			if (array.Length > 7)
			{
				policeData.Bought = (bool)array[7];
			}
			return policeData;
		}
	}
}

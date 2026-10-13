using System;
using CPM.Database;

namespace _Car_Parking.Scripts.Multiplayer.MultiplayerData
{
	public static class PsDataConverter
	{
		private enum PsDataType
		{
			Bought = 0,
			Installed = 1
		}

		public static object[] ToArray(PsData data)
		{
			object[] array = new object[Enum.GetValues(typeof(PsDataType)).Length];
			array[1] = data.Installed;
			array[0] = data.Bought;
			return array;
		}

		public static PsData Get(object dataObject)
		{
			PsData psData = new PsData();
			object[] array = (object[])dataObject;
			psData.Installed = (bool)array[1];
			psData.Bought = (bool)array[0];
			return psData;
		}
	}
}

using System;

namespace _Car_Parking.Scripts.EventSystem.Data
{
	[Serializable]
	public class RaceData : PayloadData
	{
		public float LapTime;

		public int CarClass;
	}
}

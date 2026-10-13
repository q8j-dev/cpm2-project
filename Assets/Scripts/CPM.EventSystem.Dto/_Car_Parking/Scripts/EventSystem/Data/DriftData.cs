using System;

namespace _Car_Parking.Scripts.EventSystem.Data
{
	[Serializable]
	public class DriftData : PayloadData
	{
		public float Points;

		public int CarClass;
	}
}

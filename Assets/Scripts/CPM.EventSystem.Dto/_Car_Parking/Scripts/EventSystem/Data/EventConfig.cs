using System;
using System.Collections.Generic;

namespace _Car_Parking.Scripts.EventSystem.Data
{
	[Serializable]
	public class EventConfig
	{
		public EventType Type;

		public string NameKey;

		public string DescriptionKey;

		public int MaxProgress;

		public List<string> PointsKeys;

		public int UnlockCarId;
	}
}

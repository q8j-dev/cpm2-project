using System;
using UnityEngine.Scripting;
using _Car_Parking.Scripts.EventSystem.Data.Response;

namespace _Car_Parking.Scripts.EventSystem.Data
{
	[Serializable]
	[Preserve]
	public class Event
	{
		public string Id;

		public int Progress;

		public EventType Type;

		public string NameLocalizeKey;

		public string DescriptionLocalizeKey;

		public EventStatus Status;

		public int MaxProgress;

		public long EndTime;

		public int UnlockCarId;

		public EvenDataBase Data;

		public EventImages Images;
	}
}

using System;
using UnityEngine.Scripting;

namespace _Car_Parking.Scripts.EventSystem.Data.Response
{
	[Serializable]
	[Preserve]
	public class EventResponse
	{
		public string Id;

		public int Progress;

		public int Status;

		public long EndTime;

		public EvenDataBase Data;

		public EventImages Images;
	}
}

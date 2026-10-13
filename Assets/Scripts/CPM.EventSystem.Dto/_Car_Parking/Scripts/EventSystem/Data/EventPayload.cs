using System;
using UnityEngine.Scripting;

namespace _Car_Parking.Scripts.EventSystem.Data
{
	[Serializable]
	[Preserve]
	public class EventPayload
	{
		public string EventId;

		public PayloadData Data;
	}
}

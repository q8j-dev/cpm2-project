using System;
using UnityEngine.Scripting;

namespace _Car_Parking.Scripts.EventSystem.Data
{
	[Serializable]
	[Preserve]
	public struct GetEventRequest
	{
		public bool Dev;

		public string Version;

		public string Platform;
	}
}

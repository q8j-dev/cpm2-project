using System;
using System.Collections.Generic;
using CPMEngine;

namespace _Car_Parking.Scripts.EventSystem.Data.Restriction
{
	[Serializable]
	public class EventRestrictions
	{
		public EventType EventType;

		public List<VehicleClass> AllowedCarClasses;
	}
}

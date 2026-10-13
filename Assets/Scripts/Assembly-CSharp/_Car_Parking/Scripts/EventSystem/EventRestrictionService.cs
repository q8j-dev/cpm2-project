using System.Collections.Generic;
using System.Linq;
using CPMEngine;
using UnityEngine.Scripting;
using _Car_Parking.Scripts.EventSystem.Data;
using _Car_Parking.Scripts.EventSystem.Data.Restriction;

namespace _Car_Parking.Scripts.EventSystem
{
	[Preserve]
	public class EventRestrictionService
	{
		private readonly EventRestrictionsConfig _config;

		public EventRestrictionService(EventRestrictionsConfig config)
		{
			_config = config;
		}

		public List<VehicleClass> GetVehicleClasses(EventType eventType)
		{
			return _config.EventRestrictions.FirstOrDefault((EventRestrictions r) => r.EventType == eventType)?.AllowedCarClasses;
		}

		public bool IsVehicleAllowed(EventType eventType, VehicleClass vehicleClass)
		{
			EventRestrictions eventRestrictions = _config.EventRestrictions.FirstOrDefault((EventRestrictions r) => r.EventType == eventType);
			if (eventRestrictions == null)
			{
				return true;
			}
			if (eventRestrictions.AllowedCarClasses.Count == 0)
			{
				return true;
			}
			return eventRestrictions.AllowedCarClasses.Contains(vehicleClass);
		}
	}
}

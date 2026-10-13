using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;
using _Car_Parking.Scripts.EventSystem.Data.Restriction;

namespace _Car_Parking.Scripts.EventSystem.Data
{
	[Preserve]
	[CreateAssetMenu(fileName = "EventRestrictionsConfig", menuName = "Configs/EventRestrictionsConfig")]
	public class EventRestrictionsConfig : ScriptableObject
	{
		public List<EventRestrictions> EventRestrictions = new List<EventRestrictions>();

		[Preserve]
		public EventRestrictionsConfig()
		{
		}
	}
}

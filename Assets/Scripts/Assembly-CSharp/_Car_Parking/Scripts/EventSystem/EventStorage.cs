using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Scripting;
using _Car_Parking.Scripts.EventSystem.Data;

namespace _Car_Parking.Scripts.EventSystem
{
	[Preserve]
	[CreateAssetMenu(fileName = "EventStorage", menuName = "Configs/EventStorage")]
	public class EventStorage : ScriptableObject
	{
		private readonly Dictionary<string, EventType> _eventTypes = new Dictionary<string, EventType>
		{
			{
				"time_attack",
				EventType.TimeAttack
			},
			{
				"new_year_snow_man_attack",
				EventType.NewYearSnowManAttack
			},
			{
				"new_year_presents",
				EventType.NewYearPresents
			},
			{
				"new_year_decorations",
				EventType.NewYearDecorations
			},
			{
				"time_attack_drift",
				EventType.DriftTimeAttack
			},
			{
				"time_attack_nurburg",
				EventType.TimeAttackNurb
			},
			{
				"steam_present",
				EventType.SteamExclusiveCar
			},
			{
				"marathon",
				EventType.ObstacleRace
			},
			{
				"rally_offroad_data",
				EventType.RallyOffroadEvent
			},
			{
				"message",
				EventType.PromoSteamWishList
			}
		};

		public List<EventConfig> EventsConfigs;

		[Preserve]
		public EventStorage()
		{
		}

		public string GetEventNameKey(EventType eventType)
		{
			return EventsConfigs.FirstOrDefault((EventConfig x) => x.Type == eventType)?.NameKey;
		}

		public EventConfig GetEventConfig(EventType eventType)
		{
			return EventsConfigs.FirstOrDefault((EventConfig x) => x.Type == eventType);
		}

		public bool DefineEvent(string data, out EventType eventType)
		{
			if (_eventTypes.TryGetValue(data, out eventType))
			{
				return true;
			}
			return false;
		}
	}
}

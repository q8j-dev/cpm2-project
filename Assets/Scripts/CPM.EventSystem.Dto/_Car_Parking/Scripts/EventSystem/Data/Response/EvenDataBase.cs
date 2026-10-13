using System;
using JsonSubTypes;
using Newtonsoft.Json;
using UnityEngine.Scripting;

namespace _Car_Parking.Scripts.EventSystem.Data.Response
{
	[Serializable]
	[JsonSubtypes.KnownSubType(typeof(TimeAttackNurburgEventData), "time_attack_nurburg")]
	[JsonSubtypes.KnownSubType(typeof(PromoEventData), "message")]
	[JsonSubtypes.KnownSubType(typeof(ObstacleRaceEventData), "marathon")]
	[JsonSubtypes.KnownSubType(typeof(DriftAttackEventData), "time_attack_drift")]
	[JsonSubtypes.KnownSubType(typeof(RallyOffroadEventData), "rally_offroad_data")]
	[JsonSubtypes.KnownSubType(typeof(CollectablesEventData), "new_year_decorations")]
	[JsonSubtypes.KnownSubType(typeof(CollectablesEventData), "new_year_presents")]
	[JsonSubtypes.KnownSubType(typeof(TimeAttackEventData), "time_attack")]
	[JsonConverter(typeof(JsonSubtypes), new object[] { "Type" })]
	[Preserve]
	[JsonSubtypes.KnownSubType(typeof(SnowmanAttackEventData), "new_year_snow_man_attack")]
	public class EvenDataBase
	{
		public string Type;
	}
}

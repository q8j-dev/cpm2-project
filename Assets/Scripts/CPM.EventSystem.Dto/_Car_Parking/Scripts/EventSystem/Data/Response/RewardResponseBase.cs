using System;
using JsonSubTypes;
using Newtonsoft.Json;
using UnityEngine.Scripting;

namespace _Car_Parking.Scripts.EventSystem.Data.Response
{
	[Serializable]
	[JsonSubtypes.KnownSubType(typeof(RewardEquipmentResponse), "suit")]
	[JsonConverter(typeof(JsonSubtypes), new object[] { "Type" })]
	[JsonSubtypes.KnownSubType(typeof(RewardCarResponse), "car")]
	[JsonSubtypes.KnownSubType(typeof(RewardMessageResponse), "message")]
	[Preserve]
	public class RewardResponseBase
	{
		public int Amount;

		public string Type;
	}
}

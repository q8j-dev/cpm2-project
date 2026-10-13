using System;
using System.Collections.Generic;
using JsonSubTypes;
using Newtonsoft.Json;
using UnityEngine.Scripting;

namespace _Car_Parking.Scripts.Offers
{
	[Serializable]
	[Preserve]
	[JsonConverter(typeof(JsonSubtypes), new object[] { "Type" })]
	[JsonSubtypes.KnownSubType(typeof(RewardItemCarData), RewardType.Car)]
	[JsonSubtypes.KnownSubType(typeof(RewardItemSuitData), RewardType.Suit)]
	public class RewardItemData
	{
		public RewardType Type;

		public int ID;

		public string URL;

		public virtual Dictionary<string, object> ToAnalyticsDictionary()
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("item_type", AnalyticsItemType());
			dictionary[AnalyticsIdKey()] = ID;
			return dictionary;
		}

		protected string AnalyticsItemType()
		{
			return Type switch
			{
				RewardType.Car => "CAR", 
				RewardType.SuitPack => "SUIT_PACK", 
				RewardType.Suit => "SUIT", 
				_ => Type.ToString(), 
			};
		}

		protected string AnalyticsIdKey()
		{
			return Type switch
			{
				RewardType.Car => "car_id", 
				RewardType.SuitPack => "pack_id", 
				RewardType.Suit => "suit_id", 
				_ => "item_id", 
			};
		}
	}
}

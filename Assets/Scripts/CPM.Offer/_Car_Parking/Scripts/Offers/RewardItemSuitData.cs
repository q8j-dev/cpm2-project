using System;
using System.Collections.Generic;

namespace _Car_Parking.Scripts.Offers
{
	[Serializable]
	public class RewardItemSuitData : RewardItemData
	{
		public int TypeId;

		public int BodyType;

		public override Dictionary<string, object> ToAnalyticsDictionary()
		{
			Dictionary<string, object> dictionary = base.ToAnalyticsDictionary();
			dictionary["type_id"] = TypeId;
			dictionary["body_type"] = BodyType;
			return dictionary;
		}
	}
}

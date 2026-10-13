using System;
using Newtonsoft.Json;

namespace _Car_Parking.Scripts.Analytics.Data
{
	[Serializable]
	public class ItemIdData
	{
		[JsonProperty(PropertyName = "itemId")]
		public string ItemId;

		[JsonProperty(PropertyName = "amount")]
		public int Amount;
	}
}

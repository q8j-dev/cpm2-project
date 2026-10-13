using System;
using Newtonsoft.Json;
using UnityEngine.Scripting;

namespace _Car_Parking.Scripts.EventSystem.Data.Response
{
	[Serializable]
	[Preserve]
	public class PromoEventData : EvenDataBase
	{
		[JsonProperty("app_id")]
		public int AppId;

		[JsonProperty("app_name")]
		public string AppName;

		[JsonProperty("hyperlink")]
		public bool Hyperlink;

		[Preserve]
		public PromoEventData()
		{
		}
	}
}

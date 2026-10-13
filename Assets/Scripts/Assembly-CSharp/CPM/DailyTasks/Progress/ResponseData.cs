using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace CPM.DailyTasks.Progress
{
	[Serializable]
	public class ResponseData
	{
		public Type daily;

		public Dictionary<string, int> ToDictionary()
		{
			return null;
		}

		public string ToJson()
		{
			return JsonConvert.SerializeObject(daily);
		}
	}
}

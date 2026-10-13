using System;
using System.Collections.Generic;

namespace _Car_Parking.Scripts.Analytics.Root
{
	[Serializable]
	public sealed class Event
	{
		public DateTime SendTime;

		public Parameter[] Parameters;

		public Dictionary<string, object> ToDictionary()
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			Parameter[] parameters = Parameters;
			foreach (Parameter parameter in parameters)
			{
				dictionary.Add(parameter.Name, parameter.ParameterValue);
			}
			return dictionary;
		}
	}
}

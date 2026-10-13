using System;

namespace _Car_Parking.Scripts.Analytics.Root
{
	[Serializable]
	public sealed class Parameter
	{
		public string Name;

		public object ParameterValue;

		public Parameter(string parameterName, object parameterValue)
		{
			Name = parameterName;
			ParameterValue = parameterValue;
		}
	}
}

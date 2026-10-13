using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CPM.RemoteConfig
{
	[JsonConverter(typeof(StringEnumConverter))]
	public enum AnalyticsEnvironment
	{
		UNKNOWN = 0,
		DEV = 1,
		PROD = 2,
		BETA = 3
	}
}

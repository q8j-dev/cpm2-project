using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace CPM.Balance
{
	public static class DocumentJsonSettings
	{
		public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
		{
			ContractResolver = new CamelCasePropertyNamesContractResolver(),
			Converters = { (JsonConverter)new TolerantEnumConverter() },
			NullValueHandling = NullValueHandling.Ignore,
			MissingMemberHandling = MissingMemberHandling.Ignore
		};
	}
}

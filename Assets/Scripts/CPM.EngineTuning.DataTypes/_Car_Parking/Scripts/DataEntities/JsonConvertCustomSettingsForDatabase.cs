using System;
using Newtonsoft.Json;
using _Car_Parking.Scripts.EngineTuning;

namespace _Car_Parking.Scripts.DataEntities
{
	public class JsonConvertCustomSettingsForDatabase
	{
		public static JsonSerializerSettings JsonSettingsForDataBase;

		public static void Init(Func<PartData, PartData> partDataChecker)
		{
			JsonSettingsForDataBase = new JsonSerializerSettings
			{
				ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
				Converters = 
				{
					(JsonConverter)new PartDataConverter(partDataChecker),
					(JsonConverter)new ColorConverter(),
					(JsonConverter)new CarPaintColorConverter(),
					(JsonConverter)new ColorSerializable(),
					(JsonConverter)new VectorConverter(),
					(JsonConverter)new MaintenanceDataConverter(),
					(JsonConverter)new InstalledBodyKitsConverter(),
					(JsonConverter)new BodyKitColorConverter(),
					(JsonConverter)new ForceStringConverter()
				}
			};
		}
	}
}

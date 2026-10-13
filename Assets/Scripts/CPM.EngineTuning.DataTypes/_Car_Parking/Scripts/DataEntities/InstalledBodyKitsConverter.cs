using System;
using System.Linq;
using CPM.Database;
using Newtonsoft.Json;

namespace _Car_Parking.Scripts.DataEntities
{
	public class InstalledBodyKitsConverter : JsonConverter
	{
		private const string Separator = "#";

		public override void WriteJson(JsonWriter writer, object data, JsonSerializer serializer)
		{
			InstalledBodyKits installedBodyKits = (InstalledBodyKits)data;
			object[] array = new object[10] { installedBodyKits.SpoilerId, installedBodyKits.FrontBumperId, installedBodyKits.RearBumperId, installedBodyKits.SkirtId, installedBodyKits.HoodId, installedBodyKits.HoodAirIntakeId, installedBodyKits.RoofAirIntakeId, installedBodyKits.FenderId, "", null };
			if (installedBodyKits.Trims != null && installedBodyKits.Trims.Length != 0)
			{
				array[8] = string.Join("#", installedBodyKits.Trims);
			}
			array[9] = installedBodyKits.InteriorId;
			serializer.Serialize(writer, array);
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			object[] array = serializer.Deserialize<object[]>(reader);
			if (array == null || array.Length < 9)
			{
				return new InstalledBodyKits();
			}
			InstalledBodyKits installedBodyKits = new InstalledBodyKits
			{
				SpoilerId = Convert.ToInt32(array[0]),
				FrontBumperId = Convert.ToInt32(array[1]),
				RearBumperId = Convert.ToInt32(array[2]),
				SkirtId = Convert.ToInt32(array[3]),
				HoodId = Convert.ToInt32(array[4]),
				HoodAirIntakeId = Convert.ToInt32(array[5]),
				RoofAirIntakeId = Convert.ToInt32(array[6]),
				FenderId = Convert.ToInt32(array[7]),
				Trims = Array.Empty<int>()
			};
			if (array.Length > 8)
			{
				string text = Convert.ToString(array[8]);
				if (!string.IsNullOrEmpty(text))
				{
					installedBodyKits.Trims = text.Split("#").Select(int.Parse).ToArray();
				}
			}
			if (array.Length > 9)
			{
				installedBodyKits.InteriorId = Convert.ToInt32(array[9]);
			}
			return installedBodyKits;
		}

		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(InstalledBodyKits);
		}
	}
}

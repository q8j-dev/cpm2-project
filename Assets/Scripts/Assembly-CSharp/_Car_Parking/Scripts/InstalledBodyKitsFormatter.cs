using System;
using System.Linq;
using CPM.Database;
using MemoryPack;

namespace _Car_Parking.Scripts
{
	public class InstalledBodyKitsFormatter : MemoryPackFormatter<InstalledBodyKits>
	{
		private const string Separator = "#";

		public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref InstalledBodyKits value)
		{
			if (value == null)
			{
				writer.WriteNullObjectHeader();
				return;
			}
			writer.WriteObjectHeader(10);
			writer.WriteValue(in value.SpoilerId);
			writer.WriteValue(in value.FrontBumperId);
			writer.WriteValue(in value.RearBumperId);
			writer.WriteValue(in value.SkirtId);
			writer.WriteValue(in value.HoodId);
			writer.WriteValue(in value.HoodAirIntakeId);
			writer.WriteValue(in value.RoofAirIntakeId);
			writer.WriteValue(in value.FenderId);
			string value2 = "";
			if (value.Trims != null && value.Trims.Length != 0)
			{
				value2 = string.Join("#", value.Trims);
			}
			writer.WriteValue(in value2);
			writer.WriteValue(in value.InteriorId);
		}

		public override void Deserialize(ref MemoryPackReader reader, ref InstalledBodyKits value)
		{
			value = new InstalledBodyKits();
			value.Trims = Array.Empty<int>();
			if (!reader.TryReadObjectHeader(out var memberCount))
			{
				return;
			}
			value.SpoilerId = reader.ReadValue<int>();
			value.FrontBumperId = reader.ReadValue<int>();
			value.RearBumperId = reader.ReadValue<int>();
			value.SkirtId = reader.ReadValue<int>();
			value.HoodId = reader.ReadValue<int>();
			value.HoodAirIntakeId = reader.ReadValue<int>();
			value.RoofAirIntakeId = reader.ReadValue<int>();
			value.FenderId = reader.ReadValue<int>();
			if (memberCount > 8)
			{
				string text = reader.ReadValue<string>();
				if (!string.IsNullOrEmpty(text))
				{
					value.Trims = text.Split("#").Select(int.Parse).ToArray();
				}
				value.InteriorId = reader.ReadValue<int>();
			}
		}
	}
}

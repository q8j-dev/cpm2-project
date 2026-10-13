#nullable enable annotations
using System;
using System.Buffers;
using System.Collections.Generic;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;

namespace CPM.Database
{
	[Serializable]
	[GenerateTypeScript]
	[MemoryPackable(GenerateType.Object)]
	public class BoughtBodyKits : IMemoryPackable<BoughtBodyKits>, IMemoryPackFormatterRegister
	{
		[Preserve]
		private sealed class BoughtBodyKitsFormatter : MemoryPackFormatter<BoughtBodyKits>
		{
			[Preserve]
			public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref BoughtBodyKits value)
			{
				BoughtBodyKits.Serialize(ref writer, ref value);
			}

			[Preserve]
			public override void Deserialize(ref MemoryPackReader reader, ref BoughtBodyKits value)
			{
				BoughtBodyKits.Deserialize(ref reader, ref value);
			}
		}

		public List<int> SpoilerIds = new List<int>();

		public List<int> FrontBumperIds = new List<int>();

		public List<int> RearBumperIds = new List<int>();

		public List<int> SkirtIds = new List<int>();

		public List<int> HoodIds = new List<int>();

		public List<int> HoodAirIntakeIds = new List<int>();

		public List<int> RoofAirIntakeIds = new List<int>();

		public List<int> FenderIds = new List<int>();

		public List<int> TrimIds = new List<int>();

		public List<int> Interiors = new List<int>();

		[MemoryPackConstructor]
		public BoughtBodyKits()
		{
		}

		public BoughtBodyKits(InstalledBodyKits carInfoInstalledBodyKits)
		{
			SpoilerIds.Add(carInfoInstalledBodyKits.SpoilerId);
			FrontBumperIds.Add(carInfoInstalledBodyKits.FrontBumperId);
			RearBumperIds.Add(carInfoInstalledBodyKits.RearBumperId);
			SkirtIds.Add(carInfoInstalledBodyKits.SkirtId);
			HoodIds.Add(carInfoInstalledBodyKits.HoodId);
			HoodAirIntakeIds.Add(carInfoInstalledBodyKits.HoodAirIntakeId);
			RoofAirIntakeIds.Add(carInfoInstalledBodyKits.RoofAirIntakeId);
			FenderIds.Add(carInfoInstalledBodyKits.FenderId);
			Interiors.Add(carInfoInstalledBodyKits.InteriorId);
		}

		static BoughtBodyKits()
		{
			RegisterFormatter();
		}

		[Preserve]
		public static void RegisterFormatter()
		{
			if (!MemoryPackFormatterProvider.IsRegistered<BoughtBodyKits>())
			{
				MemoryPackFormatterProvider.Register(new BoughtBodyKitsFormatter());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<BoughtBodyKits[]>())
			{
				MemoryPackFormatterProvider.Register(new ArrayFormatter<BoughtBodyKits>());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<List<int>>())
			{
				MemoryPackFormatterProvider.Register(new ListFormatter<int>());
			}
		}

		[Preserve]
		public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref BoughtBodyKits? value) where TBufferWriter : class, IBufferWriter<byte>
		{
			if (value == null)
			{
				writer.WriteNullObjectHeader();
				return;
			}
			writer.WriteObjectHeader(10);
			writer.WriteValue(in value.SpoilerIds);
			writer.WriteValue(in value.FrontBumperIds);
			writer.WriteValue(in value.RearBumperIds);
			writer.WriteValue(in value.SkirtIds);
			writer.WriteValue(in value.HoodIds);
			writer.WriteValue(in value.HoodAirIntakeIds);
			writer.WriteValue(in value.RoofAirIntakeIds);
			writer.WriteValue(in value.FenderIds);
			writer.WriteValue(in value.TrimIds);
			writer.WriteValue(in value.Interiors);
		}

		[Preserve]
		public static void Deserialize(ref MemoryPackReader reader, ref BoughtBodyKits? value)
		{
			if (!reader.TryReadObjectHeader(out var memberCount))
			{
				value = null;
				return;
			}
			List<int> value2;
			List<int> value3;
			List<int> value4;
			List<int> value5;
			List<int> value6;
			List<int> value7;
			List<int> value8;
			List<int> value9;
			List<int> value10;
			List<int> value11;
			if (memberCount == 10)
			{
				if (value != null)
				{
					value2 = value.SpoilerIds;
					value3 = value.FrontBumperIds;
					value4 = value.RearBumperIds;
					value5 = value.SkirtIds;
					value6 = value.HoodIds;
					value7 = value.HoodAirIntakeIds;
					value8 = value.RoofAirIntakeIds;
					value9 = value.FenderIds;
					value10 = value.TrimIds;
					value11 = value.Interiors;
					reader.ReadValue(ref value2);
					reader.ReadValue(ref value3);
					reader.ReadValue(ref value4);
					reader.ReadValue(ref value5);
					reader.ReadValue(ref value6);
					reader.ReadValue(ref value7);
					reader.ReadValue(ref value8);
					reader.ReadValue(ref value9);
					reader.ReadValue(ref value10);
					reader.ReadValue(ref value11);
					goto IL_0229;
				}
				value2 = reader.ReadValue<List<int>>();
				value3 = reader.ReadValue<List<int>>();
				value4 = reader.ReadValue<List<int>>();
				value5 = reader.ReadValue<List<int>>();
				value6 = reader.ReadValue<List<int>>();
				value7 = reader.ReadValue<List<int>>();
				value8 = reader.ReadValue<List<int>>();
				value9 = reader.ReadValue<List<int>>();
				value10 = reader.ReadValue<List<int>>();
				value11 = reader.ReadValue<List<int>>();
			}
			else
			{
				if (memberCount > 10)
				{
					MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(BoughtBodyKits), 10, memberCount);
					return;
				}
				if (value == null)
				{
					value2 = null;
					value3 = null;
					value4 = null;
					value5 = null;
					value6 = null;
					value7 = null;
					value8 = null;
					value9 = null;
					value10 = null;
					value11 = null;
				}
				else
				{
					value2 = value.SpoilerIds;
					value3 = value.FrontBumperIds;
					value4 = value.RearBumperIds;
					value5 = value.SkirtIds;
					value6 = value.HoodIds;
					value7 = value.HoodAirIntakeIds;
					value8 = value.RoofAirIntakeIds;
					value9 = value.FenderIds;
					value10 = value.TrimIds;
					value11 = value.Interiors;
				}
				if (memberCount != 0)
				{
					reader.ReadValue(ref value2);
					if (memberCount != 1)
					{
						reader.ReadValue(ref value3);
						if (memberCount != 2)
						{
							reader.ReadValue(ref value4);
							if (memberCount != 3)
							{
								reader.ReadValue(ref value5);
								if (memberCount != 4)
								{
									reader.ReadValue(ref value6);
									if (memberCount != 5)
									{
										reader.ReadValue(ref value7);
										if (memberCount != 6)
										{
											reader.ReadValue(ref value8);
											if (memberCount != 7)
											{
												reader.ReadValue(ref value9);
												if (memberCount != 8)
												{
													reader.ReadValue(ref value10);
													if (memberCount != 9)
													{
														reader.ReadValue(ref value11);
														_ = 10;
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
				if (value != null)
				{
					goto IL_0229;
				}
			}
			value = new BoughtBodyKits
			{
				SpoilerIds = value2,
				FrontBumperIds = value3,
				RearBumperIds = value4,
				SkirtIds = value5,
				HoodIds = value6,
				HoodAirIntakeIds = value7,
				RoofAirIntakeIds = value8,
				FenderIds = value9,
				TrimIds = value10,
				Interiors = value11
			};
			return;
			IL_0229:
			value.SpoilerIds = value2;
			value.FrontBumperIds = value3;
			value.RearBumperIds = value4;
			value.SkirtIds = value5;
			value.HoodIds = value6;
			value.HoodAirIntakeIds = value7;
			value.RoofAirIntakeIds = value8;
			value.FenderIds = value9;
			value.TrimIds = value10;
			value.Interiors = value11;
		}
	}
}

#nullable enable annotations
using System;
using System.Buffers;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;

namespace CPM.Database
{
	[Serializable]
	[GenerateTypeScript]
	[MemoryPackable(GenerateType.Object)]
	public class InstalledBodyKits : ICloneable, IMemoryPackable<InstalledBodyKits>, IMemoryPackFormatterRegister
	{
		[Preserve]
		private sealed class InstalledBodyKitsFormatter : MemoryPackFormatter<InstalledBodyKits>
		{
			[Preserve]
			public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref InstalledBodyKits value)
			{
				InstalledBodyKits.Serialize(ref writer, ref value);
			}

			[Preserve]
			public override void Deserialize(ref MemoryPackReader reader, ref InstalledBodyKits value)
			{
				InstalledBodyKits.Deserialize(ref reader, ref value);
			}
		}

		public int FrontBumperId;

		public int RearBumperId;

		public int SpoilerId;

		public int SkirtId;

		public int FenderId;

		public int HoodId;

		public int HoodAirIntakeId;

		public int RoofAirIntakeId;

		public int[] Trims;

		public int InteriorId;

		public object Clone()
		{
			return MemberwiseClone();
		}

		static InstalledBodyKits()
		{
			RegisterFormatter();
		}

		[Preserve]
		public static void RegisterFormatter()
		{
			if (!MemoryPackFormatterProvider.IsRegistered<InstalledBodyKits>())
			{
				MemoryPackFormatterProvider.Register(new InstalledBodyKitsFormatter());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<InstalledBodyKits[]>())
			{
				MemoryPackFormatterProvider.Register(new ArrayFormatter<InstalledBodyKits>());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<int[]>())
			{
				MemoryPackFormatterProvider.Register(new ArrayFormatter<int>());
			}
		}

		[Preserve]
		public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref InstalledBodyKits? value) where TBufferWriter : class, IBufferWriter<byte>
		{
			if (value == null)
			{
				writer.WriteNullObjectHeader();
				return;
			}
			writer.WriteUnmanagedWithObjectHeader(10, in value.FrontBumperId, in value.RearBumperId, in value.SpoilerId, in value.SkirtId, in value.FenderId, in value.HoodId, in value.HoodAirIntakeId, in value.RoofAirIntakeId);
			writer.WriteUnmanagedArray(value.Trims);
			writer.WriteUnmanaged(in value.InteriorId);
		}

		[Preserve]
		public static void Deserialize(ref MemoryPackReader reader, ref InstalledBodyKits? value)
		{
			if (!reader.TryReadObjectHeader(out var memberCount))
			{
				value = null;
				return;
			}
			int value2;
			int value3;
			int value4;
			int value5;
			int value6;
			int value7;
			int value8;
			int value9;
			int[] value10;
			int value11;
			if (memberCount == 10)
			{
				if (value != null)
				{
					value2 = value.FrontBumperId;
					value3 = value.RearBumperId;
					value4 = value.SpoilerId;
					value5 = value.SkirtId;
					value6 = value.FenderId;
					value7 = value.HoodId;
					value8 = value.HoodAirIntakeId;
					value9 = value.RoofAirIntakeId;
					value10 = value.Trims;
					value11 = value.InteriorId;
					reader.ReadUnmanaged<int>(out value2);
					reader.ReadUnmanaged<int>(out value3);
					reader.ReadUnmanaged<int>(out value4);
					reader.ReadUnmanaged<int>(out value5);
					reader.ReadUnmanaged<int>(out value6);
					reader.ReadUnmanaged<int>(out value7);
					reader.ReadUnmanaged<int>(out value8);
					reader.ReadUnmanaged<int>(out value9);
					reader.ReadUnmanagedArray(ref value10);
					reader.ReadUnmanaged<int>(out value11);
					goto IL_0202;
				}
				reader.ReadUnmanaged<int, int, int, int, int, int, int, int>(out value2, out value3, out value4, out value5, out value6, out value7, out value8, out value9);
				value10 = reader.ReadUnmanagedArray<int>();
				reader.ReadUnmanaged<int>(out value11);
			}
			else
			{
				if (memberCount > 10)
				{
					MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(InstalledBodyKits), 10, memberCount);
					return;
				}
				if (value == null)
				{
					value2 = 0;
					value3 = 0;
					value4 = 0;
					value5 = 0;
					value6 = 0;
					value7 = 0;
					value8 = 0;
					value9 = 0;
					value10 = null;
					value11 = 0;
				}
				else
				{
					value2 = value.FrontBumperId;
					value3 = value.RearBumperId;
					value4 = value.SpoilerId;
					value5 = value.SkirtId;
					value6 = value.FenderId;
					value7 = value.HoodId;
					value8 = value.HoodAirIntakeId;
					value9 = value.RoofAirIntakeId;
					value10 = value.Trims;
					value11 = value.InteriorId;
				}
				if (memberCount != 0)
				{
					reader.ReadUnmanaged<int>(out value2);
					if (memberCount != 1)
					{
						reader.ReadUnmanaged<int>(out value3);
						if (memberCount != 2)
						{
							reader.ReadUnmanaged<int>(out value4);
							if (memberCount != 3)
							{
								reader.ReadUnmanaged<int>(out value5);
								if (memberCount != 4)
								{
									reader.ReadUnmanaged<int>(out value6);
									if (memberCount != 5)
									{
										reader.ReadUnmanaged<int>(out value7);
										if (memberCount != 6)
										{
											reader.ReadUnmanaged<int>(out value8);
											if (memberCount != 7)
											{
												reader.ReadUnmanaged<int>(out value9);
												if (memberCount != 8)
												{
													reader.ReadUnmanagedArray(ref value10);
													if (memberCount != 9)
													{
														reader.ReadUnmanaged<int>(out value11);
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
					goto IL_0202;
				}
			}
			value = new InstalledBodyKits
			{
				FrontBumperId = value2,
				RearBumperId = value3,
				SpoilerId = value4,
				SkirtId = value5,
				FenderId = value6,
				HoodId = value7,
				HoodAirIntakeId = value8,
				RoofAirIntakeId = value9,
				Trims = value10,
				InteriorId = value11
			};
			return;
			IL_0202:
			value.FrontBumperId = value2;
			value.RearBumperId = value3;
			value.SpoilerId = value4;
			value.SkirtId = value5;
			value.FenderId = value6;
			value.HoodId = value7;
			value.HoodAirIntakeId = value8;
			value.RoofAirIntakeId = value9;
			value.Trims = value10;
			value.InteriorId = value11;
		}
	}
}

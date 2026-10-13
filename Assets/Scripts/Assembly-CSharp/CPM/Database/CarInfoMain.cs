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
	public class CarInfoMain : IMemoryPackable<CarInfoMain>, IMemoryPackFormatterRegister
	{
		[Preserve]
		private sealed class CarInfoMainFormatter : MemoryPackFormatter<CarInfoMain>
		{
			[Preserve]
			public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref CarInfoMain value)
			{
				CarInfoMain.Serialize(ref writer, ref value);
			}

			[Preserve]
			public override void Deserialize(ref MemoryPackReader reader, ref CarInfoMain value)
			{
				CarInfoMain.Deserialize(ref reader, ref value);
			}
		}

		public int CarID;

		public InstalledBodyKits InstalledBodyKits;

		public Colors Colors;

		public AdditionalData AdditionalData;

		public CarInfoMain(CarInfo carInfo)
		{
			CarID = carInfo.CarID;
			InstalledBodyKits = carInfo.InstalledBodyKits;
			Colors = carInfo.Colors;
			AdditionalData = carInfo.AdditionalData;
		}

		[MemoryPackConstructor]
		protected CarInfoMain()
		{
		}

		static CarInfoMain()
		{
			RegisterFormatter();
		}

		[Preserve]
		public static void RegisterFormatter()
		{
			if (!MemoryPackFormatterProvider.IsRegistered<CarInfoMain>())
			{
				MemoryPackFormatterProvider.Register(new CarInfoMainFormatter());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<CarInfoMain[]>())
			{
				MemoryPackFormatterProvider.Register(new ArrayFormatter<CarInfoMain>());
			}
		}

		[Preserve]
		public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref CarInfoMain? value) where TBufferWriter : class, IBufferWriter<byte>
		{
			if (value == null)
			{
				writer.WriteNullObjectHeader();
				return;
			}
			writer.WriteUnmanagedWithObjectHeader(4, in value.CarID);
			writer.WritePackable(in value.InstalledBodyKits);
			writer.WritePackable(in value.Colors);
			writer.WritePackable(in value.AdditionalData);
		}

		[Preserve]
		public static void Deserialize(ref MemoryPackReader reader, ref CarInfoMain? value)
		{
			if (!reader.TryReadObjectHeader(out var memberCount))
			{
				value = null;
				return;
			}
			int value2;
			InstalledBodyKits value3;
			Colors value4;
			AdditionalData value5;
			if (memberCount == 4)
			{
				if (value != null)
				{
					value2 = value.CarID;
					value3 = value.InstalledBodyKits;
					value4 = value.Colors;
					value5 = value.AdditionalData;
					reader.ReadUnmanaged<int>(out value2);
					reader.ReadPackable(ref value3);
					reader.ReadPackable(ref value4);
					reader.ReadPackable(ref value5);
					goto IL_00f9;
				}
				reader.ReadUnmanaged<int>(out value2);
				value3 = reader.ReadPackable<InstalledBodyKits>();
				value4 = reader.ReadPackable<Colors>();
				value5 = reader.ReadPackable<AdditionalData>();
			}
			else
			{
				if (memberCount > 4)
				{
					MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(CarInfoMain), 4, memberCount);
					return;
				}
				if (value == null)
				{
					value2 = 0;
					value3 = null;
					value4 = null;
					value5 = null;
				}
				else
				{
					value2 = value.CarID;
					value3 = value.InstalledBodyKits;
					value4 = value.Colors;
					value5 = value.AdditionalData;
				}
				if (memberCount != 0)
				{
					reader.ReadUnmanaged<int>(out value2);
					if (memberCount != 1)
					{
						reader.ReadPackable(ref value3);
						if (memberCount != 2)
						{
							reader.ReadPackable(ref value4);
							if (memberCount != 3)
							{
								reader.ReadPackable(ref value5);
								_ = 4;
							}
						}
					}
				}
				if (value != null)
				{
					goto IL_00f9;
				}
			}
			value = new CarInfoMain
			{
				CarID = value2,
				InstalledBodyKits = value3,
				Colors = value4,
				AdditionalData = value5
			};
			return;
			IL_00f9:
			value.CarID = value2;
			value.InstalledBodyKits = value3;
			value.Colors = value4;
			value.AdditionalData = value5;
		}
	}
}

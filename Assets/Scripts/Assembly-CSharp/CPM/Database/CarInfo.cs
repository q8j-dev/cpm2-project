using System;
using System.Buffers;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using _Car_Parking.Scripts.EngineTuning;

namespace CPM.Database
{
	[Serializable]
	[MemoryPackable(GenerateType.Object)]
	[GenerateTypeScript]
	public class CarInfo : CarInfoMain, IMemoryPackable<CarInfo>, IMemoryPackFormatterRegister
	{
		[Preserve]
		private sealed class CarInfoFormatter : MemoryPackFormatter<CarInfo>
		{
			[Preserve]
			public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref CarInfo value)
			{
				CarInfo.Serialize(ref writer, ref value);
			}

			[Preserve]
			public override void Deserialize(ref MemoryPackReader reader, ref CarInfo value)
			{
				CarInfo.Deserialize(ref reader, ref value);
			}
		}

		public string CarInstanceId;

		public BoughtBodyKits BoughtBodyKits = new BoughtBodyKits();

		public CarTuningData TuningData { get; set; }

		[MemoryPackConstructor]
		public CarInfo()
		{
		}

		public CarInfo(CarInfoMain carInfoMain)
		{
			CarID = carInfoMain.CarID;
			InstalledBodyKits = carInfoMain.InstalledBodyKits.Clone() as InstalledBodyKits;
			Colors = carInfoMain.Colors.Clone() as Colors;
			AdditionalData = carInfoMain.AdditionalData.Clone() as AdditionalData;
		}

		static CarInfo()
		{
			RegisterFormatter();
		}

		[Preserve]
		public new static void RegisterFormatter()
		{
			if (!MemoryPackFormatterProvider.IsRegistered<CarInfo>())
			{
				MemoryPackFormatterProvider.Register(new CarInfoFormatter());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<CarInfo[]>())
			{
				MemoryPackFormatterProvider.Register(new ArrayFormatter<CarInfo>());
			}
		}

		[Preserve]
		public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref CarInfo? value) where TBufferWriter : class, IBufferWriter<byte>
		{
			if (value == null)
			{
				writer.WriteNullObjectHeader();
				return;
			}
			writer.WriteUnmanagedWithObjectHeader(7, in value.CarID);
			writer.WritePackable(in value.InstalledBodyKits);
			writer.WritePackable(in value.Colors);
			writer.WritePackable(in value.AdditionalData);
			writer.WriteString(value.CarInstanceId);
			writer.WritePackable<CarTuningData>(value.TuningData);
			writer.WritePackable(in value.BoughtBodyKits);
		}

		[Preserve]
		public static void Deserialize(ref MemoryPackReader reader, ref CarInfo? value)
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
			CarTuningData value6;
			BoughtBodyKits value7;
			string carInstanceId;
			if (memberCount == 7)
			{
				if (value != null)
				{
					value2 = value.CarID;
					value3 = value.InstalledBodyKits;
					value4 = value.Colors;
					value5 = value.AdditionalData;
					carInstanceId = value.CarInstanceId;
					value6 = value.TuningData;
					value7 = value.BoughtBodyKits;
					reader.ReadUnmanaged<int>(out value2);
					reader.ReadPackable(ref value3);
					reader.ReadPackable(ref value4);
					reader.ReadPackable(ref value5);
					carInstanceId = reader.ReadString();
					reader.ReadPackable(ref value6);
					reader.ReadPackable(ref value7);
					goto IL_0192;
				}
				reader.ReadUnmanaged<int>(out value2);
				value3 = reader.ReadPackable<InstalledBodyKits>();
				value4 = reader.ReadPackable<Colors>();
				value5 = reader.ReadPackable<AdditionalData>();
				carInstanceId = reader.ReadString();
				value6 = reader.ReadPackable<CarTuningData>();
				value7 = reader.ReadPackable<BoughtBodyKits>();
			}
			else
			{
				if (memberCount > 7)
				{
					MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(CarInfo), 7, memberCount);
					return;
				}
				if (value == null)
				{
					value2 = 0;
					value3 = null;
					value4 = null;
					value5 = null;
					carInstanceId = null;
					value6 = null;
					value7 = null;
				}
				else
				{
					value2 = value.CarID;
					value3 = value.InstalledBodyKits;
					value4 = value.Colors;
					value5 = value.AdditionalData;
					carInstanceId = value.CarInstanceId;
					value6 = value.TuningData;
					value7 = value.BoughtBodyKits;
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
								if (memberCount != 4)
								{
									carInstanceId = reader.ReadString();
									if (memberCount != 5)
									{
										reader.ReadPackable(ref value6);
										if (memberCount != 6)
										{
											reader.ReadPackable(ref value7);
											_ = 7;
										}
									}
								}
							}
						}
					}
				}
				if (value != null)
				{
					goto IL_0192;
				}
			}
			value = new CarInfo
			{
				CarID = value2,
				InstalledBodyKits = value3,
				Colors = value4,
				AdditionalData = value5,
				CarInstanceId = carInstanceId,
				TuningData = value6,
				BoughtBodyKits = value7
			};
			return;
			IL_0192:
			value.CarID = value2;
			value.InstalledBodyKits = value3;
			value.Colors = value4;
			value.AdditionalData = value5;
			value.CarInstanceId = carInstanceId;
			value.TuningData = value6;
			value.BoughtBodyKits = value7;
		}
	}
}

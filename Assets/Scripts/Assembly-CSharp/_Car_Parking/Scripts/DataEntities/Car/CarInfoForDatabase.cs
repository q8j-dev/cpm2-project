using System;
using System.Buffers;
using System.Collections.Generic;
using CPM.Database;
using DataEntities.Vinyl;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using UnityEngine.Scripting;
using _Car_Parking.Scripts.EngineTuning;

namespace _Car_Parking.Scripts.DataEntities.Car
{
	[Serializable]
	[MemoryPackable(GenerateType.Object)]
	[GenerateTypeScript]
	[UnityEngine.Scripting.Preserve]
	public class CarInfoForDatabase : CarInfo, IMemoryPackable<CarInfoForDatabase>, IMemoryPackFormatterRegister
	{
		[MemoryPack.Internal.Preserve]
		private sealed class CarInfoForDatabaseFormatter : MemoryPackFormatter<CarInfoForDatabase>
		{
			[MemoryPack.Internal.Preserve]
			public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref CarInfoForDatabase value)
			{
				CarInfoForDatabase.Serialize(ref writer, ref value);
			}

			[MemoryPack.Internal.Preserve]
			public override void Deserialize(ref MemoryPackReader reader, ref CarInfoForDatabase value)
			{
				CarInfoForDatabase.Deserialize(ref reader, ref value);
			}
		}

		public List<VinylData> Vinyls;

		public List<VinylData> Window;

		[MemoryPackConstructor]
		public CarInfoForDatabase()
		{
		}

		public CarInfoForDatabase(CarInfo carInfo)
		{
			CarID = carInfo.CarID;
			InstalledBodyKits = carInfo.InstalledBodyKits;
			Colors = carInfo.Colors;
			AdditionalData = carInfo.AdditionalData;
			CarInstanceId = carInfo.CarInstanceId;
			base.TuningData = carInfo.TuningData;
			BoughtBodyKits = carInfo.BoughtBodyKits;
		}

		static CarInfoForDatabase()
		{
			RegisterFormatter();
		}

		[MemoryPack.Internal.Preserve]
		public new static void RegisterFormatter()
		{
			if (!MemoryPackFormatterProvider.IsRegistered<CarInfoForDatabase>())
			{
				MemoryPackFormatterProvider.Register(new CarInfoForDatabaseFormatter());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<CarInfoForDatabase[]>())
			{
				MemoryPackFormatterProvider.Register(new ArrayFormatter<CarInfoForDatabase>());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<List<VinylData>>())
			{
				MemoryPackFormatterProvider.Register(new ListFormatter<VinylData>());
			}
		}

		[MemoryPack.Internal.Preserve]
		public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref CarInfoForDatabase? value) where TBufferWriter : class, IBufferWriter<byte>
		{
			if (value == null)
			{
				writer.WriteNullObjectHeader();
				return;
			}
			writer.WriteUnmanagedWithObjectHeader(9, in value.CarID);
			writer.WritePackable(in value.InstalledBodyKits);
			writer.WritePackable(in value.Colors);
			writer.WritePackable(in value.AdditionalData);
			writer.WriteString(value.CarInstanceId);
			writer.WritePackable<CarTuningData>(value.TuningData);
			writer.WritePackable(in value.BoughtBodyKits);
			ListFormatter.SerializePackable(ref writer, value.Vinyls);
			ListFormatter.SerializePackable(ref writer, value.Window);
		}

		[MemoryPack.Internal.Preserve]
		public static void Deserialize(ref MemoryPackReader reader, ref CarInfoForDatabase? value)
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
			List<VinylData> value8;
			List<VinylData> value9;
			string carInstanceId;
			if (memberCount == 9)
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
					value8 = value.Vinyls;
					value9 = value.Window;
					reader.ReadUnmanaged<int>(out value2);
					reader.ReadPackable(ref value3);
					reader.ReadPackable(ref value4);
					reader.ReadPackable(ref value5);
					carInstanceId = reader.ReadString();
					reader.ReadPackable(ref value6);
					reader.ReadPackable(ref value7);
					ListFormatter.DeserializePackable(ref reader, ref value8);
					ListFormatter.DeserializePackable(ref reader, ref value9);
					goto IL_01f8;
				}
				reader.ReadUnmanaged<int>(out value2);
				value3 = reader.ReadPackable<InstalledBodyKits>();
				value4 = reader.ReadPackable<Colors>();
				value5 = reader.ReadPackable<AdditionalData>();
				carInstanceId = reader.ReadString();
				value6 = reader.ReadPackable<CarTuningData>();
				value7 = reader.ReadPackable<BoughtBodyKits>();
				value8 = ListFormatter.DeserializePackable<VinylData>(ref reader);
				value9 = ListFormatter.DeserializePackable<VinylData>(ref reader);
			}
			else
			{
				if (memberCount > 9)
				{
					MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(CarInfoForDatabase), 9, memberCount);
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
					value8 = null;
					value9 = null;
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
					value8 = value.Vinyls;
					value9 = value.Window;
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
											if (memberCount != 7)
											{
												ListFormatter.DeserializePackable(ref reader, ref value8);
												if (memberCount != 8)
												{
													ListFormatter.DeserializePackable(ref reader, ref value9);
													_ = 9;
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
					goto IL_01f8;
				}
			}
			value = new CarInfoForDatabase
			{
				CarID = value2,
				InstalledBodyKits = value3,
				Colors = value4,
				AdditionalData = value5,
				CarInstanceId = carInstanceId,
				TuningData = value6,
				BoughtBodyKits = value7,
				Vinyls = value8,
				Window = value9
			};
			return;
			IL_01f8:
			value.CarID = value2;
			value.InstalledBodyKits = value3;
			value.Colors = value4;
			value.AdditionalData = value5;
			value.CarInstanceId = carInstanceId;
			value.TuningData = value6;
			value.BoughtBodyKits = value7;
			value.Vinyls = value8;
			value.Window = value9;
		}
	}
}

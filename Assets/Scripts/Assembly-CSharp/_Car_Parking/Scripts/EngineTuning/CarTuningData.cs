using System;
using System.Buffers;
using System.Reflection;
using CPM.Rating.Entities;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using UnityEngine;
using _Car_Parking.Scripts.EngineTuning.DataTypes;

namespace _Car_Parking.Scripts.EngineTuning
{
	[Serializable]
	[GenerateTypeScript]
	[MemoryPackable(GenerateType.Object)]
	public class CarTuningData : IMemoryPackable<CarTuningData>, IMemoryPackFormatterRegister
	{
		[Preserve]
		private sealed class CarTuningDataFormatter : MemoryPackFormatter<CarTuningData>
		{
			[Preserve]
			public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref CarTuningData value)
			{
				CarTuningData.Serialize(ref writer, ref value);
			}

			[Preserve]
			public override void Deserialize(ref MemoryPackReader reader, ref CarTuningData value)
			{
				CarTuningData.Deserialize(ref reader, ref value);
			}
		}

		[Header("Performance Points")]
		public int PerformancePoints;

		[Header("Engine")]
		public EngineKitData EngineKitData;

		[Header("Electrical")]
		public PartData ECU;

		[Header("Gearbox")]
		public PartData GearBox;

		[Header("Differentials")]
		public PartData FrontLSD;

		public PartData RearLSD;

		[Header("Front Suspension")]
		public FrontWheelData FrontWheelData = new FrontWheelData();

		[Header("Rear Suspension")]
		public RearWheelData RearWheelData = new RearWheelData();

		[Header("Maintenance")]
		[MaintenanceInfoFormatter.MemoryPackCustomFormatterAttributeMaintenanceInfo]
		public MaintenanceInfo Maintenance;

		[Header("FromDynostand")]
		public DynamicTuningSettings DynamicTuningSettings = new DynamicTuningSettings();

		private static readonly IMemoryPackFormatter<MaintenanceInfo> __MaintenanceFormatter;

		static CarTuningData()
		{
			__MaintenanceFormatter = typeof(CarTuningData).GetField("Maintenance", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).GetCustomAttribute<MaintenanceInfoFormatter.MemoryPackCustomFormatterAttributeMaintenanceInfo>().GetFormatter();
			RegisterFormatter();
		}

		[Preserve]
		public static void RegisterFormatter()
		{
			if (!MemoryPackFormatterProvider.IsRegistered<CarTuningData>())
			{
				MemoryPackFormatterProvider.Register(new CarTuningDataFormatter());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<CarTuningData[]>())
			{
				MemoryPackFormatterProvider.Register(new ArrayFormatter<CarTuningData>());
			}
		}

		[Preserve]
		public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref CarTuningData? value) where TBufferWriter : class, IBufferWriter<byte>
		{
			if (value == null)
			{
				writer.WriteNullObjectHeader();
				return;
			}
			writer.WriteUnmanagedWithObjectHeader(10, in value.PerformancePoints);
			writer.WritePackable(in value.EngineKitData);
			writer.WritePackable(in value.ECU);
			writer.WritePackable(in value.GearBox);
			writer.WritePackable(in value.FrontLSD);
			writer.WritePackable(in value.RearLSD);
			writer.WritePackable(in value.FrontWheelData);
			writer.WritePackable(in value.RearWheelData);
			writer.WriteValueWithFormatter(__MaintenanceFormatter, in value.Maintenance);
			writer.WritePackable(in value.DynamicTuningSettings);
		}

		[Preserve]
		public static void Deserialize(ref MemoryPackReader reader, ref CarTuningData? value)
		{
			if (!reader.TryReadObjectHeader(out var memberCount))
			{
				value = null;
				return;
			}
			int value2;
			EngineKitData value3;
			PartData value4;
			PartData value5;
			PartData value6;
			PartData value7;
			FrontWheelData value8;
			RearWheelData value9;
			MaintenanceInfo value10;
			DynamicTuningSettings value11;
			if (memberCount == 10)
			{
				if (value != null)
				{
					value2 = value.PerformancePoints;
					value3 = value.EngineKitData;
					value4 = value.ECU;
					value5 = value.GearBox;
					value6 = value.FrontLSD;
					value7 = value.RearLSD;
					value8 = value.FrontWheelData;
					value9 = value.RearWheelData;
					value10 = value.Maintenance;
					value11 = value.DynamicTuningSettings;
					reader.ReadUnmanaged<int>(out value2);
					reader.ReadPackable(ref value3);
					reader.ReadPackable(ref value4);
					reader.ReadPackable(ref value5);
					reader.ReadPackable(ref value6);
					reader.ReadPackable(ref value7);
					reader.ReadPackable(ref value8);
					reader.ReadPackable(ref value9);
					reader.ReadValueWithFormatter(__MaintenanceFormatter, ref value10);
					reader.ReadPackable(ref value11);
					goto IL_023e;
				}
				reader.ReadUnmanaged<int>(out value2);
				value3 = reader.ReadPackable<EngineKitData>();
				value4 = reader.ReadPackable<PartData>();
				value5 = reader.ReadPackable<PartData>();
				value6 = reader.ReadPackable<PartData>();
				value7 = reader.ReadPackable<PartData>();
				value8 = reader.ReadPackable<FrontWheelData>();
				value9 = reader.ReadPackable<RearWheelData>();
				value10 = reader.ReadValueWithFormatter<IMemoryPackFormatter<MaintenanceInfo>, MaintenanceInfo>(__MaintenanceFormatter);
				value11 = reader.ReadPackable<DynamicTuningSettings>();
			}
			else
			{
				if (memberCount > 10)
				{
					MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(CarTuningData), 10, memberCount);
					return;
				}
				if (value == null)
				{
					value2 = 0;
					value3 = null;
					value4 = null;
					value5 = null;
					value6 = null;
					value7 = null;
					value8 = null;
					value9 = null;
					value10 = default;
					value11 = null;
				}
				else
				{
					value2 = value.PerformancePoints;
					value3 = value.EngineKitData;
					value4 = value.ECU;
					value5 = value.GearBox;
					value6 = value.FrontLSD;
					value7 = value.RearLSD;
					value8 = value.FrontWheelData;
					value9 = value.RearWheelData;
					value10 = value.Maintenance;
					value11 = value.DynamicTuningSettings;
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
									reader.ReadPackable(ref value6);
									if (memberCount != 5)
									{
										reader.ReadPackable(ref value7);
										if (memberCount != 6)
										{
											reader.ReadPackable(ref value8);
											if (memberCount != 7)
											{
												reader.ReadPackable(ref value9);
												if (memberCount != 8)
												{
													reader.ReadValueWithFormatter(__MaintenanceFormatter, ref value10);
													if (memberCount != 9)
													{
														reader.ReadPackable(ref value11);
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
					goto IL_023e;
				}
			}
			value = new CarTuningData
			{
				PerformancePoints = value2,
				EngineKitData = value3,
				ECU = value4,
				GearBox = value5,
				FrontLSD = value6,
				RearLSD = value7,
				FrontWheelData = value8,
				RearWheelData = value9,
				Maintenance = value10,
				DynamicTuningSettings = value11
			};
			return;
			IL_023e:
			value.PerformancePoints = value2;
			value.EngineKitData = value3;
			value.ECU = value4;
			value.GearBox = value5;
			value.FrontLSD = value6;
			value.RearLSD = value7;
			value.FrontWheelData = value8;
			value.RearWheelData = value9;
			value.Maintenance = value10;
			value.DynamicTuningSettings = value11;
		}
	}
}

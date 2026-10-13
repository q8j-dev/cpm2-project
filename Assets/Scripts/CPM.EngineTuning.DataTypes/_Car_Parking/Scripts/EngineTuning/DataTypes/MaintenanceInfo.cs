using System;
using System.Buffers;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;

namespace _Car_Parking.Scripts.EngineTuning.DataTypes
{
	[Serializable]
	[MemoryPackable(GenerateType.Object)]
	public struct MaintenanceInfo : ICloneable, IMemoryPackable<MaintenanceInfo>, IMemoryPackFormatterRegister
	{
		[Preserve]
		private sealed class MaintenanceInfoFormatter : MemoryPackFormatter<MaintenanceInfo>
		{
			[Preserve]
			public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref MaintenanceInfo value)
			{
				MaintenanceInfo.Serialize(ref writer, ref value);
			}

			[Preserve]
			public override void Deserialize(ref MemoryPackReader reader, ref MaintenanceInfo value)
			{
				MaintenanceInfo.Deserialize(ref reader, ref value);
			}
		}

		public float FuelInTank;

		public float OilCapacity;

		public float OilLife;

		public float[] FrontTyreHealth;

		public float[] RearTyreHealth;

		public float Mileage;

		public object Clone()
		{
			return MemberwiseClone();
		}

		static MaintenanceInfo()
		{
			RegisterFormatter();
		}

		[Preserve]
		public static void RegisterFormatter()
		{
			if (!MemoryPackFormatterProvider.IsRegistered<MaintenanceInfo>())
			{
				MemoryPackFormatterProvider.Register(new MaintenanceInfoFormatter());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<MaintenanceInfo[]>())
			{
				MemoryPackFormatterProvider.Register(new ArrayFormatter<MaintenanceInfo>());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<float[]>())
			{
				MemoryPackFormatterProvider.Register(new ArrayFormatter<float>());
			}
		}

		[Preserve]
		public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref MaintenanceInfo value) where TBufferWriter : class, IBufferWriter<byte>
		{
			writer.WriteUnmanagedWithObjectHeader(6, in value.FuelInTank, in value.OilCapacity, in value.OilLife);
			writer.WriteUnmanagedArray(value.FrontTyreHealth);
			writer.WriteUnmanagedArray(value.RearTyreHealth);
			writer.WriteUnmanaged(in value.Mileage);
		}

		[Preserve]
		public static void Deserialize(ref MemoryPackReader reader, ref MaintenanceInfo value)
		{
			if (!reader.TryReadObjectHeader(out var memberCount))
			{
				value = default;
				return;
			}
			float value2;
			float value3;
			float value4;
			float[] value5;
			float[] value6;
			float value7;
			if (memberCount == 6)
			{
				reader.ReadUnmanaged<float, float, float>(out value2, out value3, out value4);
				value5 = reader.ReadUnmanagedArray<float>();
				value6 = reader.ReadUnmanagedArray<float>();
				reader.ReadUnmanaged<float>(out value7);
			}
			else
			{
				if (memberCount > 6)
				{
					MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(MaintenanceInfo), 6, memberCount);
					return;
				}
				value2 = 0f;
				value3 = 0f;
				value4 = 0f;
				value5 = null;
				value6 = null;
				value7 = 0f;
				if (memberCount != 0)
				{
					reader.ReadUnmanaged<float>(out value2);
					if (memberCount != 1)
					{
						reader.ReadUnmanaged<float>(out value3);
						if (memberCount != 2)
						{
							reader.ReadUnmanaged<float>(out value4);
							if (memberCount != 3)
							{
								reader.ReadUnmanagedArray(ref value5);
								if (memberCount != 4)
								{
									reader.ReadUnmanagedArray(ref value6);
									if (memberCount != 5)
									{
										reader.ReadUnmanaged<float>(out value7);
										_ = 6;
									}
								}
							}
						}
					}
				}
			}
			value = new MaintenanceInfo
			{
				FuelInTank = value2,
				OilCapacity = value3,
				OilLife = value4,
				FrontTyreHealth = value5,
				RearTyreHealth = value6,
				Mileage = value7
			};
		}
	}
}

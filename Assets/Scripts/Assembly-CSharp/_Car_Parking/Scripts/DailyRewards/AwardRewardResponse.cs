using System;
using System.Buffers;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;

namespace _Car_Parking.Scripts.DailyRewards
{
	[Serializable]
	[MemoryPackable(GenerateType.Object)]
	[GenerateTypeScript]
	public class AwardRewardResponse : IMemoryPackable<AwardRewardResponse>, IMemoryPackFormatterRegister
	{
		[Preserve]
		private sealed class AwardRewardResponseFormatter : MemoryPackFormatter<AwardRewardResponse>
		{
			[Preserve]
			public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref AwardRewardResponse value)
			{
				AwardRewardResponse.Serialize(ref writer, ref value);
			}

			[Preserve]
			public override void Deserialize(ref MemoryPackReader reader, ref AwardRewardResponse value)
			{
				AwardRewardResponse.Deserialize(ref reader, ref value);
			}
		}

		public int? NewMoney;

		public int? NewCoins;

		public int? NewCar;

		public string? CarInstanceId;

		public override string ToString()
		{
			return $"NewMoney:{NewMoney}, NewCoins:{NewCoins}, NewCar:{NewCar}";
		}

		static AwardRewardResponse()
		{
			RegisterFormatter();
		}

		[Preserve]
		public static void RegisterFormatter()
		{
			if (!MemoryPackFormatterProvider.IsRegistered<AwardRewardResponse>())
			{
				MemoryPackFormatterProvider.Register(new AwardRewardResponseFormatter());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<AwardRewardResponse[]>())
			{
				MemoryPackFormatterProvider.Register(new ArrayFormatter<AwardRewardResponse>());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<int?>())
			{
				MemoryPackFormatterProvider.Register(new NullableFormatter<int>());
			}
		}

		[Preserve]
		public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref AwardRewardResponse? value) where TBufferWriter : class, IBufferWriter<byte>
		{
			if (value == null)
			{
				writer.WriteNullObjectHeader();
				return;
			}
			writer.DangerousWriteUnmanagedWithObjectHeader(4, in value.NewMoney, in value.NewCoins, in value.NewCar);
			writer.WriteString(value.CarInstanceId);
		}

		[Preserve]
		public static void Deserialize(ref MemoryPackReader reader, ref AwardRewardResponse? value)
		{
			if (!reader.TryReadObjectHeader(out var memberCount))
			{
				value = null;
				return;
			}
			int? value2;
			int? value3;
			int? value4;
			string carInstanceId;
			if (memberCount == 4)
			{
				if (value != null)
				{
					value2 = value.NewMoney;
					value3 = value.NewCoins;
					value4 = value.NewCar;
					carInstanceId = value.CarInstanceId;
					reader.DangerousReadUnmanaged<int?>(out value2);
					reader.DangerousReadUnmanaged<int?>(out value3);
					reader.DangerousReadUnmanaged<int?>(out value4);
					carInstanceId = reader.ReadString();
					goto IL_0104;
				}
				reader.DangerousReadUnmanaged<int?, int?, int?>(out value2, out value3, out value4);
				carInstanceId = reader.ReadString();
			}
			else
			{
				if (memberCount > 4)
				{
					MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(AwardRewardResponse), 4, memberCount);
					return;
				}
				if (value == null)
				{
					value2 = null;
					value3 = null;
					value4 = null;
					carInstanceId = null;
				}
				else
				{
					value2 = value.NewMoney;
					value3 = value.NewCoins;
					value4 = value.NewCar;
					carInstanceId = value.CarInstanceId;
				}
				if (memberCount != 0)
				{
					reader.DangerousReadUnmanaged<int?>(out value2);
					if (memberCount != 1)
					{
						reader.DangerousReadUnmanaged<int?>(out value3);
						if (memberCount != 2)
						{
							reader.DangerousReadUnmanaged<int?>(out value4);
							if (memberCount != 3)
							{
								carInstanceId = reader.ReadString();
								_ = 4;
							}
						}
					}
				}
				if (value != null)
				{
					goto IL_0104;
				}
			}
			value = new AwardRewardResponse
			{
				NewMoney = value2,
				NewCoins = value3,
				NewCar = value4,
				CarInstanceId = carInstanceId
			};
			return;
			IL_0104:
			value.NewMoney = value2;
			value.NewCoins = value3;
			value.NewCar = value4;
			value.CarInstanceId = carInstanceId;
		}
	}
}

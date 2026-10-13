using System;
using System.Buffers;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;

namespace _Car_Parking.Scripts.DataStorageService.PlayerData
{
	[Serializable]
	[GenerateTypeScript]
	[MemoryPackable(GenerateType.Object)]
	public class CircuitRacingRecord : IMemoryPackable<CircuitRacingRecord>, IMemoryPackFormatterRegister
	{
		[Preserve]
		private sealed class CircuitRacingRecordFormatter : MemoryPackFormatter<CircuitRacingRecord>
		{
			[Preserve]
			public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref CircuitRacingRecord value)
			{
				CircuitRacingRecord.Serialize(ref writer, ref value);
			}

			[Preserve]
			public override void Deserialize(ref MemoryPackReader reader, ref CircuitRacingRecord value)
			{
				CircuitRacingRecord.Deserialize(ref reader, ref value);
			}
		}

		public int ClassIndex;

		public int FirstPlace;

		public int SecondPlace;

		public int ThirdPlace;

		public float BestTime;

		static CircuitRacingRecord()
		{
			RegisterFormatter();
		}

		[Preserve]
		public static void RegisterFormatter()
		{
			if (!MemoryPackFormatterProvider.IsRegistered<CircuitRacingRecord>())
			{
				MemoryPackFormatterProvider.Register(new CircuitRacingRecordFormatter());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<CircuitRacingRecord[]>())
			{
				MemoryPackFormatterProvider.Register(new ArrayFormatter<CircuitRacingRecord>());
			}
		}

		[Preserve]
		public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref CircuitRacingRecord? value) where TBufferWriter : class, IBufferWriter<byte>
		{
			if (value == null)
			{
				writer.WriteNullObjectHeader();
			}
			else
			{
				writer.WriteUnmanagedWithObjectHeader(5, in value.ClassIndex, in value.FirstPlace, in value.SecondPlace, in value.ThirdPlace, in value.BestTime);
			}
		}

		[Preserve]
		public static void Deserialize(ref MemoryPackReader reader, ref CircuitRacingRecord? value)
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
			float value6;
			if (memberCount == 5)
			{
				if (value != null)
				{
					value2 = value.ClassIndex;
					value3 = value.FirstPlace;
					value4 = value.SecondPlace;
					value5 = value.ThirdPlace;
					value6 = value.BestTime;
					reader.ReadUnmanaged<int>(out value2);
					reader.ReadUnmanaged<int>(out value3);
					reader.ReadUnmanaged<int>(out value4);
					reader.ReadUnmanaged<int>(out value5);
					reader.ReadUnmanaged<float>(out value6);
					goto IL_011b;
				}
				reader.ReadUnmanaged<int, int, int, int, float>(out value2, out value3, out value4, out value5, out value6);
			}
			else
			{
				if (memberCount > 5)
				{
					MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(CircuitRacingRecord), 5, memberCount);
					return;
				}
				if (value == null)
				{
					value2 = 0;
					value3 = 0;
					value4 = 0;
					value5 = 0;
					value6 = 0f;
				}
				else
				{
					value2 = value.ClassIndex;
					value3 = value.FirstPlace;
					value4 = value.SecondPlace;
					value5 = value.ThirdPlace;
					value6 = value.BestTime;
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
									reader.ReadUnmanaged<float>(out value6);
									_ = 5;
								}
							}
						}
					}
				}
				if (value != null)
				{
					goto IL_011b;
				}
			}
			value = new CircuitRacingRecord
			{
				ClassIndex = value2,
				FirstPlace = value3,
				SecondPlace = value4,
				ThirdPlace = value5,
				BestTime = value6
			};
			return;
			IL_011b:
			value.ClassIndex = value2;
			value.FirstPlace = value3;
			value.SecondPlace = value4;
			value.ThirdPlace = value5;
			value.BestTime = value6;
		}
	}
}

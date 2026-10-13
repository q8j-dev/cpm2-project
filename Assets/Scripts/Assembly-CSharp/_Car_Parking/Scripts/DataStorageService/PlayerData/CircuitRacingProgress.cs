using System;
using System.Buffers;
using System.Collections.Generic;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;

namespace _Car_Parking.Scripts.DataStorageService.PlayerData
{
	[Serializable]
	[GenerateTypeScript]
	[MemoryPackable(GenerateType.Object)]
	public class CircuitRacingProgress : IMemoryPackable<CircuitRacingProgress>, IMemoryPackFormatterRegister
	{
		[Preserve]
		private sealed class CircuitRacingProgressFormatter : MemoryPackFormatter<CircuitRacingProgress>
		{
			[Preserve]
			public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref CircuitRacingProgress value)
			{
				CircuitRacingProgress.Serialize(ref writer, ref value);
			}

			[Preserve]
			public override void Deserialize(ref MemoryPackReader reader, ref CircuitRacingProgress value)
			{
				CircuitRacingProgress.Deserialize(ref reader, ref value);
			}
		}

		public int RaceWin;

		public int RaceCount;

		public int MoneyEarned;

		public string LastMoneyEarnedDay;

		public Dictionary<int, List<CircuitRacingRecord>> RacingTrackRecords;

		public void Clear()
		{
			RaceWin = 0;
			RaceCount = 0;
			MoneyEarned = 0;
			LastMoneyEarnedDay = "";
			RacingTrackRecords?.Clear();
		}

		static CircuitRacingProgress()
		{
			RegisterFormatter();
		}

		[Preserve]
		public static void RegisterFormatter()
		{
			if (!MemoryPackFormatterProvider.IsRegistered<CircuitRacingProgress>())
			{
				MemoryPackFormatterProvider.Register(new CircuitRacingProgressFormatter());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<CircuitRacingProgress[]>())
			{
				MemoryPackFormatterProvider.Register(new ArrayFormatter<CircuitRacingProgress>());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<Dictionary<int, List<CircuitRacingRecord>>>())
			{
				MemoryPackFormatterProvider.Register(new DictionaryFormatter<int, List<CircuitRacingRecord>>());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<List<CircuitRacingRecord>>())
			{
				MemoryPackFormatterProvider.Register(new ListFormatter<CircuitRacingRecord>());
			}
		}

		[Preserve]
		public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref CircuitRacingProgress? value) where TBufferWriter : class, IBufferWriter<byte>
		{
			if (value == null)
			{
				writer.WriteNullObjectHeader();
				return;
			}
			writer.WriteUnmanagedWithObjectHeader(5, in value.RaceWin, in value.RaceCount, in value.MoneyEarned);
			writer.WriteString(value.LastMoneyEarnedDay);
			writer.WriteValue(in value.RacingTrackRecords);
		}

		[Preserve]
		public static void Deserialize(ref MemoryPackReader reader, ref CircuitRacingProgress? value)
		{
			if (!reader.TryReadObjectHeader(out var memberCount))
			{
				value = null;
				return;
			}
			int value2;
			int value3;
			int value4;
			Dictionary<int, List<CircuitRacingRecord>> value5;
			string lastMoneyEarnedDay;
			if (memberCount == 5)
			{
				if (value != null)
				{
					value2 = value.RaceWin;
					value3 = value.RaceCount;
					value4 = value.MoneyEarned;
					lastMoneyEarnedDay = value.LastMoneyEarnedDay;
					value5 = value.RacingTrackRecords;
					reader.ReadUnmanaged<int>(out value2);
					reader.ReadUnmanaged<int>(out value3);
					reader.ReadUnmanaged<int>(out value4);
					lastMoneyEarnedDay = reader.ReadString();
					reader.ReadValue(ref value5);
					goto IL_0123;
				}
				reader.ReadUnmanaged<int, int, int>(out value2, out value3, out value4);
				lastMoneyEarnedDay = reader.ReadString();
				value5 = reader.ReadValue<Dictionary<int, List<CircuitRacingRecord>>>();
			}
			else
			{
				if (memberCount > 5)
				{
					MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(CircuitRacingProgress), 5, memberCount);
					return;
				}
				if (value == null)
				{
					value2 = 0;
					value3 = 0;
					value4 = 0;
					lastMoneyEarnedDay = null;
					value5 = null;
				}
				else
				{
					value2 = value.RaceWin;
					value3 = value.RaceCount;
					value4 = value.MoneyEarned;
					lastMoneyEarnedDay = value.LastMoneyEarnedDay;
					value5 = value.RacingTrackRecords;
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
								lastMoneyEarnedDay = reader.ReadString();
								if (memberCount != 4)
								{
									reader.ReadValue(ref value5);
									_ = 5;
								}
							}
						}
					}
				}
				if (value != null)
				{
					goto IL_0123;
				}
			}
			value = new CircuitRacingProgress
			{
				RaceWin = value2,
				RaceCount = value3,
				MoneyEarned = value4,
				LastMoneyEarnedDay = lastMoneyEarnedDay,
				RacingTrackRecords = value5
			};
			return;
			IL_0123:
			value.RaceWin = value2;
			value.RaceCount = value3;
			value.MoneyEarned = value4;
			value.LastMoneyEarnedDay = lastMoneyEarnedDay;
			value.RacingTrackRecords = value5;
		}
	}
}

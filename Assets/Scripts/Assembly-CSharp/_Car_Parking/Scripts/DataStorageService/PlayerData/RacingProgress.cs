#nullable enable annotations
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
	public class RacingProgress : IMemoryPackable<RacingProgress>, IMemoryPackFormatterRegister
	{
		[Preserve]
		private sealed class RacingProgressFormatter : MemoryPackFormatter<RacingProgress>
		{
			[Preserve]
			public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref RacingProgress value)
			{
				RacingProgress.Serialize(ref writer, ref value);
			}

			[Preserve]
			public override void Deserialize(ref MemoryPackReader reader, ref RacingProgress value)
			{
				RacingProgress.Deserialize(ref reader, ref value);
			}
		}

		public OneRaceResult BestResult;

		public int RaceWin;

		public int RaceCount;

		public List<RacingClassRecord> RacingClassRecords;

		public List<RacingClassRecord> ManualRacingClassRecords;

		public void Clear()
		{
			BestResult?.Clear();
			RaceWin = 0;
			RaceCount = 0;
			RacingClassRecords?.Clear();
			ManualRacingClassRecords?.Clear();
		}

		static RacingProgress()
		{
			RegisterFormatter();
		}

		[Preserve]
		public static void RegisterFormatter()
		{
			if (!MemoryPackFormatterProvider.IsRegistered<RacingProgress>())
			{
				MemoryPackFormatterProvider.Register(new RacingProgressFormatter());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<RacingProgress[]>())
			{
				MemoryPackFormatterProvider.Register(new ArrayFormatter<RacingProgress>());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<List<RacingClassRecord>>())
			{
				MemoryPackFormatterProvider.Register(new ListFormatter<RacingClassRecord>());
			}
		}

		[Preserve]
		public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref RacingProgress? value) where TBufferWriter : class, IBufferWriter<byte>
		{
			if (value == null)
			{
				writer.WriteNullObjectHeader();
				return;
			}
			writer.WriteObjectHeader(5);
			writer.WritePackable(in value.BestResult);
			writer.WriteUnmanaged(in value.RaceWin, in value.RaceCount);
			ListFormatter.SerializePackable(ref writer, value.RacingClassRecords);
			ListFormatter.SerializePackable(ref writer, value.ManualRacingClassRecords);
		}

		[Preserve]
		public static void Deserialize(ref MemoryPackReader reader, ref RacingProgress? value)
		{
			if (!reader.TryReadObjectHeader(out var memberCount))
			{
				value = null;
				return;
			}
			OneRaceResult value2;
			int value3;
			int value4;
			List<RacingClassRecord> value5;
			List<RacingClassRecord> value6;
			if (memberCount == 5)
			{
				if (value != null)
				{
					value2 = value.BestResult;
					value3 = value.RaceWin;
					value4 = value.RaceCount;
					value5 = value.RacingClassRecords;
					value6 = value.ManualRacingClassRecords;
					reader.ReadPackable(ref value2);
					reader.ReadUnmanaged<int>(out value3);
					reader.ReadUnmanaged<int>(out value4);
					ListFormatter.DeserializePackable(ref reader, ref value5);
					ListFormatter.DeserializePackable(ref reader, ref value6);
					goto IL_012b;
				}
				value2 = reader.ReadPackable<OneRaceResult>();
				reader.ReadUnmanaged<int, int>(out value3, out value4);
				value5 = ListFormatter.DeserializePackable<RacingClassRecord>(ref reader);
				value6 = ListFormatter.DeserializePackable<RacingClassRecord>(ref reader);
			}
			else
			{
				if (memberCount > 5)
				{
					MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(RacingProgress), 5, memberCount);
					return;
				}
				if (value == null)
				{
					value2 = null;
					value3 = 0;
					value4 = 0;
					value5 = null;
					value6 = null;
				}
				else
				{
					value2 = value.BestResult;
					value3 = value.RaceWin;
					value4 = value.RaceCount;
					value5 = value.RacingClassRecords;
					value6 = value.ManualRacingClassRecords;
				}
				if (memberCount != 0)
				{
					reader.ReadPackable(ref value2);
					if (memberCount != 1)
					{
						reader.ReadUnmanaged<int>(out value3);
						if (memberCount != 2)
						{
							reader.ReadUnmanaged<int>(out value4);
							if (memberCount != 3)
							{
								ListFormatter.DeserializePackable(ref reader, ref value5);
								if (memberCount != 4)
								{
									ListFormatter.DeserializePackable(ref reader, ref value6);
									_ = 5;
								}
							}
						}
					}
				}
				if (value != null)
				{
					goto IL_012b;
				}
			}
			value = new RacingProgress
			{
				BestResult = value2,
				RaceWin = value3,
				RaceCount = value4,
				RacingClassRecords = value5,
				ManualRacingClassRecords = value6
			};
			return;
			IL_012b:
			value.BestResult = value2;
			value.RaceWin = value3;
			value.RaceCount = value4;
			value.RacingClassRecords = value5;
			value.ManualRacingClassRecords = value6;
		}
	}
}

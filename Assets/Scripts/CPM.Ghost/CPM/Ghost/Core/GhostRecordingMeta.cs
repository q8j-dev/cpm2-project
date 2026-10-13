using System.Buffers;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using UnityEngine.Scripting;

namespace CPM.Ghost.Core
{
	[UnityEngine.Scripting.Preserve]
	[MemoryPackable(GenerateType.Object)]
	public class GhostRecordingMeta : IMemoryPackable<GhostRecordingMeta>, IMemoryPackFormatterRegister
	{
		[MemoryPack.Internal.Preserve]
		private sealed class GhostRecordingMetaFormatter : MemoryPackFormatter<GhostRecordingMeta>
		{
			[MemoryPack.Internal.Preserve]
			public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref GhostRecordingMeta value)
			{
				GhostRecordingMeta.Serialize(ref writer, ref value);
			}

			[MemoryPack.Internal.Preserve]
			public override void Deserialize(ref MemoryPackReader reader, ref GhostRecordingMeta value)
			{
				GhostRecordingMeta.Deserialize(ref reader, ref value);
			}
		}

		public string ReplayId;

		public string RaceKey;

		public int CarId;

		public long TimeMs;

		public long RecordedAtUnixMs;

		static GhostRecordingMeta()
		{
			RegisterFormatter();
		}

		[MemoryPack.Internal.Preserve]
		public static void RegisterFormatter()
		{
			if (!MemoryPackFormatterProvider.IsRegistered<GhostRecordingMeta>())
			{
				MemoryPackFormatterProvider.Register(new GhostRecordingMetaFormatter());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<GhostRecordingMeta[]>())
			{
				MemoryPackFormatterProvider.Register(new ArrayFormatter<GhostRecordingMeta>());
			}
		}

		[MemoryPack.Internal.Preserve]
		public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref GhostRecordingMeta? value) where TBufferWriter : class, IBufferWriter<byte>
		{
			if (value == null)
			{
				writer.WriteNullObjectHeader();
				return;
			}
			writer.WriteObjectHeader(5);
			writer.WriteString(value.ReplayId);
			writer.WriteString(value.RaceKey);
			writer.WriteUnmanaged(in value.CarId, in value.TimeMs, in value.RecordedAtUnixMs);
		}

		[MemoryPack.Internal.Preserve]
		public static void Deserialize(ref MemoryPackReader reader, ref GhostRecordingMeta? value)
		{
			if (!reader.TryReadObjectHeader(out var memberCount))
			{
				value = null;
				return;
			}
			int value2;
			long value3;
			long value4;
			string replayId;
			string raceKey;
			if (memberCount == 5)
			{
				if (value != null)
				{
					replayId = value.ReplayId;
					raceKey = value.RaceKey;
					value2 = value.CarId;
					value3 = value.TimeMs;
					value4 = value.RecordedAtUnixMs;
					replayId = reader.ReadString();
					raceKey = reader.ReadString();
					reader.ReadUnmanaged<int>(out value2);
					reader.ReadUnmanaged<long>(out value3);
					reader.ReadUnmanaged<long>(out value4);
					goto IL_011f;
				}
				replayId = reader.ReadString();
				raceKey = reader.ReadString();
				reader.ReadUnmanaged<int, long, long>(out value2, out value3, out value4);
			}
			else
			{
				if (memberCount > 5)
				{
					MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(GhostRecordingMeta), 5, memberCount);
					return;
				}
				if (value == null)
				{
					replayId = null;
					raceKey = null;
					value2 = 0;
					value3 = 0L;
					value4 = 0L;
				}
				else
				{
					replayId = value.ReplayId;
					raceKey = value.RaceKey;
					value2 = value.CarId;
					value3 = value.TimeMs;
					value4 = value.RecordedAtUnixMs;
				}
				if (memberCount != 0)
				{
					replayId = reader.ReadString();
					if (memberCount != 1)
					{
						raceKey = reader.ReadString();
						if (memberCount != 2)
						{
							reader.ReadUnmanaged<int>(out value2);
							if (memberCount != 3)
							{
								reader.ReadUnmanaged<long>(out value3);
								if (memberCount != 4)
								{
									reader.ReadUnmanaged<long>(out value4);
									_ = 5;
								}
							}
						}
					}
				}
				if (value != null)
				{
					goto IL_011f;
				}
			}
			value = new GhostRecordingMeta
			{
				ReplayId = replayId,
				RaceKey = raceKey,
				CarId = value2,
				TimeMs = value3,
				RecordedAtUnixMs = value4
			};
			return;
			IL_011f:
			value.ReplayId = replayId;
			value.RaceKey = raceKey;
			value.CarId = value2;
			value.TimeMs = value3;
			value.RecordedAtUnixMs = value4;
		}
	}
}

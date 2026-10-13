#nullable enable annotations
using System.Buffers;
using System.Collections.Generic;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using UnityEngine.Scripting;

namespace CPM.Ghost.Core
{
	[MemoryPackable(GenerateType.Object)]
	[UnityEngine.Scripting.Preserve]
	public class GhostRecording : IMemoryPackable<GhostRecording>, IMemoryPackFormatterRegister
	{
		[MemoryPack.Internal.Preserve]
		private sealed class GhostRecordingFormatter : MemoryPackFormatter<GhostRecording>
		{
			[MemoryPack.Internal.Preserve]
			public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref GhostRecording value)
			{
				GhostRecording.Serialize(ref writer, ref value);
			}

			[MemoryPack.Internal.Preserve]
			public override void Deserialize(ref MemoryPackReader reader, ref GhostRecording value)
			{
				GhostRecording.Deserialize(ref reader, ref value);
			}
		}

		public string ReplayId;

		public string RaceKey;

		public int CarId;

		public long TimeMs;

		public long RecordedAtUnixMs;

		public int RecordHz;

		public List<GhostFrame> Frames = new List<GhostFrame>();

		static GhostRecording()
		{
			RegisterFormatter();
		}

		[MemoryPack.Internal.Preserve]
		public static void RegisterFormatter()
		{
			if (!MemoryPackFormatterProvider.IsRegistered<GhostRecording>())
			{
				MemoryPackFormatterProvider.Register(new GhostRecordingFormatter());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<GhostRecording[]>())
			{
				MemoryPackFormatterProvider.Register(new ArrayFormatter<GhostRecording>());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<List<GhostFrame>>())
			{
				MemoryPackFormatterProvider.Register(new ListFormatter<GhostFrame>());
			}
		}

		[MemoryPack.Internal.Preserve]
		public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref GhostRecording? value) where TBufferWriter : class, IBufferWriter<byte>
		{
			if (value == null)
			{
				writer.WriteNullObjectHeader();
				return;
			}
			writer.WriteObjectHeader(7);
			writer.WriteString(value.ReplayId);
			writer.WriteString(value.RaceKey);
			writer.WriteUnmanaged(in value.CarId, in value.TimeMs, in value.RecordedAtUnixMs, in value.RecordHz);
			ListFormatter.SerializePackable(ref writer, value.Frames);
		}

		[MemoryPack.Internal.Preserve]
		public static void Deserialize(ref MemoryPackReader reader, ref GhostRecording? value)
		{
			if (!reader.TryReadObjectHeader(out var memberCount))
			{
				value = null;
				return;
			}
			int value2;
			long value3;
			long value4;
			int value5;
			List<GhostFrame> value6;
			string replayId;
			string raceKey;
			if (memberCount == 7)
			{
				if (value != null)
				{
					replayId = value.ReplayId;
					raceKey = value.RaceKey;
					value2 = value.CarId;
					value3 = value.TimeMs;
					value4 = value.RecordedAtUnixMs;
					value5 = value.RecordHz;
					value6 = value.Frames;
					replayId = reader.ReadString();
					raceKey = reader.ReadString();
					reader.ReadUnmanaged<int>(out value2);
					reader.ReadUnmanaged<long>(out value3);
					reader.ReadUnmanaged<long>(out value4);
					reader.ReadUnmanaged<int>(out value5);
					ListFormatter.DeserializePackable(ref reader, ref value6);
					goto IL_017e;
				}
				replayId = reader.ReadString();
				raceKey = reader.ReadString();
				reader.ReadUnmanaged<int, long, long, int>(out value2, out value3, out value4, out value5);
				value6 = ListFormatter.DeserializePackable<GhostFrame>(ref reader);
			}
			else
			{
				if (memberCount > 7)
				{
					MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(GhostRecording), 7, memberCount);
					return;
				}
				if (value == null)
				{
					replayId = null;
					raceKey = null;
					value2 = 0;
					value3 = 0L;
					value4 = 0L;
					value5 = 0;
					value6 = null;
				}
				else
				{
					replayId = value.ReplayId;
					raceKey = value.RaceKey;
					value2 = value.CarId;
					value3 = value.TimeMs;
					value4 = value.RecordedAtUnixMs;
					value5 = value.RecordHz;
					value6 = value.Frames;
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
									if (memberCount != 5)
									{
										reader.ReadUnmanaged<int>(out value5);
										if (memberCount != 6)
										{
											ListFormatter.DeserializePackable(ref reader, ref value6);
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
					goto IL_017e;
				}
			}
			value = new GhostRecording
			{
				ReplayId = replayId,
				RaceKey = raceKey,
				CarId = value2,
				TimeMs = value3,
				RecordedAtUnixMs = value4,
				RecordHz = value5,
				Frames = value6
			};
			return;
			IL_017e:
			value.ReplayId = replayId;
			value.RaceKey = raceKey;
			value.CarId = value2;
			value.TimeMs = value3;
			value.RecordedAtUnixMs = value4;
			value.RecordHz = value5;
			value.Frames = value6;
		}
	}
}

using System;
using System.Buffers;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;

namespace CPM.Database
{
	[Serializable]
	[GenerateTypeScript]
	[MemoryPackable(GenerateType.Object)]
	public class PlateData : IMemoryPackable<PlateData>, IMemoryPackFormatterRegister
	{
		[Preserve]
		private sealed class PlateDataFormatter : MemoryPackFormatter<PlateData>
		{
			[Preserve]
			public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref PlateData value)
			{
				PlateData.Serialize(ref writer, ref value);
			}

			[Preserve]
			public override void Deserialize(ref MemoryPackReader reader, ref PlateData value)
			{
				PlateData.Deserialize(ref reader, ref value);
			}
		}

		public int plateId;

		public string frontCarUId;

		public string rearCarUId;

		[MemoryPackConstructor]
		public PlateData()
		{
		}

		public PlateData(int id)
		{
			plateId = id;
		}

		static PlateData()
		{
			RegisterFormatter();
		}

		[Preserve]
		public static void RegisterFormatter()
		{
			if (!MemoryPackFormatterProvider.IsRegistered<PlateData>())
			{
				MemoryPackFormatterProvider.Register(new PlateDataFormatter());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<PlateData[]>())
			{
				MemoryPackFormatterProvider.Register(new ArrayFormatter<PlateData>());
			}
		}

		[Preserve]
		public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref PlateData? value) where TBufferWriter : class, IBufferWriter<byte>
		{
			if (value == null)
			{
				writer.WriteNullObjectHeader();
				return;
			}
			writer.WriteUnmanagedWithObjectHeader(3, in value.plateId);
			writer.WriteString(value.frontCarUId);
			writer.WriteString(value.rearCarUId);
		}

		[Preserve]
		public static void Deserialize(ref MemoryPackReader reader, ref PlateData? value)
		{
			if (!reader.TryReadObjectHeader(out var memberCount))
			{
				value = null;
				return;
			}
			int value2;
			string text;
			string text2;
			if (memberCount == 3)
			{
				if (value != null)
				{
					value2 = value.plateId;
					text = value.frontCarUId;
					text2 = value.rearCarUId;
					reader.ReadUnmanaged<int>(out value2);
					text = reader.ReadString();
					text2 = reader.ReadString();
					goto IL_00c4;
				}
				reader.ReadUnmanaged<int>(out value2);
				text = reader.ReadString();
				text2 = reader.ReadString();
			}
			else
			{
				if (memberCount > 3)
				{
					MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(PlateData), 3, memberCount);
					return;
				}
				if (value == null)
				{
					value2 = 0;
					text = null;
					text2 = null;
				}
				else
				{
					value2 = value.plateId;
					text = value.frontCarUId;
					text2 = value.rearCarUId;
				}
				if (memberCount != 0)
				{
					reader.ReadUnmanaged<int>(out value2);
					if (memberCount != 1)
					{
						text = reader.ReadString();
						if (memberCount != 2)
						{
							text2 = reader.ReadString();
							_ = 3;
						}
					}
				}
				if (value != null)
				{
					goto IL_00c4;
				}
			}
			value = new PlateData
			{
				plateId = value2,
				frontCarUId = text,
				rearCarUId = text2
			};
			return;
			IL_00c4:
			value.plateId = value2;
			value.frontCarUId = text;
			value.rearCarUId = text2;
		}
	}
}

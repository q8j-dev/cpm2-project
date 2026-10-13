using System;
using System.Buffers;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;

namespace _Car_Parking.Scripts.EngineTuning
{
	[Serializable]
	[GenerateTypeScript]
	[MemoryPackable(GenerateType.Object)]
	public class PartData : IMemoryPackable<PartData>, IMemoryPackFormatterRegister
	{
		[Preserve]
		private sealed class PartDataFormatter : MemoryPackFormatter<PartData>
		{
			[Preserve]
			public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref PartData value)
			{
				PartData.Serialize(ref writer, ref value);
			}

			[Preserve]
			public override void Deserialize(ref MemoryPackReader reader, ref PartData value)
			{
				PartData.Deserialize(ref reader, ref value);
			}
		}

		public PartType PartType;

		public int InstanceID = -1;

		public int ModelId;

		public float Wear;

		static PartData()
		{
			RegisterFormatter();
		}

		[Preserve]
		public static void RegisterFormatter()
		{
			if (!MemoryPackFormatterProvider.IsRegistered<PartData>())
			{
				MemoryPackFormatterProvider.Register(new PartDataFormatter());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<PartData[]>())
			{
				MemoryPackFormatterProvider.Register(new ArrayFormatter<PartData>());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<PartType>())
			{
				MemoryPackFormatterProvider.Register(new UnmanagedFormatter<PartType>());
			}
		}

		[Preserve]
		public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref PartData? value) where TBufferWriter : class, IBufferWriter<byte>
		{
			if (value == null)
			{
				writer.WriteNullObjectHeader();
			}
			else
			{
				writer.WriteUnmanagedWithObjectHeader(4, in value.PartType, in value.InstanceID, in value.ModelId, in value.Wear);
			}
		}

		[Preserve]
		public static void Deserialize(ref MemoryPackReader reader, ref PartData? value)
		{
			if (!reader.TryReadObjectHeader(out var memberCount))
			{
				value = null;
				return;
			}
			PartType value2;
			int value3;
			int value4;
			float value5;
			if (memberCount == 4)
			{
				if (value != null)
				{
					value2 = value.PartType;
					value3 = value.InstanceID;
					value4 = value.ModelId;
					value5 = value.Wear;
					reader.ReadUnmanaged<PartType>(out value2);
					reader.ReadUnmanaged<int>(out value3);
					reader.ReadUnmanaged<int>(out value4);
					reader.ReadUnmanaged<float>(out value5);
					goto IL_00f0;
				}
				reader.ReadUnmanaged<PartType, int, int, float>(out value2, out value3, out value4, out value5);
			}
			else
			{
				if (memberCount > 4)
				{
					MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(PartData), 4, memberCount);
					return;
				}
				if (value == null)
				{
					value2 = PartType.None;
					value3 = 0;
					value4 = 0;
					value5 = 0f;
				}
				else
				{
					value2 = value.PartType;
					value3 = value.InstanceID;
					value4 = value.ModelId;
					value5 = value.Wear;
				}
				if (memberCount != 0)
				{
					reader.ReadUnmanaged<PartType>(out value2);
					if (memberCount != 1)
					{
						reader.ReadUnmanaged<int>(out value3);
						if (memberCount != 2)
						{
							reader.ReadUnmanaged<int>(out value4);
							if (memberCount != 3)
							{
								reader.ReadUnmanaged<float>(out value5);
								_ = 4;
							}
						}
					}
				}
				if (value != null)
				{
					goto IL_00f0;
				}
			}
			value = new PartData
			{
				PartType = value2,
				InstanceID = value3,
				ModelId = value4,
				Wear = value5
			};
			return;
			IL_00f0:
			value.PartType = value2;
			value.InstanceID = value3;
			value.ModelId = value4;
			value.Wear = value5;
		}
	}
}

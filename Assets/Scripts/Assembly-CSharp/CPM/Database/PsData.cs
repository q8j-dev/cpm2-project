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
	public class PsData : IMemoryPackable<PsData>, IMemoryPackFormatterRegister
	{
		[Preserve]
		private sealed class PsDataFormatter : MemoryPackFormatter<PsData>
		{
			[Preserve]
			public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref PsData value)
			{
				PsData.Serialize(ref writer, ref value);
			}

			[Preserve]
			public override void Deserialize(ref MemoryPackReader reader, ref PsData value)
			{
				PsData.Deserialize(ref reader, ref value);
			}
		}

		public bool Bought;

		public bool Installed;

		static PsData()
		{
			RegisterFormatter();
		}

		[Preserve]
		public static void RegisterFormatter()
		{
			if (!MemoryPackFormatterProvider.IsRegistered<PsData>())
			{
				MemoryPackFormatterProvider.Register(new PsDataFormatter());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<PsData[]>())
			{
				MemoryPackFormatterProvider.Register(new ArrayFormatter<PsData>());
			}
		}

		[Preserve]
		public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref PsData? value) where TBufferWriter : class, IBufferWriter<byte>
		{
			if (value == null)
			{
				writer.WriteNullObjectHeader();
			}
			else
			{
				writer.WriteUnmanagedWithObjectHeader(2, in value.Bought, in value.Installed);
			}
		}

		[Preserve]
		public static void Deserialize(ref MemoryPackReader reader, ref PsData? value)
		{
			if (!reader.TryReadObjectHeader(out var memberCount))
			{
				value = null;
				return;
			}
			bool value2;
			bool value3;
			if (memberCount == 2)
			{
				if (value != null)
				{
					value2 = value.Bought;
					value3 = value.Installed;
					reader.ReadUnmanaged<bool>(out value2);
					reader.ReadUnmanaged<bool>(out value3);
					goto IL_0096;
				}
				reader.ReadUnmanaged<bool, bool>(out value2, out value3);
			}
			else
			{
				if (memberCount > 2)
				{
					MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(PsData), 2, memberCount);
					return;
				}
				if (value == null)
				{
					value2 = false;
					value3 = false;
				}
				else
				{
					value2 = value.Bought;
					value3 = value.Installed;
				}
				if (memberCount != 0)
				{
					reader.ReadUnmanaged<bool>(out value2);
					if (memberCount != 1)
					{
						reader.ReadUnmanaged<bool>(out value3);
						_ = 2;
					}
				}
				if (value != null)
				{
					goto IL_0096;
				}
			}
			value = new PsData
			{
				Bought = value2,
				Installed = value3
			};
			return;
			IL_0096:
			value.Bought = value2;
			value.Installed = value3;
		}
	}
}

#nullable enable annotations
using System;
using System.Buffers;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using UnityEngine.Scripting;

namespace CPM.Database
{
	[Serializable]
	[MemoryPackable(GenerateType.Object)]
	[GenerateTypeScript]
	[UnityEngine.Scripting.Preserve]
	public class FunctionResult : IMemoryPackable<FunctionResult>, IMemoryPackFormatterRegister
	{
		[MemoryPack.Internal.Preserve]
		private sealed class FunctionResultFormatter : MemoryPackFormatter<FunctionResult>
		{
			[MemoryPack.Internal.Preserve]
			public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref FunctionResult value)
			{
				FunctionResult.Serialize(ref writer, ref value);
			}

			[MemoryPack.Internal.Preserve]
			public override void Deserialize(ref MemoryPackReader reader, ref FunctionResult value)
			{
				FunctionResult.Deserialize(ref reader, ref value);
			}
		}

		static FunctionResult()
		{
			RegisterFormatter();
		}

		[MemoryPack.Internal.Preserve]
		public static void RegisterFormatter()
		{
			if (!MemoryPackFormatterProvider.IsRegistered<FunctionResult>())
			{
				MemoryPackFormatterProvider.Register(new FunctionResultFormatter());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<FunctionResult[]>())
			{
				MemoryPackFormatterProvider.Register(new ArrayFormatter<FunctionResult>());
			}
		}

		[MemoryPack.Internal.Preserve]
		public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref FunctionResult? value) where TBufferWriter : class, IBufferWriter<byte>
		{
			if (value == null)
			{
				writer.WriteNullObjectHeader();
			}
			else
			{
				writer.WriteObjectHeader(0);
			}
		}

		[MemoryPack.Internal.Preserve]
		public static void Deserialize(ref MemoryPackReader reader, ref FunctionResult? value)
		{
			if (!reader.TryReadObjectHeader(out var memberCount))
			{
				value = null;
				return;
			}
			if (memberCount == 0)
			{
				if (value != null)
				{
					return;
				}
			}
			else
			{
				if (memberCount > 0)
				{
					MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(FunctionResult), 0, memberCount);
					return;
				}
				_ = value;
				if (value != null)
				{
					return;
				}
			}
			value = new FunctionResult();
		}
	}
}

using System;
using System.Buffers;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;

[Serializable]
[GenerateTypeScript]
[MemoryPackable(GenerateType.Object)]
public class OneRaceResult : IMemoryPackable<OneRaceResult>, IMemoryPackFormatterRegister
{
	[Preserve]
	private sealed class OneRaceResultFormatter : MemoryPackFormatter<OneRaceResult>
	{
		[Preserve]
		public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref OneRaceResult value)
		{
			OneRaceResult.Serialize(ref writer, ref value);
		}

		[Preserve]
		public override void Deserialize(ref MemoryPackReader reader, ref OneRaceResult value)
		{
			OneRaceResult.Deserialize(ref reader, ref value);
		}
	}

	public float RT;

	public float quaterMilResult;

	public float mileResult;

	public float exitSpeed;

	public void Clear()
	{
		RT = 0f;
		quaterMilResult = 0f;
		mileResult = 0f;
		exitSpeed = 0f;
	}

	static OneRaceResult()
	{
		RegisterFormatter();
	}

	[Preserve]
	public static void RegisterFormatter()
	{
		if (!MemoryPackFormatterProvider.IsRegistered<OneRaceResult>())
		{
			MemoryPackFormatterProvider.Register(new OneRaceResultFormatter());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<OneRaceResult[]>())
		{
			MemoryPackFormatterProvider.Register(new ArrayFormatter<OneRaceResult>());
		}
	}

	[Preserve]
	public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref OneRaceResult? value) where TBufferWriter : class, IBufferWriter<byte>
	{
		if (value == null)
		{
			writer.WriteNullObjectHeader();
		}
		else
		{
			writer.WriteUnmanagedWithObjectHeader(4, in value.RT, in value.quaterMilResult, in value.mileResult, in value.exitSpeed);
		}
	}

	[Preserve]
	public static void Deserialize(ref MemoryPackReader reader, ref OneRaceResult? value)
	{
		if (!reader.TryReadObjectHeader(out var memberCount))
		{
			value = null;
			return;
		}
		float value2;
		float value3;
		float value4;
		float value5;
		if (memberCount == 4)
		{
			if (value != null)
			{
				value2 = value.RT;
				value3 = value.quaterMilResult;
				value4 = value.mileResult;
				value5 = value.exitSpeed;
				reader.ReadUnmanaged<float>(out value2);
				reader.ReadUnmanaged<float>(out value3);
				reader.ReadUnmanaged<float>(out value4);
				reader.ReadUnmanaged<float>(out value5);
				goto IL_00fc;
			}
			reader.ReadUnmanaged<float, float, float, float>(out value2, out value3, out value4, out value5);
		}
		else
		{
			if (memberCount > 4)
			{
				MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(OneRaceResult), 4, memberCount);
				return;
			}
			if (value == null)
			{
				value2 = 0f;
				value3 = 0f;
				value4 = 0f;
				value5 = 0f;
			}
			else
			{
				value2 = value.RT;
				value3 = value.quaterMilResult;
				value4 = value.mileResult;
				value5 = value.exitSpeed;
			}
			if (memberCount != 0)
			{
				reader.ReadUnmanaged<float>(out value2);
				if (memberCount != 1)
				{
					reader.ReadUnmanaged<float>(out value3);
					if (memberCount != 2)
					{
						reader.ReadUnmanaged<float>(out value4);
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
				goto IL_00fc;
			}
		}
		value = new OneRaceResult
		{
			RT = value2,
			quaterMilResult = value3,
			mileResult = value4,
			exitSpeed = value5
		};
		return;
		IL_00fc:
		value.RT = value2;
		value.quaterMilResult = value3;
		value.mileResult = value4;
		value.exitSpeed = value5;
	}
}

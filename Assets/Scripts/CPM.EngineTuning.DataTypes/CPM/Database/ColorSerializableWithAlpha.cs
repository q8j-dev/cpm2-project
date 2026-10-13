using System;
using System.Buffers;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using UnityEngine;

namespace CPM.Database
{
	[Serializable]
	[MemoryPackable(GenerateType.Object)]
	public struct ColorSerializableWithAlpha : IMemoryPackable<ColorSerializableWithAlpha>, IMemoryPackFormatterRegister
	{
		[Preserve]
		private sealed class ColorSerializableWithAlphaFormatter : MemoryPackFormatter<ColorSerializableWithAlpha>
		{
			[Preserve]
			public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref ColorSerializableWithAlpha value)
			{
				ColorSerializableWithAlpha.Serialize(ref writer, ref value);
			}

			[Preserve]
			public override void Deserialize(ref MemoryPackReader reader, ref ColorSerializableWithAlpha value)
			{
				ColorSerializableWithAlpha.Deserialize(ref reader, ref value);
			}
		}

		public byte R;

		public byte G;

		public byte B;

		public byte A;

		public static implicit operator ColorSerializableWithAlpha(Color color)
		{
			return new ColorSerializableWithAlpha
			{
				R = (byte)(color.r * 255f),
				G = (byte)(color.g * 255f),
				B = (byte)(color.b * 255f),
				A = (byte)(color.a * 255f)
			};
		}

		public static implicit operator Color(ColorSerializableWithAlpha colorSerializable)
		{
			return new Color((float)(int)colorSerializable.R / 255f, (float)(int)colorSerializable.G / 255f, (float)(int)colorSerializable.B / 255f, (float)(int)colorSerializable.A / 255f);
		}

		static ColorSerializableWithAlpha()
		{
			RegisterFormatter();
		}

		[Preserve]
		public static void RegisterFormatter()
		{
			if (!MemoryPackFormatterProvider.IsRegistered<ColorSerializableWithAlpha>())
			{
				MemoryPackFormatterProvider.Register(new ColorSerializableWithAlphaFormatter());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<ColorSerializableWithAlpha[]>())
			{
				MemoryPackFormatterProvider.Register(new ArrayFormatter<ColorSerializableWithAlpha>());
			}
		}

		[Preserve]
		public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref ColorSerializableWithAlpha value) where TBufferWriter : class, IBufferWriter<byte>
		{
			writer.WriteUnmanaged(in value);
		}

		[Preserve]
		public static void Deserialize(ref MemoryPackReader reader, ref ColorSerializableWithAlpha value)
		{
			reader.ReadUnmanaged<ColorSerializableWithAlpha>(out value);
		}
	}
}

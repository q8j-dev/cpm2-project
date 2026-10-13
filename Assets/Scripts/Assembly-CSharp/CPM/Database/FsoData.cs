using System;
using System.Buffers;
using System.Reflection;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using UnityEngine;
using _Car_Parking.Scripts;

namespace CPM.Database
{
	[Serializable]
	[MemoryPackable(GenerateType.Object)]
	public struct FsoData : IMemoryPackable<FsoData>, IMemoryPackFormatterRegister
	{
		[Preserve]
		private sealed class FsoDataFormatter : MemoryPackFormatter<FsoData>
		{
			[Preserve]
			public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref FsoData value)
			{
				FsoData.Serialize(ref writer, ref value);
			}

			[Preserve]
			public override void Deserialize(ref MemoryPackReader reader, ref FsoData value)
			{
				FsoData.Deserialize(ref reader, ref value);
			}
		}

		public int ModelIndex;

		public int PositionIndex;

		[ColorFormatter.MemoryPackCustomFormatterAttributeColor]
		public Color LeftColor;

		[ColorFormatter.MemoryPackCustomFormatterAttributeColor]
		public Color RightColor;

		private static readonly IMemoryPackFormatter<Color> __LeftColorFormatter;

		private static readonly IMemoryPackFormatter<Color> __RightColorFormatter;

		static FsoData()
		{
			__LeftColorFormatter = typeof(FsoData).GetField("LeftColor", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).GetCustomAttribute<ColorFormatter.MemoryPackCustomFormatterAttributeColor>().GetFormatter();
			__RightColorFormatter = typeof(FsoData).GetField("RightColor", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).GetCustomAttribute<ColorFormatter.MemoryPackCustomFormatterAttributeColor>().GetFormatter();
			RegisterFormatter();
		}

		[Preserve]
		public static void RegisterFormatter()
		{
			if (!MemoryPackFormatterProvider.IsRegistered<FsoData>())
			{
				MemoryPackFormatterProvider.Register(new FsoDataFormatter());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<FsoData[]>())
			{
				MemoryPackFormatterProvider.Register(new ArrayFormatter<FsoData>());
			}
		}

		[Preserve]
		public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref FsoData value) where TBufferWriter : class, IBufferWriter<byte>
		{
			writer.WriteUnmanaged(in value);
		}

		[Preserve]
		public static void Deserialize(ref MemoryPackReader reader, ref FsoData value)
		{
			reader.ReadUnmanaged<FsoData>(out value);
		}
	}
}

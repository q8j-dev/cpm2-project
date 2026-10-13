using System;
using System.Buffers;
using System.Collections.Generic;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;

namespace _Car_Parking.Scripts.VisualTuningFavourite
{
	[Serializable]
	[MemoryPackable(GenerateType.Object)]
	[GenerateTypeScript]
	public class VisualTuningFavouriteData : IMemoryPackable<VisualTuningFavouriteData>, IMemoryPackFormatterRegister
	{
		[Preserve]
		private sealed class VisualTuningFavouriteDataFormatter : MemoryPackFormatter<VisualTuningFavouriteData>
		{
			[Preserve]
			public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref VisualTuningFavouriteData value)
			{
				VisualTuningFavouriteData.Serialize(ref writer, ref value);
			}

			[Preserve]
			public override void Deserialize(ref MemoryPackReader reader, ref VisualTuningFavouriteData value)
			{
				VisualTuningFavouriteData.Deserialize(ref reader, ref value);
			}
		}

		public List<int> FavouriteVinyls;

		public List<int> FavouriteWheels;

		public VisualTuningFavouriteData(List<int> favouriteVinyls, List<int> favouriteWheels)
		{
			FavouriteVinyls = favouriteVinyls;
			FavouriteWheels = favouriteWheels;
		}

		public void Clear()
		{
			FavouriteVinyls?.Clear();
			FavouriteWheels?.Clear();
		}

		static VisualTuningFavouriteData()
		{
			RegisterFormatter();
		}

		[Preserve]
		public static void RegisterFormatter()
		{
			if (!MemoryPackFormatterProvider.IsRegistered<VisualTuningFavouriteData>())
			{
				MemoryPackFormatterProvider.Register(new VisualTuningFavouriteDataFormatter());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<VisualTuningFavouriteData[]>())
			{
				MemoryPackFormatterProvider.Register(new ArrayFormatter<VisualTuningFavouriteData>());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<List<int>>())
			{
				MemoryPackFormatterProvider.Register(new ListFormatter<int>());
			}
		}

		[Preserve]
		public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref VisualTuningFavouriteData? value) where TBufferWriter : class, IBufferWriter<byte>
		{
			if (value == null)
			{
				writer.WriteNullObjectHeader();
				return;
			}
			writer.WriteObjectHeader(2);
			writer.WriteValue(in value.FavouriteVinyls);
			writer.WriteValue(in value.FavouriteWheels);
		}

		[Preserve]
		public static void Deserialize(ref MemoryPackReader reader, ref VisualTuningFavouriteData? value)
		{
			if (!reader.TryReadObjectHeader(out var memberCount))
			{
				value = null;
				return;
			}
			List<int> value2;
			List<int> value3;
			if (memberCount == 2)
			{
				if (value == null)
				{
					value2 = reader.ReadValue<List<int>>();
					value3 = reader.ReadValue<List<int>>();
				}
				else
				{
					value2 = value.FavouriteVinyls;
					value3 = value.FavouriteWheels;
					reader.ReadValue(ref value2);
					reader.ReadValue(ref value3);
				}
			}
			else
			{
				if (memberCount > 2)
				{
					MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(VisualTuningFavouriteData), 2, memberCount);
					return;
				}
				if (value == null)
				{
					value2 = null;
					value3 = null;
				}
				else
				{
					value2 = value.FavouriteVinyls;
					value3 = value.FavouriteWheels;
				}
				if (memberCount != 0)
				{
					reader.ReadValue(ref value2);
					if (memberCount != 1)
					{
						reader.ReadValue(ref value3);
						_ = 2;
					}
				}
				_ = value;
			}
			value = new VisualTuningFavouriteData(value2, value3);
		}
	}
}

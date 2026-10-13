#nullable enable annotations
using System;
using System.Buffers;
using System.Collections.Generic;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using _Car_Parking.Scripts.EngineTuning;

namespace _Car_Parking.Scripts.Inventory
{
	[Serializable]
	[GenerateTypeScript]
	[MemoryPackable(GenerateType.Object)]
	public class EngineKitsInventoryData : IMemoryPackable<EngineKitsInventoryData>, IMemoryPackFormatterRegister
	{
		[Preserve]
		private sealed class EngineKitsInventoryDataFormatter : MemoryPackFormatter<EngineKitsInventoryData>
		{
			[Preserve]
			public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref EngineKitsInventoryData value)
			{
				EngineKitsInventoryData.Serialize(ref writer, ref value);
			}

			[Preserve]
			public override void Deserialize(ref MemoryPackReader reader, ref EngineKitsInventoryData value)
			{
				EngineKitsInventoryData.Deserialize(ref reader, ref value);
			}
		}

		public List<EngineKitData> Inventory = new List<EngineKitData>();

		static EngineKitsInventoryData()
		{
			RegisterFormatter();
		}

		[Preserve]
		public static void RegisterFormatter()
		{
			if (!MemoryPackFormatterProvider.IsRegistered<EngineKitsInventoryData>())
			{
				MemoryPackFormatterProvider.Register(new EngineKitsInventoryDataFormatter());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<EngineKitsInventoryData[]>())
			{
				MemoryPackFormatterProvider.Register(new ArrayFormatter<EngineKitsInventoryData>());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<List<EngineKitData>>())
			{
				MemoryPackFormatterProvider.Register(new ListFormatter<EngineKitData>());
			}
		}

		[Preserve]
		public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref EngineKitsInventoryData? value) where TBufferWriter : class, IBufferWriter<byte>
		{
			if (value == null)
			{
				writer.WriteNullObjectHeader();
				return;
			}
			writer.WriteObjectHeader(1);
			ListFormatter.SerializePackable(ref writer, value.Inventory);
		}

		[Preserve]
		public static void Deserialize(ref MemoryPackReader reader, ref EngineKitsInventoryData? value)
		{
			if (!reader.TryReadObjectHeader(out var memberCount))
			{
				value = null;
				return;
			}
			List<EngineKitData> value2;
			if (memberCount == 1)
			{
				if (value != null)
				{
					value2 = value.Inventory;
					ListFormatter.DeserializePackable(ref reader, ref value2);
					goto IL_006a;
				}
				value2 = ListFormatter.DeserializePackable<EngineKitData>(ref reader);
			}
			else
			{
				if (memberCount > 1)
				{
					MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(EngineKitsInventoryData), 1, memberCount);
					return;
				}
				value2 = ((value != null) ? value.Inventory : null);
				if (memberCount != 0)
				{
					ListFormatter.DeserializePackable(ref reader, ref value2);
					_ = 1;
				}
				if (value != null)
				{
					goto IL_006a;
				}
			}
			value = new EngineKitsInventoryData
			{
				Inventory = value2
			};
			return;
			IL_006a:
			value.Inventory = value2;
		}
	}
}

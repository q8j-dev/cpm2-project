#nullable enable annotations
using System;
using System.Buffers;
using System.Collections.Generic;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using UnityEngine.Scripting;
using _Car_Parking.Scripts.EngineTuning;

namespace _Car_Parking.Scripts.Inventory
{
	[Serializable]
	[GenerateTypeScript]
	[MemoryPackable(GenerateType.Object)]
	[UnityEngine.Scripting.Preserve]
	public class InventoryData : IMemoryPackable<InventoryData>, IMemoryPackFormatterRegister
	{
		[MemoryPack.Internal.Preserve]
		private sealed class InventoryDataFormatter : MemoryPackFormatter<InventoryData>
		{
			[MemoryPack.Internal.Preserve]
			public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref InventoryData value)
			{
				InventoryData.Serialize(ref writer, ref value);
			}

			[MemoryPack.Internal.Preserve]
			public override void Deserialize(ref MemoryPackReader reader, ref InventoryData value)
			{
				InventoryData.Deserialize(ref reader, ref value);
			}
		}

		public List<PartData> Inventory = new List<PartData>();

		static InventoryData()
		{
			RegisterFormatter();
		}

		[MemoryPack.Internal.Preserve]
		public static void RegisterFormatter()
		{
			if (!MemoryPackFormatterProvider.IsRegistered<InventoryData>())
			{
				MemoryPackFormatterProvider.Register(new InventoryDataFormatter());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<InventoryData[]>())
			{
				MemoryPackFormatterProvider.Register(new ArrayFormatter<InventoryData>());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<List<PartData>>())
			{
				MemoryPackFormatterProvider.Register(new ListFormatter<PartData>());
			}
		}

		[MemoryPack.Internal.Preserve]
		public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref InventoryData? value) where TBufferWriter : class, IBufferWriter<byte>
		{
			if (value == null)
			{
				writer.WriteNullObjectHeader();
				return;
			}
			writer.WriteObjectHeader(1);
			ListFormatter.SerializePackable(ref writer, value.Inventory);
		}

		[MemoryPack.Internal.Preserve]
		public static void Deserialize(ref MemoryPackReader reader, ref InventoryData? value)
		{
			if (!reader.TryReadObjectHeader(out var memberCount))
			{
				value = null;
				return;
			}
			List<PartData> value2;
			if (memberCount == 1)
			{
				if (value != null)
				{
					value2 = value.Inventory;
					ListFormatter.DeserializePackable(ref reader, ref value2);
					goto IL_006a;
				}
				value2 = ListFormatter.DeserializePackable<PartData>(ref reader);
			}
			else
			{
				if (memberCount > 1)
				{
					MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(InventoryData), 1, memberCount);
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
			value = new InventoryData
			{
				Inventory = value2
			};
			return;
			IL_006a:
			value.Inventory = value2;
		}
	}
}

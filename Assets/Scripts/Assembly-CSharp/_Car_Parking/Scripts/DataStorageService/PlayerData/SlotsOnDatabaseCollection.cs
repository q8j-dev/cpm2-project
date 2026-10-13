#nullable enable annotations
using System.Buffers;
using System.Collections.Generic;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using UnityEngine.Scripting;

namespace _Car_Parking.Scripts.DataStorageService.PlayerData
{
	[MemoryPackable(GenerateType.Object)]
	[GenerateTypeScript]
	[UnityEngine.Scripting.Preserve]
	public class SlotsOnDatabaseCollection : IMemoryPackable<SlotsOnDatabaseCollection>, IMemoryPackFormatterRegister
	{
		[MemoryPack.Internal.Preserve]
		private sealed class SlotsOnDatabaseCollectionFormatter : MemoryPackFormatter<SlotsOnDatabaseCollection>
		{
			[MemoryPack.Internal.Preserve]
			public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref SlotsOnDatabaseCollection value)
			{
				SlotsOnDatabaseCollection.Serialize(ref writer, ref value);
			}

			[MemoryPack.Internal.Preserve]
			public override void Deserialize(ref MemoryPackReader reader, ref SlotsOnDatabaseCollection value)
			{
				SlotsOnDatabaseCollection.Deserialize(ref reader, ref value);
			}
		}

		public Dictionary<int, string> Slots;

		[MemoryPackConstructor]
		public SlotsOnDatabaseCollection()
		{
			Slots = new Dictionary<int, string>();
		}

		public SlotsOnDatabaseCollection(Dictionary<int, string> slotCollectionForDatabase)
		{
			Slots = slotCollectionForDatabase;
		}

		static SlotsOnDatabaseCollection()
		{
			RegisterFormatter();
		}

		[MemoryPack.Internal.Preserve]
		public static void RegisterFormatter()
		{
			if (!MemoryPackFormatterProvider.IsRegistered<SlotsOnDatabaseCollection>())
			{
				MemoryPackFormatterProvider.Register(new SlotsOnDatabaseCollectionFormatter());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<SlotsOnDatabaseCollection[]>())
			{
				MemoryPackFormatterProvider.Register(new ArrayFormatter<SlotsOnDatabaseCollection>());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<Dictionary<int, string>>())
			{
				MemoryPackFormatterProvider.Register(new DictionaryFormatter<int, string>());
			}
		}

		[MemoryPack.Internal.Preserve]
		public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref SlotsOnDatabaseCollection? value) where TBufferWriter : class, IBufferWriter<byte>
		{
			if (value == null)
			{
				writer.WriteNullObjectHeader();
				return;
			}
			writer.WriteObjectHeader(1);
			writer.WriteValue(in value.Slots);
		}

		[MemoryPack.Internal.Preserve]
		public static void Deserialize(ref MemoryPackReader reader, ref SlotsOnDatabaseCollection? value)
		{
			if (!reader.TryReadObjectHeader(out var memberCount))
			{
				value = null;
				return;
			}
			Dictionary<int, string> value2;
			if (memberCount == 1)
			{
				if (value != null)
				{
					value2 = value.Slots;
					reader.ReadValue(ref value2);
					goto IL_006a;
				}
				value2 = reader.ReadValue<Dictionary<int, string>>();
			}
			else
			{
				if (memberCount > 1)
				{
					MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(SlotsOnDatabaseCollection), 1, memberCount);
					return;
				}
				value2 = ((value != null) ? value.Slots : null);
				if (memberCount != 0)
				{
					reader.ReadValue(ref value2);
					_ = 1;
				}
				if (value != null)
				{
					goto IL_006a;
				}
			}
			value = new SlotsOnDatabaseCollection
			{
				Slots = value2
			};
			return;
			IL_006a:
			value.Slots = value2;
		}
	}
}

using System.Buffers;
using System.Collections.Generic;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using UnityEngine.Scripting;

namespace _Car_Parking.Scripts.DataStorageService.PlayerData
{
	[UnityEngine.Scripting.Preserve]
	[MemoryPackable(GenerateType.Object)]
	[GenerateTypeScript]
	public class FriendsCollection : IMemoryPackable<FriendsCollection>, IMemoryPackFormatterRegister
	{
		[MemoryPack.Internal.Preserve]
		private sealed class FriendsCollectionFormatter : MemoryPackFormatter<FriendsCollection>
		{
			[MemoryPack.Internal.Preserve]
			public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref FriendsCollection value)
			{
				FriendsCollection.Serialize(ref writer, ref value);
			}

			[MemoryPack.Internal.Preserve]
			public override void Deserialize(ref MemoryPackReader reader, ref FriendsCollection value)
			{
				FriendsCollection.Deserialize(ref reader, ref value);
			}
		}

		public List<PrefFriendItem> FriendsID = new List<PrefFriendItem>();

		static FriendsCollection()
		{
			RegisterFormatter();
		}

		[MemoryPack.Internal.Preserve]
		public static void RegisterFormatter()
		{
			if (!MemoryPackFormatterProvider.IsRegistered<FriendsCollection>())
			{
				MemoryPackFormatterProvider.Register(new FriendsCollectionFormatter());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<FriendsCollection[]>())
			{
				MemoryPackFormatterProvider.Register(new ArrayFormatter<FriendsCollection>());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<List<PrefFriendItem>>())
			{
				MemoryPackFormatterProvider.Register(new ListFormatter<PrefFriendItem>());
			}
		}

		[MemoryPack.Internal.Preserve]
		public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref FriendsCollection? value) where TBufferWriter : class, IBufferWriter<byte>
		{
			if (value == null)
			{
				writer.WriteNullObjectHeader();
				return;
			}
			writer.WriteObjectHeader(1);
			ListFormatter.SerializePackable(ref writer, value.FriendsID);
		}

		[MemoryPack.Internal.Preserve]
		public static void Deserialize(ref MemoryPackReader reader, ref FriendsCollection? value)
		{
			if (!reader.TryReadObjectHeader(out var memberCount))
			{
				value = null;
				return;
			}
			List<PrefFriendItem> value2;
			if (memberCount == 1)
			{
				if (value != null)
				{
					value2 = value.FriendsID;
					ListFormatter.DeserializePackable(ref reader, ref value2);
					goto IL_006a;
				}
				value2 = ListFormatter.DeserializePackable<PrefFriendItem>(ref reader);
			}
			else
			{
				if (memberCount > 1)
				{
					MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(FriendsCollection), 1, memberCount);
					return;
				}
				value2 = ((value != null) ? value.FriendsID : null);
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
			value = new FriendsCollection
			{
				FriendsID = value2
			};
			return;
			IL_006a:
			value.FriendsID = value2;
		}
	}
}

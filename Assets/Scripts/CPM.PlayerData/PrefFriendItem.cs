using System.Buffers;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;

[GenerateTypeScript]
[MemoryPackable(GenerateType.Object)]
public class PrefFriendItem : IMemoryPackable<PrefFriendItem>, IMemoryPackFormatterRegister
{
	[Preserve]
	private sealed class PrefFriendItemFormatter : MemoryPackFormatter<PrefFriendItem>
	{
		[Preserve]
		public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref PrefFriendItem value)
		{
			PrefFriendItem.Serialize(ref writer, ref value);
		}

		[Preserve]
		public override void Deserialize(ref MemoryPackReader reader, ref PrefFriendItem value)
		{
			PrefFriendItem.Deserialize(ref reader, ref value);
		}
	}

	public string id;

	public string Name;

	public string accountID;

	static PrefFriendItem()
	{
		RegisterFormatter();
	}

	[Preserve]
	public static void RegisterFormatter()
	{
		if (!MemoryPackFormatterProvider.IsRegistered<PrefFriendItem>())
		{
			MemoryPackFormatterProvider.Register(new PrefFriendItemFormatter());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<PrefFriendItem[]>())
		{
			MemoryPackFormatterProvider.Register(new ArrayFormatter<PrefFriendItem>());
		}
	}

	[Preserve]
	public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref PrefFriendItem? value) where TBufferWriter : class, IBufferWriter<byte>
	{
		if (value == null)
		{
			writer.WriteNullObjectHeader();
			return;
		}
		writer.WriteObjectHeader(3);
		writer.WriteString(value.id);
		writer.WriteString(value.Name);
		writer.WriteString(value.accountID);
	}

	[Preserve]
	public static void Deserialize(ref MemoryPackReader reader, ref PrefFriendItem? value)
	{
		if (!reader.TryReadObjectHeader(out var memberCount))
		{
			value = null;
			return;
		}
		string text;
		string name;
		string text2;
		if (memberCount == 3)
		{
			if (value != null)
			{
				text = value.id;
				name = value.Name;
				text2 = value.accountID;
				text = reader.ReadString();
				name = reader.ReadString();
				text2 = reader.ReadString();
				goto IL_00c1;
			}
			text = reader.ReadString();
			name = reader.ReadString();
			text2 = reader.ReadString();
		}
		else
		{
			if (memberCount > 3)
			{
				MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(PrefFriendItem), 3, memberCount);
				return;
			}
			if (value == null)
			{
				text = null;
				name = null;
				text2 = null;
			}
			else
			{
				text = value.id;
				name = value.Name;
				text2 = value.accountID;
			}
			if (memberCount != 0)
			{
				text = reader.ReadString();
				if (memberCount != 1)
				{
					name = reader.ReadString();
					if (memberCount != 2)
					{
						text2 = reader.ReadString();
						_ = 3;
					}
				}
			}
			if (value != null)
			{
				goto IL_00c1;
			}
		}
		value = new PrefFriendItem
		{
			id = text,
			Name = name,
			accountID = text2
		};
		return;
		IL_00c1:
		value.id = text;
		value.Name = name;
		value.accountID = text2;
	}
}

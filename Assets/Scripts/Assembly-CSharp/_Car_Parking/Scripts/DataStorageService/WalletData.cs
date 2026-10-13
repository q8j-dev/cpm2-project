#nullable enable annotations
using System;
using System.Buffers;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using UnityEngine.Scripting;

namespace _Car_Parking.Scripts.DataStorageService
{
	[Serializable]
	[MemoryPackable(GenerateType.Object)]
	[GenerateTypeScript]
	[UnityEngine.Scripting.Preserve]
	public class WalletData : IMemoryPackable<WalletData>, IMemoryPackFormatterRegister
	{
		[MemoryPack.Internal.Preserve]
		private sealed class WalletDataFormatter : MemoryPackFormatter<WalletData>
		{
			[MemoryPack.Internal.Preserve]
			public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref WalletData value)
			{
				WalletData.Serialize(ref writer, ref value);
			}

			[MemoryPack.Internal.Preserve]
			public override void Deserialize(ref MemoryPackReader reader, ref WalletData value)
			{
				WalletData.Deserialize(ref reader, ref value);
			}
		}

		public int Money;

		[MemoryPackConstructor]
		public WalletData()
		{
			Money = 0;
		}

		public WalletData(int money)
		{
			Money = money;
		}

		static WalletData()
		{
			RegisterFormatter();
		}

		[MemoryPack.Internal.Preserve]
		public static void RegisterFormatter()
		{
			if (!MemoryPackFormatterProvider.IsRegistered<WalletData>())
			{
				MemoryPackFormatterProvider.Register(new WalletDataFormatter());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<WalletData[]>())
			{
				MemoryPackFormatterProvider.Register(new ArrayFormatter<WalletData>());
			}
		}

		[MemoryPack.Internal.Preserve]
		public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref WalletData? value) where TBufferWriter : class, IBufferWriter<byte>
		{
			if (value == null)
			{
				writer.WriteNullObjectHeader();
			}
			else
			{
				writer.WriteUnmanagedWithObjectHeader(1, in value.Money);
			}
		}

		[MemoryPack.Internal.Preserve]
		public static void Deserialize(ref MemoryPackReader reader, ref WalletData? value)
		{
			if (!reader.TryReadObjectHeader(out var memberCount))
			{
				value = null;
				return;
			}
			int value2;
			if (memberCount == 1)
			{
				if (value != null)
				{
					value2 = value.Money;
					reader.ReadUnmanaged<int>(out value2);
					goto IL_006b;
				}
				reader.ReadUnmanaged<int>(out value2);
			}
			else
			{
				if (memberCount > 1)
				{
					MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(WalletData), 1, memberCount);
					return;
				}
				value2 = ((value != null) ? value.Money : 0);
				if (memberCount != 0)
				{
					reader.ReadUnmanaged<int>(out value2);
					_ = 1;
				}
				if (value != null)
				{
					goto IL_006b;
				}
			}
			value = new WalletData
			{
				Money = value2
			};
			return;
			IL_006b:
			value.Money = value2;
		}
	}
}

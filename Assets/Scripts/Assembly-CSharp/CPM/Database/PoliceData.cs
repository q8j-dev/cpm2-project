#nullable enable annotations
using System;
using System.Buffers;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;

namespace CPM.Database
{
	[Serializable]
	[MemoryPackable(GenerateType.Object)]
	[GenerateTypeScript]
	public class PoliceData : IMemoryPackable<PoliceData>, IMemoryPackFormatterRegister
	{
		[Serializable]
		[MemoryPackable(GenerateType.Object)]
		[GenerateTypeScript]
		public class InstalledBodyKitsData : IMemoryPackable<InstalledBodyKitsData>, IMemoryPackFormatterRegister
		{
			[Preserve]
			private sealed class InstalledBodyKitsDataFormatter : MemoryPackFormatter<InstalledBodyKitsData>
			{
				[Preserve]
				public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref InstalledBodyKitsData value)
				{
					InstalledBodyKitsData.Serialize(ref writer, ref value);
				}

				[Preserve]
				public override void Deserialize(ref MemoryPackReader reader, ref InstalledBodyKitsData value)
				{
					InstalledBodyKitsData.Deserialize(ref reader, ref value);
				}
			}

			public int TopLight = -1;

			public int Roobar = -1;

			public int FrontInterior = -1;

			public int RearInterior = -1;

			public int Skirt = -1;

			static InstalledBodyKitsData()
			{
				RegisterFormatter();
			}

			[Preserve]
			public static void RegisterFormatter()
			{
				if (!MemoryPackFormatterProvider.IsRegistered<InstalledBodyKitsData>())
				{
					MemoryPackFormatterProvider.Register(new InstalledBodyKitsDataFormatter());
				}
				if (!MemoryPackFormatterProvider.IsRegistered<InstalledBodyKitsData[]>())
				{
					MemoryPackFormatterProvider.Register(new ArrayFormatter<InstalledBodyKitsData>());
				}
			}

			[Preserve]
			public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref InstalledBodyKitsData? value) where TBufferWriter : class, IBufferWriter<byte>
			{
				if (value == null)
				{
					writer.WriteNullObjectHeader();
				}
				else
				{
					writer.WriteUnmanagedWithObjectHeader(5, in value.TopLight, in value.Roobar, in value.FrontInterior, in value.RearInterior, in value.Skirt);
				}
			}

			[Preserve]
			public static void Deserialize(ref MemoryPackReader reader, ref InstalledBodyKitsData? value)
			{
				if (!reader.TryReadObjectHeader(out var memberCount))
				{
					value = null;
					return;
				}
				int value2;
				int value3;
				int value4;
				int value5;
				int value6;
				if (memberCount == 5)
				{
					if (value != null)
					{
						value2 = value.TopLight;
						value3 = value.Roobar;
						value4 = value.FrontInterior;
						value5 = value.RearInterior;
						value6 = value.Skirt;
						reader.ReadUnmanaged<int>(out value2);
						reader.ReadUnmanaged<int>(out value3);
						reader.ReadUnmanaged<int>(out value4);
						reader.ReadUnmanaged<int>(out value5);
						reader.ReadUnmanaged<int>(out value6);
						goto IL_0117;
					}
					reader.ReadUnmanaged<int, int, int, int, int>(out value2, out value3, out value4, out value5, out value6);
				}
				else
				{
					if (memberCount > 5)
					{
						MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(InstalledBodyKitsData), 5, memberCount);
						return;
					}
					if (value == null)
					{
						value2 = 0;
						value3 = 0;
						value4 = 0;
						value5 = 0;
						value6 = 0;
					}
					else
					{
						value2 = value.TopLight;
						value3 = value.Roobar;
						value4 = value.FrontInterior;
						value5 = value.RearInterior;
						value6 = value.Skirt;
					}
					if (memberCount != 0)
					{
						reader.ReadUnmanaged<int>(out value2);
						if (memberCount != 1)
						{
							reader.ReadUnmanaged<int>(out value3);
							if (memberCount != 2)
							{
								reader.ReadUnmanaged<int>(out value4);
								if (memberCount != 3)
								{
									reader.ReadUnmanaged<int>(out value5);
									if (memberCount != 4)
									{
										reader.ReadUnmanaged<int>(out value6);
										_ = 5;
									}
								}
							}
						}
					}
					if (value != null)
					{
						goto IL_0117;
					}
				}
				value = new InstalledBodyKitsData
				{
					TopLight = value2,
					Roobar = value3,
					FrontInterior = value4,
					RearInterior = value5,
					Skirt = value6
				};
				return;
				IL_0117:
				value.TopLight = value2;
				value.Roobar = value3;
				value.FrontInterior = value4;
				value.RearInterior = value5;
				value.Skirt = value6;
			}
		}

		[Serializable]
		[GenerateTypeScript]
		[MemoryPackable(GenerateType.Object)]
		public class SirenSoundData : IMemoryPackable<SirenSoundData>, IMemoryPackFormatterRegister
		{
			[Preserve]
			private sealed class SirenSoundDataFormatter : MemoryPackFormatter<SirenSoundData>
			{
				[Preserve]
				public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref SirenSoundData value)
				{
					SirenSoundData.Serialize(ref writer, ref value);
				}

				[Preserve]
				public override void Deserialize(ref MemoryPackReader reader, ref SirenSoundData value)
				{
					SirenSoundData.Deserialize(ref reader, ref value);
				}
			}

			public int InstalledIndex;

			static SirenSoundData()
			{
				RegisterFormatter();
			}

			[Preserve]
			public static void RegisterFormatter()
			{
				if (!MemoryPackFormatterProvider.IsRegistered<SirenSoundData>())
				{
					MemoryPackFormatterProvider.Register(new SirenSoundDataFormatter());
				}
				if (!MemoryPackFormatterProvider.IsRegistered<SirenSoundData[]>())
				{
					MemoryPackFormatterProvider.Register(new ArrayFormatter<SirenSoundData>());
				}
			}

			[Preserve]
			public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref SirenSoundData? value) where TBufferWriter : class, IBufferWriter<byte>
			{
				if (value == null)
				{
					writer.WriteNullObjectHeader();
				}
				else
				{
					writer.WriteUnmanagedWithObjectHeader(1, in value.InstalledIndex);
				}
			}

			[Preserve]
			public static void Deserialize(ref MemoryPackReader reader, ref SirenSoundData? value)
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
						value2 = value.InstalledIndex;
						reader.ReadUnmanaged<int>(out value2);
						goto IL_006b;
					}
					reader.ReadUnmanaged<int>(out value2);
				}
				else
				{
					if (memberCount > 1)
					{
						MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(SirenSoundData), 1, memberCount);
						return;
					}
					value2 = ((value != null) ? value.InstalledIndex : 0);
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
				value = new SirenSoundData
				{
					InstalledIndex = value2
				};
				return;
				IL_006b:
				value.InstalledIndex = value2;
			}
		}

		[Preserve]
		private sealed class PoliceDataFormatter : MemoryPackFormatter<PoliceData>
		{
			[Preserve]
			public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref PoliceData value)
			{
				PoliceData.Serialize(ref writer, ref value);
			}

			[Preserve]
			public override void Deserialize(ref MemoryPackReader reader, ref PoliceData value)
			{
				PoliceData.Deserialize(ref reader, ref value);
			}
		}

		public bool Bought;

		public bool Installed;

		public InstalledBodyKitsData InstalledBodyKits = new InstalledBodyKitsData();

		public SirenSoundData SirenSound = new SirenSoundData();

		public void CheckForNulls()
		{
			if (InstalledBodyKits == null)
			{
				InstalledBodyKits = new InstalledBodyKitsData();
			}
			if (SirenSound == null)
			{
				SirenSound = new SirenSoundData();
			}
		}

		static PoliceData()
		{
			RegisterFormatter();
		}

		[Preserve]
		public static void RegisterFormatter()
		{
			if (!MemoryPackFormatterProvider.IsRegistered<PoliceData>())
			{
				MemoryPackFormatterProvider.Register(new PoliceDataFormatter());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<PoliceData[]>())
			{
				MemoryPackFormatterProvider.Register(new ArrayFormatter<PoliceData>());
			}
		}

		[Preserve]
		public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref PoliceData? value) where TBufferWriter : class, IBufferWriter<byte>
		{
			if (value == null)
			{
				writer.WriteNullObjectHeader();
				return;
			}
			writer.WriteUnmanagedWithObjectHeader(4, in value.Bought, in value.Installed);
			writer.WritePackable(in value.InstalledBodyKits);
			writer.WritePackable(in value.SirenSound);
		}

		[Preserve]
		public static void Deserialize(ref MemoryPackReader reader, ref PoliceData? value)
		{
			if (!reader.TryReadObjectHeader(out var memberCount))
			{
				value = null;
				return;
			}
			bool value2;
			bool value3;
			InstalledBodyKitsData value4;
			SirenSoundData value5;
			if (memberCount == 4)
			{
				if (value != null)
				{
					value2 = value.Bought;
					value3 = value.Installed;
					value4 = value.InstalledBodyKits;
					value5 = value.SirenSound;
					reader.ReadUnmanaged<bool>(out value2);
					reader.ReadUnmanaged<bool>(out value3);
					reader.ReadPackable(ref value4);
					reader.ReadPackable(ref value5);
					goto IL_00f4;
				}
				reader.ReadUnmanaged<bool, bool>(out value2, out value3);
				value4 = reader.ReadPackable<InstalledBodyKitsData>();
				value5 = reader.ReadPackable<SirenSoundData>();
			}
			else
			{
				if (memberCount > 4)
				{
					MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(PoliceData), 4, memberCount);
					return;
				}
				if (value == null)
				{
					value2 = false;
					value3 = false;
					value4 = null;
					value5 = null;
				}
				else
				{
					value2 = value.Bought;
					value3 = value.Installed;
					value4 = value.InstalledBodyKits;
					value5 = value.SirenSound;
				}
				if (memberCount != 0)
				{
					reader.ReadUnmanaged<bool>(out value2);
					if (memberCount != 1)
					{
						reader.ReadUnmanaged<bool>(out value3);
						if (memberCount != 2)
						{
							reader.ReadPackable(ref value4);
							if (memberCount != 3)
							{
								reader.ReadPackable(ref value5);
								_ = 4;
							}
						}
					}
				}
				if (value != null)
				{
					goto IL_00f4;
				}
			}
			value = new PoliceData
			{
				Bought = value2,
				Installed = value3,
				InstalledBodyKits = value4,
				SirenSound = value5
			};
			return;
			IL_00f4:
			value.Bought = value2;
			value.Installed = value3;
			value.InstalledBodyKits = value4;
			value.SirenSound = value5;
		}
	}
}

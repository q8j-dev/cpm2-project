#nullable enable annotations
using System;
using System.Buffers;
using System.Collections.Generic;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;

namespace CPM.Database
{
	[Serializable]
	[MemoryPackable(GenerateType.Object)]
	[GenerateTypeScript]
	public class EquipmentsData : IMemoryPackable<EquipmentsData>, IMemoryPackFormatterRegister
	{
		[Preserve]
		private sealed class EquipmentsDataFormatter : MemoryPackFormatter<EquipmentsData>
		{
			[Preserve]
			public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref EquipmentsData value)
			{
				EquipmentsData.Serialize(ref writer, ref value);
			}

			[Preserve]
			public override void Deserialize(ref MemoryPackReader reader, ref EquipmentsData value)
			{
				EquipmentsData.Deserialize(ref reader, ref value);
			}
		}

		public const int CountOfEquipment = 11;

		public List<int> hair = new List<int>();

		public List<int> face = new List<int>();

		public List<int> beard = new List<int>();

		public List<int> cap = new List<int>();

		public List<int> mask = new List<int>();

		public List<int> top = new List<int>();

		public List<int> gloves = new List<int>();

		public List<int> bag = new List<int>();

		public List<int> pants = new List<int>();

		public List<int> shoes = new List<int>();

		public List<int> glasses = new List<int>();

		public List<int> SelectedEquipments = new List<int>();

		public EquipmentsData Clone()
		{
			return new EquipmentsData
			{
				hair = new List<int>(hair),
				face = new List<int>(face),
				beard = new List<int>(beard),
				cap = new List<int>(cap),
				mask = new List<int>(mask),
				top = new List<int>(top),
				gloves = new List<int>(gloves),
				bag = new List<int>(bag),
				pants = new List<int>(pants),
				shoes = new List<int>(shoes),
				glasses = new List<int>(glasses),
				SelectedEquipments = new List<int>(SelectedEquipments)
			};
		}

		static EquipmentsData()
		{
			RegisterFormatter();
		}

		[Preserve]
		public static void RegisterFormatter()
		{
			if (!MemoryPackFormatterProvider.IsRegistered<EquipmentsData>())
			{
				MemoryPackFormatterProvider.Register(new EquipmentsDataFormatter());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<EquipmentsData[]>())
			{
				MemoryPackFormatterProvider.Register(new ArrayFormatter<EquipmentsData>());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<List<int>>())
			{
				MemoryPackFormatterProvider.Register(new ListFormatter<int>());
			}
		}

		[Preserve]
		public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref EquipmentsData? value) where TBufferWriter : class, IBufferWriter<byte>
		{
			if (value == null)
			{
				writer.WriteNullObjectHeader();
				return;
			}
			writer.WriteObjectHeader(12);
			writer.WriteValue(in value.hair);
			writer.WriteValue(in value.face);
			writer.WriteValue(in value.beard);
			writer.WriteValue(in value.cap);
			writer.WriteValue(in value.mask);
			writer.WriteValue(in value.top);
			writer.WriteValue(in value.gloves);
			writer.WriteValue(in value.bag);
			writer.WriteValue(in value.pants);
			writer.WriteValue(in value.shoes);
			writer.WriteValue(in value.glasses);
			writer.WriteValue(in value.SelectedEquipments);
		}

		[Preserve]
		public static void Deserialize(ref MemoryPackReader reader, ref EquipmentsData? value)
		{
			if (!reader.TryReadObjectHeader(out var memberCount))
			{
				value = null;
				return;
			}
			List<int> value2;
			List<int> value3;
			List<int> value4;
			List<int> value5;
			List<int> value6;
			List<int> value7;
			List<int> value8;
			List<int> value9;
			List<int> value10;
			List<int> value11;
			List<int> value12;
			List<int> value13;
			if (memberCount == 12)
			{
				if (value != null)
				{
					value2 = value.hair;
					value3 = value.face;
					value4 = value.beard;
					value5 = value.cap;
					value6 = value.mask;
					value7 = value.top;
					value8 = value.gloves;
					value9 = value.bag;
					value10 = value.pants;
					value11 = value.shoes;
					value12 = value.glasses;
					value13 = value.SelectedEquipments;
					reader.ReadValue(ref value2);
					reader.ReadValue(ref value3);
					reader.ReadValue(ref value4);
					reader.ReadValue(ref value5);
					reader.ReadValue(ref value6);
					reader.ReadValue(ref value7);
					reader.ReadValue(ref value8);
					reader.ReadValue(ref value9);
					reader.ReadValue(ref value10);
					reader.ReadValue(ref value11);
					reader.ReadValue(ref value12);
					reader.ReadValue(ref value13);
					goto IL_0293;
				}
				value2 = reader.ReadValue<List<int>>();
				value3 = reader.ReadValue<List<int>>();
				value4 = reader.ReadValue<List<int>>();
				value5 = reader.ReadValue<List<int>>();
				value6 = reader.ReadValue<List<int>>();
				value7 = reader.ReadValue<List<int>>();
				value8 = reader.ReadValue<List<int>>();
				value9 = reader.ReadValue<List<int>>();
				value10 = reader.ReadValue<List<int>>();
				value11 = reader.ReadValue<List<int>>();
				value12 = reader.ReadValue<List<int>>();
				value13 = reader.ReadValue<List<int>>();
			}
			else
			{
				if (memberCount > 12)
				{
					MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(EquipmentsData), 12, memberCount);
					return;
				}
				if (value == null)
				{
					value2 = null;
					value3 = null;
					value4 = null;
					value5 = null;
					value6 = null;
					value7 = null;
					value8 = null;
					value9 = null;
					value10 = null;
					value11 = null;
					value12 = null;
					value13 = null;
				}
				else
				{
					value2 = value.hair;
					value3 = value.face;
					value4 = value.beard;
					value5 = value.cap;
					value6 = value.mask;
					value7 = value.top;
					value8 = value.gloves;
					value9 = value.bag;
					value10 = value.pants;
					value11 = value.shoes;
					value12 = value.glasses;
					value13 = value.SelectedEquipments;
				}
				if (memberCount != 0)
				{
					reader.ReadValue(ref value2);
					if (memberCount != 1)
					{
						reader.ReadValue(ref value3);
						if (memberCount != 2)
						{
							reader.ReadValue(ref value4);
							if (memberCount != 3)
							{
								reader.ReadValue(ref value5);
								if (memberCount != 4)
								{
									reader.ReadValue(ref value6);
									if (memberCount != 5)
									{
										reader.ReadValue(ref value7);
										if (memberCount != 6)
										{
											reader.ReadValue(ref value8);
											if (memberCount != 7)
											{
												reader.ReadValue(ref value9);
												if (memberCount != 8)
												{
													reader.ReadValue(ref value10);
													if (memberCount != 9)
													{
														reader.ReadValue(ref value11);
														if (memberCount != 10)
														{
															reader.ReadValue(ref value12);
															if (memberCount != 11)
															{
																reader.ReadValue(ref value13);
																_ = 12;
															}
														}
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
				if (value != null)
				{
					goto IL_0293;
				}
			}
			value = new EquipmentsData
			{
				hair = value2,
				face = value3,
				beard = value4,
				cap = value5,
				mask = value6,
				top = value7,
				gloves = value8,
				bag = value9,
				pants = value10,
				shoes = value11,
				glasses = value12,
				SelectedEquipments = value13
			};
			return;
			IL_0293:
			value.hair = value2;
			value.face = value3;
			value.beard = value4;
			value.cap = value5;
			value.mask = value6;
			value.top = value7;
			value.gloves = value8;
			value.bag = value9;
			value.pants = value10;
			value.shoes = value11;
			value.glasses = value12;
			value.SelectedEquipments = value13;
		}
	}
}

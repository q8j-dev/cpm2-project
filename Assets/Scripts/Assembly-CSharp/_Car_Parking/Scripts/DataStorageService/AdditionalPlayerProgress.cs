using System;
using System.Buffers;
using System.Collections.Generic;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;

namespace _Car_Parking.Scripts.DataStorageService
{
	[Serializable]
	[MemoryPackable(GenerateType.Object)]
	[GenerateTypeScript]
	public class AdditionalPlayerProgress : IMemoryPackable<AdditionalPlayerProgress>, IMemoryPackFormatterRegister
	{
		[Preserve]
		private sealed class AdditionalPlayerProgressFormatter : MemoryPackFormatter<AdditionalPlayerProgress>
		{
			[Preserve]
			public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref AdditionalPlayerProgress value)
			{
				AdditionalPlayerProgress.Serialize(ref writer, ref value);
			}

			[Preserve]
			public override void Deserialize(ref MemoryPackReader reader, ref AdditionalPlayerProgress value)
			{
				AdditionalPlayerProgress.Deserialize(ref reader, ref value);
			}
		}

		public List<float> LevelsDoneTime;

		public Dictionary<int, int> Flags;

		public Dictionary<int, string> PoliceBodyKits;

		public List<int> PoliceSounds;

		public float[] Main;

		public int[] Purchased;

		public float[] Camera;

		public long Animations;

		public long Homes;

		public List<long> Eggs;

		public List<int> SelectedAnimations;

		public int Slots;

		[MemoryPackConstructor]
		public AdditionalPlayerProgress()
		{
		}

		public AdditionalPlayerProgress(List<float> levelsDoneTime, long animations, List<int> selectedAnimations, Dictionary<int, int> flags, long homes, List<long> eggs, int slots, float[] main, int[] purchased, float[] camera, Dictionary<int, string> policeBodyKits, List<int> policeSounds)
		{
			Slots = slots;
			LevelsDoneTime = levelsDoneTime;
			Animations = animations;
			SelectedAnimations = selectedAnimations;
			Flags = flags;
			Homes = homes;
			Eggs = eggs;
			Main = main;
			Purchased = purchased;
			Camera = camera;
			PoliceBodyKits = policeBodyKits;
			PoliceSounds = policeSounds;
		}

		public void Clear()
		{
			LevelsDoneTime?.Clear();
			Flags?.Clear();
			PoliceBodyKits?.Clear();
			PoliceSounds.Clear();
			Main = Array.Empty<float>();
			Purchased = Array.Empty<int>();
			Camera = Array.Empty<float>();
			Animations = 0L;
			Homes = 0L;
			Eggs.Clear();
			SelectedAnimations?.Clear();
			Slots = 0;
		}

		static AdditionalPlayerProgress()
		{
			RegisterFormatter();
		}

		[Preserve]
		public static void RegisterFormatter()
		{
			if (!MemoryPackFormatterProvider.IsRegistered<AdditionalPlayerProgress>())
			{
				MemoryPackFormatterProvider.Register(new AdditionalPlayerProgressFormatter());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<AdditionalPlayerProgress[]>())
			{
				MemoryPackFormatterProvider.Register(new ArrayFormatter<AdditionalPlayerProgress>());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<List<float>>())
			{
				MemoryPackFormatterProvider.Register(new ListFormatter<float>());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<Dictionary<int, int>>())
			{
				MemoryPackFormatterProvider.Register(new DictionaryFormatter<int, int>());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<Dictionary<int, string>>())
			{
				MemoryPackFormatterProvider.Register(new DictionaryFormatter<int, string>());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<List<int>>())
			{
				MemoryPackFormatterProvider.Register(new ListFormatter<int>());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<float[]>())
			{
				MemoryPackFormatterProvider.Register(new ArrayFormatter<float>());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<int[]>())
			{
				MemoryPackFormatterProvider.Register(new ArrayFormatter<int>());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<List<long>>())
			{
				MemoryPackFormatterProvider.Register(new ListFormatter<long>());
			}
		}

		[Preserve]
		public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref AdditionalPlayerProgress? value) where TBufferWriter : class, IBufferWriter<byte>
		{
			if (value == null)
			{
				writer.WriteNullObjectHeader();
				return;
			}
			writer.WriteObjectHeader(12);
			writer.WriteValue(in value.LevelsDoneTime);
			writer.WriteValue(in value.Flags);
			writer.WriteValue(in value.PoliceBodyKits);
			writer.WriteValue(in value.PoliceSounds);
			writer.WriteUnmanagedArray(value.Main);
			writer.WriteUnmanagedArray(value.Purchased);
			writer.WriteUnmanagedArray(value.Camera);
			writer.WriteUnmanaged(in value.Animations, in value.Homes);
			writer.WriteValue(in value.Eggs);
			writer.WriteValue(in value.SelectedAnimations);
			writer.WriteUnmanaged(in value.Slots);
		}

		[Preserve]
		public static void Deserialize(ref MemoryPackReader reader, ref AdditionalPlayerProgress? value)
		{
			if (!reader.TryReadObjectHeader(out var memberCount))
			{
				value = null;
				return;
			}
			List<float> value2;
			Dictionary<int, int> value3;
			Dictionary<int, string> value4;
			List<int> value5;
			float[] value6;
			int[] value7;
			float[] value8;
			long value9;
			long value10;
			List<long> value11;
			List<int> value12;
			int value13;
			if (memberCount == 12)
			{
				if (value != null)
				{
					value2 = value.LevelsDoneTime;
					value3 = value.Flags;
					value4 = value.PoliceBodyKits;
					value5 = value.PoliceSounds;
					value6 = value.Main;
					value7 = value.Purchased;
					value8 = value.Camera;
					value9 = value.Animations;
					value10 = value.Homes;
					value11 = value.Eggs;
					value12 = value.SelectedAnimations;
					value13 = value.Slots;
					reader.ReadValue(ref value2);
					reader.ReadValue(ref value3);
					reader.ReadValue(ref value4);
					reader.ReadValue(ref value5);
					reader.ReadUnmanagedArray(ref value6);
					reader.ReadUnmanagedArray(ref value7);
					reader.ReadUnmanagedArray(ref value8);
					reader.ReadUnmanaged<long>(out value9);
					reader.ReadUnmanaged<long>(out value10);
					reader.ReadValue(ref value11);
					reader.ReadValue(ref value12);
					reader.ReadUnmanaged<int>(out value13);
					goto IL_028f;
				}
				value2 = reader.ReadValue<List<float>>();
				value3 = reader.ReadValue<Dictionary<int, int>>();
				value4 = reader.ReadValue<Dictionary<int, string>>();
				value5 = reader.ReadValue<List<int>>();
				value6 = reader.ReadUnmanagedArray<float>();
				value7 = reader.ReadUnmanagedArray<int>();
				value8 = reader.ReadUnmanagedArray<float>();
				reader.ReadUnmanaged<long, long>(out value9, out value10);
				value11 = reader.ReadValue<List<long>>();
				value12 = reader.ReadValue<List<int>>();
				reader.ReadUnmanaged<int>(out value13);
			}
			else
			{
				if (memberCount > 12)
				{
					MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(AdditionalPlayerProgress), 12, memberCount);
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
					value9 = 0L;
					value10 = 0L;
					value11 = null;
					value12 = null;
					value13 = 0;
				}
				else
				{
					value2 = value.LevelsDoneTime;
					value3 = value.Flags;
					value4 = value.PoliceBodyKits;
					value5 = value.PoliceSounds;
					value6 = value.Main;
					value7 = value.Purchased;
					value8 = value.Camera;
					value9 = value.Animations;
					value10 = value.Homes;
					value11 = value.Eggs;
					value12 = value.SelectedAnimations;
					value13 = value.Slots;
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
									reader.ReadUnmanagedArray(ref value6);
									if (memberCount != 5)
									{
										reader.ReadUnmanagedArray(ref value7);
										if (memberCount != 6)
										{
											reader.ReadUnmanagedArray(ref value8);
											if (memberCount != 7)
											{
												reader.ReadUnmanaged<long>(out value9);
												if (memberCount != 8)
												{
													reader.ReadUnmanaged<long>(out value10);
													if (memberCount != 9)
													{
														reader.ReadValue(ref value11);
														if (memberCount != 10)
														{
															reader.ReadValue(ref value12);
															if (memberCount != 11)
															{
																reader.ReadUnmanaged<int>(out value13);
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
					goto IL_028f;
				}
			}
			value = new AdditionalPlayerProgress
			{
				LevelsDoneTime = value2,
				Flags = value3,
				PoliceBodyKits = value4,
				PoliceSounds = value5,
				Main = value6,
				Purchased = value7,
				Camera = value8,
				Animations = value9,
				Homes = value10,
				Eggs = value11,
				SelectedAnimations = value12,
				Slots = value13
			};
			return;
			IL_028f:
			value.LevelsDoneTime = value2;
			value.Flags = value3;
			value.PoliceBodyKits = value4;
			value.PoliceSounds = value5;
			value.Main = value6;
			value.Purchased = value7;
			value.Camera = value8;
			value.Animations = value9;
			value.Homes = value10;
			value.Eggs = value11;
			value.SelectedAnimations = value12;
			value.Slots = value13;
		}
	}
}

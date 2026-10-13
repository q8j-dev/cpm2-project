using System;
using System.Buffers;
using System.Collections.Generic;
using CPM.Database;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using UnityEngine.Scripting;
using _Car_Parking.Scripts.DataStorageService.PlayerData;
using _Car_Parking.Scripts.VisualTuningFavourite;

namespace _Car_Parking.Scripts.DataStorageService
{
	[Serializable]
	[UnityEngine.Scripting.Preserve]
	[GenerateTypeScript]
	[MemoryPackable(GenerateType.Object)]
	public class PlayerStorage : IMemoryPackable<PlayerStorage>, IMemoryPackFormatterRegister
	{
		[MemoryPack.Internal.Preserve]
		private sealed class PlayerStorageFormatter : MemoryPackFormatter<PlayerStorage>
		{
			[MemoryPack.Internal.Preserve]
			public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref PlayerStorage value)
			{
				PlayerStorage.Serialize(ref writer, ref value);
			}

			[MemoryPack.Internal.Preserve]
			public override void Deserialize(ref MemoryPackReader reader, ref PlayerStorage value)
			{
				PlayerStorage.Deserialize(ref reader, ref value);
			}
		}

		public string Name = "NewPlayer";

		public string LocalID = "";

		public int MatTypes = 3;

		public long Brakes;

		public long Calipers;

		public List<int> Wheels = new List<int>();

		public int Gender;

		public EquipmentsData EquipmentsMale = new EquipmentsData();

		public EquipmentsData EquipmentsFemale = new EquipmentsData();

		public RacingProgress RacingProgress;

		public VisualTuningFavouriteData VisualTuningFavouriteData;

		public CircuitRacingProgress CircuitRacingProgress;

		public AdditionalPlayerProgress Other;

		public void ResetNameAndLocalId()
		{
			Name = "";
			LocalID = "";
		}

		public void SetNameAndLocalId(string name, string localId, string steamId = "")
		{
			Name = name;
			LocalID = localId;
		}

		public void SetSteamId(string steamId)
		{
		}

		public void RSetSteamId()
		{
		}

		public void Clear()
		{
			Name = "NewPlayer";
			LocalID = "";
			MatTypes = 3;
			Brakes = 0L;
			Calipers = 0L;
			Wheels = new List<int>();
			Gender = 0;
			EquipmentsMale = new EquipmentsData();
			EquipmentsFemale = new EquipmentsData();
			RacingProgress?.Clear();
			VisualTuningFavouriteData?.Clear();
			CircuitRacingProgress?.Clear();
			Other?.Clear();
		}

		static PlayerStorage()
		{
			RegisterFormatter();
		}

		[MemoryPack.Internal.Preserve]
		public static void RegisterFormatter()
		{
			if (!MemoryPackFormatterProvider.IsRegistered<PlayerStorage>())
			{
				MemoryPackFormatterProvider.Register(new PlayerStorageFormatter());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<PlayerStorage[]>())
			{
				MemoryPackFormatterProvider.Register(new ArrayFormatter<PlayerStorage>());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<List<int>>())
			{
				MemoryPackFormatterProvider.Register(new ListFormatter<int>());
			}
		}

		[MemoryPack.Internal.Preserve]
		public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref PlayerStorage? value) where TBufferWriter : class, IBufferWriter<byte>
		{
			if (value == null)
			{
				writer.WriteNullObjectHeader();
				return;
			}
			writer.WriteObjectHeader(13);
			writer.WriteString(value.Name);
			writer.WriteString(value.LocalID);
			writer.WriteUnmanaged(in value.MatTypes, in value.Brakes, in value.Calipers);
			writer.WriteValue(in value.Wheels);
			writer.WriteUnmanaged(in value.Gender);
			writer.WritePackable(in value.EquipmentsMale);
			writer.WritePackable(in value.EquipmentsFemale);
			writer.WritePackable(in value.RacingProgress);
			writer.WritePackable(in value.VisualTuningFavouriteData);
			writer.WritePackable(in value.CircuitRacingProgress);
			writer.WritePackable(in value.Other);
		}

		[MemoryPack.Internal.Preserve]
		public static void Deserialize(ref MemoryPackReader reader, ref PlayerStorage? value)
		{
			if (!reader.TryReadObjectHeader(out var memberCount))
			{
				value = null;
				return;
			}
			int value2;
			long value3;
			long value4;
			List<int> value5;
			int value6;
			EquipmentsData value7;
			EquipmentsData value8;
			RacingProgress value9;
			VisualTuningFavouriteData value10;
			CircuitRacingProgress value11;
			AdditionalPlayerProgress value12;
			string name;
			string localID;
			if (memberCount == 13)
			{
				if (value != null)
				{
					name = value.Name;
					localID = value.LocalID;
					value2 = value.MatTypes;
					value3 = value.Brakes;
					value4 = value.Calipers;
					value5 = value.Wheels;
					value6 = value.Gender;
					value7 = value.EquipmentsMale;
					value8 = value.EquipmentsFemale;
					value9 = value.RacingProgress;
					value10 = value.VisualTuningFavouriteData;
					value11 = value.CircuitRacingProgress;
					value12 = value.Other;
					name = reader.ReadString();
					localID = reader.ReadString();
					reader.ReadUnmanaged<int>(out value2);
					reader.ReadUnmanaged<long>(out value3);
					reader.ReadUnmanaged<long>(out value4);
					reader.ReadValue(ref value5);
					reader.ReadUnmanaged<int>(out value6);
					reader.ReadPackable(ref value7);
					reader.ReadPackable(ref value8);
					reader.ReadPackable(ref value9);
					reader.ReadPackable(ref value10);
					reader.ReadPackable(ref value11);
					reader.ReadPackable(ref value12);
					goto IL_02bb;
				}
				name = reader.ReadString();
				localID = reader.ReadString();
				reader.ReadUnmanaged<int, long, long>(out value2, out value3, out value4);
				value5 = reader.ReadValue<List<int>>();
				reader.ReadUnmanaged<int>(out value6);
				value7 = reader.ReadPackable<EquipmentsData>();
				value8 = reader.ReadPackable<EquipmentsData>();
				value9 = reader.ReadPackable<RacingProgress>();
				value10 = reader.ReadPackable<VisualTuningFavouriteData>();
				value11 = reader.ReadPackable<CircuitRacingProgress>();
				value12 = reader.ReadPackable<AdditionalPlayerProgress>();
			}
			else
			{
				if (memberCount > 13)
				{
					MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(PlayerStorage), 13, memberCount);
					return;
				}
				if (value == null)
				{
					name = null;
					localID = null;
					value2 = 0;
					value3 = 0L;
					value4 = 0L;
					value5 = null;
					value6 = 0;
					value7 = null;
					value8 = null;
					value9 = null;
					value10 = null;
					value11 = null;
					value12 = null;
				}
				else
				{
					name = value.Name;
					localID = value.LocalID;
					value2 = value.MatTypes;
					value3 = value.Brakes;
					value4 = value.Calipers;
					value5 = value.Wheels;
					value6 = value.Gender;
					value7 = value.EquipmentsMale;
					value8 = value.EquipmentsFemale;
					value9 = value.RacingProgress;
					value10 = value.VisualTuningFavouriteData;
					value11 = value.CircuitRacingProgress;
					value12 = value.Other;
				}
				if (memberCount != 0)
				{
					name = reader.ReadString();
					if (memberCount != 1)
					{
						localID = reader.ReadString();
						if (memberCount != 2)
						{
							reader.ReadUnmanaged<int>(out value2);
							if (memberCount != 3)
							{
								reader.ReadUnmanaged<long>(out value3);
								if (memberCount != 4)
								{
									reader.ReadUnmanaged<long>(out value4);
									if (memberCount != 5)
									{
										reader.ReadValue(ref value5);
										if (memberCount != 6)
										{
											reader.ReadUnmanaged<int>(out value6);
											if (memberCount != 7)
											{
												reader.ReadPackable(ref value7);
												if (memberCount != 8)
												{
													reader.ReadPackable(ref value8);
													if (memberCount != 9)
													{
														reader.ReadPackable(ref value9);
														if (memberCount != 10)
														{
															reader.ReadPackable(ref value10);
															if (memberCount != 11)
															{
																reader.ReadPackable(ref value11);
																if (memberCount != 12)
																{
																	reader.ReadPackable(ref value12);
																	_ = 13;
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
				}
				if (value != null)
				{
					goto IL_02bb;
				}
			}
			value = new PlayerStorage
			{
				Name = name,
				LocalID = localID,
				MatTypes = value2,
				Brakes = value3,
				Calipers = value4,
				Wheels = value5,
				Gender = value6,
				EquipmentsMale = value7,
				EquipmentsFemale = value8,
				RacingProgress = value9,
				VisualTuningFavouriteData = value10,
				CircuitRacingProgress = value11,
				Other = value12
			};
			return;
			IL_02bb:
			value.Name = name;
			value.LocalID = localID;
			value.MatTypes = value2;
			value.Brakes = value3;
			value.Calipers = value4;
			value.Wheels = value5;
			value.Gender = value6;
			value.EquipmentsMale = value7;
			value.EquipmentsFemale = value8;
			value.RacingProgress = value9;
			value.VisualTuningFavouriteData = value10;
			value.CircuitRacingProgress = value11;
			value.Other = value12;
		}
	}
}

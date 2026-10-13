#nullable enable annotations
using System;
using System.Buffers;
using System.Collections.Generic;
using CPM.Database;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using UnityEngine.Scripting;
using _Car_Parking.Scripts.Characters;
using _Car_Parking.Scripts.DataStorageService.PlayerData;
using _Car_Parking.Scripts.Inventory;

namespace _Car_Parking.Scripts.DataStorageService
{
	[Serializable]
	[MemoryPackable(GenerateType.Object)]
	[UnityEngine.Scripting.Preserve]
	[GenerateTypeScript]
	public class PlayerStorageFromDataBase : IMemoryPackable<PlayerStorageFromDataBase>, IMemoryPackFormatterRegister
	{
		[MemoryPack.Internal.Preserve]
		private sealed class PlayerStorageFromDataBaseFormatter : MemoryPackFormatter<PlayerStorageFromDataBase>
		{
			[MemoryPack.Internal.Preserve]
			public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref PlayerStorageFromDataBase value)
			{
				PlayerStorageFromDataBase.Serialize(ref writer, ref value);
			}

			[MemoryPack.Internal.Preserve]
			public override void Deserialize(ref MemoryPackReader reader, ref PlayerStorageFromDataBase value)
			{
				PlayerStorageFromDataBase.Deserialize(ref reader, ref value);
			}
		}

		private static readonly KitType[] MaleRequiredSlots;

		private static readonly KitType[] FemaleRequiredSlots;

		public PlayerStorage PlayerStorage = new PlayerStorage();

		public WalletData WalletData = new WalletData();

		public InventoryData InventoryData = new InventoryData();

		public EngineKitsInventoryData EngineKits = new EngineKitsInventoryData();

		public PlatesDataForDatabase PlatesData = new PlatesDataForDatabase();

		public FriendsCollection FriendsCollection = new FriendsCollection();

		public SlotsOnDatabaseCollection SlotsOnDatabaseCollection = new SlotsOnDatabaseCollection();

		public List<int> UserGarages = new List<int>();

		public static PlayerStorageFromDataBase CreateDefault()
		{
			return new PlayerStorageFromDataBase
			{
				WalletData = new WalletData
				{
					Money = CurrencyManagerController.Money
				}
			};
		}

		public void CheckNulls()
		{
			if (PlayerStorage == null)
			{
				PlayerStorage = new PlayerStorage();
			}
			CheckNullsPlayerStorageData();
			if (WalletData == null)
			{
				WalletData = new WalletData();
			}
			if (InventoryData == null)
			{
				InventoryData = new InventoryData();
			}
			if (EngineKits == null)
			{
				EngineKits = new EngineKitsInventoryData();
			}
			if (PlatesData == null)
			{
				PlatesData = new PlatesDataForDatabase();
			}
			if (FriendsCollection == null)
			{
				FriendsCollection = new FriendsCollection();
			}
			if (SlotsOnDatabaseCollection == null)
			{
				SlotsOnDatabaseCollection = new SlotsOnDatabaseCollection();
			}
			if (UserGarages == null)
			{
				UserGarages = new List<int>();
			}
		}

		private void CheckNullsPlayerStorageData()
		{
			if (PlayerStorage.EquipmentsMale == null)
			{
				PlayerStorage.EquipmentsMale = new EquipmentsData();
			}
			if (PlayerStorage.EquipmentsFemale == null)
			{
				PlayerStorage.EquipmentsFemale = new EquipmentsData();
			}
			CheckNullsEquipmentsData(PlayerStorage.EquipmentsMale, MaleRequiredSlots);
			CheckNullsEquipmentsData(PlayerStorage.EquipmentsFemale, FemaleRequiredSlots);
		}

		private void CheckNullsEquipmentsData(EquipmentsData equipmentsData, KitType[] requiredSlots)
		{
			if (equipmentsData.SelectedEquipments == null)
			{
				equipmentsData.SelectedEquipments = new List<int>();
			}
			while (equipmentsData.SelectedEquipments.Count < 11)
			{
				equipmentsData.SelectedEquipments.Add(-1);
			}
			foreach (KitType index in requiredSlots)
			{
				if (equipmentsData.SelectedEquipments[(int)index] < 0)
				{
					equipmentsData.SelectedEquipments[(int)index] = 0;
				}
			}
		}

		static PlayerStorageFromDataBase()
		{
			MaleRequiredSlots = new KitType[1] { KitType.Face };
			FemaleRequiredSlots = new KitType[2]
			{
				KitType.Face,
				KitType.Hair
			};
			RegisterFormatter();
		}

		[MemoryPack.Internal.Preserve]
		public static void RegisterFormatter()
		{
			if (!MemoryPackFormatterProvider.IsRegistered<PlayerStorageFromDataBase>())
			{
				MemoryPackFormatterProvider.Register(new PlayerStorageFromDataBaseFormatter());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<PlayerStorageFromDataBase[]>())
			{
				MemoryPackFormatterProvider.Register(new ArrayFormatter<PlayerStorageFromDataBase>());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<List<int>>())
			{
				MemoryPackFormatterProvider.Register(new ListFormatter<int>());
			}
		}

		[MemoryPack.Internal.Preserve]
		public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref PlayerStorageFromDataBase? value) where TBufferWriter : class, IBufferWriter<byte>
		{
			if (value == null)
			{
				writer.WriteNullObjectHeader();
				return;
			}
			writer.WriteObjectHeader(8);
			writer.WritePackable(in value.PlayerStorage);
			writer.WritePackable(in value.WalletData);
			writer.WritePackable(in value.InventoryData);
			writer.WritePackable(in value.EngineKits);
			writer.WritePackable(in value.PlatesData);
			writer.WritePackable(in value.FriendsCollection);
			writer.WritePackable(in value.SlotsOnDatabaseCollection);
			writer.WriteValue(in value.UserGarages);
		}

		[MemoryPack.Internal.Preserve]
		public static void Deserialize(ref MemoryPackReader reader, ref PlayerStorageFromDataBase? value)
		{
			if (!reader.TryReadObjectHeader(out var memberCount))
			{
				value = null;
				return;
			}
			PlayerStorage value2;
			WalletData value3;
			InventoryData value4;
			EngineKitsInventoryData value5;
			PlatesDataForDatabase value6;
			FriendsCollection value7;
			SlotsOnDatabaseCollection value8;
			List<int> value9;
			if (memberCount == 8)
			{
				if (value != null)
				{
					value2 = value.PlayerStorage;
					value3 = value.WalletData;
					value4 = value.InventoryData;
					value5 = value.EngineKits;
					value6 = value.PlatesData;
					value7 = value.FriendsCollection;
					value8 = value.SlotsOnDatabaseCollection;
					value9 = value.UserGarages;
					reader.ReadPackable(ref value2);
					reader.ReadPackable(ref value3);
					reader.ReadPackable(ref value4);
					reader.ReadPackable(ref value5);
					reader.ReadPackable(ref value6);
					reader.ReadPackable(ref value7);
					reader.ReadPackable(ref value8);
					reader.ReadValue(ref value9);
					goto IL_01c2;
				}
				value2 = reader.ReadPackable<PlayerStorage>();
				value3 = reader.ReadPackable<WalletData>();
				value4 = reader.ReadPackable<InventoryData>();
				value5 = reader.ReadPackable<EngineKitsInventoryData>();
				value6 = reader.ReadPackable<PlatesDataForDatabase>();
				value7 = reader.ReadPackable<FriendsCollection>();
				value8 = reader.ReadPackable<SlotsOnDatabaseCollection>();
				value9 = reader.ReadValue<List<int>>();
			}
			else
			{
				if (memberCount > 8)
				{
					MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(PlayerStorageFromDataBase), 8, memberCount);
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
				}
				else
				{
					value2 = value.PlayerStorage;
					value3 = value.WalletData;
					value4 = value.InventoryData;
					value5 = value.EngineKits;
					value6 = value.PlatesData;
					value7 = value.FriendsCollection;
					value8 = value.SlotsOnDatabaseCollection;
					value9 = value.UserGarages;
				}
				if (memberCount != 0)
				{
					reader.ReadPackable(ref value2);
					if (memberCount != 1)
					{
						reader.ReadPackable(ref value3);
						if (memberCount != 2)
						{
							reader.ReadPackable(ref value4);
							if (memberCount != 3)
							{
								reader.ReadPackable(ref value5);
								if (memberCount != 4)
								{
									reader.ReadPackable(ref value6);
									if (memberCount != 5)
									{
										reader.ReadPackable(ref value7);
										if (memberCount != 6)
										{
											reader.ReadPackable(ref value8);
											if (memberCount != 7)
											{
												reader.ReadValue(ref value9);
												_ = 8;
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
					goto IL_01c2;
				}
			}
			value = new PlayerStorageFromDataBase
			{
				PlayerStorage = value2,
				WalletData = value3,
				InventoryData = value4,
				EngineKits = value5,
				PlatesData = value6,
				FriendsCollection = value7,
				SlotsOnDatabaseCollection = value8,
				UserGarages = value9
			};
			return;
			IL_01c2:
			value.PlayerStorage = value2;
			value.WalletData = value3;
			value.InventoryData = value4;
			value.EngineKits = value5;
			value.PlatesData = value6;
			value.FriendsCollection = value7;
			value.SlotsOnDatabaseCollection = value8;
			value.UserGarages = value9;
		}
	}
}

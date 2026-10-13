using System;
using System.Collections.Generic;
using CPM.Account;
using CPM.Database;
using CPM.PlayerData;
using CPMEngine;
using CodeStage.AntiCheat.ObscuredTypes;
using Persistence.Systems;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using _Car_Parking.Scripts.Characters;
using _Car_Parking.Scripts.CommonUtility;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.EngineTuning;
using _Car_Parking.Scripts.EngineTuning.DataTypes;

namespace _Car_Parking.Scripts.DataStorageService
{
	public class PlayerStorageService : IInitializable, IPlayerStorageService, IAccountDataProvider, IWheelsDataService, ICharacterEquipmentDataService, IPaintDataService, IDeleteUserData, IDisposable
	{
		public static ICharacterEquipmentDataService Instance;

		public const string FileKey = "PlayerData";

		private PlayerDataStorage _playerDataStorage;

		private IDeleterUserDataGetter _deleterUserDataGetter;

		private PresetGrantSession _presetGrantSession;

		private PlayerStorage _data;

		private bool _initialized;

		public PlayerStorage Data
		{
			get
			{
				return _data;
			}
			set
			{
				_data = value;
				_presetGrantSession?.Clear();
			}
		}

		public List<int> BoughtWheels => Data.Wheels;

		public EquipmentsData Male
		{
			get
			{
				return Data.EquipmentsMale;
			}
			set
			{
				Data.EquipmentsMale = value;
			}
		}

		public EquipmentsData Female
		{
			get
			{
				return Data.EquipmentsFemale;
			}
			set
			{
				Data.EquipmentsFemale = value;
			}
		}

		public int Gender
		{
			get
			{
				return Data.Gender;
			}
			set
			{
				Data.Gender = value;
			}
		}

		public ObscuredString Name
		{
			get
			{
				return _playerDataStorage.PlayerName;
			}
			set
			{
				_playerDataStorage.PlayerName = value;
			}
		}

		public ObscuredString AccountID
		{
			get
			{
				return _playerDataStorage.AccountID;
			}
			set
			{
				_playerDataStorage.AccountID = value;
			}
		}

		public ObscuredString LocalID
		{
			get
			{
				return _playerDataStorage.PlayerID;
			}
			set
			{
				_playerDataStorage.PlayerID = value;
			}
		}

		public event Action<CarMatType> OnBought;

		[Inject]
		public void Construct(PlayerDataStorage playerDataStorage, IDeleterUserDataGetter deleterUserDataGetter, PresetGrantSession presetGrantSession)
		{
			_playerDataStorage = playerDataStorage;
			_deleterUserDataGetter = deleterUserDataGetter;
			_presetGrantSession = presetGrantSession;
			_deleterUserDataGetter.Add(this);
		}

		public void Dispose()
		{
			_deleterUserDataGetter?.Remove(this);
		}

		public void Initialize()
		{
			if (!_initialized)
			{
				_initialized = true;
				Instance = this;
				Load();
			}
		}

		public bool IsCaliperBought(int value)
		{
			return (Data.Calipers & (1L << value)) != 0;
		}

		public void BuyCaliper(int value)
		{
			Data.Calipers |= 1L << value;
			Save();
		}

		public bool IsBrakeStock(BrakeDiscType type, int carId)
		{
			VehicleStockTuningData defaultCarTuningDataConfig = CarTuningFactory.GetDefaultCarTuningDataConfig(carId);
			if ((bool)defaultCarTuningDataConfig)
			{
				if (type != defaultCarTuningDataConfig.Tuning.FrontWheelData.BrakesData.DiscType)
				{
					return type == defaultCarTuningDataConfig.Tuning.RearWheelData.BrakesData.DiscType;
				}
				return true;
			}
			return false;
		}

		public bool IsBrakeBought(int value)
		{
			return (Data.Brakes & (1L << value)) != 0;
		}

		public void BuyBrake(int value)
		{
			Data.Brakes |= 1L << value;
			Save();
		}

		public bool CheckPaintBought(CarMatType paintType)
		{
			return (Data.MatTypes & (1 << (int)paintType)) != 0;
		}

		public void SetPaintBought(CarMatType paintType)
		{
			Data.MatTypes |= 1 << (int)paintType;
			Save();
			OnBought?.Invoke(paintType);
		}

		public void Save()
		{
			Debug.Log("Save Storage");
			using (_presetGrantSession?.Exclude(Data))
			{
				EasySaveDataHandler.SaveValues(new Dictionary<string, object> { { "PlayerData", Data } }, "PlayerData");
			}
		}

		private void Load()
		{
			if (EasySaveDataHandler.TryGetFile("PlayerData", out var file))
			{
				Data = PlayerDataExtensions.Read(file);
				return;
			}
			Data = new PlayerStorage();
			Save();
		}

		public void SetBasicPlayerData(string playerName, string localID)
		{
			_playerDataStorage.SetBasicPlayerData(playerName, localID);
		}

		public void DeleteData()
		{
			Data?.Clear();
			_presetGrantSession?.Clear();
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using Persistence.Systems;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using _Car_Parking.Scripts.DataEntities.Car;
using _Car_Parking.Scripts.DataStorageService;
using _Car_Parking.Scripts.EngineTuning;

namespace _Car_Parking.Scripts.Inventory
{
	public class EngineKitsInventoryService : IEngineKitsInventoryService, IInventoryBaseService<EngineKitData>, IInitializable
	{
		public const string InventoryEnginesDataKey = "InventoryEnginesData";

		private AllParts _allParts;

		private IStorageService _storageService;

		public EngineKitsInventoryData InventoryData => _storageService.Data.EngineKitsInventoryData;

		public event Action<EngineKitData> OnEngineRemoved;

		[Inject]
		public void Construct(AllParts allParts, IStorageService storageService)
		{
			_storageService = storageService;
			_allParts = allParts;
		}

		public void Add(EngineKitData partData)
		{
			if (partData != null)
			{
				if (IsInInventory(partData))
				{
					partData.EngineId.GenerateInstanceId();
				}
				_storageService.Data.EngineKitsInventoryData.Inventory.Add(partData);
				Save();
			}
		}

		public void Remove(EngineKitData partData)
		{
			if (!IsInInventory(partData))
			{
				Debug.LogWarning("Cant remove item. It's iot in inventory:" + partData);
				return;
			}
			OnEngineRemoved?.Invoke(partData);
			EngineKitData item = _storageService.Data.EngineKitsInventoryData.Inventory.Find((EngineKitData x) => x.EngineId.InstanceID == partData.EngineId.InstanceID);
			_storageService.Data.EngineKitsInventoryData.Inventory.Remove(item);
			Save();
		}

		public void SetData(EngineKitsInventoryData inventoryData)
		{
			if (inventoryData == null)
			{
				_storageService.Data.EngineKitsInventoryData = new EngineKitsInventoryData();
				Save();
				return;
			}
			_storageService.Data.EngineKitsInventoryData = inventoryData;
			foreach (EngineKitData item in _storageService.Data.EngineKitsInventoryData.Inventory)
			{
				item.RemoveEmptyPartSlots();
			}
			Save();
		}

		public void Clear()
		{
			_storageService.Data.EngineKitsInventoryData.Inventory.Clear();
			Save();
		}

		public EngineKitData GetEngineById(int engineID)
		{
			return _storageService.Data.EngineKitsInventoryData.Inventory.Find((EngineKitData x) => x.EngineId.InstanceID == engineID);
		}

		public bool IsInInventory(EngineKitData partData)
		{
			foreach (EngineKitData item in _storageService.Data.EngineKitsInventoryData.Inventory)
			{
				if (item.EngineId.InstanceID == partData.EngineId.InstanceID)
				{
					return true;
				}
			}
			return false;
		}

		public void Save()
		{
			EasySaveDataHandler.SaveValues(new Dictionary<string, object> { 
			{
				"InventoryEnginesData",
				_storageService.Data.EngineKitsInventoryData
			} }, "InventoryEnginesData");
		}

		public List<EngineKitData> GetAllEngines()
		{
			return _storageService.Data.EngineKitsInventoryData.Inventory.ToList();
		}

		public void Initialize()
		{
			Load();
		}

		private void Load()
		{
			if (EasySaveDataHandler.TryGetFile("InventoryEnginesData", out var file))
			{
				_storageService.Data.EngineKitsInventoryData = InventoryExtensions.ReadEnginesSetData(file);
				return;
			}
			_storageService.Data.EngineKitsInventoryData = new EngineKitsInventoryData();
			Save();
		}
	}
}

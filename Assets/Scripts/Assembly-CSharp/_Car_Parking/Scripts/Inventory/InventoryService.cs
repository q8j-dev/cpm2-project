using System.Collections.Generic;
using System.Linq;
using Persistence.Systems;
using UnityEngine;
using UnityEngine.Scripting;
using VContainer;
using VContainer.Unity;
using _Car_Parking.Scripts.DataStorageService;
using _Car_Parking.Scripts.Drag.Engine;
using _Car_Parking.Scripts.EngineTuning;

namespace _Car_Parking.Scripts.Inventory
{
	[UnityEngine.Scripting.Preserve]
	public class InventoryService : IInventoryPartsService, IInventoryBaseService<PartData>, IInitializable
	{
		public const string InventoryDataKey = "InventoryData";

		private AllParts _allParts;

		private IStorageService _storageService;

		public InventoryData InventoryData => _storageService.Data.InventoryData;

		[Inject]
		public InventoryService(AllParts allParts, IStorageService storageService)
		{
			_storageService = storageService;
			_allParts = allParts;
		}

		public void Add(PartData partData)
		{
			if (partData != null && partData.ModelId != 0)
			{
				if (IsInInventory(partData))
				{
					partData.GenerateInstanceId();
				}
				_storageService.Data.InventoryData.Inventory.Add(partData);
				PartItem part = partData.GetPart(_allParts);
				Debug.Log($"Add {partData} to inventory.", part);
				Debug.Log(JsonUtility.ToJson(partData));
				Save();
			}
		}

		public void Remove(PartData partData)
		{
			if (!IsInInventory(partData))
			{
				Debug.LogWarning("Cant remove item. It's iot in inventory:" + partData);
				return;
			}
			PartData item = _storageService.Data.InventoryData.Inventory.Find((PartData x) => x.InstanceID == partData.InstanceID);
			_storageService.Data.InventoryData.Inventory.Remove(item);
			Save();
		}

		public void Clear()
		{
			_storageService.Data.InventoryData.Inventory.Clear();
			Save();
		}

		public bool IsInInventory(PartData partData)
		{
			foreach (PartData item in _storageService.Data.InventoryData.Inventory)
			{
				if (item.InstanceID == partData.InstanceID)
				{
					return true;
				}
			}
			return false;
		}

		public List<PartData> GetAllParts(PartType partType)
		{
			return _storageService.Data.InventoryData.Inventory.Where((PartData x) => x.PartType == partType).ToList();
		}

		public void SetData(InventoryData inventoryData)
		{
			if (inventoryData == null)
			{
				_storageService.Data.InventoryData = new InventoryData();
				Save();
				return;
			}
			_storageService.Data.InventoryData = inventoryData;
			_storageService.Data.InventoryData.Inventory.RemoveAll((PartData partData) => _allParts.CheckCompactPartDataFromDataBase(partData) == null);
			Save();
		}

		public PartData GetCompatibleECU(int engineModelId, int carId)
		{
			return _storageService.Data.InventoryData.Inventory.Where((PartData x) => x.PartType == PartType.ECU).FirstOrDefault((PartData x) =>
			{
				ECUPart eCUPart = x.GetPart(_allParts) as ECUPart;
				return (bool)eCUPart && !eCUPart.CheckNotCompatibleFull(carId, engineModelId);
			});
		}

		public void Save()
		{
			EasySaveDataHandler.SaveValues(new Dictionary<string, object> { 
			{
				"InventoryData",
				_storageService.Data.InventoryData
			} }, "InventoryData");
			Debug.Log("Try save.");
		}

		public void Initialize()
		{
			Load();
		}

		private void Load()
		{
			if (EasySaveDataHandler.TryGetFile("InventoryData", out var file))
			{
				_storageService.Data.InventoryData = InventoryExtensions.Read(file);
				return;
			}
			_storageService.Data.InventoryData = new InventoryData();
			Save();
		}
	}
}

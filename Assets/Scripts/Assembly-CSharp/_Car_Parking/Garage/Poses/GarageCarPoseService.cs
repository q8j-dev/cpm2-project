using System;
using System.Collections.Generic;
using CPMEngine;
using Persistence.Systems;
using UnityEngine;
using UnityEngine.Scripting;
using VContainer.Unity;
using _Car_Parking.Scripts.CarSlots;
using _Car_Parking.Scripts.Database.Cars;

namespace _Car_Parking.Garage.Poses
{
	[Preserve]
	public class GarageCarPoseService : IGarageCarPoseService, IInitializable, IDisposable
	{
		internal const string FilePath = "GarageCarPoses_v2.es3";

		private readonly IDatabaseCarsService _databaseCarsService;

		public GarageCarPoseService(IDatabaseCarsService databaseCarsService)
		{
			_databaseCarsService = databaseCarsService;
		}

		public void Initialize()
		{
			_databaseCarsService.DeleteCarLocalData += DeleteCarLocalDataEventHandler;
		}

		private void DeleteCarLocalDataEventHandler(CarSlot carSlot)
		{
			RemovePose(carSlot.CarFilePath);
		}

		public bool TryGetPose(int garageId, string carId, out SavedPoseDto pose)
		{
			pose = default;
			if (string.IsNullOrEmpty(carId))
			{
				return false;
			}
			GarageCarPoseStash.RestoreInto(carId);
			if (!EasySaveDataHandler.TryGetFile("GarageCarPoses_v2.es3", out var file) || file == null)
			{
				return false;
			}
			if (!file.KeyExists(carId))
			{
				return false;
			}
			List<SavedPoseDto> list = file.Load(carId, new List<SavedPoseDto>());
			int num = list.FindIndex((SavedPoseDto p) => p.GarageId == garageId);
			if (num >= 0)
			{
				pose = list[num];
				return true;
			}
			return false;
		}

		public void SetPose(int garageId, string carId, Powertrain powertrain)
		{
			if (!string.IsNullOrEmpty(carId) && (bool)powertrain)
			{
				Vector3 position = powertrain.transform.position;
				float y = powertrain.transform.eulerAngles.y;
				float value = (powertrain ? (0f - powertrain.GetSteer()) : 0f);
				List<SavedPoseDto> list = null;
				if (EasySaveDataHandler.TryGetFile("GarageCarPoses_v2.es3", out var file) && file != null && file.KeyExists(carId))
				{
					list = file.Load(carId, new List<SavedPoseDto>());
				}
				if (list == null)
				{
					list = new List<SavedPoseDto>();
				}
				int num = list.FindIndex((SavedPoseDto p) => p.GarageId == garageId);
				SavedPoseDto savedPoseDto = new SavedPoseDto
				{
					GarageId = garageId,
					Position = position,
					Yaw = y,
					SteerNorm = Mathf.Clamp(value, -1f, 1f)
				};
				if (num >= 0)
				{
					list[num] = savedPoseDto;
				}
				else
				{
					list.Add(savedPoseDto);
				}
				EasySaveDataHandler.SaveValues(new Dictionary<string, object> { { carId, list } }, "GarageCarPoses_v2.es3");
			}
		}

		public void RemovePose(int garageId, string carId)
		{
			if (string.IsNullOrEmpty(carId) || !EasySaveDataHandler.TryGetFile("GarageCarPoses_v2.es3", out var file) || file == null || !file.KeyExists(carId))
			{
				return;
			}
			List<SavedPoseDto> list = file.Load(carId, new List<SavedPoseDto>());
			if (list.RemoveAll((SavedPoseDto p) => p.GarageId == garageId) != 0)
			{
				if (list.Count == 0)
				{
					EasySaveDataHandler.DeleteValue("GarageCarPoses_v2.es3", carId);
					EasySaveDataHandler.SaveFile("GarageCarPoses_v2.es3");
				}
				else
				{
					EasySaveDataHandler.SaveValues(new Dictionary<string, object> { { carId, list } }, "GarageCarPoses_v2.es3");
				}
			}
		}

		public void RemovePose(string carId)
		{
			if (string.IsNullOrEmpty(carId))
			{
				return;
			}
			GarageCarPoseStash.Forget(carId);
			if (!EasySaveDataHandler.TryGetFile("GarageCarPoses_v2.es3", out var file) || file == null || !file.KeyExists(carId))
			{
				return;
			}
			string[] keys = file.GetKeys();
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			string[] array = keys;
			foreach (string text in array)
			{
				if (!(text == carId))
				{
					List<SavedPoseDto> list = file.Load<List<SavedPoseDto>>(text, null);
					if (list != null)
					{
						dictionary[text] = list;
					}
				}
			}
			EasySaveDataHandler.DeleteFile("GarageCarPoses_v2.es3");
			if (dictionary.Count > 0)
			{
				EasySaveDataHandler.SaveValues(dictionary, "GarageCarPoses_v2.es3");
			}
		}

		public void Dispose()
		{
			_databaseCarsService.DeleteCarLocalData -= DeleteCarLocalDataEventHandler;
		}
	}
}

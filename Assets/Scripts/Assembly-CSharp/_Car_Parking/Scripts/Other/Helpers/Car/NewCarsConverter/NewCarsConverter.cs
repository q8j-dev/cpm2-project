using System;
using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

namespace _Car_Parking.Scripts.Other.Helpers.Car.NewCarsConverter
{
	public class NewCarsConverter
	{
		private BodyKitDataBuilder _bodyKitDataBuilder;

		private NewCarsDataForBackend _currentNewCarsDataForBackend;

		public async UniTask GenerateExternal(int carId)
		{
			_currentNewCarsDataForBackend = new NewCarsDataForBackend
			{
				CarId = carId
			};
			GenerateVinyls();
			await GenerateBodyKits();
			WriteFile();
		}

		private void GenerateVinyls()
		{
			AllVinylPreset allVinylPreset = Resources.Load<AllVinylPreset>("LiveryPresets/PresetLiveryCar" + _currentNewCarsDataForBackend.CarId);
			if (!allVinylPreset)
			{
				Debug.LogWarning("VinylsPreset not found: " + _currentNewCarsDataForBackend.CarId);
				return;
			}
			List<VinylPreset> list = new List<VinylPreset>();
			foreach (OneVinylsPreset allPreset in allVinylPreset.allPresetList)
			{
				list.Add(new VinylPreset(allPreset.vynilOneCarList, allPreset.carColor));
			}
			_currentNewCarsDataForBackend.VinylPresets = list;
		}

		private async UniTask GenerateBodyKits()
		{
			_bodyKitDataBuilder = new BodyKitDataBuilder();
			NewCarsDataForBackend currentNewCarsDataForBackend = _currentNewCarsDataForBackend;
			currentNewCarsDataForBackend.BodyKitDatas = await _bodyKitDataBuilder.LoadBodyKits(_currentNewCarsDataForBackend.CarId);
		}

		private void WriteFile()
		{
			string path = "CarPreset" + _currentNewCarsDataForBackend.CarId;
			string text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "NewCarsPresets");
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
			string path2 = Path.Combine(text, path);
			string contents = JsonConvert.SerializeObject(_currentNewCarsDataForBackend, Formatting.Indented);
			File.WriteAllText(path2, contents);
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using CodeStage.AntiCheat.Storage;
using Cysharp.Threading.Tasks;
using DataEntities.Car;
using Legacy.Database;
using Persistence.Systems;
using UnityEngine;
using _Car_Parking.Scripts.CommonUtility;
using _Car_Parking.Scripts.DataEntities.Car.CarEntities;
using _Car_Parking.Scripts.Other.FsoStrobeLights;
using _Car_Parking.Scripts.Persistence.Migration;

namespace _Car_Parking.Scripts.DataEntities.Migrators
{
	[DataMigrator]
	public class OneCarPrefsToEasySaveMigrator : MigratorBase
	{
		private int[] vectorsToRemove = new int[4] { 6, 13, 15, 16 };

		private int[] gearsToRemove = new int[3] { 0, 1, 9 };

		public override async UniTask<object> Migrate(object legacyData)
		{
			OneCarLegacy oneCarLegacy = (OneCarLegacy)legacyData;
			List<float> list = oneCarLegacy.floats;
			if (list == null)
			{
				list = new List<float>();
			}
			list.Resize2(44, 0f);
			List<Vector3> list2 = oneCarLegacy.vectors;
			if (list2 == null)
			{
				list2 = new List<Vector3>();
				list2.Resize2(18, Vector3.one * 2f);
			}
			if (list2.Count > 6)
			{
				list[33] = list2[6].x;
				list[34] = list2[6].y;
				if (list2[4].x != 2f)
				{
					list[35] = list2[6].z;
				}
				else
				{
					list[35] = 0.5f;
				}
			}
			else
			{
				list[33] = 0f;
				list[34] = 0f;
				list[35] = 0.5f;
			}
			if (list[21] == 0f)
			{
				list[21] = 0.03f;
			}
			if (list[22] == 0f)
			{
				list[22] = 0.03f;
			}
			if (list[24] == 0f)
			{
				list[24] = 0.03f;
			}
			if (list[26] == 0f)
			{
				list[26] = 0.03f;
			}
			if (list2.Count > 16)
			{
				list[36] = list2[16].x;
				list[37] = list2[16].y;
				list[38] = list2[16].z;
			}
			else
			{
				list[36] = 0f;
				list[37] = 0f;
				list[38] = 2f;
			}
			list[39] = 1f;
			list[40] = 1f;
			list[41] = 1f;
			list[42] = 1f;
			for (int num = vectorsToRemove.Length - 1; num >= 0; num--)
			{
				if (list2.Count > vectorsToRemove[num])
				{
					list2.RemoveAt(vectorsToRemove[num]);
				}
			}
			list2.Resize2(18, new Vector3(2f, 2f, 2f));
			if (oneCarLegacy.gears == null || (oneCarLegacy.gears.Count != 11 && oneCarLegacy.gears.Count != 8))
			{
				oneCarLegacy.gears = new List<float>();
				oneCarLegacy.gears.Resize2(8, -10f);
			}
			else
			{
				for (int num2 = gearsToRemove.Length - 1; num2 >= 0; num2--)
				{
					if (oneCarLegacy.gears.Count > gearsToRemove[num2])
					{
						oneCarLegacy.gears.RemoveAt(gearsToRemove[num2]);
					}
				}
				if (oneCarLegacy.gears.Count == 6)
				{
					float value = oneCarLegacy.gears[5];
					oneCarLegacy.gears.Resize2(8, -10f);
					oneCarLegacy.gears[5] = -10f;
					oneCarLegacy.gears[7] = value;
				}
			}
			List<int> list3 = BuyCar.BoughtPartsSetAndHasSync(oneCarLegacy.BoughtParts, 71);
			if (list3[27] == 1)
			{
				list[43] = 3000f;
			}
			_ = oneCarLegacy.fsoData;
			int[] fsoData;
			if (oneCarLegacy.fsoData == null)
			{
				oneCarLegacy.fsoData = FsoStrobeLightsConfig.DefaultFsoData;
				fsoData = oneCarLegacy.fsoData;
			}
			else
			{
				List<int> list4 = oneCarLegacy.fsoData.ToList();
				list4.Resize2(8, 0);
				fsoData = list4.ToArray();
			}
			if (oneCarLegacy.texts == null)
			{
				oneCarLegacy.texts = new List<string>();
				oneCarLegacy.texts.Resize2(3);
			}
			if (oneCarLegacy.typeToInstall == null)
			{
				oneCarLegacy.typeToInstall = new List<int>();
				oneCarLegacy.typeToInstall.Resize2(9, -2);
			}
			OneCar oneCar = new OneCar
			{
				BoughtParts = list3,
				CarID = oneCarLegacy.CarID,
				flagID = oneCarLegacy.flagID,
				floats = list,
				fsoData = fsoData,
				gears = oneCarLegacy.gears,
				texts = oneCarLegacy.texts,
				typeToInstall = oneCarLegacy.typeToInstall,
				vectors = list2,
				dataVersion = 2
			};
			OneCarFactory.CheckLength(oneCar);
			OverrideDataFromPrefs(oneCar);
			_ = oneCarLegacy.Vynils;
			return oneCar;
		}

		private void OverrideDataFromPrefs(OneCar oneCar)
		{
			int carID = oneCar.CarID;
			Color color = new Color(2f, 2f, 2f);
			Color color2 = PlayerDatas.GetColor("CarColor" + carID, color);
			if (color2 != color && oneCar.vectors[0].ToColor() != color2)
			{
				oneCar.vectors[0] = color2.ToVector3();
				Debug.LogError("carColor - load from prefs again car: " + carID);
			}
			float num = ObscuredPrefs.GetFloat("maxPower" + carID);
			float num2 = 0.01f;
			if (num != 0f && Math.Abs(oneCar.floats[1] - num) > num2)
			{
				oneCar.floats[1] = num;
				Debug.LogError("hp - load from prefs again");
			}
			float num3 = ObscuredPrefs.GetFloat("maxPowerRpm" + carID);
			if (num3 != 0f && Math.Abs(oneCar.floats[2] - num3) > num2)
			{
				oneCar.floats[2] = num;
				Debug.LogError("maxPowerRpm - load from prefs again car: " + carID);
			}
			float num4 = ObscuredPrefs.GetFloat("mileage" + carID, 2.4f);
			if (Math.Abs(num4 - 2.4f) > num2 && Math.Abs(num4 - oneCar.floats[19]) > num2)
			{
				oneCar.floats[19] = num4;
				Debug.LogError("mileage - load from prefs again car: " + carID);
			}
			List<int> list = new List<int>();
			for (int i = 0; i < oneCar.typeToInstall.Count; i++)
			{
				if (list[i] != -2 && oneCar.typeToInstall[i] != list[i])
				{
					oneCar.typeToInstall[i] = list[i];
					Debug.LogError("type to install - load from prefs again car: " + carID);
				}
			}
		}

		public override async UniTask<object> Migrate(object data, DataSource dataSource)
		{
			if (dataSource == DataSource.Internal)
			{
				OneCarLegacy legacyData = new OneCarLegacy((int)data);
				await UniTask.Delay(TimeSpan.FromMilliseconds(15.0));
				return await Migrate(legacyData);
			}
			throw new Exception("External fetch is not supported for single car entity");
		}

		public override Type GetFirstType()
		{
			return typeof(OneCarLegacy);
		}

		public override Type GetSecondType()
		{
			return typeof(OneCar);
		}

		public override IMigrationDataProvider GetMigrationProvider()
		{
			return new PlayerPrefsCarIdProvider();
		}

		public override async UniTask Commit(object data, bool external = false)
		{
			OneCar newData = (OneCar)data;
			newData.Save(path, deleteOld: true);
			int saveRetries = 10;
			for (int i = 0; i <= saveRetries; i++)
			{
				if (EasySaveDataHandler.FileExists(path + "/" + newData.Suffix))
				{
					break;
				}
				await UniTask.Delay(100);
				Debug.LogError("File at path " + path + "/" + newData.Suffix + " didnt exist after save! Retrying");
				newData.Save(path, deleteOld: true);
			}
			ObscuredPrefs.SetInt($"car{newData.CarID}", 1);
		}

		public override async UniTask Delete(object data)
		{
			_ = (int)data;
		}
	}
}

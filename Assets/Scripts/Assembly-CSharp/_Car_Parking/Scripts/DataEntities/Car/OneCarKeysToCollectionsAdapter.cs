using System;
using System.Collections.Generic;
using System.Linq;
using DataEntities.Car;
using Persistence.Systems;
using UnityEngine;
using _Car_Parking.Scripts.DataEntities.Car.CarEntities;
using _Car_Parking.Scripts.Other.FsoStrobeLights;

namespace _Car_Parking.Scripts.DataEntities.Car
{
	public class OneCarKeysToCollectionsAdapter : IOneCarKeysToCollectionsAdapter
	{
		private OneCar _car;

		public OneCarKeysToCollectionsAdapter(OneCar car)
		{
			_car = car;
			OneCarAdapterDictionaries.InitDictionaries();
		}

		public void SetVector(string key, Vector3 value)
		{
			if (OneCarAdapterDictionaries.vectorsInOneCar.TryGetValue(key, out var value2))
			{
				_car.vectors[value2] = value;
			}
		}

		public Vector3 GetVector(string key, Vector3 defaultValue)
		{
			if (OneCarAdapterDictionaries.vectorsInOneCar.TryGetValue(key, out var value))
			{
				Vector3 vector = _car.vectors[value];
				if (vector == new Vector3(2f, 2f, 2f))
				{
					return defaultValue;
				}
				return vector;
			}
			return defaultValue;
		}

		public void SetFloat(string key, float value, bool saveNow = false)
		{
			try
			{
				int index = OneCarAdapterDictionaries.floatsInOneCar[key];
				_car.floats[index] = value;
			}
			catch (Exception message)
			{
				Debug.LogWarning(message);
			}
			if (saveNow)
			{
				Save();
			}
		}

		public float GetFloat(string key, float defaultValue = 0f, bool ifZeroReturnDefault = false)
		{
			try
			{
				int index = OneCarAdapterDictionaries.floatsInOneCar[key];
				if (_car.floats == null)
				{
					_car.floats = Enumerable.Repeat(0f, 44).ToList();
					_car.floats[35] = 0.5f;
					_car.floats[10] = 0.22f;
					_car.floats[11] = 0.22f;
					_car.floats[33] = 2f;
					_car.floats[34] = 2f;
					_car.floats[39] = 1f;
					_car.floats[40] = 1f;
					_car.floats[41] = 1f;
					_car.floats[42] = 1f;
					Save();
				}
				float num = _car.floats[index];
				if (ifZeroReturnDefault && num == 0f)
				{
					return defaultValue;
				}
				return num;
			}
			catch (Exception)
			{
				if (_car.floats != null)
				{
					_car.floats.Count.ToString();
				}
				return defaultValue;
			}
		}

		public int GetIntFromFloat(string key, int defaultValue = 0, bool ifZeroReturnDefault = false)
		{
			try
			{
				return (int)GetFloat(key, defaultValue, ifZeroReturnDefault);
			}
			catch (Exception)
			{
				return defaultValue;
			}
		}

		public void SetBoughtPartBuy(string key, int value, bool saveNow = false)
		{
			int index = OneCarAdapterDictionaries.boughtPartsInOneCar[key];
			_car.BoughtParts[index] = value;
			if (saveNow)
			{
				Save();
			}
		}

		public void SetBoughtPart(string key, int value, bool saveNow = false)
		{
			int index = OneCarAdapterDictionaries.boughtPartsInOneCar[key];
			_car.BoughtParts[index] = value;
			if (saveNow)
			{
				Save();
			}
		}

		public int GetBoughtPart(string key, int defaultValue = 0)
		{
			try
			{
				int index = OneCarAdapterDictionaries.boughtPartsInOneCar[key];
				if (_car.BoughtParts == null)
				{
					_car.BoughtParts = Enumerable.Repeat(0, 71).ToList();
					Save();
				}
				int num = ((_car.BoughtParts != null) ? _car.BoughtParts[index] : defaultValue);
				if (key == "hasTurbo" && num == -1)
				{
					return defaultValue;
				}
				return num;
			}
			catch (Exception)
			{
				return defaultValue;
			}
		}

		public bool HasBoughtPart(string key)
		{
			if (OneCarAdapterDictionaries.boughtPartsInOneCar != null)
			{
				return OneCarAdapterDictionaries.boughtPartsInOneCar.Count > 0;
			}
			return false;
		}

		public bool IsBought(int i)
		{
			return GetBoughtPart(i + "_") == 1;
		}

		public void SetTypeToInstall(int keyInt, int value, bool saveNow = false)
		{
			_car.typeToInstall[keyInt] = value;
			if (saveNow)
			{
				Save();
			}
		}

		public int GetTypeToInstall(int keyInt, int defaultValue = 0)
		{
			try
			{
				if (_car.typeToInstall == null)
				{
					_car.typeToInstall = Enumerable.Repeat(-2, 9).ToList();
					Save();
				}
				return (_car.typeToInstall != null) ? _car.typeToInstall[keyInt] : defaultValue;
			}
			catch (Exception)
			{
				return defaultValue;
			}
		}

		public void SetGears(int keyInt, float value, bool saveNow = false)
		{
			_car.gears[keyInt] = value;
			if (saveNow)
			{
				Save();
			}
		}

		public void SetGears(string key, float value, bool saveNow = false)
		{
			if (key == "finalRatio")
			{
				_car.gears[7] = value;
			}
			if (saveNow)
			{
				Save();
			}
		}

		public float GetGears(int index, float defaultValue = 0f)
		{
			try
			{
				if (_car.gears == null)
				{
					_car.gears = Enumerable.Repeat(-10f, 8).ToList();
					Save();
				}
				if (index < _car.gears.Count)
				{
					float num = _car.gears[index];
					if (num == -10f)
					{
						return defaultValue;
					}
					return num;
				}
				return defaultValue;
			}
			catch (Exception)
			{
				return defaultValue;
			}
		}

		public float GetGears(string key, float defaultValue)
		{
			try
			{
				if (key == "finalRatio")
				{
					if (_car.gears == null)
					{
						_car.gears = Enumerable.Repeat(-10f, 8).ToList();
						Save();
					}
					float num = ((_car.gears != null) ? _car.gears[7] : defaultValue);
					if (num == 0f || num == -10f)
					{
						return defaultValue;
					}
					return num;
				}
				return 0f;
			}
			catch (Exception)
			{
				return defaultValue;
			}
		}

		public void SetString(string key, string value, bool saveNow = false)
		{
			CheckAndCreateTexts();
			switch (key)
			{
			case "tireText":
				_car.texts[0] = value;
				break;
			case "hasTypeToInstall":
				_car.texts[1] = value;
				break;
			case "carLocalId":
				_car.texts[2] = value;
				break;
			default:
				Debug.LogWarning("error SetString key:" + key);
				break;
			}
			if (saveNow)
			{
				Save();
			}
		}

		public string GetString(string key, string defaultValue = "")
		{
			CheckAndCreateTexts();
			try
			{
				if (key == "tireText")
				{
					return (_car.texts != null) ? _car.texts[0] : "";
				}
				return defaultValue;
			}
			catch (Exception)
			{
				return defaultValue;
			}
		}

		private void CheckAndCreateTexts()
		{
			if (_car.texts == null)
			{
				_car.texts = new List<string>();
				_car.texts.Add("");
				_car.texts.Add("");
				_car.texts.Add("");
				Save();
			}
		}

		public void SetColor(string key, Color color, bool saveNow = false)
		{
			SetVector(key, color.ToVector3());
			if (saveNow)
			{
				Save();
			}
		}

		public Color GetColor(string key, Color defaultValue)
		{
			Vector3 defaultValue2 = defaultValue.ToVector3();
			return GetVector(key, defaultValue2).ToColor();
		}

		public void DeleteAllData()
		{
			int carID = _car.CarID;
			EasySaveDataHandler.DeleteFile(string.Format("{0}/car{1}", "cars", _car.CarID));
			Debug.Log($"Car{carID} files was deleted.");
		}

		public void Commit()
		{
			Save();
		}

		private void Save()
		{
			_car.Save("cars");
		}

		public void SetFlag(int lastSelectedFlagIndex, bool saveNow = false)
		{
			_car.flagID = lastSelectedFlagIndex;
			if (saveNow)
			{
				Save();
			}
		}

		public int GetFlag(int i)
		{
			return _car.flagID;
		}

		public int GetFsoIndex()
		{
			try
			{
				if (_car.fsoData == null)
				{
					_car.fsoData = FsoStrobeLightsConfig.DefaultFsoData;
					Save();
				}
				return (_car.fsoData != null) ? _car.fsoData[0] : (-1);
			}
			catch
			{
				return -1;
			}
		}

		public void SetFsoIndex(int value, bool saveNow = false)
		{
			_car.fsoData[0] = value;
			if (saveNow)
			{
				Save();
			}
		}

		public void SetFsoPos(int value, bool saveNow = false)
		{
			_car.fsoData[1] = value;
			if (saveNow)
			{
				Save();
			}
		}

		public int GetFsoPos()
		{
			try
			{
				if (_car.fsoData == null)
				{
					_car.fsoData = FsoStrobeLightsConfig.DefaultFsoData;
					Save();
				}
				return (_car.fsoData != null) ? _car.fsoData[1] : 0;
			}
			catch
			{
				return 0;
			}
		}

		public void SetFsoColor(Color left, Color right, bool saveNow = false)
		{
			_car.fsoData[2] = (int)(left.r * 255f);
			_car.fsoData[3] = (int)(left.g * 255f);
			_car.fsoData[4] = (int)(left.b * 255f);
			_car.fsoData[5] = (int)(right.r * 255f);
			_car.fsoData[6] = (int)(right.g * 255f);
			_car.fsoData[7] = (int)(right.b * 255f);
			if (saveNow)
			{
				Save();
			}
		}

		public Color GetFsoColorPrefsLeft()
		{
			try
			{
				return new Color((float)_car.fsoData[2] / 255f, (float)_car.fsoData[3] / 255f, (float)_car.fsoData[4] / 255f);
			}
			catch
			{
				return Color.white;
			}
		}

		public Color GetFsoColorPrefsRight()
		{
			try
			{
				if (_car.fsoData.Length > 5)
				{
					return new Color((float)_car.fsoData[5] / 255f, (float)_car.fsoData[6] / 255f, (float)_car.fsoData[7] / 255f);
				}
				return new Color((float)_car.fsoData[2] / 255f, (float)_car.fsoData[3] / 255f, (float)_car.fsoData[4] / 255f);
			}
			catch
			{
				return Color.white;
			}
		}

		public void DeleteKeyFso()
		{
			_car.fsoData = FsoStrobeLightsConfig.DefaultFsoData;
			Save();
		}
	}
}

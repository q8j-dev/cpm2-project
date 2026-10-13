using System.Collections.Generic;
using CodeStage.AntiCheat.Storage;
using UnityEngine;

namespace _Car_Parking.Scripts.Other
{
	public class PrefsArrayInt
	{
		public enum TypeSave
		{
			PlayerPrefs = 0,
			ObscuredPrefs = 1
		}

		private class SaveLoadPlayerPrefs : ISaveLoad
		{
			public void Set(string key, string value)
			{
				PlayerPrefs.SetString(key, value);
			}

			public string Get(string key)
			{
				return PlayerPrefs.GetString(key);
			}
		}

		private class SaveLoadObscuredPrefs : ISaveLoad
		{
			public void Set(string key, string value)
			{
				ObscuredPrefs.SetString(key, value);
			}

			public string Get(string key)
			{
				return ObscuredPrefs.GetString(key);
			}
		}

		private interface ISaveLoad
		{
			void Set(string key, string value);

			string Get(string key);
		}

		private const string Separator = "#";

		private string _key;

		private ISaveLoad _saveLoad;

		public string PrefsValue
		{
			get
			{
				return _saveLoad.Get(_key);
			}
			set
			{
				_saveLoad.Set(_key, value);
			}
		}

		public PrefsArrayInt(string key, TypeSave typeSave)
		{
			_key = key;
			switch (typeSave)
			{
			case TypeSave.PlayerPrefs:
				_saveLoad = new SaveLoadPlayerPrefs();
				break;
			case TypeSave.ObscuredPrefs:
				_saveLoad = new SaveLoadObscuredPrefs();
				break;
			}
		}

		public List<int> GetIntList()
		{
			List<int> list = new List<int>();
			string prefsValue = PrefsValue;
			if (string.IsNullOrEmpty(prefsValue))
			{
				return list;
			}
			string[] array = prefsValue.Split("#");
			foreach (string text in array)
			{
				if (!string.IsNullOrEmpty(text))
				{
					list.Add(int.Parse(text));
				}
			}
			return list;
		}

		public bool Contains(int value)
		{
			return GetIntList().Contains(value);
		}

		public void AddIfNotContains(int value)
		{
			if (!Contains(value))
			{
				Add(value);
			}
		}

		public void Add(int value)
		{
			List<int> intList = GetIntList();
			intList.Add(value);
			PrefsValue = string.Join("#", intList);
		}

		public void Remove(int value)
		{
			List<int> intList = GetIntList();
			intList.Remove(value);
			PrefsValue = string.Join("#", intList);
		}
	}
}

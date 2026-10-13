using System.Collections.Generic;
using UnityEngine;
using _Car_Parking.Scripts.CommonUtility;

namespace _Car_Parking.Scripts.EditableHUD
{
	public class EditablePrefsSaver : IAllPrefsSaver
	{
		private string _allPlayerPrefsKeys;

		private Dictionary<string, ElementChangesData> _changesDatas = new Dictionary<string, ElementChangesData>();

		public void Save()
		{
			_allPlayerPrefsKeys = PlayerPrefs.GetString("EditableHUDAllKeys", "");
			string[] array = _allPlayerPrefsKeys.Split("###");
			for (int i = 0; i < array.Length; i++)
			{
				ElementChangesData elementChangesData = new ElementChangesData(Vector2.zero);
				elementChangesData.Load(array[i]);
				_changesDatas.Add(array[i], elementChangesData);
			}
		}

		public void Load()
		{
			PlayerPrefs.SetString("EditableHUDAllKeys", _allPlayerPrefsKeys);
			foreach (KeyValuePair<string, ElementChangesData> changesData in _changesDatas)
			{
				changesData.Value.Save(changesData.Key);
				PlayerPrefs.SetInt(changesData.Key, 1);
			}
			_allPlayerPrefsKeys = "";
			_changesDatas.Clear();
		}
	}
}

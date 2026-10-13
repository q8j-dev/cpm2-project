using System;
using System.Collections.Generic;
using CPM.PlayerData;
using Newtonsoft.Json;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace _Car_Parking.Scripts.Notification
{
	public class EquipmentNotificationService : INotificationService, IDisposable, IInitializable
	{
		private readonly PlayerDataStorage _playerDataStorage;

		private const string SaveKey = "equipment_notices_";

		private string _saveKey;

		private Dictionary<EquipmentKey, EquipmentNotice> _notices = new Dictionary<EquipmentKey, EquipmentNotice>();

		public bool HasNotification => _notices.Count > 0;

		public event Action<int, int> NoticeByTypeChanged;

		[Inject]
		public EquipmentNotificationService(PlayerDataStorage playerDataStorage)
		{
			_playerDataStorage = playerDataStorage;
		}

		public void Initialize()
		{
			_saveKey = "equipment_notices_" + _playerDataStorage.AccountID;
			if (PlayerPrefs.HasKey(_saveKey))
			{
				List<EquipmentNotice> list = JsonConvert.DeserializeObject<List<EquipmentNotice>>(PlayerPrefs.GetString(_saveKey));
				_notices = new Dictionary<EquipmentKey, EquipmentNotice>();
				if (list == null)
				{
					return;
				}
				{
					foreach (EquipmentNotice item in list)
					{
						EquipmentKey key = new EquipmentKey(item.Type, item.Index, item.Gender);
						_notices[key] = item;
					}
					return;
				}
			}
			_notices.Clear();
			Save();
		}

		public void SetNotice(int type, int index, int gender)
		{
			EquipmentKey key = new EquipmentKey(type, index, gender);
			if (!_notices.ContainsKey(key))
			{
				_notices[key] = new EquipmentNotice(type, index, gender);
				Save();
			}
		}

		public void MarkAsRead(int type, int index, int gender)
		{
			EquipmentKey key = new EquipmentKey(type, index, gender);
			_notices.Remove(key);
			Save();
			NoticeByTypeChanged?.Invoke(type, gender);
		}

		public List<EquipmentNotice> GetNotices(int gender)
		{
			List<EquipmentNotice> list = new List<EquipmentNotice>();
			foreach (EquipmentNotice value in _notices.Values)
			{
				if (value.Gender == gender)
				{
					list.Add(value);
				}
			}
			return list;
		}

		public bool HasNotice(int type, int index, int gender)
		{
			EquipmentKey key = new EquipmentKey(type, index, gender);
			return _notices.ContainsKey(key);
		}

		public bool HasNotice(PersonWindowCategoryType categoryType, int gender)
		{
			if (_notices == null || _notices.Count == 0)
			{
				return false;
			}
			int num;
			int num2;
			switch (categoryType)
			{
			case PersonWindowCategoryType.Skin:
				num = 0;
				num2 = 2;
				break;
			case PersonWindowCategoryType.Cloth:
			case PersonWindowCategoryType.Events:
				num = 3;
				num2 = 10;
				break;
			default:
				return false;
			}
			foreach (EquipmentNotice value in _notices.Values)
			{
				if (value.Gender == gender && value.Type >= num && value.Type <= num2)
				{
					return true;
				}
			}
			return false;
		}

		public bool HasNotice(int type, int gender)
		{
			foreach (KeyValuePair<EquipmentKey, EquipmentNotice> notice in _notices)
			{
				if (notice.Value.Type == type && notice.Value.Gender == gender)
				{
					return true;
				}
			}
			return false;
		}

		public bool HasNotice(int gender)
		{
			foreach (KeyValuePair<EquipmentKey, EquipmentNotice> notice in _notices)
			{
				if (notice.Value.Gender == gender)
				{
					return true;
				}
			}
			return false;
		}

		public bool AnyNotice(int gender, Func<EquipmentNotice, bool> predicate)
		{
			foreach (EquipmentNotice value in _notices.Values)
			{
				if (value.Gender == gender && predicate(value))
				{
					return true;
				}
			}
			return false;
		}

		public void Dispose()
		{
			Save();
		}

		private void Save()
		{
			List<EquipmentNotice> value = new List<EquipmentNotice>(_notices.Values);
			PlayerPrefs.SetString(_saveKey, JsonConvert.SerializeObject(value));
			PlayerPrefs.Save();
		}
	}
}

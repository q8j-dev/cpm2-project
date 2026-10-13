using System.Collections.Generic;
using CPM.PlayerData;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.Notification;

namespace _Car_Parking.Scripts.Animations.Emotes.Locker
{
	public class EmoteNoticeTracker
	{
		private const string SnapshotKeyPrefix = "emoteLockerSeen_";

		private readonly INotificationService _notifications;

		private readonly PlayerDataStorage _playerDataStorage;

		[Inject]
		public EmoteNoticeTracker(INotificationService notifications, PlayerDataStorage playerDataStorage)
		{
			_notifications = notifications;
			_playerDataStorage = playerDataStorage;
		}

		public void RegisterCurrentContent(int type, IReadOnlyList<int> currentIds)
		{
			string key = SnapshotKey(type);
			bool flag = !PlayerPrefs.HasKey(key);
			HashSet<int> hashSet = new HashSet<int>(EmoteLockerPrefs.ParseList(PlayerPrefs.GetString(key, string.Empty)));
			if (!flag)
			{
				foreach (int currentId in currentIds)
				{
					if (!hashSet.Contains(currentId))
					{
						_notifications.SetNotice(type, currentId, -1);
					}
				}
			}
			if (flag || !hashSet.SetEquals(currentIds))
			{
				PlayerPrefs.SetString(key, EmoteLockerPrefs.JoinList(currentIds));
				PlayerPrefs.Save();
			}
		}

		public bool HasNotice(int type)
		{
			return _notifications.HasNotice(type, -1);
		}

		public void MarkRead(int type, IReadOnlyList<int> ids)
		{
			foreach (int id in ids)
			{
				_notifications.MarkAsRead(type, id, -1);
			}
		}

		private string SnapshotKey(int type)
		{
			return "emoteLockerSeen_" + type + "_" + _playerDataStorage.AccountID;
		}
	}
}

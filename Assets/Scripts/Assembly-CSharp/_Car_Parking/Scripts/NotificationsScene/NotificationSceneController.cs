using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using _Car_Parking.Scripts.Other.Helpers;

namespace _Car_Parking.Scripts.NotificationsScene
{
	public class NotificationSceneController : SerializedMonoBehaviour
	{
		[OdinSerialize]
		private Dictionary<SceneNotificationReason, GameObject> _windows = new Dictionary<SceneNotificationReason, GameObject>();

		private void Awake()
		{
			FlagsStorage.IsMainMenu = false;
			SceneNotificationReason reason = SceneNotificationContext.Reason;
			SceneNotificationContext.Reset();
			foreach (KeyValuePair<SceneNotificationReason, GameObject> window in _windows)
			{
				window.Value.SetActive(window.Key == reason);
			}
			if (!_windows.ContainsKey(reason) && _windows.ContainsKey(SceneNotificationReason.AppUpdate))
			{
				_windows[SceneNotificationReason.AppUpdate].SetActive(value: true);
			}
		}

		private void OnValidate()
		{
			foreach (KeyValuePair<SceneNotificationReason, GameObject> window in _windows)
			{
				if (!window.Value)
				{
					Debug.LogError($"Window for {window.Key} is not set");
				}
			}
		}
	}
}

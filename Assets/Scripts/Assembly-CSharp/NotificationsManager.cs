using UnityEngine;

public static class NotificationsManager
{
	public delegate void NotificationDelegate(string s, float? time);

	private static NotificationDelegate temp;

	public static event NotificationDelegate OnNotification;

	public static event NotificationDelegate CurrentNotifications;

	public static void ShowNotification(string s, float time)
	{
		CurrentNotifications?.Invoke(s, time);
	}

	public static void SetActive(GameObject gameObject, bool state = true)
	{
		gameObject.SetActive(state);
	}
}

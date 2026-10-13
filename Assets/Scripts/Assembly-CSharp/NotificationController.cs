using TMPro;
using UnityEngine;

public class NotificationController : MonoBehaviour
{
	public TextMeshProUGUI text;

	public new bool animation;

	public NotificationController notificationController;

	private float speed = 5f;

	private void Awake()
	{
		NotificationsManager.CurrentNotifications += ShowMessage;
		if (!animation)
		{
			base.gameObject.SetActive(value: false);
		}
		NotificationsManager.SetActive(base.gameObject, state: false);
	}

	private void ShowMessage(string s, float? time)
	{
		text.text = s;
		base.gameObject.SetActive(value: true);
		NotificationsManager.SetActive(base.gameObject);
		if (animation)
		{
			_ = base.gameObject.activeInHierarchy;
		}
	}

	private void OnEnable()
	{
		if (notificationController != null)
		{
			notificationController.gameObject.SetActive(value: false);
		}
	}

	private void OnDisable()
	{
		if (notificationController != null)
		{
			notificationController.gameObject.SetActive(value: true);
		}
	}

	private void OnDestroy()
	{
		NotificationsManager.CurrentNotifications -= ShowMessage;
	}
}

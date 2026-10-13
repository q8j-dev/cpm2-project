using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using _Car_Parking.Scripts.Database.PlayerData;

namespace _Car_Parking.Scripts.UIElement
{
	public class PhoneNotificationMarkController : MonoBehaviour
	{
		private const float DelaySecondsToShow = 90f;

		private const int MinMinutesGameplayToShow = 20;

		[SerializeField]
		private GameObject Notification;

		[SerializeField]
		private Button PhoneButton;

		private CancellationTokenSource _cancellationShowNotification;

		private void Start()
		{
			if (AllTimeGamePlayUtil.Get() < 20)
			{
				CancelShowNotification();
				_cancellationShowNotification = new CancellationTokenSource();
				ShowNotificationAsync(_cancellationShowNotification.Token).Forget();
			}
		}

		private void OnEnable()
		{
			PhoneButton.onClick.AddListener(OnPhoneButtonClick);
		}

		private void OnDisable()
		{
			PhoneButton.onClick.RemoveListener(OnPhoneButtonClick);
		}

		private void OnDestroy()
		{
			CancelShowNotification();
		}

		private void OnPhoneButtonClick()
		{
			Notification.SetActive(value: false);
		}

		private async UniTaskVoid ShowNotificationAsync(CancellationToken cancellationToken)
		{
			await UniTask.Delay(TimeSpan.FromSeconds(90.0), ignoreTimeScale: false, PlayerLoopTiming.Update, cancellationToken);
			if (!cancellationToken.IsCancellationRequested)
			{
				Notification.SetActive(value: true);
			}
		}

		private void CancelShowNotification()
		{
			_cancellationShowNotification?.Cancel();
			_cancellationShowNotification?.Dispose();
			_cancellationShowNotification = null;
		}
	}
}

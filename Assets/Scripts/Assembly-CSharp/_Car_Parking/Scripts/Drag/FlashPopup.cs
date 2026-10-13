using System;
using UniRx;
using UnityEngine;
using VContainer;

namespace _Car_Parking.Scripts.Drag
{
	public class FlashPopup : MonoBehaviour, IPopupWindow
	{
		[SerializeField]
		private FinishFlashController _finishFlashController;

		private IDisposable _flash;

		private UiNotificationsManager _uiNotificationsManager;

		[Inject]
		public void Construct(UiNotificationsManager uiNotificationsManager)
		{
			_uiNotificationsManager = uiNotificationsManager;
		}

		public void Init(PopupData popupData)
		{
			SoundEventsDataHelper.PlayUISound(SoundEventsDataHelper.UITimeAttackFinishFlash, base.gameObject);
			_finishFlashController.TriggerFinishFlash();
			_flash = Observable.Timer(TimeSpan.FromSeconds(1.0)).Subscribe((long _) =>
			{
				_uiNotificationsManager.Remove(popupData);
			}).AddTo(this);
		}

		public void Close()
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}
}

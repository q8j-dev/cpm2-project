using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.WindowSystem;

namespace _Car_Parking.Scripts.NotificationsScene
{
	public class WrongAuthorization : MonoBehaviour
	{
		[SerializeField]
		private float DelayToShow = 0.1f;

		[SerializeField]
		private string TitleKey = default;

		[SerializeField]
		private string DescriptionKey = default;

		private ManagerUI _managerUI;

		[Inject]
		public void Construct(ManagerUI managerUI)
		{
			_managerUI = managerUI;
		}

		private void Start()
		{
			ShowMessage(this.GetCancellationTokenOnDestroy()).Forget();
		}

		private async UniTask ShowMessage(CancellationToken ct = default(CancellationToken))
		{
			await UniTask.Delay(TimeSpan.FromSeconds(DelayToShow), ignoreTimeScale: false, PlayerLoopTiming.Update, ct);
			if (!_managerUI)
			{
				Debug.LogError("can't even show message");
				return;
			}
			_managerUI.gameObject.SetActive(value: true);
			ConfirmWindow.AdditionalData additionalData = new ConfirmWindow.AdditionalData
			{
				CancelButtonVisible = false
			};
			_managerUI.ShowConfirmWindow(() =>
			{
				AuthorizationWindow.AdditionalData value = new AuthorizationWindow.AdditionalData
				{
					BackWindowKey = "WrongAuthorization"
				};
				_managerUI.ShowAuthorizationWindow(JsonConvert.SerializeObject(value));
			}, new LocalizedString(TitleKey), new LocalizedString(DescriptionKey), null, "Confirm", null, additionalData).Forget();
		}
	}
}

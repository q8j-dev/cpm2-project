using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.SOAP_Registry;

namespace _Car_Parking.Scripts.WindowSystem.Confirmation
{
	public class SimpleConfirmWindow : WindowScript
	{
		[SerializeField]
		private TextMeshProUGUI Title = default;

		[SerializeField]
		private TextMeshProUGUI ContentText = default;

		[SerializeField]
		private Button ConfirmButton = default;

		[SerializeField]
		private Button CancelButton = default;

		[SerializeField]
		private TextMeshProUGUI ConfirmButtonText = default;

		private Action _onConfirmed;

		private Action _onCanceled;

		protected override InputGroupsEnum WindowInputGroup => InputGroupsEnum.Ui;

		protected override void Start()
		{
			SoundEventsDataHelper.PlayUISound(SoundEventsDataHelper.ConfirmationWindowSwitch, base.gameObject);
			ConfirmButton.onClick.AddListener(Confirm);
			CancelButton.onClick.AddListener(Cancel);
			_managerUI.OnShowScreen?.Invoke(obj: false);
		}

		protected override void OnDestroy()
		{
			ConfirmButton.onClick.RemoveAllListeners();
			CancelButton.onClick.RemoveAllListeners();
			_onConfirmed = null;
			_onCanceled = null;
			_managerUI.OnShowScreen?.Invoke(obj: true);
		}

		public void Init(string title, string content, Action onConfirmed, Action onCanceled, string confirmButtonText = null)
		{
			Title.SetText(title);
			ContentText.SetText(content);
			ConfirmButtonText.SetText(confirmButtonText ?? ((string)new LocalizedString("Playerprofilewindow.Confirm")));
			_onConfirmed = onConfirmed;
			_onCanceled = onCanceled;
		}

		private void Confirm()
		{
			_onConfirmed?.Invoke();
			Close();
		}

		private void Cancel()
		{
			_onCanceled?.Invoke();
			Close();
		}
	}
}

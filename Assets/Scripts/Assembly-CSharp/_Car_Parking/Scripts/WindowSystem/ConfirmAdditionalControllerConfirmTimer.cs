using TMPro;
using UnityEngine;
using UnityEngine.UI;
using _Car_Parking.Scripts.Localization;

namespace _Car_Parking.Scripts.WindowSystem
{
	public class ConfirmAdditionalControllerConfirmTimer : ConfirmAdditionalController
	{
		private const string LocalizationConfirmKey = "Confirmwindow.Confirm";

		private float _timeToConfirm;

		private TMP_Text _timeText;

		private Button _confirmButton;

		private bool _showConfirmText;

		public ConfirmAdditionalControllerConfirmTimer(float timeToConfirm, TMP_Text timeText, Button confirmButton, bool showConfirmText = true)
		{
			_timeToConfirm = timeToConfirm;
			_timeText = timeText;
			_confirmButton = confirmButton;
			_confirmButton.interactable = false;
			_showConfirmText = showConfirmText;
		}

		public override void Update()
		{
			if (!(_timeToConfirm < 0f))
			{
				_timeToConfirm -= Time.unscaledDeltaTime;
				if (_timeToConfirm < 0f && (bool)_confirmButton)
				{
					_confirmButton.interactable = true;
					_timeText.text = (_showConfirmText ? ((string)new LocalizedString("Confirmwindow.Confirm")) : "");
				}
				else
				{
					_timeText.text = $"{Mathf.RoundToInt(_timeToConfirm)}";
				}
			}
		}
	}
}

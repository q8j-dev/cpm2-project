using System;
using CPMEngine;
using UnityEngine;
using UnityEngine.UI;

namespace _Car_Parking.Scripts.PhotoMode.UI
{
	public class PhotoModeParameterItem : MonoBehaviour
	{
		[SerializeField]
		private Toggle SelectToggle;

		[field: SerializeField]
		public CPMGraphicsSettings.ColorCorrectionLut Filter { get; private set; }

		public event Action<CPMGraphicsSettings.ColorCorrectionLut> OnSelected;

		private void Start()
		{
			SelectToggle.onValueChanged.AddListener(Select);
		}

		public void SetToggleState(bool isOn)
		{
			SelectToggle.isOn = isOn;
		}

		private void Select(bool isOn)
		{
			if (isOn)
			{
				SoundEventsDataHelper.PlayUISound(SoundEventsDataHelper.PhotoModeSettingsSwitch, base.gameObject);
				OnSelected?.Invoke(Filter);
				Utils.Vibrate(0.05f, 0.1f, unscaledTime: true);
			}
		}
	}
}

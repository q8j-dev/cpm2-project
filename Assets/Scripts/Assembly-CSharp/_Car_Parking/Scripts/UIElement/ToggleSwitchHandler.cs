using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Car_Parking.Scripts.UIElement
{
	[RequireComponent(typeof(Toggle))]
	public class ToggleSwitchHandler : MonoBehaviour
	{
		[Serializable]
		public class BackgroundElements
		{
			public Image Background;

			public Sprite On;

			public Sprite Off;
		}

		[Serializable]
		public class TextElements
		{
			public TextMeshProUGUI Text;

			public Color On = Color.black;

			public Color Off = Color.white;
		}

		[Serializable]
		public class InteractableElements
		{
			public TextMeshProUGUI Text;

			public Image CheckMark;

			public Color On = Color.white;

			public Color Off = Color.gray;
		}

		[Serializable]
		public class ToggleImageElements
		{
			public Image ImageToColorChange;

			public Color On = Color.white;

			public Color Off = Color.white;
		}

		[Serializable]
		public class ToggleRawImageElements
		{
			public RawImage ImageToColorChange;

			public Color On = Color.white;

			public Color Off = Color.white;
		}

		[SerializeField]
		private Toggle CurrentToggle;

		[SerializeField]
		private bool SwitchBackground;

		[SerializeField]
		private BackgroundElements BackgroundToSwitch;

		[SerializeField]
		private bool SwitchTextColor;

		[SerializeField]
		private TextElements TextToSwitch;

		[SerializeField]
		private bool CanSetInteractable;

		[SerializeField]
		private InteractableElements Interactable;

		[SerializeField]
		private bool SwitchImageColor;

		[SerializeField]
		private ToggleImageElements ColorSwitchElements;

		[SerializeField]
		private bool SwitchRawImageColor;

		[SerializeField]
		private ToggleRawImageElements RawColorSwitchElements;

		private void Start()
		{
			if (!CurrentToggle)
			{
				CurrentToggle = GetComponent<Toggle>();
			}
			Switch(CurrentToggle.isOn);
			CurrentToggle.onValueChanged.AddListener(Switch);
		}

		public void Switch(bool isOn)
		{
			if (SwitchBackground)
			{
				Sprite sprite = (isOn ? BackgroundToSwitch.On : BackgroundToSwitch.Off);
				BackgroundToSwitch.Background.sprite = sprite;
			}
			if (SwitchTextColor)
			{
				Color color = (isOn ? TextToSwitch.On : TextToSwitch.Off);
				TextToSwitch.Text.color = color;
			}
			if (SwitchImageColor && (bool)ColorSwitchElements.ImageToColorChange)
			{
				Color color2 = (isOn ? ColorSwitchElements.On : ColorSwitchElements.Off);
				ColorSwitchElements.ImageToColorChange.color = color2;
			}
			if (SwitchRawImageColor && (bool)RawColorSwitchElements.ImageToColorChange)
			{
				Color color3 = (isOn ? RawColorSwitchElements.On : RawColorSwitchElements.Off);
				RawColorSwitchElements.ImageToColorChange.color = color3;
			}
		}

		public void SetInteractable(bool isOn)
		{
			if (CanSetInteractable)
			{
				CurrentToggle.interactable = isOn;
				Color color = (isOn ? Interactable.On : Interactable.Off);
				Interactable.Text.color = color;
				Interactable.CheckMark.color = color;
			}
		}
	}
}

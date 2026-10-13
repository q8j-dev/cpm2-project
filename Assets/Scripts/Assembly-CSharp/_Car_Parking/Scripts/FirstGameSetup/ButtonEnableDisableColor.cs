using TMPro;
using UnityEngine;
using UnityEngine.UI;
using _Car_Parking.Scripts.Localization;

namespace _Car_Parking.Scripts.FirstGameSetup
{
	public class ButtonEnableDisableColor : MonoBehaviour
	{
		[SerializeField]
		private ColorImageSetup[] ColorImageSetups;

		[SerializeField]
		private ColorTMPTextSetup[] ColorTMPTextSetups;

		[field: SerializeField]
		public RectTransform RectTransform { get; private set; }

		[field: SerializeField]
		public Button Button { get; private set; }

		public void Enable()
		{
			Button.enabled = true;
			ColorImageSetup[] colorImageSetups = ColorImageSetups;
			foreach (ColorImageSetup colorImageSetup in colorImageSetups)
			{
				SetColorImage(colorImageSetup.Image, colorImageSetup.EnabledColor);
			}
			ColorTMPTextSetup[] colorTMPTextSetups = ColorTMPTextSetups;
			foreach (ColorTMPTextSetup colorTMPTextSetup in colorTMPTextSetups)
			{
				SetColorTMPText(colorTMPTextSetup.TMPText, colorTMPTextSetup.EnabledColor);
				SetLocalizationText(colorTMPTextSetup.TMPText, colorTMPTextSetup.EnabledTextKey);
			}
			LayoutRebuilder.ForceRebuildLayoutImmediate(RectTransform);
		}

		public void Disable()
		{
			Button.enabled = false;
			ColorImageSetup[] colorImageSetups = ColorImageSetups;
			foreach (ColorImageSetup colorImageSetup in colorImageSetups)
			{
				SetColorImage(colorImageSetup.Image, colorImageSetup.DisabledColor);
			}
			ColorTMPTextSetup[] colorTMPTextSetups = ColorTMPTextSetups;
			foreach (ColorTMPTextSetup colorTMPTextSetup in colorTMPTextSetups)
			{
				SetColorTMPText(colorTMPTextSetup.TMPText, colorTMPTextSetup.DisabledColor);
				SetLocalizationText(colorTMPTextSetup.TMPText, colorTMPTextSetup.DisabledTextKey);
			}
			LayoutRebuilder.ForceRebuildLayoutImmediate(RectTransform);
		}

		private void SetColorImage(Image image, Color color)
		{
			image.color = color;
		}

		private void SetColorTMPText(TMP_Text text, Color color)
		{
			text.color = color;
		}

		private void SetLocalizationText(TMP_Text text, string key)
		{
			if (!key.IsNullOrEmpty())
			{
				text.text = new LocalizedString(key);
			}
		}

		private void OnDestroy()
		{
			Button.onClick.RemoveAllListeners();
		}
	}
}

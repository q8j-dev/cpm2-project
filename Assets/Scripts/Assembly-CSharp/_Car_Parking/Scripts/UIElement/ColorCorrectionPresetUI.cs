using UnityEngine;
using UnityEngine.UI;

namespace _Car_Parking.Scripts.UIElement
{
	public class ColorCorrectionPresetUI : MonoBehaviour
	{
		[SerializeField]
		private Button PickButton;

		[SerializeField]
		private CPMGraphicsSettings.ColorCorrectionLut ColorCorrectionLut;

		private void Start()
		{
			PickButton.onClick.AddListener(PickPreset);
		}

		private void PickPreset()
		{
			PlayerPrefs.SetInt("colorCorrection", (int)ColorCorrectionLut);
			CPMGraphicsSettingsManager.Instance.UseColorCorrectionLut = ColorCorrectionLut;
			CPMGraphicsSettingsManager.Instance.SaveConfig();
		}
	}
}

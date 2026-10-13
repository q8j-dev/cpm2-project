using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Car_Parking.Scripts.UIElement
{
	public class SurvivalTimerView : MonoBehaviour
	{
		[SerializeField]
		private TMP_Text TimerText;

		[SerializeField]
		private Image TimerBg;

		[SerializeField]
		private Color GoodColor;

		[SerializeField]
		private Color BadColor;

		public void UpdateVisibility(bool on)
		{
			base.gameObject.SetActive(on);
		}

		public void SetColor(bool isGood)
		{
			if ((bool)TimerBg)
			{
				TimerBg.color = (isGood ? GoodColor : BadColor);
			}
		}

		public void SetTimer(float remainingSeconds)
		{
			TimerText.text = TimeFormatter.FormatCountdownSeconds(remainingSeconds);
		}
	}
}

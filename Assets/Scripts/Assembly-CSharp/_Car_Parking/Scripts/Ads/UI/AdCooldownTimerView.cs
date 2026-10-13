using TMPro;
using UnityEngine;

namespace _Car_Parking.Scripts.Ads.UI
{
	public sealed class AdCooldownTimerView
	{
		private readonly TMP_Text _label;

		public AdCooldownTimerView(TMP_Text label)
		{
			_label = label;
		}

		public void SetCooldown(float secondsLeft)
		{
			if ((bool)_label)
			{
				bool flag = secondsLeft > 0f;
				GameObject gameObject = _label.gameObject;
				if (gameObject.activeSelf != flag)
				{
					gameObject.SetActive(flag);
				}
				if (flag)
				{
					int num = Mathf.CeilToInt(secondsLeft);
					_label.text = $"{num / 60:00}:{num % 60:00}";
				}
			}
		}
	}
}

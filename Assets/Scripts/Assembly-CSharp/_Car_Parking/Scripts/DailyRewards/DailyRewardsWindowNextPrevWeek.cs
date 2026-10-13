using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using _Car_Parking.Scripts.Localization;

namespace _Car_Parking.Scripts.DailyRewards
{
	public class DailyRewardsWindowNextPrevWeek : MonoBehaviour
	{
		[SerializeField]
		private TMP_Text Text;

		[SerializeField]
		private Button Button;

		public event Action<DailyRewardsWindowNextPrevWeek> OnClicked;

		public void UpdateText(int week)
		{
			Text.text = string.Format(new LocalizedString("Mainmenuwindow.Rewards.Week"), week + 1);
		}

		private void OnEnable()
		{
			Button.onClick.AddListener(OnButtonClicked);
		}

		private void OnDisable()
		{
			Button.onClick.RemoveListener(OnButtonClicked);
		}

		private void OnButtonClicked()
		{
			OnClicked?.Invoke(this);
		}
	}
}

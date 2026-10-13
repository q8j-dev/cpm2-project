using TMPro;
using UnityEngine;

namespace _Car_Parking.Scripts.DailyRewards
{
	public class DailyRewardItemTypeViewMoney : DailyRewardItemTypeView
	{
		[SerializeField]
		private TMP_Text MoneyText;

		public override DailyRewardData.TypeReward TypeReward => DailyRewardData.TypeReward.Money;

		public override void Init(DailyRewardData dailyRewardData)
		{
			MoneyText.text = TextExtensions.IntFormatThousandSpace(dailyRewardData.Money);
		}
	}
}

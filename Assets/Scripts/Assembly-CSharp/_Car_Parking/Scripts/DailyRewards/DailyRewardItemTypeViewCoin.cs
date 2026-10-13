using TMPro;
using UnityEngine;

namespace _Car_Parking.Scripts.DailyRewards
{
	public class DailyRewardItemTypeViewCoin : DailyRewardItemTypeView
	{
		[SerializeField]
		private TMP_Text CoinText = default;

		public override DailyRewardData.TypeReward TypeReward => DailyRewardData.TypeReward.Coin;

		public override void Init(DailyRewardData dailyRewardData)
		{
			CoinText.text = TextExtensions.IntFormatThousandSpace(dailyRewardData.Coin);
		}
	}
}

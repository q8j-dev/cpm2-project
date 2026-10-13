using UnityEngine;
using _Car_Parking.Scripts.Other.Helpers;
using _Car_Parking.Scripts.UIElement;

namespace _Car_Parking.Scripts.DailyRewards
{
	public class DailyRewardItemTypeViewCar : DailyRewardItemTypeView
	{
		[SerializeField]
		private RawImageLoadAsync CarIcon = default;

		[SerializeField]
		private GameObject CarMisteryIcon = default;

		public override DailyRewardData.TypeReward TypeReward => DailyRewardData.TypeReward.Car;

		public override void Init(DailyRewardData dailyRewardData)
		{
			CarMisteryIcon.SetActive(value: false);
			CarIcon.Load(PathGetterHelper.GetCarPreviewPath(dailyRewardData.Car));
		}
	}
}

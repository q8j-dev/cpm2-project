using UnityEngine;
using _Car_Parking.Scripts.Other.Helpers;
using _Car_Parking.Scripts.UIElement;

namespace _Car_Parking.Scripts.DailyRewards
{
	public class RewardSellCarView : MonoBehaviour
	{
		[SerializeField]
		private GameObject LoadingRoot;

		[SerializeField]
		private CombinedPriceView PriceView;

		public void UpdateLoadingView(bool isOn)
		{
			LoadingRoot.SetActive(isOn);
		}

		public void UpdatePriceView(bool isOn)
		{
			PriceView.SetActive(isOn);
		}

		public void SetPrice(CombinedPrice combinedPrice)
		{
			PriceView.Init(combinedPrice);
		}
	}
}

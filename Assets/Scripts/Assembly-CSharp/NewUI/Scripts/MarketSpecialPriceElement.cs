using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace NewUI.Scripts
{
	public class MarketSpecialPriceElement : MonoBehaviour
	{
		[SerializeField]
		private Text OldPriceText = default;

		[SerializeField]
		private Text SpecialPriceText = default;

		[SerializeField]
		private TextMeshProUGUI DiscountText = default;

		[SerializeField]
		private Button BuyButton = default;

		public void Init(string oldPrice, string specialPrice, string discount)
		{
			OldPriceText.text = oldPrice;
			SpecialPriceText.text = specialPrice;
			DiscountText.text = discount;
		}

		public void AddListener(UnityAction action)
		{
			BuyButton.onClick.RemoveAllListeners();
			BuyButton.onClick.AddListener(action);
		}
	}
}

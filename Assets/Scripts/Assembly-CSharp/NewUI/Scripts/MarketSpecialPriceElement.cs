using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace NewUI.Scripts
{
	public class MarketSpecialPriceElement : MonoBehaviour
	{
		[SerializeField]
		private Text OldPriceText;

		[SerializeField]
		private Text SpecialPriceText;

		[SerializeField]
		private TextMeshProUGUI DiscountText;

		[SerializeField]
		private Button BuyButton;

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

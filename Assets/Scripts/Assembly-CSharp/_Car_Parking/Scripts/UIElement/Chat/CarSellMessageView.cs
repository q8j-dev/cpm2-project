using TMPro;
using UnityEngine;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Trade;

namespace _Car_Parking.Scripts.UIElement.Chat
{
	public class CarSellMessageView : CarTradeMessageView
	{
		[SerializeField]
		private TextMeshProUGUI PriceText;

		[SerializeField]
		private TextMeshProUGUI TitleText;

		protected override void SetRequestedItem()
		{
			if (TradeOffer.RequestItem is TradeItemMoney { Price: var price })
			{
				PriceText.text = price.ToString();
			}
		}

		protected override void OfferCompleteText()
		{
			TitleText.text = new LocalizedString("Chat.CarPurchased");
		}
	}
}

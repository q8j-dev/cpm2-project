using UnityEngine;
using UnityEngine.UI;
using _Car_Parking.Scripts.Other.Helpers;
using _Car_Parking.Scripts.Trade;

namespace _Car_Parking.Scripts.UIElement.Chat
{
	public class CarExchangeMessageView : CarTradeMessageView
	{
		[SerializeField]
		private Image ReceiverCarImage;

		protected override void SetRequestedItem()
		{
			if (TradeOffer.RequestItem is TradeItemCar tradeItemCar)
			{
				Sprite sprite = Resources.Load(PathGetterHelper.GetCarPreviewPath(tradeItemCar.CarID), typeof(Sprite)) as Sprite;
				ReceiverCarImage.sprite = sprite;
			}
		}
	}
}

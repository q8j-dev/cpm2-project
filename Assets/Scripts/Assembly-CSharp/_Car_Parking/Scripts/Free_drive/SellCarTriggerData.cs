using System;
using _Car_Parking.Scripts.CustomDataTypes;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Trade;

namespace _Car_Parking.Scripts.Free_drive
{
	public class SellCarTriggerData
	{
		private readonly CommonProvider _commonProvider;

		public int Index { get; }

		public string CarModel { get; private set; }

		public string TradeOfferId { get; private set; }

		public AdvancedInt PriceCar
		{
			get
			{
				TradeOffer tradeOffer = _commonProvider.TradeService?.GetOffer(TradeOfferId) ?? null;
				if (tradeOffer != null && tradeOffer.RequestItem != null && tradeOffer.RequestItem.ItemType == TradeItemType.Money)
				{
					return (tradeOffer.RequestItem as TradeItemMoney).Price;
				}
				return new AdvancedInt(-1);
			}
		}

		public bool IsOccupied => !string.IsNullOrEmpty(TradeOfferId);

		public event Action<SellCarTriggerData> OnDataChanged;

		public SellCarTriggerData(int index, CommonProvider commonProvider)
		{
			Index = index;
			_commonProvider = commonProvider;
		}

		public void Init(string carModel, string tradeOfferId)
		{
			CarModel = carModel;
			TradeOfferId = tradeOfferId;
			OnDataChanged?.Invoke(this);
		}

		public void Clear()
		{
			Init("", "");
		}
	}
}

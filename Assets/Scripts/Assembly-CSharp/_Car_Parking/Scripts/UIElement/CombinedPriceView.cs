using TMPro;
using UnityEngine;
using UnityEngine.UI;
using _Car_Parking.Scripts.CarSlots;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Other.Helpers;

namespace _Car_Parking.Scripts.UIElement
{
	public class CombinedPriceView : MonoBehaviour
	{
		private const float InVisibleAlpha = 0.25f;

		[SerializeField]
		private CanvasGroup Root;

		[SerializeField]
		private GameObject MoneyRoot;

		[SerializeField]
		private GameObject CoinsRoot;

		[SerializeField]
		private TMP_Text MoneyText;

		[SerializeField]
		private TMP_Text CoinsText;

		[SerializeField]
		private Image CoinsIcon;

		[SerializeField]
		private Image MoneyIcon;

		[SerializeField]
		private GameObject Spacer;

		[SerializeField]
		private bool UseDollarInMoney = true;

		[Header("Real currency")]
		[SerializeField]
		private GameObject RealCurrencyRoot;

		[SerializeField]
		private GameObject MoneyCoinRoot;

		[SerializeField]
		private GameObject SimpleCurrencyPriceRoot;

		[SerializeField]
		private GameObject BothCurrencyPriceRoot;

		[SerializeField]
		private Text[] CurrencyPriceText;

		[SerializeField]
		private Text CurrencyOldPriceText;

		[SerializeField]
		private TMP_Text DiscountText;

		[SerializeField]
		private Text DiscountTextV2;

		[SerializeField]
		private GameObject DiscountRoot;

		[SerializeField]
		private GameObject CurrencyOldPriceRoot;

		[SerializeField]
		private GameObject ExtraSlotRoot;

		[SerializeField]
		private TMP_Text ExtraSlotText;

		[SerializeField]
		private ForceRebuildLayoutImmediate[] ForceRebuildLayoutImmediates;

		public void Init(CombinedPrice price, bool mustBuySlot = false)
		{
			if (price == null)
			{
				return;
			}
			SetCoinsPanelActive(state: false);
			Spacer.SetActive(value: false);
			SetMoneyPanelActive(state: false);
			if (price.IsHaveMoneyAndCoin)
			{
				SetCoinsPanelActive(state: true);
				Spacer.SetActive(value: true);
				SetMoneyPanelActive(state: true);
				MoneyText.text = GetMoneyText(price);
				SetCoinText(price, mustBuySlot);
			}
			else if (price.IsHaveCoin)
			{
				SetCoinsPanelActive(state: true);
				SetCoinText(price, mustBuySlot);
			}
			else
			{
				SetMoneyPanelActive(state: true);
				MoneyText.text = GetMoneyText(price);
			}
			SetRealCurrencyIfNeed(price, mustBuySlot);
			if (ForceRebuildLayoutImmediates == null)
			{
				return;
			}
			ForceRebuildLayoutImmediate[] forceRebuildLayoutImmediates = ForceRebuildLayoutImmediates;
			foreach (ForceRebuildLayoutImmediate forceRebuildLayoutImmediate in forceRebuildLayoutImmediates)
			{
				if ((bool)forceRebuildLayoutImmediate)
				{
					forceRebuildLayoutImmediate.RebuildLayout();
				}
			}
		}

		private string GetMoneyText(CombinedPrice price)
		{
			if (!UseDollarInMoney)
			{
				return $"{price.Money:n0}";
			}
			return $"{price.Money:n0}$";
		}

		private void SetCoinText(CombinedPrice price, bool mustBuySlot)
		{
			if (mustBuySlot)
			{
				int num = VehicleSlotPaymentService.Price;
				int num2 = (int)price.Coin - num;
				string text = new LocalizedString("SlotUI.TheSlot");
				CoinsText.text = ((num2 > 0) ? $"{num2:n0} + {num:n0} ({text})" : $"+{num:n0} ({text})");
			}
			else
			{
				CoinsText.text = $"{price.Coin:n0}";
			}
		}

		public void UpdateVisibility(bool isVisible)
		{
			if ((bool)Root)
			{
				Root.alpha = (isVisible ? 1f : 0.25f);
			}
		}

		private void SetCoinsPanelActive(bool state)
		{
			CoinsText.SetActive(state);
			CoinsIcon.SetActive(state);
			if ((bool)CoinsRoot)
			{
				CoinsRoot.SetActive(state);
			}
		}

		private void SetMoneyPanelActive(bool state)
		{
			MoneyText.SetActive(state);
			MoneyIcon.SetActive(state);
			if ((bool)MoneyRoot)
			{
				MoneyRoot.SetActive(state);
			}
		}

		private void SetRealCurrencyIfNeed(CombinedPrice price, bool mustBuySlot = false)
		{
			if (!RealCurrencyRoot || !MoneyCoinRoot)
			{
				return;
			}
			RealCurrencyRoot.SetActive(price.IsRealСurrency);
			MoneyCoinRoot.SetActive(!price.IsRealСurrency);
			if (!price.IsRealСurrency)
			{
				return;
			}
			Text[] currencyPriceText = CurrencyPriceText;
			foreach (Text text in currencyPriceText)
			{
				text.text = TextExtensions.FormatPrice(price.RealСurrencyPrice.Price, price.RealСurrencyPrice.IsoCurrencyCode);
				if (!string.IsNullOrEmpty(price.RealСurrencyPrice.PriceText))
				{
					text.text = price.RealСurrencyPrice.PriceText;
				}
			}
			CurrencyOldPriceText.text = TextExtensions.FormatPrice(price.RealСurrencyPrice.OldPrice, price.RealСurrencyPrice.IsoCurrencyCode);
			if (!string.IsNullOrEmpty(price.RealСurrencyPrice.OldPriceText))
			{
				CurrencyOldPriceText.text = price.RealСurrencyPrice.OldPriceText;
			}
			DiscountText.text = $"-{price.RealСurrencyPrice.Discount}%";
			if ((bool)DiscountTextV2)
			{
				DiscountTextV2.text = DiscountText.text;
			}
			DiscountRoot.SetActive(price.RealСurrencyPrice.Discount > 0);
			SimpleCurrencyPriceRoot.SetActive(price.RealСurrencyPrice.OldPrice <= 0f);
			BothCurrencyPriceRoot.SetActive(price.RealСurrencyPrice.OldPrice > 0f);
			if (RealCurrencyRoot.TryGetComponentInChildren<CrossedLine>(out var component))
			{
				component.UpdateLine();
			}
			if ((bool)ExtraSlotRoot)
			{
				ExtraSlotRoot.SetActive(mustBuySlot);
				ExtraSlotText.text = string.Format("+{0:n0} ({1})", VehicleSlotPaymentService.Price, new LocalizedString("SlotUI.TheSlot"));
			}
			IRectTransformUpdated[] componentsInChildren = RealCurrencyRoot.GetComponentsInChildren<IRectTransformUpdated>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].OnRectTransformUpdated();
			}
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using _Car_Parking.Scripts.FirstGameSetup;

namespace _Car_Parking.Scripts.Basket
{
	public class BasketView : MonoBehaviour
	{
		[SerializeField]
		private RectTransform BasketAreaRect;

		[SerializeField]
		private RectTransform Content;

		[SerializeField]
		private Image BasketButtonImage;

		[SerializeField]
		private Image CoinBasketButtonImage;

		[SerializeField]
		private TMP_Text BasketButtonText;

		[SerializeField]
		private BasketItem BasketItemPrefab;

		[SerializeField]
		private BasketItemsVisualConfig BasketItemsVisualConfig;

		[field: SerializeField]
		public float TotalObjectHeight { get; private set; }

		[field: SerializeField]
		public float BasketItemHeight { get; private set; }

		[field: SerializeField]
		public float BasketSpacing { get; private set; }

		[field: SerializeField]
		public float InitialBasketAreaHeight { get; private set; }

		[field: SerializeField]
		public int MaxVisibleItems { get; private set; } = 6;

		[field: SerializeField]
		public ButtonEnableDisableColor BuyButton { get; private set; }

		[field: SerializeField]
		public GameObject TotalPriceObject { get; private set; }

		[field: SerializeField]
		public TMP_Text TotalPriceText { get; private set; }

		public Dictionary<Enum, BasketItem> BasketItemsDic { get; private set; } = new Dictionary<Enum, BasketItem>();

		public Dictionary<Enum, BasketItemVisual> BasketItemsVisualDic { get; private set; } = new Dictionary<Enum, BasketItemVisual>();

		public bool BasketEmpty { get; private set; } = true;

		public bool HaveLockedItems
		{
			get
			{
				foreach (KeyValuePair<Enum, BasketItem> item in BasketItemsDic)
				{
					if (item.Value.gameObject.activeSelf && item.Value.Info.IsLocked)
					{
						return true;
					}
				}
				return false;
			}
		}

		public event Action<BasketItem> BasketItemCreated;

		public void Init()
		{
			BasketItemsVisualDic = BasketItemsVisualConfig.GetBasketItemsVisuals();
			BuyButton.enabled = false;
		}

		public void AddItemToBasket<TEnum>(TEnum type, BasketItemInfoBase info) where TEnum : Enum
		{
			if (!BasketItemsDic.ContainsKey(type))
			{
				BasketItem basketItem = UnityEngine.Object.Instantiate(BasketItemPrefab, Content);
				BasketItemsDic.Add(type, basketItem);
				BasketItemCreated?.Invoke(basketItem);
			}
			BasketItemsDic[type].gameObject.SetActive(value: true);
			BasketItemsDic[type].Init(info);
			BasketItemsDic[type].SetPrice();
			if (BasketItemsVisualDic.TryGetValue(type, out var value))
			{
				BasketItemsDic[type].SetIcon(value.Icon);
				BasketItemsDic[type].SetName(value.NameKey);
			}
			if (BasketItemsDic.Values.Count((BasketItem item) => item.gameObject.activeSelf) == 1)
			{
				BuyButton.Enable();
				TotalPriceObject.SetActive(value: true);
			}
			RecalculateAreaHeight();
			BasketEmpty = false;
		}

		private void RecalculateAreaHeight()
		{
			int num = BasketItemsDic.Values.Count((BasketItem item) => item.gameObject.activeSelf);
			float num2 = InitialBasketAreaHeight;
			if (num > 0)
			{
				num2 += TotalObjectHeight;
				num2 += (float)Mathf.Min(num, MaxVisibleItems) * (BasketItemHeight + BasketSpacing);
			}
			Vector2 sizeDelta = BasketAreaRect.sizeDelta;
			sizeDelta.y = num2;
			BasketAreaRect.sizeDelta = sizeDelta;
		}

		public void RemoveAllItemsFromBasket()
		{
			BasketItemsDic.Values.Where((BasketItem item) => item.gameObject.activeSelf).ForEach((BasketItem item) =>
			{
				item.gameObject.SetActive(value: false);
			});
			BuyButton.Disable();
			Vector2 sizeDelta = BasketAreaRect.sizeDelta;
			sizeDelta.y = InitialBasketAreaHeight;
			BasketAreaRect.sizeDelta = sizeDelta;
			TotalPriceObject.SetActive(value: false);
			BasketEmpty = true;
		}

		private void OnDestroy()
		{
			BasketItemCreated = null;
		}
	}
}

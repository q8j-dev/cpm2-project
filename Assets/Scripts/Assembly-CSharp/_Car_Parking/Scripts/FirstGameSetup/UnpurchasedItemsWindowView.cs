using System;
using System.Collections.Generic;
using Sirenix.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.Basket;
using _Car_Parking.Scripts.Drag;

namespace _Car_Parking.Scripts.FirstGameSetup
{
	public class UnpurchasedItemsWindowView : MonoBehaviour, IBackableWindow
	{
		[Inject]
		private ManagerUI _managerUI;

		[field: SerializeField]
		public RectTransform ParentForItems { get; private set; }

		[field: SerializeField]
		public ButtonEnableDisableColor BuyButton { get; private set; }

		[field: SerializeField]
		public Button ContinueNotBuyButton { get; private set; }

		[field: SerializeField]
		public Button BackButton { get; private set; }

		[field: SerializeField]
		public GameObject Total { get; private set; }

		[field: SerializeField]
		public TMP_Text TotalPriceText { get; private set; }

		[field: SerializeField]
		public BasketItemsVisualConfig BasketItemsVisualConfig { get; private set; }

		[field: SerializeField]
		public BasketItem BasketItemPrefab { get; private set; }

		public Dictionary<Enum, BasketItem> BasketItemsDic { get; private set; } = new Dictionary<Enum, BasketItem>();

		public Dictionary<Enum, BasketItemVisual> BasketItemsVisualDic { get; private set; } = new Dictionary<Enum, BasketItemVisual>();

		public event Action<BasketItem> BasketItemCreated;

		public void Init()
		{
			BasketItemsVisualDic = BasketItemsVisualConfig.GetBasketItemsVisuals();
			BackButton.onClick.AddListener(Back);
			ContinueNotBuyButton.onClick.AddListener(Back);
		}

		public void Back()
		{
			BackButton.onClick.Invoke();
		}

		public void InitBackButton()
		{
			_managerUI.SetCurrentWindow(this);
		}

		public void AddItem<TEnum>(TEnum type, BasketItemInfoBase info, Texture texture) where TEnum : Enum
		{
			if (!BasketItemsDic.ContainsKey(type))
			{
				BasketItem basketItem = UnityEngine.Object.Instantiate(BasketItemPrefab, ParentForItems);
				BasketItemsDic.Add(type, basketItem);
				BasketItemCreated?.Invoke(basketItem);
			}
			BasketItemsDic[type].gameObject.SetActive(value: true);
			BasketItemsDic[type].Init(info);
			BasketItemsDic[type].SetPrice();
			BasketItemsDic[type].SetName(BasketItemsVisualDic[type].NameKey);
			BasketItemsDic[type].SetTexture(texture);
		}

		private void OnEnable()
		{
			_managerUI.PersonWindowUnpurchasedWindowActive = true;
			InitBackButton();
		}

		private void OnDisable()
		{
			BasketItemsDic.Values.ForEach((BasketItem item) =>
			{
				item.gameObject.SetActive(value: false);
			});
			_managerUI.PersonWindowUnpurchasedWindowActive = false;
			_managerUI.TryRemoveFromCurrentWindows(this);
		}

		private void OnDestroy()
		{
			UnsubscribeUI();
		}

		public void UnsubscribeUI()
		{
			BackButton.onClick.RemoveAllListeners();
			ContinueNotBuyButton.onClick.RemoveAllListeners();
			BuyButton.Button.onClick.RemoveAllListeners();
			BasketItemCreated = null;
		}
	}
}

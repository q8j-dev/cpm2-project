using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.Analytics.Analyzers;
using _Car_Parking.Scripts.CarSlots;
using _Car_Parking.Scripts.MenuUI.LocalizationUI;
using _Car_Parking.Scripts.Other.Helpers;

namespace _Car_Parking.Scripts.UIElement
{
	public class ButtonBuyNewCar : MonoBehaviour
	{
		[SerializeField]
		private Button Button;

		[SerializeField]
		private CombinedPriceView CombinedPriceView;

		[SerializeField]
		private RectTransform ContentLayoutRect;

		private bool _isDestroyed;

		private CombinedPrice _price;

		private bool _mustBuySlot;

		private VirtualPurchaseAnalytic _virtualPurchaseAnalytic;

		private ISlotService _slotService;

		public Action<ButtonBuyNewCar> OnClick;

		[field: SerializeField]
		public LocalizedTextBase Label { get; private set; }

		[field: SerializeField]
		public string DefaultLabelKey { get; private set; }

		public GameObject PriceRoot
		{
			get
			{
				if (!ContentLayoutRect)
				{
					return null;
				}
				return ContentLayoutRect.gameObject;
			}
		}

		public bool Interactable
		{
			get
			{
				return Button.interactable;
			}
			set
			{
				Button.interactable = value;
				CombinedPriceView.UpdateVisibility(value);
			}
		}

		[Inject]
		public void Construct(VirtualPurchaseAnalytic virtualPurchaseAnalytic, ISlotService slotService)
		{
			_virtualPurchaseAnalytic = virtualPurchaseAnalytic;
			_slotService = slotService;
		}

		public async UniTaskVoid SetPrice(CombinedPrice price, bool mustBuySlot)
		{
			OnClick = null;
			if ((bool)CombinedPriceView)
			{
				CombinedPriceView.Init(price, mustBuySlot);
			}
			await UniTask.Yield();
			if (!_isDestroyed)
			{
				_price = price;
				_mustBuySlot = mustBuySlot;
				if ((bool)ContentLayoutRect && ContentLayoutRect.gameObject.activeInHierarchy)
				{
					LayoutRebuilder.ForceRebuildLayoutImmediate(ContentLayoutRect);
				}
			}
		}

		private void OnEnable()
		{
			Button.onClick.AddListener(OnClickButton);
		}

		private void OnDisable()
		{
			Button.onClick.RemoveListener(OnClickButton);
		}

		private void OnDestroy()
		{
			_isDestroyed = true;
		}

		private void OnClickButton()
		{
			OnClick?.Invoke(this);
		}
	}
}

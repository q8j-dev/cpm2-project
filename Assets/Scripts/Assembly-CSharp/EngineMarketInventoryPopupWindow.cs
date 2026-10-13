using System;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using _Car_Parking.Scripts.Analytics.Analyzers;
using _Car_Parking.Scripts.Analytics.Types;
using _Car_Parking.Scripts.EngineTuning;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.UIElement;

public class EngineMarketInventoryPopupWindow : MonoBehaviour
{
	[SerializeField]
	private GameObject BuyPanel = default;

	[SerializeField]
	private GameObject BuyAndSetPanel = default;

	[SerializeField]
	private Button BuyButton = default;

	[SerializeField]
	private Button CancelButton = default;

	[SerializeField]
	private ContentSizeFitter BuyButtonSizeFitter = default;

	[SerializeField]
	private TextMeshProUGUI BuyButtonText = default;

	[SerializeField]
	private TextMeshProUGUI TitleText = default;

	[SerializeField]
	private TextMeshProUGUI EnginePriceText = default;

	[SerializeField]
	private TextMeshProUGUI InstalledDetailNameText = default;

	[SerializeField]
	private TextMeshProUGUI BuyDetailNameText = default;

	[SerializeField]
	private TextMeshProUGUI BuyAndSetDetailText = default;

	[SerializeField]
	private DefaultEngineMarketWindow DefaultEngineMarketWindow = default;

	[SerializeField]
	private GameObject MoneyIcon = default;

	private VirtualPurchaseAnalytic _virtualPurchaseAnalytic;

	private EngineKitOffer _offer;

	private Action _onConfirm;

	private Action _onCancel = default;

	private void Start()
	{
		BuyButton.onClick.AddListener(Confirm);
		CancelButton.onClick.AddListener(Cancel);
	}

	private void Cancel()
	{
		_onCancel?.Invoke();
		base.gameObject.SetActive(value: false);
		DefaultEngineMarketWindow.OnCancelPopup();
		if (_virtualPurchaseAnalytic != null && DefaultEngineMarketWindow.SelectedEngineItem != null)
		{
			_virtualPurchaseAnalytic.LogPurchaseVirtual_Engine(DefaultEngineMarketWindow.SelectedEngineItem.EngineId.ToString(), DefaultEngineMarketWindow.SelectedEngineItem.GetPrice, PurchaseStatus.PURCHASE_CANCELED_BY_GAME);
			_virtualPurchaseAnalytic = null;
		}
	}

	private void Confirm()
	{
		_onConfirm?.Invoke();
		base.gameObject.SetActive(value: false);
	}

	public void BuyEngine(EngineKitOffer offer, Action callback, string price, VirtualPurchaseAnalytic virtualPurchaseAnalytic, bool isSell = false)
	{
		BuyButtonText.text = new LocalizedString("BuyThisCarWindow.Buy");
		UIHelper.SizeFitterMaxWidth(BuyButtonSizeFitter, 300f).Forget();
		TitleText.text = new LocalizedString("MarketInventoryPopup.Confirm");
		BuyDetailNameText.text = offer.GetDisplayName();
		ShowPopupWithOneItem(callback, isFromInventory: false);
		EnginePriceText.transform.parent.gameObject.SetActive(value: true);
		EnginePriceText.text = price;
		Rebuild().Forget();
		_offer = offer;
		_virtualPurchaseAnalytic = virtualPurchaseAnalytic;
	}

	public void SellEngine(EngineKitData engine, Action callback, string price)
	{
		BuyButtonText.text = new LocalizedString("AllInventoryPopup.Sell");
		UIHelper.SizeFitterMaxWidth(BuyButtonSizeFitter, 300f).Forget();
		TitleText.text = new LocalizedString("MarketInventoryPopup.Confirm");
		BuyDetailNameText.text = engine.EngineId.GetName();
		ShowPopupWithOneItem(callback, isFromInventory: true);
		EnginePriceText.transform.parent.gameObject.SetActive(value: true);
		EnginePriceText.text = price;
		Rebuild().Forget();
	}

	public void SetEngine(EngineKitData installedDetail, EngineKitData buyDetail, Action callback)
	{
		bool num = installedDetail != null && installedDetail.EngineId.InstanceID != -1;
		BuyAndSetDetailText.text = buyDetail.EngineId.GetName();
		if (num)
		{
			InstalledDetailNameText.text = installedDetail.EngineId.GetName();
		}
		else
		{
			ShowPopupWithOneItem(callback, isFromInventory: true);
		}
		ShowPopupWithTwoItems(callback, isFromInventory: true);
		EnginePriceText.transform.parent.gameObject.SetActive(value: false);
	}

	private void ShowPopupWithOneItem(Action callback, bool isFromInventory)
	{
		DefaultEngineMarketWindow.HideMainPanel();
		base.gameObject.SetActive(value: true);
		_onConfirm = callback;
		base.gameObject.SetActive(value: true);
		BuyAndSetPanel.SetActive(value: false);
		BuyPanel.SetActive(value: true);
	}

	private void ShowPopupWithTwoItems(Action callback, bool isFromInventory)
	{
		TitleText.text = new LocalizedString("MarketInventoryPopup.ConfirmChanges");
		DefaultEngineMarketWindow.HideMainPanel();
		base.gameObject.SetActive(value: true);
		_onConfirm = callback;
		base.gameObject.SetActive(value: true);
		BuyAndSetPanel.SetActive(value: true);
		BuyPanel.SetActive(value: false);
		BuyButtonText.text = (isFromInventory ? new LocalizedString("AllInventoryPopup.Set") : new LocalizedString("MarketInventoryPopup.BuySet"));
		UIHelper.SizeFitterMaxWidth(BuyButtonSizeFitter, 300f).Forget();
		MoneyIcon.SetActive(!isFromInventory);
		UIHelper.DelayedRebuildLayout(BuyButtonText.transform.parent.GetComponent<RectTransform>()).Forget();
	}

	public void BuyAndSetEngine(EngineKitData installedEngine, EngineKitOffer buyOffer, Action callback, string price, VirtualPurchaseAnalytic virtualPurchaseAnalytic)
	{
		BuyAndSetDetailText.text = buyOffer.GetDisplayName();
		if (installedEngine != null)
		{
			InstalledDetailNameText.text = installedEngine.EngineId.GetName();
			ShowPopupWithTwoItems(callback, isFromInventory: false);
		}
		else
		{
			ShowPopupWithOneItem(callback, isFromInventory: false);
		}
		EnginePriceText.transform.parent.gameObject.SetActive(value: true);
		EnginePriceText.text = price;
		Rebuild().Forget();
		_virtualPurchaseAnalytic = virtualPurchaseAnalytic;
	}

	private async UniTaskVoid Rebuild()
	{
		await UniTask.Yield();
		LayoutRebuilder.ForceRebuildLayoutImmediate(EnginePriceText.transform.parent.GetComponent<RectTransform>());
	}
}

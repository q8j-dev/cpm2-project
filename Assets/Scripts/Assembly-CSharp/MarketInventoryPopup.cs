using System;
using CodeStage.AntiCheat.ObscuredTypes;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.Analytics.Analyzers;
using _Car_Parking.Scripts.Analytics.Types;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.EngineTuning;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.UIElement;

public class MarketInventoryPopup : MonoBehaviour
{
	[SerializeField]
	private GameObject BuyPanel;

	[SerializeField]
	private GameObject BuyAndSetPanel;

	[SerializeField]
	private Image BuyDetailImage;

	[SerializeField]
	private Image BuyDetailVendorImage;

	[SerializeField]
	private Image BuyDetailImagePanel;

	[SerializeField]
	private Image InstalledDetailImage;

	[SerializeField]
	private Image InstalledDetailVendorImage;

	[SerializeField]
	private Image InstalledDetailImagePanel;

	[SerializeField]
	private Image BuyAndSellDetailImage;

	[SerializeField]
	private Image BuyAndSellDetailVendorImage;

	[SerializeField]
	private Image BuyAndSellDetailImagePanel;

	[SerializeField]
	private Button BuyButton;

	[SerializeField]
	private Button CancelButton;

	[SerializeField]
	private ContentSizeFitter BuyButtonSizeFitter;

	[SerializeField]
	private TextMeshProUGUI BuyButtonText;

	[SerializeField]
	private TextMeshProUGUI TitleText;

	[SerializeField]
	private TextMeshProUGUI InstalledDetailNameText;

	[SerializeField]
	private TextMeshProUGUI BuyDetailNameText;

	[SerializeField]
	private TextMeshProUGUI BuyAndSetDetailText;

	[SerializeField]
	private TextMeshProUGUI PriceText;

	[SerializeField]
	private MarketInventoryWindow MarketInventoryWindow;

	[SerializeField]
	private GameObject MoneyIcon;

	[SerializeField]
	private GameObject InstalledDetailPanel;

	private ManagerUI _managerUI;

	private Action _onConfirm;

	private Action _onCancel;

	private CarTuningDetailRaritySpritesConfig _carTuningDetailRaritySpritesConfig;

	private DetailVendorsConfigs _detailVendorsConfigs;

	private VirtualPurchaseAnalytic _virtualPurchaseAnalytic;

	private PartItem _partItem;

	[Inject]
	public void Construct(ManagerUI managerUI, CarTuningDetailRaritySpritesConfig carTuningDetailRaritySpritesConfig, DetailVendorsConfigs detailVendorsConfigs, VirtualPurchaseAnalytic virtualPurchaseAnalytic)
	{
		_managerUI = managerUI;
		_carTuningDetailRaritySpritesConfig = carTuningDetailRaritySpritesConfig;
		_detailVendorsConfigs = detailVendorsConfigs;
		_virtualPurchaseAnalytic = virtualPurchaseAnalytic;
	}

	private void Start()
	{
		BuyButton.onClick.AddListener(Confirm);
		CancelButton.onClick.AddListener(Cancel);
	}

	private void Cancel()
	{
		_onCancel?.Invoke();
		base.gameObject.SetActive(value: false);
		MarketInventoryWindow.OnCancelPopup();
		if ((bool)_partItem)
		{
			_virtualPurchaseAnalytic.LogPurchaseVirtual_InventoryItem(_partItem.GetName, _partItem.GetPrice, _partItem.TypeId.ToString(), PurchaseStatus.PURCHASE_CLICKED);
		}
		_partItem = null;
	}

	private void Confirm()
	{
		_onConfirm?.Invoke();
	}

	public void ShowPopup(PartData partData, PartItem part, Action callback, string btnText)
	{
		BuyButtonText.text = btnText;
		UIHelper.SizeFitterMaxWidth(BuyButtonSizeFitter, 300f).Forget();
		TitleText.text = new LocalizedString("MarketInventoryPopup.Confirm");
		MarketInventoryWindow.HideMainPanel();
		base.gameObject.SetActive(value: true);
		_onConfirm = callback;
		base.gameObject.SetActive(value: true);
		BuyAndSetPanel.SetActive(value: false);
		BuyPanel.SetActive(value: true);
		BuyDetailImage.sprite = _managerUI.GetEngineIcon((int)part.TypeId);
		BuyDetailVendorImage.sprite = _detailVendorsConfigs.Vendors[part.VendorId].CarTuningWindowSprite;
		BuyDetailImagePanel.sprite = _carTuningDetailRaritySpritesConfig.CarTuningDetailRarityDatas[part.Rarity].SquareRaritySpriteForInventory;
		BuyDetailNameText.text = partData.GetName();
		UpdatePriceText(partData, part, isTrading: true);
		Rebuild();
		_partItem = part;
	}

	public void ShowPopup(PartData installedDetail, PartItem installedPartItem, PartData buyDetail, PartItem buyPartItem, Action callback, bool isFromInventory = false)
	{
		bool flag = installedDetail.InstanceID != -1 && installedDetail.ModelId != 0;
		InstalledDetailPanel.gameObject.SetActive(flag);
		if (flag)
		{
			InstalledDetailImage.sprite = _managerUI.GetEngineIcon((int)installedPartItem.TypeId);
			InstalledDetailVendorImage.sprite = _detailVendorsConfigs.Vendors[installedPartItem.VendorId].CarTuningWindowSprite;
			InstalledDetailImagePanel.sprite = _carTuningDetailRaritySpritesConfig.CarTuningDetailRarityDatas[installedPartItem.Rarity].SquareRaritySpriteForInventory;
			InstalledDetailNameText.text = installedDetail.GetName();
		}
		TitleText.text = new LocalizedString("MarketInventoryPopup.ConfirmChanges");
		MarketInventoryWindow.HideMainPanel();
		base.gameObject.SetActive(value: true);
		_onConfirm = callback;
		base.gameObject.SetActive(value: true);
		BuyAndSetPanel.SetActive(value: true);
		BuyPanel.SetActive(value: false);
		BuyButtonText.text = (isFromInventory ? new LocalizedString("AllInventoryPopup.Set") : new LocalizedString("MarketInventoryPopup.BuySet"));
		UIHelper.SizeFitterMaxWidth(BuyButtonSizeFitter, 300f).Forget();
		MoneyIcon.SetActive(!isFromInventory);
		UpdatePriceText(buyDetail, buyPartItem, isTrading: false);
		BuyAndSellDetailImage.sprite = _managerUI.GetEngineIcon((int)buyPartItem.TypeId);
		BuyAndSellDetailVendorImage.sprite = _detailVendorsConfigs.Vendors[buyPartItem.VendorId].CarTuningWindowSprite;
		BuyAndSellDetailImagePanel.sprite = _carTuningDetailRaritySpritesConfig.CarTuningDetailRarityDatas[buyPartItem.Rarity].SquareRaritySpriteForInventory;
		BuyAndSetDetailText.text = buyDetail.GetName();
		Rebuild();
	}

	private void UpdatePriceText(PartData partData, PartItem partItem, bool isTrading)
	{
		if (isTrading)
		{
			PriceText.transform.parent.gameObject.SetActive(value: true);
			int num = partItem.GetPrice;
			ObscuredInt obscuredInt = Mathf.RoundToInt(partData.GetCondition() * (float)num * 0.2f);
			obscuredInt = Mathf.Clamp(obscuredInt, 1, 2000);
			PriceText.text = ((MarketInventoryWindow.openPanel == MarketInventoryWindow.OpenPanelType.Inventory) ? $"{obscuredInt:d0}" : $"{partItem.GetPrice:d0}");
		}
		else if (MarketInventoryWindow.openPanel == MarketInventoryWindow.OpenPanelType.Inventory)
		{
			PriceText.transform.parent.gameObject.SetActive(value: false);
		}
		else
		{
			PriceText.transform.parent.gameObject.SetActive(value: true);
			PriceText.text = $"{partItem.GetPrice:d0}";
		}
	}

	private void Rebuild()
	{
		UIHelper.DelayedRebuildLayout(BuyButtonText.transform.parent.GetComponent<RectTransform>()).Forget();
		LayoutRebuilder.ForceRebuildLayoutImmediate(PriceText.transform.parent.GetComponent<RectTransform>());
		BuyButtonText.transform.parent.gameObject.SetActive(value: true);
	}
}

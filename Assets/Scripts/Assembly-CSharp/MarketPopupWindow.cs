using System;
using CPMEngine;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.CustomDataTypes;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.EngineTuning;
using _Car_Parking.Scripts.Localization;

public class MarketPopupWindow : WindowScript
{
	private PartItem _partItem;

	[SerializeField]
	private TextMeshProUGUI detailNameText = default;

	[SerializeField]
	private TextMeshProUGUI milliageText = default;

	[SerializeField]
	private TextMeshProUGUI massText = default;

	[SerializeField]
	private TextMeshProUGUI priceText = default;

	[SerializeField]
	private TextMeshProUGUI descriptionText = default;

	[SerializeField]
	private Button confirmButton = default;

	[SerializeField]
	private Button closeButton = default;

	[SerializeField]
	private Button hiddenCloseButton = default;

	private AllParts _allParts;

	private EngineItem _engine;

	private IMenuWindowsMediator _menuWindowsMediator;

	private Action _onBuyConfirmed;

	private CurrencyManagerController _currencyManagerController;

	[Inject]
	public void Construct(AllParts allParts, IMenuWindowsMediator menuWindowsMediator, CurrencyManagerController currencyManagerController)
	{
		_currencyManagerController = currencyManagerController;
		_menuWindowsMediator = menuWindowsMediator;
		_allParts = allParts;
	}

	protected override void Awake()
	{
		base.Awake();
		if ((bool)closeButton)
		{
			closeButton.onClick.AddListener(Close);
		}
		if ((bool)hiddenCloseButton)
		{
			hiddenCloseButton.onClick.AddListener(Close);
		}
		if ((bool)confirmButton)
		{
			confirmButton.onClick.AddListener(Confirm);
		}
	}

	public void InitPart(PartData partData, Action onBuyConfirmed)
	{
		_onBuyConfirmed = onBuyConfirmed;
		_partItem = _allParts.GetPart<PartItem>(partData);
		_ = ParseData();
	}

	public void InitEngine(EngineItem engine, Action onBuyConfirmed)
	{
		_onBuyConfirmed = onBuyConfirmed;
		_engine = engine;
		ParseData(isEngine: true).Forget();
	}

	private void Confirm()
	{
		int num = ((_partItem != null) ? ((int)_partItem.GetPrice) : ((int)_engine.GetPrice));
		_currencyManagerController.AddMoney(new AdvancedInt(num));
		Close();
		_onBuyConfirmed?.Invoke();
	}

	private async UniTaskVoid ParseData(bool isEngine = false)
	{
		if ((bool)detailNameText)
		{
			detailNameText.text = (isEngine ? _engine.name : _partItem.GetName);
		}
		if ((bool)milliageText)
		{
			milliageText.text = (isEngine ? $"{Utils.GetConvertedMileage(_engine.MaxMileage):n0} {Utils.CurrentMetric}" : $"{Utils.GetConvertedMileage(_partItem.MaxMileage):n0} {Utils.CurrentMetric}");
		}
		if ((bool)massText)
		{
			massText.text = (isEngine ? (Utils.GetMass(_engine.Mass) ?? "") : (Utils.GetMass(_partItem.Mass) ?? ""));
		}
		if ((bool)priceText)
		{
			priceText.text = (isEngine ? $"{_engine.GetPrice:n0}" : $"{_partItem.GetPrice:n0}");
		}
		if ((bool)descriptionText)
		{
			descriptionText.text = (isEngine ? new LocalizedString(_engine.Description) : new LocalizedString(_partItem.Description));
		}
	}
}

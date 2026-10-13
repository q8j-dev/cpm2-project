using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.Drag;

public class MarketInventoryFilterWindow : WindowScript
{
	[SerializeField]
	private Toggle AllToggle;

	[SerializeField]
	private Toggle CompatibleToggle;

	[SerializeField]
	private Toggle NotCompatibleToggle;

	[SerializeField]
	private Button ApplyButton;

	[SerializeField]
	private Button BackBackgroundButton;

	private WindowScript _inventoryWindow;

	private MarketInventoryFilters _currentMarketInventoryFilter;

	[Inject]
	public void Construct(ManagerUI managerUI)
	{
		_managerUI = managerUI;
	}

	protected override void Start()
	{
		base.Start();
		_inventoryWindow = _managerUI.GetWindowsParent().GetComponentInChildren<WindowScript>();
		AllToggle.onValueChanged.AddListener(SelectAll);
		CompatibleToggle.onValueChanged.AddListener(SetCompatible);
		NotCompatibleToggle.onValueChanged.AddListener(SetNotCompatible);
		ApplyButton.onClick.AddListener(Apply);
		BackBackgroundButton.onClick.AddListener(Close);
		AllToggle.SetIsOnWithoutNotify(_managerUI.CurrentInventoryElement == MarketInventoryFilters.All);
		CompatibleToggle.SetIsOnWithoutNotify(_managerUI.CurrentInventoryElement == MarketInventoryFilters.Compatible);
		NotCompatibleToggle.SetIsOnWithoutNotify(_managerUI.CurrentInventoryElement == MarketInventoryFilters.InCompatible);
	}

	public void SelectAll(bool state)
	{
		if (state)
		{
			_currentMarketInventoryFilter = MarketInventoryFilters.All;
		}
	}

	public void SetCompatible(bool state)
	{
		if (state)
		{
			_currentMarketInventoryFilter = MarketInventoryFilters.Compatible;
		}
	}

	public void SetNotCompatible(bool state)
	{
		if (state)
		{
			_currentMarketInventoryFilter = MarketInventoryFilters.InCompatible;
		}
	}

	public void Apply()
	{
		_managerUI.CurrentInventoryElement = _currentMarketInventoryFilter;
		_inventoryWindow.RebuildWindow();
		Close();
	}

	public override void Close()
	{
		if (!base.IsCloseCanceled)
		{
			base.Close();
			_inventoryWindow.RebuildWindow();
		}
	}
}

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using _Car_Parking.Scripts.Drag;

public class AllInventoryFilterWindow : WindowScript, IBackableWindow
{
	[SerializeField]
	private Toggle AllInstalledToggle;

	[SerializeField]
	private Toggle InstalledPartToggle;

	[SerializeField]
	private Toggle NotInstalledPartToggle;

	[SerializeField]
	private FilterElementToggle[] FilterElementToggles;

	[SerializeField]
	private Toggle AllToggle;

	[SerializeField]
	private Button ApplyButton;

	[SerializeField]
	private Button CloseButton;

	[SerializeField]
	private Button BackBackgroundButton;

	private HashSet<AllInventoryFilters> _currentSelectedFilter = new HashSet<AllInventoryFilters>();

	private AllInventoryInstalledFilter _currentAllInventoryInstalledElement;

	private WindowScript _inventoryWindow;

	public void InitBackButton()
	{
		_managerUI.SetCurrentWindow(this);
	}

	public void Back()
	{
		Close();
	}

	protected override void Start()
	{
		base.Start();
		for (int i = 0; i < FilterElementToggles.Length; i++)
		{
			FilterElementToggles[i].Init(this);
		}
		InstalledPartToggle.onValueChanged.AddListener(SetInstalled);
		NotInstalledPartToggle.onValueChanged.AddListener(SetNotInstalled);
		AllInstalledToggle.onValueChanged.AddListener(SetAllInstalled);
		_currentSelectedFilter = _managerUI.CurrentAllInventoryFilters;
		AllToggle.onValueChanged.AddListener(SelectAll);
		_inventoryWindow = _managerUI.GetWindowsParent().GetComponentInChildren<WindowScript>();
		ApplyButton.onClick.AddListener(Apply);
		CloseButton.onClick.AddListener(Close);
		BackBackgroundButton.onClick.AddListener(Close);
		FillData();
		InitBackButton();
	}

	public override void Close()
	{
		if (!base.IsCloseCanceled)
		{
			_inventoryWindow.RebuildWindow();
			base.Close();
		}
	}

	public void SetInstalled(bool state)
	{
		if (state)
		{
			_currentAllInventoryInstalledElement = AllInventoryInstalledFilter.Installed;
		}
	}

	public void SetNotInstalled(bool state)
	{
		if (state)
		{
			_currentAllInventoryInstalledElement = AllInventoryInstalledFilter.NotInstalled;
		}
	}

	public void SetAllInstalled(bool state)
	{
		if (state)
		{
			_currentAllInventoryInstalledElement = AllInventoryInstalledFilter.All;
		}
	}

	private void FillData()
	{
		bool flag = _managerUI.CurrentAllInventoryFilters.Contains(AllInventoryFilters.All);
		FillInstalledFilter();
		foreach (AllInventoryFilters currentAllInventoryFilter in _managerUI.CurrentAllInventoryFilters)
		{
			for (int i = 0; i < FilterElementToggles.Length; i++)
			{
				if ((FilterElementToggles[i].CurrentAllInventoryFilter == currentAllInventoryFilter) | flag)
				{
					FilterElementToggles[i].ChangeState(state: true);
				}
			}
		}
		AllToggle.SetIsOnWithoutNotify(flag);
	}

	private void FillInstalledFilter()
	{
		AllInstalledToggle.SetIsOnWithoutNotify(_managerUI.CurrentAllInventoryInstalledElement == AllInventoryInstalledFilter.All);
		InstalledPartToggle.SetIsOnWithoutNotify(_managerUI.CurrentAllInventoryInstalledElement == AllInventoryInstalledFilter.Installed);
		NotInstalledPartToggle.SetIsOnWithoutNotify(_managerUI.CurrentAllInventoryInstalledElement == AllInventoryInstalledFilter.NotInstalled);
	}

	public void SelectAll(bool state)
	{
		for (int i = 0; i < FilterElementToggles.Length; i++)
		{
			FilterElementToggles[i].ChangeState(state);
		}
		if (state)
		{
			_currentSelectedFilter = new HashSet<AllInventoryFilters> { AllInventoryFilters.All };
		}
	}

	public void SelectFilterElement(bool state)
	{
		bool flag = CheckAllStatus();
		_currentSelectedFilter = GetCurrentSelectFilter();
		AllToggle.SetIsOnWithoutNotify(flag);
		if (flag)
		{
			SelectAll(state: true);
		}
		else if (_currentSelectedFilter.Contains(AllInventoryFilters.All))
		{
			_currentSelectedFilter.Remove(AllInventoryFilters.All);
		}
	}

	private HashSet<AllInventoryFilters> GetCurrentSelectFilter()
	{
		HashSet<AllInventoryFilters> hashSet = new HashSet<AllInventoryFilters>();
		for (int i = 0; i < FilterElementToggles.Length; i++)
		{
			if (FilterElementToggles[i].Status)
			{
				hashSet.Add(FilterElementToggles[i].CurrentAllInventoryFilter);
			}
		}
		return hashSet;
	}

	private bool CheckAllStatus()
	{
		bool result = false;
		for (int i = 0; i < FilterElementToggles.Length; i++)
		{
			if (!FilterElementToggles[i].Status)
			{
				return false;
			}
			result = FilterElementToggles[i].Status;
		}
		return result;
	}

	public void Apply()
	{
		if (_currentSelectedFilter.Count < 1)
		{
			_managerUI.CurrentAllInventoryFilters = _managerUI.CurrentAllInventoryFilters;
		}
		else
		{
			_managerUI.CurrentAllInventoryFilters = _currentSelectedFilter;
		}
		_managerUI.CurrentAllInventoryInstalledElement = _currentAllInventoryInstalledElement;
		_inventoryWindow.RebuildWindow();
		Close();
	}
}

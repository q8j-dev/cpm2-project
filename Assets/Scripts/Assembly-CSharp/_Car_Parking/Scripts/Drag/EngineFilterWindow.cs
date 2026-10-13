using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace _Car_Parking.Scripts.Drag
{
	public class EngineFilterWindow : WindowScript, IBackableWindow
	{
		[SerializeField]
		private Toggle AllInstalledToggle;

		[SerializeField]
		private Toggle CompatiblePartToggle;

		[SerializeField]
		private Toggle NotCompatiblePartToggle;

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

		private HashSet<EngineFilterTypeFilter> _currentEngineFilterTypeFilters = new HashSet<EngineFilterTypeFilter>();

		private MarketInventoryFilters _compatibleFilter;

		private WindowScript _inventoryWindow;

		protected override void Start()
		{
			base.Start();
			for (int i = 0; i < FilterElementToggles.Length; i++)
			{
				FilterElementToggles[i].Init(this);
			}
			CompatiblePartToggle.onValueChanged.AddListener(SetCompatible);
			NotCompatiblePartToggle.onValueChanged.AddListener(SetNotCompatible);
			AllInstalledToggle.onValueChanged.AddListener(SelectAllCompatibele);
			_compatibleFilter = _managerUI.CurrentInventoryElement;
			AllToggle.onValueChanged.AddListener(SelectAll);
			_inventoryWindow = _managerUI.GetWindowsParent().GetComponentInChildren<WindowScript>();
			ApplyButton.onClick.AddListener(Apply);
			CloseButton.onClick.AddListener(Close);
			BackBackgroundButton.onClick.AddListener(Close);
			FillData();
			InitBackButton();
		}

		public void InitBackButton()
		{
			_managerUI.SetCurrentWindow(this);
		}

		public void Back()
		{
			Close();
		}

		public override void Close()
		{
			if (!base.IsCloseCanceled)
			{
				_inventoryWindow.RebuildWindow();
				base.Close();
			}
		}

		private void SelectAllCompatibele(bool state)
		{
			if (state)
			{
				_compatibleFilter = MarketInventoryFilters.All;
			}
		}

		private void SetCompatible(bool state)
		{
			if (state)
			{
				_compatibleFilter = MarketInventoryFilters.Compatible;
			}
		}

		private void SetNotCompatible(bool state)
		{
			if (state)
			{
				_compatibleFilter = MarketInventoryFilters.InCompatible;
			}
		}

		private void FillData()
		{
			bool flag = _managerUI.CurrentEngineFilterTypeFilters.Contains(EngineFilterTypeFilter.All);
			FillInstalledFilter();
			foreach (EngineFilterTypeFilter currentEngineFilterTypeFilter in _managerUI.CurrentEngineFilterTypeFilters)
			{
				for (int i = 0; i < FilterElementToggles.Length; i++)
				{
					if ((FilterElementToggles[i].CurrentEngineTypeFilter == currentEngineFilterTypeFilter) | flag)
					{
						FilterElementToggles[i].ChangeState(state: true);
					}
				}
			}
			AllToggle.SetIsOnWithoutNotify(flag);
		}

		private void FillInstalledFilter()
		{
			AllToggle.SetIsOnWithoutNotify(_managerUI.CurrentInventoryElement == MarketInventoryFilters.All);
			CompatiblePartToggle.SetIsOnWithoutNotify(_managerUI.CurrentInventoryElement == MarketInventoryFilters.Compatible);
			NotCompatiblePartToggle.SetIsOnWithoutNotify(_managerUI.CurrentInventoryElement == MarketInventoryFilters.InCompatible);
		}

		public void SelectAll(bool state)
		{
			for (int i = 0; i < FilterElementToggles.Length; i++)
			{
				FilterElementToggles[i].ChangeState(state);
			}
			if (state)
			{
				_currentEngineFilterTypeFilters = new HashSet<EngineFilterTypeFilter> { EngineFilterTypeFilter.All };
			}
		}

		public void SelectFilterElement(bool state)
		{
			bool flag = CheckAllStatus();
			_currentEngineFilterTypeFilters = GetCurrentSelectFilter();
			AllToggle.SetIsOnWithoutNotify(flag);
			if (flag)
			{
				SelectAll(state: true);
			}
			else if (_currentEngineFilterTypeFilters.Contains(EngineFilterTypeFilter.All))
			{
				_currentEngineFilterTypeFilters.Remove(EngineFilterTypeFilter.All);
			}
		}

		private HashSet<EngineFilterTypeFilter> GetCurrentSelectFilter()
		{
			HashSet<EngineFilterTypeFilter> hashSet = new HashSet<EngineFilterTypeFilter>();
			for (int i = 0; i < FilterElementToggles.Length; i++)
			{
				if (FilterElementToggles[i].Status)
				{
					hashSet.Add(FilterElementToggles[i].CurrentEngineTypeFilter);
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
			if (_currentEngineFilterTypeFilters.Count < 1)
			{
				_managerUI.CurrentEngineFilterTypeFilters = _managerUI.CurrentEngineFilterTypeFilters;
			}
			else
			{
				_managerUI.CurrentEngineFilterTypeFilters = _currentEngineFilterTypeFilters;
			}
			_managerUI.CurrentInventoryElement = _compatibleFilter;
			_inventoryWindow.RebuildWindow();
			Close();
		}
	}
}

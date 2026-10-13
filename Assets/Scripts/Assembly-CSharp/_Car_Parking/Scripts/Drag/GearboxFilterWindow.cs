using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace _Car_Parking.Scripts.Drag
{
	public class GearboxFilterWindow : WindowScript, IBackableWindow
	{
		[SerializeField]
		private Toggle AllGearboxTypeToggle = default;

		[SerializeField]
		private Toggle AllTransmissionToggle = default;

		[SerializeField]
		private FilterElementToggle[] GearboxTypeFilters = default;

		[SerializeField]
		private FilterElementToggle[] GearboxTransmissionFilters = default;

		[SerializeField]
		private Button ApplyButton = default;

		[SerializeField]
		private Button CloseButton = default;

		[SerializeField]
		private Button BackBackgroundButton = default;

		private HashSet<GearBoxTransmissionTypeFilter> _gearBoxTransMissionTypes = new HashSet<GearBoxTransmissionTypeFilter>();

		private HashSet<GearBoxTypeFilters> _gearBoxTypeFilters = new HashSet<GearBoxTypeFilters>();

		private WindowScript _inventoryWindow;

		protected override void Start()
		{
			base.Start();
			InitToggles();
			_inventoryWindow = _managerUI.GetWindowsParent().GetComponentInChildren<WindowScript>();
			FillData();
			InitButtons();
			InitBackButton();
		}

		private void InitButtons()
		{
			ApplyButton.onClick.AddListener(Apply);
			CloseButton.onClick.AddListener(Close);
			BackBackgroundButton.onClick.AddListener(Close);
		}

		public void Back()
		{
			Close();
		}

		public void InitBackButton()
		{
			_managerUI.SetCurrentWindow(this);
		}

		private void InitToggles()
		{
			AllGearboxTypeToggle.onValueChanged.AddListener(SetAllTypes);
			AllTransmissionToggle.onValueChanged.AddListener(SetAllTransmissionSet);
			for (int i = 0; i < GearboxTypeFilters.Length; i++)
			{
				GearboxTypeFilters[i].Init(this, isGearboxTypeToggle: true);
			}
			for (int j = 0; j < GearboxTransmissionFilters.Length; j++)
			{
				GearboxTransmissionFilters[j].Init(this, isGearboxTypeToggle: false);
			}
		}

		private void SetAllTransmissionSet(bool state)
		{
			SelectAllType(isType: false, state);
		}

		private void SetAllTypes(bool state)
		{
			SelectAllType(isType: true, state);
		}

		private void FillData()
		{
			bool flag = _managerUI.CurrentGearBoxTypeFilters.Contains(GearBoxTypeFilters.All);
			foreach (GearBoxTypeFilters currentGearBoxTypeFilter in _managerUI.CurrentGearBoxTypeFilters)
			{
				for (int i = 0; i < GearboxTypeFilters.Length; i++)
				{
					if ((GearboxTypeFilters[i].CurrentGearBoxTypeFilter == currentGearBoxTypeFilter) | flag)
					{
						GearboxTypeFilters[i].ChangeState(state: true);
					}
				}
			}
			AllGearboxTypeToggle.SetIsOnWithoutNotify(flag);
			flag = _managerUI.CurrentGearBoxTransMissionTypes.Contains(GearBoxTransmissionTypeFilter.All);
			foreach (GearBoxTransmissionTypeFilter currentGearBoxTransMissionType in _managerUI.CurrentGearBoxTransMissionTypes)
			{
				for (int j = 0; j < GearboxTransmissionFilters.Length; j++)
				{
					if ((GearboxTransmissionFilters[j].currenGearBoxTransmissionTypeFilter == currentGearBoxTransMissionType) | flag)
					{
						GearboxTransmissionFilters[j].ChangeState(state: true);
					}
				}
			}
			AllTransmissionToggle.SetIsOnWithoutNotify(flag);
		}

		public void SelectAllType(bool isType, bool state)
		{
			FilterElementToggle[] array = (isType ? GearboxTypeFilters : GearboxTransmissionFilters);
			for (int i = 0; i < array.Length; i++)
			{
				array[i].ChangeState(state);
			}
			if (state)
			{
				if (isType)
				{
					_gearBoxTypeFilters = new HashSet<GearBoxTypeFilters> { GearBoxTypeFilters.All };
				}
				else
				{
					_gearBoxTransMissionTypes = new HashSet<GearBoxTransmissionTypeFilter> { GearBoxTransmissionTypeFilter.All };
				}
			}
		}

		public void SelectTypeFilterElement(bool state)
		{
			bool flag = CheckAllStatus(isType: true);
			_gearBoxTypeFilters = GetCurrentSelectedTypeFilter();
			AllGearboxTypeToggle.SetIsOnWithoutNotify(flag);
			if (flag)
			{
				SelectAllType(isType: true, state);
			}
			else if (_gearBoxTypeFilters.Contains(GearBoxTypeFilters.All))
			{
				_gearBoxTypeFilters.Remove(GearBoxTypeFilters.All);
			}
		}

		public void SelectTransmissionTypeElement(bool state)
		{
			bool flag = CheckAllStatus(isType: false);
			_gearBoxTransMissionTypes = GetCurrentSelectedTransmissionTypeFilter();
			AllTransmissionToggle.SetIsOnWithoutNotify(flag);
			if (flag)
			{
				SelectAllType(isType: false, state);
			}
			else if (_gearBoxTransMissionTypes.Contains(GearBoxTransmissionTypeFilter.All))
			{
				_gearBoxTransMissionTypes.Remove(GearBoxTransmissionTypeFilter.All);
			}
		}

		private HashSet<GearBoxTypeFilters> GetCurrentSelectedTypeFilter()
		{
			HashSet<GearBoxTypeFilters> hashSet = new HashSet<GearBoxTypeFilters>();
			for (int i = 0; i < GearboxTypeFilters.Length; i++)
			{
				if (GearboxTypeFilters[i].Status)
				{
					hashSet.Add(GearboxTypeFilters[i].CurrentGearBoxTypeFilter);
				}
			}
			return hashSet;
		}

		private HashSet<GearBoxTransmissionTypeFilter> GetCurrentSelectedTransmissionTypeFilter()
		{
			HashSet<GearBoxTransmissionTypeFilter> hashSet = new HashSet<GearBoxTransmissionTypeFilter>();
			for (int i = 0; i < GearboxTransmissionFilters.Length; i++)
			{
				if (GearboxTransmissionFilters[i].Status)
				{
					hashSet.Add(GearboxTransmissionFilters[i].currenGearBoxTransmissionTypeFilter);
				}
			}
			return hashSet;
		}

		private bool CheckAllStatus(bool isType)
		{
			bool result = false;
			FilterElementToggle[] array = (isType ? GearboxTypeFilters : GearboxTransmissionFilters);
			for (int i = 0; i < array.Length; i++)
			{
				if (!array[i].Status)
				{
					return false;
				}
				result = array[i].Status;
			}
			return result;
		}

		public void Apply()
		{
			if (_gearBoxTypeFilters.Count < 1)
			{
				_managerUI.CurrentGearBoxTypeFilters = _managerUI.CurrentGearBoxTypeFilters;
			}
			else
			{
				_managerUI.CurrentGearBoxTypeFilters = _gearBoxTypeFilters;
			}
			if (_gearBoxTransMissionTypes.Count < 1)
			{
				_managerUI.CurrentGearBoxTransMissionTypes = _managerUI.CurrentGearBoxTransMissionTypes;
			}
			else
			{
				_managerUI.CurrentGearBoxTransMissionTypes = _gearBoxTransMissionTypes;
			}
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
}

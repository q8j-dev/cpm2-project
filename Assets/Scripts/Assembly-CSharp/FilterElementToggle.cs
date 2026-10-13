using UnityEngine;
using UnityEngine.UI;
using _Car_Parking.Scripts.Drag;

public class FilterElementToggle : MonoBehaviour
{
	public AllInventoryFilters CurrentAllInventoryFilter;

	public GearBoxTypeFilters CurrentGearBoxTypeFilter;

	public EngineFilterTypeFilter CurrentEngineTypeFilter;

	public GearBoxTransmissionTypeFilter currenGearBoxTransmissionTypeFilter;

	[SerializeField]
	private Toggle SelectToggle = default;

	private AllInventoryFilterWindow _allInventoryFilterWindow;

	private GearboxFilterWindow _gearboxFilterWindow;

	private EngineFilterWindow _engineFilterWindow;

	public bool Status => SelectToggle.isOn;

	public void Init(AllInventoryFilterWindow allInventoryFilterWindow)
	{
		_allInventoryFilterWindow = allInventoryFilterWindow;
		SelectToggle.onValueChanged.AddListener(_allInventoryFilterWindow.SelectFilterElement);
	}

	public void Init(EngineFilterWindow engineFilterWindow)
	{
		_engineFilterWindow = engineFilterWindow;
		SelectToggle.onValueChanged.AddListener(_engineFilterWindow.SelectFilterElement);
	}

	public void Init(GearboxFilterWindow gearboxFilterWindow, bool isGearboxTypeToggle)
	{
		_gearboxFilterWindow = gearboxFilterWindow;
		if (isGearboxTypeToggle)
		{
			SelectToggle.onValueChanged.AddListener(_gearboxFilterWindow.SelectTypeFilterElement);
		}
		else
		{
			SelectToggle.onValueChanged.AddListener(_gearboxFilterWindow.SelectTransmissionTypeElement);
		}
	}

	public void ChangeState(bool state)
	{
		SelectToggle.SetIsOnWithoutNotify(state);
	}
}

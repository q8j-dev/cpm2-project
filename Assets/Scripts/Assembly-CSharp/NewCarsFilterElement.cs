using System;
using CPMEngine;
using UnityEngine;
using UnityEngine.UI;
using _Car_Parking.Scripts.UIElement;

public class NewCarsFilterElement : MonoBehaviour
{
	public DealerWindow.FilterType FilterType;

	public CarType CarType;

	public CarRegion Region;

	public VehicleClass VehicleClass;

	[SerializeField]
	private Toggle Toggle;

	[SerializeField]
	private ToggleAdditionalView ToggleAdditionalView;

	public bool IsOn
	{
		get
		{
			return Toggle.isOn;
		}
		set
		{
			Toggle.SetIsOnWithoutNotify(value);
			if ((bool)ToggleAdditionalView)
			{
				ToggleAdditionalView.UpdateView();
			}
		}
	}

	public event Action<NewCarsFilterElement, bool> OnToggleChanged;

	private void Start()
	{
		Toggle.onValueChanged.AddListener(OnToggleValueChanged);
	}

	private void OnToggleValueChanged(bool state)
	{
		OnToggleChanged?.Invoke(this, state);
	}
}

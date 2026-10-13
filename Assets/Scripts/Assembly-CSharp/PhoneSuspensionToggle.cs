using UnityEngine;
using UnityEngine.UI;
using _Car_Parking.Scripts.NewUI;

public class PhoneSuspensionToggle : MonoBehaviour
{
	[SerializeField]
	private CarControlUIPanel carControlUIPanel;

	[SerializeField]
	private PhoneCarControlUIPanelType CurrentPanelType;

	private Toggle _toggle;

	private void Start()
	{
		_toggle = GetComponent<Toggle>();
		if ((bool)_toggle)
		{
			_toggle.onValueChanged.AddListener(OnToggleValueChanged);
		}
	}

	private void OnToggleValueChanged(bool state)
	{
		if (state)
		{
			carControlUIPanel.ShowSuspensionTab(CurrentPanelType);
		}
	}
}

using UnityEngine;
using UnityEngine.UI;
using _Car_Parking.Scripts.WindowSystem;

public class SuspensionTabToggleElement : MonoBehaviour
{
	[SerializeField]
	private SuspensionTuningWindow SuspensionTuningWindow = default;

	[SerializeField]
	private Toggle TabToggle = default;

	[SerializeField]
	private SuspensionTabs CurrentSuspensionTabs = default;

	private void Start()
	{
		TabToggle.onValueChanged.AddListener(OnValueChanged);
		SuspensionTuningWindow.RegisterTabToggle(CurrentSuspensionTabs, TabToggle);
	}

	private void OnValueChanged(bool state)
	{
		if (state)
		{
			SuspensionTuningWindow.OnTabSelected(CurrentSuspensionTabs);
		}
	}
}

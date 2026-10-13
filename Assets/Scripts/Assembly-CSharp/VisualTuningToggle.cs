using UnityEngine.UI;
using _Car_Parking.Scripts.MenuUI.VisualTuning;

public class VisualTuningToggle : Toggle
{
	public VisualTuningTypes togglePanelState;

	public VisualTuningWindow _visualTuningWindow;

	protected override void Start()
	{
		onValueChanged.AddListener(OnClick);
	}

	private void OnClick(bool state)
	{
		if (state)
		{
			_visualTuningWindow.ChangePanel(togglePanelState);
		}
	}

	public void ChangeState(bool state)
	{
		base.isOn = state;
	}
}

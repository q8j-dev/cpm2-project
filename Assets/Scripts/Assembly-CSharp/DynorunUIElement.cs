using UnityEngine;
using UnityEngine.UI;

public class DynorunUIElement : MonoBehaviour
{
	[SerializeField]
	private DynoWindowPanels Panel;

	[SerializeField]
	private Toggle PanelToggle;

	[SerializeField]
	private DynoWindow DynoWindow;

	private void Start()
	{
		PanelToggle.onValueChanged.AddListener(OnToggleValueChange);
	}

	private void OnToggleValueChange(bool state)
	{
		if (state)
		{
			DynoWindow.OnChangeWindow(Panel);
		}
	}
}

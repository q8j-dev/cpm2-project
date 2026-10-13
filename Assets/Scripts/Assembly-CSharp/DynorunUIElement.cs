using UnityEngine;
using UnityEngine.UI;

public class DynorunUIElement : MonoBehaviour
{
	[SerializeField]
	private DynoWindowPanels Panel = default;

	[SerializeField]
	private Toggle PanelToggle = default;

	[SerializeField]
	private DynoWindow DynoWindow = default;

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

using UnityEngine;
using UnityEngine.EventSystems;

public class DragThresholdUtil : MonoBehaviour
{
	[SerializeField]
	private EventSystem EventSystem = default;

	private void Start()
	{
		int pixelDragThreshold = EventSystem.pixelDragThreshold;
		EventSystem.pixelDragThreshold = Mathf.Max(pixelDragThreshold, (int)((float)pixelDragThreshold * Screen.dpi / 160f));
	}
}

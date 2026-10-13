using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UGUIJoystic : MonoBehaviour, IDragHandler, IEventSystemHandler, IPointerUpHandler, IPointerDownHandler
{
	public bool isDrag;

	private Image backPanel;

	private Image knob;

	public RectTransform knobRect;

	public Vector3 InputDirection { get; set; }

	private void Start()
	{
		backPanel = GetComponent<Image>();
		knob = base.transform.GetChild(0).GetComponent<Image>();
		knobRect = knob.GetComponent<RectTransform>();
	}

	public virtual void OnDrag(PointerEventData pointerEventData)
	{
		isDrag = true;
		Vector2 localPoint = Vector2.zero;
		if (RectTransformUtility.ScreenPointToLocalPointInRectangle(backPanel.rectTransform, pointerEventData.position, pointerEventData.pressEventCamera, out localPoint))
		{
			localPoint.x /= backPanel.rectTransform.sizeDelta.x;
			localPoint.y /= backPanel.rectTransform.sizeDelta.y;
			float x = ((backPanel.rectTransform.pivot.x == 1f) ? (localPoint.x * 2f + 1f) : (localPoint.x * 2f - 1f));
			float z = ((backPanel.rectTransform.pivot.y == 1f) ? (localPoint.y * 2f + 1f) : (localPoint.y * 2f - 1f));
			InputDirection = new Vector3(x, 0f, z);
			InputDirection = ((InputDirection.magnitude > 1f) ? InputDirection.normalized : InputDirection);
			knob.rectTransform.anchoredPosition = new Vector3(InputDirection.x * (backPanel.rectTransform.sizeDelta.x / 3f), InputDirection.z * (backPanel.rectTransform.sizeDelta.y / 3f));
		}
	}

	public virtual void OnPointerDown(PointerEventData pointerEventData)
	{
		OnDrag(pointerEventData);
	}

	public virtual void OnPointerUp(PointerEventData pointerEventData)
	{
		isDrag = false;
		InputDirection = Vector3.zero;
		knob.rectTransform.anchoredPosition = Vector3.zero;
	}
}

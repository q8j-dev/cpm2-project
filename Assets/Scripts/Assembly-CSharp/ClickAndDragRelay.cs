using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(MaskableGraphic))]
public class ClickAndDragRelay : MonoBehaviour, IPointerClickHandler, IEventSystemHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
	public UnityEvent<Vector2> onClick;

	public UnityEvent<Vector2> onDragDelta;

	public UnityEvent<Vector2> onDragAbsolute;

	public float ClickThreshold = 10f;

	private float dragdistance;

	private Vector2 lastPos;

	private Vector2 startPos;

	private Vector2 shiftDrag;

	private MaskableGraphic GR;

	private bool sendDrag;

	private void Awake()
	{
		GR = GetComponent<MaskableGraphic>();
		GR.raycastTarget = true;
	}

	private void LateUpdate()
	{
		if (sendDrag)
		{
			onDragAbsolute?.Invoke(shiftDrag);
		}
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		if (dragdistance < ClickThreshold)
		{
			onClick?.Invoke(eventData.position);
		}
		dragdistance = 0f;
	}

	public void OnBeginDrag(PointerEventData eventData)
	{
		lastPos = eventData.position;
		startPos = eventData.position;
		dragdistance = 0f;
		shiftDrag = Vector2.zero;
		sendDrag = true;
	}

	public void OnDrag(PointerEventData eventData)
	{
		Vector2 arg = eventData.position - lastPos;
		Vector2 vector = eventData.position - startPos;
		shiftDrag = new Vector2(Mathf.Clamp(vector.x / GR.rectTransform.sizeDelta.x, -1f, 1f), Mathf.Clamp(vector.y / GR.rectTransform.sizeDelta.y, -1f, 1f));
		dragdistance += Vector2.Distance(eventData.position, lastPos);
		lastPos = eventData.position;
		onDragDelta?.Invoke(arg);
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		shiftDrag = Vector2.zero;
		sendDrag = false;
	}
}

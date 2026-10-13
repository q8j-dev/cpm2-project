using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class ShifterSlider : MonoBehaviour, IPointerUpHandler, IEventSystemHandler, IPointerDownHandler
{
	[SerializeField]
	private UnityEvent onPointerDown = default;

	[SerializeField]
	private UnityEvent onPointerUp = default;

	public void OnPointerDown(PointerEventData eventData)
	{
		onPointerDown?.Invoke();
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		onPointerUp?.Invoke();
	}
}

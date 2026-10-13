using AK.Wwise;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using _Car_Parking.Scripts.UIElement;

public class NewUIPlaySound : MonoBehaviour, IPointerClickHandler, IEventSystemHandler, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler, ISubmitHandler, ISelectHandler, IDeselectHandler
{
	public enum Trigger
	{
		OnClick = 0,
		OnPointerEnter = 1,
		OnPointerExit = 2,
		OnDown = 3,
		OnUp = 4
	}

	public AK.Wwise.Event uiEvent;

	public Switch wwiseSound;

	public Trigger trigger;

	[Range(0f, 1f)]
	public float volume = 1f;

	[Range(0f, 2f)]
	public float pitch = 1f;

	private bool CanPlay
	{
		get
		{
			if (!base.enabled)
			{
				return false;
			}
			if (TryGetComponent<ButtonUpDown>(out var component))
			{
				return component.enabled;
			}
			if (TryGetComponent<Button>(out var component2))
			{
				if (component2.enabled)
				{
					return component2.interactable;
				}
				return false;
			}
			if (TryGetComponent<Toggle>(out var component3))
			{
				if (component3.enabled)
				{
					return component3.interactable;
				}
				return false;
			}
			return false;
		}
	}

	public void OnPointerClick(PointerEventData data)
	{
		if (CanPlay && trigger == Trigger.OnClick)
		{
			wwiseSound?.SetValue(base.gameObject);
			uiEvent?.Post(base.gameObject);
		}
	}

	public void OnPointerDown(PointerEventData data)
	{
		if (CanPlay && trigger == Trigger.OnDown)
		{
			wwiseSound?.SetValue(base.gameObject);
			uiEvent?.Post(base.gameObject);
		}
	}

	public void OnPointerEnter(PointerEventData data)
	{
		if (CanPlay && trigger == Trigger.OnPointerEnter)
		{
			wwiseSound?.SetValue(base.gameObject);
			uiEvent?.Post(base.gameObject);
		}
	}

	public void OnPointerExit(PointerEventData data)
	{
		if (CanPlay && trigger == Trigger.OnPointerExit)
		{
			wwiseSound?.SetValue(base.gameObject);
			uiEvent?.Post(base.gameObject);
		}
	}

	public void OnPointerUp(PointerEventData data)
	{
		if (CanPlay && trigger == Trigger.OnUp)
		{
			wwiseSound?.SetValue(base.gameObject);
			uiEvent?.Post(base.gameObject);
		}
	}

	public void OnSubmit(BaseEventData eventData)
	{
		if ((CanPlay && trigger == Trigger.OnClick) || trigger == Trigger.OnDown)
		{
			wwiseSound?.SetValue(base.gameObject);
			uiEvent?.Post(base.gameObject);
		}
	}

	public void OnSelect(BaseEventData eventData)
	{
		if (CanPlay && trigger == Trigger.OnPointerEnter)
		{
			wwiseSound?.SetValue(base.gameObject);
			uiEvent?.Post(base.gameObject);
		}
	}

	public void OnDeselect(BaseEventData eventData)
	{
		if (CanPlay && trigger == Trigger.OnPointerExit)
		{
			wwiseSound?.SetValue(base.gameObject);
			uiEvent?.Post(base.gameObject);
		}
	}
}

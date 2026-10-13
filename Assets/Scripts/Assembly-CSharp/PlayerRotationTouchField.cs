using UnityEngine;
using UnityEngine.EventSystems;
using VContainer;
using _Car_Parking.Scripts.Providers;

public class PlayerRotationTouchField : MonoBehaviour, IPointerUpHandler, IEventSystemHandler, IPointerDownHandler
{
	[HideInInspector]
	public Vector2 TouchDist;

	[HideInInspector]
	protected Vector2 PointerOld;

	[HideInInspector]
	protected int PointerId;

	[HideInInspector]
	public bool Pressed;

	public bool UseFixedUpdate;

	private InputItemsProvider _inputItemsProvider;

	[Inject]
	private void Construct(InputItemsProvider inputItemsProvider)
	{
		_inputItemsProvider = inputItemsProvider;
	}

	private void Update()
	{
		if (!UseFixedUpdate)
		{
			DoUpdate();
		}
	}

	private void FixedUpdate()
	{
		if (UseFixedUpdate)
		{
			DoUpdate();
		}
	}

	private void DoUpdate()
	{
		if (Pressed)
		{
			if (PointerId >= 0 && PointerId < Input.touches.Length)
			{
				TouchDist = Input.touches[PointerId].position - PointerOld;
				PointerOld = Input.touches[PointerId].position;
			}
			else
			{
				TouchDist = new Vector2(Input.mousePosition.x, Input.mousePosition.y) - PointerOld;
				PointerOld = Input.mousePosition;
			}
		}
		else
		{
			TouchDist = default;
		}
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		Touch[] touches = Input.touches;
		for (int i = 0; i < touches.Length; i++)
		{
			Touch touch = touches[i];
			if (touch.phase == TouchPhase.Began)
			{
				Pressed = true;
				PointerOld = touch.position;
				PointerId = touch.fingerId;
			}
		}
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		Pressed = false;
	}
}

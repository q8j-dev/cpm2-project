using UnityEngine;
using UnityEngine.EventSystems;
using VContainer;
using _Car_Parking.Scripts.Providers;

public class TouchField : MonoBehaviour, IPointerUpHandler, IEventSystemHandler, IPointerDownHandler
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
	public void Construct(InputItemsProvider controlItemsProvider)
	{
		_inputItemsProvider = controlItemsProvider;
	}

	private void Awake()
	{
		_inputItemsProvider.TouchField = this;
	}

	private void OnDestroy()
	{
		_inputItemsProvider.TouchField = null;
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
		Pressed = true;
		PointerId = eventData.pointerId;
		PointerOld = eventData.position;
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		Pressed = false;
	}
}

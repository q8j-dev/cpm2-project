using System;
using CPM.Shifter.Contracts;
using CPM.Shifter.Logic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ManualHShifterInput : MonoBehaviour, IPointerDownHandler, IEventSystemHandler, IPointerUpHandler, IDragHandler, IManualHShifterInput
{
	[SerializeField]
	private float _neutralBandHalfHeight = 30f;

	private readonly ShifterInputStateMachine _sm = new ShifterInputStateMachine();

	private bool _gamepadOwnsGrab;

	public bool IsTouching => _sm.IsTouching;

	public event Action<int> OnGearRequested
	{
		add
		{
			_sm.OnGearRequested += value;
		}
		remove
		{
			_sm.OnGearRequested -= value;
		}
	}

	public event Action<int> OnPreviewGearChanged
	{
		add
		{
			_sm.OnPreviewGearChanged += value;
		}
		remove
		{
			_sm.OnPreviewGearChanged -= value;
		}
	}

	public event Action OnGrabStarted
	{
		add
		{
			_sm.OnGrabStarted += value;
		}
		remove
		{
			_sm.OnGrabStarted -= value;
		}
	}

	public event Action OnGrabReleased
	{
		add
		{
			_sm.OnGrabReleased += value;
		}
		remove
		{
			_sm.OnGrabReleased -= value;
		}
	}

	public event Action OnInputDisabled;

	private void Awake()
	{
		_sm.ClampFingerToBounds = true;
	}

	public void Bind(IManualHShifterView view, Func<float> clutchProvider)
	{
		_sm.Bind(view, clutchProvider);
	}

	public void Unbind()
	{
		_sm.Unbind();
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		_sm.PointerDown();
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		_sm.PointerUp(_neutralBandHalfHeight);
	}

	public void OnDrag(PointerEventData eventData)
	{
		_sm.Drag(eventData.delta, _neutralBandHalfHeight);
	}

	public bool BeginGamepadGrab()
	{
		if (_sm.IsTouching)
		{
			return false;
		}
		_gamepadOwnsGrab = true;
		_sm.PointerDown(gamepad: true);
		return true;
	}

	public void GamepadDrag(Vector2 delta)
	{
		if (_gamepadOwnsGrab)
		{
			_sm.Drag(delta, _neutralBandHalfHeight);
		}
	}

	public void EndGamepadGrab()
	{
		if (_gamepadOwnsGrab)
		{
			_gamepadOwnsGrab = false;
			_sm.PointerUp(_neutralBandHalfHeight);
		}
	}

	private void OnDisable()
	{
		_gamepadOwnsGrab = false;
		OnInputDisabled?.Invoke();
		_sm.Unbind();
	}
}

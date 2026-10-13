using System;
using UnityEngine;
using VContainer;
using _Car_Parking;
using _Car_Parking.Scripts;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.SOAP;
using _Car_Parking.Scripts.UIElement;

public class InputTouch : MonoBehaviour
{
	public GameObject leftJoy;

	public GameObject rightJoy;

	public Joystick leftJoy2;

	public PersonJumpButton jumpButton;

	public PersonSprintButton sprintButton;

	public PlayerRotationTouchField TouchField;

	[SerializeField]
	private Transform[] _extraWalkUi;

	private InputItemsProvider _inputItemsProvider;

	private EventRegistry _eventRegistry;

	private RegistryHandle<Vector2Variable> _movePersonHandle = new RegistryHandle<Vector2Variable>(EventRegistryIDs.MovePerson);

	private bool _gamepadActiveApplied;

	[Inject]
	public void Construct(InputItemsProvider controlItemsProvider, EventRegistry eventRegistry)
	{
		_inputItemsProvider = controlItemsProvider;
		_eventRegistry = eventRegistry;
	}

	private void OnEnable()
	{
		EnableComponent();
		V.OnOutOfCarChanged = (Action<bool>)Delegate.Remove(V.OnOutOfCarChanged, new Action<bool>(OutOfCarChangedCallback));
		V.OnOutOfCarChanged = (Action<bool>)Delegate.Combine(V.OnOutOfCarChanged, new Action<bool>(OutOfCarChangedCallback));
		V.OnPhotoModeChanged = (Action<bool>)Delegate.Remove(V.OnPhotoModeChanged, new Action<bool>(PhotoModeChangedCallback));
		V.OnPhotoModeChanged = (Action<bool>)Delegate.Combine(V.OnPhotoModeChanged, new Action<bool>(PhotoModeChangedCallback));
		ApplyOutOfCarState(FlagsStorage.OutOfCar);
	}

	private void OnDisable()
	{
		V.OnOutOfCarChanged = (Action<bool>)Delegate.Remove(V.OnOutOfCarChanged, new Action<bool>(OutOfCarChangedCallback));
		V.OnPhotoModeChanged = (Action<bool>)Delegate.Remove(V.OnPhotoModeChanged, new Action<bool>(PhotoModeChangedCallback));
		ClearWalkInput();
	}

	private void OnDestroy()
	{
		if (_inputItemsProvider != null && _inputItemsProvider.InputTouch == this)
		{
			_inputItemsProvider.InputTouch = null;
		}
		leftJoy = null;
		rightJoy = null;
		leftJoy2 = null;
		jumpButton = null;
		sprintButton = null;
		TouchField = null;
		_inputItemsProvider = null;
	}

	public void EnableComponent()
	{
		if (_inputItemsProvider != null)
		{
			_inputItemsProvider.InputTouch = this;
		}
	}

	public void ChangeComponentsState(bool state)
	{
		if ((bool)leftJoy)
		{
			leftJoy.SetActive(state);
		}
		if ((bool)rightJoy)
		{
			rightJoy.SetActive(state);
		}
	}

	private void OutOfCarChangedCallback(bool outOfCar)
	{
		ApplyOutOfCarState(outOfCar);
	}

	private void PhotoModeChangedCallback(bool isPhotoMode)
	{
		ApplyOutOfCarState(FlagsStorage.OutOfCar);
	}

	private void ApplyOutOfCarState(bool outOfCar)
	{
		_gamepadActiveApplied = InputStorage.GamepadActive;
		bool flag = outOfCar && !FlagsStorage.IsPhotoMode;
		flag = false;
		if ((bool)leftJoy)
		{
			leftJoy.SetActive(flag);
		}
		if ((bool)rightJoy)
		{
			rightJoy.SetActive(flag);
		}
		if ((bool)jumpButton)
		{
			jumpButton.gameObject.SetActive(flag);
		}
		if ((bool)sprintButton)
		{
			sprintButton.gameObject.SetActive(flag);
		}
		bool flag2 = outOfCar && !FlagsStorage.IsPhotoMode;
		if (_extraWalkUi != null)
		{
			for (int i = 0; i < _extraWalkUi.Length; i++)
			{
				if ((bool)_extraWalkUi[i])
				{
					bool flag3 = _extraWalkUi[i].GetComponentInChildren<PersonCrouchButton>(includeInactive: true);
					_extraWalkUi[i].gameObject.SetActive(!flag3 & flag2);
				}
			}
		}
		if (!outOfCar || FlagsStorage.IsPhotoMode)
		{
			ClearWalkInput();
		}
	}

	private void ClearWalkInput()
	{
		if ((bool)TouchField)
		{
			TouchField.Pressed = false;
			TouchField.TouchDist = Vector2.zero;
		}
		InputStorage.LeftJoyInputX = 0f;
		InputStorage.LeftJoyInputY = 0f;
		InputStorage.RightJoyInputX = 0f;
		InputStorage.RightJoyInputY = 0f;
		InputStorage.LeftJoyIsDrag = false;
		InputStorage.SprintLockAiming = false;
		InputStorage.SprintLockCancelRequested = true;
	}

	private void Update()
	{
		if (FlagsStorage.OutOfCar)
		{
			Vector2 value = _movePersonHandle.Get(_eventRegistry).Value;
			if ((bool)leftJoy2)
			{
				leftJoy2.isDrag = (double)value.magnitude > 0.0001;
			}
			InputStorage.LeftJoyInputX = value.x;
			InputStorage.LeftJoyInputY = value.y;
		}
	}
}

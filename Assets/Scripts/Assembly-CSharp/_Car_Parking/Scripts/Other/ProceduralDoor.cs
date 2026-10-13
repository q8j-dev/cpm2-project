using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.SOAP;
using _Car_Parking.Scripts.SOAP_Registry;

namespace _Car_Parking.Scripts.Other
{
	public abstract class ProceduralDoor : MonoBehaviour
	{
		[SerializeField]
		private bool IsGarageDoor = default;

		public Collider Front;

		public Collider Back;

		public float Duration;

		private bool _isActive;

		private FreeDriveProvider _freeDriveProvider;

		private QuickCarSettings _quickCarSettings;

		private Button _trackedButton;

		private EventRegistry _eventRegistry;

		private InputMapSetEvent _inputMapSetEvent;

		private RegistryHandle<ScriptableEvent> _interactHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.Interact);

		private bool _personInside;

		private bool _carInside;

		private bool _listening;

		private bool _wasOutOfCar = true;

		private const float PersonChestHeight = 0.9f;

		protected bool IsFront { get; private set; }

		protected Tween Tween { get; set; }

		protected bool IsOpen { get; private set; }

		private void Update()
		{
			if (_personInside || _carInside)
			{
				TrackCarTransition();
				Refresh();
			}
		}

		private void TrackCarTransition()
		{
			bool outOfCar = FlagsStorage.OutOfCar;
			if (outOfCar != _wasOutOfCar)
			{
				_wasOutOfCar = outOfCar;
				if (!outOfCar)
				{
					_personInside = false;
				}
				else if (_carInside && !_personInside)
				{
					_personInside = ContainsPerson();
				}
			}
		}

		private bool ContainsPerson()
		{
			if (!V.DriverPerson)
			{
				return false;
			}
			Vector3 point = V.DriverPerson.transform.position + Vector3.up * 0.9f;
			if (!Contains(Front, point))
			{
				return Contains(Back, point);
			}
			return true;
		}

		private static bool Contains(Collider area, Vector3 point)
		{
			if ((bool)area)
			{
				return (area.ClosestPoint(point) - point).sqrMagnitude < 0.0001f;
			}
			return false;
		}

		private bool ShouldListen()
		{
			if (!_quickCarSettings || !_inputMapSetEvent)
			{
				return false;
			}
			InputGroupsEnum currentInputGroup = _inputMapSetEvent.CurrentInputGroup;
			if (!FlagsStorage.OutOfCar)
			{
				if (IsGarageDoor && _carInside)
				{
					if (currentInputGroup != InputGroupsEnum.CarFree)
					{
						return currentInputGroup == InputGroupsEnum.CarCircleMenu;
					}
					return true;
				}
				return false;
			}
			if (!IsGarageDoor && _personInside && (currentInputGroup == InputGroupsEnum.Person || currentInputGroup == InputGroupsEnum.PersonOther || currentInputGroup == InputGroupsEnum.PersonCircleMenu))
			{
				return !IsCarDoorButtonActive();
			}
			return false;
		}

		private bool IsCarDoorButtonActive()
		{
			if (!_quickCarSettings.Door || !_quickCarSettings.Door.gameObject.activeInHierarchy)
			{
				if ((bool)_quickCarSettings.DoorPassenger)
				{
					return _quickCarSettings.DoorPassenger.gameObject.activeInHierarchy;
				}
				return false;
			}
			return true;
		}

		private void Refresh()
		{
			bool flag = ShouldListen();
			if (flag != _listening)
			{
				_listening = flag;
				_isActive = flag;
				_quickCarSettings.ShowHomeDoorButton(flag, IsGarageDoor);
				ScriptableEvent scriptableEvent = _interactHandle.Get(_eventRegistry);
				scriptableEvent?.Unsubscribe(AcceptDelegate);
				if (flag)
				{
					scriptableEvent?.Subscribe(AcceptDelegate);
				}
			}
		}

		private void AcceptDelegate()
		{
			InputGroupsEnum currentInputGroup = _inputMapSetEvent.CurrentInputGroup;
			if ((currentInputGroup == InputGroupsEnum.CarFree || currentInputGroup == InputGroupsEnum.Person) && (bool)_trackedButton && _trackedButton.isActiveAndEnabled)
			{
				Interact();
			}
		}

		protected virtual void Start()
		{
			_freeDriveProvider = V.CoreGameManager.FreeDriveProvider;
			if ((bool)_freeDriveProvider.QuickCarSettings)
			{
				_quickCarSettings = _freeDriveProvider.QuickCarSettings;
				_eventRegistry = _quickCarSettings._eventRegistry;
				_inputMapSetEvent = _quickCarSettings._inputMapSetEvent;
				if (IsGarageDoor && (bool)_quickCarSettings.GarageDoorButton)
				{
					_trackedButton = _quickCarSettings.GarageDoorButton;
				}
				else if ((bool)_quickCarSettings.HomeDoorButton)
				{
					_trackedButton = _quickCarSettings.HomeDoorButton;
				}
				if ((bool)_trackedButton && _trackedButton.isActiveAndEnabled)
				{
					_trackedButton.onClick.AddListener(Interact);
				}
			}
		}

		private void OnDestroy()
		{
			if (_freeDriveProvider != null && (bool)_quickCarSettings && (bool)_trackedButton && _trackedButton.isActiveAndEnabled)
			{
				_trackedButton.onClick.RemoveListener(Interact);
			}
			if (_listening)
			{
				_interactHandle.Get(_eventRegistry)?.Unsubscribe(AcceptDelegate);
			}
		}

		private void Interact()
		{
			if (_isActive)
			{
				Tween?.Kill();
				InteractInternal();
				IsOpen = !IsOpen;
			}
		}

		protected abstract void InteractInternal();

		private void Awake()
		{
			if ((bool)Front && (bool)Back)
			{
				Front.isTrigger = true;
				Back.isTrigger = true;
				Front.gameObject.EnsureComponent<ProceduralDoorFrontCollider>().Init(this);
				Back.gameObject.EnsureComponent<ProceduralDoorBackCollider>().Init(this);
			}
		}

		public Action CreateCallback(bool isFront, bool entering)
		{
			return () =>
			{
				_isActive = entering;
				IsFront = isFront;
				_quickCarSettings.ShowHomeDoorButton(entering, IsGarageDoor);
				_wasOutOfCar = FlagsStorage.OutOfCar;
				if (_wasOutOfCar)
				{
					_personInside = entering;
				}
				else
				{
					_carInside = entering;
				}
				_listening = !ShouldListen();
				Refresh();
			};
		}
	}
}

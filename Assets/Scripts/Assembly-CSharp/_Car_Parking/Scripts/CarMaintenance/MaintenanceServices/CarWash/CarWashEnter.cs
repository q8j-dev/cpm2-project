using System;
using System.Collections.Generic;
using CPMEngine;
using CodeStage.AntiCheat.ObscuredTypes;
using CodeStage.AntiCheat.Storage;
using DG.Tweening;
using UniRx;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.Audio;
using _Car_Parking.Scripts.CustomDataTypes;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.SOAP;

namespace _Car_Parking.Scripts.CarMaintenance.MaintenanceServices.CarWash
{
	public class CarWashEnter : MonoBehaviour
	{
		private uint _playRoboCarWash = AudioWrapper.GetId("Play_robocarwash");

		private uint _stopRoboCarWash = AudioWrapper.GetId("Stop_robocarwash");

		private const string Car = "Car";

		private const float MaxDistance = 6.25f;

		private const float MaxAngle = 10f;

		private ObscuredInt _serviceCost = 5000;

		private const string CarCleanLocalizeKey = "MaintenanceService.Carisclean";

		private const string NotEnoughMoneyLocalizeKey = "FreeDriveControl.NotEnoughMoney";

		private const string MoneyKey = "money";

		private const float MaxDistanceToEnter = 49f;

		private float _carThrottleValue = 0.5f;

		[SerializeField]
		private Transform EnterPoint = default;

		[SerializeField]
		private Transform ExitPoint = default;

		[SerializeField]
		private List<Animator> Animators = default;

		[SerializeField]
		private List<ParticleSystem> Particles = default;

		private VehicleData _currentVehicleToWash;

		private Tween _reduceCarDirtTween;

		private bool _isCarWashPlaying;

		private bool _isOtherSide;

		private bool _notEnoughMoney;

		private bool _carIsClean;

		private bool _controlsHidden;

		private bool _isWashing;

		private CurrencyManagerController _currencyManagerController;

		private FreeDriveProvider _freeDriveProvider;

		private IDisposable _followDisposable;

		private bool _isAutomaticGearbox;

		private CarInputManager _carInputManager;

		private InputItemsProvider _inputItemsProvider;

		private EventRegistry _eventRegistry;

		private RegistryHandle<BoolEvent> _setActionsActiveHandle = new RegistryHandle<BoolEvent>(EventRegistryIDs.SetActionsActive);

		private RegistryHandle<BoolEvent> _onHandbrakeHandle = new RegistryHandle<BoolEvent>(EventRegistryIDs.OnHandbrake);

		private RegistryHandle<FloatVariable> _handbrakeHandle = new RegistryHandle<FloatVariable>(EventRegistryIDs.Handbrake);

		private Powertrain _washPowertrain;

		private bool _controlReleased;

		[Inject]
		public void Construct(CurrencyManagerController currencyManagerController, FreeDriveProvider freeDriveProvider, InputItemsProvider inputItemsProvider, EventRegistry eventRegistry, CarInputManager carInputManager)
		{
			_currencyManagerController = currencyManagerController;
			_freeDriveProvider = freeDriveProvider;
			_inputItemsProvider = inputItemsProvider;
			_carInputManager = carInputManager;
			_eventRegistry = eventRegistry;
		}

		public void OnTriggerEnter(Collider other)
		{
			if (_isWashing || !other.CompareTag("Car") || other.transform.parent.gameObject != V.Car || !IsEnter(other.transform) || !other.transform.parent.TryGetComponent<Rigidbody>(out var carRigidbody) || !other.transform.parent.TryGetComponent<Powertrain>(out var powertrain) || !other.transform.parent.TryGetComponent<Axles>(out var _))
			{
				return;
			}
			if (other.transform.parent.TryGetComponent<VehicleData>(out _currentVehicleToWash) && _currentVehicleToWash.DirtComposition.DirtIntensity < 0.1f)
			{
				_carIsClean = true;
				V.CoreGameManager.SetMessage(new LocalizedString("MaintenanceService.Carisclean"), 2f);
				return;
			}
			if (ObscuredPrefs.GetFloat("money", 25000f) < (float)(int)_serviceCost)
			{
				V.CoreGameManager.SetMessage(new LocalizedString("FreeDriveControl.NotEnoughMoney"), 2f);
				_notEnoughMoney = true;
				return;
			}
			_isWashing = true;
			_controlReleased = false;
			_washPowertrain = powertrain;
			PrepareCarForAutoDrive(powertrain);
			_isAutomaticGearbox = powertrain.IsAutomaticGearbox;
			foreach (Animator animator in Animators)
			{
				if ((bool)animator)
				{
					animator.enabled = true;
				}
			}
			foreach (ParticleSystem particle in Particles)
			{
				if ((bool)particle)
				{
					particle.Play();
				}
			}
			if (!_isCarWashPlaying)
			{
				AudioWrapper.PostEvent(_playRoboCarWash, base.gameObject);
				_isCarWashPlaying = true;
			}
			Vector3 pointToFollow = ExitPoint.position - V.GlobalPoint.position;
			powertrain.IsAutomaticGearbox = true;
			powertrain.AutoClutch = true;
			_setActionsActiveHandle.Get(_eventRegistry).Raise(value: false);
			_followDisposable = Observable.EveryUpdate().TakeWhile((long _) => carRigidbody.velocity.magnitude > 0.1f).Subscribe((long _) =>
			{
				carRigidbody.velocity *= 0.95f;
				carRigidbody.angularVelocity *= 0.95f;
				_carInputManager.ThrottleAutoInput = 0f;
				_carInputManager.TargetSpeed = 0f;
				_carInputManager.BrakeAutoInput = 0.6f;
				_carInputManager.SetFollowPoint(pointToFollow);
				if (V.CoreGameManager != null && (bool)V.CoreGameManager.MenuLevelParent)
				{
					V.CoreGameManager.MenuLevelParent.SetVisualSteerControlOff();
				}
				_carInputManager.IgnoreAllControl(state: true);
				other.transform.parent.position = new Vector3(EnterPoint.position.x, other.transform.parent.position.y, EnterPoint.position.z);
				other.transform.parent.rotation = Quaternion.Euler(other.transform.rotation.eulerAngles.x, EnterPoint.rotation.eulerAngles.y, other.transform.rotation.eulerAngles.z);
			}, () =>
			{
				_freeDriveProvider.FreeDriveControl.toRoadButton.SetActive(value: false);
				_carInputManager.ThrottleAutoInput = _carThrottleValue;
				_carInputManager.TargetSpeed = 5f;
				_carInputManager.BrakeAutoInput = 0f;
				_carInputManager.OnLeftRightSteerReleased();
				_carInputManager.ResetSteerAndWheelsPosition();
				powertrain.SetSteer(0f, useAssistent: false);
				if (_freeDriveProvider != null && (bool)_freeDriveProvider.QuickCarSettings)
				{
					if (_inputItemsProvider.ShifterControl != null)
					{
						_controlsHidden = true;
						_inputItemsProvider.ShifterControl.HideObjects();
					}
					BlockExitCar(block: true);
					if (powertrain.Gear <= 0)
					{
						powertrain.Gear = 1;
					}
				}
				other.transform.parent.position = new Vector3(EnterPoint.position.x, other.transform.parent.position.y, EnterPoint.position.z);
				other.transform.parent.rotation = Quaternion.Euler(other.transform.rotation.eulerAngles.x, EnterPoint.rotation.eulerAngles.y, other.transform.rotation.eulerAngles.z);
			})
				.AddTo(this);
			StartReduceCarDirt();
		}

		private void BlockExitCar(bool block)
		{
			if ((bool)V.DriverPerson && V.DriverPerson.TryGetComponent<DriverActionController>(out var component))
			{
				component.IsExitCarBlocked = block;
			}
		}

		private bool IsEnter(Transform car)
		{
			bool flag = (car.position - EnterPoint.position).sqrMagnitude < 49f;
			_isOtherSide = !flag;
			return flag;
		}

		private void OnTriggerExit(Collider other)
		{
			if (other.CompareTag("Car") && !(other.transform.parent.gameObject != V.Car) && other.transform.parent.TryGetComponent<Powertrain>(out var component))
			{
				if (_notEnoughMoney || _isOtherSide || _carIsClean)
				{
					_notEnoughMoney = false;
					_isOtherSide = false;
					_carIsClean = false;
					_currentVehicleToWash = null;
				}
				else
				{
					CancelMovement(component);
					_currentVehicleToWash = null;
				}
			}
		}

		public void CancelMovement(Powertrain powerTrain)
		{
			_isWashing = false;
			foreach (Animator animator in Animators)
			{
				if ((bool)animator)
				{
					animator.enabled = false;
				}
			}
			foreach (ParticleSystem particle in Particles)
			{
				if ((bool)particle)
				{
					particle.Stop();
				}
			}
			StopCarWashSound();
			_reduceCarDirtTween?.Kill();
			ReleaseCarControl(powerTrain);
			if ((bool)_currentVehicleToWash)
			{
				_currentVehicleToWash = null;
			}
		}

		private void ReleaseCarControl(Powertrain powerTrain)
		{
			if (_controlReleased)
			{
				return;
			}
			_controlReleased = true;
			_followDisposable?.Dispose();
			_freeDriveProvider.FreeDriveControl.toRoadButton.SetActive(value: true);
			_carInputManager.ThrottleAutoInput = 0f;
			_carInputManager.TargetSpeed = 0f;
			_carInputManager.BrakeAutoInput = 0.6f;
			_carInputManager.IgnoreAllControl(state: false);
			_setActionsActiveHandle.Get(_eventRegistry).Raise(value: true);
			_carInputManager.SetFollowPoint(Vector3.zero);
			if (V.CoreGameManager != null && (bool)V.CoreGameManager.MenuLevelParent)
			{
				V.CoreGameManager.MenuLevelParent.SetSteering();
			}
			powerTrain.IsAutomaticGearbox = _isAutomaticGearbox;
			powerTrain.AutoClutch = _isAutomaticGearbox;
			powerTrain.ApplyEngineData();
			BlockExitCar(block: false);
			if (_freeDriveProvider != null && (bool)_freeDriveProvider.QuickCarSettings && _controlsHidden && _inputItemsProvider.ShifterControl != null)
			{
				_controlsHidden = false;
				_inputItemsProvider.ShifterControl.ShowObjects(fromControlUi: false);
				if (_isAutomaticGearbox)
				{
					_freeDriveProvider.QuickCarSettings.SetGearSelf(4);
					return;
				}
				_inputItemsProvider.ShifterControl.SetNeutral();
				_inputItemsProvider.ShifterControl.ResetPos();
			}
		}

		private void PrepareCarForAutoDrive(Powertrain powertrain)
		{
			if (!powertrain.IsEngineWorks)
			{
				powertrain.StartEngine(starterSound: true);
			}
			FloatVariable floatVariable = _handbrakeHandle.Get(_eventRegistry);
			if ((bool)floatVariable && floatVariable.Value > 0f)
			{
				_onHandbrakeHandle.Get(_eventRegistry)?.Raise(value: false);
				floatVariable.Value = 0f;
			}
		}

		private void StopCarWashSound()
		{
			if (_isCarWashPlaying)
			{
				AudioWrapper.PostEvent(_stopRoboCarWash, base.gameObject);
				_isCarWashPlaying = false;
			}
		}

		private void StartReduceCarDirt()
		{
			if ((bool)_currentVehicleToWash)
			{
				_reduceCarDirtTween?.Kill();
				_reduceCarDirtTween = DOVirtual.Float(_currentVehicleToWash.DirtComposition.DirtIntensity, 0f, 3f, (float v) =>
				{
					_currentVehicleToWash.DirtComposition.ReduceCarDirtIntensity(_currentVehicleToWash.DirtComposition.DirtIntensity - v);
				}).SetDelay(4f).OnComplete(TakeMoney)
					.SetTarget(_currentVehicleToWash.gameObject);
			}
		}

		private void TakeMoney()
		{
			AdvancedInt price = new AdvancedInt(_serviceCost);
			_currencyManagerController.TryMoneySpend(out var _, price);
			V.CoreGameManager.SetMessage(string.Concat(new LocalizedString("MaintenanceService.Carisclean"), "\n", (-(int)_serviceCost).ToString()), 2f);
			++_freeDriveProvider.FreeDriveDB.carWash;
			if ((bool)_washPowertrain)
			{
				ReleaseCarControl(_washPowertrain);
			}
		}

		private void OnDisable()
		{
			StopCarWashSound();
			BlockExitCar(block: false);
			_isWashing = false;
			_setActionsActiveHandle.Get(_eventRegistry)?.Raise(value: true);
		}

		private void OnDestroy()
		{
			_reduceCarDirtTween?.Kill();
			_setActionsActiveHandle.Get(_eventRegistry)?.Raise(value: true);
		}
	}
}

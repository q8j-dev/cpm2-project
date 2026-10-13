using System;
using System.Threading;
using CPM.Scripts.Garage;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.Events;
using VContainer;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.DynoRun;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Other.Helpers;

namespace _Car_Parking.Scripts
{
	public class CarParkTester : MonoBehaviour
	{
		[SerializeField]
		private ConditionBase[] Conditions;

		[SerializeField]
		private TesterType Type;

		[SerializeField]
		private bool IsDyno = true;

		public UnityEvent OnPassed;

		private bool _isPassed;

		private bool _isChecking;

		private CoreGameManager _coreGameManager;

		private UniTask _checkConditionsTask;

		private ManagerUI _managerUI;

		private GarageController _garageController;

		private bool _tempIgnoreCollision;

		private IDisposable _tempIgnoreCollisionDisposable;

		private ExteriorTuning _exteriorTuning;

		private GaragePositionsConfig _garagePositionsConfig;

		private IGarageSelectorService _garageSelectorService;

		private CancellationTokenSource _checkCts;

		public TesterType TesterType => Type;

		[Inject]
		public void Construct(CoreGameManager coreGameManager, ManagerUI managerUI, GarageController garageController, ExteriorTuning exteriorTuning, GaragePositionsConfig garagePositionsConfig, IGarageSelectorService garageSelectorService)
		{
			_exteriorTuning = exteriorTuning;
			_garageController = garageController;
			_managerUI = managerUI;
			_coreGameManager = coreGameManager;
			_garagePositionsConfig = garagePositionsConfig;
			_garageSelectorService = garageSelectorService;
		}

		private void Awake()
		{
			GlobalEventSystem.OnTesterCollisionIgnored += OnTesterCollisionIgnored;
		}

		private void Start()
		{
			int currentGarageIndex = _garageSelectorService.CurrentGarageIndex;
			switch (Type)
			{
			case TesterType.Exterior:
				SetPositionAndRotation(_garagePositionsConfig.GaragePositionsDatas[currentGarageIndex].VisualTuningChecker);
				_garageController.SetChecker(this);
				break;
			case TesterType.Dyno:
				SetPositionAndRotation(_garagePositionsConfig.GaragePositionsDatas[currentGarageIndex].DynoChecker);
				_garageController.SetChecker(this);
				break;
			}
		}

		private void SetPositionAndRotation(PositionsAndRotation positionsAndRotation)
		{
			base.transform.SetPositionAndRotation(positionsAndRotation.Position, positionsAndRotation.Rotation);
		}

		private void OnTesterCollisionIgnored(TesterType type, float duration)
		{
			if (type == Type)
			{
				_tempIgnoreCollision = true;
				_tempIgnoreCollisionDisposable?.Dispose();
				_tempIgnoreCollisionDisposable = Observable.Timer(TimeSpan.FromSeconds(duration)).Subscribe((long _) =>
				{
					_tempIgnoreCollision = false;
				}).AddTo(this);
			}
		}

		private void OnTriggerEnter(Collider other)
		{
			if (!FlagsStorage.IsPhotoMode && !_tempIgnoreCollision && !_isChecking && (other.CompareTag("Car") || other.CompareTag("OnlineCar")))
			{
				VehicleData componentInParent = other.GetComponentInParent<VehicleData>();
				_isPassed = false;
				if (!componentInParent.Towing && _checkConditionsTask.Status != UniTaskStatus.Pending)
				{
					_isChecking = true;
					_checkCts?.Cancel();
					_checkCts?.Dispose();
					_checkCts = new CancellationTokenSource();
					_checkConditionsTask = CheckConditionsTask(componentInParent, _checkCts.Token);
				}
			}
		}

		private async UniTask CheckConditionsTask(VehicleData vehicle, CancellationToken token)
		{
			_ = 1;
			try
			{
				await UniTask.Delay(TimeSpan.FromSeconds(2.0), ignoreTimeScale: false, PlayerLoopTiming.Update, token);
				while (!_isPassed && _isChecking && !token.IsCancellationRequested)
				{
					bool flag = true;
					ConditionBase[] conditions = Conditions;
					foreach (ConditionBase conditionBase in conditions)
					{
						if (!conditionBase.CheckCondition(vehicle, base.gameObject))
						{
							flag = false;
							if (!(V.Car != vehicle.gameObject))
							{
								_coreGameManager.SetMessage(new LocalizedString(conditionBase.FailMessage), 2f);
								break;
							}
						}
					}
					if (!flag || FlagsStorage.IsPhotoMode)
					{
						await UniTask.Delay(TimeSpan.FromSeconds(3.0), ignoreTimeScale: false, PlayerLoopTiming.Update, token);
					}
					else
					{
						_isPassed = true;
					}
				}
				if (!_isPassed)
				{
					return;
				}
				if (IsDyno)
				{
					_managerUI.ShowDynoWindow();
					_coreGameManager.menuControl.OpenDyno();
				}
				else
				{
					if ((bool)vehicle.GetPowertrain && (vehicle.GetPowertrain.IsEngineWorks || vehicle.GetPowertrain.Starter > 0f))
					{
						vehicle.GetPowertrain.ShutdownEngine();
					}
					_managerUI.ShowVisualTuningWindow();
					_exteriorTuning.OpenVisualTuning(openFromUI: false);
				}
				if (ScenesHelper.IsMenuSceneActive())
				{
					V.CoreGameManager.FreeDriveProvider?.QuickCarSettings?.DisableAllLights();
				}
				_managerUI.CloseWindow("MenuControllerHUD");
			}
			catch (OperationCanceledException)
			{
			}
		}

		private void OnTriggerExit(Collider other)
		{
			if (!_tempIgnoreCollision && (other.CompareTag("Car") || other.CompareTag("OnlineCar")))
			{
				SetOffDynochecker(Type);
			}
		}

		private void OnDestroy()
		{
			GlobalEventSystem.OnTesterCollisionIgnored -= OnTesterCollisionIgnored;
			_isChecking = false;
			_tempIgnoreCollision = false;
			_checkCts?.Cancel();
			_checkCts?.Dispose();
			_checkCts = null;
		}

		public void SetOffDynochecker(TesterType testerType)
		{
			if (testerType == Type)
			{
				_checkCts?.Cancel();
				_checkCts?.Dispose();
				_checkCts = null;
				_isChecking = false;
				_isPassed = false;
			}
		}
	}
}

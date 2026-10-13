using System;
using Cysharp.Threading.Tasks;
using Service;
using UniRx;
using UnityEngine;
using VContainer;
using _Car_Parking;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Providers;

namespace Car_Parking.Scripts.Racing
{
	public class RaceStartTrigger : MonoBehaviour
	{
		[SerializeField]
		private float CheckInterval = 0.5f;

		[SerializeField]
		private int RaceIndex;

		[SerializeField]
		private int TrajectoryIndex;

		[SerializeField]
		private ZoneTriggerHandler ZoneTriggerHandler;

		[SerializeField]
		private TimeAttackMode TimeAttackMode;

		[SerializeField]
		private bool DontUseCheckpoints;

		[SerializeField]
		private bool RotateCarToStart;

		[SerializeField]
		private RaceTriggersValidator RaceTriggersValidator;

		private FreeDriveProvider _freeDriveProvider;

		private FreeDriveControl _freeDriveControl;

		private IPrefabFactory _prefabFactory;

		private FreeTimeAttackController _timeAttackController;

		private CircleProgressBarItem _progressBarItem;

		private MenuLevelParent _menuLevelParent;

		private UIActionButton _uiActionButton;

		private ManagerUI _managerUI;

		private VehicleData _vehicleData;

		[Inject]
		public void Construct(FreeDriveProvider freeDriveProvider, IPrefabFactory prefabFactory, FreeTimeAttackController timeAttackController, MenuLevelParent menuLevelParent, ManagerUI managerUI)
		{
			_freeDriveProvider = freeDriveProvider;
			_prefabFactory = prefabFactory;
			_timeAttackController = timeAttackController;
			_menuLevelParent = menuLevelParent;
			_managerUI = managerUI;
		}

		private void Start()
		{
			ZoneTriggerHandler zoneTriggerHandler = ZoneTriggerHandler;
			zoneTriggerHandler.OnZoneEntered = (Action)Delegate.Combine(zoneTriggerHandler.OnZoneEntered, new Action(OnZoneEnter));
			ZoneTriggerHandler zoneTriggerHandler2 = ZoneTriggerHandler;
			zoneTriggerHandler2.OnZoneExit = (Action)Delegate.Combine(zoneTriggerHandler2.OnZoneExit, new Action(OnZoneExit));
			V.OnOutOfCarChanged = (Action<bool>)Delegate.Combine(V.OnOutOfCarChanged, new Action<bool>(OnOutOfCarChanged));
			Observable.Interval(TimeSpan.FromSeconds(CheckInterval)).Subscribe((long _) =>
			{
				ZoneTriggerHandler.CheckZone();
			}).AddTo(this);
		}

		private void OnOutOfCarChanged(bool outOfCar)
		{
			if (base.gameObject.activeSelf && !outOfCar && ZoneTriggerHandler.IsInZone)
			{
				ZoneTriggerHandler.ForceExit();
			}
		}

		private void OnZoneEnter()
		{
			if (!_freeDriveProvider.FirstCarMultiPlayer.GetComponent<FreeRaceCar>())
			{
				if (!_uiActionButton)
				{
					CreateButton(_freeDriveProvider.FirstCarMultiPlayer).Forget();
				}
				if (!RaceTriggersValidator)
				{
					RaceTriggersValidator = GetComponentInParent<RaceTriggersValidator>(includeInactive: true);
				}
				if (!RaceTriggersValidator)
				{
					RaceTriggersValidator = _timeAttackController.ResolveValidatorFor(RaceIndex, TrajectoryIndex, base.gameObject.scene);
				}
				if ((bool)RaceTriggersValidator)
				{
					_timeAttackController.RegisterTriggerValidator(RaceTriggersValidator);
				}
				_timeAttackController.SetTimeAttackMode(TimeAttackMode);
				_timeAttackController.PrepareRace();
			}
		}

		private void OnZoneExit()
		{
			CloseStartRaceButton();
			if (_timeAttackController.RaceStatus == TimeAttackStatus.Prepare)
			{
				_timeAttackController.StopPrepareRace();
			}
			if (!_freeDriveProvider.FreeDriveControl.inviteToRaceButton.gameObject.activeSelf)
			{
				_freeDriveProvider.FreeDriveControl.inviteToRaceButton.SetActive(active: true);
			}
		}

		private async UniTaskVoid CreateButton(GameObject car)
		{
			_uiActionButton = await _managerUI.CreateActionButton("SpeedActionButton", () =>
			{
				OnCarEnter(car);
			}, CantStartRace);
		}

		private void CantStartRace()
		{
			_managerUI.ShowNotificationWindow(new LocalizedString("TimeAttack.CantStartRace"), 3f).Forget();
		}

		private void CloseStartRaceButton()
		{
			if ((bool)_uiActionButton)
			{
				_uiActionButton.Close();
			}
		}

		private void OnCarEnter(GameObject car)
		{
			SoundEventsDataHelper.PlayUISound(SoundEventsDataHelper.AcceptSwitch, base.gameObject);
			CloseStartRaceButton();
			if (Global.TaskManager.Task != null || FlagsStorage.IsPhotoMode || _menuLevelParent.isDroneEnabled)
			{
				V.CoreGameManager.SetMessage("Free.FinishCurrentMission", 3f);
			}
			else if (!FlagsStorage.IsPassenger && !(V.Car != car) && !(car.gameObject != _freeDriveProvider.FirstCarMultiPlayer))
			{
				_timeAttackController.Init(RaceIndex, DontUseCheckpoints, TrajectoryIndex);
				FreeRaceCar freeRaceCar = _prefabFactory.InstantiateComponent<FreeRaceCar>(car);
				_timeAttackController.RegisterCar(freeRaceCar);
				freeRaceCar.InitTimeAttackMode(TimeAttackMode);
				if (RotateCarToStart)
				{
					freeRaceCar.ResetRigidbody();
					_timeAttackController.RotateCarToStart();
				}
			}
		}

		private void OnDisable()
		{
			CloseStartRaceButton();
		}

		private void OnDestroy()
		{
			ZoneTriggerHandler zoneTriggerHandler = ZoneTriggerHandler;
			zoneTriggerHandler.OnZoneEntered = (Action)Delegate.Remove(zoneTriggerHandler.OnZoneEntered, new Action(OnZoneEnter));
			ZoneTriggerHandler zoneTriggerHandler2 = ZoneTriggerHandler;
			zoneTriggerHandler2.OnZoneExit = (Action)Delegate.Remove(zoneTriggerHandler2.OnZoneExit, new Action(OnZoneExit));
			V.OnOutOfCarChanged = (Action<bool>)Delegate.Remove(V.OnOutOfCarChanged, new Action<bool>(OnOutOfCarChanged));
		}
	}
}

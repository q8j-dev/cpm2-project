using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.DataStorageService;
using _Car_Parking.Scripts.DataStorageService.PlayerData;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.EngineTuning.TuningTimer;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Other.Helpers;
using _Car_Parking.Scripts.Scopes;
using _Car_Parking.Scripts.UIElement;

namespace _Car_Parking.Scripts.WindowSystem
{
	public class ClassChooseWindow : WindowScript, IBackableWindow
	{
		[SerializeField]
		private DragClassUIElement S12;

		[SerializeField]
		private DragClassUIElement S11;

		[SerializeField]
		private DragClassUIElement S10;

		[SerializeField]
		private DragClassUIElement S9;

		[SerializeField]
		private DragClassUIElement Unlimited;

		[SerializeField]
		private Button BackButton;

		private int _selectedClass = 12;

		private ITimerTuningService _timerTuningService;

		private OptionalDependency<ITimerTuningService> _timerTuningServiceDep;

		private DragRacingController _dragRacingController;

		private OptionalDependency<DragRacingController> _dragRacingControllerDep;

		private IPlayerStorageService _playerStorageService;

		[Inject]
		public void Construct(IPlayerStorageService playerStorageService, OptionalDependency<ITimerTuningService> timerTuningService, OptionalDependency<DragRacingController> dragRacingController)
		{
			_playerStorageService = playerStorageService;
			_dragRacingControllerDep = dragRacingController;
			_timerTuningServiceDep = timerTuningService;
		}

		protected override void Awake()
		{
			base.Awake();
			_timerTuningService = _timerTuningServiceDep?.Value;
			_dragRacingController = _dragRacingControllerDep?.Value;
		}

		protected override void Start()
		{
			base.Start();
			InitBackButton();
			if (ScenesHelper.IsMenuSceneActive())
			{
				BackButton.SetActive(active: true);
				BackButton.onClick.AddListener(Back);
			}
			else
			{
				BackButton.SetActive(active: false);
			}
			PlayerStorage data = _playerStorageService.Data;
			if (data.RacingProgress == null)
			{
				data.RacingProgress = new RacingProgress();
			}
			List<RacingClassRecord> list = _playerStorageService.Data.RacingProgress.RacingClassRecords ?? new List<RacingClassRecord>();
			S12.Init(list.Find((RacingClassRecord record) => record.TimeClass == 12));
			S11.Init(list.Find((RacingClassRecord record) => record.TimeClass == 11));
			S10.Init(list.Find((RacingClassRecord record) => record.TimeClass == 10));
			S9.Init(list.Find((RacingClassRecord record) => record.TimeClass == 9));
			Unlimited.Init(list.Find((RacingClassRecord record) => record.TimeClass == 8));
			DragClassUIElement s = S12;
			s.OnStartButtonClicked = (Action)Delegate.Combine(s.OnStartButtonClicked, (Action)(() =>
			{
				StartDrag(12);
			}));
			DragClassUIElement s2 = S11;
			s2.OnStartButtonClicked = (Action)Delegate.Combine(s2.OnStartButtonClicked, (Action)(() =>
			{
				StartDrag(11);
			}));
			DragClassUIElement s3 = S10;
			s3.OnStartButtonClicked = (Action)Delegate.Combine(s3.OnStartButtonClicked, (Action)(() =>
			{
				StartDrag(10);
			}));
			DragClassUIElement s4 = S9;
			s4.OnStartButtonClicked = (Action)Delegate.Combine(s4.OnStartButtonClicked, (Action)(() =>
			{
				StartDrag(9);
			}));
			DragClassUIElement unlimited = Unlimited;
			unlimited.OnStartButtonClicked = (Action)Delegate.Combine(unlimited.OnStartButtonClicked, (Action)(() =>
			{
				StartDrag(8);
			}));
		}

		private void StartDrag(int timeClass)
		{
			_selectedClass = timeClass;
			RunDragRacing();
		}

		private void RunDragRacing()
		{
			_timerTuningService.IsCarBlocked(V.SlotID).Get(out var value);
			if (ScenesHelper.IsMenuSceneActive() & value)
			{
				_managerUI.ShowNotificationWindow(new LocalizedString("ClassChooseWindow.Yourcarcurrentlyisinservice"), 3f).Forget();
				return;
			}
			if (ScenesHelper.IsMenuSceneActive())
			{
				_managerUI.ShowWaitingWindow(SingleTrack).Forget();
				return;
			}
			_dragRacingController.ChangeClass(_selectedClass);
			Close();
		}

		private void SingleTrack()
		{
			if ((bool)V.DriverPerson)
			{
				UnityEngine.Object.DontDestroyOnLoad(V.DriverPerson);
			}
			if ((bool)V.Car)
			{
				UnityEngine.Object.DontDestroyOnLoad(V.Car);
			}
			FlagsStorage.RacingGameMode = true;
			SceneManager.LoadSceneAsync(32);
		}

		public void InitBackButton()
		{
			_managerUI.OnBackButtonActivated?.Invoke(obj: true);
			_managerUI.SetCurrentWindow(this);
		}

		public void Back()
		{
			_managerUI.ShowGameModeWindow();
			Close();
		}
	}
}

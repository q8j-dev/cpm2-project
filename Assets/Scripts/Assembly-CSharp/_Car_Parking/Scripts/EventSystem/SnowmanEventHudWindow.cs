using System;
using CPMEngine;
using TMPro;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.Drag;

namespace _Car_Parking.Scripts.EventSystem
{
	public class SnowmanEventHudWindow : WindowScript
	{
		private TimedActivityZoneController _timedActivityZoneController;

		[SerializeField]
		private GameObject PenaltyPanel = default;

		[SerializeField]
		private TextMeshProUGUI PenaltyText = default;

		[SerializeField]
		private GameObject MainPanel = default;

		[SerializeField]
		private TextMeshProUGUI TimeText = default;

		[SerializeField]
		private TextMeshProUGUI CurentCountText = default;

		private UiNotificationsManager _uiNotificationsManager;

		[Inject]
		public void Construct(TimedActivityZoneController timedActivityZoneController, UiNotificationsManager uiNotificationsManager)
		{
			_timedActivityZoneController = timedActivityZoneController;
			_uiNotificationsManager = uiNotificationsManager;
		}

		protected override void OnEnable()
		{
			PenaltyText.text = "";
			base.OnEnable();
		}

		protected override void Start()
		{
			base.Start();
			_uiNotificationsManager.StopPopupQueue();
			OnUserExitActivityZone(_timedActivityZoneController.PenaltyTimerComponent.IsActive);
			EventZoneTimerComponent timerComponent = _timedActivityZoneController.TimerComponent;
			timerComponent.OnTick = (Action<int>)Delegate.Combine(timerComponent.OnTick, new Action<int>(OnCurrentTimeTick));
			EventZoneTimerComponent penaltyTimerComponent = _timedActivityZoneController.PenaltyTimerComponent;
			penaltyTimerComponent.OnTick = (Action<int>)Delegate.Combine(penaltyTimerComponent.OnTick, new Action<int>(OnCurrentPenaltyTimeTick));
			TimedActivityZoneController timedActivityZoneController = _timedActivityZoneController;
			timedActivityZoneController.OnCurrentZoneExit = (Action<bool>)Delegate.Combine(timedActivityZoneController.OnCurrentZoneExit, new Action<bool>(OnUserExitActivityZone));
		}

		private void OnUserExitActivityZone(bool state)
		{
			MainPanel.SetActive(!state);
			PenaltyPanel.SetActive(state);
			PenaltyText.text = "";
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			if (_timedActivityZoneController != null)
			{
				EventZoneTimerComponent timerComponent = _timedActivityZoneController.TimerComponent;
				timerComponent.OnTick = (Action<int>)Delegate.Remove(timerComponent.OnTick, new Action<int>(OnCurrentTimeTick));
				EventZoneTimerComponent penaltyTimerComponent = _timedActivityZoneController.PenaltyTimerComponent;
				penaltyTimerComponent.OnTick = (Action<int>)Delegate.Remove(penaltyTimerComponent.OnTick, new Action<int>(OnCurrentPenaltyTimeTick));
				TimedActivityZoneController timedActivityZoneController = _timedActivityZoneController;
				timedActivityZoneController.OnCurrentZoneExit = (Action<bool>)Delegate.Remove(timedActivityZoneController.OnCurrentZoneExit, new Action<bool>(OnUserExitActivityZone));
			}
		}

		private void OnCurrentTimeTick(int tick)
		{
			TimeText.text = Utils.ConvertToTime(tick);
			(int, int) currentProgress = _timedActivityZoneController.GetCurrentProgress();
			CurentCountText.text = $" <color=red>{currentProgress.Item1} </color> / {currentProgress.Item2}";
		}

		private void OnCurrentPenaltyTimeTick(int tick)
		{
			PenaltyText.text = Utils.ConvertToTime(tick);
		}

		private void OnDisable()
		{
			PenaltyText.text = "";
		}
	}
}

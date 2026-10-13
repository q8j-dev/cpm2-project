using System;
using MVPFree.MapIcons.ScrObjs.Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.EventSystem;
using _Car_Parking.Scripts.EventSystem.Data;
using _Car_Parking.Scripts.EventSystem.Data.Response;
using _Car_Parking.Scripts.EventSystem.EventService;
using _Car_Parking.Scripts.Infrastructure.Start;
using _Car_Parking.Scripts.Localization;

namespace _Car_Parking.Scripts.Drag
{
	public class EventMapPopupPanel : MonoBehaviour
	{
		[SerializeField]
		private TextMeshProUGUI EventButtonText = default;

		[SerializeField]
		private TextMeshProUGUI ChallengeText = default;

		[SerializeField]
		private TextMeshProUGUI ChallengeDescriptionText = default;

		[SerializeField]
		private TextMeshProUGUI EventTimeExpiredText = default;

		[SerializeField]
		private GameObject[] ButtonBackgrounds = default;

		[SerializeField]
		private GameObject[] ButtonOutlines = default;

		[SerializeField]
		private GameObject CanClaimIcon = default;

		[SerializeField]
		private GameObject EventDoneIcon = default;

		[SerializeField]
		private Button EventButton = default;

		private TimeUnitSuffixes _timeUnitSuffixes;

		private IEventService _eventService;

		private ServerTimeService _serverTimeService;

		private TimeAttackDataSO _timeAttackDataSo;

		private _Car_Parking.Scripts.EventSystem.Data.Event _currentEvent;

		private ManagerUI _managerUI;

		[Inject]
		[UnityEngine.Scripting.Preserve]
		public void Construct(TimeAttackDataSO timeAttackDataSo, IEventService eventService, ServerTimeService serverTimeService, ManagerUI managerUI)
		{
			_timeAttackDataSo = timeAttackDataSo;
			_eventService = eventService;
			_serverTimeService = serverTimeService;
			_managerUI = managerUI;
		}

		private void OnEnable()
		{
			_eventService.OnEventClaimed += OnEventClaimed;
		}

		public void Init(_Car_Parking.Scripts.EventSystem.Data.Event currentEvent)
		{
			_currentEvent = currentEvent;
			PrepareEventPanel();
		}

		private void OnDisable()
		{
			_eventService.OnEventClaimed -= OnEventClaimed;
		}

		private void Start()
		{
			EventButton.onClick.AddListener(ShowEventBanner);
			_timeUnitSuffixes = TextExtensions.CreateTimeUnitSuffixes();
		}

		private void PrepareEventPanel()
		{
			ChallengeText.text = new LocalizedString(_currentEvent.NameLocalizeKey);
			ChallengeDescriptionText.text = GetDescription();
			ChangeEventButton(_currentEvent.Status);
		}

		private string GetDescription()
		{
			string result = "";
			string text = "";
			switch (_currentEvent.Type)
			{
			case _Car_Parking.Scripts.EventSystem.EventType.DriftTimeAttack:
			{
				DriftAttackEventData driftAttackEventData = (DriftAttackEventData)_currentEvent.Data;
				text = new LocalizedString("EventBigBanner.Points.Drift.Condition2");
				result = $"{text} : {driftAttackEventData.Points} DP";
				break;
			}
			case _Car_Parking.Scripts.EventSystem.EventType.TimeAttack:
			case _Car_Parking.Scripts.EventSystem.EventType.TimeAttackNurb:
				text = new LocalizedString("EventViewWindow.CompleteLap");
				result = $"{text} {_currentEvent.Progress}/{_currentEvent.MaxProgress}";
				break;
			}
			return result;
		}

		private void ShowEventBanner()
		{
			if (_currentEvent != null)
			{
				_managerUI.ShowSingleEventViewWindow(_currentEvent.Type, showOnTop: true);
			}
		}

		private void ChangeEventButton(EventStatus eventStatus)
		{
			EventStatus[] array = (EventStatus[])Enum.GetValues(typeof(EventStatus));
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] == eventStatus)
				{
					ButtonBackgrounds[i].gameObject.SetActive(value: true);
					ButtonOutlines[i].gameObject.SetActive(value: true);
					EventButtonText.text = new LocalizedString($"EventButton_{eventStatus}");
					break;
				}
			}
			bool flag = eventStatus == EventStatus.Finished;
			bool flag2 = eventStatus == EventStatus.Claimed;
			CanClaimIcon.SetActive(flag);
			EventDoneIcon.SetActive(flag | flag2);
		}

		private void Update()
		{
			if (_currentEvent != null)
			{
				TimeSpan remaining = TimeFormatter.RemainingTime(_currentEvent.EndTime, _serverTimeService.CurrentServerUnixTimeSec);
				EventTimeExpiredText.text = TimeFormatter.FormatRemainingFull(remaining, in _timeUnitSuffixes);
			}
		}

		private void OnEventClaimed(_Car_Parking.Scripts.EventSystem.EventType eventType)
		{
			if (_currentEvent != null && eventType == _currentEvent.Type)
			{
				ChangeEventButton(EventStatus.Claimed);
			}
		}
	}
}

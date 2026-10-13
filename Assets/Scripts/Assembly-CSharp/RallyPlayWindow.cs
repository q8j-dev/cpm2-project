using System;
using CPMEngine;
using Cysharp.Threading.Tasks;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Racing_Scripts;
using _Car_Parking.Scripts.Rally.Data;
using _Car_Parking.Scripts.SOAP;
using _Car_Parking.Scripts.SOAP_Registry;

public class RallyPlayWindow : WindowScript
{
	private const float timeAlphaBeatValue = 0.1f;

	private const float timeAlphaDefaultValue = 1f;

	[SerializeField]
	private TextMeshProUGUI TimerText;

	[SerializeField]
	private TextMeshProUGUI PenaltyText;

	[SerializeField]
	private GameObject OutOfRoadIcon;

	[SerializeField]
	private GameObject WrongWayIcon;

	[SerializeField]
	private TextMeshProUGUI OffRoadCountdownText;

	[SerializeField]
	private Button SettingsButton;

	[SerializeField]
	private Button PhotoModeButton;

	[SerializeField]
	private RacingCarControllerUI RacingCarControllerUI;

	[SerializeField]
	private CanvasGroup[] TimesPanels;

	[SerializeField]
	private RallyPenaltyIconElement[] RallyPenaltyIconElements;

	[SerializeField]
	private TextMeshProUGUI[] TimesPanelTexts;

	private RallyController _rallyController;

	private RallyTriggersControl _triggersControl;

	private RallyCar _car;

	private RallySequenceData _sequenceData;

	private EventRegistry _eventRegistry;

	private RegistryHandle<ScriptableEvent> _openPhotoModeHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.OpenPhotoMode);

	private RegistryHandle<ScriptableEvent> _onMenuHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.OnMenu);

	protected override InputGroupsEnum WindowInputGroup => InputGroupsEnum.CarRally;

	protected override bool ChangeRootGameInputGroup => true;

	[Inject]
	public void Construct(ManagerUI managerUI, EventRegistry eventRegistry)
	{
		_managerUI = managerUI;
		_eventRegistry = eventRegistry;
	}

	protected override void Start()
	{
		SettingsButton.onClick.AddListener(ShowSettingsWindow);
		PhotoModeButton.onClick.AddListener(OpenPhotoMode);
		base.Start();
	}

	private void SetTargetTimes()
	{
		float num = _rallyController.RallyData?.EffectiveNgFactor ?? 1f;
		int num2 = Mathf.Min(TimesPanelTexts.Length, _sequenceData.RallyTimes.Length);
		for (int i = 0; i < num2; i++)
		{
			TimesPanelTexts[i].text = Utils.FormatTime(_sequenceData.RallyTimes[i].TargetTime * num);
		}
	}

	private void ShowSettingsWindow()
	{
		_rallyController.Pause(state: true);
		_managerUI.ShowRallyPauseWindow();
		base.gameObject.SetActive(value: false);
	}

	private void OpenPhotoMode()
	{
		OpenAndWaitPhotoMode().Forget();
	}

	private async UniTask OpenAndWaitPhotoMode()
	{
		WindowScript windowScript = await _managerUI.ShowPhotoModeWindow(cleanScreen: false);
		if ((bool)windowScript)
		{
			SoundEventsDataHelper.SetGroupState(SoundEventsDataHelper.StateReverPause, SoundEventsDataHelper.OffReverState);
			if ((bool)V.DriverPerson && V.DriverPerson.TryGetComponent<MainPerson>(out var component))
			{
				component.EnterPhotoModeHeadVisible();
			}
			base.gameObject.SetActive(value: false);
			windowScript.OnDestroyAsObservable().Subscribe((Unit _) =>
			{
				base.gameObject.SetActive(value: true);
			});
		}
	}

	protected override void OnEnable()
	{
		TimerText.text = "";
		PenaltyText.text = "";
		base.OnEnable();
		_onMenuHandle.Get(_eventRegistry).Subscribe(ShowSettingsWindow);
		_openPhotoModeHandle.Get(_eventRegistry).Subscribe(OpenPhotoMode);
	}

	public void OnDisable()
	{
		_onMenuHandle.Get(_eventRegistry)?.Unsubscribe(ShowSettingsWindow);
		_openPhotoModeHandle.Get(_eventRegistry)?.Unsubscribe(OpenPhotoMode);
	}

	public void Init(RallyController rallyController, RallyTriggersControl triggersControl, RallyCar car)
	{
		_rallyController = rallyController;
		_triggersControl = triggersControl;
		_car = car;
		_sequenceData = _rallyController.RallySequenceData;
		PenaltyText.text = "";
		SubscribeListeners();
		SetTargetTimes();
	}

	public void Resume()
	{
		RacingCarControllerUI.Resume();
	}

	private void SubscribeListeners()
	{
		RallyCar car = _car;
		car.OnTimerUpdate = (Action<float, float>)Delegate.Combine(car.OnTimerUpdate, new Action<float, float>(OnTimerUpdate));
		RallyCar car2 = _car;
		car2.OnOffRoadChanged = (Action<bool>)Delegate.Combine(car2.OnOffRoadChanged, new Action<bool>(OnOutOfRoad));
		RallyCar car3 = _car;
		car3.OnWrongWay = (Action<bool>)Delegate.Combine(car3.OnWrongWay, new Action<bool>(OnWrongWay));
		_car.OnOffRoadTimerChanged += OnOffRoadTimerChanged;
		_car.OnPenaltyAdded += OnPenaltyAdded;
		RacingCarControllerUI racingCarControllerUI = RacingCarControllerUI;
		racingCarControllerUI.OnToRoad = (Action)Delegate.Combine(racingCarControllerUI.OnToRoad, new Action(OnCarToRoad));
	}

	private void UnsubscribeListeners()
	{
		if (!(_car == null))
		{
			RallyCar car = _car;
			car.OnTimerUpdate = (Action<float, float>)Delegate.Remove(car.OnTimerUpdate, new Action<float, float>(OnTimerUpdate));
			RallyCar car2 = _car;
			car2.OnOffRoadChanged = (Action<bool>)Delegate.Remove(car2.OnOffRoadChanged, new Action<bool>(OnOutOfRoad));
			RallyCar car3 = _car;
			car3.OnWrongWay = (Action<bool>)Delegate.Remove(car3.OnWrongWay, new Action<bool>(OnWrongWay));
			_car.OnOffRoadTimerChanged -= OnOffRoadTimerChanged;
			_car.OnPenaltyAdded -= OnPenaltyAdded;
		}
	}

	private void OnOutOfRoad(bool state)
	{
		OutOfRoadIcon.SetActive(state);
		if (!state && (bool)OffRoadCountdownText)
		{
			OffRoadCountdownText.SetActive(active: false);
		}
	}

	private void OnWrongWay(bool state)
	{
		WrongWayIcon.SetActive(state);
	}

	private void OnOffRoadTimerChanged(float normalizedProgress)
	{
		if ((bool)OffRoadCountdownText)
		{
			bool flag = normalizedProgress > 0f;
			OffRoadCountdownText.SetActive(flag);
			if (flag)
			{
				float num = Mathf.Ceil((1f - normalizedProgress) * _car.DisqualifyTimeout);
				OffRoadCountdownText.text = $"{num:0}s";
			}
		}
	}

	private void OnTimerUpdate(float time, float penalty)
	{
		TimerText.text = Utils.FormatTime(time) ?? "";
		if (penalty > 0f)
		{
			PenaltyText.text = "+" + Utils.FormatTime(penalty);
		}
		CompareTimes(time + penalty);
	}

	private void CompareTimes(float time)
	{
		float num = _rallyController.RallyData?.EffectiveNgFactor ?? 1f;
		int num2 = Mathf.Min(TimesPanels.Length, _sequenceData.RallyTimes.Length);
		for (int i = 0; i < num2; i++)
		{
			TimesPanels[i].alpha = ((_sequenceData.RallyTimes[i].TargetTime * num < time) ? 0.1f : 1f);
		}
	}

	public void OnRaceStarted()
	{
		_triggersControl.ActivateFirstCheckpointTrigger();
	}

	protected override void OnDestroy()
	{
		if (RacingCarControllerUI != null)
		{
			RacingCarControllerUI racingCarControllerUI = RacingCarControllerUI;
			racingCarControllerUI.OnToRoad = (Action)Delegate.Remove(racingCarControllerUI.OnToRoad, new Action(OnCarToRoad));
		}
		UnsubscribeListeners();
		base.OnDestroy();
	}

	private void OnPenaltyAdded(PenaltyRecord record)
	{
		if (RallyPenaltyIconElements == null)
		{
			return;
		}
		RallyPenaltyIconElement[] rallyPenaltyIconElements = RallyPenaltyIconElements;
		foreach (RallyPenaltyIconElement rallyPenaltyIconElement in rallyPenaltyIconElements)
		{
			if ((bool)rallyPenaltyIconElement && rallyPenaltyIconElement.PenaltyType == record.Type)
			{
				rallyPenaltyIconElement.SetPenalty(record.Amount);
				break;
			}
		}
	}

	private void OnCarToRoad()
	{
		_rallyController.ToRoad();
	}
}

using System;
using AK.Wwise;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.Audio;
using _Car_Parking.Scripts.Audio.SoundBanks;
using _Car_Parking.Scripts.ScenesLogic.WorldTime;

[RequireComponent(typeof(Collider))]
public class EventPositionConfiner : MonoBehaviour
{
	[Header("Event to clamp to AkAudioListener")]
	public AK.Wwise.Event PlayEvent = new AK.Wwise.Event();

	public AK.Wwise.Event StopEvent = new AK.Wwise.Event();

	[SerializeField]
	private SoundBankType _soundBank = default;

	private SoundBankLoadService _soundBankService;

	public SkyboxBlender DayNightCont;

	public static uint DayNightRtpc;

	private TimeAndWeatherController _timeAndWeatherController;

	private IDisposable _timerStartCycleDisposable;

	private const float TimeToStartCycleDayNight = 10f;

	private SoundBankLoadService SoundBankService
	{
		get
		{
			if (_soundBankService == null && (bool)V.CoreGameManager)
			{
				_soundBankService = V.CoreGameManager.SoundBankLoadService;
			}
			return _soundBankService;
		}
	}

	[Inject]
	public void Construct(SoundBankLoadService soundBankService)
	{
		_soundBankService = soundBankService;
	}

	private void OnTriggerEnter(Collider other)
	{
		if ((bool)other.GetComponent<AkAudioListener>())
		{
			PlaySound();
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if ((bool)other.GetComponent<AkAudioListener>())
		{
			StopSound();
		}
	}

	public void PlaySound()
	{
		SoundBankService?.PostEvent(PlayEvent.Name, _soundBank, base.gameObject).Forget();
	}

	public void StopSound()
	{
		SoundBankLoadService soundBankService = SoundBankService;
		if (soundBankService != null && soundBankService.IsLoaded(_soundBank))
		{
			soundBankService.PostEvent(StopEvent.Name, _soundBank, base.gameObject).Forget();
		}
	}

	private void Awake()
	{
		Init();
		SoundBankService?.TryLoadBank(_soundBank).Forget();
	}

	private void Start()
	{
		OnStart();
	}

	private void Init()
	{
		if (DayNightRtpc == 0)
		{
			DayNightRtpc = AudioWrapper.GetId("RTPC_Day_Night_Cycle");
		}
		GetComponent<Collider>().isTrigger = true;
		V.OnSkyboxChanged = (Action)Delegate.Combine(V.OnSkyboxChanged, new Action(SetRTPCValue));
		DayNightCont = TimeProvider.skyboxBlender;
		InvokeRepeating("UpdateNights", 5f, 5f);
	}

	private void OnStart()
	{
		if (V.CoreGameManager.GetComponentInChildren<AkAudioListener>() == null)
		{
			base.enabled = false;
		}
		if ((bool)V.CoreGameManager.directionalLight)
		{
			V.CoreGameManager.directionalLight.TryGetComponent<TimeAndWeatherController>(out _timeAndWeatherController);
		}
		SetRTPCValue();
		if (DayNightCont == null)
		{
			DayNightCont = TimeProvider.skyboxBlender;
		}
	}

	private void OnDisable()
	{
		AudioWrapper.StopAll(base.gameObject);
	}

	private void OnDestroy()
	{
		V.OnSkyboxChanged = (Action)Delegate.Remove(V.OnSkyboxChanged, new Action(SetRTPCValue));
		if (_soundBankService != null)
		{
			_soundBankService.TryUnloadBank(_soundBank).Forget();
		}
	}

	private void UpdateNights()
	{
		if (TimeProvider.skyboxSelectedRuntime == 2)
		{
			if (DayNightCont == null)
			{
				DayNightCont = TimeProvider.skyboxBlender;
			}
			else
			{
				AudioWrapper.SetRTPCValue(DayNightRtpc, GetTimeValue(TimeProvider.skyboxSelectedRuntime), base.gameObject);
			}
		}
	}

	private void SetRTPCValue()
	{
		if ((bool)_timeAndWeatherController)
		{
			if (TimeProvider.skyboxSelectedRuntime == 3)
			{
				StartCycleDayNight();
				return;
			}
			_timerStartCycleDisposable?.Dispose();
			AudioWrapper.SetRTPCValue(DayNightRtpc, GetTimeValue(TimeProvider.skyboxSelectedRuntime), base.gameObject);
		}
	}

	private void StartCycleDayNight()
	{
		AudioWrapper.SetRTPCValue(DayNightRtpc, GetTimeValue(TimeProvider.skyboxSelectedRuntime), base.gameObject);
		_timerStartCycleDisposable?.Dispose();
		_timerStartCycleDisposable = Observable.Timer(TimeSpan.FromSeconds(10.0)).Subscribe((long _) =>
		{
			StartCycleDayNight();
		}).AddTo(this);
	}

	private float GetTimeValue(int id)
	{
		return id switch
		{
			0 => ConvertTimeToRange(_timeAndWeatherController.DayTime.Hour, _timeAndWeatherController.DayTime.Minute), 
			1 => ConvertTimeToRange(_timeAndWeatherController.EveningTime.Hour, _timeAndWeatherController.EveningTime.Minute), 
			2 => ConvertTimeToRange(_timeAndWeatherController.NightTime.Hour, _timeAndWeatherController.NightTime.Minute), 
			5 => ConvertTimeToRangeWithoutOffset(TimeProvider.CustomHours, 0f), 
			_ => ConvertTimeToRange(_timeAndWeatherController.CurrentTime.Hour, _timeAndWeatherController.CurrentTime.Minute), 
		};
	}

	private float ConvertTimeToRangeWithoutOffset(float hours, float minutes)
	{
		hours %= 24f;
		return hours + minutes / 60f;
	}

	private float ConvertTimeToRange(float hours, float minutes)
	{
		hours -= -7f;
		hours %= 24f;
		return hours + minutes / 60f;
	}
}

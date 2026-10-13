using System;
using System.Collections.Generic;
using CPMEngine;
using Car_Parking.Scripts.Drag.CarCompositions;
using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking;
using _Car_Parking.Scripts.Audio;

public class ParkTronicControll : MonoBehaviour
{
	private const string BeepEvent = "Play_Car_Parking_Beep";

	private readonly float _closeDelay = 0.2f;

	private readonly float _middleDelay = 0.4f;

	private readonly float _farDelay = 1f;

	private readonly float _closeVolume = 0.6f;

	private readonly float _middleVolume = 0.4f;

	private readonly float _farVolume = 0.1f;

	private readonly float _toMilliseconds = 1000f;

	private readonly int _maxPlayingSounds = 50;

	public List<ParkTronicTrigger> ParkTronicTrigger = new List<ParkTronicTrigger>();

	public bool[] partTronicValues;

	public bool Invoked;

	public int lastI;

	public bool ShowParktronic;

	public Image[] ParkTronicItems2;

	private bool _close;

	private bool _far;

	private bool _middle;

	private CanvasGroup _canvasGroup;

	private CarCameras _carCameras;

	private Powertrain _powertrain;

	private VehicleData _vehicleData;

	private GameObject _soundTarget;

	private IDisposable _playBeep;

	private IDisposable _resetInvoked;

	private CarInputManager _carInputManager;

	private CompositeDisposable _disposables;

	private bool _isBeepPlaying;

	private bool UseParkTronicSound => !FlagsStorage.IsFreeDriveMode;

	[Inject]
	public void Construct(CarInputManager carInputManager)
	{
		_carInputManager = carInputManager;
	}

	private void Awake()
	{
		if (TryGetComponent<CanvasGroup>(out _canvasGroup))
		{
			_canvasGroup.alpha = 0f;
		}
		ParkTronicTrigger.Clear();
		Image[] parkTronicItems = ParkTronicItems2;
		for (int i = 0; i < parkTronicItems.Length; i++)
		{
			parkTronicItems[i].color = Color.white;
		}
		partTronicValues = new bool[18];
		if (_carInputManager != null)
		{
			_carInputManager.ParkTronicControl = this;
		}
		if ((bool)V.CoreGameManager && V.CoreGameManager.FreeDriveProvider != null && (bool)V.CoreGameManager.FreeDriveProvider.CarCameras)
		{
			_carCameras = V.CoreGameManager.FreeDriveProvider.CarCameras;
		}
	}

	private void OnDestroy()
	{
		if (_carInputManager != null && _carInputManager.ParkTronicControl == this)
		{
			_carInputManager.ParkTronicControl = null;
		}
		_disposables?.Dispose();
	}

	public void ResetValues()
	{
		for (int i = 0; i < partTronicValues.Length; i++)
		{
			partTronicValues[i] = false;
		}
		foreach (ParkTronicTrigger item in ParkTronicTrigger)
		{
			item.RemoveAllSigns();
		}
	}

	private void Start()
	{
		FlagsStorage.ShowParkingSensors = PlayerPrefs.GetInt("parkingSensor", 1) == 1;
	}

	public void SetCar(Powertrain powertrain, VehicleData vehicleData)
	{
		_powertrain = powertrain;
		_vehicleData = vehicleData;
		ParkTronikCompositions parkTronikCompositions = (_vehicleData ? _vehicleData.ParkTronikComposition : null);
		if (!_powertrain || parkTronikCompositions == null)
		{
			DisableParkTronic();
			return;
		}
		ParkTronicTrigger = parkTronikCompositions.ParkTronicTriggers;
		_soundTarget = _powertrain.gameObject;
		if (_vehicleData.isTruck)
		{
			base.gameObject.SetActive(value: false);
			return;
		}
		parkTronikCompositions.TrySetActiveParktronikParent(FlagsStorage.ShowParkingSensors);
		ResetValues();
		HideVisual();
		_disposables?.Dispose();
		_disposables = new CompositeDisposable();
		_powertrain.SpeedMpsReactive.Subscribe(UpdateVisual).AddTo(_disposables);
	}

	private void UpdateVisual(float speed)
	{
		if (!FlagsStorage.ShowParkingSensors)
		{
			return;
		}
		if (speed < 3f)
		{
			CheckParktronic();
			if (_far || _middle || _close)
			{
				ShowVisual();
			}
			else
			{
				HideVisual();
			}
		}
		else
		{
			HideVisual();
		}
	}

	public void DisableParkTronic()
	{
		_disposables?.Dispose();
		HideVisual();
		if ((bool)_vehicleData && _vehicleData.ParkTronikComposition != null)
		{
			_vehicleData.ParkTronikComposition.TrySetActiveParktronikParent(active: false);
		}
		_powertrain = null;
	}

	private void CheckParktronic()
	{
		int gear = _powertrain.Gear;
		if (gear <= 0)
		{
			if (gear == -1)
			{
				_close = partTronicValues[9] || partTronicValues[12] || partTronicValues[15];
				_middle = partTronicValues[10] || partTronicValues[13] || partTronicValues[16];
				_far = partTronicValues[11] || partTronicValues[14] || partTronicValues[17];
			}
			else
			{
				_close = false;
				_middle = false;
				_far = false;
			}
		}
		else
		{
			_close = partTronicValues[0] || partTronicValues[3] || partTronicValues[6];
			_middle = partTronicValues[1] || partTronicValues[4] || partTronicValues[7];
			_far = partTronicValues[2] || partTronicValues[5] || partTronicValues[8];
		}
	}

	private void Update()
	{
		if (FlagsStorage.ShowParkingSensors && (bool)_powertrain && ShowParktronic && !FlagsStorage.RacingGameMode && !FlagsStorage.OutOfCar)
		{
			Vector3 vector = _powertrain.transform.InverseTransformPoint(V.CoreGameManager.gameObject.transform.position);
			float num = Mathf.Atan2(vector.x, vector.z) * 57.29578f;
			if ((bool)_carCameras && _carCameras.driverView)
			{
				num = -180f;
			}
			base.transform.localRotation = Quaternion.Euler(0f, 0f, num + 180f);
			if (_close)
			{
				PlaySound(0);
			}
			else if (_middle)
			{
				PlaySound(1);
			}
			else if (_far)
			{
				PlaySound(2);
			}
			else
			{
				PlaySound(3);
			}
		}
	}

	public void HideVisual()
	{
		if (ShowParktronic)
		{
			ShowParktronic = false;
			_canvasGroup.DOFade(0f, 0.5f);
			PlaySound(3);
		}
	}

	public void ShowVisual()
	{
		if (!ShowParktronic)
		{
			ShowParktronic = true;
			_canvasGroup.DOFade(1f, 0.5f);
		}
	}

	public void PlaySound(int i)
	{
		if (!UseParkTronicSound)
		{
			return;
		}
		if (lastI != i)
		{
			_playBeep?.Dispose();
			_resetInvoked?.Dispose();
			Invoked = false;
			lastI = i;
		}
		switch (i)
		{
		case 0:
			if (!Invoked)
			{
				Invoked = true;
				PlayAfter(_closeDelay);
				SetVolume("Play_Car_Parking_Beep", _closeVolume);
			}
			break;
		case 1:
			if (!Invoked)
			{
				Invoked = true;
				PlayAfter(_middleDelay);
				SetVolume("Play_Car_Parking_Beep", _middleVolume);
			}
			break;
		case 2:
			if (!Invoked)
			{
				Invoked = true;
				PlayAfter(_farDelay);
				SetVolume("Play_Car_Parking_Beep", _farVolume);
			}
			break;
		case 3:
			Invoked = false;
			SetVolume("Play_Car_Parking_Beep", 0f);
			break;
		}
	}

	private void PlayAfter(float t)
	{
		_resetInvoked?.Dispose();
		_playBeep = Observable.Timer(TimeSpan.FromMilliseconds(t * _toMilliseconds)).Subscribe((long _) =>
		{
			PlayBeep();
		}).AddTo(this);
		_resetInvoked = Observable.Timer(TimeSpan.FromMilliseconds((t + 1f) * _toMilliseconds)).Subscribe((long _) =>
		{
			ResetInvoked();
		}).AddTo(this);
	}

	private void ResetInvoked()
	{
		Invoked = false;
	}

	private void PlayBeep()
	{
		if (UseParkTronicSound && (bool)_soundTarget && !_isBeepPlaying)
		{
			AudioWrapper.PostEvent("Play_Car_Parking_Beep", _soundTarget, 1u, CallBackBeep, base.gameObject);
			_isBeepPlaying = true;
		}
	}

	private void CallBackBeep(object in_cookie, AkCallbackType callType, object in_info)
	{
		if (callType == AkCallbackType.AK_EndOfEvent)
		{
			_isBeepPlaying = false;
		}
	}

	private void SetVolume(string eventName, float volume)
	{
		AudioWrapper.SetRTPCValue(AudioWrapper.GetId(eventName), volume * 100f);
	}
}

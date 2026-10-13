using System;
using CPMEngine;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using _Car_Parking.Scripts.Audio;
using _Car_Parking.Scripts.Audio.SoundBanks;

[RequireComponent(typeof(Animator))]
public class SpoilerSystem : MonoBehaviour
{
	public SpoilerType Type;

	[SerializeField]
	private bool IsOpen;

	[SerializeField]
	private bool IsBraking;

	[SerializeField]
	private string SpoilerUpSound = "Play_Car_Wings_Up";

	[SerializeField]
	private string SpoilerDownSound = "Play_Car_Wings_Close";

	[SerializeField]
	private string SpoilerBrakeOpenSound;

	[SerializeField]
	private string SpoilerBrakeCloseSound;

	[SerializeField]
	private bool PlayBrakeSounds;

	public float OpenBrakeSpeed = 60f;

	public float CloseBrakeSpeed = 10f;

	public float OpenSpoilerSpeed = 120f;

	public float CloseSpoilerSpeed = 50f;

	private Animator _animator;

	private static readonly int Open = Animator.StringToHash("Open");

	private static readonly int Close = Animator.StringToHash("Close");

	private static readonly int Brake = Animator.StringToHash("isBarking");

	private Axles _axles;

	private Powertrain _powertrain;

	private MultiPlayerPosControl _mppc;

	private bool _isInit;

	private IDisposable _brakeSub;

	private IDisposable _speedSub;

	private bool _isPlayer;

	private SoundBankLoadService _soundBankService;

	private void Awake()
	{
		TryGetComponent<Animator>(out _animator);
		if ((bool)_animator)
		{
			_animator.fireEvents = false;
		}
	}

	public void SetSound(bool isPlayer)
	{
		_isPlayer = isPlayer;
	}

	public void Init(Axles axles, Powertrain powertrain)
	{
		_axles = axles;
		_powertrain = powertrain;
		_powertrain.TryGetComponent<MultiPlayerPosControl>(out _mppc);
		_isInit = true;
		if ((bool)_powertrain)
		{
			_speedSub?.Dispose();
			_speedSub = _powertrain.OnSpeedChangedStream().Subscribe(HandleSpeedChanged).AddTo(this);
		}
		if ((bool)_axles)
		{
			_brakeSub?.Dispose();
			_brakeSub = _axles.IsBrakePressed.Subscribe(HandleBrakeChanged).AddTo(this);
		}
		if (_powertrain.gameObject == V.Car)
		{
			_isPlayer = true;
		}
		_powertrain.TryGetComponent<VehicleData>(out var component);
		_soundBankService = (component ? component.SoundBankService : null);
		if (Type != SpoilerType.Static)
		{
			_soundBankService?.TryLoadBank(SoundBankType.CarWings);
		}
	}

	private void OnDestroy()
	{
		if (Type != SpoilerType.Static)
		{
			_soundBankService?.TryUnloadBank(SoundBankType.CarWings);
		}
	}

	private void OnEnable()
	{
		if (_isInit)
		{
			if ((bool)_powertrain)
			{
				_speedSub?.Dispose();
				_speedSub = _powertrain.OnSpeedChangedStream().Subscribe(HandleSpeedChanged).AddTo(this);
			}
			if ((bool)_axles)
			{
				_brakeSub?.Dispose();
				_brakeSub = _axles.IsBrakePressed.Subscribe(HandleBrakeChanged).AddTo(this);
			}
		}
	}

	private void OnDisable()
	{
		_speedSub?.Dispose();
		_brakeSub?.Dispose();
		IsOpen = false;
		IsBraking = false;
		if ((bool)_animator)
		{
			_animator.ResetTrigger(Open);
			_animator.ResetTrigger(Close);
			_animator.SetBool(Brake, value: false);
		}
	}

	private void HandleSpeedChanged(float speed)
	{
		switch (Type)
		{
		case SpoilerType.Accelerated:
			UpdateSpoiler(speed, CloseSpoilerSpeed, OpenSpoilerSpeed);
			break;
		case SpoilerType.Brake:
			if (!_axles.IsBrakePressed.Value)
			{
				UpdateSpoiler(speed, CloseSpoilerSpeed, OpenSpoilerSpeed);
			}
			break;
		case SpoilerType.Static:
			break;
		}
	}

	private void HandleBrakeChanged(bool pressed)
	{
		if (Type != SpoilerType.Brake)
		{
			return;
		}
		float num = (_powertrain.OnLineCar ? Utils.Mps2Kmh(_powertrain.SpeedMpsOnline) : _powertrain.SpeedKmh);
		if (pressed)
		{
			if (num > OpenBrakeSpeed)
			{
				SpoilerBrakeOpen();
			}
			else if (num < CloseBrakeSpeed)
			{
				SpoilerBrakeClose();
			}
		}
		else
		{
			SpoilerBrakeClose();
		}
	}

	private void UpdateSpoiler(float currentSpeed, float minSpeed, float maxSpeed)
	{
		if (!IsBraking)
		{
			if (currentSpeed > maxSpeed && !IsOpen)
			{
				SpoilerOpen();
			}
			else if (currentSpeed < minSpeed && IsOpen)
			{
				SpoilerClose();
			}
		}
	}

	public void UpdateSpoilerOnEnable()
	{
		float num = (_powertrain.OnLineCar ? Utils.Mps2Kmh(_powertrain.SpeedMpsOnline) : _powertrain.SpeedKmh);
		switch (Type)
		{
		case SpoilerType.Accelerated:
			UpdateSpoiler(num, CloseSpoilerSpeed, OpenSpoilerSpeed);
			break;
		case SpoilerType.Brake:
			if (_axles.IsBrakePressed.Value)
			{
				if (num > OpenBrakeSpeed)
				{
					SpoilerBrakeOpen();
				}
				else if (num < CloseBrakeSpeed)
				{
					SpoilerBrakeClose();
				}
			}
			else
			{
				SpoilerBrakeClose();
				UpdateSpoiler(num, CloseSpoilerSpeed, OpenSpoilerSpeed);
			}
			break;
		}
	}

	public void SpoilerBrakeOpen()
	{
		if (!IsBraking && (bool)_animator)
		{
			IsBraking = true;
			_animator.SetBool(Brake, IsBraking);
			IsOpen = false;
			if (AudioWrapper.IsInitialized() && PlayBrakeSounds && !SpoilerBrakeOpenSound.IsNullOrEmpty())
			{
				_soundBankService?.PostEvent(SpoilerBrakeOpenSound, SoundBankType.CarWings, base.gameObject).Forget();
			}
		}
	}

	public void SpoilerBrakeClose()
	{
		if (IsBraking && (bool)_animator)
		{
			IsBraking = false;
			_animator.SetBool(Brake, IsBraking);
			IsOpen = false;
			if (AudioWrapper.IsInitialized() && PlayBrakeSounds && !SpoilerBrakeCloseSound.IsNullOrEmpty())
			{
				_soundBankService?.PostEvent(SpoilerBrakeCloseSound, SoundBankType.CarWings, base.gameObject).Forget();
			}
		}
	}

	public void SpoilerOpen()
	{
		SpoilerBrakeClose();
		if (!IsOpen && (bool)_animator)
		{
			_animator.ResetTrigger(Close);
			_animator.SetTrigger(Open);
			IsOpen = true;
			if (_isPlayer && AudioWrapper.IsInitialized())
			{
				_soundBankService?.PostEvent(SpoilerUpSound, SoundBankType.CarWings, base.gameObject).Forget();
			}
		}
	}

	public void SpoilerClose()
	{
		if (IsOpen && (bool)_animator)
		{
			_animator.SetTrigger(Close);
			IsOpen = false;
			if (_isPlayer && AudioWrapper.IsInitialized())
			{
				_soundBankService?.PostEvent(SpoilerDownSound, SoundBankType.CarWings, base.gameObject).Forget();
			}
		}
	}
}

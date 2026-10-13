using CPMEngine;
using UnityEngine;
using _Car_Parking;
using _Car_Parking.Scripts.Audio;
using _Car_Parking.Scripts.Car.Sounds;

[RequireComponent(typeof(WheelProvider))]
public class WheelSound : MonoBehaviour
{
	private const float HiSpeed = 300f;

	private const float MidSpeed = 100f;

	private const float LowSpeed = 10f;

	private const float WheelCrashHeight = 0.02f;

	private const float MinCarSpeedForAnySkidSoundMps = 1f;

	private static readonly uint PlayWheelsSurface = AudioWrapper.GetId("Play_Wheels_Surface");

	private static readonly uint StopWheelsSurface = AudioWrapper.GetId("Stop_Wheels_Surface");

	private static readonly uint AsphaltSurface = AudioWrapper.GetId("Asphalt");

	private static readonly uint OffRoadSurface = AudioWrapper.GetId("Offroad");

	private static readonly uint SnowSurface = AudioWrapper.GetId("Snow");

	private static readonly uint KerbSurface = AudioWrapper.GetId("Kerb");

	private static readonly uint ConcreteSurface = AudioWrapper.GetId("Concrete");

	private static readonly uint GravelSurface = AudioWrapper.GetId("Gravel");

	private static readonly uint WoodenSurface = AudioWrapper.GetId("Wooden");

	private static readonly uint WetAsphaltSurface = AudioWrapper.GetId("Wet_Asphalt");

	private static readonly uint WetOffRoadDirtSurface = AudioWrapper.GetId("Wet_Offroad_Dirt");

	private static readonly uint WheelSurfaceSwitch = AudioWrapper.GetId("Wheels_Surface");

	private static readonly uint StuddedTire = AudioWrapper.GetId("Studded_Tire");

	private static readonly uint RtpcWheelsSpeed = AudioWrapper.GetId("RTPC_Wheels_Speed");

	private static readonly uint RtpcSkids = AudioWrapper.GetId("RTPC_Skids");

	private static readonly uint RtpcSkidsPitch = AudioWrapper.GetId("RTPC_Skids_Pitch");

	private static readonly uint PlaySkidsSurface = AudioWrapper.GetId("Play_SkidsSurface");

	private static readonly uint StopSkidsSurface = AudioWrapper.GetId("Stop_SkidsSurface");

	private static readonly uint StuddedDrift = AudioWrapper.GetId("StuddedDrift");

	private static readonly uint DriftSurfaceSwitch = AudioWrapper.GetId("Drift_Surface_Switch");

	private static readonly uint DirtDrift = AudioWrapper.GetId("DirtDrift");

	private static readonly uint SnowDrift = AudioWrapper.GetId("SnowDrift");

	private static readonly uint AsphaltDrift = AudioWrapper.GetId("AsphaltDrift");

	private static readonly uint OffroadDrift = AudioWrapper.GetId("OffroadDrift");

	private static readonly uint GravelDrift = AudioWrapper.GetId("GravelDrift");

	private static readonly uint WoodenDrift = AudioWrapper.GetId("WoodenDrift");

	private static readonly uint WetAsphaltDrift = AudioWrapper.GetId("WetAsphaltDrift");

	private static readonly uint PlayPuddleSplash = AudioWrapper.GetId("Play_Puddle_Splash");

	private static readonly uint StopPuddleSplash = AudioWrapper.GetId("Stop_Puddle_Splash");

	private static readonly uint PlayWheelHitSwitch = AudioWrapper.GetId("Play_Wheel_hit_switch");

	[SerializeField]
	[Range(0f, 1f)]
	[Header("Sound Settings")]
	private float SkidVolumeMultiplier = 1f;

	[Range(0f, 1f)]
	[SerializeField]
	private float RollVolumeMultiplier = 0.025f;

	[Range(0.1f, 2f)]
	[SerializeField]
	private float UpdateInterval = 0.1f;

	private Powertrain _powertrain;

	private WheelProvider _wheel;

	private VehicleData _vehicleData;

	private SoundController _soundController;

	private IWheel _wheelInterface;

	private const float SkidPitchMaxSlipMps = 30f;

	private const float PuddleEnterK = 0.02f;

	private const float PuddleExitK = 0.015f;

	private const float PuddleMinSpeedMps = 1f;

	private Surface _currentSurface;

	private bool _currentWet;

	private float _rollVolume;

	private float _skidVolume;

	private float _lastRollVolume;

	private float _lastSkidVolume;

	private float _lastSkidPitch;

	private float _updateTimer;

	private bool _isSurfacePlaying;

	private bool _isSkidPlaying;

	private bool _isPuddlePlaying;

	private bool _skidSoundPlayable = true;

	private float _prevSpringLength;

	private float _skidTimer;

	public void Init(Powertrain powertrain, VehicleData vehicleData)
	{
		_powertrain = powertrain;
		_vehicleData = vehicleData;
		_powertrain.TryGetComponent<SoundController>(out _soundController);
	}

	private void Awake()
	{
		TryGetComponent<WheelProvider>(out _wheel);
		if (!_wheel)
		{
			Debug.LogError("WheelSound: Wheel component is missing", this);
			base.enabled = false;
		}
	}

	private void Start()
	{
		if ((bool)_wheel)
		{
			_wheelInterface = _wheel.Wheel;
		}
	}

	private void OnEnable()
	{
		_wheel.OnUpdateWheelEvent += HandleOnUpdateWheelEvent;
	}

	private void OnDisable()
	{
		_wheel.OnUpdateWheelEvent -= HandleOnUpdateWheelEvent;
		StopAllSounds();
	}

	private void OnDestroy()
	{
		if ((bool)_wheel)
		{
			_wheel.OnUpdateWheelEvent -= HandleOnUpdateWheelEvent;
		}
		_wheel = null;
		_wheelInterface = null;
		_powertrain = null;
		_vehicleData = null;
		_soundController = null;
	}

	private void StopAllSounds()
	{
		if (AudioWrapper.IsInitialized() && (bool)base.gameObject)
		{
			StopWheelSounds();
		}
	}

	private void HandleOnUpdateWheelEvent(WheelProvider wheel)
	{
		if (!AudioWrapper.IsInitialized() || !wheel || !wheel.Wheel.Enabled || _wheel.WheelSurface == Surface.None || !_soundController)
		{
			_updateTimer += Time.unscaledDeltaTime;
			if (_updateTimer >= UpdateInterval && Random.value > 0.99f)
			{
				_updateTimer = 0f;
			}
			_skidTimer = Mathf.Repeat(_skidTimer + Time.deltaTime, 0.15f);
			StopAllSounds();
		}
		else
		{
			if (!_soundController.WheelBankReady)
			{
				return;
			}
			_updateTimer += Time.unscaledDeltaTime;
			if (!(_updateTimer < UpdateInterval))
			{
				_updateTimer -= UpdateInterval;
				SkidsSounds();
				if (!_powertrain || !_powertrain.OnLineCar)
				{
					RollSounds();
					CrashSound();
					UpdatePuddleSound();
				}
			}
		}
	}

	private void UpdatePuddleSound()
	{
		bool flag = false;
		if (RainWetnessDriver.HasActiveDriver && IsPuddleSurface(_wheel.WheelSurface) && _wheelInterface.OnGroundDown && Mathf.Abs(_wheelInterface.WheelTireVelocity) > 1f)
		{
			flag = RainWetnessDriver.SamplePuddleAtCached(base.transform.position) >= (_isPuddlePlaying ? 0.015f : 0.02f);
		}
		if (flag != _isPuddlePlaying)
		{
			AudioWrapper.PostEvent(flag ? PlayPuddleSplash : StopPuddleSplash, base.gameObject);
			_isPuddlePlaying = flag;
		}
	}

	private static bool IsPuddleSurface(Surface surface)
	{
		switch (surface)
		{
		case Surface.Dryasphalt:
		case Surface.Wetasphalt:
		case Surface.Damp:
		case Surface.Indoor:
		case Surface.Wood:
		case Surface.Kerb:
		case Surface.Concrete:
			return true;
		default:
			return false;
		}
	}

	private void SkidsSounds()
	{
		UpdateSkidSounds();
		UpdateSurfaceSwitch();
	}

	private void RollSounds()
	{
		if (!_vehicleData || !_vehicleData.IsRacingCar)
		{
			UpdateRollSound();
			UpdateRollVolume();
		}
	}

	private void UpdateSurfaceSwitch()
	{
		if (_wheel.WheelSurface == Surface.None)
		{
			StopWheelSounds();
			return;
		}
		bool isWetForEffects = WeatherPhysics.IsWetForEffects;
		if (_currentSurface == _wheel.WheelSurface && _currentWet == isWetForEffects)
		{
			return;
		}
		_currentSurface = _wheel.WheelSurface;
		_currentWet = isWetForEffects;
		switch (_currentSurface)
		{
		case Surface.OutOfRoad:
		case Surface.Damp:
		case Surface.DirtRoad:
		case Surface.Offroad:
		case Surface.Grass:
		case Surface.Sand:
			SetSurfaceSwitches(isWetForEffects ? WetOffRoadDirtSurface : OffRoadSurface, isWetForEffects ? DirtDrift : OffroadDrift);
			return;
		case Surface.Gravel:
			SetSurfaceSwitches(isWetForEffects ? WetOffRoadDirtSurface : GravelSurface, isWetForEffects ? DirtDrift : GravelDrift);
			return;
		case Surface.Wood:
			SetSurfaceSwitches(WoodenSurface, WoodenDrift);
			return;
		case Surface.Snow:
		case Surface.Winterroad:
		case Surface.Ice:
			SetSurfaceSwitches(SnowSurface, SnowDrift);
			return;
		case Surface.Kerb:
			AudioWrapper.SetSwitch(WheelSurfaceSwitch, KerbSurface, base.gameObject);
			return;
		case Surface.Concrete:
			SetSurfaceSwitches(ConcreteSurface, AsphaltDrift);
			return;
		case Surface.Wetasphalt:
			SetSurfaceSwitches(WetAsphaltSurface, WetAsphaltDrift);
			return;
		case Surface.None:
			StopWheelSounds();
			return;
		}
		if (_wheel.TyreType == TyreType.Winter)
		{
			SetSurfaceSwitches(StuddedTire, StuddedDrift);
		}
		else
		{
			SetSurfaceSwitches(isWetForEffects ? WetAsphaltSurface : AsphaltSurface, isWetForEffects ? WetAsphaltDrift : AsphaltDrift);
		}
	}

	private void SetSurfaceSwitches(uint surfaceSwitch, uint driftSwitch)
	{
		AudioWrapper.SetSwitch(WheelSurfaceSwitch, surfaceSwitch, base.gameObject);
		AudioWrapper.SetSwitch(DriftSurfaceSwitch, driftSwitch, base.gameObject);
	}

	private void UpdateRollSound()
	{
		bool flag = _currentSurface != Surface.None && Mathf.Abs(_wheelInterface.WheelTireVelocity) >= 1f;
		if (flag != _isSurfacePlaying)
		{
			if (flag)
			{
				AudioWrapper.PostEvent(PlayWheelsSurface, base.gameObject);
			}
			else
			{
				AudioWrapper.PostEvent(StopWheelsSurface, base.gameObject);
				AudioWrapper.ResetRTPCValue(RtpcWheelsSpeed, base.gameObject);
			}
			_isSurfacePlaying = flag;
		}
	}

	private void UpdateRollVolume()
	{
		float num = (_powertrain.OnLineCar ? _powertrain.SpeedMpsOnline : _powertrain.SpeedMps);
		_rollVolume = Mathf.Clamp(num * RollVolumeMultiplier, 0f, 1f);
		if (!Mathf.Approximately(_lastRollVolume, _rollVolume))
		{
			AudioWrapper.SetRTPCValue(RtpcWheelsSpeed, _rollVolume, base.gameObject);
			_lastRollVolume = _rollVolume;
		}
	}

	private void UpdateSkidSounds()
	{
		if ((!_vehicleData || !_vehicleData.Towing) && _skidSoundPlayable)
		{
			_skidVolume = CalculateSkidVolume();
			UpdateSkidVolume();
			CheckSkidPlaying();
		}
	}

	private float CalculateSkidVolume()
	{
		if (FlagsStorage.IsCarInService || FlagsStorage.DynorunMode)
		{
			return 0f;
		}
		if (!_powertrain || !_wheel)
		{
			return 0f;
		}
		if (_powertrain.OnLineCar)
		{
			if (!_wheel.IsSkidmarkOnline)
			{
				return 0f;
			}
			return Mathf.Clamp(_wheel.SmokeTimeMultiplayerCar / 8f, 0f, 0.8f);
		}
		if (Mathf.Abs(_wheelInterface.WheelTireVelocity) < 1f && _powertrain.SpeedMps < 1f)
		{
			return 0f;
		}
		float num = _skidTimer * 3.3333f + 0.6f;
		float getSkidVolume = _wheelInterface.GetSkidVolume;
		if (!(getSkidVolume <= 0f))
		{
			return Mathf.Clamp01(getSkidVolume * num);
		}
		return 0f;
	}

	private void CheckSkidPlaying()
	{
		if ((bool)_vehicleData && _vehicleData.IsRacingCar)
		{
			if (_wheel.Wheel.GetSkidVolume > 0.5f)
			{
				_soundController.PlaySkidSound(PlaySkidsSurface, _wheel);
			}
			else
			{
				_soundController.StopSkidSound(StopSkidsSurface, _wheel);
			}
			return;
		}
		bool flag = _skidVolume > 0.05f;
		if (flag != _isSkidPlaying)
		{
			AudioWrapper.PostEvent(flag ? PlaySkidsSurface : StopSkidsSurface, base.gameObject);
			_isSkidPlaying = flag;
		}
	}

	private void UpdateSkidVolume()
	{
		GameObject in_gameObjectID = (((bool)_vehicleData && _vehicleData.IsRacingCar) ? _soundController.gameObject : base.gameObject);
		if (!Mathf.Approximately(_lastSkidVolume, _skidVolume))
		{
			AudioWrapper.SetRTPCValue(RtpcSkids, _skidVolume, in_gameObjectID);
			_lastSkidVolume = _skidVolume;
		}
		float num = CalculateSkidPitch();
		if (!Mathf.Approximately(_lastSkidPitch, num))
		{
			AudioWrapper.SetRTPCValue(RtpcSkidsPitch, num, in_gameObjectID);
			_lastSkidPitch = num;
		}
	}

	private float CalculateSkidPitch()
	{
		if ((bool)_powertrain && _powertrain.OnLineCar)
		{
			return _skidVolume;
		}
		float num = _wheelInterface.WheelTireVelocity - _wheelInterface.ForwardVelocity;
		float sideVelocity = _wheelInterface.SideVelocity;
		return Mathf.Clamp01(Mathf.Sqrt(num * num + sideVelocity * sideVelocity) / 30f);
	}

	public void TurnOnSkidSound(bool state)
	{
		_skidSoundPlayable = state;
		if (!state)
		{
			if ((bool)_vehicleData && _vehicleData.IsRacingCar)
			{
				_soundController.StopSkidSound(StopSkidsSurface, _wheel);
				AudioWrapper.ResetRTPCValue(RtpcSkids, _soundController?.gameObject);
				AudioWrapper.ResetRTPCValue(RtpcSkidsPitch, _soundController?.gameObject);
			}
			else if (_isSkidPlaying)
			{
				AudioWrapper.PostEvent(StopSkidsSurface, base.gameObject);
				AudioWrapper.ResetRTPCValue(RtpcSkids, base.gameObject);
				AudioWrapper.ResetRTPCValue(RtpcSkidsPitch, base.gameObject);
				_isSkidPlaying = false;
			}
		}
	}

	public void CrashSound()
	{
		if (_wheel.Wheel.CurSpringLen - _prevSpringLength > 0.02f)
		{
			float speedKmh = _powertrain.SpeedKmh;
			if (speedKmh > 100f)
			{
				if (speedKmh > 300f)
				{
					AudioWrapper.SetSwitch(SoundController.CarCrash, SoundController.HiSpeedCrush, base.gameObject);
				}
				else
				{
					AudioWrapper.SetSwitch(SoundController.CarCrash, SoundController.MidSpeedCrush, base.gameObject);
				}
			}
			else
			{
				if (!(speedKmh > 10f))
				{
					return;
				}
				AudioWrapper.SetSwitch(SoundController.CarCrash, SoundController.LowSpeedCrush, base.gameObject);
			}
			AudioWrapper.PostEvent(PlayWheelHitSwitch, base.gameObject);
		}
		_prevSpringLength = _wheel.Wheel.CurSpringLen;
	}

	private void StopWheelSounds()
	{
		if ((bool)_vehicleData && _vehicleData.IsRacingCar)
		{
			_soundController.StopSkidSound(StopSkidsSurface, _wheel);
		}
		if (_isSurfacePlaying)
		{
			AudioWrapper.PostEvent(StopWheelsSurface, base.gameObject);
			AudioWrapper.ResetRTPCValue(RtpcWheelsSpeed, base.gameObject);
			_isSurfacePlaying = false;
		}
		if (_isPuddlePlaying)
		{
			AudioWrapper.PostEvent(StopPuddleSplash, base.gameObject);
			_isPuddlePlaying = false;
		}
		if (_isSkidPlaying)
		{
			AudioWrapper.PostEvent(StopSkidsSurface, base.gameObject);
			AudioWrapper.ResetRTPCValue(RtpcSkids, base.gameObject);
			AudioWrapper.ResetRTPCValue(RtpcSkidsPitch, base.gameObject);
			_isSkidPlaying = false;
		}
	}
}

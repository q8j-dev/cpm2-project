using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CPMEngine;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using VContainer;
using _Car_Parking.Scripts.Audio;
using _Car_Parking.Scripts.Audio.SoundBanks;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Other.Helpers;

namespace _Car_Parking.Scripts.Car.Sounds
{
	public class SoundController : MonoBehaviour
	{
		private class CrashSoundData
		{
			private const float TimeToRemoveCollision = 0.1f;

			public float TimeCollision;

			public Collider Collider;

			public bool TimeCollisionIsOver => Time.time - TimeCollision > 0.1f;

			public CrashSoundData(Collider collider)
			{
				Collider = collider;
				TimeCollision = Time.time;
			}
		}

		private const float MaxSpeed = 150f;

		private const float WindGraceSeconds = 1.5f;

		private const float TeleportSpeedFactor = 2f;

		private const float TeleportSpeedSlackKmh = 50f;

		private static readonly uint PlayCarBodyTrans = AudioWrapper.GetId("Play_Carbody_Trans");

		private static readonly uint AirSuspensionDown = AudioWrapper.GetId("Play_Air_Susp_Sequence");

		private static readonly uint AirSuspensionStop = AudioWrapper.GetId("Stop_Air_Susp_Sequence");

		private static readonly uint RtpcAirSuspension = AudioWrapper.GetId("RTPC_Air_Release");

		private static readonly uint PlayLockOpen = AudioWrapper.GetId("Play_lockopen");

		private static readonly uint PlayLockClose = AudioWrapper.GetId("Play_lockClose");

		private static readonly uint PlayCarLockBeep = AudioWrapper.GetId("Play_Car_Lock_Beep");

		private static readonly uint PlayCarUnlockBeep = AudioWrapper.GetId("Play_Car_UnLock_Beep");

		private static readonly uint PlayDoor = AudioWrapper.GetId("Play_Car_Door_Close");

		public static readonly uint CarCrash = AudioWrapper.GetId("CarCrash");

		private static readonly uint StopScratches = AudioWrapper.GetId("Stop_Scratches");

		private static readonly uint PlayScratches = AudioWrapper.GetId("Play_Scratches");

		private static readonly uint RtpcScratch = AudioWrapper.GetId("RTPC_Scratch");

		public static readonly uint PlayCarCrashExtrerior = AudioWrapper.GetId("Play_Car_crash_exterior");

		private static readonly uint PlayCarCrashInterior = AudioWrapper.GetId("Play_Car_crash_interior");

		public static readonly uint PlayHornsSwitch = AudioWrapper.GetId("Play_Horns_Switch");

		private static readonly uint StopHornsSwitch = AudioWrapper.GetId("Stop_Horns_Switch");

		private static readonly uint GrassEvent = AudioWrapper.GetId("Play_grass_loop");

		private static readonly uint GrassStopEvent = AudioWrapper.GetId("Stop_grass_loop");

		private static readonly uint GrassVolume = AudioWrapper.GetId("RTPC_Grass_Play");

		private static readonly uint PlayAbs = AudioWrapper.GetId("Play_ABS");

		private static readonly uint PlayShifts = AudioWrapper.GetId("Play_shifts");

		private static readonly uint PlayBrakes = AudioWrapper.GetId("Play_brakes");

		private static readonly uint StopBrakes = AudioWrapper.GetId("Stop_brakes");

		private static readonly uint RtpcBrakes = AudioWrapper.GetId("RTPC_Brake_Volume");

		public static readonly uint PlayOpenUpstairsDoor = AudioWrapper.GetId("Play_open_door_prison_upstairs");

		public static readonly uint PlayCloseUpstairsDoor = AudioWrapper.GetId("Play_close_door_prison_upstairs");

		public static readonly uint HiSpeedCrush = AudioWrapper.GetId("HiSpeedCrush");

		public static readonly uint MidSpeedCrush = AudioWrapper.GetId("MidSpeedCrush");

		public static readonly uint LowSpeedCrush = AudioWrapper.GetId("LowSpeedCrush");

		private static readonly uint PlayTruckBrakes = AudioWrapper.GetId("Play_Truck_Brakes");

		private static readonly uint StopTruckBrakes = AudioWrapper.GetId("Stop_Truck_Brakes");

		private static readonly uint TruckBrakesSwitch = AudioWrapper.GetId("Truck_Brakes_Pressure");

		private static readonly uint RtpcBrakesPressure = AudioWrapper.GetId("RTPC_Truck_Brakes_Pressure");

		private static readonly uint AirBrakesLow = AudioWrapper.GetId("low");

		private static readonly uint AirBrakesHi = AudioWrapper.GetId("hi");

		private static readonly string Police = "Police";

		private static readonly string HornsSwitch = "Horns_Switch";

		public static readonly string StateCarInOut = "State_Car_In_Out";

		public static readonly string OutCar = "OutCar";

		public static readonly string InCar = "InCar";

		private static readonly uint PlayWindRideSoundEvent = AudioWrapper.GetId("Play_wind_ride");

		private static readonly uint StopWindRideSoundEvent = AudioWrapper.GetId("Stop_wind_ride");

		private static readonly uint RtpcCarSpeed = AudioWrapper.GetId("RTPC_Car_Speed");

		public EngineSound EngineSound;

		public ExhaustSystem ExhaustSystem;

		public int currentHorn = 24;

		public bool CanPlayCarSounds = true;

		private Powertrain _powertrain;

		private Axles _axles;

		private VehicleData _vehicleData;

		private Camera _mainCamera;

		private LightController _lightController;

		private float _volumeFactor;

		private List<CrashSoundData> _crashSoundDatas = new List<CrashSoundData>();

		private int _burbleType = 1;

		private int _blowOffType;

		private int _superChargerType;

		private float _lastBrakeVolume;

		private int _soundId = -1;

		private int _starterId;

		private bool _exhaustBankHeld;

		private float _brakeVolume;

		private float _steeringOld;

		private bool _hornPlaying;

		private bool _rimScraping;

		private bool _scrapeNoisePlaying;

		private bool _skidsPlaying;

		private bool _isGrassPlaying;

		private bool _crashSoundPlayable = true;

		private bool _isTruckBrakesPlaying;

		private bool _isMufflerShotPlaying;

		private bool _isBrakeSoundPlaying;

		private bool _isSkidPlaying;

		private bool _isPneumaticFrontPlaying;

		private bool _isPneumaticRearPlaying;

		private bool _isFrontBodyTransPlaying;

		private bool _isRearBodyTransPlaying;

		private bool _isPlayingWindRideSound;

		private List<WheelProvider> _skidWheels = new List<WheelProvider>();

		private Vector3 _lastPosition;

		private bool _hasLastWindPosition;

		private float _windSuppressedUntil;

		private CompositeDisposable _disposables;

		private AsyncOperationHandle<GameObject>? _handle;

		private CancellationTokenSource _swapCts;

		private SoundBankLoadService _soundBankService;

		private IPrefabFactory _prefabFactory;

		public bool HasBurble { get; set; }

		public bool HasExhaustShot { get; set; }

		public Action OnExhaustChanged { get; set; }

		public int BurbleType
		{
			get
			{
				return _burbleType;
			}
			set
			{
				_burbleType = Mathf.Clamp(value, 1, 5);
			}
		}

		public int BlowOffType
		{
			get
			{
				return _blowOffType;
			}
			set
			{
				_blowOffType = Mathf.Clamp(value, 0, 5);
			}
		}

		public int SuperChargerType
		{
			get
			{
				return _superChargerType;
			}
			set
			{
				_superChargerType = Mathf.Clamp(value - 1, 0, 3);
			}
		}

		public int StarterId
		{
			get
			{
				return _starterId;
			}
			set
			{
				_starterId = Mathf.Clamp(value, 0, 3);
			}
		}

		public bool WheelBankReady
		{
			get
			{
				if (_soundBankService != null)
				{
					return _soundBankService.IsLoaded(SoundBankType.CarWheels);
				}
				return true;
			}
		}

		public async UniTask SwapEngineSound(int id)
		{
			if (id == -1 || id == _soundId)
			{
				return;
			}
			_soundId = id;
			_swapCts?.Cancel();
			_swapCts?.Dispose();
			_swapCts = new CancellationTokenSource();
			CancellationToken token = _swapCts.Token;
			try
			{
				if (_handle.HasValue && _handle.Value.IsValid())
				{
					Addressables.Release(_handle.Value);
					_handle = null;
				}
				AsyncOperationHandle<GameObject> asyncOperationHandle = Addressables.LoadAssetAsync<GameObject>($"EngineSound_{id}");
				_handle = asyncOperationHandle;
				GameObject obj = await asyncOperationHandle.WithCancellation(token);
				if (!token.IsCancellationRequested && (bool)this)
				{
					if ((bool)EngineSound)
					{
						UnityEngine.Object.Destroy(EngineSound.gameObject);
						EngineSound = null;
					}
					GameObject gameObject = _prefabFactory.Instantiate(obj, base.transform);
					gameObject.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
					EngineSound = gameObject.GetComponent<EngineSound>();
					if ((bool)EngineSound)
					{
						EngineSound.GetPositionGO(out var turboPos, out var exhaustPos);
						EngineSound.RepositionGO(turboPos, exhaustPos);
					}
				}
			}
			catch (OperationCanceledException)
			{
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
		}

		private void PlayBrakeSound(bool isBrake)
		{
			if (isBrake)
			{
				if (!_isBrakeSoundPlaying && !(_powertrain.SpeedKmh < 5f))
				{
					AudioWrapper.PostEvent(PlayBrakes, base.gameObject);
					_isBrakeSoundPlaying = true;
				}
			}
			else
			{
				StopBrakeSound();
			}
		}

		private void StopBrakeSound()
		{
			if (_isBrakeSoundPlaying)
			{
				AudioWrapper.PostEvent(StopBrakes, base.gameObject);
				_isBrakeSoundPlaying = false;
			}
		}

		private void CalcBrakeVolume(bool isPressed)
		{
			if (isPressed)
			{
				float num = (_axles.FrontAxle.LeftWheel ? _axles.FrontAxle.LeftWheel.Wheel.SlipAngleVolume : 0f);
				float num2 = (_axles.FrontAxle.RightWheel ? _axles.FrontAxle.RightWheel.Wheel.SlipAngleVolume : num);
				_brakeVolume = Mathf.Clamp01(_powertrain.Brake * (Mathf.Abs(num + num2) * 0.33f));
			}
			else
			{
				_brakeVolume = 0f;
			}
			if (!Mathf.Approximately(_lastBrakeVolume, _brakeVolume))
			{
				AudioWrapper.SetRTPCValue(RtpcBrakes, _brakeVolume, base.gameObject);
				_lastBrakeVolume = _brakeVolume;
			}
		}

		private void OnDestroy()
		{
			_soundBankService?.TryUnloadBank(SoundBankType.CarWheels).Forget();
			if ((bool)_vehicleData && _vehicleData.isTruck)
			{
				_soundBankService?.TryUnloadBank(SoundBankType.CarTruckBrakes).Forget();
			}
			if (_exhaustBankHeld)
			{
				_exhaustBankHeld = false;
				_soundBankService?.TryUnloadBank(SoundBankType.CarExhaust).Forget();
			}
			_swapCts?.Cancel();
			_swapCts?.Dispose();
			if (_handle.HasValue && _handle.Value.IsValid())
			{
				Addressables.Release(_handle.Value);
			}
			PlayHornSound(isPlay: false);
			if (FlagsStorage.IsMainMenu)
			{
				if ((bool)EngineSound)
				{
					UnityEngine.Object.Destroy(EngineSound);
					EngineSound = null;
				}
				_disposables?.Dispose();
			}
		}

		public void LockCar()
		{
			AudioWrapper.PostEvent(PlayLockClose, base.gameObject);
			AudioWrapper.PostEvent(PlayCarLockBeep, base.gameObject);
		}

		public void UnlockCar()
		{
			AudioWrapper.PostEvent(PlayLockOpen, base.gameObject);
			AudioWrapper.PostEvent(PlayCarUnlockBeep, base.gameObject);
		}

		public void AirBrakes(float vol)
		{
			if (_vehicleData.isTruck)
			{
				if (vol > 0f)
				{
					PlayTruckBrakesSound();
				}
				else
				{
					StopTruckBrakesSound();
				}
				AirBrakesSwitch(vol);
				AudioWrapper.SetRTPCValue(RtpcBrakesPressure, vol, base.gameObject);
			}
		}

		public void PneumaticFrontSuspensionDown(float vol)
		{
			if (AudioWrapper.IsInitialized() && _axles.HasPneumaticSuspension && (bool)EngineSound && (bool)EngineSound.TurboSuperChargerPos)
			{
				if (vol > 0f)
				{
					PlayFrontPneumatic();
				}
				AudioWrapper.SetRTPCValue(RtpcAirSuspension, vol, EngineSound.TurboSuperChargerPos);
			}
		}

		public void PneumaticFrontSuspensionUp()
		{
			StopFrontPneumatic();
		}

		private void PlayFrontPneumatic()
		{
			if (!_isPneumaticFrontPlaying)
			{
				_soundBankService?.PostEvent(AirSuspensionDown, SoundBankType.CarAirSuspension, EngineSound.TurboSuperChargerPos).Forget();
			}
			_isPneumaticFrontPlaying = true;
		}

		private void StopFrontPneumatic()
		{
			if (AudioWrapper.IsInitialized() && _axles.HasPneumaticSuspension && (bool)EngineSound && (bool)EngineSound.TurboSuperChargerPos)
			{
				if (_isPneumaticFrontPlaying)
				{
					_soundBankService?.PostEvent(AirSuspensionStop, SoundBankType.CarAirSuspension, EngineSound.TurboSuperChargerPos).Forget();
				}
				_isPneumaticFrontPlaying = false;
				AudioWrapper.SetRTPCValue(RtpcAirSuspension, 0f, EngineSound.TurboSuperChargerPos);
			}
		}

		public void PneumaticRearSuspensionDown(float vol)
		{
			if (AudioWrapper.IsInitialized() && _axles.HasPneumaticSuspension && (bool)EngineSound && (bool)EngineSound.ExhaustSound)
			{
				if (vol > 0f)
				{
					PlayRearPneumatic();
				}
				AudioWrapper.SetRTPCValue(RtpcAirSuspension, vol, EngineSound.ExhaustSound.gameObject);
			}
		}

		public void PneumaticRearSuspensionUp()
		{
			StopRearPneumatic();
		}

		private void PlayRearPneumatic()
		{
			if (!_isPneumaticRearPlaying)
			{
				_soundBankService?.PostEvent(AirSuspensionDown, SoundBankType.CarAirSuspension, EngineSound.ExhaustSound.gameObject).Forget();
			}
			_isPneumaticRearPlaying = true;
		}

		private void StopRearPneumatic()
		{
			if (AudioWrapper.IsInitialized() && _axles.HasPneumaticSuspension && (bool)EngineSound && (bool)EngineSound.ExhaustSound)
			{
				if (_isPneumaticRearPlaying)
				{
					_soundBankService?.PostEvent(AirSuspensionStop, SoundBankType.CarAirSuspension, EngineSound.ExhaustSound.gameObject).Forget();
				}
				_isPneumaticRearPlaying = false;
				AudioWrapper.SetRTPCValue(RtpcAirSuspension, 0f, EngineSound.ExhaustSound.gameObject);
			}
		}

		private void AirBrakesSwitch(float vol)
		{
			if (!(vol <= 0.5f))
			{
				if (vol > 0.5f)
				{
					AudioWrapper.SetSwitch(TruckBrakesSwitch, AirBrakesHi, base.gameObject);
				}
			}
			else
			{
				AudioWrapper.SetSwitch(TruckBrakesSwitch, AirBrakesLow, base.gameObject);
			}
		}

		private void PlayTruckBrakesSound()
		{
			if (!_isTruckBrakesPlaying)
			{
				_soundBankService?.PostEvent(PlayTruckBrakes, SoundBankType.CarTruckBrakes, base.gameObject, 1u, CallBackTruckBrakesSound, base.gameObject).Forget();
			}
			_isTruckBrakesPlaying = true;
		}

		private void CallBackTruckBrakesSound(object in_cookie, AkCallbackType callType, object in_info)
		{
			if (callType == AkCallbackType.AK_EndOfEvent)
			{
				_isTruckBrakesPlaying = false;
			}
		}

		private void StopTruckBrakesSound()
		{
			if (_isTruckBrakesPlaying)
			{
				_soundBankService?.PostEvent(StopTruckBrakes, SoundBankType.CarTruckBrakes, base.gameObject).Forget();
			}
		}

		public void TurnOnSkidSound(bool state)
		{
			foreach (WheelProvider allWheel in _axles.AllWheels)
			{
				allWheel.GetComponent<WheelSound>().TurnOnSkidSound(state);
			}
		}

		private void OnEnable()
		{
			_hasLastWindPosition = false;
			if (FlagsStorage.InGarageMenu)
			{
				CanPlayCarSounds = false;
			}
		}

		private void OnDisable()
		{
			StopAllWwiseSounds();
		}

		public void StopAllWwiseSounds()
		{
			StopGrassAudio();
			StopScrapeSound();
			StopWindSound();
			StopBrakeSound();
			_isTruckBrakesPlaying = false;
			_isMufflerShotPlaying = false;
		}

		[Inject]
		public void Construct(SoundBankLoadService soundBankService, IPrefabFactory prefabFactory)
		{
			_soundBankService = soundBankService;
			_prefabFactory = prefabFactory;
		}

		private void Awake()
		{
			TryGetComponent<Powertrain>(out _powertrain);
			TryGetComponent<Axles>(out _axles);
			TryGetComponent<VehicleData>(out _vehicleData);
			TryGetComponent<LightController>(out _lightController);
			_soundBankService?.TryLoadBank(SoundBankType.CarWheels).Forget();
			if ((bool)_vehicleData && _vehicleData.isTruck)
			{
				_soundBankService?.TryLoadBank(SoundBankType.CarTruckBrakes).Forget();
			}
		}

		public void RefreshExhaustBank()
		{
			bool flag = HasBurble || HasExhaustShot;
			if (flag != _exhaustBankHeld)
			{
				_exhaustBankHeld = flag;
				if (flag)
				{
					_soundBankService?.TryLoadBank(SoundBankType.CarExhaust).Forget();
				}
				else
				{
					_soundBankService?.TryUnloadBank(SoundBankType.CarExhaust).Forget();
				}
			}
		}

		private void Start()
		{
			_disposables = new CompositeDisposable();
		}

		public void Init()
		{
			_disposables?.Dispose();
			_disposables = new CompositeDisposable();
			_powertrain.SpeedMpsReactive.Subscribe(AutoStopScrape).AddTo(_disposables);
			if ((bool)_axles)
			{
				_axles.IsBrakePressed.DistinctUntilChanged().Subscribe(PlayBrakeSound).AddTo(_disposables);
			}
		}

		public void Clear()
		{
			_disposables?.Dispose();
		}

		public void PlaySkidSound(uint source, WheelProvider skidWheel)
		{
			if (!_skidWheels.Any((WheelProvider wheel) => wheel == skidWheel))
			{
				_skidWheels.Add(skidWheel);
				if (!_isSkidPlaying)
				{
					AudioWrapper.PostEvent(source, base.gameObject);
					_isSkidPlaying = true;
				}
			}
		}

		public void StopSkidSound(uint source, WheelProvider skidWheel)
		{
			if (_skidWheels.Count == 0)
			{
				return;
			}
			for (int i = 0; i < _skidWheels.Count; i++)
			{
				if (!(_skidWheels[i] == skidWheel))
				{
					continue;
				}
				_skidWheels.Remove(_skidWheels[i]);
				if (_skidWheels.Count == 0)
				{
					if (!_isSkidPlaying)
					{
						break;
					}
					AudioWrapper.PostEvent(source, base.gameObject);
					_isSkidPlaying = false;
				}
			}
		}

		private void UpdateBodyTransSound()
		{
			if (((bool)_vehicleData && _vehicleData.IsRacingCar) || ((bool)V.Car && V.Car != base.gameObject) || _axles.SuspensionsEditing)
			{
				return;
			}
			int num = 0;
			int num2 = 0;
			foreach (WheelProvider wheel in _axles.FrontAxle.Wheels)
			{
				if (wheel.Wheel.Compression)
				{
					num++;
				}
			}
			foreach (WheelProvider wheel2 in _axles.RearAxle.Wheels)
			{
				if (wheel2.Wheel.Compression)
				{
					num2++;
				}
			}
			bool flag = num > 1;
			bool flag2 = num2 > 1;
			if (flag && !_isFrontBodyTransPlaying)
			{
				AudioWrapper.PostEvent(PlayCarBodyTrans, EngineSound.TurboSuperChargerPos);
			}
			_isFrontBodyTransPlaying = flag;
			if (flag2 && !_isRearBodyTransPlaying)
			{
				AudioWrapper.PostEvent(PlayCarBodyTrans, EngineSound.ExhaustSound.gameObject);
			}
			_isRearBodyTransPlaying = flag2;
		}

		private void AutoStopScrape(float speed)
		{
			if (speed < 0.2f)
			{
				StopScrapeSound();
			}
		}

		private void Update()
		{
			if (!EngineSound || !_powertrain)
			{
				return;
			}
			CarHorn();
			UpdateWinRideSound();
			WhooshSound();
			if (!_vehicleData.IsRacingCar && !_vehicleData.isTraffiCar && !_powertrain.OnLineCar)
			{
				CalcBrakeVolume(_isBrakeSoundPlaying);
				UpdateBodyTransSound();
				if (CarIdStorage.SportCars.Contains(_vehicleData.CarId) && _powertrain.IsShifting && !_powertrain.Automatic)
				{
					AudioWrapper.PostEvent(PlayShifts, base.gameObject);
				}
				if (_powertrain.AbsTrigger)
				{
					AudioWrapper.PostEvent(PlayAbs, base.gameObject);
				}
			}
		}

		private void WhooshSound()
		{
			if (!(V.Car != base.gameObject) && (bool)V.CoreGameManager && _powertrain.SpeedKmh >= 60f)
			{
				V.CoreGameManager.WhooshSoundController.Update(_powertrain.SpeedKmh);
			}
		}

		private void UpdateWinRideSound()
		{
			if (base.gameObject != V.Car)
			{
				_hasLastWindPosition = false;
				if (_isPlayingWindRideSound)
				{
					AudioWrapper.PostEvent(StopWindRideSoundEvent, base.gameObject);
					_isPlayingWindRideSound = false;
				}
			}
			else if (FlagsStorage.InGarageMenu || FlagsStorage.InGarageFree)
			{
				_hasLastWindPosition = false;
				StopWindSound();
			}
			else
			{
				if (Time.timeScale == 0f)
				{
					return;
				}
				float num = Time.deltaTime;
				if (num == 0f)
				{
					num = 1E-06f;
				}
				if (!_hasLastWindPosition)
				{
					_lastPosition = base.transform.position;
					_hasLastWindPosition = true;
					_windSuppressedUntil = Time.time + 1.5f;
					return;
				}
				float num2 = ((base.transform.position - _lastPosition) / num).magnitude * 3.6f;
				_lastPosition = base.transform.position;
				if (num2 > _powertrain.SpeedKmh * 2f + 50f)
				{
					if (!(_powertrain.SpeedKmh > 1f))
					{
						_windSuppressedUntil = Time.time + 1.5f;
						StopWindSound();
					}
					return;
				}
				if (Time.time < _windSuppressedUntil)
				{
					StopWindSound();
					return;
				}
				float in_value = Mathf.Clamp01(num2 / 150f);
				float num3 = num2;
				if (!(num3 <= 1f))
				{
					if (num3 > 1f && !_isPlayingWindRideSound)
					{
						AudioWrapper.PostEvent(PlayWindRideSoundEvent, base.gameObject);
						_isPlayingWindRideSound = true;
					}
				}
				else if (_isPlayingWindRideSound)
				{
					AudioWrapper.PostEvent(StopWindRideSoundEvent, base.gameObject);
					_isPlayingWindRideSound = false;
				}
				if (_isPlayingWindRideSound && _isPlayingWindRideSound)
				{
					AudioWrapper.SetRTPCValue(RtpcCarSpeed, in_value, base.gameObject);
				}
			}
		}

		private void StopWindSound()
		{
			if (_isPlayingWindRideSound)
			{
				AudioWrapper.PostEvent(StopWindRideSoundEvent, base.gameObject);
				_isPlayingWindRideSound = false;
			}
		}

		public void SetGrassVolume(float volume)
		{
			if (_isGrassPlaying)
			{
				AudioWrapper.SetRTPCValue(GrassVolume, 1f - volume);
			}
		}

		private float AverageWheelVelo()
		{
			float num = 0f;
			foreach (WheelProvider allWheel in _axles.AllWheels)
			{
				num += allWheel.Wheel.WheelTireVelocity;
			}
			return num / (float)_axles.AllWheels.Count;
		}

		public bool IsShootTurboPlaying()
		{
			return _isMufflerShotPlaying;
		}

		public void PlayExhaustShootParticle(int type)
		{
			_isMufflerShotPlaying = true;
			switch (type)
			{
			case 0:
				_vehicleData.ExhaustComposition.PlayWeakShootParticle();
				break;
			case 1:
				_vehicleData.ExhaustComposition.PlayStrongShootParticle();
				break;
			}
			_isMufflerShotPlaying = false;
		}

		public void PlayExhaustBurbleParticle()
		{
			_vehicleData.ExhaustComposition.PlayBurbleParticle();
		}

		private void CarHorn()
		{
			if ((bool)_lightController)
			{
				PlayHornSound(_lightController.HState == 3);
			}
		}

		private void PlayHornSound(bool isPlay)
		{
			if (_hornPlaying != isPlay)
			{
				if (isPlay)
				{
					AudioWrapper.SetSwitch(HornsSwitch, _vehicleData.CarInfoProvider.CarInfo.AdditionalData.Police.Installed ? Police : $"horn_{currentHorn}", base.gameObject);
					AudioWrapper.PostEvent(PlayHornsSwitch, base.gameObject);
				}
				else
				{
					AudioWrapper.PostEvent(StopHornsSwitch, base.gameObject);
				}
				_hornPlaying = isPlay;
			}
		}

		public void TurnOnCrashSound(bool state)
		{
			_crashSoundPlayable = state;
		}

		private void OnCollisionEnter(Collision collInfo)
		{
			CarCrashSound(collInfo);
		}

		private void CarCrashSound(Collision collInfo)
		{
			if (!_crashSoundPlayable || collInfo.contacts.Length == 0 || collInfo.contacts[0].thisCollider.gameObject.layer == LayerMask.NameToLayer("Wheel") || collInfo.transform.CompareTag("DriverPerson") || collInfo.transform.CompareTag("Road") || collInfo.transform.TryGetComponent<DogMove>(out var _))
			{
				return;
			}
			_volumeFactor = Mathf.Clamp01(collInfo.relativeVelocity.magnitude * 0.1f);
			_volumeFactor *= Mathf.Clamp01(0.3f + Mathf.Abs(Vector3.Dot(collInfo.relativeVelocity.normalized, collInfo.contacts[0].normal)));
			if (!CanPlayCarSounds)
			{
				return;
			}
			uint eventId = PlayCarCrashExtrerior;
			if (!_vehicleData.IsRacingCar || !_powertrain.OnLineCar)
			{
				if ((bool)V.CoreGameManager.carCamerasController)
				{
					eventId = (V.CoreGameManager.carCamerasController.IsFirstPersonView ? PlayCarCrashInterior : PlayCarCrashExtrerior);
				}
				if (FlagsStorage.RacingGameMode && (bool)_vehicleData.RacingProvider.DragRacingController && (bool)_vehicleData.RacingProvider.DragRacingController.dragRacingCamera)
				{
					eventId = ((_vehicleData.RacingProvider.DragRacingController.dragRacingCamera.currentCamera == DragRacingCamera.CameraType.driverView) ? PlayCarCrashInterior : PlayCarCrashExtrerior);
				}
				if ((bool)V.CoreGameManager && V.CoreGameManager.TryGetComponent<RacingCameraController>(out var component2))
				{
					eventId = ((component2.currentCamera == DragRacingCamera.CameraType.driverView) ? PlayCarCrashInterior : PlayCarCrashExtrerior);
				}
			}
			for (int i = 0; i < _crashSoundDatas.Count; i++)
			{
				if (_crashSoundDatas[i].TimeCollisionIsOver)
				{
					_crashSoundDatas.RemoveAt(i);
					i--;
				}
			}
			for (int j = 0; j < _crashSoundDatas.Count; j++)
			{
				if ((bool)_crashSoundDatas[j].Collider && _crashSoundDatas[j].Collider == collInfo.collider)
				{
					return;
				}
			}
			_crashSoundDatas.Add(new CrashSoundData(collInfo.collider));
			Rigidbody component3 = collInfo.gameObject.GetComponent<Rigidbody>();
			if (!component3)
			{
				if (collInfo.gameObject.CompareTag("NoCrushSound") || collInfo.gameObject.layer == LayerMask.NameToLayer("DriverPerson") || (collInfo.gameObject.layer == LayerMask.NameToLayer("Road") && (FlagsStorage.IsLevelScene || !V.Car)))
				{
					return;
				}
				float volumeFactor = _volumeFactor;
				if (volumeFactor > 0.35f)
				{
					if (volumeFactor > 0.7f)
					{
						AudioWrapper.SetSwitch(CarCrash, HiSpeedCrush, base.gameObject);
					}
					else
					{
						AudioWrapper.SetSwitch(CarCrash, MidSpeedCrush, base.gameObject);
					}
				}
				else
				{
					if (!(volumeFactor > 0.1f))
					{
						return;
					}
					AudioWrapper.SetSwitch(CarCrash, LowSpeedCrush, base.gameObject);
				}
				AudioWrapper.PostEvent(eventId, base.gameObject);
			}
			else if (collInfo.gameObject.layer != LayerMask.NameToLayer("DriverPerson") && collInfo.gameObject.layer != LayerMask.NameToLayer("Road") && !collInfo.gameObject.CompareTag("NoCrushSound") && (bool)V.Car && !component3.isKinematic && CanPlayCarSounds)
			{
				AudioWrapper.SetSwitch(CarCrash, LowSpeedCrush, base.gameObject);
				AudioWrapper.PostEvent(eventId, base.gameObject);
			}
		}

		private void OnCollisionExit()
		{
			StopScrapeSound();
		}

		private void OnCollisionStay(Collision collInfo)
		{
			if ((bool)V.Car && (bool)_powertrain)
			{
				UpdateScrapeSound(collInfo);
			}
		}

		public void PlayDoorSound()
		{
			AudioWrapper.PostEvent(PlayDoor, base.gameObject);
		}

		public void HitLeaves()
		{
			if (!FlagsStorage.IsMainMenu && !_isGrassPlaying)
			{
				AudioWrapper.PostEvent(GrassEvent, base.gameObject);
				_isGrassPlaying = true;
			}
		}

		public void StopGrassAudio()
		{
			if (_isGrassPlaying)
			{
				AudioWrapper.PostEvent(GrassStopEvent, base.gameObject);
				_isGrassPlaying = false;
			}
		}

		private void StopScrapeSound()
		{
			if (_scrapeNoisePlaying)
			{
				AudioWrapper.PostEvent(StopScratches, base.gameObject);
				_scrapeNoisePlaying = false;
			}
		}

		private void UpdateScrapeSound(Collision collInfo)
		{
			if (_powertrain.SpeedMps > 0.3f)
			{
				if (!_scrapeNoisePlaying && !collInfo.gameObject.GetComponent<Rigidbody>() && (FlagsStorage.IsFreeDriveMode || ScenesHelper.IsLapRacingSceneActive()))
				{
					AudioWrapper.PostEvent(PlayScratches, base.gameObject);
					_scrapeNoisePlaying = true;
				}
				if (_scrapeNoisePlaying)
				{
					AudioWrapper.SetRTPCValue(RtpcScratch, _powertrain.SpeedMps * 0.0714f, base.gameObject);
				}
			}
			else
			{
				StopScrapeSound();
			}
		}
	}
}

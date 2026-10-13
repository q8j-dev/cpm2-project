using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using AK.Wwise;
using AK.Wwise.Unity.WwiseAddressables;
using CPM.Transmission;
using CPMEngine;
using Cysharp.Threading.Tasks;
using UniRx;
using Unity.Mathematics;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.Audio;
using _Car_Parking.Scripts.Audio.SoundBanks;
using _Car_Parking.Scripts.Other.Helpers;

namespace _Car_Parking.Scripts.Car.Sounds
{
	[RequireComponent(typeof(AkGameObj))]
	[RequireComponent(typeof(SphereCollider))]
	public class EngineSound : MonoBehaviour
	{
		private sealed class CallbackCookie
		{
			public int InstanceId;

			public int Generation;
		}

		private static readonly uint PlayGearboxShifts = AudioWrapper.GetId("Play_shifts");

		private static readonly uint PlayTransmission = AudioWrapper.GetId("Play_Transmission");

		private static readonly uint StopTransmission = AudioWrapper.GetId("Stop_Transmission");

		private static readonly uint PlayTransferCase = AudioWrapper.GetId("Play_transfer_case");

		private static readonly uint StopTransferCase = AudioWrapper.GetId("Stop_transfer_case");

		private static readonly uint PlayBrokenEngine = AudioWrapper.GetId("Play_Broken_Engine");

		private static readonly uint StopBrokenEngine = AudioWrapper.GetId("Stop_Broken_Engine");

		private static readonly uint RtpcChargerVolume = AudioWrapper.GetId("RTPC_Charger_Volume");

		private static readonly uint PlayTurbo = AudioWrapper.GetId("Play_Rev_Turbo_1");

		private static readonly uint StopTurbo = AudioWrapper.GetId("Stop_Rev_Turbo_1");

		private static readonly uint PlaySuperCharger = AudioWrapper.GetId("Play_SuperCharger_Type");

		private static readonly uint StopSuperCharger = AudioWrapper.GetId("Stop_SuperCharger_Type");

		private static readonly uint SuperchargerSwitch = AudioWrapper.GetId("Supercharger_Type");

		private static readonly uint PlayBlowOffShot = AudioWrapper.GetId("Play_TurboShotSwitch");

		private static readonly uint StopBlowOffShot = AudioWrapper.GetId("Stop_TurboShotSwitch");

		private static readonly uint TurboShootSwitch = AudioWrapper.GetId("Turbo_Type_Switch");

		private static readonly uint VariableStock = AudioWrapper.GetId("Type_0");

		private static readonly uint VariableType1 = AudioWrapper.GetId("Type_1");

		private static readonly uint VariableType2 = AudioWrapper.GetId("Type_2");

		private static readonly uint VariableType3 = AudioWrapper.GetId("Type_3");

		private static readonly uint VariableType4 = AudioWrapper.GetId("Type_4");

		private static readonly uint VariableType5 = AudioWrapper.GetId("Type_5");

		private static readonly uint RtpcBlowOffVolume = AudioWrapper.GetId("RTPC_BlowOff_Volume");

		private static readonly uint RtpcSimulateThrottle = AudioWrapper.GetId("Rtpc_throttle");

		private static readonly uint RtpcSimulateGear = AudioWrapper.GetId("Rtpc_gear");

		private static readonly uint RtpcSimulateShiftGear = AudioWrapper.GetId("Rtpc_shift");

		private static readonly uint RtpcRpm = AudioWrapper.GetId("Rtpc_rpm");

		private static readonly uint RtpcTransmissionSpeed = AudioWrapper.GetId("RTPC_Transmission_Pitch");

		private static readonly uint RtpcTransmissionLoad = AudioWrapper.GetId("RTPC_Transmission_Volume");

		private static readonly uint RtpcDoppler = AudioWrapper.GetId("RTPC_Doppler_Pitch");

		private static readonly uint RtpcUpShiftOnLoad = AudioWrapper.GetId("RTPC_UpShift_Onload");

		private static readonly uint PlayOffRoadEngineStart = AudioWrapper.GetId("Play_Offroad_EngineStart");

		private static readonly uint PlaySedanEngineStart = AudioWrapper.GetId("Play_Sedan_EngineStart");

		private static readonly uint PlaySportCarEngineStart = AudioWrapper.GetId("Play_Sportcar_EngineStart");

		private static readonly uint PlayF1CarEngineStart = AudioWrapper.GetId("Play_Engine_Starting_F1");

		private static readonly uint StopOffRoadEngine = AudioWrapper.GetId("Play_engine_turnoff_offroad");

		private static readonly uint StopSedanEngine = AudioWrapper.GetId("Play_engine_turnoff_sedan");

		private static readonly uint StopSportCarEngine = AudioWrapper.GetId("Play_engine_turnoff_sportcar");

		private static readonly uint StopF1CarEngine = AudioWrapper.GetId("Play_f1_engine_off");

		private const float DopplerCameraTeleportDistance = 4f;

		private const float DopplerSpeedSmoothOnline = 10f;

		private const float DopplerSpeedSmoothOffline = 20f;

		private const float DopplerPitchSmoothOnline = 10f;

		private const float DopplerPitchSmoothOffline = 22f;

		private static readonly uint StopOffRoadEngineStart = AudioWrapper.GetId("Stop_Offroad_EngineStart");

		private static readonly uint StopSedanEngineStart = AudioWrapper.GetId("Stop_Sedan_EngineStart");

		private static readonly uint StopSportCarEngineStart = AudioWrapper.GetId("Stop_Sportcar_EngineStart");

		private static readonly uint StopF1CarEngineStart = AudioWrapper.GetId("Stop_Engine_Starting_F1");

		private uint _turboPlayEvent;

		private uint _turboStopEvent;

		private uint _turboOffShotEvent;

		public string EnginePlayEvent;

		public string EngineStopEvent;

		private uint _enginePlayEvent;

		private uint _engineStopEvent;

		private uint _engineStarterEvent;

		public float Throttle;

		public int Randomizer;

		public Bank EngineBank;

		private static readonly Dictionary<string, WwiseAddressableSoundBank> s_sharedBanks = new Dictionary<string, WwiseAddressableSoundBank>();

		private WwiseAddressableSoundBank _heldBank;

		private static readonly FieldInfo BankLoadStateField = typeof(WwiseAddressableSoundBank).GetField("loadState", BindingFlags.Instance | BindingFlags.NonPublic);

		private bool _isBankLoaded;

		private bool _isSpectated;

		public ExhaustSound ExhaustSound;

		public GameObject TurboSuperChargerPos;

		private GameObject _cachedChargerGo;

		private bool _isEngineBrokePlaying;

		private bool _isStarterPlaying;

		private bool _isEnginePlaying;

		private bool _isEngineStopping;

		private bool _isTurboPlaying;

		private bool _isBlowOffPlaying;

		private bool _isSuperChargerPlaying;

		private bool _isTransmissionPlaying;

		private bool _transmissionBankHeld;

		private bool _playTransmission;

		private const float TransferCaseWhineOnSpeedMps = 0.55f;

		private const float TransferCaseWhineOffSpeedMps = 0.05f;

		private bool _isTransferCasePlaying;

		private bool _transferCaseBankHeld;

		private const int TransferCaseStopFadeMs = 600;

		private const uint InvalidPlayingId = 0u;

		private uint _transferCasePlayingId;

		private int _transferCasePlayGeneration;

		private bool _transmissionRtpcFreshThisTick;

		private int _isShifting;

		private VehicleData _vehicleData;

		private SoundController _soundController;

		private Powertrain _powertrain;

		private static int s_nextInstanceId;

		private static readonly Dictionary<int, EngineSound> s_instances = new Dictionary<int, EngineSound>();

		private int _instanceId;

		private CallbackCookie _sharedCookie;

		private IDisposable _engineSwitchDelay;

		private IDisposable _disposeGearOff;

		private IDisposable _disposeEngineDamage;

		private IDisposable _disposableTestEngine;

		private TurboType _turboType;

		private bool _engineBroke;

		private bool _isBankLoadedSucess;

		private bool _isGearboxShiftsPlaying;

		private int _lastShotGear = int.MinValue;

		private float _lastGearShotTime;

		private const float GearShotCooldown = 0.3f;

		private Vector3 _turboPos;

		private Vector3 _exhaustPos;

		private Vector3 _emitterLastPosition = Vector3.zero;

		private Vector3 _listenerLastPosition = Vector3.zero;

		private float _rtpcRpmValue;

		private float _topSpeedMps;

		private const float TransmissionFallbackTopSpeedMps = 100f;

		private const float RatioEpsilon = 0.01f;

		private const float RadPerSecToRpm = 9.549297f;

		private const float OverrunWhineLevel = 0.18f;

		private const float OverrunFullSpeedMps = 10f;

		private float _smoothedTransmissionLoad;

		private const float TransmissionLoadRiseSpeed = 20f;

		private const float TransmissionLoadFallSpeed = 4f;

		private float _fakeRpm;

		private float _fakeRedlineRpm;

		private float _fakePeakNm;

		private float _fakeInertia;

		private float _fakeMaxTqRpm;

		private float _fakeDisplacementCc;

		private bool _fakeIsRotary;

		private float[] _fakeTorqueCurve;

		private float _fakeIdleRpm = 800f;

		private bool _isWaitingEngineEndCallback;

		private bool _isDemoRev;

		private int _enginePlayGeneration;

		private bool _isTurnOnEnginePending;

		private TurboType _fakeTurboType;

		public int FakeBlowOffType;

		private float _blowOffTiming;

		private Vector3 _emitterSpeed;

		private Vector3 _listenerSpeed;

		private Transform _lastListenerTransform;

		private bool _dopplerInitialized;

		private float _dopplerPitchSmoothed = 1f;

		private WhooshSoundControllerForBot _whooshSoundController;

		private float _smoothedBoost;

		private float _prevBoost;

		private float _chargerVol;

		private float _chargerThrottle;

		private SoundBankLoadService _soundBankService;

		private const float EngineMechBankWaitSeconds = 2f;

		private const float SilentLoadThreshold = 0.02f;

		private float _poweredWheelRadius;

		private float _dbgTransmissionSpeed;

		private float _dbgTransmissionLoad;

		private int _dbgTransmissionLogTick;

		private const int TransmissionLogEveryFrames = 15;

		private const float GearRtpcMin = 1f;

		private const float GearRtpcMax = 5f;

		private bool RallyTransmission
		{
			get
			{
				if ((bool)_vehicleData && (bool)_vehicleData.CarValues)
				{
					return _vehicleData.CarValues.RallyTransmission;
				}
				return false;
			}
		}

		private bool SequentialTransmission
		{
			get
			{
				if ((bool)_powertrain)
				{
					return _powertrain.GearboxKind == GearboxKind.Sequential;
				}
				return false;
			}
		}

		private bool HasTransmission
		{
			get
			{
				if (!RallyTransmission)
				{
					return SequentialTransmission;
				}
				return true;
			}
		}

		public bool IsBankLoaded { get; private set; }

		private bool IsBankReady
		{
			get
			{
				if (!_isBankLoaded)
				{
					return false;
				}
				if (BankLoadStateField == null || !_heldBank)
				{
					return true;
				}
				return (BankLoadState)BankLoadStateField.GetValue(_heldBank) == BankLoadState.Loaded;
			}
		}

		private GameObject GetChargerPos
		{
			get
			{
				if (!_cachedChargerGo)
				{
					_cachedChargerGo = (TurboSuperChargerPos ? TurboSuperChargerPos : base.gameObject);
				}
				return _cachedChargerGo;
			}
		}

		private Transform ListenerTransform
		{
			get
			{
				if (_isSpectated && (bool)V.CoreGameManager)
				{
					return V.CoreGameManager.transform;
				}
				if ((bool)V.Car && V.Car.gameObject == _powertrain.gameObject)
				{
					if ((bool)V.CoreGameManager && (bool)V.CoreGameManager.carCamerasController)
					{
						if (!V.CoreGameManager.carCamerasController.IsSpectatorCam)
						{
							if (!V.DriverPerson)
							{
								return null;
							}
							return V.DriverPerson.transform;
						}
						return V.CoreGameManager.transform;
					}
					if (FlagsStorage.RacingGameMode && (bool)_vehicleData.RacingProvider.DragRacingController && (bool)_vehicleData.RacingProvider.DragRacingController.dragRacingCamera)
					{
						if (_vehicleData.RacingProvider.DragRacingController.dragRacingCamera.currentCamera != DragRacingCamera.CameraType.spectator)
						{
							return _vehicleData.RacingProvider.DragRacingController.dragRacingCamera.transform;
						}
						if (!V.DriverPerson)
						{
							return null;
						}
						return V.DriverPerson.transform;
					}
				}
				if ((bool)V.DriverPerson)
				{
					return V.DriverPerson.transform;
				}
				if (!V.CoreGameManager)
				{
					return null;
				}
				return V.CoreGameManager.transform;
			}
		}

		private bool DisableDoopler
		{
			get
			{
				bool num = ((bool)V.Car && V.Car == _powertrain.gameObject) || _isSpectated;
				bool flag = (bool)V.CoreGameManager && (bool)V.CoreGameManager.carCamerasController && !V.CoreGameManager.carCamerasController.IsSpectatorCam;
				bool flag2 = FlagsStorage.RacingGameMode && (bool)_vehicleData.RacingProvider.DragRacingController && (bool)_vehicleData.RacingProvider.DragRacingController.dragRacingCamera && _vehicleData.RacingProvider.DragRacingController.dragRacingCamera.currentCamera != DragRacingCamera.CameraType.spectator;
				bool flag3 = ScenesHelper.IsLapRacingSceneActive();
				if (num)
				{
					return flag | flag2 | flag3;
				}
				return false;
			}
		}

		private bool IsEngineMechBankReady
		{
			get
			{
				if (_soundBankService != null)
				{
					return _soundBankService.IsLoaded(SoundBankType.CarEngineMechSounds);
				}
				return true;
			}
		}

		private bool DriveLineTurning => Mathf.Abs(_powertrain.SpeedMps) >= (_isTransferCasePlaying ? 0.05f : 0.55f);

		private bool EngineDeadAndStopped
		{
			get
			{
				if (!_powertrain.IsEngineWorks)
				{
					return !DriveLineTurning;
				}
				return false;
			}
		}

		private bool TransmissionUnloaded => _smoothedTransmissionLoad <= 0.02f;

		private uint GetCarStarterSound => _soundController.StarterId switch
		{
			0 => PlaySedanEngineStart, 
			1 => PlayOffRoadEngineStart, 
			2 => PlaySportCarEngineStart, 
			3 => PlaySportCarEngineStart, 
			_ => PlaySedanEngineStart, 
		};

		private uint GetStopEngineSound => _soundController.StarterId switch
		{
			0 => StopSedanEngine, 
			1 => StopOffRoadEngine, 
			2 => StopSportCarEngine, 
			3 => StopSportCarEngine, 
			_ => StopSedanEngine, 
		};

		private uint GetStopCarStarterSound => _soundController.StarterId switch
		{
			0 => StopSedanEngineStart, 
			1 => StopOffRoadEngineStart, 
			2 => StopSportCarEngineStart, 
			3 => StopSportCarEngineStart, 
			_ => StopSedanEngineStart, 
		};

		public event Action<bool> OnBankLoaded;

		private void RaiseBankLoaded(bool loaded)
		{
			IsBankLoaded = loaded;
			OnBankLoaded?.Invoke(loaded);
		}

		public void SpectateThisCar(bool state)
		{
			_isSpectated = state;
		}

		public void SetSellCar(float redline, float peak, float inertia, TurboType aspirated, float maxTqRpm, float displacementCc, bool isRotary, float[] torqueCurve, float idleRpm)
		{
			_fakeRedlineRpm = redline;
			_fakePeakNm = peak;
			_fakeTurboType = aspirated;
			_fakeInertia = inertia;
			_fakeMaxTqRpm = maxTqRpm;
			_fakeDisplacementCc = displacementCc;
			_fakeIsRotary = isRotary;
			_fakeTorqueCurve = torqueCurve;
			_fakeIdleRpm = Mathf.Max(idleRpm, 100f);
		}

		private static float Norm01(float x, float a, float b)
		{
			if (!(Mathf.Abs(b - a) < 0.0001f))
			{
				return Mathf.Clamp01((x - a) / (b - a));
			}
			return 0f;
		}

		private IObservable<Unit> DemoEngineCycle()
		{
			float idleRpm = _fakeIdleRpm;
			float redline = _fakeRedlineRpm;
			float scaleI = 1.5f * Mathf.Max(_fakeInertia, 0.0001f);
			float atmoPeakTorque = Mathf.Max(_fakePeakNm, 1f);
			float maxTqRpm = Mathf.Max(_fakeMaxTqRpm, 1f);
			float displacementCc = ((_fakeDisplacementCc > 0f) ? _fakeDisplacementCc : PhysMath.EstimateDisplacementCc(atmoPeakTorque));
			float[] curve = _fakeTorqueCurve;
			if (curve == null || curve.Length < 2)
			{
				float num = Mathf.Max(0.7f, CalcDurationSec(idleRpm, redline, _fakePeakNm, _fakeInertia));
				float duration = num / 0.55f;
				return AnimateRpmLinear(idleRpm, redline, num).Concat(AnimateRpmLinear(redline, idleRpm, duration, triggerBlowOff: true)).Finally(TurnOffFakeEngineSound);
			}
			return IntegrateRise().Concat(IntegrateFall()).Finally(TurnOffFakeEngineSound);
			IObservable<Unit> AnimateRpmLinear(float from, float to, float num2, bool triggerBlowOff = false)
			{
				bool blowed = false;
				bool isTurboLocal = _fakeTurboType != TurboType.NA;
				return (from t in (from _ in Observable.EveryUpdate()
						select Time.deltaTime).Scan(0f, (float t, float dt) => t + dt).TakeWhile((float t) => t < num2)
					select Mathf.Lerp(@from, to, t / Mathf.Max(0.0001f, num2))).Do((float rpm) =>
				{
					_fakeRpm = rpm;
					SetRtpcRpmFake(Norm01(_fakeRpm, idleRpm, redline));
					float num3 = Mathf.InverseLerp(idleRpm, redline, _fakeRpm);
					SetRtpcThrottleFake(num3);
					if (isTurboLocal)
					{
						float volume = _fakeRpm / redline;
						ChargerVolume(volume);
						if (triggerBlowOff)
						{
							TurboType fakeTurboType = _fakeTurboType;
							if (fakeTurboType != TurboType.NA && fakeTurboType != TurboType.Supercharger)
							{
								SetRtpcThrottleTurboFake(0f);
								if (!blowed)
								{
									FakeBlowOff(1f, FakeBlowOffType);
								}
								blowed = true;
								return;
							}
						}
						SetRtpcThrottleTurboFake(num3);
					}
				}).Last().AsUnitObservable();
			}
			void ApplyRtpcs(float rpm, float throttle, bool isTurboLocal, bool triggerBlowOff, ref bool blowed)
			{
				_fakeRpm = rpm;
				SetRtpcRpmFake(Norm01(_fakeRpm, idleRpm, redline));
				SetRtpcThrottleFake(throttle);
				if (isTurboLocal)
				{
					float volume = _fakeRpm / redline;
					ChargerVolume(volume);
					if (triggerBlowOff)
					{
						TurboType fakeTurboType = _fakeTurboType;
						if (fakeTurboType != TurboType.NA && fakeTurboType != TurboType.Supercharger)
						{
							SetRtpcThrottleTurboFake(0f);
							if (!blowed)
							{
								FakeBlowOff(1f, FakeBlowOffType);
							}
							blowed = true;
							return;
						}
					}
					SetRtpcThrottleTurboFake(throttle);
				}
			}
			IObservable<Unit> IntegrateFall()
			{
				bool isTurboLocal = _fakeTurboType != TurboType.NA;
				bool blowed = false;
				float rpm = redline;
				float elapsed = 0f;
				bool first = true;
				return Observable.EveryUpdate().Select((long _) =>
				{
					float deltaTime = Time.deltaTime;
					elapsed += deltaTime;
					float num2 = (0f - 2.5f * PhysMath.CalcPumpingBrakeTorque(rpm, maxTqRpm, displacementCc, _fakeIsRotary)) / scaleI / ((float)Math.PI / 30f) * deltaTime;
					rpm = Mathf.Max(rpm + num2, idleRpm);
					ApplyRtpcs(rpm, 0f, isTurboLocal, first, ref blowed);
					first = false;
					return (rpm: rpm, elapsed: elapsed);
				}).TakeWhile(((float rpm, float elapsed) s) => s.rpm > idleRpm + 20f && s.elapsed < 5f)
					.AsUnitObservable();
			}
			IObservable<Unit> IntegrateRise()
			{
				bool isTurboLocal = _fakeTurboType != TurboType.NA;
				bool blowed = false;
				float rpm = idleRpm;
				float elapsed = 0f;
				return Observable.EveryUpdate().Select((long _) =>
				{
					float deltaTime = Time.deltaTime;
					elapsed += deltaTime;
					float num2 = SampleTorque(rpm) / scaleI / ((float)Math.PI / 30f) * deltaTime;
					rpm = Mathf.Min(rpm + num2, redline);
					float throttle = Mathf.InverseLerp(idleRpm, redline, rpm);
					ApplyRtpcs(rpm, throttle, isTurboLocal, triggerBlowOff: false, ref blowed);
					return (rpm: rpm, elapsed: elapsed);
				}).TakeWhile(((float rpm, float elapsed) s) => s.rpm < redline - 1f && s.elapsed < 5f)
					.AsUnitObservable();
			}
			float SampleTorque(float rpm)
			{
				int num2 = curve.Length;
				float num3 = Mathf.Clamp01((rpm - idleRpm) / Mathf.Max(redline - idleRpm, 1f)) * (float)(num2 - 1);
				int num4 = Mathf.Clamp((int)num3, 0, num2 - 2);
				float t = num3 - (float)num4;
				return Mathf.Max(Mathf.Lerp(curve[num4], curve[num4 + 1], t), 1f);
			}
		}

		private static float RpmToRad(float rpm)
		{
			return rpm * ((float)Math.PI / 180f) * 6f;
		}

		private static float CalcDurationSec(float fromRpm, float toRpm, float torqueNm, float engineInertia)
		{
			float num = Mathf.Abs(RpmToRad(toRpm) - RpmToRad(fromRpm));
			float num2 = Mathf.Max(0.001f, torqueNm);
			return Mathf.Max(0.0001f, engineInertia) * num / num2 * 4f;
		}

		public void StartDemoRevCycle()
		{
			if (!_isDemoRev && base.gameObject.activeInHierarchy)
			{
				_isDemoRev = true;
				_disposableTestEngine?.Dispose();
				TurnOnFakeEngineSound();
				SwitchCharger(_fakeTurboType);
				_disposableTestEngine = DemoEngineCycle().Subscribe().AddTo(this);
			}
		}

		private void DopplerEffect(bool isTurbo)
		{
			if (!V.CoreGameManager && !FlagsStorage.RacingGameMode)
			{
				return;
			}
			if (DisableDoopler)
			{
				_dopplerInitialized = false;
				_lastListenerTransform = null;
				_dopplerPitchSmoothed = 1f;
				AudioWrapper.SetRTPCValue(RtpcDoppler, 1f, base.gameObject);
				if (isTurbo)
				{
					AudioWrapper.SetRTPCValue(RtpcDoppler, 1f, GetChargerPos);
				}
				if ((bool)_soundController)
				{
					AudioWrapper.SetRTPCValue(RtpcDoppler, 1f, _soundController.gameObject);
				}
				if ((bool)_vehicleData && (bool)_vehicleData.PoliceCarController)
				{
					AudioWrapper.SetRTPCValue(RtpcDoppler, 1f, _vehicleData.PoliceCarController.gameObject);
				}
			}
			else
			{
				if (Time.deltaTime <= Mathf.Epsilon)
				{
					return;
				}
				Transform listenerTransform = ListenerTransform;
				if (!listenerTransform)
				{
					return;
				}
				bool flag = _lastListenerTransform != listenerTransform;
				if (!_dopplerInitialized | flag)
				{
					_dopplerInitialized = true;
					_lastListenerTransform = listenerTransform;
					_emitterLastPosition = base.transform.position;
					_listenerLastPosition = listenerTransform.position;
					_emitterSpeed = Vector3.zero;
					_listenerSpeed = Vector3.zero;
					_dopplerPitchSmoothed = 1f;
					AudioWrapper.SetRTPCValue(RtpcDoppler, 1f, base.gameObject);
					if (isTurbo)
					{
						AudioWrapper.SetRTPCValue(RtpcDoppler, 1f, GetChargerPos);
					}
					if ((bool)_soundController)
					{
						AudioWrapper.SetRTPCValue(RtpcDoppler, 1f, _soundController.gameObject);
					}
					if ((bool)_vehicleData && (bool)_vehicleData.PoliceCarController)
					{
						AudioWrapper.SetRTPCValue(RtpcDoppler, 1f, _vehicleData.PoliceCarController.gameObject);
					}
					return;
				}
				bool num = (listenerTransform.position - _listenerLastPosition).sqrMagnitude > 16f;
				Vector3 b;
				if (_powertrain.OnLineCar)
				{
					b = base.transform.forward * _powertrain.SpeedMpsOnline;
				}
				else
				{
					b = (base.transform.position - _emitterLastPosition) / Time.deltaTime;
					_emitterLastPosition = base.transform.position;
				}
				Vector3 b2 = (num ? Vector3.zero : ((listenerTransform.position - _listenerLastPosition) / Time.deltaTime));
				_listenerLastPosition = listenerTransform.position;
				float num2 = (_powertrain.OnLineCar ? 10f : 20f);
				float t = 1f - Mathf.Exp((0f - num2) * Time.deltaTime);
				_emitterSpeed = Vector3.Lerp(_emitterSpeed, b, t);
				_listenerSpeed = Vector3.Lerp(_listenerSpeed, b2, t);
				Vector3 vector = listenerTransform.position - base.transform.position;
				float num3 = vector.magnitude + 1E-06f;
				Vector3 lhs = vector / num3;
				float value = 0f - Vector3.Dot(lhs, _listenerSpeed);
				float value2 = Vector3.Dot(lhs, _emitterSpeed);
				float num4 = 318.25f;
				value = Mathf.Clamp(value, 0f - num4, num4);
				value2 = Mathf.Clamp(value2, 0f - num4, num4);
				float fixedVariable = Utils.GetFixedVariable((335f + value) / (335f - value2));
				fixedVariable = Mathf.Clamp(fixedVariable, 0f, 2f);
				float num5 = (_powertrain.OnLineCar ? 10f : 22f);
				float t2 = 1f - Mathf.Exp((0f - num5) * Time.deltaTime);
				_dopplerPitchSmoothed = Mathf.Lerp(_dopplerPitchSmoothed, fixedVariable, t2);
				float dopplerPitchSmoothed = _dopplerPitchSmoothed;
				AudioWrapper.SetRTPCValue(RtpcDoppler, dopplerPitchSmoothed, base.gameObject);
				if (isTurbo)
				{
					AudioWrapper.SetRTPCValue(RtpcDoppler, dopplerPitchSmoothed, GetChargerPos);
				}
				if ((bool)_soundController)
				{
					AudioWrapper.SetRTPCValue(RtpcDoppler, dopplerPitchSmoothed, _soundController.gameObject);
				}
				if ((bool)_vehicleData && (bool)_vehicleData.PoliceCarController)
				{
					AudioWrapper.SetRTPCValue(RtpcDoppler, dopplerPitchSmoothed, _vehicleData.PoliceCarController.gameObject);
				}
			}
		}

		[Inject]
		public void Construct(SoundBankLoadService soundBankService)
		{
			_soundBankService = soundBankService;
		}

		private void PostEngineMechEvent(uint soundEvent, int? engineGeneration = null)
		{
			if (IsEngineMechBankReady)
			{
				AudioWrapper.PostEvent(soundEvent, base.gameObject);
			}
			else
			{
				PostEngineMechEventWhenBankReady(soundEvent, engineGeneration).Forget();
			}
		}

		private async UniTaskVoid PostEngineMechEventWhenBankReady(uint soundEvent, int? engineGeneration)
		{
			CancellationToken cancellationTokenOnDestroy = this.GetCancellationTokenOnDestroy();
			await UniTask.WaitUntil(() => _soundBankService.IsLoaded(SoundBankType.CarEngineMechSounds), PlayerLoopTiming.Update, cancellationTokenOnDestroy).TimeoutWithoutException(TimeSpan.FromSeconds(2.0));
			if ((bool)this && _soundBankService.IsLoaded(SoundBankType.CarEngineMechSounds) && (!engineGeneration.HasValue || engineGeneration.Value == _enginePlayGeneration))
			{
				AudioWrapper.PostEvent(soundEvent, base.gameObject);
			}
		}

		private void Awake()
		{
			_instanceId = Interlocked.Increment(ref s_nextInstanceId);
			s_instances[_instanceId] = this;
			_sharedCookie = new CallbackCookie
			{
				InstanceId = _instanceId
			};
			GetComponent<SphereCollider>().isTrigger = true;
			Transform parent = base.transform.parent;
			parent.TryGetComponent<VehicleData>(out _vehicleData);
			parent.TryGetComponent<Powertrain>(out _powertrain);
			if (parent.TryGetComponent<SoundController>(out _soundController))
			{
				_soundController.EngineSound = this;
				if (!ExhaustSound)
				{
					ExhaustSound = GetComponentInChildren<ExhaustSound>();
				}
				if ((bool)_powertrain)
				{
					_powertrain.SetRealisticES(this);
					if ((bool)ExhaustSound)
					{
						ExhaustSound.Init(_soundController, _powertrain);
						_exhaustPos = ExhaustSound.transform.localPosition;
					}
				}
				if ((bool)GetChargerPos)
				{
					_turboPos = GetChargerPos.transform.localPosition;
				}
			}
			_whooshSoundController = new WhooshSoundControllerForBot(base.transform);
			LoadBank();
			if (HasTransmission)
			{
				_transmissionBankHeld = true;
				_soundBankService?.TryLoadBank(SoundBankType.CarTransmission).Forget();
			}
		}

		public void GetPositionGO(out Vector3 turboPos, out Vector3 exhaustPos)
		{
			turboPos = _turboPos;
			exhaustPos = _exhaustPos;
		}

		public void RepositionGO(Vector3 turboPos, Vector3 exhaustPos)
		{
			if ((bool)GetChargerPos)
			{
				GetChargerPos.transform.localPosition = turboPos;
			}
			if ((bool)ExhaustSound)
			{
				ExhaustSound.transform.localPosition = exhaustPos;
			}
		}

		private void GenerateEngineSoundEvents()
		{
			if (!_powertrain)
			{
				_enginePlayEvent = AudioWrapper.GetId(EnginePlayEvent);
				_engineStopEvent = AudioWrapper.GetId(EngineStopEvent);
			}
			else if (_powertrain.IsRaceCar && Randomizer > 0)
			{
				int num = UnityEngine.Random.Range(1, Randomizer);
				string enginePlayEvent = EnginePlayEvent;
				_enginePlayEvent = AudioWrapper.GetId(enginePlayEvent.Substring(0, enginePlayEvent.Length - 1) + num);
				enginePlayEvent = EngineStopEvent;
				_engineStopEvent = AudioWrapper.GetId(enginePlayEvent.Substring(0, enginePlayEvent.Length - 1) + num);
			}
			else
			{
				_enginePlayEvent = AudioWrapper.GetId(EnginePlayEvent);
				_engineStopEvent = AudioWrapper.GetId(EngineStopEvent);
			}
		}

		private void DamageEngineVisual(bool damage)
		{
			_isEngineBrokePlaying = damage;
			_disposeEngineDamage?.Dispose();
			if ((bool)_vehicleData)
			{
				_disposeEngineDamage = Observable.Timer(TimeSpan.FromSeconds(1.0)).Subscribe((long _) =>
				{
					_vehicleData.ExhaustComposition.UpdateDamagedExhaust(damage);
				}).AddTo(this);
			}
		}

		public void SetEngineBrokeSound()
		{
			_engineBroke = _powertrain.CheckEngineStatus;
			if (_engineBroke && _isEnginePlaying)
			{
				PlayBrokenEngineSound();
			}
			else
			{
				StopEngineBrokenSound();
			}
		}

		private void PlayBrokenEngineSound()
		{
			if (!_isEngineBrokePlaying)
			{
				PostEngineMechEvent(PlayBrokenEngine);
				DamageEngineVisual(damage: true);
			}
		}

		private void StopEngineBrokenSound()
		{
			if (_isEngineBrokePlaying)
			{
				PostEngineMechEvent(StopBrokenEngine);
				DamageEngineVisual(damage: false);
			}
		}

		private WwiseAddressableSoundBank ResolveSharedBank()
		{
			if (EngineBank == null || !EngineBank.WwiseObjectReference)
			{
				return null;
			}
			WwiseAddressableSoundBank addressableBank = EngineBank.WwiseObjectReference.AddressableBank;
			if (!addressableBank)
			{
				return null;
			}
			if (s_sharedBanks.TryGetValue(addressableBank.name, out var value) && (bool)value)
			{
				return value;
			}
			s_sharedBanks[addressableBank.name] = addressableBank;
			return addressableBank;
		}

		private void LoadBank()
		{
			if (!EngineBank.IsValid() || _isBankLoaded || (bool)_heldBank)
			{
				return;
			}
			WwiseAddressableSoundBank wwiseAddressableSoundBank = ResolveSharedBank();
			if ((bool)wwiseAddressableSoundBank)
			{
				_heldBank = wwiseAddressableSoundBank;
				AkAddressableBankManager.Instance.LoadBank(wwiseAddressableSoundBank);
				(from _ in Observable.Interval(TimeSpan.FromSeconds(0.10000000149011612))
					where CheckBankIsLoaded()
					select _).Take(1).Delay(TimeSpan.FromSeconds(0.10000000149011612)).Subscribe((long _) =>
				{
					_isBankLoaded = true;
				})
					.AddTo(this);
			}
		}

		private bool CheckBankIsLoaded()
		{
			if (!_heldBank)
			{
				return false;
			}
			if (_heldBank.Data == null)
			{
				return false;
			}
			if (_heldBank.Data.Count == 0)
			{
				return false;
			}
			if (_heldBank.Data.First().Value == null)
			{
				return false;
			}
			bool isDone = _heldBank.Data.First().Value.OperationHandle.IsDone;
			if (isDone)
			{
				GenerateEngineSoundEvents();
				RaiseBankLoaded(loaded: true);
			}
			return isDone;
		}

		private void UnloadBank()
		{
			if (!_heldBank)
			{
				RaiseBankLoaded(loaded: false);
				_isBankLoaded = false;
				return;
			}
			RaiseBankLoaded(loaded: false);
			AkAddressableBankManager.Instance.UnloadBank(_heldBank);
			_heldBank = null;
			_isBankLoaded = false;
		}

		private void OnDestroy()
		{
			s_instances.Remove(_instanceId);
			_disposeGearOff?.Dispose();
			TurnOffEngineSoundForce();
			UnloadBank();
			if (_transmissionBankHeld)
			{
				_transmissionBankHeld = false;
				_soundBankService?.TryUnloadBank(SoundBankType.CarTransmission).Forget();
			}
			if (_transferCaseBankHeld)
			{
				_transferCaseBankHeld = false;
				_soundBankService?.TryUnloadBank(SoundBankType.CarTransferCase).Forget();
			}
		}

		public void TurnOffEngineSoundForce()
		{
			if ((!EngineBank.IsValid() || _isBankLoaded) && AudioWrapper.IsInitialized())
			{
				if (GetChargerPos != base.gameObject)
				{
					AudioWrapper.StopAll(GetChargerPos);
				}
				AudioWrapper.StopAll(base.gameObject);
			}
		}

		private void TurnOffFakeEngineSound()
		{
			if ((!EngineBank.IsValid() || IsBankReady) && AudioWrapper.IsInitialized())
			{
				if (_isEnginePlaying)
				{
					AudioWrapper.PostEvent(_engineStopEvent, base.gameObject);
				}
				if (_isTurboPlaying)
				{
					_soundBankService?.PostEvent(StopTurbo, SoundBankType.CarCompressor, (GetChargerPos != base.gameObject) ? GetChargerPos : base.gameObject).Forget();
				}
				if (_isSuperChargerPlaying)
				{
					_soundBankService?.PostEvent(StopSuperCharger, SoundBankType.CarCompressor, (GetChargerPos != base.gameObject) ? GetChargerPos : base.gameObject).Forget();
				}
				StopTransmissionSound();
				StopTransferCaseSound();
				_isEnginePlaying = false;
				_isTurboPlaying = false;
				_isSuperChargerPlaying = false;
				_isDemoRev = false;
			}
		}

		public void TurnOffEngineSound()
		{
			if ((bool)_powertrain && _powertrain.OnLineCar)
			{
				StopOnlineEngine();
				return;
			}
			StopEngineStartSound();
			if ((!EngineBank.IsValid() || IsBankReady) && AudioWrapper.IsInitialized() && !_isEngineStopping)
			{
				StopEngineSound();
				if (_isTurboPlaying)
				{
					_soundBankService?.PostEvent(StopTurbo, SoundBankType.CarCompressor, (GetChargerPos != base.gameObject) ? GetChargerPos : base.gameObject).Forget();
				}
				if (_isSuperChargerPlaying)
				{
					_soundBankService?.PostEvent(StopSuperCharger, SoundBankType.CarCompressor, (GetChargerPos != base.gameObject) ? GetChargerPos : base.gameObject).Forget();
				}
				_isTurboPlaying = false;
				_isSuperChargerPlaying = false;
				StopEngineBrokenSound();
				_isDemoRev = false;
			}
		}

		private void StopEngineSound()
		{
			if (_isEnginePlaying)
			{
				AudioWrapper.PostEvent(_engineStopEvent, base.gameObject, 1u, CallBackDestroyStatic, _sharedCookie);
				PostEngineMechEvent(GetStopEngineSound, _enginePlayGeneration);
				_isEngineStopping = true;
			}
			_isEnginePlaying = false;
		}

		public void OnRtpcUpShiftOnLoad(float gearTime)
		{
			AudioWrapper.SetRTPCValue(RtpcUpShiftOnLoad, 0f, base.gameObject);
			_disposeGearOff?.Dispose();
			_disposeGearOff = Observable.Timer(TimeSpan.FromSeconds(gearTime)).Subscribe((long _) =>
			{
				AudioWrapper.SetRTPCValue(RtpcUpShiftOnLoad, 1f, base.gameObject);
			}).AddTo(this);
		}

		public void SwitchCharger(TurboType turboType)
		{
			switch (turboType)
			{
			case TurboType.SingleTurboMonoScroll:
			case TurboType.SingleTurboTwinScroll:
			case TurboType.TwinTurboSingleScroll:
			case TurboType.TwinTurboTwinScroll:
			case TurboType.BiTurbo:
			case TurboType.QuadTurboCharger:
				PlayTurboSound();
				break;
			case TurboType.Supercharger:
				PlaySuperChargerSound();
				break;
			case TurboType.NA:
				break;
			}
		}

		private void TurnOnTurbo()
		{
			_powertrain.GetEngineTurboParams(out var _, out var _, out var _, out _turboType);
			SwitchCharger(_turboType);
		}

		public void TurnOnFakeEngineSound()
		{
			if (!AudioWrapper.IsInitialized())
			{
				return;
			}
			if (EngineBank.IsValid() && !_isBankLoaded)
			{
				Observable.Timer(TimeSpan.FromSeconds(0.20000000298023224)).Subscribe((long _) =>
				{
					TurnOnFakeEngineSound();
				}).AddTo(this);
			}
			else if (!_isEnginePlaying)
			{
				AudioWrapper.PostEvent(_enginePlayEvent, base.gameObject, 1u, CallBackEngineStatic, new CallbackCookie
				{
					InstanceId = _instanceId,
					Generation = _enginePlayGeneration
				});
				_isEnginePlaying = true;
			}
		}

		public void TurnOnEngineSound()
		{
			if (!AudioWrapper.IsInitialized())
			{
				return;
			}
			if (EngineBank.IsValid() && !_isBankLoaded)
			{
				if (!_isTurnOnEnginePending)
				{
					_isTurnOnEnginePending = true;
					Observable.Timer(TimeSpan.FromSeconds(0.20000000298023224)).Subscribe((long _) =>
					{
						_isTurnOnEnginePending = false;
						TurnOnEngineSound();
					}).AddTo(this);
				}
				return;
			}
			_isTurnOnEnginePending = false;
			if (_powertrain.IsEngineWorks && (!_isEnginePlaying || !_powertrain.IsEngineWorks))
			{
				StopEngineStartSound();
				int generation = ++_enginePlayGeneration;
				AudioWrapper.PostEvent(_enginePlayEvent, base.gameObject, 1u, CallBackEngineStatic, new CallbackCookie
				{
					InstanceId = _instanceId,
					Generation = generation
				});
				_isEnginePlaying = true;
				_isEngineStopping = false;
				_lastShotGear = _powertrain.Gear;
				StopTransferCaseSound();
				_topSpeedMps = 0f;
				SetEngineBrokeSound();
				TurnOnTurbo();
				_soundController.CanPlayCarSounds = true;
			}
		}

		public void PlayEngineStartSound()
		{
			if (!_isStarterPlaying)
			{
				PostEngineMechEvent(GetCarStarterSound);
				_isStarterPlaying = true;
			}
		}

		public void StopEngineStartSound()
		{
			if (_isStarterPlaying)
			{
				PostEngineMechEvent(GetStopCarStarterSound);
				_isStarterPlaying = false;
			}
		}

		private static void CallBackEngineStatic(object inCookie, AkCallbackType callType, object inInfo)
		{
			if (callType == AkCallbackType.AK_EndOfEvent && inCookie is CallbackCookie callbackCookie && s_instances.TryGetValue(callbackCookie.InstanceId, out var value) && (bool)value)
			{
				if (callbackCookie.Generation == value._enginePlayGeneration)
				{
					value._isEnginePlaying = false;
				}
				value._isWaitingEngineEndCallback = false;
			}
		}

		private static void CallBackDestroyStatic(object inCookie, AkCallbackType callType, object inInfo)
		{
			if (inCookie is CallbackCookie callbackCookie && s_instances.TryGetValue(callbackCookie.InstanceId, out var value) && !(value == null))
			{
				value._isEngineStopping = false;
			}
		}

		private void LateUpdate()
		{
			if ((bool)_powertrain && _powertrain.OnLineCar)
			{
				if (!_isEnginePlaying && _powertrain.IsEngineWorks)
				{
					TurnOnEngineSound();
				}
				else if (_isEnginePlaying && !_powertrain.IsEngineWorks)
				{
					StopOnlineEngine();
				}
			}
		}

		private void Update()
		{
			if ((EngineBank.IsValid() && !_isBankLoaded) || !_powertrain)
			{
				return;
			}
			_rtpcRpmValue = math.remap(_powertrain.EngineIdleRpm, _powertrain.RedlineRpm, 0f, 1f, _powertrain.CurrentRpm);
			_rtpcRpmValue = Mathf.Clamp01(_rtpcRpmValue);
			if (Time.timeScale == 0f)
			{
				return;
			}
			if (_isEnginePlaying && _powertrain.IsEngineWorks)
			{
				bool flag = _turboType != TurboType.NA;
				DopplerEffect(flag);
				SetRtpcThrottle(flag);
				SetRtpcRpm();
				SetRtpcShifting();
				_powertrain.GetEngineTurboParams(out var maxTurboPressure, out var curTurboPressure, out var _, out var _);
				switch (_turboType)
				{
				case TurboType.SingleTurboMonoScroll:
				case TurboType.SingleTurboTwinScroll:
				case TurboType.TwinTurboSingleScroll:
				case TurboType.TwinTurboTwinScroll:
				case TurboType.BiTurbo:
				case TurboType.QuadTurboCharger:
					ChargerVolume(curTurboPressure, maxTurboPressure);
					break;
				case TurboType.Supercharger:
					SuperChargerVolume(_rtpcRpmValue);
					break;
				}
			}
			if (_isEnginePlaying && (bool)_vehicleData && _vehicleData.IsRacingCar)
			{
				_whooshSoundController.PlayWhoosh(_listenerSpeed, _emitterSpeed);
			}
			if (!_powertrain.OnLineCar && (!_vehicleData || !_vehicleData.IsRacingCar))
			{
				_transmissionRtpcFreshThisTick = false;
				TransmissionBuzz();
				TransferCaseBuzz();
			}
		}

		private void ChargerVolume(float curTurboPressure, float maxTurboPressure)
		{
			float num = 5f;
			float num2 = 1.2f;
			float num3 = ((curTurboPressure > _smoothedBoost) ? num : num2);
			_smoothedBoost += (curTurboPressure - _smoothedBoost) * (1f - Mathf.Exp((0f - num3) * Time.deltaTime));
			float value = Mathf.InverseLerp(0.2f, maxTurboPressure, _smoothedBoost);
			value = Mathf.Clamp01(value);
			_chargerVol = Mathf.Log10(value * 9f + 1f);
			ChargerVolume(_chargerVol);
			_prevBoost = _smoothedBoost;
		}

		private void SuperChargerVolume(float vol)
		{
			ChargerVolume(vol);
		}

		private void TransferCaseBuzz()
		{
			TransferCase transferCase = (_powertrain ? _powertrain.TransferCase : null);
			if (transferCase != null && transferCase.WhinesAudibly && DriveLineTurning && ResolveInteriorView())
			{
				UpdateTransmissionRtpc();
				PlayTransferCaseSound();
			}
			else
			{
				StopTransferCaseSound();
			}
		}

		private void UpdateTransmissionRtpc()
		{
			if (!_transmissionRtpcFreshThisTick)
			{
				_transmissionRtpcFreshThisTick = true;
				SetRtpcTransmissionSpeed();
				SetRtpcTransmissionLoad();
				SetSimulateRtpcGear(_powertrain.Gear);
			}
		}

		private bool ResolveInteriorView()
		{
			if ((bool)V.CoreGameManager && (bool)V.CoreGameManager.carCamerasController)
			{
				return V.CoreGameManager.carCamerasController.IsFirstPersonView;
			}
			if ((bool)_vehicleData.RacingProvider?.RacingCameraController)
			{
				return _vehicleData.RacingProvider.RacingCameraController.currentCamera == DragRacingCamera.CameraType.driverView;
			}
			return _playTransmission;
		}

		private void TransmissionBuzz()
		{
			if (RallyTransmission || SequentialTransmission || !(_powertrain.EngineMaxTorque < 900f))
			{
				_playTransmission = ResolveInteriorView();
				if (_playTransmission)
				{
					UpdateTransmissionRtpc();
				}
				if (_playTransmission && !TransmissionUnloaded)
				{
					PlayTransmissionSound();
				}
				else
				{
					StopTransmissionSound();
				}
			}
		}

		private float GetPoweredWheelRadius()
		{
			if (_poweredWheelRadius > 0.05f)
			{
				return _poweredWheelRadius;
			}
			foreach (WheelProvider allWheel in _powertrain.GetAxles.AllWheels)
			{
				if (allWheel.Wheel.IsPowered && !(allWheel.Wheel.Radius <= 0.05f))
				{
					_poweredWheelRadius = allWheel.Wheel.Radius;
					break;
				}
			}
			return _poweredWheelRadius;
		}

		private void SetRtpcTransmissionSpeed()
		{
			if (_topSpeedMps <= 1f)
			{
				_topSpeedMps = _powertrain.CalcTopSpeed();
				if (!(_topSpeedMps > 1f))
				{
					_topSpeedMps = 100f;
				}
			}
			float f = (_powertrain.TryGetPoweredWheelsTireVelocity(out var velocity) ? velocity : _powertrain.SpeedMps);
			float num = Mathf.Abs(_powertrain.GetGearRatio(_powertrain.Gear + 1));
			float num2 = Mathf.Abs(_powertrain.GetGearRatio(_powertrain.NumGears + 1));
			float redlineRpm = _powertrain.RedlineRpm;
			float poweredWheelRadius = GetPoweredWheelRadius();
			AudioWrapper.SetRTPCValue(in_value: _dbgTransmissionSpeed = ((num > 0.01f && redlineRpm > 1f && poweredWheelRadius > 0.05f) ? Mathf.Clamp01(Mathf.Abs(f) / poweredWheelRadius * num * _powertrain.FinalDrive * 9.549297f / redlineRpm) : ((!(num > 0.01f) || !(num2 > 0.01f)) ? Mathf.Clamp01(Mathf.Abs(f) / _topSpeedMps) : Mathf.Clamp01(Mathf.Abs(f) * num / (_topSpeedMps * num2)))), in_rtpcID: RtpcTransmissionSpeed, in_gameObjectID: base.gameObject);
		}

		private void SetRtpcTransmissionLoad()
		{
			bool engineDeadAndStopped = EngineDeadAndStopped;
			float num = 0f;
			if (!engineDeadAndStopped && _powertrain.Gear != 0)
			{
				float num2 = 1f - Mathf.Clamp01(_powertrain.Clutch);
				num = Mathf.Clamp01(Mathf.Max(b: 0.18f * Mathf.Clamp01(Mathf.Abs(_powertrain.SpeedMps) / 10f), a: _powertrain.Load * num2));
			}
			if (engineDeadAndStopped)
			{
				_smoothedTransmissionLoad = 0f;
			}
			else
			{
				float num3 = ((num > _smoothedTransmissionLoad) ? 20f : 4f);
				_smoothedTransmissionLoad += (num - _smoothedTransmissionLoad) * (1f - Mathf.Exp((0f - num3) * Time.deltaTime));
			}
			AudioWrapper.SetRTPCValue(in_value: _dbgTransmissionLoad = _smoothedTransmissionLoad, in_rtpcID: RtpcTransmissionLoad, in_gameObjectID: base.gameObject);
		}

		private void LogTransmissionRtpc()
		{
			if (++_dbgTransmissionLogTick >= 15)
			{
				_dbgTransmissionLogTick = 0;
				int gear = _powertrain.Gear;
				Mathf.Abs(_powertrain.GetGearRatio(gear + 1));
			}
		}

		private void SetRtpcRpm()
		{
			AudioWrapper.SetRTPCValue(RtpcRpm, _rtpcRpmValue, base.gameObject);
		}

		private void SetRtpcRpmFake(float rpm)
		{
			AudioWrapper.SetRTPCValue(RtpcRpm, rpm, base.gameObject);
		}

		private void SetRtpcShifting()
		{
			if (!_powertrain.OnLineCar)
			{
				_isShifting = ((!_powertrain.Automatic) ? ((_powertrain.Clutch >= 1f) ? 1 : 0) : (_powertrain.IsShifting ? 1 : 0));
				AudioWrapper.SetRTPCValue(RtpcSimulateShiftGear, _isShifting, base.gameObject);
				TryPlayGearboxShot();
			}
		}

		private void TryPlayGearboxShot()
		{
			if (!HasTransmission)
			{
				return;
			}
			int gear = _powertrain.Gear;
			if (gear != _lastShotGear)
			{
				bool num = _lastShotGear != int.MinValue;
				_lastShotGear = gear;
				if (num && gear > 0 && !_isGearboxShiftsPlaying && !(Time.time < _lastGearShotTime + 0.3f) && IsEngineMechBankReady)
				{
					_lastGearShotTime = Time.time;
					AudioWrapper.PostEvent(PlayGearboxShifts, base.gameObject, 1u, CallBackGearboxShiftStatic, _sharedCookie);
					_isGearboxShiftsPlaying = true;
				}
			}
		}

		private static void CallBackGearboxShiftStatic(object inCookie, AkCallbackType callType, object inInfo)
		{
			if (callType == AkCallbackType.AK_EndOfEvent && inCookie is CallbackCookie callbackCookie && s_instances.TryGetValue(callbackCookie.InstanceId, out var value) && (bool)value)
			{
				value._isGearboxShiftsPlaying = false;
			}
		}

		public void SetSimulateRtpcGear(int gear)
		{
			int num = ((!_powertrain) ? 1 : Mathf.Max(_powertrain.NumGears, 1));
			float t = ((num > 1) ? Mathf.Clamp01(((float)gear - 1f) / ((float)num - 1f)) : 0f);
			AudioWrapper.SetRTPCValue(RtpcSimulateGear, Mathf.Lerp(1f, 5f, t), base.gameObject);
		}

		private void SetRtpcThrottle(bool turbo)
		{
			Throttle = _powertrain.ThrottleRev;
			AudioWrapper.SetRTPCValue(RtpcSimulateThrottle, Throttle, base.gameObject);
			if (!turbo)
			{
				return;
			}
			float throttle = _powertrain.Throttle;
			float num = ((throttle > _chargerThrottle) ? 10f : 1.5f);
			_chargerThrottle += (throttle - _chargerThrottle) * (1f - Mathf.Exp((0f - num) * Time.deltaTime));
			AudioWrapper.SetRTPCValue(RtpcSimulateThrottle, _chargerThrottle, GetChargerPos);
			if (_powertrain.Throttle > 0.5f && _isBlowOffPlaying)
			{
				_blowOffTiming += Time.deltaTime;
				if (_blowOffTiming >= _powertrain.GearDelay)
				{
					_soundBankService?.PostEvent(StopBlowOffShot, SoundBankType.CarCompressor, GetChargerPos).Forget();
					AudioWrapper.SetRTPCValue(RtpcBlowOffVolume, 0f, GetChargerPos);
					_blowOffTiming = 0f;
				}
			}
		}

		private void SetRtpcThrottleFake(float throttle)
		{
			AudioWrapper.SetRTPCValue(RtpcSimulateThrottle, throttle, base.gameObject);
		}

		private void SetRtpcThrottleTurboFake(float throttle)
		{
			AudioWrapper.SetRTPCValue(RtpcSimulateThrottle, throttle, GetChargerPos);
		}

		private void OnEnable()
		{
			if ((bool)_powertrain)
			{
				_powertrain.OnEngineStarted += TurnOnEngineSound;
				_powertrain.OnEngineShutdown += TurnOffEngineSound;
				Powertrain powertrain = _powertrain;
				powertrain.OnCheckEngineChanged = (Action)Delegate.Combine(powertrain.OnCheckEngineChanged, new Action(SetEngineBrokeSound));
				if (!_isEnginePlaying && _powertrain.IsEngineWorks)
				{
					TurnOnEngineSound();
				}
			}
		}

		private void StopOnlineEngine()
		{
			if (_isTurboPlaying)
			{
				_soundBankService?.PostEvent(StopTurbo, SoundBankType.CarCompressor, (GetChargerPos != base.gameObject) ? GetChargerPos : base.gameObject).Forget();
			}
			if (_isSuperChargerPlaying)
			{
				_soundBankService?.PostEvent(StopSuperCharger, SoundBankType.CarCompressor, (GetChargerPos != base.gameObject) ? GetChargerPos : base.gameObject).Forget();
			}
			StopEngineBrokenSound();
			StopTransmissionSound();
			StopTransferCaseSound();
			if ((bool)_powertrain && _powertrain.OnLineCar)
			{
				AudioWrapper.StopAll(base.gameObject);
			}
			_isTurboPlaying = false;
			_isEnginePlaying = false;
			_isSuperChargerPlaying = false;
			_isStarterPlaying = false;
			_isGearboxShiftsPlaying = false;
		}

		private void OnDisable()
		{
			_isSpectated = false;
			if (!_powertrain)
			{
				if (_isDemoRev)
				{
					TurnOffFakeEngineSound();
				}
				return;
			}
			if (_powertrain.OnLineCar)
			{
				StopOnlineEngine();
			}
			else
			{
				TurnOffEngineSound();
				StopTransmissionSound();
				StopTransferCaseSound();
			}
			_powertrain.OnEngineStarted -= TurnOnEngineSound;
			_powertrain.OnEngineShutdown -= TurnOffEngineSound;
			Powertrain powertrain = _powertrain;
			powertrain.OnCheckEngineChanged = (Action)Delegate.Remove(powertrain.OnCheckEngineChanged, new Action(SetEngineBrokeSound));
		}

		private void PlayTransmissionSound()
		{
			if (!_isTransmissionPlaying && HasTransmission)
			{
				if (!_transmissionBankHeld)
				{
					_transmissionBankHeld = true;
					_soundBankService?.TryLoadBank(SoundBankType.CarTransmission).Forget();
				}
				else
				{
					_soundBankService?.PostEvent(PlayTransmission, SoundBankType.CarTransmission, base.gameObject).Forget();
					_isTransmissionPlaying = true;
				}
			}
		}

		private void StopTransmissionSound()
		{
			if (_isTransmissionPlaying)
			{
				_isTransmissionPlaying = false;
				if (_soundBankService != null && _soundBankService.IsLoaded(SoundBankType.CarTransmission))
				{
					AudioWrapper.PostEvent(StopTransmission, base.gameObject);
				}
			}
		}

		private void PlayTransferCaseSound()
		{
			if (!_isTransferCasePlaying)
			{
				if (!_transferCaseBankHeld)
				{
					_transferCaseBankHeld = true;
					_soundBankService?.TryLoadBank(SoundBankType.CarTransferCase).Forget();
					return;
				}
				_transferCasePlayingId = 0u;
				int generation = ++_transferCasePlayGeneration;
				_isTransferCasePlaying = true;
				CaptureTransferCasePlayingId(generation).Forget();
			}
		}

		private async UniTask CaptureTransferCasePlayingId(int generation)
		{
			if (_soundBankService == null)
			{
				return;
			}
			uint num = await _soundBankService.PostEventGetId(PlayTransferCase, SoundBankType.CarTransferCase, base.gameObject);
			if (num != 0)
			{
				if (generation != _transferCasePlayGeneration || !_isTransferCasePlaying)
				{
					AudioWrapper.StopPlayingId(num, 600);
				}
				else
				{
					_transferCasePlayingId = num;
				}
			}
		}

		private void StopTransferCaseSound()
		{
			if (_isTransferCasePlaying)
			{
				_isTransferCasePlaying = false;
				if (_transferCasePlayingId != 0)
				{
					AudioWrapper.StopPlayingId(_transferCasePlayingId, 600);
					_transferCasePlayingId = 0u;
				}
				else if (_soundBankService != null && _soundBankService.IsLoaded(SoundBankType.CarTransferCase))
				{
					AudioWrapper.PostEvent(StopTransferCase, base.gameObject);
				}
			}
		}

		private void PlayTurboSound()
		{
			if (!_isTurboPlaying)
			{
				_soundBankService?.PostEvent(PlayTurbo, SoundBankType.CarCompressor, GetChargerPos).Forget();
				_isTurboPlaying = true;
			}
		}

		public void ChargerVolume(float volume)
		{
			AudioWrapper.SetRTPCValue(RtpcChargerVolume, volume, GetChargerPos);
		}

		private void PlayFakeSuperChargerSound()
		{
			if (!_isSuperChargerPlaying)
			{
				SuperChargerSwitch(0);
				_soundBankService?.PostEvent(PlaySuperCharger, SoundBankType.CarCompressor, GetChargerPos).Forget();
				_isSuperChargerPlaying = true;
			}
		}

		private void PlaySuperChargerSound()
		{
			if (!_soundController)
			{
				PlayFakeSuperChargerSound();
			}
			else if (!_isSuperChargerPlaying)
			{
				SuperChargerSwitch(_soundController.SuperChargerType);
				_soundBankService?.PostEvent(PlaySuperCharger, SoundBankType.CarCompressor, GetChargerPos).Forget();
				_isSuperChargerPlaying = true;
			}
		}

		private void SuperChargerSwitch(int type)
		{
			uint state = type switch
			{
				0 => VariableStock, 
				1 => VariableType1, 
				2 => VariableType2, 
				3 => VariableType3, 
				4 => VariableType4, 
				5 => VariableType5, 
				_ => VariableStock, 
			};
			AudioWrapper.SetSwitch(SuperchargerSwitch, state, GetChargerPos);
		}

		private void BlowOffSwitch(int type)
		{
			uint state = type switch
			{
				0 => VariableStock, 
				1 => VariableType1, 
				2 => VariableType2, 
				3 => VariableType3, 
				4 => VariableType4, 
				5 => VariableType5, 
				_ => VariableStock, 
			};
			AudioWrapper.SetSwitch(TurboShootSwitch, state, GetChargerPos);
		}

		public void FakeBlowOff(float vol, int type)
		{
			BlowOffSwitch(type);
			_soundBankService?.PostEvent(PlayBlowOffShot, SoundBankType.CarCompressor, GetChargerPos, 1u, CallBackBlowStatic, _sharedCookie).Forget();
			AudioWrapper.SetRTPCValue(RtpcBlowOffVolume, vol, GetChargerPos);
		}

		private static void CallBackBlowStatic(object inCookie, AkCallbackType callType, object inInfo)
		{
			if (callType == AkCallbackType.AK_EndOfEvent && inCookie is CallbackCookie callbackCookie && s_instances.TryGetValue(callbackCookie.InstanceId, out var value) && !(value == null))
			{
				value._isBlowOffPlaying = false;
				value._blowOffTiming = 0f;
			}
		}

		public void BlowOff(float vol)
		{
			if (!_vehicleData.isTruck)
			{
				BlowOffSwitch(_soundController.BlowOffType);
				PlayBlowOff(vol);
			}
		}

		private void PlayBlowOff(float vol)
		{
			if (!_isBlowOffPlaying)
			{
				_soundBankService?.PostEvent(PlayBlowOffShot, SoundBankType.CarCompressor, GetChargerPos, 1u, CallBackBlowStatic, _sharedCookie).Forget();
				_isBlowOffPlaying = true;
				AudioWrapper.SetRTPCValue(RtpcBlowOffVolume, vol, GetChargerPos);
			}
		}

		[ContextMenu("Fix Exhaust Position")]
		public void FixPosition()
		{
			float y = base.transform.localPosition.y;
			float z = base.transform.localPosition.z;
			ExhaustSound componentInChildren = GetComponentInChildren<ExhaustSound>();
			if ((bool)componentInChildren)
			{
				float y2 = componentInChildren.transform.localPosition.y + y;
				float z2 = componentInChildren.transform.localPosition.z + z;
				Vector3 localPosition = new Vector3(componentInChildren.transform.localPosition.x, y2, z2);
				componentInChildren.transform.localPosition = localPosition;
				base.transform.localPosition = Vector3.zero;
			}
		}
	}
}

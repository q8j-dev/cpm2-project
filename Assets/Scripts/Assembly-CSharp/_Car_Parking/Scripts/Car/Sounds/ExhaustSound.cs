using System.Collections.Generic;
using System.Threading;
using CPMEngine;
using UnityEngine;
using _Car_Parking.Scripts.Audio;

namespace _Car_Parking.Scripts.Car.Sounds
{
	public class ExhaustSound : MonoBehaviour
	{
		private sealed class CallbackCookie
		{
			public int InstanceId;
		}

		private const float BurbleRpmFrac = 0.45f;

		private const float ShotHeatFrac = 0.4f;

		private const float StrongShotHeatFrac = 0.83f;

		private const float BurbleGapMin = 0.12f;

		private const float BurbleGapMax = 0.4f;

		private const float BangCooldown = 1.5f;

		private const float BangShiftCooldown = 3f;

		private const float BangRevCooldown = 3.5f;

		private const float ThrottleDropRatePerSec = 4f;

		private const float MaxThrottleForBurble = 0.1f;

		private const float ShootShiftingProbability = 0.7f;

		private const float ShootRevProbability = 0.6f;

		private const float ExhaustMaxTemp = 900f;

		private const float ExhaustHeatApproach = 0.7f;

		private const float ExhaustCoolApproach = 0.25f;

		private const float LoadHeatScale = 1.25f;

		private float _throttle;

		private bool _isMufflerShotPlaying;

		private bool _isExhaustBurblePlaying;

		private bool _canBurblePlay;

		private bool _isExhaustHeatPlaying;

		private float _exhaustTemperature;

		private float _lastThrottle;

		private float _lastBangTime;

		private float _lastPopTime;

		private float _burbleGap = 0.12f;

		private int _lastShiftGear = int.MinValue;

		private bool _wasRevLimited;

		private Powertrain _powertrain;

		private SoundController _soundController;

		private static int s_nextInstanceId;

		private static readonly Dictionary<int, ExhaustSound> s_instances = new Dictionary<int, ExhaustSound>();

		private int _instanceId;

		private CallbackCookie _sharedCookie;

		private static readonly uint PlayExhaustShot = AudioWrapper.GetId("Play_Exh_Shot");

		private static readonly uint ExhaustShotSwitch = AudioWrapper.GetId("Exh_Shot_Switch");

		private static readonly uint ExhaustShotType1 = AudioWrapper.GetId("weak_shot");

		private static readonly uint ExhaustShotType2 = AudioWrapper.GetId("strong_shot");

		private static readonly uint BurbleTypeSwitch = AudioWrapper.GetId("Burble_Type_Switch");

		private static readonly uint PlayBurble = AudioWrapper.GetId("Play_Burble_Switch");

		private static readonly uint BurbleType1 = AudioWrapper.GetId("Type_1");

		private static readonly uint BurbleType2 = AudioWrapper.GetId("Type_2");

		private static readonly uint BurbleType3 = AudioWrapper.GetId("Type_3");

		private static readonly uint BurbleType4 = AudioWrapper.GetId("Type_4");

		private static readonly uint BurbleType5 = AudioWrapper.GetId("Type_5");

		private static readonly uint RtpcThrottle = AudioWrapper.GetId("Rtpc_throttle");

		private static readonly uint PlayExhaustHeat = AudioWrapper.GetId("Play_Engine_CoolDown");

		private static readonly uint StopExhaustHeat = AudioWrapper.GetId("Stop_Engine_CoolDown");

		private float ShotTempThreshold => Mathf.Lerp(25f, 900f, 0.4f);

		private float StrongShotTempThreshold => Mathf.Lerp(25f, 900f, 0.83f);

		public void Init(SoundController sc, Powertrain powertrain)
		{
			_soundController = sc;
			_powertrain = powertrain;
		}

		private void Awake()
		{
			_instanceId = Interlocked.Increment(ref s_nextInstanceId);
			s_instances[_instanceId] = this;
			_sharedCookie = new CallbackCookie
			{
				InstanceId = _instanceId
			};
		}

		private void OnDestroy()
		{
			s_instances.Remove(_instanceId);
		}

		public void PlayExhaustBurble()
		{
			if (!_isExhaustBurblePlaying)
			{
				BurbleSwitch(_soundController.BurbleType);
				AudioWrapper.PostEvent(PlayBurble, base.gameObject, 1u, CallExhaustBurbleStatic, _sharedCookie);
				_isExhaustBurblePlaying = true;
				_lastPopTime = Time.time;
				_burbleGap = Random.Range(0.12f, 0.4f);
				_soundController.PlayExhaustBurbleParticle();
			}
		}

		private static void CallExhaustBurbleStatic(object inCookie, AkCallbackType callType, object inInfo)
		{
			if (callType == AkCallbackType.AK_EndOfEvent && inCookie is CallbackCookie callbackCookie && s_instances.TryGetValue(callbackCookie.InstanceId, out var value) && !(value == null))
			{
				value._isExhaustBurblePlaying = false;
			}
		}

		private void PlayExhaustShoot(int type)
		{
			if (!_isMufflerShotPlaying)
			{
				if ((bool)_soundController.ExhaustSystem)
				{
					_soundController.ExhaustSystem.Play();
				}
				ExhaustShootSwitch(type);
				_soundController.PlayExhaustShootParticle(type);
				AudioWrapper.PostEvent(PlayExhaustShot, base.gameObject, 1u, CallMufflerShootStatic, new CallbackCookie
				{
					InstanceId = _instanceId
				});
				_isMufflerShotPlaying = true;
			}
		}

		private static void CallMufflerShootStatic(object inCookie, AkCallbackType callType, object inInfo)
		{
			if (callType == AkCallbackType.AK_EndOfEvent && inCookie is CallbackCookie callbackCookie && s_instances.TryGetValue(callbackCookie.InstanceId, out var value) && !(value == null))
			{
				value._isMufflerShotPlaying = false;
			}
		}

		private void ExhaustShootSwitch(int type)
		{
			switch (type)
			{
			case 0:
				AudioWrapper.SetSwitch(ExhaustShotSwitch, ExhaustShotType1, base.gameObject);
				break;
			case 1:
				AudioWrapper.SetSwitch(ExhaustShotSwitch, ExhaustShotType2, base.gameObject);
				break;
			}
		}

		private void BurbleSwitch(int type)
		{
			switch (type)
			{
			case 0:
				break;
			case 1:
				AudioWrapper.SetSwitch(BurbleTypeSwitch, BurbleType1, base.gameObject);
				break;
			case 2:
				AudioWrapper.SetSwitch(BurbleTypeSwitch, BurbleType2, base.gameObject);
				break;
			case 3:
				AudioWrapper.SetSwitch(BurbleTypeSwitch, BurbleType3, base.gameObject);
				break;
			case 4:
				AudioWrapper.SetSwitch(BurbleTypeSwitch, BurbleType4, base.gameObject);
				break;
			case 5:
				AudioWrapper.SetSwitch(BurbleTypeSwitch, BurbleType5, base.gameObject);
				break;
			}
		}

		private void Update()
		{
			if (!_powertrain)
			{
				return;
			}
			_throttle = _powertrain.Throttle;
			SimulateExhaustTemperature(_throttle);
			if (!_powertrain.OnLineCar)
			{
				SimulateExhaustHeat();
			}
			if (_powertrain.IsEngineWorks)
			{
				if (!_canBurblePlay)
				{
					_canBurblePlay = _throttle > 0.5f;
				}
				if (ShouldShoot(_throttle, Time.deltaTime))
				{
					PlayExhaustShoot((_exhaustTemperature > StrongShotTempThreshold) ? 1 : 0);
				}
				if (ShouldBurble(_throttle))
				{
					PlayExhaustBurble();
				}
				_lastThrottle = _throttle;
				_lastShiftGear = _powertrain.Gear;
				_wasRevLimited = _powertrain.RevLimiterTriggered;
			}
		}

		private bool ShouldShoot(float throttle, float dt)
		{
			if ((bool)_soundController && !_soundController.HasExhaustShot)
			{
				return false;
			}
			bool flag = _exhaustTemperature >= ShotTempThreshold;
			if ((((throttle - _lastThrottle) / Mathf.Max(dt, 0.0001f) < -4f) & flag) && Time.time > _lastBangTime + 1.5f)
			{
				_lastBangTime = Time.time;
				return true;
			}
			if (_lastShiftGear != int.MinValue && _powertrain.Gear != _lastShiftGear && throttle > 0f && _powertrain.CurrentRpm > _powertrain.EngineIdleRpm * 2f && Time.time > _lastBangTime + 3f && Random.value > 0.7f)
			{
				_lastBangTime = Time.time;
				return true;
			}
			if (((_powertrain.RevLimiterTriggered && !_wasRevLimited) & flag) && Time.time > _lastBangTime + 3.5f && Random.value > 0.6f)
			{
				_lastBangTime = Time.time;
				return true;
			}
			return false;
		}

		private void OnDisable()
		{
			if ((bool)_soundController)
			{
				AudioWrapper.PostEvent(StopExhaustHeat, _soundController.gameObject);
			}
		}

		private bool ShouldBurble(float throttle)
		{
			if ((bool)_soundController && !_soundController.HasBurble)
			{
				return false;
			}
			float num = _powertrain.EngineIdleRpm + (_powertrain.RedlineRpm - _powertrain.EngineIdleRpm) * 0.45f;
			bool num2 = _powertrain.CurrentRpm > num;
			bool flag = _powertrain.EngineCurTorque < 0f;
			bool flag2 = throttle < 0.1f;
			bool flag3 = _exhaustTemperature >= ShotTempThreshold;
			bool flag4 = !_isExhaustBurblePlaying && Time.time > _lastPopTime + _burbleGap;
			return ((num2 & flag & flag2 & flag3) && _canBurblePlay) & flag4;
		}

		private void SimulateExhaustHeat()
		{
			if (_powertrain.IsEngineWorks)
			{
				StopExhaustHeatSound();
			}
			else if (_exhaustTemperature > 130f)
			{
				PlayExhaustHeatSound();
			}
			else
			{
				StopExhaustHeatSound();
			}
		}

		public void PlayExhaustHeatSound()
		{
			if (!_isExhaustHeatPlaying)
			{
				AudioWrapper.PostEvent(PlayExhaustHeat, _soundController.gameObject);
				_isExhaustHeatPlaying = true;
			}
		}

		public void StopExhaustHeatSound()
		{
			if (_isExhaustHeatPlaying)
			{
				AudioWrapper.PostEvent(StopExhaustHeat, _soundController.gameObject);
				_isExhaustHeatPlaying = false;
			}
		}

		private void SimulateExhaustTemperature(float throttle)
		{
			float num = 25f;
			float t = Mathf.Clamp01(_powertrain.CurrentRpm / Mathf.Max(_powertrain.RedlineRpm, 1f));
			float a = throttle * Mathf.Lerp(0.4f, 1f, t);
			float b = Mathf.Clamp01(_powertrain.Load * 1.25f);
			float t2 = Mathf.Clamp01(Mathf.Max(a, b));
			float num2 = Mathf.Lerp(num, 900f, t2);
			float num3 = ((num2 > _exhaustTemperature) ? 0.7f : 0.25f);
			_exhaustTemperature += (num2 - _exhaustTemperature) * Mathf.Clamp01(num3 * Time.deltaTime);
			_exhaustTemperature = Mathf.Clamp(_exhaustTemperature, num, 900f);
			AudioWrapper.SetRTPCValue(RtpcThrottle, throttle, base.gameObject);
		}
	}
}

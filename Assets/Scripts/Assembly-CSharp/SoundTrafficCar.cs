using System;
using System.Linq;
using CPMEngine;
using UnityEngine;
using _Car_Parking;
using _Car_Parking.Scripts.Audio;
using _Car_Parking.Scripts.Audio.SoundBanks;
using _Car_Parking.Scripts.Car.Sounds;

public class SoundTrafficCar : MonoBehaviour
{
	private static readonly uint RTPC_RPM_NPC_car = AudioWrapper.GetId("RTPC_RPM_NPC_car");

	private static readonly uint PlayNPCWheels = AudioWrapper.GetId("Play_NPC_Asph_Roadnoise");

	private static readonly uint StopNPCWheels = AudioWrapper.GetId("Stop_NPC_Asph_Roadnoise");

	private static readonly uint RtpcDoppler = AudioWrapper.GetId("RTPC_Doppler_Pitch");

	public float engineThrottleVolume = 0.35f;

	public GameObject enginePosition;

	public AudioClip brakeNoise;

	public float brakeNoiseVolume = 0.2f;

	public float skidVolume = 1f;

	public float skidPitchFactor = 1f;

	public float scrapeNoiseVolume = 1f;

	private AudioSource startEngineSource;

	private AudioSource brakeNoiseSource;

	public AudioSource[] skidSource;

	private AudioSource scrapeNoiseSource;

	private AudioSource shiftTriggerSource;

	private AudioSource windSource;

	private AudioSource[] rollingNoiseSource;

	private AudioSource offRoadSoundSource;

	private Powertrain _powertrain;

	private Axles axles;

	private bool rimScraping;

	private float s;

	private float volumeFactor;

	private int i;

	private MultiPlayerPosControl mppc;

	public GameObject SoundParent;

	private AudioSource hornSource;

	private bool HornPlaying;

	public int currentHorn = 24;

	public float engineSoundKoff = 1f;

	private float EngineRPM;

	private float oldRPMburble;

	private float steeringOld;

	private bool isEngineSoundPlaying;

	private int soundId;

	private bool asphaltSoundPlaying;

	private bool crashPlaying;

	private float _thr;

	private float _factor;

	private bool _disable;

	private bool _isTrafficSurfacePlaying;

	[SerializeField]
	private Camera _mainCamera;

	private Vector3 _emitterLastPosition = Vector3.zero;

	private Vector3 _listenerLastPosition = Vector3.zero;

	private Camera MainCamera
	{
		get
		{
			if (_mainCamera == null)
			{
				_mainCamera = Camera.main;
			}
			return _mainCamera;
		}
	}

	private void DopplerEffect()
	{
		if ((bool)V.DriverPerson)
		{
			Vector3 rhs = (_emitterLastPosition - base.transform.position) / Time.deltaTime;
			_emitterLastPosition = base.transform.position;
			Vector3 rhs2 = (_listenerLastPosition - V.DriverPerson.transform.position) / Time.deltaTime;
			_listenerLastPosition = V.DriverPerson.transform.position;
			Vector3 lhs = V.DriverPerson.transform.position - base.transform.position;
			float a = Vector3.Dot(lhs, rhs2) / (lhs.magnitude + 1E-06f);
			float a2 = Vector3.Dot(lhs, rhs) / (lhs.magnitude + 1E-06f);
			a = Mathf.Min(a, 335f);
			a2 = Mathf.Min(a2, 335f);
			float value = Utils.RoundTo((335f + a) / (335f + a2) + 1E-06f, 2);
			AudioWrapper.SetRTPCValue(RtpcDoppler, Mathf.Clamp(value, 0f, 2f), base.gameObject);
		}
	}

	private void CheckDistanceToCamera()
	{
		if (!MainCamera)
		{
			_disable = true;
		}
		else if (Vector3.Distance(MainCamera.transform.position, base.transform.position) > 15f)
		{
			if (!_disable)
			{
				StopRollSounds();
			}
			_disable = true;
		}
		else
		{
			_disable = false;
		}
	}

	private void StopRollSounds()
	{
		if (_isTrafficSurfacePlaying)
		{
			AudioWrapper.PostEvent("Stop_NPC_Asph_Roadnoise", base.gameObject);
		}
	}

	private AudioSource CreateAudioSource(AudioClip clip, bool loop, bool playImmediately, Vector3 position)
	{
		GameObject obj = new GameObject("audio");
		obj.transform.parent = base.transform;
		obj.transform.localPosition = position;
		obj.transform.localRotation = Quaternion.identity;
		GameObject gameObject = obj;
		gameObject.AddComponent(typeof(AudioSource));
		gameObject.GetComponent<AudioSource>().clip = clip;
		gameObject.GetComponent<AudioSource>().playOnAwake = false;
		if (loop)
		{
			gameObject.GetComponent<AudioSource>().volume = 0f;
			gameObject.GetComponent<AudioSource>().loop = true;
		}
		else
		{
			gameObject.GetComponent<AudioSource>().loop = false;
		}
		gameObject.name = clip.name;
		gameObject.GetComponent<AudioSource>().spatialBlend = 1f;
		gameObject.transform.parent = SoundParent.transform;
		return gameObject.GetComponent<AudioSource>();
	}

	private void Awake()
	{
		GameObject obj = new GameObject();
		obj.transform.parent = base.gameObject.transform;
		obj.transform.name = "Sound Parent";
		obj.transform.localPosition = Vector3.zero;
		SoundParent = obj;
	}

	private void Start()
	{
		TryGetComponent<Powertrain>(out _powertrain);
		TryGetComponent<Axles>(out axles);
		TryGetComponent<MultiPlayerPosControl>(out mppc);
		_ = Vector3.zero;
		if (enginePosition != null)
		{
			_ = enginePosition.transform.position;
		}
		if ((bool)brakeNoise)
		{
			brakeNoiseSource = CreateAudioSource(brakeNoise, loop: true, playImmediately: false, Vector3.zero);
		}
		Array.Resize(ref skidSource, axles.AllWheels.Count);
		i = 0;
		engineSoundKoff = 0.7f;
		soundId = WiseHelper.SoundIdForNpc();
	}

	private void Update()
	{
		if ((bool)V.CoreGameManager && V.CoreGameManager.SoundBankLoadService != null && V.CoreGameManager.SoundBankLoadService.IsLoaded(SoundBankType.CpmNpcTraffic))
		{
			CheckDistanceToCamera();
			FixedUpdateOld();
			DopplerEffect();
			if ((bool)_powertrain && (bool)brakeNoiseSource)
			{
				brakeNoiseSource.volume = Mathf.Clamp01(Mathf.Abs(AverageWheelVelo()) * 0.1f) * brakeNoiseVolume;
			}
			if (FlagsStorage.IsMainMenu && Mathf.Abs(base.transform.position.z) > 90f)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
			UpdateRollSoundTraffic();
			EngineWorkSound();
		}
	}

	private void EngineWorkSound()
	{
		if (!isEngineSoundPlaying)
		{
			AudioWrapper.PostEvent(TrafficMainMenuTrigger.PlayNpcCar + soundId, base.gameObject);
			isEngineSoundPlaying = true;
		}
		if (isEngineSoundPlaying)
		{
			AudioWrapper.SetRTPCValue(RTPC_RPM_NPC_car, Mathf.Clamp01(_thr * engineThrottleVolume * _factor + engineThrottleVolume * 0.2f * _factor) * engineSoundKoff, base.gameObject);
		}
	}

	private float AverageWheelVelo()
	{
		return axles.AllWheels.Sum((WheelProvider w) => w.Wheel.WheelTireVelocity) / (float)axles.AllWheels.Count;
	}

	private void PlayEngineSounds()
	{
		_thr = _powertrain.Throttle;
		EngineRPM = _powertrain.CurrentRpm;
		EngineRPM = Mathf.Clamp(EngineRPM, 0f, 8000f);
		_factor = EngineRPM / _powertrain.RedlineRpm;
	}

	private void OnDestroy()
	{
		if (isEngineSoundPlaying)
		{
			AudioWrapper.PostEvent($"Stop_Npc_Car_{soundId}", base.gameObject);
		}
		if (_isTrafficSurfacePlaying)
		{
			AudioWrapper.PostEvent(StopNPCWheels, base.gameObject);
		}
	}

	private void UpdateRollSoundTraffic()
	{
		if (_disable)
		{
			return;
		}
		if (_powertrain.SpeedMps < 0.3f)
		{
			if (_isTrafficSurfacePlaying)
			{
				AudioWrapper.PostEvent(StopNPCWheels, base.gameObject);
			}
		}
		else if (!_isTrafficSurfacePlaying)
		{
			AudioWrapper.PostEvent(PlayNPCWheels, base.gameObject, 1u, CallRollWheelTraffic, base.gameObject);
			_isTrafficSurfacePlaying = true;
		}
	}

	private void CallRollWheelTraffic(object in_cookie, AkCallbackType callType, object in_info)
	{
		if (callType == AkCallbackType.AK_EndOfEvent)
		{
			_isTrafficSurfacePlaying = false;
		}
	}

	private void FixedUpdateOld()
	{
		if ((bool)_powertrain)
		{
			PlayEngineSounds();
		}
		rimScraping = false;
		if (!_powertrain || !axles || skidSource == null || skidSource.Length == 0)
		{
			return;
		}
		for (int i = 0; i < 4; i++)
		{
			WheelProvider wheelProvider = axles.AllWheels[i];
			if ((bool)skidSource[i])
			{
				skidSource[i].pitch = skidPitchFactor;
				if (_powertrain.OnLineCar)
				{
					skidSource[i].volume = 0f;
				}
				else
				{
					skidSource[i].volume = Mathf.Clamp(Mathf.Abs(wheelProvider.Wheel.LongitudinalSlip) * 0.0089f, 0f, skidVolume);
				}
				if (skidSource[i].volume <= 0.01f || wheelProvider.CurrentPhysMat.Smoothness != 0f)
				{
					skidSource[i].volume = 0f;
				}
			}
		}
	}

	private void OnCollisionEnter(Collision collInfo)
	{
		if (collInfo.contacts.Length != 0 && collInfo.contacts[0].thisCollider.gameObject.layer != LayerMask.NameToLayer("Wheel") && !collInfo.transform.CompareTag("DriverPerson") && !collInfo.transform.CompareTag("Road"))
		{
			volumeFactor = Mathf.Clamp01(collInfo.relativeVelocity.magnitude * 0.1f);
			volumeFactor *= Mathf.Clamp01(0.3f + Mathf.Abs(Vector3.Dot(collInfo.relativeVelocity.normalized, collInfo.contacts[0].normal)));
			if (collInfo.gameObject.layer != LayerMask.NameToLayer("DriverPerson"))
			{
				AudioWrapper.SetSwitch(SoundController.CarCrash, SoundController.LowSpeedCrush, base.gameObject);
				AudioWrapper.PostEvent(SoundController.PlayCarCrashExtrerior, base.gameObject);
			}
		}
	}

	private void OnCollisionExit()
	{
		if (!rimScraping && (bool)scrapeNoiseSource)
		{
			scrapeNoiseSource.volume = 0f;
			scrapeNoiseSource.loop = false;
		}
	}

	private float SetScrapeNoiseVolume(Collision collInfo, float factor = 1f)
	{
		float num = collInfo.relativeVelocity.magnitude / factor;
		float num2 = 0f;
		if (collInfo.contacts.Length != 0)
		{
			num2 = 1f - Mathf.Abs(Vector3.Dot(collInfo.contacts[0].normal, collInfo.relativeVelocity.normalized));
		}
		return Mathf.Clamp01(num * num2 * scrapeNoiseVolume);
	}
}

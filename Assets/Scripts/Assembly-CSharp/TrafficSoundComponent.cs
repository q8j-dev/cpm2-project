using System;
using CPMEngine;
using Gley.TrafficSystem;
using UniRx;
using Unity.Mathematics;
using UnityEngine;
using _Car_Parking.Scripts.Audio;
using _Car_Parking.Scripts.Car.Sounds;

public class TrafficSoundComponent : MonoBehaviour
{
	private VehicleComponent _vehicle;

	private static readonly uint RtpcRpmNpcCar = AudioWrapper.GetId("RTPC_RPM_NPC_car");

	private static readonly uint[] PlayNpcCar = new uint[3]
	{
		AudioWrapper.GetId("Play_Npc_Car_1"),
		AudioWrapper.GetId("Play_Npc_Car_2"),
		AudioWrapper.GetId("Play_Npc_Car_3")
	};

	private static readonly uint[] StopNpcCar = new uint[3]
	{
		AudioWrapper.GetId("Stop_Npc_Car_1"),
		AudioWrapper.GetId("Stop_Npc_Car_2"),
		AudioWrapper.GetId("Stop_Npc_Car_3")
	};

	private static readonly uint PlayNpcWheels = AudioWrapper.GetId("Play_NPC_Asph_Roadnoise");

	private static readonly uint StopNpcWheels = AudioWrapper.GetId("Stop_NPC_Asph_Roadnoise");

	private static readonly uint PlayHornsSwitch = AudioWrapper.GetId("Play_Horns_Switch");

	private static readonly uint StopHornsSwitch = AudioWrapper.GetId("Stop_Horns_Switch");

	private static readonly uint RtpcDoppler = AudioWrapper.GetId("RTPC_Doppler_Pitch");

	private bool _isEnginePlaying;

	private bool _isSurfacePlaying;

	private bool _disableEngine;

	private bool _disableSurface;

	private bool _crashPlaying;

	private bool _hornPlaying;

	private int _soundId;

	private bool _hornPlayed;

	private float _sqrDistanceToListener;

	private const int EdgeDistance = 30;

	private const int EdgeSurfaceDistance = 15;

	private const float SurfaceStartSpeed = 3f;

	private const float SurfaceStopSpeed = 1f;

	private Vector3 _emitterLastPosition = Vector3.zero;

	private Vector3 _listenerLastPosition = Vector3.zero;

	private Vector3 _emitterSpeed;

	private Vector3 _listenerSpeed;

	private WhooshSoundControllerForBot _whooshController;

	private void Awake()
	{
		TryGetComponent<VehicleComponent>(out _vehicle);
		_whooshController = new WhooshSoundControllerForBot(base.transform);
	}

	private void OnDisable()
	{
		EngineWorkStop();
		RollWheelStop();
		if (_hornPlaying)
		{
			AudioWrapper.PostEvent(StopHornsSwitch, base.gameObject);
		}
		_crashPlaying = false;
		_hornPlaying = false;
	}

	public void PlayHorn()
	{
		if (!_hornPlayed)
		{
			AudioWrapper.SetSwitch("Horns_Switch", "horn_0", base.gameObject);
			AudioWrapper.PostEvent(PlayHornsSwitch, base.gameObject);
			_hornPlaying = true;
			Observable.Timer(TimeSpan.FromSeconds(1.0)).Subscribe((long _) =>
			{
				ResetHornPlayed();
			}).AddTo(this);
		}
	}

	private void ResetHornPlayed()
	{
		if (_hornPlaying)
		{
			AudioWrapper.PostEvent(StopHornsSwitch, base.gameObject);
		}
		_hornPlaying = false;
	}

	private void Start()
	{
		_soundId = UnityEngine.Random.Range(0, 3);
	}

	private void Update()
	{
		UpdateSoundDistanceState();
		UpdateRollSoundTraffic();
		UpdateEngineSound();
		_whooshController.PlayWhoosh(_listenerSpeed, _emitterSpeed);
		DopplerEffect();
	}

	private void UpdateSoundDistanceState()
	{
		if ((bool)V.CoreGameManager)
		{
			_sqrDistanceToListener = (V.CoreGameManager.transform.position - base.transform.position).sqrMagnitude;
			_disableEngine = _sqrDistanceToListener > 900f;
			_disableSurface = _sqrDistanceToListener > 225f;
		}
	}

	private void DopplerEffect()
	{
		if ((bool)V.DriverPerson)
		{
			_emitterSpeed = (_emitterLastPosition - base.transform.position) / Time.deltaTime;
			_emitterLastPosition = base.transform.position;
			Vector3 position = V.DriverPerson.transform.position;
			_listenerSpeed = (_listenerLastPosition - position) / Time.deltaTime;
			_listenerLastPosition = position;
			if (!_disableEngine)
			{
				Vector3 lhs = position - base.transform.position;
				float magnitude = lhs.magnitude;
				float a = Vector3.Dot(lhs, _listenerSpeed) / (magnitude + 1E-06f);
				float a2 = Vector3.Dot(lhs, _emitterSpeed) / (magnitude + 1E-06f);
				a = Mathf.Min(a, 335f);
				a2 = Mathf.Min(a2, 335f);
				float in_value = Utils.RoundTo((335f + a) / (335f + a2) + 1E-06f, 2);
				AudioWrapper.SetRTPCValue(RtpcDoppler, in_value, base.gameObject);
			}
		}
	}

	private void UpdateEngineSound()
	{
		if (_disableEngine)
		{
			if (_isEnginePlaying)
			{
				EngineWorkStop();
			}
			return;
		}
		if (!_isEnginePlaying)
		{
			AudioWrapper.PostEvent(PlayNpcCar[_soundId], base.gameObject);
			_isEnginePlaying = true;
		}
		float in_value = math.remap(0f, _vehicle.GetMaxSpeed(), 0f, 100f, _vehicle.GetCurrentSpeed());
		AudioWrapper.SetRTPCValue(RtpcRpmNpcCar, in_value, base.gameObject);
	}

	private void EngineWorkStop()
	{
		if (_isEnginePlaying)
		{
			AudioWrapper.PostEvent(StopNpcCar[_soundId], base.gameObject);
			_isEnginePlaying = false;
		}
	}

	private void RollWheelStop()
	{
		if (_isSurfacePlaying)
		{
			AudioWrapper.PostEvent(StopNpcWheels, base.gameObject);
			_isSurfacePlaying = false;
		}
	}

	private void UpdateRollSoundTraffic()
	{
		float currentSpeed = _vehicle.GetCurrentSpeed();
		if (_disableSurface || currentSpeed < 1f)
		{
			if (_isSurfacePlaying)
			{
				RollWheelStop();
			}
		}
		else if (!_isSurfacePlaying && !(currentSpeed < 3f))
		{
			AudioWrapper.PostEvent(PlayNpcWheels, base.gameObject);
			_isSurfacePlaying = true;
		}
	}

	private void OnCollisionEnter(Collision collInfo)
	{
		if (collInfo.contacts.Length != 0 && collInfo.contacts[0].thisCollider.gameObject.layer != LayerMask.NameToLayer("Wheel") && !collInfo.transform.CompareTag("DriverPerson") && !collInfo.transform.CompareTag("Road") && collInfo.gameObject.layer != LayerMask.NameToLayer("DriverPerson"))
		{
			AudioWrapper.SetSwitch(SoundController.CarCrash, SoundController.LowSpeedCrush, base.gameObject);
			AudioWrapper.PostEvent(SoundController.PlayCarCrashExtrerior, base.gameObject);
		}
	}
}

using Cysharp.Threading.Tasks;
using UnityEngine;
using _Car_Parking.Scripts.Audio;
using _Car_Parking.Scripts.Audio.SoundBanks;
using _Car_Parking.Scripts.Car.Configs;
using _Car_Parking.Scripts.Person.Locomotion;
using _Car_Parking.Scripts.Person.Locomotion.Audio;

public class StepSounds : MonoBehaviour
{
	private static readonly uint FootstepWalk = AudioWrapper.GetId("Play_Footstep_Walk_Type_Surface");

	private static readonly uint Clothes = AudioWrapper.GetId("Play_P_L_Clothes");

	private static readonly uint SurfaceSwitch = AudioWrapper.GetId("Locomotion_Surface_Switch");

	private static readonly uint PlayerSpeedRtpc = AudioWrapper.GetId("RTPC_Player_Speed");

	private const float Threshold = 0.2f;

	private const float WalkSpeed = 1.5f;

	private const float RunSpeed = 3.2f;

	private const float SprintSpeed = 5f;

	private float _time;

	private bool _soundPlayable = true;

	private LayerMask _layerMask;

	private RaycastHit _hitDown;

	private Surface _currentSurface;

	private MeshCollider _meshCollider;

	private RideableMeshesGraph _rideableMeshesGraph;

	private DriverActionController _driverActionController;

	private SoundBankLoadService _soundBankLoadService;

	private PersonLocomotion _locomotion;

	private bool NewLocomotionOwnsSteps
	{
		get
		{
			if ((bool)_locomotion)
			{
				return _locomotion.IsActive;
			}
			return false;
		}
	}

	private void Awake()
	{
		_rideableMeshesGraph = WheelSurfaceConfigContainer.rideableMeshesGraph;
	}

	private void Start()
	{
		_driverActionController = GetComponent<DriverActionController>();
		_locomotion = GetComponent<PersonLocomotion>();
		_layerMask = ~(1 << LayerMask.NameToLayer("car"));
		_soundBankLoadService = (V.CoreGameManager ? V.CoreGameManager.SoundBankLoadService : null);
		WiseHelper.SetAudioWiseComponents(base.gameObject, addColliderTrigger: false);
	}

	private void OnDestroy()
	{
		_driverActionController = null;
		_locomotion = null;
		_rideableMeshesGraph = null;
		_meshCollider = null;
		_soundBankLoadService = null;
	}

	private void Update()
	{
		if ((bool)_driverActionController && !_driverActionController.PersonInCar)
		{
			if (_time < 0.2f)
			{
				_time += Time.deltaTime;
				_soundPlayable = false;
			}
			else
			{
				_soundPlayable = true;
			}
		}
	}

	public void Footstep()
	{
		PlayFootstep(1.5f);
	}

	public void FootstepRun()
	{
		PlayFootstep(3.2f);
	}

	public void FootstepSprint()
	{
		PlayFootstep(5f);
	}

	private void PlayFootstep(float speed)
	{
		if (!NewLocomotionOwnsSteps && _soundPlayable)
		{
			AudioWrapper.SetSwitch(SurfaceSwitch, LocomotionSurfaceResolver.ToSwitch(_currentSurface, wet: false), base.gameObject);
			AudioWrapper.SetRTPCValue(PlayerSpeedRtpc, speed, base.gameObject);
			if (_soundBankLoadService != null)
			{
				_soundBankLoadService.PostEvent(FootstepWalk, SoundBankType.SfxMain, base.gameObject).Forget();
				_soundBankLoadService.PostEvent(Clothes, SoundBankType.SfxMain, base.gameObject).Forget();
			}
			else
			{
				AudioWrapper.PostEvent(FootstepWalk, base.gameObject);
				AudioWrapper.PostEvent(Clothes, base.gameObject);
			}
			_time = 0f;
		}
	}

	private void FixedUpdate()
	{
		if ((bool)_driverActionController && !_driverActionController.PersonInCar && !NewLocomotionOwnsSteps && Physics.Raycast(base.transform.position, -Vector3.up, out _hitDown, 1f, _layerMask, QueryTriggerInteraction.Ignore))
		{
			_currentSurface = Surface.Dryasphalt;
			_meshCollider = _hitDown.collider as MeshCollider;
			if (!(_meshCollider == null) && !(_meshCollider.sharedMesh == null) && _rideableMeshesGraph != null)
			{
				_currentSurface = _rideableMeshesGraph.GetSurface(_hitDown.collider, _hitDown.triangleIndex, _hitDown.barycentricCoordinate);
			}
		}
	}
}

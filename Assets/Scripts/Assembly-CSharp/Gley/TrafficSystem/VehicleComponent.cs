using System.Collections.Generic;
using System.Linq;
using Gley.TrafficSystem.Internal;
using Gley.UrbanSystem.Internal;
using UnityEngine;
using _Car_Parking.Scripts.TrafficSystem;

namespace Gley.TrafficSystem
{
	[HelpURL("https://gley.gitbook.io/mobile-traffic-system-v3/setup-guide/vehicle-implementation")]
	[RequireComponent(typeof(Rigidbody))]
	public class VehicleComponent : MonoBehaviour, ITrafficParticipant
	{
		[Tooltip("RigidBody of the vehicle")]
		[Header("Object References")]
		public Rigidbody rb;

		[Tooltip("MeshRenderer used for applying random color from the color pool")]
		public MeshRenderer meshRenderer;

		[Tooltip("Empty GameObject used to rotate the vehicle from the correct point")]
		public Transform carHolder;

		[Tooltip("Front trigger used to detect obstacle. It is automatically generated")]
		public Transform frontTrigger;

		[Tooltip("Assign this object if you need a hard shadow on your vehicle, leave it blank otherwise")]
		public Transform shadowHolder;

		[Tooltip("A transform representing the front of your vehicle")]
		public Transform _frontPosition;

		[Tooltip("A transform representing the back of your vehicle")]
		public Transform _backPosition;

		[Tooltip("All vehicle wheels and their properties")]
		[Header("Wheels")]
		public Wheel[] allWheels;

		[Tooltip("Max wheel turn amount in degrees")]
		public float maxSteer = 30f;

		[Tooltip("If suspension is set to 0, the value of suspension will be half of the wheel radius")]
		public float maxSuspension;

		[Tooltip("How rigid the suspension will be. Higher the value -> more rigid the suspension")]
		public float springStiffness = 5f;

		[SerializeField]
		[Range(0f, 1f)]
		[Tooltip("Factor used to calculate the leaning of the vehicle when turning. 0 = no leaning, 1 = max leaning")]
		private float sideLeaningFactor = 0.5f;

		[Tooltip("Factor used to calculate the forward leaning of the vehicle when accelerating/braking. 0 = no leaning, 1 = max leaning")]
		[Range(0f, 1f)]
		[SerializeField]
		private float forwardLeaningFactor = 0.5f;

		[Tooltip("Vehicle type used for making custom paths")]
		[Header("Car Properties")]
		public VehicleTypes vehicleType;

		[Tooltip("Min vehicle speed. Actual vehicle speed is picked random between min and max")]
		public int minPossibleSpeed = 40;

		[Tooltip("Max vehicle speed")]
		public int maxPossibleSpeed = 90;

		[Tooltip("Time in seconds to reach max speed (acceleration)")]
		public float accelerationTime = 10f;

		[Tooltip("Time in seconds to stop from max speed")]
		public float brakeTime = 10f;

		[Tooltip("Time in seconds to turn to maxSteer")]
		public float steeringTime = 1.5f;

		[Tooltip("Distance to keep from an obstacle/vehicle")]
		public float distanceToStop = 3f;

		[Tooltip("Car starts braking when an obstacle enters trigger. Total length of the trigger = distanceToStop+minTriggerLength")]
		public float triggerLength = 4f;

		[HideInInspector]
		public bool updateTrigger;

		[HideInInspector]
		public float maxTriggerLength = 10f;

		[HideInInspector]
		public TrailerComponent trailer;

		[HideInInspector]
		public Transform trailerConnectionPoint;

		[HideInInspector]
		public float length;

		[HideInInspector]
		public float coliderHeight;

		[HideInInspector]
		public float wheelDistance;

		[HideInInspector]
		public VisibilityScript visibilityScript;

		private Collider[] _allColliders;

		private List<Obstacle> _obstacleList;

		private Transform _frontAxle;

		private BoxCollider _frontCollider;

		private ModifyTriggerSize _modifyTriggerSize;

		private EngineSoundComponent _engineSound;

		private BlinkersController _blinkersController;

		private MovementInfo _movementInfo;

		private LayerMask _buildingLayers;

		private LayerMask _obstacleLayers;

		private LayerMask _playerLayers;

		private LayerMask _roadLayers;

		private IVehicleLightsComponent _vehicleLights;

		private TrafficSoundComponent _soundComponent;

		private TrafficHornController _trafficHornController;

		private float _springForce;

		private float _maxSpeedMS;

		private float _storedMaxSpeed;

		private float _minTriggerLength;

		private float _colliderWidth;

		private float _powerStep;

		private float _brakeStep;

		private float _acceleration;

		private float _steerStep;

		private float _steerAngle;

		private int _listIndex;

		private bool _lightsOn;

		private bool _ignored;

		[field: SerializeField]
		public Transform WheelsParent { get; private set; }

		public bool LightsOn => _lightsOn;

		public Collider[] AllColliders => _allColliders;

		public List<Obstacle> Obstacles => _obstacleList;

		public BlinkersController BlinkersController => _blinkersController;

		public Transform FrontTrigger => frontTrigger;

		public MovementInfo MovementInfo => _movementInfo;

		public VehicleTypes VehicleType => vehicleType;

		public float ColliderWidth => _colliderWidth;

		public float MaxSpeed => _maxSpeedMS;

		public float SpringForce => _springForce;

		public float MaxSteer => maxSteer;

		public float PowerStep => _powerStep;

		public float BrakeStep => _brakeStep;

		public float SteerStep => _steerStep;

		public float SteerAngle => _steerAngle;

		public float SpringStiffness => springStiffness;

		public float SideLeaningFactor => sideLeaningFactor;

		public float ForwardLeaningFactor => forwardLeaningFactor;

		public int ListIndex => _listIndex;

		public Transform BackPosition
		{
			get
			{
				if (_backPosition == null)
				{
					_backPosition = base.transform;
				}
				return _backPosition;
			}
		}

		public Transform FrontPosition
		{
			get
			{
				if (_frontPosition == null)
				{
					_frontPosition = base.transform;
				}
				return _frontPosition;
			}
		}

		public bool Ignored
		{
			get
			{
				return _ignored;
			}
			set
			{
				_ignored = value;
			}
		}

		public bool HasTrailer => trailer != null;

		public virtual VehicleComponent Initialize(LayerMask buildingLayers, LayerMask obstacleLayers, LayerMask playerLayers, LayerMask roadLayers, bool lightsOn, ModifyTriggerSize modifyTriggerSize, TrafficWaypointsData trafficWaypointsData, int vehicleIndex, bool ignored, float minOffset, float maxOffset)
		{
			_ignored = ignored;
			_buildingLayers = buildingLayers;
			_obstacleLayers = obstacleLayers;
			_playerLayers = playerLayers;
			_roadLayers = roadLayers;
			_modifyTriggerSize = modifyTriggerSize;
			_allColliders = GetComponentsInChildren<Collider>();
			_springForce = rb.mass * (0f - Physics.gravity.y) / (float)allWheels.Length;
			_frontCollider = frontTrigger.GetChild(0).GetComponent<BoxCollider>();
			_colliderWidth = _frontCollider.size.x;
			_minTriggerLength = _frontCollider.size.z;
			_frontAxle = new GameObject("FrontAxle").transform;
			_frontAxle.transform.SetParent(frontTrigger.parent);
			_frontAxle.transform.position = frontTrigger.position;
			DeactivateVehicle();
			Vector3 zero = Vector3.zero;
			for (int i = 0; i < allWheels.Length; i++)
			{
				allWheels[i].wheelTransform.Translate(Vector3.up * (allWheels[i].maxSuspension / 2f + allWheels[i].wheelRadius));
				zero += allWheels[i].wheelTransform.position;
			}
			rb.centerOfMass = zero / allWheels.Length;
			if (!TryGetComponent<TrafficSoundComponent>(out _soundComponent))
			{
				_soundComponent = base.gameObject.AddComponent<TrafficSoundComponent>();
			}
			_engineSound = GetComponent<EngineSoundComponent>();
			if ((bool)_engineSound)
			{
				_engineSound.Initialize();
			}
			_lightsOn = lightsOn;
			_vehicleLights = GetComponent<VehicleLightsComponent>();
			if (_vehicleLights == null)
			{
				_vehicleLights = GetComponent<VehicleLightsComponentV2>();
			}
			if (_vehicleLights != null)
			{
				_vehicleLights.Initialize();
			}
			if (trailer != null)
			{
				trailer.Initialize(this);
			}
			_listIndex = vehicleIndex;
			_movementInfo = new MovementInfo(_listIndex, ColliderWidth, Random.Range(minOffset, maxOffset));
			_blinkersController = new BlinkersController(_movementInfo, trafficWaypointsData, _vehicleLights);
			_steerStep = maxSteer / steeringTime * Time.fixedDeltaTime;
			_trafficHornController = new TrafficHornController(this, _soundComponent);
			return this;
		}

		protected virtual void OnCollisionEnter(Collision collision)
		{
			Events.TriggerVehicleCrashEvent(_listIndex, GetObstacleTypes(collision.collider), collision.collider);
			_trafficHornController?.OnCollisionEnter(collision);
		}

		protected virtual void OnTriggerExit(Collider other)
		{
			if (!other.isTrigger && (other.gameObject.layer == base.gameObject.layer || (int)_buildingLayers == ((int)_buildingLayers | (1 << other.gameObject.layer)) || (int)_obstacleLayers == ((int)_obstacleLayers | (1 << other.gameObject.layer)) || (int)_playerLayers == ((int)_playerLayers | (1 << other.gameObject.layer))))
			{
				for (int i = 0; i < _obstacleList.Count; i++)
				{
					if (_obstacleList[i].Collider == other)
					{
						Obstacle obstacleToRemove = _obstacleList[i];
						_obstacleList.RemoveAt(i);
						VehicleEvents.TriggerObstacleInTriggerRemovedEvent(_listIndex, obstacleToRemove);
					}
				}
			}
			_trafficHornController?.OnTriggerExit(other);
		}

		protected virtual void OnTriggerEnter(Collider other)
		{
			if (!other.isTrigger)
			{
				NewColliderHit(other);
			}
			_trafficHornController?.OnTriggerEnter(other);
		}

		private void OnDisable()
		{
			_trafficHornController?.OnDisable();
		}

		public virtual void ApplyAdditionalForces(float wheelTurnAngle)
		{
			_steerAngle = wheelTurnAngle;
		}

		public void SetVelocity(Vector3 initialVelocity, Vector3 angularVelocity)
		{
			rb.velocity = initialVelocity;
			rb.angularVelocity = angularVelocity;
		}

		public void SetTriggerSizeModifierDelegate(ModifyTriggerSize triggerSizeModifier)
		{
			_modifyTriggerSize = triggerSizeModifier;
		}

		public virtual void ActivateVehicle(Vector3 position, Quaternion vehicleRotation, Quaternion trailerRotation)
		{
			_storedMaxSpeed = Random.Range(minPossibleSpeed, maxPossibleSpeed);
			_maxSpeedMS = _storedMaxSpeed.KMHToMS();
			int num = (int)(accelerationTime / Time.fixedDeltaTime);
			_powerStep = MaxSpeed / (float)num;
			_acceleration = _powerStep / Time.fixedDeltaTime;
			num = (int)(brakeTime / Time.fixedDeltaTime);
			_brakeStep = MaxSpeed / (float)num;
			base.gameObject.transform.SetPositionAndRotation(position, vehicleRotation);
			float num2 = Vector3.Distance(position, frontTrigger.transform.position);
			base.transform.Translate(-base.transform.forward * num2, Space.World);
			if (trailer != null)
			{
				trailer.transform.rotation = trailerRotation;
			}
			base.gameObject.SetActive(value: true);
			if ((bool)_engineSound)
			{
				_engineSound.Play(0f);
			}
			SetMainLights(_lightsOn);
		}

		public virtual void DeactivateVehicle()
		{
			base.gameObject.SetActive(value: false);
			_obstacleList = new List<Obstacle>();
			visibilityScript.Reset();
			if ((bool)_engineSound)
			{
				_engineSound.Stop();
			}
			_vehicleLights?.DeactivateLights();
			if ((bool)trailer)
			{
				trailer.DeactivateVehicle();
			}
		}

		public float GetRayCastLength()
		{
			return allWheels[0].raycastLength;
		}

		public float GetWheelCircumference()
		{
			return allWheels[0].wheelCircumference;
		}

		public Vector3 GetVelocity()
		{
			return rb.velocity;
		}

		public float GetCurrentSpeedMS()
		{
			return GetVelocity().magnitude;
		}

		public float GetCurrentSpeed()
		{
			return GetVelocity().magnitude * 3.6f;
		}

		public float GetMaxSpeed()
		{
			return MaxSpeed;
		}

		public Vector3 GetHeading()
		{
			return frontTrigger.forward;
		}

		public Vector3 GetForwardVector()
		{
			return base.transform.forward;
		}

		public bool CanBeRemoved()
		{
			return visibilityScript.IsNotInView();
		}

		public Vector3 GetFrontAxleUpVector()
		{
			return _frontAxle.up;
		}

		public Vector3 GetFrontAxleForwardVector()
		{
			return _frontAxle.forward;
		}

		public Vector3 GetFrontAxleRightVector()
		{
			return _frontAxle.right;
		}

		public Vector3 GetFrontAxlePosition()
		{
			return _frontAxle.position;
		}

		public int GetNrOfWheels()
		{
			return allWheels.Length;
		}

		public int GetTrailerWheels()
		{
			if (trailer == null)
			{
				return 0;
			}
			return trailer.GetNrOfWheels();
		}

		public bool AlreadyCollidingWith(Collider[] colliders)
		{
			for (int i = 0; i < _obstacleList.Count; i++)
			{
				for (int j = 0; j < colliders.Length; j++)
				{
					if (_obstacleList[i].Collider == colliders[j])
					{
						return true;
					}
				}
			}
			return false;
		}

		public void ColliderRemoved(Collider collider)
		{
			if (_obstacleList != null && _obstacleList.Any((Obstacle cond) => cond.Collider == collider))
			{
				OnTriggerExit(collider);
			}
		}

		public void ColliderRemoved(Collider[] colliders)
		{
			int i;
			for (i = 0; i < colliders.Length; i++)
			{
				if (_obstacleList.Any((Obstacle cond) => cond.Collider == colliders[i]))
				{
					OnTriggerExit(colliders[i]);
				}
			}
		}

		internal void SetMainLights(bool on)
		{
			if (on != _lightsOn)
			{
				_lightsOn = on;
			}
			if (_vehicleLights != null)
			{
				_vehicleLights.SetMainLights(on);
			}
		}

		public void SetReverseLights(bool active)
		{
			if (_vehicleLights != null)
			{
				_vehicleLights.SetReverseLights(active);
			}
		}

		public void SetBrakeLights(bool active)
		{
			if (_vehicleLights != null)
			{
				_vehicleLights.SetBrakeLights(active);
			}
		}

		public virtual void SetBlinker(BlinkType blinkType)
		{
			if (_vehicleLights != null)
			{
				_vehicleLights.SetBlinker(blinkType);
			}
		}

		public void UpdateLights(float realtimeSinceStartup)
		{
			if (_vehicleLights != null)
			{
				_vehicleLights.UpdateLights(realtimeSinceStartup);
			}
		}

		public void UpdateEngineSound(float masterVolume)
		{
			if ((bool)_engineSound)
			{
				_engineSound.UpdateEngineSound(GetCurrentSpeedMS(), MaxSpeed, masterVolume);
			}
		}

		public void UpdateColliderSize()
		{
			if (updateTrigger)
			{
				_modifyTriggerSize?.Invoke(GetVelocity().magnitude * 3.6f, _frontCollider, _storedMaxSpeed, _minTriggerLength, maxTriggerLength);
			}
		}

		public virtual void UpdateVehicleScripts(float volume, float realTimeSinceStartup, bool reverseLightsOn)
		{
			UpdateEngineSound(volume);
			UpdateLights(realTimeSinceStartup);
			UpdateColliderSize();
			SetReverseLights(reverseLightsOn);
		}

		public float GetTimeToCoverDistance(float distance)
		{
			float num = (MaxSpeed - GetCurrentSpeedMS()) / _acceleration;
			float num2 = GetCurrentSpeedMS() * num + 0.5f * _acceleration * num * num;
			if (num2 >= distance)
			{
				return (0f - GetCurrentSpeedMS() + Mathf.Sqrt(GetCurrentSpeedMS() * GetCurrentSpeedMS() + 2f * _acceleration * distance)) / _acceleration;
			}
			float num3 = (distance - num2) / MaxSpeed;
			return num + num3;
		}

		private void NewColliderHit(Collider other)
		{
			ObstacleTypes obstacleTypes = GetObstacleTypes(other);
			if (obstacleTypes == ObstacleTypes.Other || obstacleTypes == ObstacleTypes.Road || _obstacleList.Any((Obstacle cond) => cond.Collider == other))
			{
				return;
			}
			bool isConvex = true;
			if (other is MeshCollider meshCollider)
			{
				isConvex = meshCollider.convex;
			}
			ITrafficParticipant vehicleScript = null;
			if (obstacleTypes == ObstacleTypes.TrafficVehicle || obstacleTypes == ObstacleTypes.Player)
			{
				Rigidbody attachedRigidbody = other.attachedRigidbody;
				if (attachedRigidbody != null)
				{
					vehicleScript = attachedRigidbody.GetComponent<ITrafficParticipant>();
				}
			}
			_obstacleList.Add(new Obstacle(other, isConvex, obstacleTypes, vehicleScript));
			VehicleEvents.TriggerObstacleInTriggerAddedEvent(_listIndex, _obstacleList[_obstacleList.Count - 1]);
		}

		private ObstacleTypes GetObstacleTypes(Collider other)
		{
			if (other.gameObject.layer == base.gameObject.layer)
			{
				Rigidbody attachedRigidbody = other.attachedRigidbody;
				if (attachedRigidbody != null && attachedRigidbody.GetComponent<ITrafficParticipant>() != null)
				{
					return ObstacleTypes.TrafficVehicle;
				}
				return ObstacleTypes.DynamicObject;
			}
			if ((int)_buildingLayers == ((int)_buildingLayers | (1 << other.gameObject.layer)))
			{
				return ObstacleTypes.StaticObject;
			}
			if ((int)_obstacleLayers == ((int)_obstacleLayers | (1 << other.gameObject.layer)))
			{
				return ObstacleTypes.DynamicObject;
			}
			if ((int)_playerLayers == ((int)_playerLayers | (1 << other.gameObject.layer)))
			{
				return ObstacleTypes.Player;
			}
			if ((int)_roadLayers == ((int)_roadLayers | (1 << other.gameObject.layer)))
			{
				return ObstacleTypes.Road;
			}
			return ObstacleTypes.Other;
		}

		private void OnDestroy()
		{
			if (meshRenderer != null)
			{
				Object.Destroy(meshRenderer.material);
			}
			_trafficHornController?.Dispose();
		}
	}
}

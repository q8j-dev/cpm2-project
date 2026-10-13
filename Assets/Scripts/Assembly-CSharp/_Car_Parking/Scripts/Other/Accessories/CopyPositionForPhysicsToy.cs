using CPMEngine;
using UnityEngine;

namespace _Car_Parking.Scripts.Other.Accessories
{
	public class CopyPositionForPhysicsToy : MonoBehaviour
	{
		private const float MinDeltaTime = 0.001f;

		[Header("Delta to spawn reference object")]
		[SerializeField]
		private Vector3 positionToStart = new Vector3(0f, -100f, 0f);

		[Header("Max reference speed")]
		[SerializeField]
		private float maxLengthVelocity = 3f;

		[SerializeField]
		[Header("View")]
		private Transform localObjectTransform = default;

		[Header("Distance to root")]
		[SerializeField]
		private float localPositionScale = 0.1f;

		[SerializeField]
		[Header("Another type of physics calculation. More active.")]
		private bool useAcceleration = default;

		[SerializeField]
		[Header("Use local Rotation")]
		private bool useLocalRotation = default;

		[Header("Prefab physics toy reference. Probably contains hinge joint.")]
		[SerializeField]
		private ReferencePhysicsObject referencePhysicsObjectPrefab = default;

		private Rigidbody _carRb;

		private ReferencePhysicsObject _referencePhysicsObject;

		private Vector3 _lastVelocity;

		private Vector3 _acceleration;

		private Vector3 _lastTransformPosition;

		private Transform _carTransform;

		private Vector3 _transformVelocity;

		private float _maxLengthOfVector = 10000f;

		private void Start()
		{
			_referencePhysicsObject = Object.Instantiate(referencePhysicsObjectPrefab, positionToStart, Quaternion.identity);
			_carRb = GetComponentInParent<Powertrain>().GetComponent<Rigidbody>();
			if (_carRb == null)
			{
				_carRb = GetComponent<Rigidbody>();
			}
			_carTransform = _carRb.transform;
			_lastVelocity = Vector3.zero;
		}

		private void OnDestroy()
		{
			if (_referencePhysicsObject != null)
			{
				Object.Destroy(_referencePhysicsObject.gameObject);
			}
		}

		private void Update()
		{
			CalculateSpeed();
			CopyFromReference();
		}

		private void CalculateSpeed()
		{
			Vector3 vector = _carTransform.position - _lastTransformPosition;
			float num = Mathf.Clamp(Time.deltaTime, 0.001f, Time.deltaTime);
			_transformVelocity = vector / num;
			if (_carRb.isKinematic)
			{
				_transformVelocity *= -1f;
			}
			if (_transformVelocity.magnitude < 0.1f)
			{
				_transformVelocity = Vector3.zero;
			}
			_lastTransformPosition = _carTransform.position;
		}

		private void LateUpdate()
		{
			MoveReference();
		}

		private void MoveReference()
		{
			Vector3 transformVelocity = _transformVelocity;
			Vector3 vector = transformVelocity - _lastVelocity;
			if (vector.magnitude > _maxLengthOfVector)
			{
				vector = Vector3.ClampMagnitude(vector, _maxLengthOfVector);
			}
			float num = Mathf.Clamp(Time.deltaTime, 0.001f, Time.deltaTime);
			_acceleration = vector / num;
			_lastVelocity = transformVelocity;
			Vector3 velocity = Vector3.ClampMagnitude(useAcceleration ? _acceleration : transformVelocity, maxLengthVelocity);
			_referencePhysicsObject.rb.velocity = velocity;
		}

		private void CopyFromReference()
		{
			if (!useLocalRotation)
			{
				base.transform.rotation = Quaternion.identity;
			}
			Vector3 localPosition = (-_referencePhysicsObject.transform.position + _referencePhysicsObject.localObject.position) * localPositionScale;
			localObjectTransform.localPosition = localPosition;
			if (useLocalRotation)
			{
				localObjectTransform.localRotation = _referencePhysicsObject.localObject.localRotation;
			}
			else
			{
				localObjectTransform.rotation = _referencePhysicsObject.localObject.rotation;
			}
		}
	}
}

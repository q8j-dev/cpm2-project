using UnityEngine;
using _Car_Parking.Scripts.Person.Locomotion.Config;

namespace _Car_Parking.Scripts.Person.Locomotion
{
	public class PersonGroundProbe
	{
		private const bool DiagCrouchStand = false;

		private readonly Transform _transform;

		private readonly CapsuleCollider _capsuleCollider;

		private readonly Rigidbody _rigidbody;

		private readonly PersonLocomotionConfig _locomotion;

		private readonly PersonCapsuleConfig _capsule;

		private readonly PersonSlopeRules _slopeRules;

		private readonly Collider[] _ceilingHits = new Collider[32];

		public float VerticalVelocity
		{
			get
			{
				if (!_rigidbody)
				{
					return 0f;
				}
				return _rigidbody.velocity.y;
			}
		}

		public PersonGroundProbe(Transform transform, CapsuleCollider capsuleCollider, Rigidbody rigidbody, PersonLocomotionConfig locomotion, PersonCapsuleConfig capsule, PersonSlopeRules slopeRules)
		{
			_transform = transform;
			_capsuleCollider = capsuleCollider;
			_rigidbody = rigidbody;
			_locomotion = locomotion;
			_capsule = capsule;
			_slopeRules = slopeRules;
		}

		public bool TryProbe(out GroundInfo ground)
		{
			if (!_locomotion || !_capsuleCollider)
			{
				ground = default;
				return false;
			}
			float num = Mathf.Min(_locomotion.GroundCheckRadius, _capsuleCollider.radius * 0.95f);
			float num2 = _capsuleCollider.center.y - _capsuleCollider.height * 0.5f + num;
			if (!Physics.SphereCast(_transform.position + new Vector3(_capsuleCollider.center.x, num2 + 0.08f, _capsuleCollider.center.z), layerMask: (int)_locomotion.GroundLayerMask & ~(1 << _transform.gameObject.layer), radius: num, direction: Vector3.down, hitInfo: out var hitInfo, maxDistance: 0.08f + _locomotion.GroundCheckDistance, queryTriggerInteraction: QueryTriggerInteraction.Ignore))
			{
				ground = new GroundInfo
				{
					Grounded = false
				};
				return true;
			}
			float num3 = Vector3.Angle(hitInfo.normal, Vector3.up);
			ground = new GroundInfo
			{
				Grounded = true,
				Normal = hitInfo.normal,
				Point = hitInfo.point,
				SlopeAngle = num3,
				Collider = hitInfo.collider,
				IsSteepSlope = (num3 > _slopeRules.MaxSlopeAngle)
			};
			return true;
		}

		public bool TryDetectWall(Vector3 flatDir, out RaycastHit hit)
		{
			hit = default;
			if (!_locomotion || !_capsuleCollider)
			{
				return false;
			}
			Vector3 vector = flatDir;
			vector.y = 0f;
			if (vector.sqrMagnitude < 0.0001f)
			{
				return false;
			}
			vector.Normalize();
			float radius = _capsuleCollider.radius * 0.9f;
			Vector3 vector2 = _transform.position + _capsuleCollider.center;
			float num = _capsuleCollider.radius + 0.05f;
			if (!Physics.SphereCast(vector2 - vector * num, maxDistance: num + _locomotion.WallProbeDistance, layerMask: (int)_locomotion.WallLayerMask & ~(1 << _transform.gameObject.layer), radius: radius, direction: vector, hitInfo: out hit, queryTriggerInteraction: QueryTriggerInteraction.Ignore))
			{
				return false;
			}
			return Vector3.Angle(hit.normal, Vector3.up) > _locomotion.WallMinSurfaceAngle;
		}

		public bool TryStepUp(float planarInputMagnitude, out StepUpProposal proposal)
		{
			if (!_locomotion || !_capsuleCollider)
			{
				proposal = default;
				return false;
			}
			if (planarInputMagnitude < _locomotion.StepInputThreshold)
			{
				proposal = default;
				return false;
			}
			if (!TryGetForwardBottomHit(out var hit))
			{
				proposal = default;
				return false;
			}
			if (Vector3.Angle(hit.normal, -_transform.forward) >= _locomotion.StepMaxFaceAngle)
			{
				proposal = default;
				return false;
			}
			if ((bool)_rigidbody && _rigidbody.velocity.y > _locomotion.StepMidJumpVelocityThreshold)
			{
				proposal = default;
				return false;
			}
			if (!TryFindLedgeTop(hit, out var ledgePoint))
			{
				proposal = default;
				return false;
			}
			float y = ledgePoint.y;
			float num = y - _transform.position.y;
			if (num <= 0f || num > _locomotion.StepHeight)
			{
				proposal = default;
				return false;
			}
			Vector3 forward = _transform.forward;
			forward.y = 0f;
			forward = ((forward.sqrMagnitude > 0.0001f) ? forward.normalized : _transform.forward);
			float num2 = _capsuleCollider.height * 0.5f - _capsuleCollider.center.y + _locomotion.LandingEpsilon;
			float num3 = _capsuleCollider.radius + 0.05f;
			Vector3 vector = new Vector3(ledgePoint.x + forward.x * num3, y + num2, ledgePoint.z + forward.z * num3);
			if (!IsLandingClear(vector))
			{
				proposal = default;
				return false;
			}
			if (!IsPathToLandingClear(vector))
			{
				proposal = default;
				return false;
			}
			proposal = new StepUpProposal
			{
				TargetPosition = vector,
				ZeroVerticalVelocity = true
			};
			return true;
		}

		public bool HasCeiling(float fromHeight, float toHeight)
		{
			if (!_capsuleCollider || !_capsule)
			{
				return false;
			}
			if (toHeight <= fromHeight)
			{
				return false;
			}
			float radius = _capsuleCollider.radius;
			Vector3 position = _transform.position;
			Vector3 up = _transform.up;
			float num = Mathf.Max(0f, (toHeight - fromHeight) * 0.5f - radius);
			float num2 = (fromHeight + toHeight) * 0.5f;
			Vector3 vector = position + up * num2;
			Vector3 point = vector + up * num;
			Vector3 point2 = vector - up * num;
			int layerMask = (int)_capsule.CeilingCheckMask & ~(1 << _transform.gameObject.layer);
			int num3 = Physics.OverlapCapsuleNonAlloc(point, point2, radius, _ceilingHits, layerMask, QueryTriggerInteraction.Ignore);
			Transform root = _transform.root;
			for (int i = 0; i < num3; i++)
			{
				Collider collider = _ceilingHits[i];
				if ((bool)collider)
				{
					Transform transform = collider.transform;
					if (!(transform == _transform) && !transform.IsChildOf(_transform) && !(transform.root == root))
					{
						return true;
					}
				}
			}
			return false;
		}

		private bool TryGetForwardBottomHit(out RaycastHit hit)
		{
			hit = default;
			Vector3 position = _transform.position;
			Vector3 forward = _transform.forward;
			forward.y = 0f;
			forward = ((forward.sqrMagnitude > 0.0001f) ? forward.normalized : _transform.forward);
			int num = Mathf.Max(1, _locomotion ? _locomotion.StepForwardSampleCount : 6);
			float b = (_locomotion ? _locomotion.StepHeight : 1f);
			for (int i = 0; i < num; i++)
			{
				float t = ((num > 1) ? ((float)i / (float)(num - 1)) : 0f);
				float num2 = Mathf.Lerp(0.05f, b, t);
				if (Physics.Raycast(new Vector3(position.x, position.y + num2, position.z), forward, out var hitInfo, _locomotion ? _locomotion.StepForwardProbeDistance : 0.5f, -1, QueryTriggerInteraction.Ignore) && Vector3.Angle(hitInfo.normal, -forward) < (_locomotion ? _locomotion.StepMaxFaceAngle : 50f))
				{
					hit = hitInfo;
					return true;
				}
			}
			return false;
		}

		private bool TryFindLedgeTop(RaycastHit bottomHit, out Vector3 ledgePoint)
		{
			ledgePoint = Vector3.zero;
			if (!Physics.Raycast(bottomHit.point + Vector3.up * _locomotion.StepHeight + _transform.forward * 0.05f, Vector3.down, out var hitInfo, _locomotion.StepHeight + 0.1f, -1, QueryTriggerInteraction.Ignore))
			{
				return false;
			}
			if (Vector3.Angle(hitInfo.normal, Vector3.up) > _locomotion.StepLedgeMaxSurfaceAngle)
			{
				return false;
			}
			ledgePoint = hitInfo.point;
			return true;
		}

		private bool IsPathToLandingClear(Vector3 landingPos)
		{
			if (!_capsuleCollider)
			{
				return true;
			}
			Vector3 vector = new Vector3(_transform.position.x, landingPos.y, _transform.position.z) + _capsuleCollider.center;
			Vector3 vector2 = landingPos + _capsuleCollider.center - vector;
			float magnitude = vector2.magnitude;
			if (magnitude < 0.001f)
			{
				return true;
			}
			int layerMask = ~(1 << _transform.gameObject.layer);
			RaycastHit hitInfo;
			return !Physics.SphereCast(vector, _capsuleCollider.radius * 0.9f, vector2 / magnitude, out hitInfo, magnitude, layerMask, QueryTriggerInteraction.Ignore);
		}

		private bool IsLandingClear(Vector3 landingPos)
		{
			if (!_capsuleCollider)
			{
				return true;
			}
			float radius = _capsuleCollider.radius;
			float num = Mathf.Max(_capsuleCollider.height, radius * 2f);
			Vector3 vector = landingPos + _capsuleCollider.center;
			float num2 = num * 0.5f - radius;
			Vector3 up = _transform.up;
			Vector3 start = vector + up * num2;
			Vector3 end = vector - up * num2 + up * 0.05f;
			int layerMask = ~(1 << _transform.gameObject.layer);
			return !Physics.CheckCapsule(start, end, radius, layerMask, QueryTriggerInteraction.Ignore);
		}
	}
}

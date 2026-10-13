using System;
using System.Collections.Generic;
using CPMEngine;
using UnityEngine;

namespace CarAnimationScripts
{
	public class SuspensionVisualAnimator : MonoBehaviour
	{
		private class Corner
		{
			public WheelProvider Wheel;

			public Transform UpperArm;

			public Transform MiddleArm;

			public Transform LowerArm;

			public Transform TrailingArm;

			public Transform Knuckle;

			public Transform UpperBallJoint;

			public Transform MiddleBallJoint;

			public Transform LowerBallJoint;

			public Transform TrailingJoint;

			public Transform SpringUnitBody;

			public Transform Spring;

			public Transform DamperLower;

			public Transform DamperUpper;

			public Transform Antiroll;

			public Transform DriveAxle;

			public Transform AntirollAttach;

			public bool Steerable;

			public bool Initialized;

			public Vector3 HubRestLocal;

			public Vector3 UpperArmPosLocal;

			public Vector3 MiddleArmPosLocal;

			public Vector3 LowerArmPosLocal;

			public Vector3 TrailingArmPosLocal;

			public Quaternion UpperArmRestRot;

			public Quaternion MiddleArmRestRot;

			public Quaternion LowerArmRestRot;

			public Quaternion TrailingArmRestRot;

			public Vector3 UpperAimDirLocal;

			public Vector3 MiddleAimDirLocal;

			public Vector3 LowerAimDirLocal;

			public Vector3 TrailingAimDirLocal;

			public Vector3 KnucklePosLocal;

			public Quaternion KnuckleRotLocal;

			public Vector3 SteerPivotLocal;

			public Vector3 SteerAxisLocal = Vector3.up;

			public bool HasBottom;

			public bool HasTop;

			public Vector3 BottomRestLocal;

			public Vector3 BottomInArmSpace;

			public Vector3 TopRestLocal;

			public Quaternion DamperUpperRestRot;

			public Quaternion DamperLowerRestRot;

			public Quaternion SpringUnitBodyRestRot;

			public Quaternion SpringRestRot;

			public Vector3 SpringRestScale;

			public float SpringRestLen;

			public int SpringAxis;

			public Vector3 AntirollPivotLocal;

			public Quaternion AntirollRestRot;

			public Vector3 AntirollAimDirLocal;

			public Vector3 DriveAxlePivotLocal;

			public Quaternion DriveAxleRestRot;

			public Vector3 DriveAxleAimDirLocal;

			public Vector3 LastKnuckleTravel;
		}

		private class AxleBeam
		{
			public Transform Beam;

			public Corner Left;

			public Corner Right;

			public bool Initialized;

			public Vector3 RestPosLocal;

			public Quaternion RestRotLocal;

			public float TrackWidth;
		}

		private Powertrain _powertrain;

		private Transform _body;

		private BodyMarker _bodyMarker;

		private Corner[] _corners;

		private AxleBeam[] _axleBeams = Array.Empty<AxleBeam>();

		private void Awake()
		{
			TryFindPowertrain();
		}

		private void LateUpdate()
		{
			if ((!(_bodyMarker == null) && _corners != null) || TryBind())
			{
				float steerDegrees = GetSteerDegrees();
				for (int i = 0; i < _corners.Length; i++)
				{
					AnimateCorner(_corners[i], steerDegrees);
				}
				for (int j = 0; j < _axleBeams.Length; j++)
				{
					AnimateAxleBeam(_axleBeams[j]);
				}
			}
		}

		private bool TryFindPowertrain()
		{
			if ((bool)_powertrain)
			{
				return true;
			}
			_powertrain = GetComponentInParent<Powertrain>();
			if (!_powertrain)
			{
				return false;
			}
			_body = _powertrain.transform;
			return true;
		}

		private bool TryBind()
		{
			if (!TryFindPowertrain())
			{
				return false;
			}
			Axles getAxles = _powertrain.GetAxles;
			if (getAxles == null)
			{
				return false;
			}
			WheelProvider wheelProvider = getAxles.FrontAxle?.LeftWheel;
			WheelProvider wheelProvider2 = getAxles.FrontAxle?.RightWheel;
			WheelProvider wheelProvider3 = getAxles.RearAxle?.LeftWheel;
			WheelProvider wheelProvider4 = getAxles.RearAxle?.RightWheel;
			if (!wheelProvider || !wheelProvider2 || !wheelProvider3 || !wheelProvider4)
			{
				return false;
			}
			BodyMarker bodyMarker = GetComponentInChildren<BodyMarker>(includeInactive: true);
			if (!bodyMarker)
			{
				bodyMarker = GetComponentInParent<BodyMarker>();
			}
			if (!bodyMarker)
			{
				return false;
			}
			_bodyMarker = bodyMarker;
			_corners = new Corner[4]
			{
				MakeCorner(wheelProvider, bodyMarker.SuspensionFL, steerable: true),
				MakeCorner(wheelProvider2, bodyMarker.SuspensionFR, steerable: true),
				MakeCorner(wheelProvider3, bodyMarker.SuspensionRL, steerable: false),
				MakeCorner(wheelProvider4, bodyMarker.SuspensionRR, steerable: false)
			};
			List<AxleBeam> list = new List<AxleBeam>(2);
			if ((bool)bodyMarker.FrontSolidAxle)
			{
				list.Add(new AxleBeam
				{
					Beam = bodyMarker.FrontSolidAxle,
					Left = _corners[0],
					Right = _corners[1]
				});
			}
			if ((bool)bodyMarker.RearSolidAxle)
			{
				list.Add(new AxleBeam
				{
					Beam = bodyMarker.RearSolidAxle,
					Left = _corners[2],
					Right = _corners[3]
				});
			}
			_axleBeams = list.ToArray();
			return true;
		}

		private Corner MakeCorner(WheelProvider wheel, SuspensionParts parts, bool steerable)
		{
			if (parts == null)
			{
				return new Corner
				{
					Wheel = wheel,
					Steerable = steerable
				};
			}
			Corner corner = new Corner
			{
				Wheel = wheel,
				Steerable = steerable,
				UpperArm = parts.UpperArm,
				MiddleArm = parts.MiddleArm,
				LowerArm = parts.LowerArm,
				TrailingArm = parts.TrailingArm,
				Knuckle = parts.Knuckle,
				UpperBallJoint = parts.UpperBallJoint,
				MiddleBallJoint = parts.MiddleBallJoint,
				LowerBallJoint = parts.LowerBallJoint,
				TrailingJoint = parts.TrailingJoint,
				SpringUnitBody = parts.SpringUnitBody,
				Spring = parts.Spring,
				DamperLower = parts.DamperLower,
				DamperUpper = parts.DamperUpper,
				Antiroll = parts.Antiroll,
				DriveAxle = parts.DriveAxle,
				AntirollAttach = parts.AntirollAttach
			};
			DropIfReferencePoint(corner, ref corner.UpperArm, "UpperArm");
			DropIfReferencePoint(corner, ref corner.MiddleArm, "MiddleArm");
			DropIfReferencePoint(corner, ref corner.LowerArm, "LowerArm");
			DropIfReferencePoint(corner, ref corner.TrailingArm, "TrailingArm");
			DropIfReferencePoint(corner, ref corner.SpringUnitBody, "SpringUnitBody");
			DropIfReferencePoint(corner, ref corner.Spring, "Spring");
			DropIfReferencePoint(corner, ref corner.DamperLower, "DamperLower");
			DropIfReferencePoint(corner, ref corner.DamperUpper, "DamperUpper");
			DropIfReferencePoint(corner, ref corner.Antiroll, "Antiroll");
			DropIfReferencePoint(corner, ref corner.DriveAxle, "DriveAxle");
			return corner;
		}

		private void DropIfReferencePoint(Corner corner, ref Transform slot, string slotName)
		{
			if ((bool)slot && (!(slot != corner.UpperBallJoint) || !(slot != corner.MiddleBallJoint) || !(slot != corner.LowerBallJoint) || !(slot != corner.TrailingJoint) || !(slot != corner.AntirollAttach)))
			{
				Debug.LogWarning("[SuspensionVisualAnimator] " + base.name + ": поле " + slotName + " указывает на референс-точку " + slot.name + " (опора/крепление стабилизатора) — поле отключено, проверь SuspensionParts.", this);
				slot = null;
			}
		}

		private void AnimateCorner(Corner c, float steerDeg)
		{
			if (c?.Wheel == null || !c.Knuckle)
			{
				return;
			}
			Transform modelTransform = c.Wheel.ModelTransform;
			if ((bool)modelTransform)
			{
				if (!c.Initialized)
				{
					CalibrateCorner(c);
				}
				Vector3 vector = _body.InverseTransformPoint(modelTransform.position) - c.HubRestLocal;
				vector.x = 0f;
				c.LastKnuckleTravel = vector;
				Quaternion quaternion = Quaternion.identity;
				if (c.Steerable && Mathf.Abs(steerDeg) > Mathf.Epsilon)
				{
					quaternion = Quaternion.AngleAxis(steerDeg, c.SteerAxisLocal);
				}
				Vector3 position = c.SteerPivotLocal + vector + quaternion * (c.KnucklePosLocal - c.SteerPivotLocal);
				Quaternion quaternion2 = quaternion * c.KnuckleRotLocal;
				c.Knuckle.SetPositionAndRotation(_body.TransformPoint(position), _body.rotation * quaternion2);
				Vector3 aimWorld = (c.UpperBallJoint ? c.UpperBallJoint.position : c.Knuckle.position);
				Vector3 aimWorld2 = (c.LowerBallJoint ? c.LowerBallJoint.position : c.Knuckle.position);
				AimArmHinged(c.UpperArm, c.UpperArmPosLocal, c.UpperAimDirLocal, c.UpperArmRestRot, aimWorld, Vector3.forward);
				AimArmHinged(c.LowerArm, c.LowerArmPosLocal, c.LowerAimDirLocal, c.LowerArmRestRot, aimWorld2, Vector3.forward);
				if ((bool)c.MiddleArm)
				{
					Vector3 aimWorld3 = (c.MiddleBallJoint ? c.MiddleBallJoint.position : c.Knuckle.position);
					AimArmHinged(c.MiddleArm, c.MiddleArmPosLocal, c.MiddleAimDirLocal, c.MiddleArmRestRot, aimWorld3, Vector3.forward);
				}
				if ((bool)c.TrailingArm)
				{
					Vector3 aimWorld4 = (c.TrailingJoint ? c.TrailingJoint.position : c.Knuckle.position);
					AimArmHinged(c.TrailingArm, c.TrailingArmPosLocal, c.TrailingAimDirLocal, c.TrailingArmRestRot, aimWorld4, Vector3.right);
				}
				Vector3 bottomWorld = GetBottomWorld(c, vector);
				AnimateStrut(c, bottomWorld);
				if ((bool)c.Antiroll)
				{
					Vector3 aimWorld5 = (c.AntirollAttach ? c.AntirollAttach.position : (c.HasBottom ? bottomWorld : c.Knuckle.position));
					AimArm(c.Antiroll, c.AntirollPivotLocal, c.AntirollAimDirLocal, c.AntirollRestRot, aimWorld5);
				}
				if ((bool)c.DriveAxle)
				{
					AimArm(c.DriveAxle, c.DriveAxlePivotLocal, c.DriveAxleAimDirLocal, c.DriveAxleRestRot, c.Knuckle.position);
				}
			}
		}

		private void AnimateAxleBeam(AxleBeam a)
		{
			if ((bool)a.Beam && a.Left != null && a.Right != null && a.Left.Initialized && a.Right.Initialized)
			{
				if (!a.Initialized)
				{
					a.RestPosLocal = _body.InverseTransformPoint(a.Beam.position);
					a.RestRotLocal = Quaternion.Inverse(_body.rotation) * a.Beam.rotation;
					a.TrackWidth = Mathf.Abs(a.Left.HubRestLocal.x - a.Right.HubRestLocal.x);
					a.Initialized = true;
				}
				float y = a.Left.LastKnuckleTravel.y;
				float y2 = a.Right.LastKnuckleTravel.y;
				Vector3 position = a.RestPosLocal + Vector3.up * (0.5f * (y + y2));
				float angle = ((a.TrackWidth > 0.0001f) ? (Mathf.Atan2(y2 - y, a.TrackWidth) * 57.29578f) : 0f);
				a.Beam.SetPositionAndRotation(_body.TransformPoint(position), _body.rotation * (Quaternion.AngleAxis(angle, Vector3.forward) * a.RestRotLocal));
			}
		}

		private Vector3 GetBottomWorld(Corner c, Vector3 knuckleTravel)
		{
			if (!c.HasBottom)
			{
				return default;
			}
			if ((bool)c.LowerArm)
			{
				return c.LowerArm.TransformPoint(c.BottomInArmSpace);
			}
			return _body.TransformPoint(c.BottomRestLocal + knuckleTravel);
		}

		private void AnimateStrut(Corner c, Vector3 bottomWorld)
		{
			if (!c.HasBottom)
			{
				return;
			}
			Vector3 bottomLocal = _body.InverseTransformPoint(bottomWorld);
			Vector3 vector = (c.HasTop ? _body.TransformPoint(c.TopRestLocal) : default(Vector3));
			Vector3 vector2 = c.TopRestLocal - c.BottomRestLocal;
			if ((bool)c.DamperUpper && c.HasTop)
			{
				AimArm(c.DamperUpper, c.TopRestLocal, -vector2, c.DamperUpperRestRot, bottomWorld);
			}
			if ((bool)c.DamperLower)
			{
				PlaceBottomPart(c.DamperLower, c.DamperLowerRestRot, bottomLocal, vector2, vector, c.HasTop);
			}
			if ((bool)c.SpringUnitBody)
			{
				PlaceBottomPart(c.SpringUnitBody, c.SpringUnitBodyRestRot, bottomLocal, vector2, vector, c.HasTop);
			}
			if ((bool)c.Spring)
			{
				PlaceBottomPart(c.Spring, c.SpringRestRot, bottomLocal, vector2, vector, c.HasTop);
				if (c.HasTop && c.SpringRestLen > 0.0001f)
				{
					Vector3 springRestScale = c.SpringRestScale;
					springRestScale[c.SpringAxis] *= Vector3.Distance(vector, bottomWorld) / c.SpringRestLen;
					c.Spring.localScale = springRestScale;
				}
			}
		}

		private void PlaceBottomPart(Transform part, Quaternion restRotLocal, Vector3 bottomLocal, Vector3 restDirLocal, Vector3 topWorld, bool hasTop)
		{
			if (hasTop)
			{
				AimArm(part, bottomLocal, restDirLocal, restRotLocal, topWorld);
			}
			else
			{
				part.SetPositionAndRotation(_body.TransformPoint(bottomLocal), _body.rotation * restRotLocal);
			}
		}

		private void AimArmHinged(Transform arm, Vector3 pivotLocal, Vector3 restDirLocal, Quaternion restRotLocal, Vector3 aimWorld, Vector3 hingeAxisLocal)
		{
			if ((bool)arm)
			{
				Vector3 vector = _body.InverseTransformPoint(aimWorld);
				Vector3 vector2 = Vector3.ProjectOnPlane(restDirLocal, hingeAxisLocal);
				Vector3 to = Vector3.ProjectOnPlane(vector - pivotLocal, hingeAxisLocal);
				if (!(vector2.sqrMagnitude < 1E-08f) && !(to.sqrMagnitude < 1E-08f))
				{
					float angle = Vector3.SignedAngle(vector2, to, hingeAxisLocal);
					arm.SetPositionAndRotation(_body.TransformPoint(pivotLocal), _body.rotation * (Quaternion.AngleAxis(angle, hingeAxisLocal) * restRotLocal));
				}
			}
		}

		private void AimArm(Transform arm, Vector3 pivotLocal, Vector3 restDirLocal, Quaternion restRotLocal, Vector3 aimWorld)
		{
			if ((bool)arm)
			{
				Vector3 toDirection = _body.InverseTransformPoint(aimWorld) - pivotLocal;
				if (!(toDirection.sqrMagnitude < 1E-08f) && !(restDirLocal.sqrMagnitude < 1E-08f))
				{
					Quaternion quaternion = Quaternion.FromToRotation(restDirLocal, toDirection);
					arm.SetPositionAndRotation(_body.TransformPoint(pivotLocal), _body.rotation * (quaternion * restRotLocal));
				}
			}
		}

		private void CalibrateCorner(Corner c)
		{
			c.HubRestLocal = _body.InverseTransformPoint(c.Knuckle.position);
			c.KnucklePosLocal = _body.InverseTransformPoint(c.Knuckle.position);
			c.KnuckleRotLocal = Quaternion.Inverse(_body.rotation) * c.Knuckle.rotation;
			if ((bool)c.UpperBallJoint && (bool)c.LowerBallJoint)
			{
				Vector3 vector = _body.InverseTransformPoint(c.UpperBallJoint.position);
				Vector3 vector2 = _body.InverseTransformPoint(c.LowerBallJoint.position);
				c.SteerPivotLocal = 0.5f * (vector + vector2);
				Vector3 vector3 = vector - vector2;
				c.SteerAxisLocal = ((vector3.sqrMagnitude > 1E-08f) ? vector3.normalized : Vector3.up);
			}
			else
			{
				c.SteerPivotLocal = c.KnucklePosLocal;
				c.SteerAxisLocal = Vector3.up;
			}
			if ((bool)c.UpperArm)
			{
				c.UpperArmPosLocal = _body.InverseTransformPoint(c.UpperArm.position);
				c.UpperArmRestRot = Quaternion.Inverse(_body.rotation) * c.UpperArm.rotation;
				Vector3 position = (c.UpperBallJoint ? c.UpperBallJoint.position : c.Knuckle.position);
				c.UpperAimDirLocal = _body.InverseTransformPoint(position) - c.UpperArmPosLocal;
			}
			if ((bool)c.MiddleArm)
			{
				c.MiddleArmPosLocal = _body.InverseTransformPoint(c.MiddleArm.position);
				c.MiddleArmRestRot = Quaternion.Inverse(_body.rotation) * c.MiddleArm.rotation;
				Vector3 position2 = (c.MiddleBallJoint ? c.MiddleBallJoint.position : c.Knuckle.position);
				c.MiddleAimDirLocal = _body.InverseTransformPoint(position2) - c.MiddleArmPosLocal;
			}
			if ((bool)c.LowerArm)
			{
				c.LowerArmPosLocal = _body.InverseTransformPoint(c.LowerArm.position);
				c.LowerArmRestRot = Quaternion.Inverse(_body.rotation) * c.LowerArm.rotation;
				Vector3 position3 = (c.LowerBallJoint ? c.LowerBallJoint.position : c.Knuckle.position);
				c.LowerAimDirLocal = _body.InverseTransformPoint(position3) - c.LowerArmPosLocal;
			}
			if ((bool)c.TrailingArm)
			{
				c.TrailingArmPosLocal = _body.InverseTransformPoint(c.TrailingArm.position);
				c.TrailingArmRestRot = Quaternion.Inverse(_body.rotation) * c.TrailingArm.rotation;
				Vector3 position4 = (c.TrailingJoint ? c.TrailingJoint.position : c.Knuckle.position);
				c.TrailingAimDirLocal = _body.InverseTransformPoint(position4) - c.TrailingArmPosLocal;
			}
			Transform transform = (c.DamperLower ? c.DamperLower : (c.Spring ? c.Spring : c.SpringUnitBody));
			c.HasBottom = transform;
			if (c.HasBottom)
			{
				Vector3 position5 = transform.position;
				c.BottomRestLocal = _body.InverseTransformPoint(position5);
				if ((bool)c.LowerArm)
				{
					c.BottomInArmSpace = c.LowerArm.InverseTransformPoint(position5);
				}
				c.HasTop = c.DamperUpper;
				if (c.HasTop)
				{
					c.TopRestLocal = _body.InverseTransformPoint(c.DamperUpper.position);
					c.DamperUpperRestRot = Quaternion.Inverse(_body.rotation) * c.DamperUpper.rotation;
				}
				if ((bool)c.DamperLower)
				{
					c.DamperLowerRestRot = Quaternion.Inverse(_body.rotation) * c.DamperLower.rotation;
				}
				if ((bool)c.SpringUnitBody)
				{
					c.SpringUnitBodyRestRot = Quaternion.Inverse(_body.rotation) * c.SpringUnitBody.rotation;
				}
				if ((bool)c.Spring)
				{
					c.SpringRestRot = Quaternion.Inverse(_body.rotation) * c.Spring.rotation;
					c.SpringRestScale = c.Spring.localScale;
					if (c.HasTop)
					{
						Vector3 direction = _body.TransformPoint(c.TopRestLocal) - position5;
						c.SpringRestLen = direction.magnitude;
						c.SpringAxis = DominantAxis(c.Spring.InverseTransformDirection(direction));
					}
				}
			}
			if ((bool)c.Antiroll)
			{
				c.AntirollPivotLocal = _body.InverseTransformPoint(c.Antiroll.position);
				c.AntirollRestRot = Quaternion.Inverse(_body.rotation) * c.Antiroll.rotation;
				Vector3 vector4 = (c.AntirollAttach ? _body.InverseTransformPoint(c.AntirollAttach.position) : (c.HasBottom ? c.BottomRestLocal : c.KnucklePosLocal));
				c.AntirollAimDirLocal = vector4 - c.AntirollPivotLocal;
			}
			if ((bool)c.DriveAxle)
			{
				c.DriveAxlePivotLocal = _body.InverseTransformPoint(c.DriveAxle.position);
				c.DriveAxleRestRot = Quaternion.Inverse(_body.rotation) * c.DriveAxle.rotation;
				c.DriveAxleAimDirLocal = c.KnucklePosLocal - c.DriveAxlePivotLocal;
			}
			c.Initialized = true;
		}

		private static int DominantAxis(Vector3 v)
		{
			float num = Mathf.Abs(v.x);
			float num2 = Mathf.Abs(v.y);
			float num3 = Mathf.Abs(v.z);
			if (num >= num2 && num >= num3)
			{
				return 0;
			}
			if (!(num2 >= num3))
			{
				return 2;
			}
			return 1;
		}

		private float GetSteerDegrees()
		{
			if (!_powertrain)
			{
				return 0f;
			}
			float steer;
			float maxSteeringAngle;
			try
			{
				steer = _powertrain.GetSteer();
				maxSteeringAngle = _powertrain.MaxSteeringAngle;
			}
			catch
			{
				return 0f;
			}
			if (!float.IsFinite(steer) || !float.IsFinite(maxSteeringAngle))
			{
				return 0f;
			}
			return (0f - steer) * maxSteeringAngle * 57.29578f;
		}

		[ContextMenu("Rebind")]
		public void Rebind()
		{
			_bodyMarker = null;
			_corners = null;
			_axleBeams = Array.Empty<AxleBeam>();
		}
	}
}

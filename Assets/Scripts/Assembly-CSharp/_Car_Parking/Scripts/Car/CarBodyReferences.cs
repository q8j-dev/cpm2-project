using System;
using UnityEngine;

namespace _Car_Parking.Scripts.Car
{
	public class CarBodyReferences : MonoBehaviour, IPhysicalWiperSource
	{
		public enum WiperAnimationMode
		{
			MirroredStrokes = 0,
			WipeAndReturn = 1
		}

		[Serializable]
		public struct TransformSnapshot
		{
			public Vector3 LocalPosition;

			public Quaternion LocalRotation;

			public Vector3 LocalScale;

			public static TransformSnapshot FromTransform(Transform source)
			{
				if (!source)
				{
					return default;
				}
				return new TransformSnapshot
				{
					LocalPosition = source.localPosition,
					LocalRotation = source.localRotation,
					LocalScale = source.localScale
				};
			}

			public static TransformSnapshot Lerp(TransformSnapshot min, TransformSnapshot max, float value)
			{
				value = Mathf.Clamp01(value);
				return new TransformSnapshot
				{
					LocalPosition = Vector3.LerpUnclamped(min.LocalPosition, max.LocalPosition, value),
					LocalRotation = Quaternion.SlerpUnclamped(min.LocalRotation, max.LocalRotation, value),
					LocalScale = Vector3.LerpUnclamped(min.LocalScale, max.LocalScale, value)
				};
			}

			public void ApplyTo(Transform target)
			{
				if ((bool)target)
				{
					target.localPosition = LocalPosition;
					target.localRotation = ((Quaternion.Dot(LocalRotation, LocalRotation) > 0f) ? LocalRotation : Quaternion.identity);
					target.localScale = LocalScale;
				}
			}
		}

		[Serializable]
		public class WiperBlade
		{
			public Transform Pivot;

			public TransformSnapshot Min;

			public TransformSnapshot Max;
		}

		[SerializeField]
		[HideInInspector]
		private WiperAnimationMode _wiperMode;

		[HideInInspector]
		[SerializeField]
		private Vector2 _wiperShaderRange = new Vector2(0f, 1f);

		[field: SerializeField]
		public CarInteriorReferences OldInterior { get; private set; }

		[field: SerializeField]
		public Transform GlassWindow { get; private set; }

		[field: SerializeField]
		public Transform Diods { get; private set; }

		[field: SerializeField]
		public Transform TurnLeftDiod { get; private set; }

		[field: SerializeField]
		public Transform TurnRightDiod { get; private set; }

		[field: SerializeField]
		public Transform RearBrake { get; set; }

		[field: SerializeField]
		public Transform RearLight { get; private set; }

		[field: SerializeField]
		public Transform ReverseLight { get; private set; }

		[field: SerializeField]
		public Transform TurnLeft { get; private set; }

		[field: SerializeField]
		public Transform TurnRight { get; private set; }

		[field: SerializeField]
		public Transform TurnLeft2 { get; private set; }

		[field: SerializeField]
		public Transform TurnRight2 { get; private set; }

		[field: SerializeField]
		public Transform TurnLeft3 { get; private set; }

		[field: SerializeField]
		public Transform TurnRight3 { get; private set; }

		[field: SerializeField]
		public Transform TurnLeft4 { get; private set; }

		[field: SerializeField]
		public Transform TurnRight4 { get; private set; }

		[field: SerializeField]
		public Transform Lights2 { get; private set; }

		[field: SerializeField]
		public Transform FrontLight { get; private set; }

		[field: SerializeField]
		public Transform Mmirror { get; private set; }

		[field: SerializeField]
		public Transform Lmirror { get; private set; }

		[field: SerializeField]
		public Transform Rmirror { get; private set; }

		[field: SerializeField]
		public MeshRenderer[] MeshRenderers { get; private set; }

		[field: SerializeField]
		public Transform FrontLeftDoor { get; private set; }

		[field: SerializeField]
		public Transform FrontRightDoor { get; private set; }

		[field: SerializeField]
		public Transform BackLeftDoor { get; private set; }

		[field: SerializeField]
		public Transform BackRightDoor { get; private set; }

		[field: SerializeField]
		public Transform Trunk { get; set; }

		[field: SerializeField]
		public Transform FirstTrunkDoor { get; private set; }

		[field: SerializeField]
		public Transform SecondTrunkDoor { get; private set; }

		[field: SerializeField]
		public Transform Hood { get; set; }

		[field: SerializeField]
		public Transform Roof { get; private set; }

		[field: SerializeField]
		public Transform RoofPlatformHondaDelSol { get; private set; }

		[field: SerializeField]
		public Transform CarFrontLeftDoorWindow { get; private set; }

		[field: SerializeField]
		public Transform CarFrontRightDoorWindow { get; private set; }

		[field: SerializeField]
		public GameObject Body { get; private set; }

		public WiperAnimationMode WiperMode
		{
			get
			{
				return _wiperMode;
			}
			set
			{
				_wiperMode = value;
			}
		}

		public Vector2 WiperShaderRange
		{
			get
			{
				return _wiperShaderRange;
			}
			set
			{
				_wiperShaderRange = value;
			}
		}

		[field: SerializeField]
		public WiperBlade[] WiperBlades { get; set; }

		public bool HasPhysicalWipers => HasBlades(WiperBlades);

		public static CarBodyReferences FindInHierarchy(GameObject root)
		{
			if (!root)
			{
				return null;
			}
			return root.GetComponentInChildren<CarBodyReferences>(includeInactive: true);
		}

		public void ApplyWiperPose(float value)
		{
			ApplyPose(WiperBlades, value);
		}

		public static bool HasBlades(WiperBlade[] blades)
		{
			if (blades == null)
			{
				return false;
			}
			foreach (WiperBlade wiperBlade in blades)
			{
				if (wiperBlade != null && (bool)wiperBlade.Pivot && !(wiperBlade.Min.LocalScale == Vector3.zero) && !(wiperBlade.Max.LocalScale == Vector3.zero))
				{
					return true;
				}
			}
			return false;
		}

		public static void ApplyPose(WiperBlade[] blades, float value)
		{
			if (blades == null)
			{
				return;
			}
			value = Mathf.Clamp01(value);
			foreach (WiperBlade wiperBlade in blades)
			{
				if (wiperBlade != null && (bool)wiperBlade.Pivot && !(wiperBlade.Min.LocalScale == Vector3.zero) && !(wiperBlade.Max.LocalScale == Vector3.zero))
				{
					TransformSnapshot.Lerp(wiperBlade.Min, wiperBlade.Max, value).ApplyTo(wiperBlade.Pivot);
				}
			}
		}
	}
}

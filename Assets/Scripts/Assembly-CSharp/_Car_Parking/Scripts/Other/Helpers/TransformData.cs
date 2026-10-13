using System;
using UnityEngine;

namespace _Car_Parking.Scripts.Other.Helpers
{
	[Serializable]
	public struct TransformData
	{
		public Vector3 Position;

		public Quaternion Rotation;

		public Vector3 Scale;

		public TransformData(Vector3 position, Quaternion rotation, Vector3 scale)
		{
			Position = position;
			Rotation = rotation;
			Scale = scale;
		}

		public TransformData(Transform transform)
		{
			if ((bool)transform)
			{
				Position = transform.localPosition;
				Rotation = transform.localRotation;
				Scale = transform.localScale;
			}
			else
			{
				Position = Vector3.zero;
				Rotation = Quaternion.identity;
				Scale = Vector3.one;
			}
		}
	}
}

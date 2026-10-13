using System;
using UnityEngine;

namespace _Car_Parking.Scripts.Racing_Scripts.RallyProps
{
	[Serializable]
	public struct RallyPropData
	{
		public Vector3 Position;

		public Quaternion Rotation;

		public Vector3 Scale;

		public int PrefabIndex;
	}
}

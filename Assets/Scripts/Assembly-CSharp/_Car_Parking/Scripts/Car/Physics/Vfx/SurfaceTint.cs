using System;
using UnityEngine;

namespace _Car_Parking.Scripts.Car.Physics.Vfx
{
	[Serializable]
	public struct SurfaceTint
	{
		public Surface Surface;

		public Color Color;

		[Tooltip("Стартовый размер частиц для этой поверхности. 0 или меньше = размер из префаба (main.startSize).")]
		public float Size;
	}
}

using System;
using UnityEngine;

namespace _Car_Parking.Scripts
{
	[Serializable]
	public struct SpawnLayoutSettings
	{
		public SpawnLayoutShape Shape;

		[Tooltip("Items per row before wrapping to the next row.")]
		[Header("Grid")]
		public int ColumnsPerRow;

		[Tooltip("Distance between columns (x) and between rows (y).")]
		public Vector2 GridSpacing;

		[Header("Circle")]
		[Tooltip("Circle radius in meters.")]
		public float Radius;

		[Tooltip("If true poses face away from the center, otherwise toward it.")]
		public bool FaceOutward;
	}
}

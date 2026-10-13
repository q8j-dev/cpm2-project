using System;
using UnityEngine;

namespace CPM.Obstacles
{
	[Serializable]
	public struct HexGridSettings
	{
		public HexShape Shape;

		public HexOrientation Orientation;

		[Min(0f)]
		[Tooltip("Hexagon only: rings around the center tile. 0 is a single tile.")]
		public int Radius;

		[Min(1f)]
		[Tooltip("Rectangle only.")]
		public int Columns;

		[Min(1f)]
		[Tooltip("Rectangle only.")]
		public int Rows;

		[Tooltip("Distance from a tile's center to its corner, in world units.")]
		[Min(0.01f)]
		public float TileSize;

		[Tooltip("1 = tiles touch, >1 opens a gap, <1 overlaps.")]
		[Min(0.01f)]
		public float Spacing;

		public static HexGridSettings Default => new HexGridSettings
		{
			Shape = HexShape.Hexagon,
			Orientation = HexOrientation.FlatTop,
			Radius = 3,
			Columns = 8,
			Rows = 8,
			TileSize = 1f,
			Spacing = 1f
		};
	}
}

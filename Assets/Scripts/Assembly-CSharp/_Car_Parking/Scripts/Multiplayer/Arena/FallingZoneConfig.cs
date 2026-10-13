using System;
using UnityEngine;

namespace _Car_Parking.Scripts.Multiplayer.Arena
{
	[Serializable]
	public class FallingZoneConfig
	{
		[Serializable]
		public class TileDestroyData
		{
			public float Delay = 0.5f;

			public float Duration = 1f;

			public Color Color = Color.red;
		}

		public int Radius = 5;

		public int Count = 3;

		public float OffsetSpawn = 8f;

		public TileDestroyData TileDestroy;
	}
}

using System.Collections.Generic;
using UnityEngine;

namespace _Car_Parking.Scripts.Multiplayer.Arena
{
	public static class ArenaRespawnPoints
	{
		public static bool TrySelectNearest(IReadOnlyList<PositionsAndRotation> points, Vector3 from, out PositionsAndRotation result)
		{
			result = null;
			if (points == null)
			{
				return false;
			}
			float num = float.MaxValue;
			for (int i = 0; i < points.Count; i++)
			{
				PositionsAndRotation positionsAndRotation = points[i];
				if (positionsAndRotation != null)
				{
					float sqrMagnitude = (positionsAndRotation.Position - from).sqrMagnitude;
					if (!(sqrMagnitude >= num))
					{
						num = sqrMagnitude;
						result = positionsAndRotation;
					}
				}
			}
			return result != null;
		}
	}
}

using System.Collections.Generic;
using UnityEngine;

namespace _Car_Parking.Scripts.CustomDataTypes
{
	public static class SpawnPointRandomizer
	{
		private const bool OnlyFirstPoint = false;

		public static Transform GetRandomSpawnPoint(this Transform[] points, out bool forceUsePoint, bool useFirstPointOverride = false)
		{
			int num = 0;
			forceUsePoint = false;
			if (useFirstPointOverride)
			{
				num = 0;
				return points[0];
			}
			num = Random.Range(0, points.Length);
			return points[num];
		}

		public static Transform GetRandomSpawnPoint(this Transform[] points, bool useFirstPointOverride = false)
		{
			bool forceUsePoint;
			return points.GetRandomSpawnPoint(out forceUsePoint, useFirstPointOverride);
		}

		public static Vector3 CheckPoints(Vector3 pos, Transform[] SpawnPoints)
		{
			if (CheckPointBox(pos))
			{
				int num = Random.Range(0, SpawnPoints.Length - 1);
				pos = SpawnPoints[num].position;
				if (CheckPointBox(pos))
				{
					num = Random.Range(0, SpawnPoints.Length - 1);
					pos = SpawnPoints[num].position;
				}
			}
			return pos;
		}

		public static Vector3 CheckPoints(Vector3 pos, IReadOnlyList<Vector3> spawnPoints)
		{
			if (spawnPoints == null || spawnPoints.Count == 0)
			{
				return pos;
			}
			if (CheckPointBox(pos))
			{
				pos = spawnPoints[Random.Range(0, spawnPoints.Count)];
				if (CheckPointBox(pos))
				{
					pos = spawnPoints[Random.Range(0, spawnPoints.Count)];
				}
			}
			return pos;
		}

		private static bool CheckPointBox(Vector3 pos)
		{
			return Physics.CheckBox(pos, new Vector3(3f, 0.6f, 3f), Quaternion.identity, -1, QueryTriggerInteraction.Ignore);
		}
	}
}

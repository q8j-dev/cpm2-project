using System.Collections.Generic;
using CPM.RouteStandings;
using UnityEngine;
using _Car_Parking.Scripts.Infrastructure;

namespace _Car_Parking.Scripts.Multiplayer.Arena
{
	public static class ObstacleRaceTrackBuilder
	{
		public static ObstacleRaceTrack Build(ObstacleRaceZonePool pool, int[] indices, Transform origin, Transform finishRoot, IPrefabFactory prefabFactory, int seed)
		{
			if (!origin)
			{
				return null;
			}
			if (!finishRoot)
			{
				return null;
			}
			if (!pool || pool.Count == 0)
			{
				return null;
			}
			if (indices == null || indices.Length == 0)
			{
				return null;
			}
			GameObject gameObject = new GameObject("ObstacleRaceTrack");
			gameObject.transform.SetParent(origin.parent);
			gameObject.transform.SetPositionAndRotation(origin.position, origin.rotation);
			List<Collider> list = new List<Collider>();
			List<string> list2 = new List<string>(indices.Length);
			List<Transform> list3 = new List<Transform> { origin };
			Vector3 position = origin.position;
			Quaternion rotation = origin.rotation;
			foreach (int index in indices)
			{
				ObstacleRaceZone obstacleRaceZone = pool.Get(index);
				if (!obstacleRaceZone || !obstacleRaceZone.Exit)
				{
					continue;
				}
				ObstacleRaceZone obstacleRaceZone2 = Spawn(obstacleRaceZone, position, rotation, gameObject.transform, prefabFactory);
				if ((bool)obstacleRaceZone2.Checkpoint)
				{
					list.Add(obstacleRaceZone2.Checkpoint);
					list2.Add(obstacleRaceZone.AnalyticsId);
				}
				if (obstacleRaceZone2.Waypoints != null)
				{
					Transform[] waypoints = obstacleRaceZone2.Waypoints;
					foreach (Transform transform in waypoints)
					{
						if ((bool)transform)
						{
							list3.Add(transform);
						}
					}
				}
				position = obstacleRaceZone2.Exit.position;
				rotation = obstacleRaceZone2.Exit.rotation;
			}
			if (gameObject.transform.childCount == 0)
			{
				Object.Destroy(gameObject);
				return null;
			}
			finishRoot.SetPositionAndRotation(position, rotation);
			RoutePath standingsRoute = BuildStandingsRoute(gameObject.transform, list3, position);
			return new ObstacleRaceTrack(gameObject, list, standingsRoute, seed, list2.ToArray());
		}

		private static ObstacleRaceZone Spawn(ObstacleRaceZone prefab, Vector3 position, Quaternion rotation, Transform parent, IPrefabFactory prefabFactory)
		{
			if (prefabFactory != null)
			{
				return prefabFactory.Instantiate(prefab, position, rotation, parent);
			}
			return Object.Instantiate(prefab, position, rotation, parent);
		}

		private static RoutePath BuildStandingsRoute(Transform root, List<Transform> waypoints, Vector3 finishPosition)
		{
			GameObject gameObject = new GameObject("StandingsRoute");
			gameObject.transform.SetParent(root, worldPositionStays: false);
			GameObject gameObject2 = new GameObject("FinishPoint");
			gameObject2.transform.SetParent(gameObject.transform);
			gameObject2.transform.position = finishPosition;
			waypoints.Add(gameObject2.transform);
			RoutePath routePath = gameObject.AddComponent<RoutePath>();
			routePath.Waypoints = waypoints.ToArray();
			return routePath;
		}
	}
}

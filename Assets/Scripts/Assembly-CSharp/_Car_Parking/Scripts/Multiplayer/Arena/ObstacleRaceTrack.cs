using System;
using System.Collections.Generic;
using CPM.RouteStandings;
using UnityEngine;

namespace _Car_Parking.Scripts.Multiplayer.Arena
{
	public sealed class ObstacleRaceTrack : IDisposable
	{
		private GameObject _root;

		public IReadOnlyList<Collider> Checkpoints { get; }

		public RoutePath StandingsRoute { get; }

		public int Seed { get; }

		public string[] Plan { get; }

		public ObstacleRaceTrack(GameObject root, IReadOnlyList<Collider> checkpoints, RoutePath standingsRoute, int seed, string[] plan)
		{
			_root = root;
			Checkpoints = checkpoints;
			StandingsRoute = standingsRoute;
			Seed = seed;
			Plan = plan;
		}

		public void SetActive(bool active)
		{
			if ((bool)_root)
			{
				_root.SetActive(active);
			}
		}

		public void Dispose()
		{
			if ((bool)_root)
			{
				UnityEngine.Object.Destroy(_root);
			}
			_root = null;
		}
	}
}

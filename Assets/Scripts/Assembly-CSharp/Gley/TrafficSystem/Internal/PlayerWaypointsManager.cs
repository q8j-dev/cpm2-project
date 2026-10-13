using System.Collections.Generic;
using Gley.UrbanSystem.Internal;

namespace Gley.TrafficSystem.Internal
{
	public class PlayerWaypointsManager : IDestroyable
	{
		private readonly Dictionary<int, List<int>> _playerTarget;

		private readonly HashSet<int> _targetWaypoints;

		public PlayerWaypointsManager()
		{
			Assign();
			_playerTarget = new Dictionary<int, List<int>>();
			_targetWaypoints = new HashSet<int>();
		}

		public void Assign()
		{
			DestroyableManager.Instance.Register(this);
		}

		public void RegisterPlayer(int id, int waypointIndex)
		{
			if (!_playerTarget.ContainsKey(id))
			{
				_playerTarget[id] = new List<int> { waypointIndex };
				_targetWaypoints.Add(waypointIndex);
			}
		}

		public void UnregisterPlayer(int id)
		{
			if (!_playerTarget.TryGetValue(id, out var value))
			{
				return;
			}
			foreach (int item in value)
			{
				_targetWaypoints.Remove(item);
			}
			_playerTarget.Remove(id);
		}

		public void UpdatePlayerWaypoint(int id, int newWaypointIndex)
		{
			UpdatePlayerWaypoint(id, new List<int> { newWaypointIndex });
		}

		public void UpdatePlayerWaypoint(int id, List<int> newWaypointIndex)
		{
			if (_playerTarget.TryGetValue(id, out var value))
			{
				foreach (int item in value)
				{
					_targetWaypoints.Remove(item);
				}
			}
			_playerTarget[id] = new List<int>(newWaypointIndex);
			foreach (int item2 in newWaypointIndex)
			{
				_targetWaypoints.Add(item2);
			}
		}

		public bool IsThisWaypointIndexATarget(int waypointIndex)
		{
			return _targetWaypoints.Contains(waypointIndex);
		}

		public List<int> GetPlayerTarget(int id)
		{
			if (_playerTarget.TryGetValue(id, out var value))
			{
				return value;
			}
			return new List<int>();
		}

		public void OnDestroy()
		{
			_playerTarget.Clear();
			_targetWaypoints.Clear();
		}
	}
}

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Gley.TrafficSystem.Internal;
using Gley.UrbanSystem.Internal;
using UnityEngine;

namespace Gley.TrafficSystem
{
	public class PlayerComponent : MonoBehaviour, ITrafficParticipant
	{
		private Rigidbody _rb;

		private Transform _myTransform;

		private bool _initialized;

		private List<TrafficWaypoint> _allWaypoints;

		private List<Vector2Int> _cellNeighbors;

		private GridData _gridData;

		private CellData _currentCell;

		private PlayerWaypointsManager _playerWaypointsManager;

		private TrafficWaypointsData _trafficWaypointsData;

		private TrafficWaypoint _proposedTarget;

		private TrafficWaypoint _currentTarget;

		private Vector3 _playerPosition;

		private bool _targetChanged;

		private void OnEnable()
		{
			StartCoroutine(Initialize());
		}

		private void OnDisable()
		{
			if (_initialized && _playerWaypointsManager != null)
			{
				_playerWaypointsManager.UnregisterPlayer(GetInstanceID());
			}
		}

		private IEnumerator Initialize()
		{
			while (!TrafficManager.Exists || !TrafficManager.Instance.Initialized)
			{
				yield return null;
			}
			_rb = GetComponent<Rigidbody>();
			_myTransform = base.transform;
			_gridData = TrafficManager.Instance.GridData;
			_trafficWaypointsData = TrafficManager.Instance.TrafficWaypointsData;
			_playerWaypointsManager = TrafficManager.Instance.PlayerWaypointsManager;
			_playerWaypointsManager.RegisterPlayer(GetInstanceID(), -1);
			_allWaypoints = new List<TrafficWaypoint>();
			_initialized = true;
		}

		private void Update()
		{
			if (!_initialized)
			{
				return;
			}
			_playerPosition = _myTransform.position;
			CellData cell = _gridData.GetCell(_playerPosition);
			if (cell != _currentCell)
			{
				_currentCell = cell;
				_cellNeighbors = _gridData.GetCellNeighbors(cell.CellProperties.Row, cell.CellProperties.Column, 1, justEdgeCells: false);
				_allWaypoints.Clear();
				foreach (Vector2Int cellNeighbor in _cellNeighbors)
				{
					_allWaypoints.AddRange(from index in _gridData.GetAllTrafficWaypointsInCell(cellNeighbor)
						select _trafficWaypointsData.AllTrafficWaypoints[index]);
				}
			}
			float num = float.PositiveInfinity;
			foreach (TrafficWaypoint allWaypoint in _allWaypoints)
			{
				float num2 = Vector3.SqrMagnitude(_playerPosition - allWaypoint.Position);
				if (num2 < num && CheckOrientation(allWaypoint, out var proposedTarget))
				{
					num = num2;
					_proposedTarget = proposedTarget;
				}
			}
			if (_currentTarget == _proposedTarget)
			{
				return;
			}
			_targetChanged = false;
			if (_currentTarget != null)
			{
				if (_currentTarget.Neighbors.Contains(_proposedTarget.ListIndex))
				{
					_targetChanged = true;
				}
				else
				{
					Vector3 forward = _myTransform.forward;
					float f = Vector3.SignedAngle(forward, _proposedTarget.Position - _playerPosition, Vector3.up);
					float f2 = Vector3.SignedAngle(forward, _currentTarget.Position - _playerPosition, Vector3.up);
					if (Mathf.Abs(f) < Mathf.Abs(f2))
					{
						_targetChanged = true;
					}
					else
					{
						float num3 = Vector3.SqrMagnitude(_playerPosition - _proposedTarget.Position);
						float num4 = Vector3.SqrMagnitude(_playerPosition - _currentTarget.Position);
						if (num3 < num4)
						{
							_targetChanged = true;
						}
					}
				}
			}
			else
			{
				_targetChanged = true;
			}
			if (_targetChanged)
			{
				_currentTarget = _proposedTarget;
				_playerWaypointsManager.UpdatePlayerWaypoint(GetInstanceID(), _proposedTarget.ListIndex);
			}
		}

		private bool CheckOrientation(TrafficWaypoint waypoint, out TrafficWaypoint proposedTarget)
		{
			proposedTarget = null;
			if (waypoint.Neighbors.Length < 1)
			{
				return false;
			}
			TrafficWaypoint trafficWaypoint = _trafficWaypointsData.AllTrafficWaypoints[waypoint.Neighbors[0]];
			if (Mathf.Abs(Vector3.SignedAngle(_myTransform.forward, trafficWaypoint.Position - waypoint.Position, Vector3.up)) < 90f)
			{
				proposedTarget = trafficWaypoint;
				return true;
			}
			return false;
		}

		public float GetCurrentSpeedMS()
		{
			if (!_initialized)
			{
				return 0f;
			}
			return _rb.velocity.magnitude;
		}

		public Vector3 GetHeading()
		{
			return _myTransform.forward;
		}

		public bool AlreadyCollidingWith(Collider[] allColliders)
		{
			return false;
		}
	}
}

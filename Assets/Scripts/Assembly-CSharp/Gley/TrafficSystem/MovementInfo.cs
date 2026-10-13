using System.Collections.Generic;
using Gley.TrafficSystem.Internal;
using Unity.Mathematics;
using UnityEngine;

namespace Gley.TrafficSystem
{
	public class MovementInfo
	{
		public delegate void KnownListUpdated(int vehicleIndex);

		public delegate void StopWaypointsUpdated(int vehicleIndex);

		public delegate void SlowDownWaypointsUpdated(int vehicleIndex);

		public delegate void GiveWayWaypointsUpdated(int vehicleIndex);

		private const float WaypointSpeedAdded = 15f;

		private Queue<int> _customPath;

		private List<int> _waypointIndexes;

		private List<Vector3> _positions;

		private List<float> _maxSpeed;

		private List<int> _angle;

		private List<bool> _stop;

		private List<GiveWayType> _giveWay;

		private List<bool> _slowDown;

		private List<Obstacle> _obstacles;

		private Queue<int> _coveredWaypoints;

		private int _activePosition;

		private int _associatedVehicle;

		private float _maxVehicleSpeed;

		private Vector3 _closestObstaclePoint;

		private Obstacle _closestObstacle;

		private float _followSpeed;

		private float _associatedVehicleWidth;

		private float _offset;

		private float _originalOffset;

		private float _correctionPercent;

		private float _speedReductionPercent = 0.1f;

		private float _speedIncreasePercent = 0.1f;

		private float _speedVariationPercent;

		private bool _hasPath;

		private bool _ignoreRuls;

		private bool _dontRemove;

		public Vector3 OldPosition { get; private set; }

		public int OldWaypointIndex { get; private set; }

		public bool NoWaypoints { get; set; }

		public bool HasPath => _hasPath;

		public bool DontRemove => _dontRemove;

		public Queue<int> CustomPath => _customPath;

		public List<int> WaypointIndexes => _waypointIndexes;

		public Queue<int> CoveredWaypoints => _coveredWaypoints;

		public int PathLength => _positions.Count;

		public int RemainingPathLength => _positions.Count - _activePosition;

		public float3 ClosestObstaclePoint => _closestObstaclePoint;

		public Obstacle ClosestObstacle => _closestObstacle;

		public float FollowSpeed => _followSpeed;

		public static event KnownListUpdated OnKnownListUpdated;

		public static event StopWaypointsUpdated OnStopWaypointsUpdated;

		public static event SlowDownWaypointsUpdated OnSlowDownWaypointsUpdated;

		public static event GiveWayWaypointsUpdated OnGiveWayWaypointsUpdated;

		public static void TriggerKnownListUpdatedEvent(int vehicleIndex)
		{
			OnKnownListUpdated?.Invoke(vehicleIndex);
		}

		public static void TriggerStopWaypointsUpdatedEvent(int vehicleIndex)
		{
			OnStopWaypointsUpdated?.Invoke(vehicleIndex);
		}

		public static void TriggerSlowDownWaypointsUpdatedEvent(int vehicleIndex)
		{
			OnSlowDownWaypointsUpdated?.Invoke(vehicleIndex);
		}

		public static void TriggerGiveWayWaypointsUpdatedEvent(int vehicleIndex)
		{
			OnGiveWayWaypointsUpdated?.Invoke(vehicleIndex);
		}

		public MovementInfo(int associatedVehicle, float associatedVehicleWidth, float originalOffset)
		{
			_waypointIndexes = new List<int>();
			_coveredWaypoints = new Queue<int>();
			_positions = new List<Vector3>();
			_maxSpeed = new List<float>();
			_angle = new List<int>();
			_slowDown = new List<bool>();
			_stop = new List<bool>();
			_giveWay = new List<GiveWayType>();
			_obstacles = new List<Obstacle>();
			_activePosition = 0;
			_associatedVehicle = associatedVehicle;
			_closestObstaclePoint = TrafficSystemConstants.DEFAULT_POSITION;
			_followSpeed = TrafficSystemConstants.DEFAULT_SPEED;
			_associatedVehicleWidth = associatedVehicleWidth / 2f;
			_originalOffset = originalOffset;
			SetOffset(originalOffset);
			SetMaxSpeedCorrectionPercent(1f);
			SetSpeedVariationPercent(_speedReductionPercent, _speedIncreasePercent);
		}

		public void SetSpeedVariationPercent(float speedReductionPercent, float speedIncreasePercent)
		{
			_speedVariationPercent = UnityEngine.Random.Range(1f - speedReductionPercent, 1f + speedIncreasePercent);
		}

		public int GetWaypointIndex(int pathPosition)
		{
			if (IsPathPositionValid(pathPosition))
			{
				return _waypointIndexes[pathPosition];
			}
			return -1;
		}

		public void AddSlowDownPoint(int pathPosition)
		{
			if (IsPathPositionValid(pathPosition) && !_slowDown[pathPosition])
			{
				_slowDown[pathPosition] = true;
				TriggerSlowDownWaypointsUpdatedEvent(_associatedVehicle);
			}
		}

		public int GetCurrentWaypointIndex()
		{
			return GetWaypointIndex(0);
		}

		public bool IsVehicleOnThisWaypoint(int waypointIndex)
		{
			if (_coveredWaypoints.Contains(waypointIndex))
			{
				return true;
			}
			return false;
		}

		public int GetAngle(int pathPosition)
		{
			if (IsPathPositionValid(pathPosition))
			{
				return _angle[pathPosition];
			}
			return 0;
		}

		public int GetAngleLength()
		{
			return _angle.Count;
		}

		public Vector3 GetFirstPosition()
		{
			return GetPosition(_activePosition);
		}

		public void SetIgnoreRules(bool ignoreRules)
		{
			_ignoreRuls = ignoreRules;
		}

		public void DontRemoveVehicle(bool dontRemove)
		{
			_dontRemove = dontRemove;
		}

		public void SetOffset(float offsetPercent)
		{
			_offset = Mathf.Clamp(offsetPercent, -1f, 1f);
		}

		public void ResetOffset()
		{
			_offset = _originalOffset;
		}

		public void SetMaxSpeedCorrectionPercent(float correctionPercent)
		{
			_correctionPercent = correctionPercent;
		}

		private Vector3 GetPositionOffset(int waypointIndex)
		{
			TrafficWaypoint waypointFromIndex = API.GetWaypointFromIndex(waypointIndex);
			return _offset * (waypointFromIndex.LaneWidth / 2f - _associatedVehicleWidth) * waypointFromIndex.LeftDirection;
		}

		public void RefreshPositionsFromWaypointData(TrafficWaypointsData waypointsData)
		{
			for (int i = 0; i < _waypointIndexes.Count; i++)
			{
				int num = _waypointIndexes[i];
				if (num >= 0 && num < waypointsData.AllTrafficWaypoints.Length)
				{
					_positions[i] = waypointsData.AllTrafficWaypoints[num].Position;
				}
			}
		}

		public Vector3 GetPosition(int pathPosition)
		{
			if (IsPathPositionValid(pathPosition))
			{
				if (_offset == 0f)
				{
					return _positions[pathPosition];
				}
				return _positions[pathPosition] + GetPositionOffset(GetWaypointIndex(pathPosition));
			}
			return TrafficSystemConstants.DEFAULT_POSITION;
		}

		public float GetFirstWaypointSpeed()
		{
			return GetWaypointSpeed(_activePosition);
		}

		public float GetFollowSpeed()
		{
			return _followSpeed;
		}

		public float GetWaypointSpeed(int pathPosition)
		{
			if (IsPathPositionValid(pathPosition))
			{
				return _maxSpeed[pathPosition] * _correctionPercent;
			}
			return TrafficSystemConstants.DEFAULT_SPEED;
		}

		public bool IsStopWaypoint(int pathPosition)
		{
			if (IsPathPositionValid(pathPosition))
			{
				return _stop[pathPosition];
			}
			return false;
		}

		public bool IsGiveWayWaypoint(int pathPosition)
		{
			if (IsPathPositionValid(pathPosition))
			{
				return _giveWay[pathPosition] != GiveWayType.None;
			}
			return false;
		}

		public GiveWayType GetGiveWayType(int pathPosition)
		{
			return _giveWay[pathPosition];
		}

		public bool HasStopWaypoints()
		{
			for (int i = 0; i < _stop.Count; i++)
			{
				if (_stop[i])
				{
					return true;
				}
			}
			return false;
		}

		public bool HasGiveWayWaypoints()
		{
			for (int i = 0; i < _giveWay.Count; i++)
			{
				if (_giveWay[i] != GiveWayType.None)
				{
					return true;
				}
			}
			return false;
		}

		public bool HasObstacles()
		{
			return _obstacles.Count != 0;
		}

		public bool HasSlowDownWaypoints()
		{
			for (int i = 0; i < _slowDown.Count; i++)
			{
				if (_slowDown[i])
				{
					return true;
				}
			}
			return false;
		}

		public void SetObstacles(List<Obstacle> obstacles, Vector3 targetPosition)
		{
			_obstacles = obstacles;
			SetClosestObstacleAndSpeed(targetPosition);
			Events.TriggerObstaclesUpdatedEvent(_associatedVehicle);
		}

		public void RemoveObstacle(ObstacleTypes typeToRemove)
		{
			for (int i = 0; i < _obstacles.Count; i++)
			{
				if (_obstacles[i].ObstacleType == typeToRemove)
				{
					return;
				}
			}
			Events.TriggerObstacleRemovedEvent(_associatedVehicle, typeToRemove);
		}

		public void SetStopState(int pathPosition, bool stopState)
		{
			if (IsPathPositionValid(pathPosition) && _stop[pathPosition] != stopState)
			{
				_stop[pathPosition] = stopState;
				TriggerStopWaypointsUpdatedEvent(_associatedVehicle);
			}
		}

		public void SetGiveWayState(int pathPosition, GiveWayType giveWayType)
		{
			if (IsPathPositionValid(pathPosition) && _giveWay[pathPosition] != giveWayType)
			{
				_giveWay[pathPosition] = giveWayType;
				TriggerGiveWayWaypointsUpdatedEvent(_associatedVehicle);
			}
		}

		public bool TrySetStopWaypoint(int waypointIndex, bool stopState)
		{
			for (int i = 0; i < _waypointIndexes.Count; i++)
			{
				if (waypointIndex == _waypointIndexes[i])
				{
					SetStopState(i, stopState);
					return true;
				}
			}
			return false;
		}

		public bool TrySetGiveWayWaypoint(int waypointIndex, GiveWayType giveWayType)
		{
			for (int i = 0; i < _waypointIndexes.Count; i++)
			{
				if (waypointIndex == _waypointIndexes[i])
				{
					SetGiveWayState(i, giveWayType);
					return true;
				}
			}
			return false;
		}

		public void AddOtherLaneWaypoint(int waypointIndex, Vector3 waypointPosition, float waypointSpeed, bool stop, GiveWayType giveWayType, int angle)
		{
			for (int num = _waypointIndexes.Count - 1; num >= 0; num--)
			{
				RemoveWaypointFromTarget(_waypointIndexes[num]);
			}
			_activePosition = 0;
			TriggerGiveWayWaypointsUpdatedEvent(_associatedVehicle);
			TriggerStopWaypointsUpdatedEvent(_associatedVehicle);
			AddWaypointAsTarget(waypointIndex, waypointPosition, waypointSpeed, stop, giveWayType, angle);
		}

		public void SetPath(List<int> pathWaypoints)
		{
			_hasPath = true;
			for (int num = _waypointIndexes.Count - 1; num >= 0; num--)
			{
				RemoveWaypointFromTarget(_waypointIndexes[num]);
			}
			_customPath = new Queue<int>(pathWaypoints);
			_activePosition = 0;
		}

		public void RemovePath()
		{
			_hasPath = false;
			if (_customPath != null)
			{
				_customPath.Clear();
			}
		}

		public void AddWaypointAsTarget(int waypointIndex, Vector3 waypointPosition, float waypointSpeed, bool stop, GiveWayType giveWayType, int angle)
		{
			if (waypointIndex < 0)
			{
				Debug.LogError($"Cannot add {waypointIndex} as target");
				return;
			}
			_waypointIndexes.Add(waypointIndex);
			_positions.Add(waypointPosition);
			waypointSpeed *= _speedVariationPercent;
			waypointSpeed += 15f;
			waypointSpeed = Mathf.Min(waypointSpeed, _maxVehicleSpeed);
			if (_maxSpeed.Count == 0 || _maxSpeed[_maxSpeed.Count - 1] <= waypointSpeed)
			{
				_slowDown.Add(item: false);
			}
			else
			{
				_slowDown.Add(item: true);
				TriggerSlowDownWaypointsUpdatedEvent(_associatedVehicle);
			}
			_angle.Add(angle);
			_maxSpeed.Add(waypointSpeed.KMHToMS());
			_stop.Add(stop);
			_giveWay.Add(giveWayType);
			TriggerKnownListUpdatedEvent(_associatedVehicle);
			if (stop)
			{
				TriggerStopWaypointsUpdatedEvent(_associatedVehicle);
			}
			if (giveWayType != GiveWayType.None)
			{
				TriggerGiveWayWaypointsUpdatedEvent(_associatedVehicle);
			}
		}

		public void RemoveWaypointFromTarget(int waypointIndex)
		{
			int num = _waypointIndexes.IndexOf(waypointIndex);
			if (num >= 0)
			{
				_waypointIndexes.RemoveAt(num);
				_positions.RemoveAt(num);
				_maxSpeed.RemoveAt(num);
				_slowDown.RemoveAt(num);
				_stop.RemoveAt(num);
				_giveWay.RemoveAt(num);
				_angle.RemoveAt(num);
			}
		}

		public int GetLastWaypointIndex()
		{
			if (_waypointIndexes.Count == 0)
			{
				return -1;
			}
			return _waypointIndexes[_waypointIndexes.Count - 1];
		}

		public Vector3 GetLastWaypointPosition()
		{
			if (_positions.Count == 0)
			{
				return TrafficSystemConstants.DEFAULT_POSITION;
			}
			return GetPosition(_positions.Count - 1);
		}

		public bool IsWaypointATarget(int waypointIndex)
		{
			return _waypointIndexes.Contains(waypointIndex);
		}

		public bool WaypointCanBeReached(int targetWaypointIndex)
		{
			bool result = false;
			for (int i = 0; i < _waypointIndexes.Count; i++)
			{
				if (_waypointIndexes[i] == targetWaypointIndex)
				{
					return true;
				}
			}
			return result;
		}

		public void TargetPassed()
		{
			if (_waypointIndexes.Count > 0)
			{
				if (_waypointIndexes[0] >= 0)
				{
					OldWaypointIndex = _waypointIndexes[0];
					_coveredWaypoints.Enqueue(OldWaypointIndex);
				}
				OldPosition = _positions[0];
				_waypointIndexes.RemoveAt(0);
				_positions.RemoveAt(0);
				_maxSpeed.RemoveAt(0);
				_slowDown.RemoveAt(0);
				_stop.RemoveAt(0);
				_giveWay.RemoveAt(0);
				_angle.RemoveAt(0);
				_activePosition--;
				TriggerKnownListUpdatedEvent(_associatedVehicle);
			}
		}

		public void IncreaseActivePosition()
		{
			if (PathLength - 1 > _activePosition)
			{
				_activePosition++;
			}
			TriggerKnownListUpdatedEvent(_associatedVehicle);
		}

		public void ClearTarget()
		{
			_waypointIndexes.Clear();
			_positions.Clear();
			_maxSpeed.Clear();
			_slowDown.Clear();
			_stop.Clear();
			_giveWay.Clear();
			_angle.Clear();
			OldWaypointIndex = -1;
			OldPosition = default;
			_obstacles.Clear();
			_activePosition = 0;
			_coveredWaypoints.Clear();
			RemovePath();
		}

		public void SetMaxVehicleSpeed(float maxVehicleSpeed)
		{
			_maxVehicleSpeed = maxVehicleSpeed.ToKMH();
		}

		private bool IsPathPositionValid(int pathPosition)
		{
			if (pathPosition < 0)
			{
				return false;
			}
			if (pathPosition >= _waypointIndexes.Count)
			{
				return false;
			}
			return true;
		}

		public void PrintWaypoints()
		{
			string text = string.Empty;
			for (int i = 0; i < _waypointIndexes.Count; i++)
			{
				text += $"{_waypointIndexes[i]} {_positions[i]} \n";
			}
			Debug.Log(text);
		}

		public Vector3 GetFirstStopPosition()
		{
			for (int i = 0; i < _stop.Count; i++)
			{
				if (_stop[i])
				{
					return GetPosition(i);
				}
			}
			return TrafficSystemConstants.DEFAULT_POSITION;
		}

		public Vector3 GetFirstGiveWayPosition()
		{
			for (int i = 0; i < _giveWay.Count; i++)
			{
				if (_giveWay[i] != GiveWayType.None)
				{
					return GetPosition(i);
				}
			}
			return TrafficSystemConstants.DEFAULT_POSITION;
		}

		public void SetClosestObstacleAndSpeed(Vector3 targetPosition)
		{
			if (HasObstacles())
			{
				int index = 0;
				float3 float5 = TrafficSystemConstants.DEFAULT_POSITION;
				float num = float.MaxValue;
				for (int i = 0; i < _obstacles.Count; i++)
				{
					Vector3 vector = ((!_obstacles[i].IsConvex) ? _obstacles[i].Collider.bounds.ClosestPoint(targetPosition) : _obstacles[i].Collider.ClosestPoint(targetPosition));
					float num2 = Vector3.SqrMagnitude(vector - targetPosition);
					if (Vector3.SqrMagnitude(vector - targetPosition) < num)
					{
						float5 = vector;
						num = num2;
						index = i;
					}
				}
				_closestObstaclePoint = float5;
				_closestObstacle = _obstacles[index];
				if (_closestObstacle.ObstacleType == ObstacleTypes.Player || _closestObstacle.ObstacleType == ObstacleTypes.TrafficVehicle)
				{
					if (_closestObstacle.VehicleScript != null)
					{
						_followSpeed = _closestObstacle.VehicleScript.GetCurrentSpeedMS();
						return;
					}
					Debug.LogWarning(_closestObstacle.Collider.name + " is a vehicle but does not implement the ITrafficParticipant interface. If this is your player, attach the PlayerComponent or your own implementation of the interface.", _closestObstacle.Collider);
					_followSpeed = TrafficSystemConstants.DEFAULT_SPEED;
				}
				else
				{
					_followSpeed = TrafficSystemConstants.DEFAULT_SPEED;
				}
			}
			else
			{
				_closestObstacle = default;
				_closestObstaclePoint = TrafficSystemConstants.DEFAULT_POSITION;
				_followSpeed = TrafficSystemConstants.DEFAULT_SPEED;
			}
		}

		public Vector3 GetFirstSlowDownPosition()
		{
			for (int i = 0; i < _slowDown.Count; i++)
			{
				if (_slowDown[i])
				{
					return GetPosition(i);
				}
			}
			return TrafficSystemConstants.DEFAULT_POSITION;
		}

		public bool IsFirstWaypointGiveWay()
		{
			if (_activePosition > 0)
			{
				return _giveWay[0] != GiveWayType.None;
			}
			return false;
		}

		public float GetFirstSlowDownSpeed()
		{
			for (int i = 0; i < _slowDown.Count; i++)
			{
				if (_slowDown[i])
				{
					return GetWaypointSpeed(i);
				}
			}
			return TrafficSystemConstants.DEFAULT_SPEED;
		}

		public bool IsAllowedToChange()
		{
			if (_activePosition <= 0)
			{
				return false;
			}
			if (_ignoreRuls)
			{
				return true;
			}
			if (_stop[0])
			{
				return false;
			}
			if (_giveWay[0] != GiveWayType.None)
			{
				return false;
			}
			return true;
		}

		public void UpdateCoveredWaypoints(Vector3 position, Vector3 forward)
		{
			if (_coveredWaypoints.Count > 0)
			{
				int waypointIndex = _coveredWaypoints.Peek();
				Vector3.Distance(API.GetWaypointFromIndex(waypointIndex).Position, position);
				if (Vector3.Dot((float3)(API.GetWaypointFromIndex(waypointIndex).Position - position), forward) < 0f)
				{
					_coveredWaypoints.Dequeue();
					UpdateCoveredWaypoints(position, forward);
				}
			}
		}
	}
}

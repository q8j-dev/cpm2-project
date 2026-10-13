using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CircuitRacingWaypoints : MonoBehaviour
{
	[Serializable]
	public class WaypointSet
	{
		public List<RacingWaypoint> waypoints = new List<RacingWaypoint>();
	}

	public int indexToUse;

	public int TestWayDirection;

	public GameObject car;

	public GameObject waypointPrefab;

	public float distanceThreshold = 10f;

	public bool recording;

	public int smoothingPointsPerSegment = 20;

	public List<RacingWaypoint> currentWaypoints = new List<RacingWaypoint>();

	public List<RacingWaypoint> PitStopWaypoints = new List<RacingWaypoint>();

	[SerializeField]
	public List<WaypointSet> AllPathWaypoints = new List<WaypointSet>();

	private Vector3 lastWaypointPosition;

	private GameObject parentObject;

	public LayerMask roadLayer;

	public List<GameObject> carsOvertaking;

	private RaceConfigSO _raceConfig;

	private bool lastState;

	public bool HasBakedData
	{
		get
		{
			if (_raceConfig != null && _raceConfig.BakedTrajectory != null)
			{
				return _raceConfig.BakedTrajectory.Count > 0;
			}
			return false;
		}
	}

	public BakedTrajectory ActiveTrajectory => _raceConfig.BakedTrajectory;

	public BakedTrajectory ActivePitStopTrajectory => _raceConfig.PitStopTrajectory;

	public int WaypointCount => ActiveTrajectory.Count;

	public int PitStopWaypointCount => _raceConfig.PitStopTrajectory?.Count ?? 0;

	public void SetRaceConfig(RaceConfigSO config)
	{
		_raceConfig = config;
	}

	public ref WaypointData GetWaypoint(int index)
	{
		return ref ActiveTrajectory.GetWaypoint(index);
	}

	public ref WaypointData GetPitStopWaypoint(int index)
	{
		return ref _raceConfig.PitStopTrajectory.GetWaypoint(index);
	}

	public int WrapIndex(int index)
	{
		return ActiveTrajectory.WrapIndex(index);
	}

	public int WrapPitStopIndex(int index)
	{
		return _raceConfig.PitStopTrajectory.WrapIndex(index);
	}

	private void Start()
	{
		if (!HasBakedData)
		{
			currentWaypoints = AllPathWaypoints[indexToUse].waypoints;
		}
	}

	public void SetIndexToUse(int index)
	{
		indexToUse = index;
	}

	public void SetWaypoints(int index)
	{
		indexToUse = index;
		if (HasBakedData)
		{
			return;
		}
		currentWaypoints = AllPathWaypoints[index].waypoints;
		foreach (RacingWaypoint currentWaypoint in currentWaypoints)
		{
			currentWaypoint.CachedPosition = currentWaypoint.transform.position;
			currentWaypoint.CachedRight = currentWaypoint.transform.right;
			if ((bool)currentWaypoint.left)
			{
				currentWaypoint.leftBoundary = currentWaypoint.left.transform.position;
			}
			if ((bool)currentWaypoint.right)
			{
				currentWaypoint.rightBoundary = currentWaypoint.right.transform.position;
			}
		}
		foreach (RacingWaypoint pitStopWaypoint in PitStopWaypoints)
		{
			pitStopWaypoint.CachedPosition = pitStopWaypoint.transform.position;
			pitStopWaypoint.CachedRight = pitStopWaypoint.transform.right;
			if ((bool)pitStopWaypoint.left)
			{
				pitStopWaypoint.leftBoundary = pitStopWaypoint.left.transform.position;
			}
			if ((bool)pitStopWaypoint.right)
			{
				pitStopWaypoint.rightBoundary = pitStopWaypoint.right.transform.position;
			}
		}
		CacheWaypointData();
	}

	private void OnEnable()
	{
		if (car != null)
		{
			lastWaypointPosition = car.transform.position;
		}
	}

	private void AddNewWaypoint()
	{
		parentObject = null;
		if (AllPathWaypoints == null)
		{
			AllPathWaypoints = new List<WaypointSet>();
		}
		WaypointSet waypointSet = new WaypointSet();
		waypointSet.waypoints = currentWaypoints;
		AllPathWaypoints.Add(waypointSet);
	}

	public List<RacingWaypoint> GetCurrentWaypoints()
	{
		return AllPathWaypoints[indexToUse].waypoints;
	}

	[ContextMenu("Parse boundaries")]
	public void ParseBoundaries()
	{
		foreach (RacingWaypoint currentWaypoint in currentWaypoints)
		{
			currentWaypoint.right.transform.localScale = Vector3.one * 0.5f;
			currentWaypoint.left.transform.localScale = Vector3.one * 0.5f;
			currentWaypoint.leftBoundary = currentWaypoint.left.transform.position;
			currentWaypoint.rightBoundary = currentWaypoint.right.transform.position;
			currentWaypoint.steeringAngle = CalculateSteeringAngle(currentWaypoint.previous, currentWaypoint, currentWaypoint.next);
		}
	}

	[ContextMenu("Clear Waypoints")]
	public void ClearWaypoints()
	{
		if (parentObject != null)
		{
			UnityEngine.Object.DestroyImmediate(parentObject);
		}
		currentWaypoints.Clear();
		lastWaypointPosition = ((car != null) ? car.transform.position : Vector3.zero);
		Debug.Log("Waypoints cleared.");
	}

	private void CreateWaypoint(Vector3 position)
	{
		if (parentObject == null)
		{
			parentObject = new GameObject("WaypointsParent Circuit");
			parentObject.transform.parent = base.transform;
		}
		RacingWaypoint racingWaypoint = UnityEngine.Object.Instantiate(waypointPrefab, position, Quaternion.identity, parentObject.transform).AddComponent<RacingWaypoint>();
		if (currentWaypoints.Count > 0)
		{
			RacingWaypoint racingWaypoint2 = (racingWaypoint.previous = currentWaypoints[currentWaypoints.Count - 1]);
			racingWaypoint2.next = racingWaypoint;
			if (racingWaypoint2.previous != null)
			{
				racingWaypoint2.steeringAngle = CalculateSteeringAngle(racingWaypoint2.previous, racingWaypoint2, racingWaypoint);
			}
		}
		currentWaypoints.Add(racingWaypoint);
	}

	public float CalculateSteeringAngle(RacingWaypoint previous, RacingWaypoint current, RacingWaypoint next)
	{
		Vector3 normalized = (current.transform.position - previous.transform.position).normalized;
		Vector3 normalized2 = (next.transform.position - current.transform.position).normalized;
		return Vector3.SignedAngle(normalized, normalized2, Vector3.up);
	}

	public void ConnectFirstAndLastWaypoints()
	{
		if (currentWaypoints.Count > 1)
		{
			RacingWaypoint racingWaypoint = currentWaypoints[0];
			RacingWaypoint racingWaypoint2 = currentWaypoints[currentWaypoints.Count - 1];
			racingWaypoint2.next = racingWaypoint;
			racingWaypoint.previous = racingWaypoint2;
			racingWaypoint2.steeringAngle = CalculateSteeringAngle(currentWaypoints[currentWaypoints.Count - 2], racingWaypoint2, racingWaypoint);
		}
	}

	private void OnDrawGizmos()
	{
		if (currentWaypoints.Count >= 4)
		{
			Gizmos.color = Color.green;
			for (int i = 0; i < currentWaypoints.Count - 3; i++)
			{
				DrawCatmullRomSpline(i);
			}
		}
	}

	private void DrawCatmullRomSpline(int index)
	{
		Vector3 position = currentWaypoints[index].transform.position;
		Vector3 position2 = currentWaypoints[index + 1].transform.position;
		Vector3 position3 = currentWaypoints[index + 2].transform.position;
		Vector3 position4 = currentWaypoints[index + 3].transform.position;
		Vector3 vector = position2;
		for (int i = 1; i <= smoothingPointsPerSegment; i++)
		{
			float t = (float)i / (float)smoothingPointsPerSegment;
			Vector3 vector2 = CalculateCatmullRomPosition(t, position, position2, position3, position4);
			Gizmos.DrawLine(vector, vector2);
			vector = vector2;
		}
	}

	private Vector3 CalculateCatmullRomPosition(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
	{
		float num = t * t;
		float num2 = num * t;
		return 0.5f * (2f * p1 + (-p0 + p2) * t + (2f * p0 - 5f * p1 + 4f * p2 - p3) * num + (-p0 + 3f * p1 - 3f * p2 + p3) * num2);
	}

	private void CacheWaypointData()
	{
		for (int i = 0; i < currentWaypoints.Count; i++)
		{
			RacingWaypoint racingWaypoint = currentWaypoints[i];
			if (racingWaypoint.next != null)
			{
				racingWaypoint.DistanceToNext = Vector3.Distance(racingWaypoint.CachedPosition, racingWaypoint.next.CachedPosition);
			}
			else
			{
				racingWaypoint.DistanceToNext = 0f;
			}
		}
	}

	public List<Vector3> GetSplineData()
	{
		List<Vector3> list = new List<Vector3>();
		if (HasBakedData)
		{
			BakedTrajectory activeTrajectory = ActiveTrajectory;
			for (int i = 0; i < activeTrajectory.Count; i++)
			{
				list.Add(activeTrajectory.Waypoints[i].Position);
			}
			return list;
		}
		foreach (RacingWaypoint currentWaypoint in currentWaypoints)
		{
			if (currentWaypoint != null)
			{
				list.Add(currentWaypoint.transform.position);
			}
		}
		return list;
	}
}

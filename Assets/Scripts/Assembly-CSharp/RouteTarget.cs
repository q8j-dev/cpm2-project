using UnityEngine;

public struct RouteTarget
{
	public Vector3 WorldTarget;

	public Vector3 LerpedTarget;

	public float SteeringNormalized;

	public float LookAheadCurvature;

	public float DistanceToCurrentWaypoint;

	public int CurrentWaypointIndex;

	public float FollowOffset;
}

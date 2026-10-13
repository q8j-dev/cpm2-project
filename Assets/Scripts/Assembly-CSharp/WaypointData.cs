using System;
using UnityEngine;

[Serializable]
public struct WaypointData
{
	public Vector3 Position;

	public Vector3 Right;

	public Vector3 LeftBoundary;

	public Vector3 RightBoundary;

	public float ForwardSteeringAngle;

	public float ReverseSteeringAngle;

	public float DistanceToNext;

	public float TurnRadius;

	public bool ShouldBrake;
}

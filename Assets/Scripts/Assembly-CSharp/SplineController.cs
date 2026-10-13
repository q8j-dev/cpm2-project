using UnityEngine;
using UnityEngine.Splines;

public class SplineController : MonoBehaviour
{
	[SerializeField]
	private CircuitRacingWaypoints CircuitRacingWaypoints;

	public SplineContainer SplineContainer;

	[SerializeField]
	private float SplineDistance;

	[SerializeField]
	private Transform StartPos;

	[SerializeField]
	private Transform WayPointParent;

	[SerializeField]
	private WaypointData[] WayPoints;

	[SerializeField]
	private float WayPointDistance;

	[SerializeField]
	private Spline Spline;

	[SerializeField]
	private Spline ReversedSpline;

	public SplineContainer GetSplineContainer()
	{
		return SplineContainer;
	}

	public float GetSplineLength()
	{
		return SplineDistance;
	}
}

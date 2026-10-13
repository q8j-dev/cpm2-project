using UnityEngine;
using UnityEngine.Splines;

public class SplineController : MonoBehaviour
{
	public SplineContainer SplineContainer;

	[SerializeField]
	private float SplineDistance = default;

	[SerializeField]
	private WaypointData[] WayPoints;

	public SplineContainer GetSplineContainer()
	{
		return SplineContainer;
	}

	public float GetSplineLength()
	{
		return SplineDistance;
	}
}

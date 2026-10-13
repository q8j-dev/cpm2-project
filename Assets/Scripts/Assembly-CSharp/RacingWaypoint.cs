using System;
using UnityEngine;

public class RacingWaypoint : MonoBehaviour
{
	public int index;

	public RacingWaypoint previous;

	public RacingWaypoint next;

	public float steeringAngle;

	public Vector3 leftBoundary;

	public Vector3 rightBoundary;

	public Vector3 CachedPosition;

	public Vector3 CachedRight;

	public GameObject left;

	public GameObject right;

	public MeshRenderer meshRenderer;

	public float ForwardTurnSteeringAngle;

	public float ReverseTurnSteeringAngle;

	public float DistanceToNext;

	public bool shouldBrake;

	private void Awake()
	{
		meshRenderer = GetComponent<MeshRenderer>();
	}

	public void SetColor(bool state)
	{
		meshRenderer.material.SetColor("_BaseColor", state ? Color.yellow : Color.red);
	}

	[ContextMenu("Reverse")]
	public void Reverse()
	{
		GameObject gameObject = left;
		left = right;
		right = gameObject;
	}

	[ContextMenu("ParseBoundaries")]
	public void ParseBoundaries()
	{
		left = base.transform.GetChild(0).gameObject;
		right = base.transform.GetChild(1).gameObject;
		leftBoundary = left.transform.position;
		rightBoundary = right.transform.position;
	}

	public float CalculateTurnRadius(RacingWaypoint currentPoint, int step)
	{
		RacingWaypoint racingWaypoint = currentPoint.previous;
		RacingWaypoint racingWaypoint2 = currentPoint.next;
		for (int i = 0; i < step; i++)
		{
			if (racingWaypoint.previous != null)
			{
				racingWaypoint = racingWaypoint.previous;
			}
			if (racingWaypoint2.next != null)
			{
				racingWaypoint2 = racingWaypoint2.next;
			}
		}
		Vector3 cachedPosition = racingWaypoint.CachedPosition;
		Vector3 cachedPosition2 = racingWaypoint2.CachedPosition;
		Vector3 vector = currentPoint.CachedPosition - cachedPosition;
		Vector3 to = cachedPosition2 - currentPoint.CachedPosition;
		float f = Vector3.Angle(vector, to) * ((float)Math.PI / 180f);
		float magnitude = vector.magnitude;
		float magnitude2 = to.magnitude;
		float num = Vector3.Distance(cachedPosition, cachedPosition2);
		float num2 = 0.5f * Mathf.Abs(magnitude * magnitude2 * Mathf.Sin(f));
		if (num2 <= 1E-06f)
		{
			return float.MaxValue;
		}
		float num3 = magnitude * magnitude2 * num / (4f * num2);
		if (float.IsNaN(num3) || float.IsInfinity(num3) || num3 <= 0f)
		{
			return float.MaxValue;
		}
		return num3;
	}
}

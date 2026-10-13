using SWS;
using UnityEngine;

public class RotateWheelWaypointTraffic : MonoBehaviour
{
	public float speedKof = 40f;

	public splineMove sm;

	public bool FrontWheel;

	private void Start()
	{
	}

	private void Update()
	{
		base.transform.Rotate(Vector3.right * Time.deltaTime * sm.speed * speedKof);
	}

	private float AngleBetweenVector2(Vector2 vec1, Vector2 vec2)
	{
		Vector2 to = vec2 - vec1;
		float num = ((vec2.y < vec1.y) ? (-1f) : 1f);
		return Vector2.Angle(Vector2.right, to) * num;
	}
}

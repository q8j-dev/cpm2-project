using System;

[Serializable]
public class BakedTrajectory
{
	public WaypointData[] Waypoints = Array.Empty<WaypointData>();

	public float TotalLength;

	public bool IsLoop = true;

	public float SampleSpacing = 2f;

	public int Count => Waypoints.Length;

	public int WrapIndex(int index)
	{
		if (Waypoints.Length == 0)
		{
			return 0;
		}
		return (index % Waypoints.Length + Waypoints.Length) % Waypoints.Length;
	}

	public ref WaypointData GetWaypoint(int index)
	{
		return ref Waypoints[WrapIndex(index)];
	}

	public int Advance(int index, int steps, int direction)
	{
		return WrapIndex(index + steps * direction);
	}
}

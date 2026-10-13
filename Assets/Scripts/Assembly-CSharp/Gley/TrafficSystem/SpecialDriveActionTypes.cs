namespace Gley.TrafficSystem
{
	public enum SpecialDriveActionTypes
	{
		Forward = 0,
		ForceForward = 1,
		Continue = 10,
		ChangeLane = 11,
		Follow = 12,
		Overtake = 13,
		AvoidForward = 15,
		GiveWay = 16,
		StopInPoint = 20,
		StopInDistance = 70,
		StopTemp = 90,
		Stop = 91,
		Reverse = 80,
		AvoidReverse = 100,
		NoWaypoint = 1000,
		NoPath = 1001,
		Destroyed = 10000
	}
}

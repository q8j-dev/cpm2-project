using CPMEngine;
using UnityEngine;

public struct BotContext
{
	public Vector3 Position;

	public Quaternion Rotation;

	public Quaternion InvRotation;

	public Vector3 Forward;

	public float SpeedMps;

	public float SpeedKmh;

	public Powertrain Powertrain;

	public float MaxSpeedKmh;

	public float BrakeAccelMps2;

	public CarSteeringState OuterState;

	public int Direction;

	public float Dt;

	public float TimeSinceRaceStart;

	public float LaneWidth;

	public int TotalLanes;

	public string OwnerName;
}

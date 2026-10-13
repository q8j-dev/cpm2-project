using UnityEngine;

public struct NearbyCar
{
	public CircuitCarStearing Source;

	public Rigidbody Rigidbody;

	public Vector3 WorldPos;

	public Vector3 Velocity;

	public float ForwardDist;

	public float SideDist;

	public float ClosingSpeed;

	public float Ttc;

	public bool SameDirection;

	public int TrajectoryLane;

	public float DecelMps2;

	public bool IsPlayer;
}

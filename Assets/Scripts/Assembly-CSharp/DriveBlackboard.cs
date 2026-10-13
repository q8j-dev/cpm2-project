using UnityEngine;

public class DriveBlackboard
{
	public int LaneIndex;

	public float DesiredLateralOffset;

	public bool SuppressApex;

	public float ApexBias = 1f;

	public int GridColumn = 2;

	public int GridRow;

	public float RaceStartTime;

	public string IncumbentBehavior = "";

	public float IncumbentSince;

	public float LaneChangeCooldown;

	public float LaneChangeGraceTimer;

	public float StuckTimer;

	public float ReversingTime;

	public Vector3 ReversePosStart;

	public float OvertakeCommitTimer;

	public int OvertakeTargetLane;

	public int OvertakeSourceLane;

	public GameObject OvertakenCar;

	public bool IsCarOutOfRoad;

	public bool Reverse;

	public void Reset()
	{
		LaneIndex = 0;
		DesiredLateralOffset = 0f;
		SuppressApex = false;
		ApexBias = 1f;
		IncumbentBehavior = "";
		IncumbentSince = 0f;
		LaneChangeCooldown = 0f;
		LaneChangeGraceTimer = 0f;
		StuckTimer = 0f;
		ReversingTime = 0f;
		ReversePosStart = Vector3.zero;
		OvertakeCommitTimer = 0f;
		OvertakeTargetLane = 0;
		OvertakeSourceLane = 0;
		OvertakenCar = null;
		IsCarOutOfRoad = false;
		Reverse = false;
	}
}

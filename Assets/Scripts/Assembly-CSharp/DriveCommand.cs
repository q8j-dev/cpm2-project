public struct DriveCommand
{
	public float Throttle01;

	public float Brake01;

	public float? SteeringOverride;

	public float DesiredLateralOffset;

	public int LaneIndexHint;

	public bool SuppressApex;

	public bool RequestReverse;

	public string ActiveBehaviorName;
}

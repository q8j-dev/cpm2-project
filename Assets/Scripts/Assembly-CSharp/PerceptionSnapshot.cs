public struct PerceptionSnapshot
{
	public const int MaxCars = 10;

	public NearbyCar[] Cars;

	public int Count;

	public int CarAheadIndex;

	public float DistanceAhead;

	public float TtcAhead;

	public float ClosingSpeed;

	public float AheadSpeedMps;

	public float AheadDecelMps2;

	public bool CarAheadIsPlayer;

	public int CarBehindIndex;

	public float DistanceBehind;

	public float ClosingSpeedBehind;

	public float BehindSideDist;

	public bool CarBehindIsPlayer;

	public LaneOccupancy Lanes;

	public bool LaneAheadHasBrakingCar;
}

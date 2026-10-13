using System.Collections.Generic;
using UnityEngine;

public class PitStopStarter : CircuitAiCarStateChanger
{
	[SerializeField]
	private PitStopTrigger PitStopTrigger;

	public List<RacingWaypoint> RacingWaypoints = new List<RacingWaypoint>();

	[SerializeField]
	private Transform PitStopWaypointParent;

	public BakedTrajectory PitStopBakedPath;

	public override void OnLapRaceCarEnter(LapRaceCar lapRaceCar)
	{
		if (!PitStopTrigger.IsBusy && lapRaceCar.IsBroken && lapRaceCar.GetRaceCarId() == PitStopTrigger.CarPlaceId)
		{
			PitStopTrigger.IsBusy = true;
			lapRaceCar.SetPitStopWaypoints(PitStopBakedPath);
			lapRaceCar.ChangeCarState(TargetState);
		}
	}
}

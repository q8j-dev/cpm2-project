using UnityEngine;

public class CruiseBehavior : IDrivingBehavior
{
	public string Name => "Cruise";

	public float Score(in BotContext ctx, in PerceptionSnapshot snap, in RouteTarget route, in SpeedTarget speed, DriveBlackboard bb)
	{
		return 0.1f;
	}

	public DriveCommand Produce(in BotContext ctx, in PerceptionSnapshot snap, in RouteTarget route, in SpeedTarget speed, DriveBlackboard bb)
	{
		float coastSpeedKmh = speed.CoastSpeedKmh;
		float speedKmh = ctx.SpeedKmh;
		float throttle = ((speedKmh < coastSpeedKmh) ? 1f : 0f);
		float num = Mathf.Max(speed.BrakeMagnitude, (speedKmh > coastSpeedKmh + 10f) ? 0.3f : 0f);
		if (num > 0.1f)
		{
			throttle = 0f;
		}
		return new DriveCommand
		{
			Throttle01 = throttle,
			Brake01 = num,
			DesiredLateralOffset = bb.DesiredLateralOffset,
			LaneIndexHint = bb.LaneIndex,
			SuppressApex = false
		};
	}

	float IDrivingBehavior.Score(in BotContext ctx, in PerceptionSnapshot snap, in RouteTarget route, in SpeedTarget speed, DriveBlackboard bb)
	{
		return Score(in ctx, in snap, in route, in speed, bb);
	}

	DriveCommand IDrivingBehavior.Produce(in BotContext ctx, in PerceptionSnapshot snap, in RouteTarget route, in SpeedTarget speed, DriveBlackboard bb)
	{
		return Produce(in ctx, in snap, in route, in speed, bb);
	}
}

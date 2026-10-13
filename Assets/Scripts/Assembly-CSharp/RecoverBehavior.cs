using UnityEngine;

public class RecoverBehavior : IDrivingBehavior
{
	public string Name => "Recover";

	public float Score(in BotContext ctx, in PerceptionSnapshot snap, in RouteTarget route, in SpeedTarget speed, DriveBlackboard bb)
	{
		if (bb.IncumbentBehavior == "Recover" && Time.time - bb.IncumbentSince > 5f && ctx.SpeedMps < 1f)
		{
			bb.IsCarOutOfRoad = false;
			bb.Reverse = false;
			bb.StuckTimer = 0f;
			return 0f;
		}
		if (bb.IsCarOutOfRoad)
		{
			return 0.95f;
		}
		if (bb.Reverse)
		{
			return 0.9f;
		}
		if (bb.StuckTimer > 1f)
		{
			return 0.85f;
		}
		return 0f;
	}

	public DriveCommand Produce(in BotContext ctx, in PerceptionSnapshot snap, in RouteTarget route, in SpeedTarget speed, DriveBlackboard bb)
	{
		if (bb.IsCarOutOfRoad)
		{
			return new DriveCommand
			{
				Throttle01 = 0.3f,
				Brake01 = 0f,
				DesiredLateralOffset = 0f,
				LaneIndexHint = 0,
				SuppressApex = true
			};
		}
		if (bb.Reverse || bb.StuckTimer > 1f)
		{
			bb.Reverse = true;
			return new DriveCommand
			{
				Throttle01 = 0.5f,
				Brake01 = 0f,
				DesiredLateralOffset = 0f,
				LaneIndexHint = 0,
				SuppressApex = true,
				RequestReverse = true
			};
		}
		return new DriveCommand
		{
			Throttle01 = 0.3f,
			Brake01 = 0f,
			DesiredLateralOffset = 0f,
			LaneIndexHint = 0,
			SuppressApex = true
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

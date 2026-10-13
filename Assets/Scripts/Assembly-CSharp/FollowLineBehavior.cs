using UnityEngine;

public class FollowLineBehavior : IDrivingBehavior
{
	public string Name => "FollowLine";

	public float Score(in BotContext ctx, in PerceptionSnapshot snap, in RouteTarget route, in SpeedTarget speed, DriveBlackboard bb)
	{
		return 0.5f;
	}

	public DriveCommand Produce(in BotContext ctx, in PerceptionSnapshot snap, in RouteTarget route, in SpeedTarget speed, DriveBlackboard bb)
	{
		float speedKmh = ctx.SpeedKmh;
		float coastSpeedKmh = speed.CoastSpeedKmh;
		float num = speedKmh - coastSpeedKmh;
		float throttle;
		float a;
		if (num > 3f)
		{
			a = Mathf.Clamp01((num - 3f) / 20f);
			throttle = 0f;
		}
		else if (num < -3f)
		{
			throttle = Mathf.Clamp01((0f - num - 3f) / 10f);
			a = 0f;
		}
		else
		{
			throttle = 0.2f;
			a = 0f;
		}
		a = Mathf.Max(a, speed.BrakeMagnitude);
		if (a > 0.1f)
		{
			throttle = 0f;
		}
		int num2 = bb.LaneIndex;
		if (bb.LaneChangeCooldown <= 0f && bb.LaneIndex != 0 && (snap.CarAheadIndex < 0 || snap.DistanceAhead > 40f))
		{
			int num3 = ((bb.LaneIndex <= 0) ? 1 : (-1));
			int num4 = bb.LaneIndex + num3;
			if (snap.Lanes.IsLaneFreeAhead(num4) && snap.Lanes.IsLaneSafeBehind(num4))
			{
				num2 = num4;
				bb.LaneChangeCooldown = 0.5f;
				BotLog.Lane(ctx.OwnerName, $"DRIFT to centre {bb.LaneIndex}→{num2}");
			}
		}
		float num5 = Mathf.Max(0.5f, ctx.LaneWidth);
		return new DriveCommand
		{
			Throttle01 = throttle,
			Brake01 = a,
			DesiredLateralOffset = (float)num2 * num5,
			LaneIndexHint = num2,
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

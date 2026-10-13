using UnityEngine;

public class MatchPaceBehavior : IDrivingBehavior
{
	public string Name => "MatchPace";

	public float Score(in BotContext ctx, in PerceptionSnapshot snap, in RouteTarget route, in SpeedTarget speed, DriveBlackboard bb)
	{
		if (snap.CarAheadIndex < 0)
		{
			return 0f;
		}
		float distanceAhead = snap.DistanceAhead;
		float num = Mathf.Max(15f, ctx.SpeedMps * 1.5f);
		if (distanceAhead > num)
		{
			return 0f;
		}
		float num2 = Mathf.Max(8f, ctx.SpeedMps * 0.8f);
		if (distanceAhead < num2)
		{
			return 0f;
		}
		return 0.3f;
	}

	public DriveCommand Produce(in BotContext ctx, in PerceptionSnapshot snap, in RouteTarget route, in SpeedTarget speed, DriveBlackboard bb)
	{
		float distanceAhead = snap.DistanceAhead;
		float num = Mathf.Max(8f, ctx.SpeedMps * 0.8f);
		float b = Mathf.Max(15f, ctx.SpeedMps * 0.5f);
		float t = Mathf.Clamp01((distanceAhead - num) / Mathf.Max(1f, b));
		float num2 = snap.AheadSpeedMps * Mathf.Lerp(0.85f, 1.05f, t);
		float num3;
		float throttle;
		if (ctx.SpeedMps > num2 + 1f)
		{
			num3 = Mathf.Clamp01((ctx.SpeedMps - num2) / 12f);
			throttle = 0f;
		}
		else
		{
			num3 = 0f;
			throttle = Mathf.Lerp(0.3f, 0.7f, t);
		}
		if (snap.AheadDecelMps2 > 3f)
		{
			num3 = Mathf.Max(num3, Mathf.Clamp01(snap.AheadDecelMps2 / 10f) * 0.5f);
		}
		if (num3 > 0.1f)
		{
			throttle = 0f;
		}
		return new DriveCommand
		{
			Throttle01 = throttle,
			Brake01 = num3,
			DesiredLateralOffset = (float)bb.LaneIndex * Mathf.Max(0.5f, ctx.LaneWidth),
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

using UnityEngine;

public class AvoidCollisionBehavior : IDrivingBehavior
{
	private const float BrakeAccelMps2 = 5f;

	private const float SafetyMargin = 1.4f;

	private const float MinSafetyM = 6f;

	private const float MatchPaceBufferBase = 20f;

	private const float MatchPaceBufferPerClosing = 2f;

	public string Name => "AvoidCollision";

	private static float BrakeZoneDistance(float closingMs)
	{
		float num = Mathf.Max(0f, closingMs);
		return num * num / 10f * 1.4f + 6f;
	}

	private static float MatchPaceZoneDistance(float closingMs)
	{
		float num = BrakeZoneDistance(closingMs);
		float num2 = 20f + Mathf.Max(0f, closingMs) * 2f;
		return num + num2;
	}

	public float Score(in BotContext ctx, in PerceptionSnapshot snap, in RouteTarget route, in SpeedTarget speed, DriveBlackboard bb)
	{
		if (snap.CarAheadIndex < 0)
		{
			return 0f;
		}
		float distanceAhead = snap.DistanceAhead;
		float closingMs = Mathf.Max(0f, snap.ClosingSpeed);
		float num = BrakeZoneDistance(closingMs);
		float num2 = MatchPaceZoneDistance(closingMs);
		if (distanceAhead < num)
		{
			float num3 = 1f - distanceAhead / num;
			return 1f + num3 * 0.5f;
		}
		if (distanceAhead < num2 && snap.ClosingSpeed > 5f)
		{
			return 0.6f;
		}
		return 0f;
	}

	public DriveCommand Produce(in BotContext ctx, in PerceptionSnapshot snap, in RouteTarget route, in SpeedTarget speed, DriveBlackboard bb)
	{
		float distanceAhead = snap.DistanceAhead;
		float num = BrakeZoneDistance(Mathf.Max(0f, snap.ClosingSpeed));
		float brake;
		float throttle;
		if (distanceAhead < num)
		{
			brake = ((!(distanceAhead < num * 0.35f)) ? Mathf.Clamp01(1f - (distanceAhead - num * 0.35f) / (num * 0.65f)) : 1f);
			throttle = 0f;
		}
		else
		{
			brake = 0f;
			float num2 = Mathf.Max(1f, snap.AheadSpeedMps);
			float num3 = Mathf.Max(1f, ctx.SpeedMps);
			throttle = Mathf.Clamp(num2 / num3 * 1.1f, 0.52f, 0.95f);
		}
		return new DriveCommand
		{
			Throttle01 = throttle,
			Brake01 = brake,
			DesiredLateralOffset = bb.DesiredLateralOffset,
			LaneIndexHint = bb.LaneIndex,
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

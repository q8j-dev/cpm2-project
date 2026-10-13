using UnityEngine;

public class OvertakeBehavior : IDrivingBehavior
{
	private const float MinSpeedAdvantageMps = 1.5f;

	private const float MinTtcSec = 1.5f;

	private const float MaxTtcSec = 8f;

	private const float MaxCornerCurvature = 0.15f;

	private const float CommitDuration = 2.5f;

	private const float CommitScore = 1.1f;

	private const float BailDistance = 6f;

	private const float BailClosingSpeed = 5f;

	public string Name => "Overtake";

	public float Score(in BotContext ctx, in PerceptionSnapshot snap, in RouteTarget route, in SpeedTarget speed, DriveBlackboard bb)
	{
		if (bb.OvertakeCommitTimer > 0f)
		{
			if (snap.CarAheadIndex >= 0 && snap.DistanceAhead < 6f && snap.ClosingSpeed > 5f)
			{
				bb.OvertakeCommitTimer = 0f;
				BotLog.Lane(ctx.OwnerName, $"OVERTAKE BAIL | d={snap.DistanceAhead:F1}m closing={snap.ClosingSpeed * 3.6f:F0}km/h");
				return 0f;
			}
			return 1.1f;
		}
		if (bb.LaneChangeCooldown > 0f)
		{
			return 0f;
		}
		if (snap.CarAheadIndex < 0)
		{
			return 0f;
		}
		if (ctx.SpeedMps < 5f)
		{
			return 0f;
		}
		ref NearbyCar reference = ref snap.Cars[snap.CarAheadIndex];
		float num = Mathf.Max(0.5f, ctx.LaneWidth);
		if (Mathf.Abs(reference.SideDist) > num * 0.7f)
		{
			return 0f;
		}
		float lookAheadCurvature = route.LookAheadCurvature;
		if (lookAheadCurvature > 0.15f)
		{
			return 0f;
		}
		float num2 = 1f - Mathf.Clamp01(lookAheadCurvature / 0.15f);
		float distanceAhead = snap.DistanceAhead;
		int num3 = bb.LaneIndex - 1;
		int num4 = bb.LaneIndex + 1;
		float num5 = ((num3 >= -2) ? snap.Lanes.NearestAheadDist(num3) : 0f);
		float num6 = ((num4 <= 2) ? snap.Lanes.NearestAheadDist(num4) : 0f);
		bool num7 = num3 >= -2 && snap.Lanes.IsLaneSafeBehind(num3) && num5 > distanceAhead + 8f;
		bool flag = num4 <= 2 && snap.Lanes.IsLaneSafeBehind(num4) && num6 > distanceAhead + 8f;
		if (!num7 && !flag)
		{
			return 0f;
		}
		if (snap.CarAheadIndex >= 0 && snap.DistanceAhead < 60f && snap.AheadSpeedMps < ctx.SpeedMps + 1.388889f)
		{
			return 0.95f * num2;
		}
		float ttcAhead = snap.TtcAhead;
		if (float.IsInfinity(ttcAhead) || ttcAhead < 1.5f || ttcAhead > 8f)
		{
			return 0f;
		}
		float num8 = ctx.SpeedMps - snap.AheadSpeedMps;
		if (num8 < 1.5f)
		{
			return 0f;
		}
		float num9 = Mathf.Clamp01(num8 / 8f);
		float num10 = Mathf.Clamp01(1f - Mathf.Abs(ttcAhead - 3.5f) / 4.5f);
		return 0.9f * num9 * num2 * num10;
	}

	public DriveCommand Produce(in BotContext ctx, in PerceptionSnapshot snap, in RouteTarget route, in SpeedTarget speed, DriveBlackboard bb)
	{
		float num = Mathf.Max(0.5f, ctx.LaneWidth);
		int num2;
		if (bb.OvertakeCommitTimer > 0f)
		{
			num2 = bb.OvertakeTargetLane;
		}
		else
		{
			float distanceAhead = snap.DistanceAhead;
			int num3 = bb.LaneIndex - 1;
			int num4 = bb.LaneIndex + 1;
			float num5 = ((num3 >= -2) ? snap.Lanes.NearestAheadDist(num3) : 0f);
			float num6 = ((num4 <= 2) ? snap.Lanes.NearestAheadDist(num4) : 0f);
			bool num7 = num3 >= -2 && snap.Lanes.IsLaneSafeBehind(num3) && num5 > distanceAhead + 8f;
			bool flag = num4 <= 2 && snap.Lanes.IsLaneSafeBehind(num4) && num6 > distanceAhead + 8f;
			num2 = bb.LaneIndex;
			if (num7 && (!flag || num5 >= num6))
			{
				num2 = num3;
			}
			else if (flag)
			{
				num2 = num4;
			}
			bb.OvertakeSourceLane = bb.LaneIndex;
			bb.OvertakeTargetLane = num2;
			bb.OvertakeCommitTimer = 2.5f;
			bb.LaneChangeCooldown = Mathf.Max(bb.LaneChangeCooldown, 0.3f);
			string arg = ((snap.CarAheadIndex >= 0 && snap.Cars[snap.CarAheadIndex].Source != null) ? snap.Cars[snap.CarAheadIndex].Source.name : "?");
			BotLog.Lane(ctx.OwnerName, $"OVERTAKE COMMIT {bb.LaneIndex}→{num2} ({2.5f:F1}s) | " + $"ahead={arg} d={snap.DistanceAhead:F0}m ttc={snap.TtcAhead:F1}s " + $"ΔV={(ctx.SpeedMps - snap.AheadSpeedMps) * 3.6f:F0}km/h");
		}
		float desiredLateralOffset = (float)num2 * num;
		return new DriveCommand
		{
			Throttle01 = 1f,
			Brake01 = 0f,
			DesiredLateralOffset = desiredLateralOffset,
			LaneIndexHint = num2,
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

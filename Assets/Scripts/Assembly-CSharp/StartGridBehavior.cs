using UnityEngine;

public class StartGridBehavior : IDrivingBehavior
{
	private const float Duration = 4f;

	public string Name => "StartGrid";

	public float Score(in BotContext ctx, in PerceptionSnapshot snap, in RouteTarget route, in SpeedTarget speed, DriveBlackboard bb)
	{
		float timeSinceRaceStart = ctx.TimeSinceRaceStart;
		if (timeSinceRaceStart < 0f || timeSinceRaceStart > 4f)
		{
			return 0f;
		}
		return Mathf.Lerp(0.95f, 0f, timeSinceRaceStart / 4f);
	}

	public DriveCommand Produce(in BotContext ctx, in PerceptionSnapshot snap, in RouteTarget route, in SpeedTarget speed, DriveBlackboard bb)
	{
		float t = Mathf.Clamp01(ctx.TimeSinceRaceStart / 4f);
		float throttle = ((bb.GridRow == 0) ? Mathf.Lerp(0.5f, 1f, t) : Mathf.Lerp(0.4f, 1f, t));
		float num = Mathf.Max(0.5f, ctx.LaneWidth);
		int num2 = bb.GridColumn - 2;
		float desiredLateralOffset = (float)num2 * num * 0.9f;
		return new DriveCommand
		{
			Throttle01 = throttle,
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

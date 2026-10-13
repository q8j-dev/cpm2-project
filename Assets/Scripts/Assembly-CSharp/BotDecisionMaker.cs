using System.Collections.Generic;
using UnityEngine;

public class BotDecisionMaker
{
	private readonly CircuitCarStearing _owner;

	private readonly List<IDrivingBehavior> _behaviors = new List<IDrivingBehavior>();

	private IDrivingBehavior _lastWinner;

	private const float IncumbentBonus = 0.1f;

	private const float IncumbentMinDuration = 0.25f;

	public string CurrentBehaviorName { get; private set; } = "";

	public BotDecisionMaker(CircuitCarStearing owner)
	{
		_owner = owner;
	}

	public void RegisterBehavior(IDrivingBehavior behavior)
	{
		if (behavior != null)
		{
			_behaviors.Add(behavior);
		}
	}

	public DriveCommand Tick(in BotContext ctx, in PerceptionSnapshot snap, in RouteTarget route, in SpeedTarget speed, DriveBlackboard bb)
	{
		IDrivingBehavior drivingBehavior = null;
		float num = float.NegativeInfinity;
		for (int i = 0; i < _behaviors.Count; i++)
		{
			float num2 = _behaviors[i].Score(in ctx, in snap, in route, in speed, bb);
			if (_lastWinner == _behaviors[i] && bb.IncumbentSince + 0.25f > Time.time)
			{
				num2 += 0.1f;
			}
			if (num2 > num)
			{
				num = num2;
				drivingBehavior = _behaviors[i];
			}
		}
		if (drivingBehavior == null)
		{
			CurrentBehaviorName = "None";
			return new DriveCommand
			{
				Throttle01 = 1f,
				Brake01 = 0f,
				DesiredLateralOffset = bb.DesiredLateralOffset,
				LaneIndexHint = bb.LaneIndex,
				ActiveBehaviorName = "None"
			};
		}
		if (drivingBehavior != _lastWinner)
		{
			bb.IncumbentBehavior = drivingBehavior.Name;
			bb.IncumbentSince = Time.time;
			_lastWinner = drivingBehavior;
		}
		CurrentBehaviorName = drivingBehavior.Name;
		DriveCommand result = drivingBehavior.Produce(in ctx, in snap, in route, in speed, bb);
		result.ActiveBehaviorName = drivingBehavior.Name;
		return result;
	}
}

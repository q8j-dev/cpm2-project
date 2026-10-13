using System;
using CPMEngine;
using UnityEngine;

public class BotRouteFollower
{
	private readonly CircuitCarStearing _owner;

	private BakedTrajectory _activeTrajectory;

	private int _currentWaypointIndex = -1;

	private float _previousSteering;

	private Vector3 _lerpedTarget;

	private Vector3 _targetPosition;

	private float _followOffset;

	private float _travelSinceSwitch;

	private const float YawDampGain = 0.12f;

	public int CurrentWaypointIndex => _currentWaypointIndex;

	public float FollowOffset => _followOffset;

	public Vector3 LerpedTarget => _lerpedTarget;

	public BakedTrajectory ActiveTrajectory => _activeTrajectory;

	public BotRouteFollower(CircuitCarStearing owner)
	{
		_owner = owner;
	}

	public void SeedFollowOffset(float offset)
	{
		_followOffset = offset;
	}

	public void SetActiveTrajectory(BakedTrajectory traj)
	{
		if (_activeTrajectory != traj)
		{
			_activeTrajectory = traj;
			_currentWaypointIndex = -1;
			_travelSinceSwitch = 0f;
		}
	}

	public void ResetToClosest(in BotContext ctx)
	{
		if (_activeTrajectory == null || _activeTrajectory.Count == 0)
		{
			_currentWaypointIndex = -1;
			return;
		}
		int num = FindClosestWaypoint(in ctx);
		if (num >= 0)
		{
			float meters = ((ctx.OuterState == CarSteeringState.PreparePitStop) ? 20f : 30f);
			_currentWaypointIndex = AdvanceByDistance(num, meters, ctx.Direction);
			_lerpedTarget = GetWP(_currentWaypointIndex).Position;
			_travelSinceSwitch = 0f;
		}
	}

	public RouteTarget Tick(in BotContext ctx, in PerceptionSnapshot snap, DriveBlackboard bb)
	{
		if (_activeTrajectory == null || _activeTrajectory.Count == 0)
		{
			return new RouteTarget
			{
				CurrentWaypointIndex = -1
			};
		}
		if (_currentWaypointIndex < 0)
		{
			ResetToClosest(in ctx);
		}
		if (_currentWaypointIndex < 0)
		{
			return new RouteTarget
			{
				CurrentWaypointIndex = -1
			};
		}
		_travelSinceSwitch += ctx.SpeedMps * ctx.Dt;
		ref WaypointData wP = ref GetWP(_currentWaypointIndex);
		Vector3 to = wP.Position - ctx.Position;
		if (Vector3.Angle(ctx.Forward, to) > 90f && ctx.SpeedMps > 2f)
		{
			int num = FindClosestWaypoint(in ctx);
			if (num >= 0)
			{
				_currentWaypointIndex = AdvanceByDistance(num, 10f, ctx.Direction);
				_travelSinceSwitch = 0f;
				wP = ref GetWP(_currentWaypointIndex);
			}
		}
		float num2 = bb.DesiredLateralOffset;
		if (!bb.SuppressApex)
		{
			num2 += CalculateRacingLineOffset(_currentWaypointIndex, in ctx, in snap);
		}
		float num3 = Mathf.Max(0.5f, _owner.LaneWidth);
		float num4 = num3 * (10f + ctx.SpeedMps / 6f) * ctx.Dt;
		_followOffset += Mathf.Clamp(num2 - _followOffset, 0f - num4, num4);
		float num5 = num3 * ((float)_owner.TotalLanes / 2f - 0.5f);
		_followOffset = Mathf.Clamp(_followOffset, 0f - num5, num5);
		float num6 = Mathf.Abs((ctx.Direction > 0) ? wP.ForwardSteeringAngle : wP.ReverseSteeringAngle);
		int wpIndex = _currentWaypointIndex;
		if (num6 > 0.55f)
		{
			int num7 = _activeTrajectory.Advance(_currentWaypointIndex, -1, ctx.Direction);
			if (num7 != _currentWaypointIndex)
			{
				wpIndex = num7;
			}
		}
		_targetPosition = GetCenterWithOffset(wpIndex, _followOffset);
		float num8 = Mathf.Clamp(4f + ctx.SpeedMps * 0.08f, 4f, 8f);
		float num9 = Mathf.Abs((ctx.Direction > 0) ? wP.ForwardSteeringAngle : wP.ReverseSteeringAngle);
		float num10 = Mathf.Lerp(1f, 3f, Mathf.Clamp01((num9 - 0.1f) / 0.4f));
		num8 *= num10;
		_lerpedTarget = Vector3.Lerp(_lerpedTarget, _targetPosition, ctx.Dt * num8);
		Powertrain powertrain = ctx.Powertrain;
		Vector3 vector = ctx.InvRotation * (_lerpedTarget - ctx.Position);
		float num11 = ((powertrain != null) ? Mathf.Cos(powertrain.Caster * ((float)Math.PI / 180f)) : 1f);
		if (num11 < 0.001f)
		{
			num11 = 0.001f;
		}
		float num12 = Mathf.Atan2(vector.x, vector.z) / num11;
		float b = ((powertrain != null) ? powertrain.MaxSteeringAngle : 35f);
		float num13 = Mathf.Clamp(num12 / Mathf.Max(1f, b), -1f, 1f);
		float num14 = Mathf.Abs(num13 - _previousSteering);
		float num15 = Mathf.Lerp(4f, 3.5f, Mathf.Clamp01(ctx.SpeedMps / 60f));
		float num16 = Mathf.Lerp(1f, 3f, Mathf.Clamp01(num14 / 0.5f));
		float maxDelta = num15 * num16 * ctx.Dt;
		num13 = (_previousSteering = Mathf.MoveTowards(_previousSteering, num13, maxDelta));
		float num17 = (_owner.Rigidbody ? _owner.Rigidbody.angularVelocity.y : 0f);
		float num18 = 0.12f * Mathf.Clamp01(ctx.SpeedMps / 30f);
		num13 = Mathf.Clamp(num13 - num18 * num17, -1f, 1f);
		float num19 = AdjustSwitchDistance(ctx.SpeedMps * 3.61f);
		float num20 = _activeTrajectory.SampleSpacing * 3f;
		if (Vector3.Distance(ctx.Position, wP.Position) < num19 && _travelSinceSwitch >= num20)
		{
			int index = AdvanceByDistance(_currentWaypointIndex, 10f, ctx.Direction);
			ref WaypointData wP2 = ref GetWP(index);
			float a = Mathf.Abs((ctx.Direction > 0) ? wP2.ForwardSteeringAngle : wP2.ReverseSteeringAngle);
			float b2 = Mathf.Abs((ctx.Direction > 0) ? wP.ForwardSteeringAngle : wP.ReverseSteeringAngle);
			float num21 = Mathf.Max(a, b2);
			_currentWaypointIndex = AdvanceByDistance(meters: (num21 >= 0.05f) ? Mathf.Clamp(ctx.SpeedMps * 0.3f, 10f, 25f) : ((!(num21 >= 0.02f)) ? Mathf.Clamp(ctx.SpeedMps * 0.4f, 15f, 40f) : Mathf.Clamp(ctx.SpeedMps * 0.3f, 12f, 30f)), index: _currentWaypointIndex, direction: ctx.Direction);
			_travelSinceSwitch = 0f;
		}
		return new RouteTarget
		{
			WorldTarget = _targetPosition,
			LerpedTarget = _lerpedTarget,
			SteeringNormalized = num13,
			LookAheadCurvature = ComputeLookAheadCurvature(_currentWaypointIndex, in ctx),
			DistanceToCurrentWaypoint = Vector3.Distance(ctx.Position, wP.Position),
			CurrentWaypointIndex = _currentWaypointIndex,
			FollowOffset = _followOffset
		};
	}

	private float CalculateRacingLineOffset(int wpIndex, in BotContext ctx, in PerceptionSnapshot snap)
	{
		float num = _owner.ApexFactor * _owner.Personality.ApexAggression;
		if (num <= 0f)
		{
			return 0f;
		}
		if (snap.CarAheadIndex >= 0 && snap.DistanceAhead < 25f)
		{
			return 0f;
		}
		ref WaypointData wP = ref GetWP(wpIndex);
		Vector3 leftBoundary = wP.LeftBoundary;
		Vector3 rightBoundary = wP.RightBoundary;
		float num2 = Vector3.Distance(leftBoundary, rightBoundary) * 0.5f;
		float num3 = ((ctx.Direction > 0) ? wP.ForwardSteeringAngle : wP.ReverseSteeringAngle);
		float num4 = 0f;
		bool flag = false;
		int num5 = wpIndex;
		int num6 = 0;
		int steps = Mathf.Max(1, Mathf.RoundToInt(10f / _activeTrajectory.SampleSpacing));
		int num7 = Mathf.Max(1, _owner.RacingLineLookAhead);
		for (int i = 0; i < num7; i++)
		{
			num5 = Advance(num5, steps, ctx.Direction);
			if (num5 == wpIndex)
			{
				break;
			}
			ref WaypointData wP2 = ref GetWP(num5);
			float num8 = ((ctx.Direction > 0) ? wP2.ForwardSteeringAngle : wP2.ReverseSteeringAngle);
			num4 += num8;
			num6++;
			if (Mathf.Abs(num3) > 0.02f && Mathf.Abs(num8) > 0.02f && Mathf.Sign(num3) != Mathf.Sign(num8))
			{
				flag = true;
			}
		}
		if (flag)
		{
			return 0f;
		}
		float num9 = ((num6 > 0) ? (num4 / (float)num6) : 0f);
		float num10 = num3 * num2 * num;
		float num11 = (0f - num9) * num2 * num * 0.5f;
		float num12 = Mathf.Abs(num3);
		float num13 = Mathf.Abs(num9);
		float num14 = num12 + num13;
		if (num14 < 0.01f)
		{
			return 0f;
		}
		return (num10 * num12 + num11 * num13) / num14;
	}

	private Vector3 GetCenterWithOffset(int wpIndex, float offset)
	{
		ref WaypointData wP = ref GetWP(wpIndex);
		Vector3 leftBoundary = wP.LeftBoundary;
		Vector3 rightBoundary = wP.RightBoundary;
		Vector3 normalized = (rightBoundary - leftBoundary).normalized;
		float num = Vector3.Distance(leftBoundary, rightBoundary) * 0.5f;
		float num2 = Mathf.Clamp(offset, (0f - num) * 0.9f, num * 0.9f);
		return wP.Position + normalized * num2;
	}

	private float ComputeLookAheadCurvature(int wpIndex, in BotContext ctx)
	{
		float num = _activeTrajectory.SampleSpacing;
		if (num < 0.01f)
		{
			num = 2f;
		}
		int num2 = Mathf.Max(5, Mathf.RoundToInt(50f / num));
		float num3 = 0f;
		int num4 = 0;
		int num5 = wpIndex;
		for (int i = 0; i < num2; i++)
		{
			num5 = Advance(num5, 1, ctx.Direction);
			if (num5 == wpIndex)
			{
				break;
			}
			ref WaypointData wP = ref GetWP(num5);
			num3 += Mathf.Abs((ctx.Direction > 0) ? wP.ForwardSteeringAngle : wP.ReverseSteeringAngle);
			num4++;
		}
		if (num4 <= 0)
		{
			return 0f;
		}
		return num3 / (float)num4;
	}

	private int FindClosestWaypoint(in BotContext ctx)
	{
		int count = _activeTrajectory.Count;
		float num = float.PositiveInfinity;
		int result = -1;
		for (int i = 0; i < count; i++)
		{
			ref WaypointData waypoint = ref _activeTrajectory.GetWaypoint(i);
			float sqrMagnitude = (ctx.Position - waypoint.Position).sqrMagnitude;
			if (sqrMagnitude < num)
			{
				num = sqrMagnitude;
				result = i;
			}
			if (sqrMagnitude < 100f)
			{
				break;
			}
		}
		return result;
	}

	private float AdjustSwitchDistance(float speedKm)
	{
		return Mathf.Lerp(_owner.BaseSwitchDistance, 25f * Mathf.Clamp(_owner.NextTargetCoefficient * (speedKm / 100f), 1f, 3f), speedKm / 150f);
	}

	private ref WaypointData GetWP(int index)
	{
		return ref _activeTrajectory.GetWaypoint(index);
	}

	private int Advance(int index, int steps, int direction)
	{
		return _activeTrajectory.Advance(index, steps, direction);
	}

	private int AdvanceByDistance(int index, float meters, int direction)
	{
		float num = _activeTrajectory.SampleSpacing;
		if (num < 0.01f)
		{
			num = 2f;
		}
		int steps = Mathf.Max(1, Mathf.RoundToInt(meters / num));
		return Advance(index, steps, direction);
	}
}

using UnityEngine;

public class BotDriveStateMachine
{
	public enum DriveState
	{
		Normal = 0,
		Slowing = 1,
		PreparingOvertake = 2,
		Overtaking = 3,
		Returning = 4,
		Defending = 5
	}

	private readonly CircuitCarStearing _owner;

	private DriveState _state;

	private float _carAheadPrevSpeedMps;

	private float _carAheadDeceleration;

	private float _physicsCheckTimer;

	private float _sideAvoidanceOffset;

	private float _baseLaneOffset;

	private float _targetFollowOffset;

	private float _followOffset;

	private float _laneChangeGraceTimer;

	private float _stuckTimer;

	private float _reversingTime;

	private Vector3 _reversePosStart;

	private CircuitCarStearing _carAheadPersistent;

	private float _carFoundTime;

	private float _defenseSide;

	private float _defenseSwitchCooldown;

	private float _brakeCheckTimer;

	private bool _sabotageActive;

	private string _sabotageMode = "";

	private float _stuckBehindTimer;

	private float _yieldBias;

	private readonly float RamRange;

	private readonly float BrakeCheckRange;

	private readonly float BrakeCheckPulse;

	private readonly float BrakeCheckGap;

	private readonly float PushLateralRange;

	private readonly float PushForwardLen;

	public float MinSpeedThreshold;

	public float MinOvertakeDistance;

	public float MinCheckInterval;

	public float MaxCheckInterval;

	public float BrakeSpeedInPercent;

	public float AppliedBrakeMultiply;

	public float HighSpeedSwerveCutoffKmh;

	public float HighSpeedOvertakeGateKmh;

	public float RearEndSafetyTtc;

	public float RearEndSafetyGap;

	public float RearEndPathHalfWidth;

	public float RearEndReactionSec;

	public float RearEndBrakeFrac;

	public float RearEndSwerveStrength;

	private float _emergencySwerveBias;

	public DriveState CurrentState => _state;

	public string CurrentBehaviorName => _state.ToString();

	public float FollowOffset => _followOffset;

	public float CarAheadDeceleration => _carAheadDeceleration;

	public BotDriveStateMachine(CircuitCarStearing owner)
	{
		_owner = owner;
		_followOffset = ((owner != null) ? owner.FollowOffset : 0f);
		_baseLaneOffset = _followOffset;
		_targetFollowOffset = _followOffset;
		BotTuningSO botTuningSO = owner.CircuitCarsControl.BotTuningSO;
		RamRange = botTuningSO.RamRange;
		BrakeCheckRange = botTuningSO.BrakeCheckRange;
		BrakeCheckPulse = botTuningSO.BrakeCheckPulse;
		BrakeCheckGap = botTuningSO.BrakeCheckGap;
		PushLateralRange = botTuningSO.PushLateralRange;
		PushForwardLen = botTuningSO.PushForwardLen;
		MinSpeedThreshold = botTuningSO.MinSpeedThreshold;
		MinOvertakeDistance = botTuningSO.MinOvertakeDistance;
		MinCheckInterval = botTuningSO.MinCheckInterval;
		MaxCheckInterval = botTuningSO.MaxCheckInterval;
		BrakeSpeedInPercent = botTuningSO.BrakeSpeedInPercent;
		AppliedBrakeMultiply = botTuningSO.AppliedBrakeMultiply;
		HighSpeedSwerveCutoffKmh = botTuningSO.HighSpeedSwerveCutoffKmh;
		HighSpeedOvertakeGateKmh = botTuningSO.HighSpeedOvertakeGateKmh;
		RearEndSafetyTtc = botTuningSO.RearEndSafetyTtc;
		RearEndSafetyGap = botTuningSO.RearEndSafetyGap;
		RearEndPathHalfWidth = botTuningSO.RearEndPathHalfWidth;
		RearEndReactionSec = botTuningSO.RearEndReactionSec;
		RearEndBrakeFrac = botTuningSO.RearEndBrakeFrac;
		RearEndSwerveStrength = botTuningSO.RearEndSwerveStrength;
	}

	public void SeedLane(float offset)
	{
		_followOffset = offset;
		_baseLaneOffset = offset;
		_targetFollowOffset = offset;
	}

	public void ResetState()
	{
		_state = DriveState.Normal;
		_carAheadPersistent = null;
		_carAheadPrevSpeedMps = 0f;
		_carAheadDeceleration = 0f;
		_physicsCheckTimer = 0f;
		_sideAvoidanceOffset = 0f;
		_emergencySwerveBias = 0f;
		_stuckTimer = 0f;
		_reversingTime = 0f;
		_reversePosStart = Vector3.zero;
		_laneChangeGraceTimer = 0f;
		_followOffset = 0f;
		_baseLaneOffset = 0f;
		_targetFollowOffset = 0f;
		_defenseSide = 0f;
		_defenseSwitchCooldown = 0f;
		_brakeCheckTimer = 0f;
		_sabotageActive = false;
		_sabotageMode = "";
		_stuckBehindTimer = 0f;
		_yieldBias = 0f;
	}

	public DriveCommand Tick(in BotContext ctx, in PerceptionSnapshot snap, in RouteTarget route, in SpeedTarget speed, DriveBlackboard bb)
	{
		if (_laneChangeGraceTimer > 0f)
		{
			_laneChangeGraceTimer -= ctx.Dt;
		}
		if (_defenseSwitchCooldown > 0f)
		{
			_defenseSwitchCooldown -= ctx.Dt;
		}
		_physicsCheckTimer += ctx.Dt;
		float num = Mathf.Max(0.5f, ctx.LaneWidth);
		_baseLaneOffset = (float)bb.LaneIndex * num;
		BotPersonality botPersonality = ((_owner != null) ? _owner.Personality : BotPersonality.Neutral);
		float num2 = botPersonality.Aggression;
		if (snap.CarAheadIsPlayer)
		{
			num2 = Mathf.Clamp01(num2 + botPersonality.PlayerAggression);
		}
		float num3 = num2 - 0.3f;
		float num4 = Mathf.Clamp(1f - num3 * 0.7f, 0.6f, 1.25f);
		float num5 = Mathf.Clamp(1.5f - num3 * 1.6f, 0.4f, 2.5f);
		float highSpeedOvertakeGateKmh = botPersonality.HighSpeedOvertakeGateKmh;
		float num6 = Mathf.Clamp((botPersonality.CornerConfidence - 1f) * 2f + 1f + num3 * 0.4f, 0.6f, 1.12f);
		float num7 = MinSpeedThreshold * num6;
		float num8 = 0f;
		if (snap.CarAheadIndex >= 0)
		{
			ref NearbyCar reference = ref snap.Cars[snap.CarAheadIndex];
			Vector3 velocity = reference.Velocity;
			num8 = velocity.magnitude;
			float num9 = Mathf.Max(0.001f, ctx.Dt);
			_carAheadDeceleration = Mathf.Max(0f, (_carAheadPrevSpeedMps - num8) / num9);
			_carAheadPrevSpeedMps = num8;
			_carAheadPersistent = reference.Source;
			_carFoundTime = Time.time;
		}
		else
		{
			float num10 = Mathf.Lerp(1f, 0.5f, Mathf.Clamp01(ctx.SpeedMps / 40f));
			if (Time.time - _carFoundTime > num10)
			{
				_carAheadPersistent = null;
			}
			_carAheadDeceleration = 0f;
			_carAheadPrevSpeedMps = 0f;
		}
		if (_carAheadPersistent != null && (_state == DriveState.Normal || _state == DriveState.Slowing) && ctx.SpeedMps > num8 - 0.5f && snap.DistanceAhead < Mathf.Max(25f, ctx.SpeedMps * 1.2f))
		{
			_stuckBehindTimer += ctx.Dt;
		}
		else
		{
			_stuckBehindTimer = Mathf.MoveTowards(_stuckBehindTimer, 0f, ctx.Dt * 3f);
		}
		float num11 = Mathf.Clamp01((_stuckBehindTimer - 3f) / 8f);
		num5 *= 1f - 0.6f * num11;
		bool flag = false;
		float t = Mathf.Clamp01(ctx.SpeedMps / 50f);
		float num12 = Mathf.Lerp(MaxCheckInterval, MinCheckInterval, t);
		if (_physicsCheckTimer >= num12)
		{
			_physicsCheckTimer = 0f;
			flag = UpdateSideAvoidance(in ctx, in snap, num);
		}
		bool flag2 = false;
		bool num13 = _carAheadPersistent != null && _laneChangeGraceTimer <= 0f;
		float num14 = Mathf.Max(4f, ((_owner != null) ? _owner.ZLenght : 2f) * 2f) * 1.2f;
		bool flag3 = snap.DistanceAhead > num14;
		bool flag4 = snap.CarAheadIsPlayer && botPersonality.PlayerSabotage > 0f;
		bool flag5 = _state == DriveState.Slowing || _state == DriveState.Normal;
		if (((num13 && bb.LaneChangeCooldown <= 0f && CanChangeLane(in ctx, in route)) & flag5) && !flag4)
		{
			float num15 = Mathf.Max(0f, ctx.SpeedMps - num8);
			float num16 = Mathf.Max(8f, ctx.SpeedMps * 0.35f + num15 * 0.5f) * num4;
			float num17 = Mathf.Max(20f, ctx.SpeedMps * 0.7f + num15 * 0.8f) * num4;
			bool flag6 = ctx.SpeedMps > num8 + num5;
			bool flag7 = Mathf.Abs(route.SteeringNormalized) * 35f < 50f;
			if ((snap.DistanceAhead < num17 && snap.DistanceAhead > num14) & flag6 & flag7)
			{
				float clearNeeded = Mathf.Clamp(ctx.SpeedMps * Mathf.Lerp(1.4f, 0.7f, num11), 8f, 90f);
				int num18 = FindBestLane(bb.LaneIndex, in snap, clearNeeded);
				float num19 = ((ctx.SpeedKmh > highSpeedOvertakeGateKmh) ? (ctx.SpeedMps * Mathf.Lerp(2.2f, 1.3f, num11)) : 0f);
				bool flag8 = num19 <= 0f || snap.Lanes.NearestAheadDist(num18) >= num19;
				if ((num18 != bb.LaneIndex && snap.Lanes.IsLaneSafeBehind(num18)) & flag8)
				{
					bb.OvertakenCar = ((_carAheadPersistent != null) ? _carAheadPersistent.gameObject : null);
					bb.LaneIndex = num18;
					_baseLaneOffset = (float)bb.LaneIndex * num;
					_targetFollowOffset = _baseLaneOffset;
					bb.LaneChangeCooldown = ((snap.DistanceAhead < num16) ? 0.5f : 1f);
					_stuckBehindTimer = 0f;
					_state = DriveState.PreparingOvertake;
					BotLog.Lane(ctx.OwnerName, $"PREP-OVERTAKE → lane {num18} d={snap.DistanceAhead:F0}m ΔV={(ctx.SpeedMps - num8) * 3.6f:F0}km/h");
				}
			}
		}
		if (_carAheadPersistent != null && _state != DriveState.PreparingOvertake && _state != DriveState.Overtaking)
		{
			float num20 = Mathf.Max(0f, ctx.SpeedMps - num8);
			float num21 = Mathf.Max(4f, ctx.SpeedMps * 0.15f + num20 * 0.3f);
			if (snap.DistanceAhead < num21 || snap.TtcAhead < 1.4f)
			{
				bool flag9 = ctx.SpeedKmh >= HighSpeedSwerveCutoffKmh;
				float num22 = ctx.SpeedMps * 2f;
				int num23 = bb.LaneIndex - 1;
				int num24 = bb.LaneIndex + 1;
				bool flag10 = flag3 && num23 >= -2 && snap.Lanes.IsLaneFreeAhead(num23) && (!flag9 || snap.Lanes.NearestAheadDist(num23) >= num22);
				bool flag11 = flag3 && num24 <= 2 && snap.Lanes.IsLaneFreeAhead(num24) && (!flag9 || snap.Lanes.NearestAheadDist(num24) >= num22);
				if (flag10)
				{
					bb.LaneIndex = num23;
					_baseLaneOffset = (float)bb.LaneIndex * num;
					bb.LaneChangeCooldown = 0.3f;
					flag2 = true;
				}
				else if (flag11)
				{
					bb.LaneIndex = num24;
					_baseLaneOffset = (float)bb.LaneIndex * num;
					bb.LaneChangeCooldown = 0.3f;
					flag2 = true;
				}
			}
		}
		if (flag2)
		{
			_carAheadPersistent = null;
			_state = DriveState.Normal;
			_laneChangeGraceTimer = Mathf.Lerp(0.6f, 0.3f, Mathf.Clamp01(ctx.SpeedMps / 50f));
			BotLog.Lane(ctx.OwnerName, $"EMERGENCY LANE → {bb.LaneIndex}");
		}
		bool flag12 = Mathf.Abs(route.SteeringNormalized) * 35f > 30f;
		float num25 = Mathf.Clamp(ctx.SpeedMps * 0.8f, 8f, 30f);
		bool num26 = botPersonality.PlayerDefense > 0.35f && snap.CarBehindIndex >= 0 && snap.DistanceBehind < num25 && snap.ClosingSpeedBehind > 0f;
		bool flag13 = _state == DriveState.PreparingOvertake || _state == DriveState.Overtaking;
		if (num26 && !flag13 && !flag12)
		{
			_state = DriveState.Defending;
		}
		else if (_state == DriveState.Defending && ((snap.CarBehindIndex < 0 || snap.DistanceBehind > num25 * 1.5f) | flag12))
		{
			_state = DriveState.Normal;
			_defenseSide = 0f;
		}
		bool num27 = botPersonality.Aggression <= 0.25f && _state != DriveState.Defending && _state != DriveState.PreparingOvertake && _state != DriveState.Overtaking && snap.CarBehindIndex >= 0 && snap.ClosingSpeedBehind > 3f && snap.DistanceBehind < Mathf.Max(20f, ctx.SpeedMps * 0.8f);
		float target = 0f;
		if (num27)
		{
			float behindSideDist = snap.BehindSideDist;
			target = ((behindSideDist > 0.3f) ? (-1f) : ((behindSideDist < -0.3f) ? 1f : ((_followOffset >= 0f) ? 1f : (-1f)))) * num * 0.7f;
		}
		_yieldBias = Mathf.MoveTowards(_yieldBias, target, num * 2f * ctx.Dt);
		bool decelAwareness = false;
		if (_carAheadPersistent != null)
		{
			float num28 = Mathf.Max(0f, ctx.SpeedMps - num8);
			float num29 = Mathf.Max(4f, ctx.SpeedMps * 0.15f + num28 * 0.3f);
			if (snap.DistanceAhead < num29 || snap.TtcAhead < 1.6f || (snap.AheadDecelMps2 > 6f && snap.DistanceAhead < ctx.SpeedMps * 1.2f))
			{
				if (_state != DriveState.PreparingOvertake && _state != DriveState.Overtaking)
				{
					_state = DriveState.Slowing;
				}
			}
			else if (_laneChangeGraceTimer <= 0f && _state != DriveState.PreparingOvertake && _state != DriveState.Overtaking && _state != DriveState.Defending)
			{
				float num30 = ((ctx.SpeedKmh > 120f) ? (ctx.SpeedMps * 0.55f) : 0f);
				float num31 = Mathf.Max(Mathf.Max(8f, ctx.SpeedMps * 0.35f + num28 * 0.5f) * num4, num30);
				float num32 = Mathf.Max(Mathf.Max(15f, ctx.SpeedMps * 0.7f + num28 * 0.8f) * num4, num30 * 1.6f);
				if (snap.DistanceAhead < num31)
				{
					_state = DriveState.Slowing;
				}
				else if (snap.DistanceAhead < num32 && num28 > 2f)
				{
					decelAwareness = true;
				}
				else if (_state == DriveState.Slowing)
				{
					_state = DriveState.Normal;
				}
			}
		}
		else if (_state == DriveState.Slowing && _laneChangeGraceTimer <= 0f)
		{
			_state = DriveState.Normal;
		}
		bool flag14 = UpdateStuckEscape(in ctx, in snap, bb, num);
		float maxSpeedKmh = speed.MaxSpeedKmh;
		bool belowMinFloor = ctx.SpeedKmh < num7 && ctx.OuterState == CarSteeringState.Race;
		ComputeCruiseControl(ctx.SpeedKmh, maxSpeedKmh, route.SteeringNormalized, belowMinFloor, out var throttle, out var brake);
		ApplyStateOverride(in ctx, in snap, in speed, num8, decelAwareness, ref throttle, ref brake);
		bool flag15 = true;
		float num33 = float.MaxValue;
		for (int i = 0; i < snap.Count; i++)
		{
			float forwardDist = snap.Cars[i].ForwardDist;
			if (forwardDist > 3f)
			{
				flag15 = false;
				break;
			}
			if (forwardDist < 0f)
			{
				num33 = Mathf.Min(num33, 0f - forwardDist);
			}
		}
		if (flag15 && ctx.OuterState == CarSteeringState.Race)
		{
			float b = ((num33 > 50f) ? 0.55f : ((num33 > 25f) ? 0.8f : 1f));
			throttle = Mathf.Min(throttle, b);
		}
		if (flag)
		{
			brake = Mathf.Max(brake, 0.5f);
			throttle = Mathf.Min(throttle, 0.3f);
		}
		if (flag14)
		{
			throttle = 0f;
			brake = 1f;
		}
		if (!flag14)
		{
			float num34 = ((_owner != null) ? _owner.ZLenght : 2f);
			float num35 = Mathf.Max(4f, ctx.BrakeAccelMps2) * Mathf.Clamp(RearEndBrakeFrac, 0.4f, 1f);
			float num36 = 0f;
			int num37 = -1;
			for (int j = 0; j < snap.Count; j++)
			{
				float forwardDist2 = snap.Cars[j].ForwardDist;
				if (forwardDist2 <= 0f || Mathf.Abs(snap.Cars[j].SideDist) > RearEndPathHalfWidth)
				{
					continue;
				}
				float num38 = Mathf.Max(0f, forwardDist2 - num34);
				if (num38 < RearEndSafetyGap)
				{
					num36 = 1f;
					num37 = j;
					break;
				}
				float closingSpeed = snap.Cars[j].ClosingSpeed;
				if (closingSpeed > 0.5f)
				{
					float num39 = closingSpeed * closingSpeed / (2f * num35) + closingSpeed * RearEndReactionSec + RearEndSafetyGap;
					if (num38 < num39)
					{
						num36 = 1f;
						num37 = j;
						break;
					}
					float num40 = num39 * 1.6f;
					if (num38 < num40)
					{
						float num41 = Mathf.Clamp01(1f - (num38 - num39) / (num40 - num39));
						if (num41 > num36)
						{
							num36 = num41;
						}
					}
				}
				float ttc = snap.Cars[j].Ttc;
				if (ttc < RearEndSafetyTtc)
				{
					float num42 = RearEndSafetyTtc * 0.5f;
					float num43 = Mathf.Clamp01(1f - (ttc - num42) / num42);
					if (num43 > num36)
					{
						num36 = num43;
					}
				}
			}
			float num44 = ((num37 >= 0 && RearEndSwerveStrength > 0.01f) ? ComputeEmergencySwerve(in ctx, in snap, num37, num, num34) : 0f);
			float num45 = num * ((num44 != 0f) ? 6f : 2.5f);
			_emergencySwerveBias = Mathf.MoveTowards(_emergencySwerveBias, num44, num45 * ctx.Dt);
			if (num36 > 0f)
			{
				brake = Mathf.Max(brake, num36);
				throttle = Mathf.Min(throttle, 1f - num36);
			}
		}
		else
		{
			_emergencySwerveBias = Mathf.MoveTowards(_emergencySwerveBias, 0f, num * 2.5f * ctx.Dt);
		}
		if (brake > 0.1f)
		{
			throttle = 0f;
		}
		throttle = Mathf.Clamp01(throttle);
		brake = Mathf.Clamp01(brake);
		float num46 = num * (10f + ctx.SpeedMps / 6f) * ctx.Dt;
		float num47 = _targetFollowOffset + _sideAvoidanceOffset + _yieldBias + _emergencySwerveBias;
		_followOffset += Mathf.Clamp(num47 - _followOffset, 0f - num46, num46);
		float num48 = num * ((float)ctx.TotalLanes / 2f - 0.5f);
		_followOffset = Mathf.Clamp(_followOffset, 0f - num48, num48);
		return new DriveCommand
		{
			Throttle01 = throttle,
			Brake01 = brake,
			DesiredLateralOffset = _followOffset,
			LaneIndexHint = bb.LaneIndex,
			SuppressApex = (_state == DriveState.PreparingOvertake || _state == DriveState.Overtaking || _state == DriveState.Defending || _sabotageActive || Mathf.Abs(_emergencySwerveBias) > 0.3f),
			RequestReverse = flag14,
			ActiveBehaviorName = (_sabotageActive ? (_state.ToString() + "+" + _sabotageMode) : _state.ToString())
		};
	}

	private void ComputeCruiseControl(float curKmh, float targetKmh, float steerNorm, bool belowMinFloor, out float throttle, out float brake)
	{
		float num = curKmh - targetKmh;
		if (num > 0f)
		{
			throttle = 0f;
			brake = Mathf.Clamp01(num / 20f);
		}
		else if ((curKmh < targetKmh) | belowMinFloor)
		{
			throttle = 1f;
			brake = 0f;
		}
		else if (curKmh > targetKmh + targetKmh * BrakeSpeedInPercent)
		{
			throttle = 0f;
			brake = (1f - Mathf.Abs(steerNorm)) * AppliedBrakeMultiply;
		}
		else
		{
			throttle = 0f;
			brake = 0f;
		}
	}

	private void ApplyStateOverride(in BotContext ctx, in PerceptionSnapshot snap, in SpeedTarget speed, float carAheadSpd, bool decelAwareness, ref float throttle, ref float brake)
	{
		switch (_state)
		{
		case DriveState.Normal:
			_targetFollowOffset = _baseLaneOffset;
			if (decelAwareness)
			{
				float num6 = Mathf.Clamp01(Mathf.Max(0f, ctx.SpeedMps - carAheadSpd) / 20f) * 0.3f;
				brake = Mathf.Max(brake, num6);
				throttle = Mathf.Min(throttle, 1f - num6 * 2f);
			}
			break;
		case DriveState.Slowing:
			ProduceSlowing(in ctx, in snap, carAheadSpd, ref throttle, ref brake);
			break;
		case DriveState.PreparingOvertake:
			brake = Mathf.Lerp(brake, 0f, ctx.Dt * 3f);
			throttle = Mathf.Lerp(throttle, 1f, ctx.Dt * 1.5f);
			GateThrottleWhileBehindTarget(in ctx, ref throttle, ref brake);
			if (Mathf.Abs(_followOffset - _baseLaneOffset) < ctx.LaneWidth * 0.4f)
			{
				_state = DriveState.Overtaking;
				_laneChangeGraceTimer = 0.3f;
			}
			else if (_carAheadPersistent == null)
			{
				_state = DriveState.Returning;
			}
			break;
		case DriveState.Overtaking:
		{
			throttle = Mathf.Lerp(throttle, 1f, ctx.Dt * 2f);
			brake = Mathf.Lerp(brake, 0f, ctx.Dt * 3f);
			GateThrottleWhileBehindTarget(in ctx, ref throttle, ref brake);
			if (_carAheadPersistent == null)
			{
				_state = DriveState.Returning;
				break;
			}
			Vector3 lhs = _carAheadPersistent.transform.position - ctx.Position;
			float num4 = Vector3.Dot(lhs, ctx.Forward);
			float num5 = Mathf.Lerp(3.2f, 5.5f, (_owner != null) ? _owner.Personality.Aggression : 0.3f);
			if (num4 < -0.5f || lhs.magnitude > MinOvertakeDistance * num5)
			{
				_state = DriveState.Returning;
			}
			break;
		}
		case DriveState.Returning:
			brake = Mathf.Lerp(brake, 0f, ctx.Dt * 2f);
			throttle = Mathf.Lerp(throttle, 1f, ctx.Dt * 1.2f);
			_targetFollowOffset = _baseLaneOffset;
			if (Mathf.Abs(_followOffset - _baseLaneOffset) < 0.2f)
			{
				_state = DriveState.Normal;
				_followOffset = _baseLaneOffset;
			}
			break;
		case DriveState.Defending:
		{
			BotPersonality botPersonality = ((_owner != null) ? _owner.Personality : BotPersonality.Neutral);
			float num = Mathf.Max(0.5f, ctx.LaneWidth);
			float num2 = 0f;
			if (snap.BehindSideDist > 0.5f)
			{
				num2 = 1f;
			}
			else if (snap.BehindSideDist < -0.5f)
			{
				num2 = -1f;
			}
			if (num2 != 0f && num2 != _defenseSide && _defenseSwitchCooldown <= 0f)
			{
				_defenseSide = num2;
				_defenseSwitchCooldown = 0.8f;
			}
			float num3 = num * 0.6f * botPersonality.PlayerDefense;
			_targetFollowOffset = _baseLaneOffset + _defenseSide * num3;
			break;
		}
		}
		ApplyPlayerSabotage(in ctx, in snap, ref throttle, ref brake);
	}

	private void ApplyPlayerSabotage(in BotContext ctx, in PerceptionSnapshot snap, ref float throttle, ref float brake)
	{
		BotPersonality botPersonality = ((_owner != null) ? _owner.Personality : BotPersonality.Neutral);
		_sabotageActive = false;
		_sabotageMode = "";
		if (botPersonality.PlayerSabotage <= 0f)
		{
			return;
		}
		int num = -1;
		for (int i = 0; i < snap.Count; i++)
		{
			if (snap.Cars[i].IsPlayer)
			{
				num = i;
				break;
			}
		}
		if (num < 0)
		{
			_brakeCheckTimer = 0f;
			return;
		}
		float forwardDist = snap.Cars[num].ForwardDist;
		float sideDist = snap.Cars[num].SideDist;
		float num2 = Mathf.Max(0.5f, ctx.LaneWidth);
		float num3 = num2 * ((float)ctx.TotalLanes / 2f - 0.5f);
		float num4 = Mathf.Max(2f, ((_owner != null) ? _owner.ZLenght : 2f) * 1.5f);
		if (Mathf.Abs(forwardDist) <= num4 * PushForwardLen && Mathf.Abs(sideDist) < num2 * PushLateralRange)
		{
			float num5 = ((sideDist >= 0f) ? 1f : (-1f));
			_targetFollowOffset = Mathf.Clamp(_followOffset + num5 * num2 * (0.5f + botPersonality.PlayerSabotage), 0f - num3, num3);
			throttle = Mathf.Max(throttle, 0.9f);
			brake = 0f;
			_brakeCheckTimer = 0f;
			_sabotageActive = true;
			_sabotageMode = "PUSH";
		}
		else if (forwardDist > 0f && forwardDist < RamRange && Mathf.Abs(sideDist) < num2 * 2.5f)
		{
			_targetFollowOffset = Mathf.Clamp(_followOffset + sideDist, 0f - num3, num3);
			throttle = ((forwardDist < num4 * 0.6f) ? 0.2f : 1f);
			brake = 0f;
			_brakeCheckTimer = 0f;
			_sabotageActive = true;
			_sabotageMode = "CUT";
		}
		else if (forwardDist < 0f && 0f - forwardDist < BrakeCheckRange && Mathf.Abs(sideDist) < num2 * 1.5f)
		{
			_brakeCheckTimer += ctx.Dt;
			float num6 = BrakeCheckPulse + BrakeCheckGap;
			if (_brakeCheckTimer > num6)
			{
				_brakeCheckTimer -= num6;
			}
			if (_brakeCheckTimer < BrakeCheckPulse)
			{
				brake = Mathf.Max(brake, 0.85f * botPersonality.PlayerSabotage);
				throttle = 0f;
				_sabotageActive = true;
				_sabotageMode = "BRAKECHECK";
			}
		}
		else
		{
			_brakeCheckTimer = 0f;
		}
	}

	private void GateThrottleWhileBehindTarget(in BotContext ctx, ref float throttle, ref float brake)
	{
		if (_carAheadPersistent == null)
		{
			return;
		}
		Vector3 vector = ctx.InvRotation * (_carAheadPersistent.transform.position - ctx.Position);
		float num = Mathf.Max(4f, (_owner != null) ? (_owner.ZLenght * 2f) : 4f);
		float num2 = 1.9f;
		if (!(vector.z > 0f) || !(vector.z < num * 2f) || !(Mathf.Abs(vector.x) < num2))
		{
			return;
		}
		Rigidbody rigidbody = _carAheadPersistent.Rigidbody;
		float num3 = (rigidbody ? rigidbody.velocity.magnitude : ctx.SpeedMps);
		float num4 = ctx.SpeedMps - num3;
		if (num4 > 0.5f)
		{
			throttle = 0f;
			if (vector.z < num && num4 > 2f)
			{
				brake = Mathf.Max(brake, Mathf.Clamp01(num4 / 10f));
			}
		}
		else
		{
			throttle = Mathf.Min(throttle, 0.6f);
		}
	}

	private float ComputeEmergencySwerve(in BotContext ctx, in PerceptionSnapshot snap, int threatIdx, float laneWidth, float halfLen)
	{
		if (threatIdx < 0 || threatIdx >= snap.Count)
		{
			return 0f;
		}
		float sideDist = snap.Cars[threatIdx].SideDist;
		float forwardDist = snap.Cars[threatIdx].ForwardDist;
		float num = laneWidth * RearEndSwerveStrength;
		float num2 = laneWidth * ((float)ctx.TotalLanes / 2f - 0.5f);
		int num3 = ((!(sideDist > 0f)) ? 1 : (-1));
		for (int i = 0; i < 2; i++)
		{
			int num4 = ((i == 0) ? num3 : (-num3));
			float num5 = (float)num4 * num;
			if (!(Mathf.Abs(_followOffset + num5) > num2) && !IsSwerveCorridorBlocked(in snap, num4, num5, forwardDist, halfLen))
			{
				return num5;
			}
		}
		return 0f;
	}

	private bool IsSwerveCorridorBlocked(in PerceptionSnapshot snap, int dir, float target, float threatFwd, float halfLen)
	{
		float num = RearEndPathHalfWidth + 0.3f;
		float num2 = Mathf.Max(12f, threatFwd + 8f);
		float num3 = halfLen * 2f + 2f;
		for (int i = 0; i < snap.Count; i++)
		{
			float forwardDist = snap.Cars[i].ForwardDist;
			float sideDist = snap.Cars[i].SideDist;
			if (forwardDist > -6f && forwardDist < num2 && Mathf.Abs(sideDist - target) < num)
			{
				return true;
			}
			if (forwardDist > -6f && forwardDist < num3 && sideDist * (float)dir > 0.5f && sideDist * (float)dir < Mathf.Abs(target) + 1f)
			{
				return true;
			}
		}
		return false;
	}

	private void ProduceSlowing(in BotContext ctx, in PerceptionSnapshot snap, float carAheadSpd, ref float throttle, ref float brake)
	{
		if (_carAheadPersistent == null)
		{
			throttle = 0.6f;
			brake = 0f;
			return;
		}
		float num = Mathf.Max(0f, ctx.SpeedMps - carAheadSpd);
		float num2 = Mathf.Max(4f, ctx.SpeedMps * 0.15f + num * 0.3f);
		bool num3 = snap.DistanceAhead < num2 || snap.TtcAhead < 1.3f || _carAheadDeceleration > 7f;
		float num4 = 0f;
		if (_carAheadDeceleration > 2f)
		{
			num4 = Mathf.Clamp01(_carAheadDeceleration / 12f);
		}
		if (num3)
		{
			brake = 1f;
			throttle = 0f;
			return;
		}
		float num5 = Mathf.Max(10f, ctx.SpeedMps * ((ctx.SpeedKmh > 120f) ? 0.55f : 0.4f));
		float num6 = Mathf.Clamp01(snap.DistanceAhead / num5);
		float num7 = carAheadSpd * Mathf.Lerp(0.85f, 1f, num6);
		if (ctx.SpeedMps > num7 + 1f)
		{
			float a = Mathf.Clamp01((ctx.SpeedMps - num7) / 15f);
			throttle = Mathf.Lerp(0f, 0.3f, num6);
			brake = Mathf.Max(a, num4);
		}
		else
		{
			brake = num4;
			throttle = ((num4 > 0.1f) ? 0f : Mathf.Clamp01(0.6f + num6 * 0.4f));
		}
	}

	private bool UpdateSideAvoidance(in BotContext ctx, in PerceptionSnapshot snap, float laneWidth)
	{
		if (_laneChangeGraceTimer > 0f)
		{
			return false;
		}
		float num = Mathf.Clamp(2f + ctx.SpeedMps * 0.15f, 2f, 5f);
		float num2 = Mathf.Clamp(2f + ctx.SpeedMps * 0.1f, 2f, 4f);
		float num3 = ((_owner != null) ? _owner.Personality.Aggression : 0.3f);
		float a = Mathf.Lerp(1f, 0.15f, Mathf.Clamp01((num3 - 0.5f) / 0.5f));
		a = Mathf.Lerp(a, 1f, Mathf.Clamp01((ctx.SpeedKmh - 140f) / 40f));
		float num4 = laneWidth * 0.25f * a;
		float num5 = Mathf.Max(0.5f, (_owner != null) ? _owner.ZLenght : 1f);
		bool flag = false;
		bool flag2 = false;
		for (int i = 0; i < snap.Count; i++)
		{
			float forwardDist = snap.Cars[i].ForwardDist;
			float sideDist = snap.Cars[i].SideDist;
			if (forwardDist > 0f - num5 && forwardDist < num2)
			{
				if (sideDist > 0.5f && sideDist < num + 0.5f)
				{
					flag = true;
				}
				if (sideDist < -0.5f && sideDist > 0f - (num + 0.5f))
				{
					flag2 = true;
				}
			}
		}
		if (flag && !flag2)
		{
			_sideAvoidanceOffset -= num4;
		}
		else if (flag2 && !flag)
		{
			_sideAvoidanceOffset += num4;
		}
		else if (!flag && !flag2)
		{
			_sideAvoidanceOffset = Mathf.MoveTowards(_sideAvoidanceOffset, 0f, num4 * 0.3f);
		}
		_sideAvoidanceOffset = Mathf.Clamp(_sideAvoidanceOffset, (0f - laneWidth) * 1.5f, laneWidth * 1.5f);
		return flag & flag2;
	}

	private bool UpdateStuckEscape(in BotContext ctx, in PerceptionSnapshot snap, DriveBlackboard bb, float laneWidth)
	{
		if (ctx.SpeedMps < 1f && (bb.StuckTimer > 0.2f || _carAheadPersistent != null))
		{
			_stuckTimer += ctx.Dt;
			if (_stuckTimer > 0.8f)
			{
				bb.LaneChangeCooldown = 0f;
				int num = bb.LaneIndex - 1;
				int num2 = bb.LaneIndex + 1;
				bool flag = false;
				if (num >= -2 && snap.Lanes.IsLaneFreeAhead(num))
				{
					bb.LaneIndex = num;
					_baseLaneOffset = (float)bb.LaneIndex * laneWidth;
					flag = true;
				}
				else if (num2 <= 2 && snap.Lanes.IsLaneFreeAhead(num2))
				{
					bb.LaneIndex = num2;
					_baseLaneOffset = (float)bb.LaneIndex * laneWidth;
					flag = true;
				}
				if (flag)
				{
					_carAheadPersistent = null;
					_state = DriveState.Normal;
					_stuckTimer = 0f;
					return false;
				}
				if (_stuckTimer > 3f)
				{
					_stuckTimer = 0f;
					return true;
				}
			}
		}
		else
		{
			_stuckTimer = Mathf.MoveTowards(_stuckTimer, 0f, ctx.Dt * 2f);
		}
		return false;
	}

	private bool CanChangeLane(in BotContext ctx, in RouteTarget route)
	{
		if (ctx.SpeedMps < 3f)
		{
			return true;
		}
		float num = ((ctx.Powertrain != null) ? ctx.Powertrain.MaxSteeringAngle : 35f);
		return Mathf.Abs(route.SteeringNormalized) * num < 28f;
	}

	private int FindBestLane(int currentLane, in PerceptionSnapshot snap, float clearNeeded)
	{
		int num = currentLane - 1;
		if (num >= -2 && snap.Lanes.NearestAheadDist(num) >= clearNeeded)
		{
			return num;
		}
		num = currentLane + 1;
		if (num <= 2 && snap.Lanes.NearestAheadDist(num) >= clearNeeded)
		{
			return num;
		}
		num = currentLane - 2;
		if (num >= -2 && snap.Lanes.NearestAheadDist(num) >= clearNeeded)
		{
			return num;
		}
		num = currentLane + 2;
		if (num <= 2 && snap.Lanes.NearestAheadDist(num) >= clearNeeded)
		{
			return num;
		}
		return currentLane;
	}
}

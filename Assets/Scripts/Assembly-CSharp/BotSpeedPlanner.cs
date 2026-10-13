using System;
using System.Collections.Generic;
using CPMEngine;
using UnityEngine;

public class BotSpeedPlanner
{
	private readonly CircuitCarStearing _owner;

	private readonly List<float> _steeringMinSpeed = new List<float>();

	private readonly List<float> _steeringMaxSpeed = new List<float>();

	private readonly List<float> _smoothedRadius = new List<float>();

	private float _minTurnCoefficient = 1f;

	private float _maxTurnCoefficient = 1f;

	private float _maxSpeedKmh = 200f;

	private float _brakeAccelMps2 = 5f;

	private float _smoothedMaxKmh = -1f;

	public float CornerMarginCoef = 0.85f;

	public float BrakeEarlinessCoef = 1f;

	public float TurnSpeedScale = 1f;

	public float GripPerCoef = 1f;

	public float GripMinMps2 = 8f;

	public float GripMaxMps2 = 22f;

	private bool _frozenSpecSet;

	private float _frozenTopSpeedKmh;

	private float _frozenBrakeMps2;

	private float _frozenLateralMps2;

	public IReadOnlyList<float> SteeringMaxSpeed => _steeringMaxSpeed;

	public IReadOnlyList<float> SteeringMinSpeed => _steeringMinSpeed;

	public float MaxSpeedKmh => _maxSpeedKmh;

	public float BrakeAccelMps2 => _brakeAccelMps2;

	public float GripFromCoef(float coef)
	{
		return Mathf.Clamp(coef * GripPerCoef, GripMinMps2, GripMaxMps2);
	}

	public BotSpeedPlanner(CircuitCarStearing owner)
	{
		_owner = owner;
	}

	public void SetCoefficients(float minCoef, float maxCoef)
	{
		_minTurnCoefficient = minCoef;
		_maxTurnCoefficient = maxCoef;
	}

	public void SetFrozenSpec(float topSpeedKmh, float brakeMps2, float lateralMps2)
	{
		_frozenSpecSet = true;
		_frozenTopSpeedKmh = Mathf.Max(1f, topSpeedKmh);
		_frozenBrakeMps2 = Mathf.Max(0.1f, brakeMps2);
		_frozenLateralMps2 = Mathf.Max(0.1f, lateralMps2);
	}

	public void BakeProfile(BakedTrajectory traj, Powertrain pt)
	{
		_steeringMinSpeed.Clear();
		_steeringMaxSpeed.Clear();
		_smoothedRadius.Clear();
		_smoothedMaxKmh = -1f;
		if (pt == null)
		{
			return;
		}
		if (_frozenSpecSet)
		{
			_maxSpeedKmh = _frozenTopSpeedKmh;
			_brakeAccelMps2 = _frozenBrakeMps2;
		}
		else
		{
			_maxSpeedKmh = pt.CalcTopSpeed() * 3.61f;
			_brakeAccelMps2 = Mathf.Max(5f, pt.MaxBrakeAccel / 1000f);
		}
		if (traj == null || traj.Count == 0)
		{
			return;
		}
		int count = traj.Count;
		float[] array = new float[count];
		float[] array2 = new float[count];
		float[] array3 = new float[count];
		for (int i = 0; i < count; i++)
		{
			float num = traj.GetWaypoint(i).TurnRadius;
			if (num <= 0f || float.IsInfinity(num) || float.IsNaN(num))
			{
				num = float.MaxValue;
			}
			array3[i] = num;
			float t = Mathf.InverseLerp(5f, 100f, num);
			float num2 = Mathf.Lerp(1.2f, 3f, t);
			float coef = (_frozenSpecSet ? _minTurnCoefficient : (_minTurnCoefficient * num2));
			float coef2 = (_frozenSpecSet ? _maxTurnCoefficient : (_maxTurnCoefficient * num2));
			array[i] = CalculateTurnSpeed(num, coef, pt);
			array2[i] = CalculateTurnSpeed(num, coef2, pt);
		}
		if (count >= 5)
		{
			float[] array4 = new float[5];
			for (int j = 0; j < count; j++)
			{
				for (int k = -2; k <= 2; k++)
				{
					array4[k + 2] = array[((j + k) % count + count) % count];
				}
				Array.Sort(array4);
				float num3 = array4[2];
				bool flag = array3[j] <= 15f && array[j] < 0.7f * num3;
				_steeringMinSpeed.Add(flag ? array[j] : num3);
				for (int l = -2; l <= 2; l++)
				{
					array4[l + 2] = array2[((j + l) % count + count) % count];
				}
				Array.Sort(array4);
				float num4 = array4[2];
				bool flag2 = array3[j] <= 15f && array2[j] < 0.7f * num4;
				_steeringMaxSpeed.Add(flag2 ? array2[j] : num4);
				for (int m = -2; m <= 2; m++)
				{
					array4[m + 2] = array3[((j + m) % count + count) % count];
				}
				Array.Sort(array4);
				float num5 = array4[2];
				bool flag3 = array3[j] <= 15f && array3[j] < 0.7f * num5;
				_smoothedRadius.Add(flag3 ? array3[j] : num5);
			}
		}
		else
		{
			for (int n = 0; n < count; n++)
			{
				_steeringMinSpeed.Add(array[n]);
				_steeringMaxSpeed.Add(array2[n]);
				_smoothedRadius.Add(array3[n]);
			}
		}
	}

	private SpeedTarget ApplyOutputSmoothing(SpeedTarget t, float dt)
	{
		float maxSpeedKmh = t.MaxSpeedKmh;
		if (_smoothedMaxKmh < 0f)
		{
			_smoothedMaxKmh = maxSpeedKmh;
		}
		float num = ((maxSpeedKmh < _smoothedMaxKmh) ? 150f : 200f);
		_smoothedMaxKmh = Mathf.MoveTowards(_smoothedMaxKmh, maxSpeedKmh, num * dt);
		t.MaxSpeedKmh = _smoothedMaxKmh;
		t.CoastSpeedKmh = _smoothedMaxKmh;
		return t;
	}

	private float CalculateTurnSpeed(float radius, float coef, Powertrain pt)
	{
		float b = (_frozenSpecSet ? GripFromCoef(coef) : (pt.MaxSideAccel / 1000f * coef));
		b = Mathf.Max(0.01f, b);
		if (radius <= 0f || float.IsInfinity(radius) || float.IsNaN(radius))
		{
			radius = float.MaxValue;
		}
		BotPersonality botPersonality = ((_owner != null) ? _owner.Personality : BotPersonality.Neutral);
		float num = Mathf.Clamp(CornerMarginCoef * botPersonality.CornerConfidence * TurnSpeedScale * botPersonality.TurnSpeedScale, 0.45f, 0.92f);
		return Mathf.Clamp(Mathf.Sqrt(b * radius) * num, 2.777778f, 138.88889f) * 3.6f;
	}

	public SpeedTarget Tick(in BotContext ctx, in PerceptionSnapshot snap, in RouteTarget route)
	{
		SpeedTarget t = new SpeedTarget
		{
			MaxSpeedKmh = _maxSpeedKmh,
			CoastSpeedKmh = _maxSpeedKmh,
			Source = SpeedSource.Cruise,
			DistanceToBrakePoint = float.MaxValue
		};
		if (ctx.OuterState == CarSteeringState.GoToPitStop || ctx.OuterState == CarSteeringState.PreparePitStop || ctx.OuterState == CarSteeringState.OnPitStop)
		{
			return ApplyOutputSmoothing(new SpeedTarget
			{
				MaxSpeedKmh = 30f,
				CoastSpeedKmh = 30f,
				Source = SpeedSource.Pit,
				DistanceToBrakePoint = float.MaxValue
			}, ctx.Dt);
		}
		BakedTrajectory bakedTrajectory = (_owner.UsingPitStopPath ? _owner.PitStopTrajectory : _owner.Trajectory);
		if (bakedTrajectory == null || bakedTrajectory.Count == 0 || _steeringMaxSpeed.Count == 0 || route.CurrentWaypointIndex < 0)
		{
			return ApplyOutputSmoothing(t, ctx.Dt);
		}
		float a = Mathf.Clamp(ctx.SpeedMps * ctx.SpeedMps / (2f * _brakeAccelMps2) + 10f, 40f, 400f);
		a = Mathf.Max(a, ctx.SpeedMps * 2f);
		float num = _maxSpeedKmh;
		float num2 = 0f;
		int num3 = route.CurrentWaypointIndex;
		ref WaypointData waypoint = ref bakedTrajectory.GetWaypoint(route.CurrentWaypointIndex);
		float num4 = Vector3.Distance(ctx.Position, waypoint.Position);
		int num5 = route.CurrentWaypointIndex;
		int count = bakedTrajectory.Count;
		float num6 = float.MaxValue;
		float num7 = 0f;
		int num8 = route.CurrentWaypointIndex;
		for (int i = 0; i < 200 && i < count; i++)
		{
			ref WaypointData waypoint2 = ref bakedTrajectory.GetWaypoint(num5);
			if (num5 >= 0 && num5 < _steeringMaxSpeed.Count)
			{
				float num9 = _steeringMaxSpeed[num5];
				if (num9 < num)
				{
					num = num9;
					num2 = num4;
					num3 = num5;
				}
			}
			float num10 = ((num5 >= 0 && num5 < _smoothedRadius.Count) ? _smoothedRadius[num5] : waypoint2.TurnRadius);
			if (num10 > 0.1f && num10 < num6)
			{
				num6 = num10;
				num7 = num4;
				num8 = num5;
			}
			num4 += ((waypoint2.DistanceToNext > 0.01f) ? waypoint2.DistanceToNext : 3f);
			if (num4 >= a)
			{
				break;
			}
			int num11 = bakedTrajectory.Advance(num5, 1, ctx.Direction);
			if (num11 == route.CurrentWaypointIndex)
			{
				break;
			}
			num5 = num11;
		}
		if (num6 < float.MaxValue)
		{
			float num12 = (_frozenSpecSet ? 1f : ((num6 < 12f) ? 2.25f : TurnSpeedScale));
			float num13 = Mathf.Sqrt((_frozenSpecSet ? GripFromCoef(_maxTurnCoefficient) : 9f) * num6) * num12 * 3.6f;
			if (num13 < num)
			{
				num = num13;
				if (num7 < num2 || num2 == 0f)
				{
					num2 = num7;
				}
				num3 = num8;
			}
		}
		float num14 = ((_owner != null) ? _owner.Personality.Consistency : 1f);
		if (num14 < 0.999f)
		{
			float num15 = Mathf.PerlinNoise((float)((_owner != null) ? (_owner.GetInstanceID() & 0x3FF) : 0) * 0.137f, Time.time * 0.25f) * 2f - 1f;
			num *= 1f + num15 * (1f - num14) * 0.25f;
		}
		float a2 = 45f;
		if (num3 >= 0 && num3 < _steeringMinSpeed.Count)
		{
			a2 = Mathf.Max(a2, _steeringMinSpeed[num3]);
		}
		num = Mathf.Max(a2, num);
		if (num >= _maxSpeedKmh)
		{
			return ApplyOutputSmoothing(t, ctx.Dt);
		}
		float num16 = num / 3.6f;
		float b = ((_owner != null) ? _owner.Personality.BrakeEarliness : 1f);
		float num17 = _brakeAccelMps2 * BrakeEarlinessCoef / Mathf.Max(0.01f, b);
		float num18 = Mathf.Sqrt(num16 * num16 + 2f * num17 * Mathf.Max(0f, num2)) * 3.6f;
		float num19 = _maxSpeedKmh;
		if (route.CurrentWaypointIndex < _steeringMaxSpeed.Count)
		{
			num19 = Mathf.Min(num19, _steeringMaxSpeed[route.CurrentWaypointIndex] * 1.1f);
		}
		float num20 = Mathf.Min(num19, num18);
		SpeedSource source = ((num18 < num19) ? SpeedSource.Cornering : SpeedSource.Cruise);
		float brakingDistanceNeeded = Mathf.Max(0f, (ctx.SpeedMps * ctx.SpeedMps - num16 * num16) / (2f * _brakeAccelMps2));
		return ApplyOutputSmoothing(new SpeedTarget
		{
			MaxSpeedKmh = num20,
			CoastSpeedKmh = num20,
			BrakeMagnitude = 0f,
			Source = source,
			BrakingDistanceNeeded = brakingDistanceNeeded,
			DistanceToBrakePoint = num2
		}, ctx.Dt);
	}
}

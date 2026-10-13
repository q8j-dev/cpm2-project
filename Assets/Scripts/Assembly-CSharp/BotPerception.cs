using System.Collections.Generic;
using UnityEngine;

public class BotPerception
{
	private const float DecelEmaAlpha = 0.3f;

	private const float SameDirSqrSpeed = 4f;

	private const float SameDirCosSqr = 0.25f;

	private readonly CircuitCarStearing _owner;

	private readonly CircuitCarsControl _carsControl;

	private readonly Dictionary<int, float> _prevSpeedById = new Dictionary<int, float>();

	private readonly Dictionary<int, float> _decelEmaById = new Dictionary<int, float>();

	private PerceptionSnapshot _snap;

	private CircuitCarStearing _lastAheadSource;

	public BotPerception(CircuitCarStearing owner, CircuitCarsControl carsControl)
	{
		_owner = owner;
		_carsControl = carsControl;
		_snap.Cars = new NearbyCar[10];
		_snap.CarAheadIndex = -1;
	}

	public ref PerceptionSnapshot Tick(in BotContext ctx)
	{
		FillNearbyCars(in ctx);
		ComputeCarAhead(in ctx);
		ComputeCarBehind(in ctx);
		ComputeLaneOccupancy(in ctx);
		return ref _snap;
	}

	private void FillNearbyCars(in BotContext ctx)
	{
		_snap.Count = 0;
		if (_carsControl == null)
		{
			return;
		}
		float num = Mathf.Clamp(ctx.SpeedMps * 4f, 20f, 300f) + 20f;
		float num2 = num * num;
		float num3 = Mathf.Max(0.5f, _owner.LaneWidth);
		Vector3 velocity = _owner.Velocity;
		IReadOnlyList<CircuitCarStearing> circuitCarStearings = _carsControl.CircuitCarStearings;
		for (int i = 0; i < circuitCarStearings.Count; i++)
		{
			if (_snap.Count >= 10)
			{
				break;
			}
			CircuitCarStearing circuitCarStearing = circuitCarStearings[i];
			if (circuitCarStearing == null || circuitCarStearing == _owner)
			{
				continue;
			}
			Rigidbody rigidbody = ((circuitCarStearing.Rigidbody != null) ? circuitCarStearing.Rigidbody : circuitCarStearing.GetComponent<Rigidbody>());
			if (rigidbody == null)
			{
				continue;
			}
			Vector3 position = circuitCarStearing.transform.position;
			float num4 = ctx.Position.x - position.x;
			float num5 = ctx.Position.z - position.z;
			if (!(num4 * num4 + num5 * num5 > num2))
			{
				Vector3 vector = ctx.InvRotation * (position - ctx.Position);
				float z = vector.z;
				float x = vector.x;
				Vector3 velocity2 = rigidbody.velocity;
				float num6 = Vector3.Dot(velocity - velocity2, ctx.Forward);
				float ttc = ((z > 0f && num6 > 0.1f) ? (z / num6) : float.MaxValue);
				bool sameDirection = IsCarOnSameTrackSection(velocity2, ctx.Forward);
				int instanceID = circuitCarStearing.GetInstanceID();
				float magnitude = velocity2.magnitude;
				float b = 0f;
				if (_prevSpeedById.TryGetValue(instanceID, out var value))
				{
					b = Mathf.Max(0f, (value - magnitude) / Mathf.Max(0.001f, ctx.Dt));
				}
				_prevSpeedById[instanceID] = magnitude;
				float a = (_decelEmaById.TryGetValue(instanceID, out var value2) ? value2 : 0f);
				a = Mathf.Lerp(a, b, 0.3f);
				_decelEmaById[instanceID] = a;
				int value3 = Mathf.RoundToInt(x / num3);
				value3 = Mathf.Clamp(value3, -2, 2);
				_snap.Cars[_snap.Count++] = new NearbyCar
				{
					Source = circuitCarStearing,
					Rigidbody = rigidbody,
					WorldPos = position,
					Velocity = velocity2,
					ForwardDist = z,
					SideDist = x,
					ClosingSpeed = num6,
					Ttc = ttc,
					SameDirection = sameDirection,
					TrajectoryLane = value3,
					DecelMps2 = a,
					IsPlayer = !circuitCarStearing.IsRaceBot
				};
			}
		}
	}

	private void ComputeCarAhead(in BotContext ctx)
	{
		int num = -1;
		float num2 = float.MaxValue;
		int num3 = -1;
		float laneWidth = _owner.LaneWidth;
		for (int i = 0; i < _snap.Count; i++)
		{
			ref NearbyCar reference = ref _snap.Cars[i];
			if (reference.SameDirection && !(reference.ForwardDist <= 0f))
			{
				float num4 = laneWidth * 0.6f + Mathf.Clamp(reference.ForwardDist * 0.03f, 0f, laneWidth * 0.35f);
				float num5 = Mathf.Abs(reference.SideDist);
				if (reference.Source == _lastAheadSource && num5 <= num4 * 1.4f)
				{
					num3 = i;
				}
				if (!(num5 > num4) && reference.ForwardDist < num2)
				{
					num2 = reference.ForwardDist;
					num = i;
				}
			}
		}
		if (num3 >= 0 && num3 != num && (num < 0 || _snap.Cars[num].ForwardDist > _snap.Cars[num3].ForwardDist * 0.7f))
		{
			num = num3;
		}
		_lastAheadSource = ((num >= 0) ? _snap.Cars[num].Source : null);
		_snap.CarAheadIndex = num;
		if (num >= 0)
		{
			ref NearbyCar reference2 = ref _snap.Cars[num];
			_snap.DistanceAhead = reference2.ForwardDist;
			_snap.TtcAhead = reference2.Ttc;
			_snap.ClosingSpeed = reference2.ClosingSpeed;
			_snap.AheadSpeedMps = reference2.Velocity.magnitude;
			_snap.AheadDecelMps2 = reference2.DecelMps2;
			_snap.LaneAheadHasBrakingCar = reference2.DecelMps2 > 3f;
			_snap.CarAheadIsPlayer = reference2.IsPlayer;
		}
		else
		{
			_snap.DistanceAhead = float.MaxValue;
			_snap.TtcAhead = float.MaxValue;
			_snap.ClosingSpeed = 0f;
			_snap.AheadSpeedMps = 0f;
			_snap.AheadDecelMps2 = 0f;
			_snap.LaneAheadHasBrakingCar = false;
			_snap.CarAheadIsPlayer = false;
		}
	}

	private void ComputeCarBehind(in BotContext ctx)
	{
		int num = -1;
		float num2 = float.MaxValue;
		float laneWidth = _owner.LaneWidth;
		for (int i = 0; i < _snap.Count; i++)
		{
			ref NearbyCar reference = ref _snap.Cars[i];
			if (!reference.SameDirection || reference.ForwardDist >= 0f)
			{
				continue;
			}
			float num3 = laneWidth * 1.1f;
			if (!(Mathf.Abs(reference.SideDist) > num3))
			{
				float num4 = 0f - reference.ForwardDist;
				if (num4 < num2)
				{
					num2 = num4;
					num = i;
				}
			}
		}
		_snap.CarBehindIndex = num;
		if (num >= 0)
		{
			ref NearbyCar reference2 = ref _snap.Cars[num];
			_snap.DistanceBehind = 0f - reference2.ForwardDist;
			_snap.ClosingSpeedBehind = 0f - reference2.ClosingSpeed;
			_snap.BehindSideDist = reference2.SideDist;
			_snap.CarBehindIsPlayer = reference2.IsPlayer;
		}
		else
		{
			_snap.DistanceBehind = float.MaxValue;
			_snap.ClosingSpeedBehind = 0f;
			_snap.BehindSideDist = 0f;
			_snap.CarBehindIsPlayer = false;
		}
	}

	private void ComputeLaneOccupancy(in BotContext ctx)
	{
		float laneWidth = _owner.LaneWidth;
		float speedMps = ctx.SpeedMps;
		float num = Mathf.Clamp(speedMps * 1.4f, 8f, 90f);
		float num2 = Mathf.Max(15f, speedMps * 0.5f);
		float num3 = laneWidth * 0.4f;
		float followOffset = _owner.FollowOffset;
		LaneOccupancy lanes = default;
		lanes.L_n2_FreeAhead = (lanes.L_n1_FreeAhead = (lanes.L_0_FreeAhead = (lanes.L_p1_FreeAhead = (lanes.L_p2_FreeAhead = true))));
		lanes.L_n2_SafeBehind = (lanes.L_n1_SafeBehind = (lanes.L_0_SafeBehind = (lanes.L_p1_SafeBehind = (lanes.L_p2_SafeBehind = true))));
		lanes.L_n2_NearestAheadDist = (lanes.L_n1_NearestAheadDist = (lanes.L_0_NearestAheadDist = (lanes.L_p1_NearestAheadDist = (lanes.L_p2_NearestAheadDist = float.MaxValue))));
		for (int i = -2; i <= 2; i++)
		{
			float num4 = (float)i * laneWidth - followOffset;
			bool flag = true;
			bool flag2 = true;
			float num5 = float.MaxValue;
			for (int j = 0; j < _snap.Count; j++)
			{
				ref NearbyCar reference = ref _snap.Cars[j];
				if (!reference.SameDirection || Mathf.Abs(reference.SideDist - num4) > num3)
				{
					continue;
				}
				if (reference.ForwardDist >= 0f)
				{
					if (reference.ForwardDist < num)
					{
						flag = false;
					}
					if (reference.ForwardDist < num5)
					{
						num5 = reference.ForwardDist;
					}
				}
				else if (reference.ForwardDist > 0f - num2 && reference.ClosingSpeed > 2f)
				{
					flag2 = false;
				}
			}
			switch (i)
			{
			case -2:
				lanes.L_n2_FreeAhead = flag;
				lanes.L_n2_SafeBehind = flag2;
				lanes.L_n2_NearestAheadDist = num5;
				break;
			case -1:
				lanes.L_n1_FreeAhead = flag;
				lanes.L_n1_SafeBehind = flag2;
				lanes.L_n1_NearestAheadDist = num5;
				break;
			case 0:
				lanes.L_0_FreeAhead = flag;
				lanes.L_0_SafeBehind = flag2;
				lanes.L_0_NearestAheadDist = num5;
				break;
			case 1:
				lanes.L_p1_FreeAhead = flag;
				lanes.L_p1_SafeBehind = flag2;
				lanes.L_p1_NearestAheadDist = num5;
				break;
			case 2:
				lanes.L_p2_FreeAhead = flag;
				lanes.L_p2_SafeBehind = flag2;
				lanes.L_p2_NearestAheadDist = num5;
				break;
			}
		}
		_snap.Lanes = lanes;
	}

	private static bool IsCarOnSameTrackSection(Vector3 otherVel, Vector3 ownForward)
	{
		float sqrMagnitude = otherVel.sqrMagnitude;
		if (sqrMagnitude > 4f)
		{
			float num = Vector3.Dot(ownForward, otherVel);
			if (num > 0f)
			{
				return num * num > 0.25f * sqrMagnitude;
			}
			return false;
		}
		return true;
	}
}

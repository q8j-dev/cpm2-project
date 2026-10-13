using System;
using Unity.Mathematics;
using UnityEngine;

namespace Gley.TrafficSystem
{
	public class BehaviourResult
	{
		private float _steerPercent;

		private int _targetGear;

		public TargetSpeedPoint SpeedInPoint { get; set; }

		public string Name { get; set; }

		public int Priority { get; set; }

		public float BrakePercent { get; set; }

		public float MaxAllowedSpeed { get; set; }

		public float SteerPercent
		{
			get
			{
				return _steerPercent;
			}
			set
			{
				if (value == -2.1474836E+09f)
				{
					_steerPercent = value;
				}
				else
				{
					_steerPercent = Mathf.Clamp(value, -1f, 1f);
				}
			}
		}

		public int TargetGear
		{
			get
			{
				return _targetGear;
			}
			set
			{
				_targetGear = Math.Sign(value);
			}
		}

		public BehaviourResult(int priority = 0)
		{
			Name = "DEFAULT";
			Priority = priority;
			BrakePercent = 0f;
			MaxAllowedSpeed = TrafficSystemConstants.DEFAULT_SPEED;
			TargetGear = 1;
			SteerPercent = -2.1474836E+09f;
			SpeedInPoint = new TargetSpeedPoint(TrafficSystemConstants.DEFAULT_POSITION, TrafficSystemConstants.DEFAULT_SPEED, 0f, "DEFAULT");
		}

		public void Append(BehaviourResult value, float3 frontTriggerPosition)
		{
			if (value.Priority != 0 || Priority != 0)
			{
				if (Priority < value.Priority)
				{
					Priority = value.Priority;
					_targetGear = value.TargetGear;
					_steerPercent = value.SteerPercent;
					SpeedInPoint = value.SpeedInPoint;
					BrakePercent = value.BrakePercent;
					MaxAllowedSpeed = value.MaxAllowedSpeed;
					Name = value.Name;
				}
				return;
			}
			if (TargetGear > value.TargetGear)
			{
				_targetGear = value.TargetGear;
			}
			if (SteerPercent == -2.1474836E+09f && value.SteerPercent != -2.1474836E+09f)
			{
				_steerPercent = value.SteerPercent;
			}
			else if (value.SteerPercent != -2.1474836E+09f && Mathf.Abs(SteerPercent) < Mathf.Abs(value.SteerPercent))
			{
				_steerPercent = value.SteerPercent;
			}
			float num = Vector3.SqrMagnitude(frontTriggerPosition - SpeedInPoint.StopPosition) - SpeedInPoint.DistanceToStop * SpeedInPoint.DistanceToStop;
			float num2 = Vector3.SqrMagnitude(frontTriggerPosition - value.SpeedInPoint.StopPosition) - value.SpeedInPoint.DistanceToStop * value.SpeedInPoint.DistanceToStop;
			if ((double)Mathf.Abs(num - num2) < 0.1)
			{
				if (SpeedInPoint.TargetSpeed > value.SpeedInPoint.TargetSpeed)
				{
					SpeedInPoint = value.SpeedInPoint;
				}
			}
			else if ((num2 < num || (Vector3.SqrMagnitude(frontTriggerPosition - value.SpeedInPoint.StopPosition) < 1f && value.SpeedInPoint.TargetSpeed == 0f)) && (!(Vector3.SqrMagnitude(frontTriggerPosition - SpeedInPoint.StopPosition) < 1f) || SpeedInPoint.TargetSpeed != 0f))
			{
				SpeedInPoint = value.SpeedInPoint;
			}
			if (BrakePercent < value.BrakePercent)
			{
				BrakePercent = value.BrakePercent;
				MaxAllowedSpeed = value.SpeedInPoint.TargetSpeed;
				Name = value.Name;
			}
			else if (BrakePercent == value.BrakePercent && BrakePercent != 0f)
			{
				MaxAllowedSpeed = Mathf.Min(SpeedInPoint.TargetSpeed, value.SpeedInPoint.TargetSpeed);
			}
			if (MaxAllowedSpeed > value.MaxAllowedSpeed)
			{
				MaxAllowedSpeed = value.MaxAllowedSpeed;
				Name = value.Name;
			}
		}

		public string Print(string eolCharacter = "\n")
		{
			return SpeedInPoint.Name + " " + eolCharacter + $"MaxAllowedSpeed {MaxAllowedSpeed} {eolCharacter}" + $"BrakePercent {BrakePercent} {eolCharacter}" + $"TargetGear {TargetGear} {eolCharacter}" + $"SteerPercent {SteerPercent} {eolCharacter}" + $"Speed Point: [Name {SpeedInPoint.Name} TargetSpeed {SpeedInPoint.TargetSpeed} StopPosition {SpeedInPoint.StopPosition} DistanceToStop {SpeedInPoint.DistanceToStop}]";
		}
	}
}

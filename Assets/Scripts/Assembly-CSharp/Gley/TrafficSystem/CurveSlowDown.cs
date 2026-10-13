using Unity.Mathematics;
using UnityEngine;

namespace Gley.TrafficSystem
{
	public class CurveSlowDown : VehicleBehaviour
	{
		private float _safeCurveSpeed = 40f;

		protected override void OnBecomeActive()
		{
			base.OnBecomeActive();
			MovementInfo.OnKnownListUpdated += KnownListUpdatedHandler;
		}

		protected override void OnBecameInactive()
		{
			base.OnBecameInactive();
			MovementInfo.OnKnownListUpdated -= KnownListUpdatedHandler;
			base.VehicleComponent.MovementInfo.SetMaxSpeedCorrectionPercent(1f);
		}

		public override BehaviourResult Execute(MovementInfo knownWaypointsList, float requiredBrakePower, bool stopTargetReached, float3 stopPosition, int currentGear)
		{
			return new BehaviourResult();
		}

		private void KnownListUpdatedHandler(int vehicleIndex)
		{
			if (vehicleIndex != base.VehicleIndex)
			{
				return;
			}
			float num = base.VehicleComponent.GetCurrentSpeedMS().ToKMH();
			if (num <= _safeCurveSpeed)
			{
				base.VehicleComponent.MovementInfo.SetMaxSpeedCorrectionPercent(1f);
				return;
			}
			bool flag = false;
			float num2 = Vector3.Angle(base.VehicleComponent.GetForwardVector(), base.VehicleComponent.MovementInfo.GetFirstPosition() - base.VehicleComponent.FrontPosition.position);
			if (num2 < 5f)
			{
				num2 = 0f;
			}
			for (int i = 0; i < base.VehicleComponent.MovementInfo.GetAngleLength(); i++)
			{
				float num3 = math.abs(base.VehicleComponent.MovementInfo.GetAngle(i));
				if (num3 > 1f)
				{
					num2 += num3;
					flag = true;
				}
			}
			if (!flag)
			{
				base.VehicleComponent.MovementInfo.SetMaxSpeedCorrectionPercent(1f);
				return;
			}
			num2 = Mathf.Clamp(num2, 0f, 90f);
			float num4 = num / (float)base.VehicleComponent.maxPossibleSpeed;
			float value = 1f - num2 / 90f * Mathf.Clamp01(num4 * 1.2f);
			value = Mathf.Clamp(value, 0.2f, 1f);
			base.VehicleComponent.MovementInfo.AddSlowDownPoint(1);
			base.VehicleComponent.MovementInfo.SetMaxSpeedCorrectionPercent(value);
		}

		public override void OnDestroy()
		{
			MovementInfo.OnKnownListUpdated -= KnownListUpdatedHandler;
		}
	}
}

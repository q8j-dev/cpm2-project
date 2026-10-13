using Unity.Mathematics;

namespace Gley.TrafficSystem
{
	public class DriveOnSide : VehicleBehaviour
	{
		protected override void OnBecameInactive()
		{
			base.OnBecameInactive();
			base.VehicleComponent.MovementInfo.ResetOffset();
		}

		public override void SetParams(object[] parameters)
		{
			base.SetParams(parameters);
			switch ((RoadSide)parameters[0])
			{
			case RoadSide.Left:
				base.VehicleComponent.MovementInfo.SetOffset(-1f);
				break;
			case RoadSide.Right:
				base.VehicleComponent.MovementInfo.SetOffset(1f);
				break;
			}
		}

		public override BehaviourResult Execute(MovementInfo knownWaypointsList, float requiredBrakePower, bool stopTargetReached, float3 stopPosition, int currentGear)
		{
			return new BehaviourResult();
		}

		public override void OnDestroy()
		{
		}
	}
}

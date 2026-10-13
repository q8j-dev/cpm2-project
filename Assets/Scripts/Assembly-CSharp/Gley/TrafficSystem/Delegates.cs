using Gley.TrafficSystem.Internal;

namespace Gley.TrafficSystem
{
	public class Delegates
	{
		public static void SetModifyTriggerSize(ModifyTriggerSize modifyTriggerSizeDelegate, int vehicleIndex = -1)
		{
			TrafficManager.Instance.AllVehiclesData?.ModifyTriggerSize(vehicleIndex, modifyTriggerSizeDelegate);
		}

		public static void SetSpawnWaypointSelector(SpawnWaypointSelector spawnWaypointSelectorDelegate)
		{
			TrafficManager.Instance.WaypointSelector?.SetSpawnWaypointSelector(spawnWaypointSelectorDelegate);
		}

		public static void SetTrafficLightsBehaviour(TrafficLightsBehaviour trafficLightsBehaviourDelegate)
		{
			TrafficManager.Instance.IntersectionManager?.SetTrafficLightsBehaviour(trafficLightsBehaviourDelegate);
		}

		public static void SetCustomPositionValidation(CustomPositionValidation customPositionValidation)
		{
			TrafficManager.Instance.PositionValidator.SetCustomPositionValidation(customPositionValidation);
		}
	}
}

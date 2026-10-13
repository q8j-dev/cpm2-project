namespace Gley.UrbanSystem.Internal
{
	public interface IIntersection
	{
		bool IsPathFree(int waypointIndex);

		void VehicleLeft(int vehicleIndex);

		void VehicleEnter(int vehicleIndex);

		void PedestrianPassed(int agentIndex);
	}
}

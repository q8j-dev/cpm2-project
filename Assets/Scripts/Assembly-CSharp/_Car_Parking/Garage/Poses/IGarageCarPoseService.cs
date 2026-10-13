using CPMEngine;

namespace _Car_Parking.Garage.Poses
{
	public interface IGarageCarPoseService
	{
		bool TryGetPose(int garageId, string carId, out SavedPoseDto pose);

		void SetPose(int garageId, string carId, Powertrain powertrain);

		void RemovePose(int garageId, string carId);

		void RemovePose(string carId);
	}
}

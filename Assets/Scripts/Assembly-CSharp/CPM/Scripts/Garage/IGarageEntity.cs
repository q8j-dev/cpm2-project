using System.Threading;
using Cysharp.Threading.Tasks;

namespace CPM.Scripts.Garage
{
	public interface IGarageEntity
	{
		UniTask Init(VehicleData enteredVehicle, GarageController controller, CancellationToken cancellationToken);

		void UpdateCarEnterSubscriptions(bool toSubscribe);

		UniTask LoadCarBySpotId(int spotId, bool focusOnCar);

		UniTaskVoid RemoveCarFromSpot(int spotId);

		void ResetLastCarPosition(bool resetWheelsRotation = false);

		void ResetSavedCarPosition(VehicleData vehicleData);

		void ResetAllCarsPositionsInCurrentGarage();

		void MoveLastCarToSavedPosition(bool resetWheelsRotation);

		void SaveCarsPositions();

		void SaveCarPosition(VehicleData vehicleData);

		void ClearSpaceForCar(VehicleData vehicleData);

		VehicleData FocusOnCarOnSpot(int spotId, bool withCameraReset = true);

		VehicleData GetCarBySpot(int slotId);

		VehicleData[] GetAllCars();

		void CorrectCamera(VehicleData vehicleData, bool withCameraReset = true);

		UniTask Exit(CancellationToken cancellationToken);

		void Clear();

		void PlaceCarOnSpot(int spotId, VehicleData vehicleData, bool turnOff = true, bool setIsOccupied = false);
	}
}

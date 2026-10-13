using UnityEngine;
using _Car_Parking;

namespace Car_Parking.Scripts.Drag.CarCompositions
{
	public class TrailerComposition
	{
		private readonly VehicleData _vehicle;

		public TrailerComposition(VehicleData vehicle)
		{
			_vehicle = vehicle;
		}

		public void Init()
		{
			if (_vehicle.isTraffiCar || !_vehicle.isTruck || !_vehicle.CarChildsReferences.Trailer)
			{
				return;
			}
			if (V.Car == _vehicle.gameObject)
			{
				_vehicle.LevelsProvider.Trailer = _vehicle.CarChildsReferences.Trailer.gameObject;
				if ((bool)_vehicle.LevelsProvider.LevelScript)
				{
					_vehicle.LevelsProvider.LevelScript.localTrailerPos2 = _vehicle.LevelsProvider.Trailer.transform.localPosition;
				}
				if (!FlagsStorage.IsLevelScene || !_vehicle.LevelsProvider.LevelScript.WithTrailer)
				{
					Object.Destroy(_vehicle.CarChildsReferences.Trailer.gameObject);
				}
				if (!FlagsStorage.IsLevelScene && FlagsStorage.IsFreeDriveMode)
				{
					Object.Destroy(_vehicle.GetComponent<CharacterJoint>());
				}
			}
			else if (FlagsStorage.IsFreeDriveMode || FlagsStorage.IsMainMenu)
			{
				Object.Destroy(_vehicle.CarChildsReferences.Trailer.gameObject);
			}
		}
	}
}

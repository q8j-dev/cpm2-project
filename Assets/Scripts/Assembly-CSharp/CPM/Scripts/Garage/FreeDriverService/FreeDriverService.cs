using Cysharp.Threading.Tasks;
using Racing.CameraManagement;
using UnityEngine;
using UnityEngine.Scripting;
using _Car_Parking;
using _Car_Parking.Scripts.DataStorageService;

namespace CPM.Scripts.Garage.FreeDriverService
{
	[Preserve]
	public class FreeDriverService : IFreeDriverService
	{
		private readonly ICharacterEquipmentDataService _characterEquipmentDataService;

		private readonly CameraManager _cameraManager;

		private readonly IGarageSelectorService _garageSelectorService;

		private readonly GaragePositionsConfig _garagePositionsConfig;

		public FreeDriverService(ICharacterEquipmentDataService characterEquipmentDataService, CameraManager cameraManager, GaragePositionsConfig garagePositionsConfig, IGarageSelectorService garageSelectorService)
		{
			_characterEquipmentDataService = characterEquipmentDataService;
			_cameraManager = cameraManager;
			_garagePositionsConfig = garagePositionsConfig;
			_garageSelectorService = garageSelectorService;
		}

		public void PlaceDriverOnDefault()
		{
			V.DriverPerson.transform.SetPositionAndRotation(_garagePositionsConfig.GaragePositionsDatas[_garageSelectorService.CurrentGarageIndex].FreeDriverPlace.Position, _garagePositionsConfig.GaragePositionsDatas[_garageSelectorService.CurrentGarageIndex].FreeDriverPlace.Rotation);
		}

		public async UniTask SetupFreeDriver(bool withDelay = false)
		{
			GameObject driver = V.DriverPerson;
			if (!driver)
			{
				return;
			}
			driver.GetComponent<Collider>().enabled = true;
			if (withDelay)
			{
				await UniTask.NextFrame();
				if (!driver)
				{
					return;
				}
				Animator animator = driver.GetComponent<Animator>();
				animator.applyRootMotion = false;
				await UniTask.NextFrame();
				if (!driver || !animator)
				{
					return;
				}
				PlaceDriverOnDefault();
				animator.applyRootMotion = true;
			}
			else
			{
				PlaceDriverOnDefault();
			}
			_cameraManager.ActivateCamera(RacingCameraType.GarageLookAround, driver.transform);
		}

		public void CreateDriver(bool withSetup = true)
		{
			CreatePerson(_characterEquipmentDataService.Gender);
			if (withSetup)
			{
				SetupFreeDriver().Forget();
			}
		}

		private void CreatePerson(int gender)
		{
			GameObject obj = Resources.Load<GameObject>((gender == 0) ? "person0" : "person1");
			V.DriverPerson = V.CoreGameManager._prefabFactory.Instantiate(obj, _garagePositionsConfig.GaragePositionsDatas[_garageSelectorService.CurrentGarageIndex].FreeDriverPlace.Position, _garagePositionsConfig.GaragePositionsDatas[_garageSelectorService.CurrentGarageIndex].FreeDriverPlace.Rotation);
			FlagsStorage.OutOfCar = true;
		}
	}
}

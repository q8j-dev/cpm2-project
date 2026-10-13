using System;
using System.Threading;
using CPMEngine;
using Cinemachine;
using Cysharp.Threading.Tasks;
using Racing.CameraManagement;
using Racing.CameraManagement.SpecificTypes;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using _Car_Parking;
using _Car_Parking.Scripts.Analytics.Event.Contexts;
using _Car_Parking.Scripts.Analytics.Event.CustomEvent;
using _Car_Parking.Scripts.Drag;

namespace CPM.Scripts.Garage
{
	public class MenuGarage : GarageEntity, IGarageEntity
	{
		private const float DistanceToSetActiveDoorButton = 1f;

		public override async UniTask Init(VehicleData enteredVehicle, GarageController controller, CancellationToken cancellationToken)
		{
			FlagsStorage.InGarageMenu = true;
			FlagsStorage.IsInputEnabledInThisWindow = false;
			FlagsStorage.CanEnterGarage = true;
			await base.Init(enteredVehicle, controller, cancellationToken);
			if (cancellationToken.IsCancellationRequested)
			{
				return;
			}
			CarInOutSound.SetOutCarSound();
			if ((bool)ControlItemsProvider.TouchField)
			{
				ControlItemsProvider.TouchField.enabled = true;
			}
			Camera.main.GetUniversalAdditionalCameraData().cameraStack.Clear();
			await UniTask.Delay(TimeSpan.FromSeconds(1.0), ignoreTimeScale: false, PlayerLoopTiming.Update, cancellationToken);
			if (!cancellationToken.IsCancellationRequested)
			{
				V.OnCarEntered += CheckEngineOnCarEnter;
				_analyticEventFactory.CreateEvent<GarageInfoEvent, DefaultContext>(new DefaultContext()).Send();
				if ((bool)CPMGraphicsSettingsManager.Instance)
				{
					CPMGraphicsSettingsManager.Instance.SetGraphics();
				}
			}
		}

		protected override async UniTask LoadOtherGarageCars()
		{
			await base.LoadOtherGarageCars();
			if (SlotService.HaveCarInAnyGarage && CameraManager.HasValue)
			{
				CameraManager.Value.StartMoveVeryFirstCam().Forget();
			}
			foreach (VehicleData value in GetSpawnedCars().Values)
			{
				if ((bool)value)
				{
					RestoreInteriorWhenBodyLoaded(value, value.GetCancellationTokenOnDestroy()).Forget();
				}
			}
		}

		private async UniTaskVoid RestoreInteriorWhenBodyLoaded(VehicleData vehicleData, CancellationToken cancellationToken)
		{
			await UniTask.WaitUntil(() => !vehicleData || vehicleData.IsCarGraphicLoaded, PlayerLoopTiming.Update, cancellationToken).SuppressCancellationThrow();
			if (!cancellationToken.IsCancellationRequested && (bool)vehicleData)
			{
				if (CPMGraphicsSettingsManager.Instance.AdditionalEffects != CPMGraphicsSettings.AdditionalEffects.Low)
				{
					await vehicleData.InteriorsComposition.ForceSetSavedInterior(cancellationToken);
				}
				else
				{
					vehicleData.InteriorsComposition.TryRestoreSavedInterior();
				}
			}
		}

		protected override void EnterCar(GameObject person, VehicleData vehicleData)
		{
			base.EnterCar(person, vehicleData);
			CameraManager.Value?.ActivateCamera(RacingCameraType.ForwardView, vehicleData.transform);
			ControlItemsProvider.TouchField.SetActive(active: false);
			bool isOpenCloseRoof = (bool)vehicleData && (bool)vehicleData.CarAnimationReferences && vehicleData.CarAnimationReferences.IsOpenCloseRoof;
			_controller.MenuGarageNavigationWindow.UpdateHUD(isInCar: true, isOpenCloseRoof);
		}

		private void CheckEngineOnCarEnter(GameObject person, VehicleData vehicleData)
		{
			if (vehicleData.TryGetComponent<VehicleEntityProvider>(out var component))
			{
				CarTuningFactory.Value?.CheckStatus(component.CarEntity.Engine);
			}
		}

		protected override void ExitCar(GameObject person, VehicleData vehicleData)
		{
			base.ExitCar(person, vehicleData);
			CameraManager.Value?.ActivateCamera(RacingCameraType.None);
			ControlItemsProvider.TouchField.SetActive(active: true);
			_controller.MenuGarageNavigationWindow.UpdateHUD(isInCar: false);
		}

		public override void CorrectCamera(VehicleData vehicleData, bool withCameraReset = true)
		{
			if ((bool)vehicleData && CameraManager.HasValue)
			{
				CameraManager.Value.ActivateCamera(RacingCameraType.GarageLookAround, vehicleData.transform, null, (CinemachineVirtualCameraBase vc) =>
				{
					if (withCameraReset && (bool)vc)
					{
						GarageMainCameraHandler component = vc.GetComponent<GarageMainCameraHandler>();
						if ((bool)component)
						{
							ApplyGarageCamReset(component, vehicleData);
						}
					}
				});
			}
			else
			{
				if (!CameraManager.HasValue)
				{
					return;
				}
				CameraManager.Value.ActivateCamera(RacingCameraType.GarageLookAround, V.DriverPerson.transform, null, (CinemachineVirtualCameraBase vc) =>
				{
					if ((bool)vc)
					{
						GarageMainCameraHandler component = vc.GetComponent<GarageMainCameraHandler>();
						if ((bool)component)
						{
							component.ResetCameraForFree();
						}
					}
				});
			}
		}

		private void ApplyGarageCamReset(GarageMainCameraHandler cam, VehicleData vehicleData)
		{
			if (!vehicleData || (bool)vehicleData.Collider)
			{
				if ((bool)vehicleData && (bool)vehicleData.Collider)
				{
					cam.collider = vehicleData.Collider;
				}
				cam.ResetCamToDefault();
			}
			else
			{
				vehicleData.OnCarGraphicLoaded -= OnGraphicLoaded;
				vehicleData.OnCarGraphicLoaded += OnGraphicLoaded;
			}
			void OnGraphicLoaded()
			{
				vehicleData.OnCarGraphicLoaded -= OnGraphicLoaded;
				if ((bool)cam && (bool)vehicleData)
				{
					if ((bool)vehicleData.Collider)
					{
						cam.collider = vehicleData.Collider;
					}
					cam.ResetCamToDefault();
				}
			}
		}

		public override async UniTask Exit(CancellationToken cancellationToken)
		{
			_spawnedCars.Clear();
			FlagsStorage.InGarageMenu = false;
			if ((bool)V.Car && V.Car.TryGetComponent<Powertrain>(out var component))
			{
				FlagsStorage.IsAutomatic = component.Automatic;
				FlagsStorage.LoadedFromGarage = true;
			}
			V.OnCarEntered -= CheckEngineOnCarEnter;
			await base.Exit(cancellationToken);
		}
	}
}

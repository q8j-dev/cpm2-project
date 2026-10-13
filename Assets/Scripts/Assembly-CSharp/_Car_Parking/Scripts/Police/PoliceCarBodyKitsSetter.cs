using System;
using CPM.Database;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.Car;

namespace _Car_Parking.Scripts.Police
{
	public class PoliceCarBodyKitsSetter : MonoBehaviour
	{
		[SerializeField]
		private PoliceCarBodyKitsUpdater PoliceCarBodyKitsUpdater;

		private VehicleData _vehicleData;

		private IPoliceSystem _policeSystem;

		private Array AllBodyKitTypes => Enum.GetValues(typeof(PoliceBodyKitType));

		private CarInfo CarInfo
		{
			get
			{
				if (!_vehicleData || !_vehicleData.CarInfoProvider.IsReady)
				{
					return null;
				}
				return _vehicleData.CarInfoProvider.CarInfo;
			}
		}

		[Inject]
		public void Construct(IPoliceSystem policeSystem)
		{
			_policeSystem = policeSystem;
		}

		public void Init(VehicleData vehicleData)
		{
			_vehicleData = vehicleData;
			_vehicleData.CarInfoProvider.OnReady += OnCarInfoReady;
		}

		public void SetDefaultBodyKits()
		{
			if (!PoliceCarBodyKitsUpdater)
			{
				return;
			}
			PoliceCarBodyKitsUpdater.Clear();
			if (CarInfo == null || CarInfo.AdditionalData == null || CarInfo.AdditionalData.Police == null || !CarInfo.AdditionalData.Police.Installed)
			{
				return;
			}
			foreach (PoliceBodyKitType allBodyKitType in AllBodyKitTypes)
			{
				int index = CarInfo.AdditionalData.Police.InstalledBodyKits.GetIndex(allBodyKitType);
				if (index >= 0)
				{
					PoliceCarBodyKitsUpdater.SetBodyKit(allBodyKitType, index);
				}
			}
		}

		private void OnEnable()
		{
			CarInfoPoliceExtensionInstall.OnInstallUpdated += OnPoliceInstalledUpdated;
		}

		private void OnDisable()
		{
			CarInfoPoliceExtensionInstall.OnInstallUpdated -= OnPoliceInstalledUpdated;
		}

		private void OnDestroy()
		{
			if ((bool)_vehicleData && _vehicleData.CarInfoProvider != null)
			{
				_vehicleData.CarInfoProvider.OnReady -= OnCarInfoReady;
			}
		}

		private void OnPoliceInstalledUpdated(PoliceData policeData, bool isInstalled)
		{
			if (CarInfo != null && CarInfo.AdditionalData.Police == policeData)
			{
				SetDefaultBodyKits();
			}
		}

		private void OnCarInfoReady(CarInfoProvider carInfoGetterController, CarInfo carInfo)
		{
			if (CarInfo != null)
			{
				SetDefaultBodyKits();
			}
		}
	}
}

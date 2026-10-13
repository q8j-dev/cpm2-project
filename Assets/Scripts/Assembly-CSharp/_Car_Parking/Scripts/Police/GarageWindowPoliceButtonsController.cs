using CPM.Database;
using UnityEngine;

namespace _Car_Parking.Scripts.Police
{
	public class GarageWindowPoliceButtonsController : PoliceUIBase
	{
		[SerializeField]
		private BuyPoliceButton BuyPoliceButton;

		[SerializeField]
		private EnablePoliceButton EnablePoliceButton;

		[SerializeField]
		private DisablePoliceButton DisablePoliceButton;

		private void OnEnable()
		{
			if ((bool)V.Car)
			{
				UpdateUI(V.Car.GetComponent<VehicleData>());
			}
			policeSystem.OnBoughtUpdated += OnSystemBoughtUpdated;
			CarInfoPoliceExtensionInstall.OnInstallUpdated += OnSystemInstalledUpdated;
			V.OnCarEntered += OnCarEntered;
		}

		private void OnDisable()
		{
			V.OnCarEntered -= OnCarEntered;
			policeSystem.OnBoughtUpdated -= OnSystemBoughtUpdated;
			CarInfoPoliceExtensionInstall.OnInstallUpdated -= OnSystemInstalledUpdated;
		}

		private void OnCarEntered(GameObject car, VehicleData vehicleData)
		{
			UpdateUI(vehicleData);
		}

		private void OnSystemBoughtUpdated(CarInfo carInfo, bool value)
		{
			UpdateUI(V.Car.GetComponent<VehicleData>());
		}

		private void OnSystemInstalledUpdated(PoliceData policeData, bool isInstalled)
		{
			UpdateUI(V.Car.GetComponent<VehicleData>());
		}

		private void UpdateUI(VehicleData vehicleData)
		{
			if ((bool)vehicleData)
			{
				if (policeSystem.IsBought(vehicleData.carInfo))
				{
					UpdateButtons(vehicleData.CarInfoProvider.CarInfo.AdditionalData.Police.Installed ? DisablePoliceButton.GetInstanceID() : EnablePoliceButton.GetInstanceID());
				}
				else
				{
					UpdateButtons(BuyPoliceButton.GetInstanceID());
				}
			}
			else
			{
				UpdateButtons(-1);
			}
		}

		private void UpdateButtons(int instanceID)
		{
			BuyPoliceButton.gameObject.SetActive(instanceID == BuyPoliceButton.GetInstanceID());
			EnablePoliceButton.gameObject.SetActive(instanceID == EnablePoliceButton.GetInstanceID());
			DisablePoliceButton.gameObject.SetActive(instanceID == DisablePoliceButton.GetInstanceID());
		}
	}
}

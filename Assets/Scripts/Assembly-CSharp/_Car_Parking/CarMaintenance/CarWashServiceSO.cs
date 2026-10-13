using CPM.Database;
using CodeStage.AntiCheat.Storage;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using _Car_Parking.Scripts.CustomDataTypes;
using _Car_Parking.Scripts.Economics;

namespace _Car_Parking.CarMaintenance
{
	[CreateAssetMenu(fileName = "CarWashMaintenanceService", menuName = "Addressables/Maintenance Services/Car Wash")]
	public class CarWashServiceSO : MaintenanceServiceBaseSO
	{
		private readonly int _dirtMaskValueId = Shader.PropertyToID("_MaskValue");

		private VehicleData _vehicleData;

		public override bool IsServiceConditionsMet(out ServiceMessages messages)
		{
			if (ObscuredPrefs.GetFloat("money", 25000f) < _serviceCost)
			{
				messages = ServiceMessages.NotEnoughMoney;
				return false;
			}
			if (!_vehicleData || ((bool)_vehicleData.CarMat && _vehicleData.CarMat.GetFloat(_dirtMaskValueId) < 0.05f))
			{
				messages = ServiceMessages.CarIsClean;
				return false;
			}
			messages = ServiceMessages.None;
			return true;
		}

		public override void Init(MaintenanceService maintenanceService)
		{
			base.Init(maintenanceService);
			_vehicleData = maintenanceService.CurrentVehicleData;
		}

		public override void Execute(IPricesService pricesService, CurrencyManagerController currencyManagerController, FreeDriveDB freeDriveDB)
		{
			if (currencyManagerController == null)
			{
				return;
			}
			int priceValue = pricesService.CarWashFreePrice;
			AdvancedInt price = new AdvancedInt(priceValue);
			currencyManagerController.TryMoneySpend(out var _, price, () =>
			{
				StartExecution();
				++freeDriveDB.carWash;
				switch (_serviceProvideType)
				{
				case ExecutionType.Continuous:
					ContinuouslyReduceCarDirt(priceValue);
					break;
				case ExecutionType.Instant:
					InstantlyReduceCarDirt(priceValue);
					break;
				}
			});
		}

		private void ContinuouslyReduceCarDirt(int spentMoney)
		{
			_maintenanceService.ExecuteDirtReduction(spentMoney);
		}

		private void InstantlyReduceCarDirt(int spentMoney)
		{
			_vehicleData.DirtComposition.ResetCarDirt();
			_maintenanceService.ShowWashCompletedMessage(spentMoney);
		}

		private void StartExecution()
		{
			if (PhotonNetwork.IsConnected)
			{
				PhotonView component = _maintenanceService.CurrentCar.GetComponent<PhotonView>();
				object[] eventContent = new object[2] { _maintenanceService.ServiceId, component.ViewID };
				RaiseEventOptions raiseEventOptions = new RaiseEventOptions
				{
					Receivers = ReceiverGroup.All
				};
				PhotonNetwork.RaiseEvent(2, eventContent, raiseEventOptions, SendOptions.SendReliable);
			}
			else
			{
				_maintenanceService.StartCarWash();
			}
		}
	}
}

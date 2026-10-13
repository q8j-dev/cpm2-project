using UnityEngine;
using _Car_Parking;

namespace Car_Parking.Scripts.Drag.CarCompositions
{
	public class CarHealthComposition
	{
		private readonly VehicleData _vehicle;

		public bool Damaged { get; set; }

		public CarHealthComposition(VehicleData vehicle)
		{
			_vehicle = vehicle;
		}

		public void DamageEngine(bool state, bool sendRpc = true)
		{
			Damaged = _vehicle.GetPowertrain.CheckEngineStatus;
			_vehicle.ExhaustComposition.DamageEngine(state);
			if (state)
			{
				if (!(_vehicle.gameObject != V.Car) && !FlagsStorage.IsPassenger && ((FlagsStorage.IsMultiFree && !_vehicle.isGarageCar) & sendRpc))
				{
					_vehicle.FreeDriveProvider.MultiPlayer.SendValueStr(36, _vehicle.photonView.ViewID.ToString(), "1");
				}
			}
			else if ((FlagsStorage.IsMultiFree && _vehicle.gameObject == V.Car && !_vehicle.isGarageCar) & sendRpc)
			{
				_vehicle.FreeDriveProvider.MultiPlayer.SendValueStr(36, _vehicle.photonView.ViewID.ToString(), "0");
			}
		}

		public void RepairVisual()
		{
			if ((bool)_vehicle.WindowComposition.WindowMat)
			{
				_vehicle.WindowComposition.SetWindowShatterHoleActive(isBreak: false);
			}
			else
			{
				Debug.LogError("No Window Material found!");
			}
			if (_vehicle.TryGetComponent<CarTriggers>(out var component))
			{
				component.DamageCount = 0;
			}
			if (_vehicle.TryGetComponent<VehicleDamage>(out var component2))
			{
				component2.repair = true;
				PlayerDatas.DisableEnableAll(isEnable: true, component2.HideWhenDamage);
			}
		}
	}
}

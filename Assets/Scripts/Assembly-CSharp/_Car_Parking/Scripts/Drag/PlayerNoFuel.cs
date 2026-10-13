using System;
using CPMEngine;
using UnityEngine;

namespace _Car_Parking.Scripts.Drag
{
	public class PlayerNoFuel : IRaceControllerModule
	{
		private Action _onNoFuel;

		private CircuitRaceControlller _circuitRaceControlller;

		private VehicleData _vehicleData;

		private Powertrain _powertrain;

		private CircuitCarStearing _circuitCarStearing;

		private float _delay;

		public PlayerNoFuel(CircuitRaceControlller circuitRaceControlller, Action OnNoFuel)
		{
			_circuitRaceControlller = circuitRaceControlller;
			_onNoFuel = OnNoFuel;
		}

		public void RaceStarted()
		{
			_delay = 3f;
			if ((bool)_circuitRaceControlller && (bool)_circuitRaceControlller.circuitCarsControl && (bool)_circuitRaceControlller.circuitCarsControl.GetUserLapRaceCar())
			{
				_vehicleData = _circuitRaceControlller.circuitCarsControl.GetUserLapRaceCar().GetComponent<VehicleData>();
				_powertrain = _vehicleData.GetComponent<Powertrain>();
				_circuitCarStearing = _vehicleData.GetComponent<CircuitCarStearing>();
			}
		}

		public void Update()
		{
			if ((_delay -= Time.deltaTime) < 0f && (bool)_vehicleData && (bool)_vehicleData.CarChildsReferences.FuelTank && _vehicleData.CarChildsReferences.FuelTank.CurrentFuel < 0.001f && (bool)_powertrain && _powertrain.SpeedKmh <= 0.1f && !_circuitRaceControlller.IsInPitstop() && !_circuitCarStearing.enabled)
			{
				_vehicleData = null;
				_onNoFuel?.Invoke();
			}
		}

		public void RaceFinished()
		{
			_vehicleData = null;
			_powertrain = null;
			_circuitCarStearing = null;
		}
	}
}

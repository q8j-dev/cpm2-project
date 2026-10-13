using System;
using CPMEngine;
using UnityEngine;

namespace _Car_Parking.Scripts.Drag
{
	public class PlayerCarBroken : IRaceControllerModule
	{
		private Action _onCarBroken;

		private CircuitRaceControlller _circuitRaceControlller;

		private Powertrain _powertrain;

		private float _delay;

		public PlayerCarBroken(CircuitRaceControlller circuitRaceControlller, Action OnCarBroken)
		{
			_circuitRaceControlller = circuitRaceControlller;
			_onCarBroken = OnCarBroken;
		}

		public void RaceStarted()
		{
			_delay = 3f;
			if ((bool)_circuitRaceControlller && (bool)_circuitRaceControlller.circuitCarsControl && (bool)_circuitRaceControlller.circuitCarsControl.GetUserLapRaceCar())
			{
				_powertrain = _circuitRaceControlller.circuitCarsControl.GetUserLapRaceCar().GetComponent<Powertrain>();
			}
		}

		public void Update()
		{
			if ((_delay -= Time.deltaTime) < 0f && (bool)_powertrain && _powertrain.IsEngineBroken)
			{
				_powertrain = null;
				_onCarBroken?.Invoke();
			}
		}

		public void RaceFinished()
		{
			_powertrain = null;
		}
	}
}

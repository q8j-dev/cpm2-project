using RacingScripts.RaceControllers;
using UnityEngine;
using VContainer.Unity;

namespace _Car_Parking.Scripts.DiscordPresence
{
	public sealed class DiscordCircuitLapReporter : IStartable, ITickable
	{
		private const float PollIntervalSeconds = 1f;

		private readonly IDiscordPresenceService _presence;

		private readonly RaceModeController _raceModeController;

		private CircuitRaceControlller _raceController;

		private float _nextPollTime;

		public DiscordCircuitLapReporter(IDiscordPresenceService presence, RaceModeController raceModeController)
		{
			_presence = presence;
			_raceModeController = raceModeController;
		}

		public void Start()
		{
			_presence.SetActivity(DiscordActivityKind.CircuitRacing);
			_presence.SetContext(new DiscordActivityContext(DiscordEnvironmentName.Current(), 0, 0));
		}

		public void Tick()
		{
			if (Time.unscaledTime < _nextPollTime)
			{
				return;
			}
			_nextPollTime = Time.unscaledTime + 1f;
			LapRaceCar lapRaceCar = TryGetUserCar();
			if (!(lapRaceCar == null))
			{
				int raceLapsCount = _raceController.GetRaceLapsCount();
				if (raceLapsCount > 0)
				{
					int lap = Mathf.Clamp(lapRaceCar.GetCurrentLap() + 1, 1, raceLapsCount);
					_presence.SetContext(new DiscordActivityContext(DiscordEnvironmentName.Current(), lap, raceLapsCount));
				}
			}
		}

		private LapRaceCar TryGetUserCar()
		{
			if (_raceController == null)
			{
				if (_raceModeController == null || _raceModeController.SpawnedObject == null)
				{
					return null;
				}
				_raceController = _raceModeController.SpawnedObject.GetComponent<CircuitRaceControlller>();
				if (_raceController == null)
				{
					return null;
				}
			}
			if (!(_raceController.circuitCarsControl == null))
			{
				return _raceController.circuitCarsControl.GetUserLapRaceCar();
			}
			return null;
		}
	}
}

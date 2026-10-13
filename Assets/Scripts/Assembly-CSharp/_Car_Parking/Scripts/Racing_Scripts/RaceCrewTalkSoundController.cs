using System;
using UniRx;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.Audio;
using _Car_Parking.Scripts.Audio.SoundBanks;

namespace _Car_Parking.Scripts.Racing_Scripts
{
	public class RaceCrewTalkSoundController : IDisposable
	{
		private CircuitRaceControlller _circuitRaceController;

		public const string PlayRaceCrewTalkNameEvent = "Play_Race_Crew_Talk";

		public const string StopRaceCrewTalkNameEvent = "Stop_Race_Crew_Talk";

		public const string CrewRadioSwitch = "Crew_Radio_Switch";

		public const string RedLightEvent = "Play_count_ready";

		public const string GreenLightEvent = "Play_count_go";

		public const string FuelRefillEvent = "Play_fuel_refill";

		public const string WheelsChangeEvent = "Play_wheels_change";

		public const string PlayFuelPut = "Play_Fuel_Put";

		public const string PlayFuelLoop = "Play_Fuel_Loop";

		public const string StopFuelLoop = "Stop_Fuel_Loop";

		public const string PlayFuelOut = "Play_Fuel_Out";

		private const float TimeToPlayOffRoadSoundEvent = 5f;

		private const float TimeToPlayWrongWaySoundEvent = 5f;

		private string _currentSwitch;

		private float _startTimeOffRoad;

		private float _startTimeWrongWay;

		private bool _fixedCarOnPitStop;

		private SoundBankLoadService _soundBankLoadService;

		[Inject]
		public void Construct(SoundBankLoadService soundBankLoadService)
		{
			_soundBankLoadService = soundBankLoadService;
		}

		public void Init(CircuitRaceControlller circuitRaceController)
		{
			_circuitRaceController = circuitRaceController;
			_soundBankLoadService?.TryLoadBank(SoundBankType.CpmEventVoices);
		}

		public void PlayRedLightSound()
		{
			AudioWrapper.PostEvent("Play_count_ready", _circuitRaceController.gameObject);
		}

		public void PlayGreenLightSound()
		{
			AudioWrapper.PostEvent("Play_count_go", _circuitRaceController.gameObject);
		}

		public void PlayFuelRefillSound()
		{
			AudioWrapper.PostEvent("Play_fuel_refill", _circuitRaceController.gameObject);
			_fixedCarOnPitStop = true;
		}

		public void PlayWheelsChangeSound()
		{
			AudioWrapper.PostEvent("Play_wheels_change", _circuitRaceController.gameObject);
			_fixedCarOnPitStop = true;
		}

		public void PlayFuelPutSound()
		{
			AudioWrapper.PostEvent("Play_Fuel_Put", _circuitRaceController.gameObject);
		}

		public void PlayFuelLoopSound()
		{
			AudioWrapper.PostEvent("Play_Fuel_Loop", _circuitRaceController.gameObject);
		}

		public void StopFuelLoopSound()
		{
			AudioWrapper.PostEvent("Stop_Fuel_Loop", _circuitRaceController.gameObject);
		}

		public void PlayFuelOutSound()
		{
			AudioWrapper.PostEvent("Play_Fuel_Out", _circuitRaceController.gameObject);
		}

		public void PlayRaceCrewTalkSoundEvent(CrewTalkSoundType crewTalkSoundType)
		{
			string text = crewTalkSoundType.ToString();
			if (!(_currentSwitch == text))
			{
				_currentSwitch = text;
				AudioWrapper.SetSwitch("Crew_Radio_Switch", text, _circuitRaceController.gameObject);
				AudioWrapper.PostEvent("Play_Race_Crew_Talk", _circuitRaceController.gameObject, 1u, (object a, AkCallbackType b, AkCallbackInfo c) =>
				{
					_currentSwitch = "";
				}, _circuitRaceController.gameObject);
			}
		}

		public void StopRaceCrewTalkSoundEvent()
		{
			AudioWrapper.PostEvent("Stop_Race_Crew_Talk", _circuitRaceController.gameObject);
		}

		public void PlayOffRoadSoundEvent(bool state)
		{
			if (!state)
			{
				_startTimeOffRoad = 0f;
				return;
			}
			if (_startTimeOffRoad == 0f)
			{
				_startTimeOffRoad = Time.time;
			}
			if (Time.time - _startTimeOffRoad > 5f)
			{
				PlayRaceCrewTalkSoundEvent(CrewTalkSoundType.Offroading);
				_startTimeOffRoad = 0f;
			}
		}

		public void PlayWrongWaySoundEvent(bool state)
		{
			if (!state)
			{
				_startTimeWrongWay = 0f;
				return;
			}
			if (_startTimeWrongWay == 0f)
			{
				_startTimeWrongWay = Time.time;
			}
			if (Time.time - _startTimeWrongWay > 5f)
			{
				PlayRaceCrewTalkSoundEvent(CrewTalkSoundType.Wrong_Way);
				_startTimeWrongWay = 0f;
			}
		}

		public void PlayStartRaceSoundEvent()
		{
			Observable.Timer(TimeSpan.FromSeconds(5.0)).Subscribe((long _) =>
			{
				PlayRaceCrewTalkSoundEvent(CrewTalkSoundType.Race_Start);
			}).AddTo(_circuitRaceController);
		}

		public void PlayPitStopSoundEvent(MaintenanceType maintenanceId)
		{
			switch (maintenanceId)
			{
			case MaintenanceType.Fuel:
				PlayRaceCrewTalkSoundEvent(CrewTalkSoundType.Pit_Fuel);
				break;
			case MaintenanceType.Wheel:
				PlayRaceCrewTalkSoundEvent(CrewTalkSoundType.Pit_Wheels);
				break;
			case MaintenanceType.None:
				if (_fixedCarOnPitStop)
				{
					PlayRaceCrewTalkSoundEvent(CrewTalkSoundType.Pit_Completed);
					_fixedCarOnPitStop = false;
				}
				break;
			case MaintenanceType.AllFix:
				break;
			}
		}

		public void PlayFinishLemanSoundEvent(LapRaceCar car, bool isTimeExpired)
		{
			if (car.IsUser())
			{
				if (isTimeExpired)
				{
					if (car.place > 2)
					{
						PlayFinishRaceSoundEvent(car);
					}
					else
					{
						PlayRaceCrewTalkSoundEvent(CrewTalkSoundType.Finish_Good_Result);
					}
				}
				else
				{
					PlayRaceCrewTalkSoundEvent(CrewTalkSoundType.You_Finish_Another_Lap);
				}
			}
			else if (isTimeExpired)
			{
				PlayRaceCrewTalkSoundEvent(CrewTalkSoundType.Leader_Finish_Last_Lap);
			}
			else
			{
				PlayRaceCrewTalkSoundEvent(CrewTalkSoundType.Leader_Finish_Another_Lap);
			}
		}

		public void PlayFinishRaceSoundEvent(LapRaceCar car)
		{
			if (car.place > 2)
			{
				PlayRaceCrewTalkSoundEvent(CrewTalkSoundType.Finish_Bad_Result);
			}
			else
			{
				PlayRaceCrewTalkSoundEvent(CrewTalkSoundType.Finish_Good_Result);
			}
		}

		public void Dispose()
		{
			_soundBankLoadService?.TryUnloadBank(SoundBankType.CpmEventVoices);
		}
	}
}

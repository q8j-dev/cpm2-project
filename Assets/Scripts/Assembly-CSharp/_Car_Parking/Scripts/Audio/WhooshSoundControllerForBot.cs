using System;
using System.Collections.Generic;
using CPMEngine;
using UniRx;
using UnityEngine;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.EngineTuning;

namespace _Car_Parking.Scripts.Audio
{
	public class WhooshSoundControllerForBot
	{
		public const float MaxDistanceToCheck = 15f;

		private const float MaxDistanceToPlayWhooshSqr = 225f;

		private static readonly uint _playWindPassbySoundFastEvent = AudioWrapper.GetId("Play_wind_passby_fast");

		private static readonly uint _playWindPassbySoundSlowEvent = AudioWrapper.GetId("Play_wind_passby_slow");

		private static readonly uint _playF1V6Flyby = AudioWrapper.GetId("Play_f1_v6_flyby");

		private static readonly uint _playF1V8Flyby = AudioWrapper.GetId("Play_f1_v8_flyby");

		private static readonly uint _playF1V10Flyby = AudioWrapper.GetId("Play_f1_v10_flyby");

		private static readonly uint _playF1V12Flyby = AudioWrapper.GetId("Play_f1_v12_flyby");

		private Dictionary<EngineType, uint> _soundEventsWhooshF1;

		private bool _whooshPlaying;

		private Transform _car;

		private int _trafficCarLayer;

		private VehicleEntityProvider _vehicleEntityProvider;

		private VehicleStockTuningData _carTuningDataConfig;

		public WhooshSoundControllerForBot(Transform car)
		{
			_car = car;
			_trafficCarLayer = LayerViaName.TrafficLayer;
			VehicleData vehicleData = (_car.parent ? _car.parent.GetComponent<VehicleData>() : null);
			if ((bool)vehicleData)
			{
				_carTuningDataConfig = CarTuningFactory.GetDefaultCarTuningDataConfig(vehicleData.CarId);
				_vehicleEntityProvider = _car.parent.GetComponent<VehicleEntityProvider>();
				_soundEventsWhooshF1 = new Dictionary<EngineType, uint>
				{
					{
						EngineType.V6,
						_playF1V6Flyby
					},
					{
						EngineType.V8,
						_playF1V8Flyby
					},
					{
						EngineType.V10,
						_playF1V10Flyby
					},
					{
						EngineType.V12,
						_playF1V12Flyby
					}
				};
			}
		}

		public void PlayWhoosh(Vector3 listenerSpeed, Vector3 emitterSpeed)
		{
			if (_whooshPlaying || (_car.gameObject.layer != _trafficCarLayer && (!_carTuningDataConfig || !_vehicleEntityProvider)))
			{
				return;
			}
			float num = (listenerSpeed - emitterSpeed).magnitude * 3.6f;
			if (num < 60f || !((_car.transform.position - V.CoreGameManager.transform.position).sqrMagnitude < 225f))
			{
				return;
			}
			uint eventId = ((num >= 100f) ? _playWindPassbySoundFastEvent : _playWindPassbySoundSlowEvent);
			if ((bool)_carTuningDataConfig && _carTuningDataConfig.GetClass == VehicleClass.F1)
			{
				EngineType engineType = EngineType.None;
				if ((bool)_vehicleEntityProvider && _vehicleEntityProvider.CarEntity != null)
				{
					engineType = (EngineType)(int)_vehicleEntityProvider.CarEntity.Engine.GetEngineType;
				}
				if (engineType == EngineType.None && (bool)_carTuningDataConfig)
				{
					EngineKitOffer engineKitOffer = _carTuningDataConfig.Tuning.EngineKitOffer;
					if ((bool)engineKitOffer && (bool)engineKitOffer.Base)
					{
						engineType = engineKitOffer.Base.GetTypeEngine;
					}
				}
				if (_soundEventsWhooshF1.TryGetValue(engineType, out var value))
				{
					eventId = value;
				}
			}
			AudioWrapper.PostEvent(eventId, _car.gameObject);
			_whooshPlaying = true;
			Observable.Timer(TimeSpan.FromSeconds(0.5)).Subscribe((long _) =>
			{
				_whooshPlaying = false;
			}).AddTo(_car);
		}
	}
}

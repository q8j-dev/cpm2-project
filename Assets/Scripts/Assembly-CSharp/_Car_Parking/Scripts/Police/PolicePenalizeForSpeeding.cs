using System;
using CPM.Database;
using Photon.Pun;
using UnityEngine;
using _Car_Parking.Scripts.Car;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Multiplayer.RpcMethods.RpcObjects;

namespace _Car_Parking.Scripts.Police
{
	public class PolicePenalizeForSpeeding : IDisposable
	{
		private bool _policeInstalled;

		private GameObject _ourCar;

		private VehicleData _vehicleData;

		private Camera _mainCam;

		private Ray _ray;

		private readonly LayerMask _carLayer;

		private PoliceSystemConfig _policeConfig;

		private float _lastPenaltyTime;

		public PoliceSystemConfig PoliceConfig => _policeConfig;

		public bool JustPenalized => Time.time - _lastPenaltyTime < _policeConfig.PenaltySystem.DelayToNextPenalty;

		public PolicePenalizeForSpeeding(PoliceSystemConfig policeConfig)
		{
			_mainCam = Camera.main;
			_carLayer = LayerMask.GetMask("car");
			_policeConfig = policeConfig;
		}

		public void Dispose()
		{
			RemoveListeners();
		}

		public void Update()
		{
			if (FlagsStorage.IsMultiFree && !FlagsStorage.OutOfCar)
			{
				if (_ourCar != V.Car)
				{
					Init();
				}
				_ = _policeInstalled;
			}
		}

		public void ResetPenaltyTime()
		{
			_lastPenaltyTime = Time.time;
		}

		private void PenalizeForSpeeding(GameObject otherCar)
		{
			if (!FlagsStorage.IsSpectatorMode && otherCar.TryGetComponent<MultiPlayerPosControl>(out var component) && component.speeding)
			{
				PhotonView component2;
				if (!_vehicleData.PoliceCarController.PoliceCarSirenControllerInGameplay.SirenActive)
				{
					V.CoreGameManager.SetMessage(new LocalizedString("FreeDriveControl.TurnOnSiren"), 3f);
				}
				else if (Vector3.Distance(otherCar.transform.position, V.DriverPerson.transform.position) > _policeConfig.PenaltySystem.MaxDistance)
				{
					V.CoreGameManager.SetMessage(new LocalizedString("FreeDriveControl.CloserToCar"), 3f);
				}
				else if (otherCar.TryGetComponent<PhotonView>(out component2) && component2.Owner != null)
				{
					V.CoreGameManager.FreeDriveProvider.MultiPlayer.SendValueStrToPlayer<RpcPolicePenaltyRequest>(component2.ViewID.ToString(), PhotonNetwork.LocalPlayer.UserId, component2.Owner.UserId);
					V.SceneControl.Whistle.transform.position = V.Car.transform.position;
					V.SceneControl.Whistle.Play();
				}
			}
		}

		private void Init()
		{
			_policeInstalled = false;
			RemoveListeners();
			_vehicleData = null;
			_ourCar = V.Car;
			if ((bool)_ourCar && _ourCar.TryGetComponent<VehicleData>(out _vehicleData))
			{
				_vehicleData.CarInfoProvider.OnReady += OnCarInfoReady;
				if (_vehicleData.CarInfoProvider.IsReady)
				{
					OnCarInfoReady(_vehicleData.CarInfoProvider, _vehicleData.CarInfoProvider.CarInfo);
				}
			}
		}

		private void OnCarInfoReady(CarInfoProvider carInfoGetterController, CarInfo carInfo)
		{
			_policeInstalled = carInfo.AdditionalData.Police.Installed;
		}

		private void RemoveListeners()
		{
			if ((bool)_vehicleData)
			{
				_vehicleData.CarInfoProvider.OnReady -= OnCarInfoReady;
			}
		}

		public void AimAtCar(GameObject otherCar)
		{
			if (_policeInstalled)
			{
				PenalizeForSpeeding(otherCar);
			}
		}
	}
}

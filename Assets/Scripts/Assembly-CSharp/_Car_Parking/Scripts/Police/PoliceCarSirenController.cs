using UniRx;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.Audio;

namespace _Car_Parking.Scripts.Police
{
	internal class PoliceCarSirenController : MonoBehaviour
	{
		[SerializeField]
		private PoliceCarController PoliceCarController;

		private bool _sirenActive;

		private IPoliceSystem _policeSystem;

		private PoliceSystemConfig _policeSystemConfig;

		private VehicleData _vehicleData;

		private int _tempSirenSoundIndex = -1;

		public bool SirenActive => _sirenActive;

		private PoliceCarSirenMuteController MuteController => PoliceCarController.MuteController;

		private PoliceCarBodyKitsUpdater PoliceCarBodyKitsUpdater => PoliceCarController.BodyKitsUpdater;

		private int SirenSoundIndex
		{
			get
			{
				if (_tempSirenSoundIndex >= 0)
				{
					return _tempSirenSoundIndex;
				}
				if ((bool)_vehicleData)
				{
					return _vehicleData.CarInfoProvider.CarInfo.AdditionalData.Police.SirenSound.InstalledIndex;
				}
				return 0;
			}
		}

		[Inject]
		public void Construct(IPoliceSystem policeSystem, PoliceSystemConfig policeSystemConfig)
		{
			_policeSystem = policeSystem;
			_policeSystemConfig = policeSystemConfig;
		}

		public void Init(VehicleData vehicleData)
		{
			_vehicleData = vehicleData;
		}

		public void SetTempSirenSoundIndex(int soundIndex)
		{
			_tempSirenSoundIndex = soundIndex;
		}

		public void RemoveTempSirenSound()
		{
			_tempSirenSoundIndex = -1;
		}

		public void StartSirens()
		{
			StartSirens(!MuteController.IsMute.Value);
		}

		public void StartSirens(bool playSound)
		{
			_sirenActive = true;
			if (base.gameObject.activeInHierarchy)
			{
				StartSirensManually(playSound);
			}
		}

		public void StopSirens()
		{
			_sirenActive = false;
			for (int i = 0; i < PoliceCarBodyKitsUpdater.PoliceCarBodyKits.Count; i++)
			{
				PoliceCarBodyKitsUpdater.PoliceCarBodyKits[i].StopLampAnimation();
			}
			StopSirenSound();
		}

		public void StartSirenSound()
		{
			StopSirenSound();
			string arg = ((SirenSoundIndex >= 2) ? "s" : "");
			AudioWrapper.PostEvent($"Play_Police_Siren{arg}_{SirenSoundIndex + 1}", base.gameObject);
		}

		public void StopSirenSound()
		{
			AudioWrapper.StopAll(base.gameObject);
		}

		private void OnEnable()
		{
			if (SirenActive)
			{
				StartSirens();
			}
			MuteController.IsMute.ObserveEveryValueChanged((ReactiveProperty<bool> x) => x.Value).Subscribe((bool x) =>
			{
				OnMuteUpdated(x);
			}).AddTo(this);
		}

		private void OnDisable()
		{
			StopSirensManually();
		}

		private void OnMuteUpdated(bool mute)
		{
			if (mute)
			{
				StopSirenSound();
			}
			else if (_sirenActive)
			{
				StartSirenSound();
			}
		}

		private void StartSirensManually(bool playSound)
		{
			for (int i = 0; i < PoliceCarBodyKitsUpdater.PoliceCarBodyKits.Count; i++)
			{
				PoliceCarBodyKitsUpdater.PoliceCarBodyKits[i].StartLampAnimation();
			}
			if (playSound)
			{
				StartSirenSound();
			}
		}

		private void StopSirensManually()
		{
			for (int i = 0; i < PoliceCarBodyKitsUpdater.PoliceCarBodyKits.Count; i++)
			{
				PoliceCarBodyKitsUpdater.PoliceCarBodyKits[i].StopLampAnimation();
			}
			StopSirenSound();
		}
	}
}

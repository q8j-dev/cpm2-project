using System;
using CPM.Database;
using Cysharp.Threading.Tasks;
using VContainer;
using _Car_Parking.Scripts.CustomDataTypes;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Police;
using _Car_Parking.Scripts.Wallet;

namespace _Car_Parking.Scripts.MenuUI.VisualTuning
{
	public class PoliceVisualScope : VisualTuningScope
	{
		private VehicleData _vehicleData;

		private PoliceCarBodyKitsConfig.BodyKitData _selectedBodyKitData;

		private PoliceSystemConfig _policeSystemConfig;

		private ManagerUI _managerUI;

		private CurrencyManagerController _walletController;

		private IPoliceSystem _policeSystem;

		private int _selectedSirenSoundIndex;

		public int SelectedSirenSoundIndex => _selectedSirenSoundIndex;

		public PoliceCarBodyKitsConfig.BodyKitData SelectedBodyKitData => _selectedBodyKitData;

		private Array AllBodyKitTypes => Enum.GetValues(typeof(PoliceBodyKitType));

		private CarInfo CarInfo
		{
			get
			{
				if (!_vehicleData)
				{
					return null;
				}
				return _vehicleData.CarInfoProvider.CarInfo;
			}
		}

		[Inject]
		public PoliceVisualScope(CurrencyManagerController walletController, ManagerUI managerUI, PoliceSystemConfig policeSystemConfig, IPoliceSystem policeSystem)
		{
			_policeSystemConfig = policeSystemConfig;
			_managerUI = managerUI;
			_walletController = walletController;
			_policeSystem = policeSystem;
		}

		public override void Init()
		{
			_vehicleData = null;
			_selectedBodyKitData = null;
			_selectedSirenSoundIndex = 0;
			if ((bool)V.Car)
			{
				_vehicleData = V.Car.GetComponent<VehicleData>();
				_selectedSirenSoundIndex = _vehicleData.CarInfoProvider.CarInfo.AdditionalData.Police.SirenSound.InstalledIndex;
			}
			InvokeOnValuesChanged();
		}

		public void UpdateSelectedSirenSound(int slideSirenSoundIndex)
		{
			if ((bool)_vehicleData)
			{
				_selectedSirenSoundIndex += slideSirenSoundIndex;
				if (_selectedSirenSoundIndex < 0)
				{
					_selectedSirenSoundIndex = _policeSystemConfig.SirenSoundsCount - 1;
				}
				else if (_selectedSirenSoundIndex >= _policeSystemConfig.SirenSoundsCount)
				{
					_selectedSirenSoundIndex = 0;
				}
				_vehicleData.PoliceCarController.PoliceCarSirenControllerInTuning.SetTempSoundIndex(_selectedSirenSoundIndex);
				_vehicleData.PoliceCarController.PoliceCarSirenControllerInTuning.StartAnimation(playSound: true);
			}
		}

		public override void Apply()
		{
			if (_selectedBodyKitData == null || CarInfo == null)
			{
				return;
			}
			PoliceBodyKitSettingsConfig.BodyKitSettings bodyKitSettings = _policeSystemConfig.GetBodyKitSettings(_selectedBodyKitData);
			if (bodyKitSettings != null)
			{
				if (_policeSystem.BoughtBodyKit.Contains(_selectedBodyKitData) || (int)bodyKitSettings.Price == 0)
				{
					InstallCurrentBodyKit();
				}
				else
				{
					TryBuyBodyKitAsync().Forget();
				}
			}
		}

		public void ApplySirenSound()
		{
			PoliceSystemConfig.SirenSoundData sirenSound = _policeSystemConfig.GetSirenSound(_selectedSirenSoundIndex);
			if (CarInfo != null && sirenSound != null)
			{
				if (_policeSystem.BoughtSiren.Contains(_selectedSirenSoundIndex) || (int)sirenSound.Price == 0)
				{
					InstallCurrentSirenSound();
				}
				else
				{
					TryBuySirenSoundAsync().Forget();
				}
			}
		}

		public override void Revert()
		{
			if ((bool)_vehicleData)
			{
				_vehicleData.PoliceCarController.BodyKitsSetter.SetDefaultBodyKits();
				_vehicleData.PoliceCarController.PoliceCarSirenControllerInTuning.RemoveTempSirenSound();
				_selectedBodyKitData = null;
				_selectedSirenSoundIndex = _vehicleData.CarInfoProvider.CarInfo.AdditionalData.Police.SirenSound.InstalledIndex;
				InvokeOnValuesChanged();
			}
		}

		public override void Discard()
		{
			if (_selectedBodyKitData != null && CarInfo != null && CarInfo.AdditionalData.Police.InstalledBodyKits.GetIndex(_selectedBodyKitData.BodyKitType) == _selectedBodyKitData.Index)
			{
				CarInfo.AdditionalData.Police.InstalledBodyKits.SetIndex(_selectedBodyKitData.BodyKitType, -1);
				if (!CarInfo.AdditionalData.Police.InstalledBodyKits.HaveAnyInstalledBodyKit())
				{
					CarInfo.AdditionalData.Police.Installed = false;
				}
				_vehicleData.PoliceCarController.BodyKitsUpdater.SetBodyKit(_selectedBodyKitData.BodyKitType, CarInfo.AdditionalData.Police.InstalledBodyKits.GetIndex(_selectedBodyKitData.BodyKitType));
				_selectedBodyKitData = null;
				InvokeOnValuesChanged();
			}
			else
			{
				Revert();
			}
		}

		public override void Select(int index)
		{
		}

		public void SelectBodyKit(PoliceCarBodyKitsConfig.BodyKitData bodyKitData)
		{
			if (bodyKitData == null || !_vehicleData)
			{
				return;
			}
			_selectedBodyKitData = bodyKitData;
			foreach (PoliceBodyKitType allBodyKitType in AllBodyKitTypes)
			{
				if (allBodyKitType == _selectedBodyKitData.BodyKitType)
				{
					_vehicleData.PoliceCarController.BodyKitsUpdater.SetBodyKit(_selectedBodyKitData.BodyKitType, _selectedBodyKitData.Index);
					continue;
				}
				int index = CarInfo.AdditionalData.Police.InstalledBodyKits.GetIndex(allBodyKitType);
				if (index >= 0)
				{
					_vehicleData.PoliceCarController.BodyKitsUpdater.SetBodyKit(allBodyKitType, index);
				}
				else
				{
					_vehicleData.PoliceCarController.BodyKitsUpdater.RemoveBodyKit(allBodyKitType);
				}
			}
			_vehicleData.PoliceCarController.PoliceCarSirenControllerInTuning.StartAnimation(playSound: false);
			InvokeOnValuesChanged();
		}

		private async UniTaskVoid TryBuyBodyKitAsync()
		{
			if (_selectedBodyKitData == null)
			{
				return;
			}
			PoliceBodyKitSettingsConfig.BodyKitSettings bodyKitSettings = _policeSystemConfig.GetBodyKitSettings(_selectedBodyKitData);
			if (bodyKitSettings != null)
			{
				bool canBuy = false;
				AdvancedInt price = new AdvancedInt(bodyKitSettings.Price);
				await _walletController.TryCoinsSpend(SpendCoinsType.Others, price, () =>
				{
					canBuy = true;
				}, null, allowZero: false);
				if (canBuy && CarInfo != null && _selectedBodyKitData != null)
				{
					_policeSystem.BoughtBodyKit.AddBought(_selectedBodyKitData);
					InstallCurrentBodyKit();
					_managerUI.ShowNotificationWindow(new LocalizedString("BodyKitsVisualScope.Detailinstalled!"), 3f).Forget();
				}
				_selectedBodyKitData = null;
				InvokeOnValuesChanged();
			}
		}

		private async UniTaskVoid TryBuySirenSoundAsync()
		{
			PoliceSystemConfig.SirenSoundData sirenSound = _policeSystemConfig.GetSirenSound(_selectedSirenSoundIndex);
			if (CarInfo != null && sirenSound != null)
			{
				bool canBuy = false;
				AdvancedInt price = new AdvancedInt(sirenSound.Price);
				await _walletController.TryCoinsSpend(SpendCoinsType.Others, price, () =>
				{
					canBuy = true;
				}, null, allowZero: false);
				if (canBuy && CarInfo != null)
				{
					_policeSystem.BoughtSiren.AddBought(_selectedSirenSoundIndex);
					InstallCurrentSirenSound();
					CarInfo.Save();
					_managerUI.ShowNotificationWindow(new LocalizedString("BodyKitsVisualScope.Detailinstalled!"), 3f).Forget();
				}
				InvokeOnValuesChanged();
			}
		}

		private void InstallCurrentBodyKit()
		{
			if (CarInfo != null && _selectedBodyKitData != null)
			{
				CarInfo.AdditionalData.Police.InstalledBodyKits.SetIndex(_selectedBodyKitData);
				CarInfo.AdditionalData.Police.Set(_policeSystem, CarInfo.CarID, isInstall: true);
				CarInfo.Save();
				_selectedBodyKitData = null;
				InvokeOnValuesChanged();
			}
		}

		private void InstallCurrentSirenSound()
		{
			if (CarInfo != null)
			{
				CarInfo.AdditionalData.Police.SirenSound.Set(_selectedSirenSoundIndex);
				CarInfo.Save();
			}
		}
	}
}

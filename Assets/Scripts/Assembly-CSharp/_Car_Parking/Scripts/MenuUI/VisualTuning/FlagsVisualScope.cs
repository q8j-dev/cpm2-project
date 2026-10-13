using System.Runtime.CompilerServices;
using CPM.Database;
using CPM.Database.Constructors;
using Cinemachine;
using CodeStage.AntiCheat.Storage;
using Cysharp.Threading.Tasks;
using Flags;
using Racing.CameraManagement;
using Racing.CameraManagement.SpecificTypes;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Wallet;

namespace _Car_Parking.Scripts.MenuUI.VisualTuning
{
	public class FlagsVisualScope : VisualTuningScope
	{
		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass17_0
		{
			public bool toIgnore;

			internal void _003CSetCamera_003Eb__0(CinemachineVirtualCameraBase vc)
			{
				vc.GetComponent<VisualTuningCameraHandler>().SetCamMode(VisualTuningCamMode.Flags, toIgnore);
			}
		}

		private const int NoFlagId = 0;

		private CurrencyManagerController _currencyManagerController;

		private FlagsDataSO _flagsDataSo;

		private GameObject _car;

		private VehicleData _vehicleData;

		private int _lastSelectedFlagIndex;

		private CameraManager _cameraManager;

		private MenuControl _menuControl;

		private ManagerUI _managerUI;

		[Inject]
		public void Construct(CurrencyManagerController currencyManagerController, FlagsDataSO flagsDataSo, CameraManager cameraManager, MenuControl menuControl, ManagerUI managerUI)
		{
			_managerUI = managerUI;
			_menuControl = menuControl;
			_cameraManager = cameraManager;
			_flagsDataSo = flagsDataSo;
			_currencyManagerController = currencyManagerController;
		}

		public override void Init()
		{
			_car = V.Car;
			_vehicleData = _car.GetComponent<VehicleData>();
			FlagConstructor.GetFlagsData();
			SetCamera();
		}

		public override void Select(int index)
		{
			VehicleData component = _car.GetComponent<VehicleData>();
			_lastSelectedFlagIndex = index;
			component.FlagConstructor.SetFlag(component, _lastSelectedFlagIndex);
		}

		public override void Apply()
		{
			int flagId = _vehicleData.carInfo.AdditionalData.FlagId;
			if (flagId != _lastSelectedFlagIndex)
			{
				if (!FlagConstructor.FlagsBought.ContainsKey(flagId))
				{
					FlagConstructor.FlagsBought[flagId] = 0;
				}
				FlagConstructor.FlagsBought[flagId]++;
				if (IsFlagUnlocked(_lastSelectedFlagIndex))
				{
					FlagConstructor.FlagsBought[_lastSelectedFlagIndex]--;
				}
				else if (_lastSelectedFlagIndex != -1 && _lastSelectedFlagIndex != 0)
				{
					TryBuyFlag();
					return;
				}
				SaveFlagsData();
			}
			else
			{
				SaveFlagsData();
			}
		}

		public override void Revert()
		{
			RevertFlag(_car ? _car.GetComponent<VehicleData>() : null);
		}

		public override void Discard()
		{
			int num = (_vehicleData ? _vehicleData.carInfo.AdditionalData.FlagId : 0);
			if (num == 0 || _lastSelectedFlagIndex != num)
			{
				base.Discard();
				return;
			}
			FlagConstructor.AddFlag(num);
			if (IsFlagUnlocked(0))
			{
				FlagConstructor.FlagsBought[0]--;
			}
			_lastSelectedFlagIndex = 0;
			SetFlagAndSave(_vehicleData, 0);
			FlagConstructor.SaveBoughtFlags();
			InvokeOnValuesChanged();
		}

		private static void RevertFlag(VehicleData vehicleData)
		{
			if ((bool)vehicleData)
			{
				int flagId = vehicleData.carInfo.AdditionalData.FlagId;
				vehicleData.FlagConstructor.SetFlag(vehicleData, flagId);
			}
		}

		private void SetCamera()
		{
			_003C_003Ec__DisplayClass17_0 obj = new _003C_003Ec__DisplayClass17_0();
		}

		private bool IsFlagUnlocked(int id)
		{
			if (FlagConstructor.FlagsBought != null && FlagConstructor.FlagsBought.ContainsKey(id))
			{
				return FlagConstructor.FlagsBought[id] > 0;
			}
			return false;
		}

		private static void SetFlagAndSave(VehicleData vehicleData, int flagIndex)
		{
			FlagConstructor.AddFlag(flagIndex);
			if ((bool)vehicleData)
			{
				vehicleData.carInfo.AdditionalData.FlagId = flagIndex;
				vehicleData.carInfo.Save();
				vehicleData.FlagConstructor.SetFlag(vehicleData, flagIndex);
			}
		}

		private async UniTask<bool> TryBuyFlag()
		{
			int flagIndex = _lastSelectedFlagIndex;
			GameObject car = _car;
			VehicleData vehicleData = _vehicleData;
			return await VisualItemPurchase.PurchaseAsync(_currencyManagerController, SpendCoinsType.Flags, flagIndex, FlagConstructor.GetCoinPriceForFlag(), alreadyOwned: false, () =>
			{
				SaveBoughtFlag(flagIndex, car);
				SaveFlagsData(vehicleData, flagIndex);
			}, () =>
			{
				RevertFlag(vehicleData);
			});
		}

		private void SaveFlagsData()
		{
			SaveFlagsData(_vehicleData, _lastSelectedFlagIndex);
		}

		private void SaveFlagsData(VehicleData vehicleData, int flagIndex)
		{
			_managerUI.ShowNotificationWindow(new LocalizedString("FlagsVisualScope.Detailinstalled!"), 3f).Forget();
			SetFlagAndSave(vehicleData, flagIndex);
			FlagConstructor.SaveBoughtFlags();
			InvokeOnValuesChanged();
		}

		private static void SaveBoughtFlag(int flagIndex, GameObject car)
		{
			ObscuredPrefs.SetBool("wasFlagBought" + flagIndex + "_atCar_" + car, value: true);
		}
	}
}

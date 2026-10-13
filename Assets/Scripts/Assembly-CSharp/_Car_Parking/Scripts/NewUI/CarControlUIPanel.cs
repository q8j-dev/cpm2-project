using System.Collections.Generic;
using CPM.Scripts.Garage;
using CPMEngine;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.EngineTuning;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Providers;

namespace _Car_Parking.Scripts.NewUI
{
	public class CarControlUIPanel : PhoneUISubPanel
	{
		[Header("Texts")]
		[SerializeField]
		private TextMeshProUGUI TitleText;

		[SerializeField]
		private TextMeshProUGUI NotAvalibleText;

		[Header("Tabs")]
		[SerializeField]
		private PneumaticCarControlUIPart PneumaticTab;

		[SerializeField]
		private ShockAbsorberCarControlUIPart ShockAbsorberTab;

		[SerializeField]
		private GameObject NotAvalibleTab;

		[Header("Toggles")]
		[SerializeField]
		private Toggle PneumaticToggle;

		[SerializeField]
		private Toggle ShockAbsorberToggle;

		[SerializeField]
		private GameObject PersonViewJoystickHandlerPrefab;

		private Dictionary<PhoneCarControlUIPanelType, PhoneUISubPanel> _tabsDictionary;

		private FreeDriveProvider _freeDriveProvider;

		private Powertrain _powertrain;

		private VehicleEntityProvider _vehicleEntityProvider;

		private VehicleData _vehicleData;

		private VehicleStockTuningData _vehicleStockTuningData;

		private bool _isOtherUser;

		private IPrefabFactory _prefabFactory;

		private GameObject _personViewJoystickHandler;

		private GarageController _garageController;

		public VehicleData GetVehicleData
		{
			get
			{
				if ((bool)_vehicleData)
				{
					return _vehicleData;
				}
				if ((bool)GetCar && GetCar.TryGetComponent<VehicleData>(out var component))
				{
					_vehicleData = component;
				}
				return _vehicleData;
			}
		}

		public VehicleEntityProvider GetVehicleEntityProvider
		{
			get
			{
				if ((bool)_vehicleEntityProvider)
				{
					return _vehicleEntityProvider;
				}
				if ((bool)GetCar && GetCar.TryGetComponent<VehicleEntityProvider>(out var component))
				{
					_vehicleEntityProvider = component;
				}
				return _vehicleEntityProvider;
			}
		}

		public Powertrain GetChassis
		{
			get
			{
				if ((bool)_powertrain)
				{
					return _powertrain;
				}
				if ((bool)GetCar && GetCar.TryGetComponent<Powertrain>(out var component))
				{
					_powertrain = component;
				}
				return _powertrain;
			}
		}

		private GameObject GetCar
		{
			get
			{
				if (FlagsStorage.IsMultiFree)
				{
					return _freeDriveProvider.FirstCarMultiPlayer;
				}
				if (!V.Car)
				{
					return V.LastUsedCar;
				}
				return V.Car;
			}
		}

		[Inject]
		public void Construct(FreeDriveProvider freeDriveProvider, IPrefabFactory prefabFactory, ManagerUI managerUI, GarageController garageController)
		{
			_freeDriveProvider = freeDriveProvider;
			_prefabFactory = prefabFactory;
			_garageController = garageController;
		}

		public override void OnEnableEvent()
		{
			base.OnEnableEvent();
		}

		private void Start()
		{
			_tabsDictionary = new Dictionary<PhoneCarControlUIPanelType, PhoneUISubPanel>
			{
				{
					PhoneCarControlUIPanelType.Pneumatics,
					PneumaticTab
				},
				{
					PhoneCarControlUIPanelType.ShockAbsorbers,
					ShockAbsorberTab
				}
			};
			if (!GetVehicleData)
			{
				ShowNotAvaliblePanel(new LocalizedString("Phone.CarControlsNotAvalible"));
				return;
			}
			_vehicleStockTuningData = CarTuningFactory.GetDefaultCarTuningDataConfig(GetVehicleData.CarId);
			PneumaticTab.PublishDatas(this);
			ShockAbsorberTab.PublishDatas(this);
			_isOtherUser = FlagsStorage.IsMultiFree && (bool)V.Car && GetCar != V.Car;
			PhoneCarControlUIPanelType phoneCarControlUIPanelType = PhoneCarControlUIPanelType.ShockAbsorbers;
			ShowSuspensionTab(phoneCarControlUIPanelType);
			SetCategoryToggles(phoneCarControlUIPanelType);
			EnableWheelsPhysics(enable: true);
		}

		public override void Show()
		{
			base.Show();
			if (!GetVehicleData)
			{
				ShowNotAvaliblePanel(new LocalizedString("Phone.CarControlsNotAvalible"));
				return;
			}
			if (FlagsStorage.OutOfCar)
			{
				GetVehicleData.GetPowertrain.ResetTransmission();
				_personViewJoystickHandler = _prefabFactory.Instantiate(PersonViewJoystickHandlerPrefab, ManagerUI.GetWindowsParent());
				_personViewJoystickHandler.transform.SetSiblingIndex(0);
			}
			InitBackButton();
		}

		private void EnableWheelsPhysics(bool enable)
		{
			if ((bool)GetCar && (bool)_garageController)
			{
				_garageController.EnableCarPhysics(GetVehicleData, enable);
			}
		}

		private void SetCategoryToggles(PhoneCarControlUIPanelType currentPanelType)
		{
			PneumaticToggle.SetIsOnWithoutNotify(currentPanelType == PhoneCarControlUIPanelType.Pneumatics);
			ShockAbsorberToggle.SetIsOnWithoutNotify(currentPanelType == PhoneCarControlUIPanelType.ShockAbsorbers);
		}

		public void ShowNotAvaliblePanel(string text)
		{
			NotAvalibleTab.SetActive(value: true);
			NotAvalibleText.text = text;
		}

		public void ShowSuspensionTab(PhoneCarControlUIPanelType currentPanelType)
		{
			NotAvalibleTab.SetActive(value: false);
			if (_isOtherUser)
			{
				ShowNotAvaliblePanel(new LocalizedString("Phone.CarControlsNotAvalible"));
				CanvasGroup.interactable = false;
				return;
			}
			foreach (KeyValuePair<PhoneCarControlUIPanelType, PhoneUISubPanel> item in _tabsDictionary)
			{
				item.Value.HideWithoutAnimation();
			}
			CanvasGroup.interactable = true;
			_tabsDictionary[currentPanelType].Show();
			TitleText.text = Utils.GetPhoneCarControlTitle(currentPanelType);
		}

		private PhoneCarControlUIPanelType GetFirstWindow()
		{
			if (GetVehicleData.carInfo.AdditionalData.AirSuspension.Bought && GetVehicleData.carInfo.AdditionalData.AirSuspension.Installed)
			{
				return PhoneCarControlUIPanelType.Pneumatics;
			}
			return PhoneCarControlUIPanelType.ShockAbsorbers;
		}

		public override void Close()
		{
			if ((bool)_personViewJoystickHandler)
			{
				Object.Destroy(_personViewJoystickHandler);
			}
			EnableWheelsPhysics(enable: true);
			base.Close();
		}
	}
}

using CPM.Database;
using CPMEngine;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.CustomDataTypes;
using _Car_Parking.Scripts.EngineTuning;
using _Car_Parking.Scripts.GooglePlayServices;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Steam.Base;

namespace _Car_Parking.Scripts.Drag
{
	public class BoostControllerGadget : MonoBehaviour
	{
		private const int MinPressure = 20000;

		private const float DesctructivePercent = 1.15f;

		private int _step = 5000;

		private VehicleEntityProvider _vehicleData;

		[SerializeField]
		private TextMeshProUGUI DisplayText;

		[SerializeField]
		private Button MinusButton;

		[SerializeField]
		private Button PlusButton;

		[SerializeField]
		private Button ResetButton;

		[SerializeField]
		private GameObject Controller;

		[SerializeField]
		private Toggle ControlToggle;

		[SerializeField]
		private DynoWindow DynoWindow;

		[SerializeField]
		private TweenPulseAnimator TweenPulseAnimator;

		private AdvancedInt _optimalBoost;

		private AdvancedInt _maxBoost;

		private AdvancedInt _currentBoost;

		private ICarInfoService _carInfoService;

		private CarInfo _carInfo;

		private ICarTuningFactory _carTuningFactory;

		private ISteamAchievementService _steamAchievementService;

		private AdvancedInt GetDescructivePressure
		{
			get
			{
				_maxBoost.Get(out int value);
				return new AdvancedInt(RoundToStep(Mathf.RoundToInt((float)value * 1.15f)));
			}
		}

		private string ColorTag
		{
			get
			{
				if (_currentBoost <= _optimalBoost)
				{
					DynoWindow.ShowWarningPanel(state: false);
					TweenPulseAnimator.ChangeState(WarningState.None);
					return "<color=#49CC01>";
				}
				DynoWindow.ShowWarningPanel(state: true);
				if (_currentBoost > _optimalBoost && _currentBoost <= _maxBoost)
				{
					TweenPulseAnimator.ChangeState(WarningState.Warning);
					return "<color=#FFB900>";
				}
				TweenPulseAnimator.ChangeState(WarningState.Error);
				if (!(_currentBoost > _maxBoost))
				{
					return string.Empty;
				}
				return "<color=#C80109>";
			}
		}

		[Inject]
		public void Construct(ICarInfoService carInfoService, ICarTuningFactory carTuningFactory, ISteamAchievementService steamAchievementService)
		{
			_carTuningFactory = carTuningFactory;
			_carInfoService = carInfoService;
			_steamAchievementService = steamAchievementService;
		}

		private void Start()
		{
			if ((bool)MinusButton)
			{
				MinusButton.onClick.AddListener(RemoveBoost);
			}
			if ((bool)PlusButton)
			{
				PlusButton.onClick.AddListener(AddBoost);
			}
		}

		private void OnEnable()
		{
			if ((bool)ResetButton)
			{
				ResetButton.onClick.RemoveAllListeners();
				ResetButton.onClick.AddListener(ResetBoost);
			}
		}

		public void Check()
		{
			_vehicleData = V.Car.GetComponent<VehicleEntityProvider>();
			TurboType turboType = _vehicleData.CarEntity.GetValue().TurboType;
			if (turboType == TurboType.NA || turboType == TurboType.Supercharger)
			{
				ControlToggle.interactable = false;
			}
		}

		public void Init()
		{
			if ((bool)_vehicleData && (bool)Controller)
			{
				_maxBoost = _vehicleData.CarEntity.Engine.Charger.GetMaxPressure;
				_optimalBoost = _vehicleData.CarEntity.Engine.Charger.GetPressure;
				AdvancedInt a = ((_vehicleData.CarEntity.Engine.DynamicTuningSettings.Boost > 20000f) ? new AdvancedInt((int)_vehicleData.CarEntity.Engine.DynamicTuningSettings.Boost) : new AdvancedInt(_vehicleData.CarEntity.GetValue().TurboPressure));
				_currentBoost = AdvancedInt.Min(a, GetDescructivePressure);
				UpdateText();
				Controller.SetActive(value: true);
				_carInfo = _carInfoService.GetFromSlot(V.SlotID);
			}
		}

		private void ResetBoost()
		{
			_currentBoost = _optimalBoost;
			UpdateText();
			ApplyBoost();
		}

		private void UpdateText()
		{
			if ((bool)DisplayText)
			{
				DisplayText.text = string.Format("pressure: {0} {1:f2} {2}", ColorTag, Utils.P2Bar(_currentBoost), new LocalizedString("Utils.bar"));
			}
		}

		private int RoundToStep(int pressureBar)
		{
			return (int)Mathf.Round(pressureBar / _step) * _step;
		}

		private void AddBoost()
		{
			_currentBoost = AdvancedInt.Min(_currentBoost + _step, GetDescructivePressure);
			UpdateText();
			ApplyBoost();
			TryUnlockCustomBoost();
		}

		private void RemoveBoost()
		{
			_currentBoost = AdvancedInt.Max(_currentBoost - _step, 20000);
			UpdateText();
			ApplyBoost();
			TryUnlockCustomBoost();
		}

		private void TryUnlockCustomBoost()
		{
			if ((bool)_vehicleData)
			{
				_currentBoost.Get(out int value);
				_optimalBoost.Get(out int value2);
				if (value > 20000 && value != value2)
				{
					GooglePlayAchievements.Unlock("CgkIuKqW0PcZEAIQGQ", "boost_tuning");
				}
			}
		}

		private void ApplyBoost()
		{
			if ((bool)_vehicleData)
			{
				_currentBoost.Get(out int value);
				_vehicleData.CarEntity.Engine.DynamicTuningSettings.Boost = value;
				_vehicleData.CarEntity.RecalculateEngine(_vehicleData, _carTuningFactory, _carInfo);
				_optimalBoost.Get(out int value2);
				if (value != value2)
				{
					_steamAchievementService?.Unlock("ACH_BOOSTED_CAREFULLY");
				}
				Debug.Log($"Pressure {_currentBoost}");
			}
		}
	}
}

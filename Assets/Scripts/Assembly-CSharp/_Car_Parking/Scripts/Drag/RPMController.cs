using CPM.Database;
using CPMEngine;
using CodeStage.AntiCheat.ObscuredTypes;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.Drag.Engine;
using _Car_Parking.Scripts.EngineTuning;
using _Car_Parking.Scripts.GooglePlayServices;
using _Car_Parking.Scripts.Steam.Base;
using _Car_Parking.Scripts.UIElement;

namespace _Car_Parking.Scripts.Drag
{
	public class RPMController : MonoBehaviour
	{
		private VehicleEntityProvider _vehicleData;

		[SerializeField]
		private TextMeshProUGUI DisplayIdleText;

		[SerializeField]
		private TextMeshProUGUI DisplayRedlineText;

		[SerializeField]
		private TextMeshProUGUI DisplayRpmText;

		[SerializeField]
		private Button MinusIdleButton;

		[SerializeField]
		private Button PlusIdleButton;

		[SerializeField]
		private Button MinusRedlineButton;

		[SerializeField]
		private Button PlusRedlineButton;

		[SerializeField]
		private Button ResetButton;

		[SerializeField]
		private Button MinusRpmButton;

		[SerializeField]
		private Button PlusRpmButton;

		[SerializeField]
		private GameObject Launch;

		[SerializeField]
		private GameObject Controller;

		[SerializeField]
		private DynoWindow DynoWindow;

		[SerializeField]
		private TweenPulseAnimator TweenPulseAnimator;

		[SerializeField]
		private RectTransform LayoutRpm;

		private ObscuredFloat _defaultIdleRpm;

		private ObscuredFloat _defaultRedlineRpm;

		private ObscuredFloat _idleMinRpm;

		private ObscuredFloat _idleMaxRpm;

		private ObscuredFloat _minRedlineRpm;

		private ObscuredFloat _maxRedlineRpm;

		private ObscuredFloat _destructiveRedlineRpm;

		private ObscuredFloat _currentRedlineRpm;

		private ObscuredFloat _currentIdleRpm;

		private ObscuredFloat _currentLaunchRpm;

		private ObscuredFloat _LaunchMinRpm;

		private ObscuredFloat _LaunchMaxRpm;

		private ObscuredFloat _defaultLaunchRpm;

		private ICarTuningFactory _carTuningFactory;

		private ICarInfoService _carInfoService;

		private CarInfo _carInfo;

		private ISteamAchievementService _steamAchievementService;

		private string RpmColorTag => "<color=#F8F8F8>";

		private string IdleColorTag
		{
			get
			{
				if ((float)_currentIdleRpm < (float)_idleMinRpm)
				{
					return "<color=#FFB900>";
				}
				return "<color=#F8F8F8>";
			}
		}

		private string RedlineColorTag
		{
			get
			{
				if ((float)_currentRedlineRpm <= (float)_defaultRedlineRpm)
				{
					DynoWindow.ShowWarningPanel(state: false);
					TweenPulseAnimator.ChangeState(WarningState.None);
					return "<color=#F8F8F8>";
				}
				DynoWindow.ShowWarningPanel(state: true);
				if ((float)_currentRedlineRpm > (float)_defaultRedlineRpm && (float)_currentRedlineRpm < (float)_destructiveRedlineRpm)
				{
					TweenPulseAnimator.ChangeState(WarningState.Warning);
					return "<color=#FFB900>";
				}
				TweenPulseAnimator.ChangeState(WarningState.Error);
				if (!((float)_currentRedlineRpm >= (float)_destructiveRedlineRpm))
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
			if ((bool)MinusIdleButton)
			{
				MinusIdleButton.onClick.AddListener(RemoveIdleRpm);
			}
			if ((bool)PlusIdleButton)
			{
				PlusIdleButton.onClick.AddListener(AddIdleRpm);
			}
			if ((bool)MinusRedlineButton)
			{
				MinusRedlineButton.onClick.AddListener(RemoveRedlineRpm);
			}
			if ((bool)PlusRedlineButton)
			{
				PlusRedlineButton.onClick.AddListener(AddRedlineRpm);
			}
			if ((bool)MinusRpmButton)
			{
				MinusRpmButton.onClick.AddListener(RemoveLaunchRpm);
			}
			if ((bool)PlusRpmButton)
			{
				PlusRpmButton.onClick.AddListener(AddLaunchRpm);
			}
		}

		private void OnEnable()
		{
			if ((bool)ResetButton)
			{
				ResetButton.onClick.RemoveAllListeners();
				ResetButton.onClick.AddListener(ResetRpm);
			}
		}

		public void Init()
		{
			if ((bool)V.Car && V.Car.TryGetComponent<VehicleEntityProvider>(out _vehicleData) && (bool)Controller)
			{
				CarEntity carEntity = _vehicleData.CarEntity;
				_currentLaunchRpm = carEntity.GearBox.GetValue().LaunchRpm;
				carEntity.GearBox.GetValue();
				VehicleDataResult value = _vehicleData.CarEntity.GetValue();
				if (carEntity.Engine.ECU != null)
				{
					_currentIdleRpm = value.IdleRpm;
					_currentRedlineRpm = value.RedlineRpm;
					_idleMinRpm = carEntity.Engine.ECU.GetMinIdleRPM;
					_idleMaxRpm = carEntity.Engine.ECU.GetMaxIdleRPM;
					_defaultIdleRpm = carEntity.Engine.ECU.GetIdleRPM;
					_defaultRedlineRpm = carEntity.Engine.ECU.GetLimitRPM;
					_defaultLaunchRpm = carEntity.GearBox.GetValue().LaunchRpm;
					_currentLaunchRpm = value.LcRpm;
					_LaunchMinRpm = carEntity.Engine.ECU.GetIdleRPM;
					_LaunchMaxRpm = carEntity.Engine.ECU.GetLimitRPM;
					_minRedlineRpm = (float)_defaultRedlineRpm * 0.7f;
					_maxRedlineRpm = carEntity.Engine.ECU.GetMaxLimitRPM;
					_destructiveRedlineRpm = value.MaxRpm;
					if ((bool)DisplayIdleText)
					{
						DisplayIdleText.text = $"{IdleColorTag}{_currentIdleRpm} RPM";
					}
					if ((bool)DisplayRedlineText)
					{
						DisplayRedlineText.text = $"{RedlineColorTag}{_currentRedlineRpm} RPM";
					}
					if ((bool)DisplayRpmText)
					{
						DisplayRpmText.text = $"{RpmColorTag}{_currentLaunchRpm} RPM";
					}
					Controller.SetActive(value: true);
					Launch.SetActive(carEntity.GearBox.GetValue().LaunchControl);
					_carInfo = _carInfoService.GetFromSlot(V.SlotID);
				}
				else
				{
					Controller.SetActive(value: false);
				}
			}
			_ = UIHelper.DelayedRebuildLayout(LayoutRpm);
		}

		private void ResetRpm()
		{
			_currentRedlineRpm = _defaultRedlineRpm;
			_currentIdleRpm = _defaultIdleRpm;
			_currentLaunchRpm = _defaultLaunchRpm;
			ApplyRpm();
		}

		private void AddIdleRpm()
		{
			_currentIdleRpm = Utils.RoundTo(math.clamp((float)_currentIdleRpm + 50f, _idleMinRpm, _idleMaxRpm), 0);
			ApplyRpm();
		}

		private void RemoveIdleRpm()
		{
			_currentIdleRpm = Utils.RoundTo(math.clamp((float)_currentIdleRpm - 50f, _idleMinRpm, _idleMaxRpm), 0);
			ApplyRpm();
		}

		private void AddRedlineRpm()
		{
			_currentRedlineRpm = Utils.RoundTo(math.clamp((float)_currentRedlineRpm + 50f, _minRedlineRpm, _maxRedlineRpm), 0);
			ApplyRpm();
			TryUnlockCustomRedline();
		}

		private void RemoveRedlineRpm()
		{
			_currentRedlineRpm = Utils.RoundTo(math.clamp((float)_currentRedlineRpm - 50f, _minRedlineRpm, _maxRedlineRpm), 0);
			ApplyRpm();
			TryUnlockCustomRedline();
		}

		private void TryUnlockCustomRedline()
		{
			if ((bool)_vehicleData && _vehicleData.CarEntity?.Engine?.ECU != null)
			{
				float a = _currentRedlineRpm;
				float b = _defaultRedlineRpm;
				if (!Mathf.Approximately(a, b))
				{
					GooglePlayAchievements.Unlock("CgkIuKqW0PcZEAIQGg", "redline_tuning");
				}
			}
		}

		private void ApplyRpm()
		{
			if ((bool)DisplayIdleText)
			{
				DisplayIdleText.text = $"{IdleColorTag}{_currentIdleRpm} RPM";
			}
			if ((bool)DisplayRedlineText)
			{
				DisplayRedlineText.text = $"{RedlineColorTag}{_currentRedlineRpm} RPM";
			}
			if ((bool)DisplayRpmText)
			{
				DisplayRpmText.text = $"{RpmColorTag}{_currentLaunchRpm} RPM";
			}
			_vehicleData.CarEntity.Engine.DynamicTuningSettings.IdleRpm = _currentIdleRpm;
			_vehicleData.CarEntity.Engine.DynamicTuningSettings.RedlineRpm = _currentRedlineRpm;
			_vehicleData.CarEntity.Engine.DynamicTuningSettings.LaunchRpm = _currentLaunchRpm;
			_vehicleData.CarEntity.RecalculateEngine(_vehicleData, _carTuningFactory, _carInfo);
			float a = _currentRedlineRpm;
			float b = _defaultRedlineRpm;
			if (_vehicleData.CarEntity.Engine.ECU != null && !Mathf.Approximately(a, b))
			{
				_steamAchievementService?.Unlock("ACH_REDLINE_SCHOLAR");
			}
		}

		private void AddLaunchRpm()
		{
			_currentLaunchRpm = Utils.RoundTo(math.clamp((float)_currentLaunchRpm + 100f, _LaunchMinRpm, _LaunchMaxRpm), 0);
			ApplyRpm();
		}

		private void RemoveLaunchRpm()
		{
			_currentLaunchRpm = Utils.RoundTo(math.clamp((float)_currentLaunchRpm - 100f, _LaunchMinRpm, _LaunchMaxRpm), 0);
			ApplyRpm();
		}
	}
}

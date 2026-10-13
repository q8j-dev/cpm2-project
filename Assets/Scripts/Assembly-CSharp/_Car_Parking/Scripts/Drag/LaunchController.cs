using CPM.Database;
using CPMEngine;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.Drag.Engine;
using _Car_Parking.Scripts.EngineTuning;

namespace _Car_Parking.Scripts.Drag
{
	public class LaunchController : MonoBehaviour
	{
		private VehicleEntityProvider _vehicleData;

		[SerializeField]
		private TextMeshProUGUI DisplayRpmText = default;

		[SerializeField]
		private TextMeshProUGUI DisplayTimeText = default;

		[SerializeField]
		private Button MinusRpmButton = default;

		[SerializeField]
		private Button PlusRpmButton = default;

		[SerializeField]
		private Button MinusTimeButton = default;

		[SerializeField]
		private Button PlusTimeButton = default;

		[SerializeField]
		private Button ResetButton = default;

		[SerializeField]
		private GameObject Controller = default;

		private float _defaultLaunchRpm;

		private float _defaultTime;

		private float _LaunchMinRpm;

		private float _LaunchMaxRpm;

		private float _maxTime;

		private float _destructiveTime;

		private float _currentTime;

		private float _currentLaunchRpm;

		private ICarTuningFactory _carTuningFactory;

		private ICarInfoService _carInfoService;

		private CarInfo _carInfo;

		private string RpmColorTag => "<color=#F8F8F8>";

		private string TimeColorTag
		{
			get
			{
				if (_currentTime <= _defaultTime)
				{
					return "<color=#F8F8F8>";
				}
				if (_currentTime > _defaultTime && _currentTime < _destructiveTime)
				{
					return "<color=#FFB900>";
				}
				if (!(_currentTime >= _destructiveTime))
				{
					return string.Empty;
				}
				return "<color=#C80109>";
			}
		}

		[Inject]
		public void Construct(ICarInfoService carInfoService, ICarTuningFactory carTuningFactory)
		{
			_carTuningFactory = carTuningFactory;
			_carInfoService = carInfoService;
		}

		private void Start()
		{
			if ((bool)MinusRpmButton)
			{
				MinusRpmButton.onClick.AddListener(RemoveLaunchRpm);
			}
			if ((bool)PlusRpmButton)
			{
				PlusRpmButton.onClick.AddListener(AddLaunchRpm);
			}
			if ((bool)MinusTimeButton)
			{
				MinusTimeButton.onClick.AddListener(RemoveTime);
			}
			if ((bool)PlusTimeButton)
			{
				PlusTimeButton.onClick.AddListener(AddTime);
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
				base.gameObject.SetActive(carEntity.GetValue().HaveLc);
				_currentLaunchRpm = carEntity.GearBox.GetValue().LaunchRpm;
				_currentTime = carEntity.GearBox.GetValue().LaunchTime;
				if (carEntity.Engine.ECU != null)
				{
					_LaunchMinRpm = carEntity.Engine.ECU.GetIdleRPM;
					_LaunchMaxRpm = carEntity.Engine.ECU.GetLimitRPM;
				}
				_defaultLaunchRpm = carEntity.GearBox.GetValue().LaunchRpm;
				_defaultTime = carEntity.GearBox.GetValue().LaunchTime;
				_maxTime = 10000f;
				_destructiveTime = carEntity.GetValue().MaxRpm;
				if ((bool)DisplayRpmText)
				{
					DisplayRpmText.text = $"{RpmColorTag}{_currentLaunchRpm} RPM";
				}
				if ((bool)DisplayTimeText)
				{
					DisplayTimeText.text = $"{TimeColorTag}{_currentTime} ms";
				}
				Controller.SetActive(value: true);
				_carInfo = _carInfoService.GetFromSlot(V.SlotID);
			}
		}

		private void ResetRpm()
		{
			_currentTime = _defaultTime;
			_currentLaunchRpm = _defaultLaunchRpm;
			ApplyRpm();
		}

		private void AddLaunchRpm()
		{
			_currentLaunchRpm = Utils.RoundTo(math.clamp(_currentLaunchRpm + 100f, _LaunchMinRpm, _LaunchMaxRpm), 0);
			ApplyRpm();
		}

		private void RemoveLaunchRpm()
		{
			_currentLaunchRpm = Utils.RoundTo(math.clamp(_currentLaunchRpm - 100f, _LaunchMinRpm, _LaunchMaxRpm), 0);
			ApplyRpm();
		}

		private void AddTime()
		{
			_currentTime = Utils.RoundTo(math.clamp(_currentTime + 100f, _defaultTime * 0.7f, _maxTime), 0);
			ApplyRpm();
		}

		private void RemoveTime()
		{
			_currentTime = Utils.RoundTo(math.clamp(_currentTime - 100f, _defaultTime * 0.7f, _maxTime), 0);
			ApplyRpm();
		}

		private void ApplyRpm()
		{
			if ((bool)DisplayRpmText)
			{
				DisplayRpmText.text = $"{RpmColorTag}{_currentLaunchRpm} RPM";
			}
			if ((bool)DisplayTimeText)
			{
				DisplayTimeText.text = $"{TimeColorTag}{_currentTime} ms";
			}
			_vehicleData.CarEntity.Engine.DynamicTuningSettings.LaunchRpm = _currentLaunchRpm;
			_vehicleData.CarEntity.Engine.DynamicTuningSettings.LaunchTime = _currentTime;
			_vehicleData.CarEntity.RecalculateEngine(_vehicleData, _carTuningFactory, _carInfo);
		}
	}
}

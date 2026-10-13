using System;
using System.Linq;
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

namespace _Car_Parking.Scripts.Drag
{
	public class AFController : MonoBehaviour
	{
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

		private ObscuredFloat _optimalRatio;

		private ObscuredFloat _maxRatio;

		private ObscuredFloat _minRatio;

		private ObscuredFloat _currentRatio;

		[SerializeField]
		private Color[] dots;

		[SerializeField]
		private Slider AfVisualSlider;

		[SerializeField]
		private Image AfVisualDot;

		private float step;

		private ICarTuningFactory _carTuningFactory;

		private ICarInfoService _carInfoService;

		private CarInfo _carInfo;

		private const int AirFuelRatioUICount = 11;

		private string ColorTag
		{
			get
			{
				if ((float)_currentRatio <= (float)_optimalRatio)
				{
					return "<color=#49CC01>";
				}
				if ((float)_currentRatio > (float)_optimalRatio && (float)_currentRatio <= (float)_maxRatio)
				{
					return "<color=#FFB900>";
				}
				if (!((float)_currentRatio > (float)_maxRatio))
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
			if ((bool)MinusButton)
			{
				MinusButton.onClick.AddListener(RemoveRatio);
			}
			if ((bool)PlusButton)
			{
				PlusButton.onClick.AddListener(AddRatio);
			}
			if ((bool)AfVisualSlider)
			{
				AfVisualSlider.onValueChanged.AddListener((float value) =>
				{
					VisualMixture();
				});
			}
		}

		private void OnEnable()
		{
			if ((bool)ResetButton)
			{
				ResetButton.onClick.RemoveAllListeners();
				ResetButton.onClick.AddListener(ResetRatio);
			}
		}

		public void Init()
		{
			if ((bool)V.Car && V.Car.TryGetComponent<VehicleEntityProvider>(out _vehicleData) && (bool)Controller)
			{
				CarEntity carEntity = _vehicleData.CarEntity;
				_currentRatio = carEntity.Engine.GetMr;
				_optimalRatio = carEntity.Engine.GetOMr;
				_maxRatio = (float)_optimalRatio * 1.1f;
				_minRatio = (float)_optimalRatio * 0.9f;
				step = (float)_optimalRatio * 0.02f;
				float[] array = new float[11];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = (float)_minRatio + step * (float)i;
				}
				float value = array.OrderBy((float n) => Math.Abs(n - (float)_currentRatio)).First();
				Controller.SetActive(value: true);
				if ((bool)AfVisualSlider)
				{
					AfVisualSlider.minValue = _minRatio;
					AfVisualSlider.maxValue = _maxRatio;
					AfVisualSlider.value = value;
				}
				VisualMixture();
				_carInfo = _carInfoService.GetFromSlot(V.SlotID);
				_vehicleData.CarEntity.RecalculateEngine(_vehicleData, _carTuningFactory, _carInfo);
			}
		}

		private void VisualMixture()
		{
			Color color = dots[1];
			if ((bool)AfVisualSlider && (bool)AfVisualDot)
			{
				if ((float)_currentRatio < (float)_optimalRatio)
				{
					color = dots[0];
				}
				if (_currentRatio.Equals(_optimalRatio))
				{
					color = dots[1];
				}
				if ((float)_currentRatio > (float)_optimalRatio)
				{
					color = dots[2];
				}
				AfVisualDot.color = color;
			}
		}

		private void ResetRatio()
		{
			_optimalRatio = _vehicleData.CarEntity.Engine.GetOMr;
			_currentRatio = _optimalRatio;
			if ((bool)DisplayText)
			{
				DisplayText.text = $"Ratio: {ColorTag}{_currentRatio}:1";
			}
			ApplyRatio();
			AfVisualSlider.value = _currentRatio;
		}

		private void AddRatio()
		{
			_currentRatio = Utils.RoundTo(math.clamp((float)_currentRatio + step, _minRatio, _maxRatio), 3);
			if ((bool)DisplayText)
			{
				DisplayText.text = $"Ratio: {ColorTag}{_currentRatio}:1";
			}
			ApplyRatio();
			AfVisualSlider.value += step;
		}

		private void RemoveRatio()
		{
			_currentRatio = Utils.RoundTo(math.clamp((float)_currentRatio - step, _minRatio, _maxRatio), 3);
			if ((bool)DisplayText)
			{
				DisplayText.text = $"Ratio: {ColorTag}{_currentRatio}:1";
			}
			ApplyRatio();
			AfVisualSlider.value -= step;
		}

		private void ApplyRatio()
		{
			if ((bool)_vehicleData)
			{
				_vehicleData.CarEntity.Engine.DynamicTuningSettings.MixtureRatio = _currentRatio;
				_vehicleData.CarEntity.RecalculateEngine(_vehicleData, _carTuningFactory, _carInfo);
				Debug.Log($"Mixture ratio: {_currentRatio}");
			}
		}
	}
}

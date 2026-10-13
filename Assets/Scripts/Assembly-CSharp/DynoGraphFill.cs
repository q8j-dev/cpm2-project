using CPMEngine;
using ChartAndGraph;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.GooglePlayServices;
using _Car_Parking.Scripts.Steam.Base;

public class DynoGraphFill : MonoBehaviour
{
	private GraphChartBase graph;

	public TextMeshProUGUI rpmInput;

	private float _maxTorque;

	private float _maxTorqueRpm;

	private float _maxPower;

	private float _maxPowerRpm;

	private int _pullCount;

	private bool _isRunning;

	public bool write;

	public float lastRPMWrote;

	private DynoRunController _dynoRunController;

	private bool _isUpdatingGraph;

	private VehicleClass _lastClass;

	private CarInputManager _carInputManager;

	private ISteamAchievementService _steamAchievementService;

	[Inject]
	public void Construct(DynoRunController dynoRunController, CarInputManager carInputManager, ISteamAchievementService steamAchievementService)
	{
		_dynoRunController = dynoRunController;
		_carInputManager = carInputManager;
		_steamAchievementService = steamAchievementService;
	}

	private void Awake()
	{
		graph = GetComponent<GraphChartBase>();
		_pullCount = PlayerPrefs.GetInt("pullCount", 0);
	}

	public void ClearChart()
	{
		ClearAll();
	}

	private float CalculateMaxHp(float autoHp)
	{
		return Mathf.Ceil(autoHp * 1.2f / 100f) * 100f;
	}

	private int CalculateMaxRpm(float autoRpm)
	{
		return (int)(Mathf.Ceil(autoRpm / 1000f) * 1000f);
	}

	private int CalculateLinesRPM(int input)
	{
		return input / 1000;
	}

	private int CalculateLinesHP(float input)
	{
		if (input <= 700f)
		{
			if (input <= 500f)
			{
				if (input <= 200f)
				{
					return 4;
				}
				return 5;
			}
			if (input <= 600f)
			{
				return 6;
			}
			return 7;
		}
		if (input <= 800f)
		{
			return 8;
		}
		return 10;
	}

	public void SetFillState(bool toRun)
	{
		if (toRun)
		{
			graph.DataSource.AutomaticVerticallView = false;
			graph.DataSource.AutomaticHorizontalView = false;
			_dynoRunController.Powertrain.CalcMaxPowerMaxTorque(out var maxHp, out var _, out var maxTq, out var _, out var _);
			float num = ((maxTq > maxHp) ? CalculateMaxHp(maxTq) : CalculateMaxHp(maxHp));
			graph.DataSource.VerticalViewSize = num;
			graph.GetComponent<VerticalAxis>().MainDivisions.Total = CalculateLinesHP(num);
			float maxRpm = _dynoRunController.Powertrain.MaxRpm;
			int num2 = CalculateMaxRpm(maxRpm);
			graph.DataSource.HorizontalViewSize = num2;
			graph.GetComponent<HorizontalAxis>().MainDivisions.Total = CalculateLinesRPM(num2);
			_isRunning = true;
			if (!_isUpdatingGraph)
			{
				UpdateGraph().Forget();
			}
		}
		else
		{
			_isRunning = false;
		}
	}

	private void WriteTorque(float rpm, float torque)
	{
		if (torque > _maxTorque)
		{
			_maxTorque = torque;
			_maxTorqueRpm = rpm;
		}
		graph.DataSource.AddPointToCategory("Torque", rpm, torque);
	}

	private void WritePower(float rpm, float power)
	{
		if (power > _maxPower)
		{
			_maxPower = power;
			_maxPowerRpm = rpm;
		}
		graph.DataSource.AddPointToCategory("Power", rpm, power);
	}

	private void ClearAll()
	{
		_maxPower = 0f;
		_maxTorque = 0f;
		_maxPowerRpm = 0f;
		_maxPowerRpm = 0f;
		graph.DataSource.ClearCategory("Torque");
		graph.DataSource.ClearCategory("Power");
	}

	private void UpdateLabels()
	{
		if ((bool)_dynoRunController && (bool)_dynoRunController.dynoWindow)
		{
			if ((bool)_dynoRunController.dynoWindow.maxPowerLabel)
			{
				_dynoRunController.dynoWindow.maxPowerLabel.text = $"{_maxPower:N0}/{_maxPowerRpm:N0}";
			}
			if ((bool)_dynoRunController.dynoWindow.maxTorqueLabel)
			{
				_dynoRunController.dynoWindow.maxTorqueLabel.text = $"{_maxTorque:N0}/{_maxTorqueRpm:N0}";
			}
		}
	}

	private void UpdateClass(VehicleClass vehicleClass)
	{
		if ((bool)_dynoRunController.dynoWindow && (bool)_dynoRunController.dynoWindow.ClassText)
		{
			_dynoRunController.dynoWindow.ClassText.text = vehicleClass.ToString();
		}
	}

	private void OnRunFinished()
	{
		_pullCount++;
		PlayerPrefs.SetInt("pullCount", _pullCount);
		GooglePlayAchievements.Unlock("CgkIuKqW0PcZEAIQDg", "dyno_run");
		GooglePlayAchievements.ReportProgress("CgkIuKqW0PcZEAIQLA", "gpg_ach_dyno_pulls", 10, 1, "dyno_run");
		_steamAchievementService?.Unlock("ACH_DYNO_DAY");
		_steamAchievementService?.ReportProgress("ACH_GRAPH_PAPER", "steam_dyno_pulls", 10, _pullCount);
		_dynoRunController.dynoWindow.pullCountLabel.text = _pullCount.ToString();
		UpdateLabels();
		UpdateClass(Utils.GetClassVehicle(_dynoRunController.GetClassPoint(), _dynoRunController.VehicleData.CarId));
		_lastClass = VehicleClass.A;
	}

	public void ResetPullCount()
	{
		_pullCount = 0;
	}

	private async UniTaskVoid UpdateGraph()
	{
		int classStepCount = 0;
		_isUpdatingGraph = true;
		while (_isRunning)
		{
			Powertrain powertrain = _dynoRunController.Powertrain;
			float currentRpm = powertrain.CurrentRpm;
			rpmInput.text = $"{currentRpm:N0}";
			if (_carInputManager.ThrottleOutput < 0.05f && write)
			{
				OnRunFinished();
				write = false;
				classStepCount = 0;
				await UniTask.DelayFrame(2);
				continue;
			}
			if (_carInputManager.ThrottleOutput > 0.5f && !write)
			{
				write = true;
				ClearAll();
			}
			float f = ((powertrain.GearType == GearboxType.GearFwd) ? powertrain.GetWheel(WheelIndex.FrontLeft).WheelTireVelocity : powertrain.GetWheel(WheelIndex.RearLeft).WheelTireVelocity);
			if (write && Mathf.Abs(lastRPMWrote - currentRpm) > 100f && Mathf.Abs(f) > 0f)
			{
				powertrain.GetDynoGraphAtRpm(powertrain.CurrentRpm, out var _, out var wtorq, out var _, out var whp);
				WriteTorque(powertrain.CurrentRpm, wtorq);
				WritePower(powertrain.CurrentRpm, whp);
				UpdateLabels();
				lastRPMWrote = powertrain.CurrentRpm;
			}
			if (write && Mathf.Abs(f) > 0f)
			{
				VehicleClass classVehicle = Utils.GetClassVehicle(classStepCount, _dynoRunController.VehicleData.CarId);
				if (classVehicle > _lastClass)
				{
					_lastClass = classVehicle;
				}
				UpdateClass(_lastClass);
				classStepCount = ((classStepCount < _dynoRunController.GetClassPoint()) ? (classStepCount + 20) : _dynoRunController.GetClassPoint());
			}
			await UniTask.DelayFrame(2);
		}
		_isUpdatingGraph = false;
	}
}

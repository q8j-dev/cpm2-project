using System;
using System.Collections.Generic;
using CPMEngine;
using UniRx;
using UnityEngine;
using _Car_Parking;
using _Car_Parking.Scripts.Car.Configs;

[DisallowMultipleComponent]
public class ClusterOsd : MonoBehaviour
{
	[Header("Digital cluster")]
	public TextMesh TextMeshGear;

	public TextMesh TextMeshTachometer;

	public TextMesh TextMeshSpeedo;

	[Header("Analog cluster")]
	[Header("Tachometer")]
	[Space]
	public bool ClockwiseRpm = true;

	[SerializeField]
	private GameObject TachometerNeedle;

	private Vector3 _cachedTachometerNeedleRotation;

	[SerializeField]
	private float MaxRpm = 8000f;

	[SerializeField]
	private float MinRpmAngle;

	[SerializeField]
	private float MaxRpmAngle;

	[SerializeField]
	private List<SpeedAnglePoint> TachoCalibration;

	private float _lerpedRpm;

	private float _valueRpm;

	private float _tachoAngle;

	[Space]
	[Header("Speedometer")]
	public bool ClockwiseSpeed = true;

	[SerializeField]
	private GameObject SpeedometerNeedle;

	[SerializeField]
	private bool Mph;

	[SerializeField]
	private float MaxSpeed = 200f;

	[SerializeField]
	private float MinSpeedometerAngle;

	[SerializeField]
	private float MaxSpeedometerAngle;

	[SerializeField]
	private List<SpeedAnglePoint> SpeedoCalibration;

	private float _lerpedSpeedo;

	private float _valueSpeed;

	private float _speedoAngle;

	private Vector3 _cachedSpeedometerNeedleRotation;

	private Powertrain _powertrain;

	private VehicleData _vehicleData;

	private float _speedInt;

	private IDisposable _speedDigitalSub;

	private DefaultMaterialsConfig _defaultMaterialsConfig;

	private int _lastDisplayedSpeed = int.MinValue;

	private float _lastDisplayedRpm = float.MinValue;

	private void Awake()
	{
		TryGetComponent<Powertrain>(out _powertrain);
		TryGetComponent<VehicleData>(out _vehicleData);
		_defaultMaterialsConfig = _vehicleData.DefaultMaterialsConfig;
		FixDigitalTextMat();
		if ((bool)SpeedometerNeedle)
		{
			_cachedSpeedometerNeedleRotation = SpeedometerNeedle.transform.localEulerAngles;
		}
		if ((bool)TachometerNeedle)
		{
			_cachedTachometerNeedleRotation = TachometerNeedle.transform.localEulerAngles;
		}
	}

	private void OnEnable()
	{
		if ((bool)_powertrain)
		{
			if ((bool)TextMeshGear)
			{
				_powertrain.OnGearChanged += SetGearText;
			}
			_speedDigitalSub?.Dispose();
			_speedDigitalSub = _powertrain.SpeedKmhReactive.Subscribe(UpdateSpeed).AddTo(this);
		}
	}

	private void OnDisable()
	{
		if ((bool)_powertrain && (bool)TextMeshGear)
		{
			_powertrain.OnGearChanged -= SetGearText;
		}
	}

	public void SetTachometerNeedle(GameObject needle)
	{
		if ((bool)TachometerNeedle)
		{
			UnityEngine.Object.Destroy(TachometerNeedle);
		}
		TachometerNeedle = needle;
		_cachedTachometerNeedleRotation = needle.transform.localEulerAngles;
	}

	public void SetSpeedometerNeedle(GameObject needle)
	{
		if ((bool)SpeedometerNeedle)
		{
			UnityEngine.Object.Destroy(SpeedometerNeedle);
		}
		SpeedometerNeedle = needle;
		_cachedSpeedometerNeedleRotation = needle.transform.localEulerAngles;
	}

	public void SetDigitalGear(TextMesh gear)
	{
		if (TextMeshGear != null && _powertrain != null)
		{
			_powertrain.OnGearChanged -= SetGearText;
		}
		TextMeshGear = gear;
		if (TextMeshGear != null && _powertrain != null)
		{
			_powertrain.OnGearChanged += SetGearText;
		}
	}

	public void FixDigitalTextMat()
	{
		FixDigitalTextMesh(TextMeshSpeedo);
		FixDigitalTextMesh(TextMeshGear);
		FixDigitalTextMesh(TextMeshTachometer);
		if ((bool)TextMeshSpeedo)
		{
			TextMeshSpeedo.text = "0";
		}
		if ((bool)TextMeshGear)
		{
			SetGearText();
		}
		if ((bool)TextMeshTachometer)
		{
			TextMeshTachometer.text = "0";
		}
	}

	private void FixDigitalTextMesh(TextMesh textMesh)
	{
		if ((bool)textMesh)
		{
			if (textMesh.TryGetComponent<MeshRenderer>(out var component))
			{
				component.sharedMaterial = _defaultMaterialsConfig.SpeedInCarTextMeshMaterial;
			}
			textMesh.font = _defaultMaterialsConfig.SpeedInCarFont;
		}
	}

	private void SetGearText()
	{
		TextMeshGear.text = _powertrain.GetSpeedometerGearName;
	}

	private void Update()
	{
		if ((bool)_powertrain)
		{
			_valueRpm = _powertrain.CurrentRpm;
			UpdateRpmNeedle();
			UpdateSpeedoNeedle();
			UpdateDigitalTacho();
		}
	}

	private void UpdateDigitalTacho()
	{
		if ((bool)TextMeshTachometer)
		{
			float num = (float)Math.Round(_valueRpm * 0.001f, 1);
			if (num != _lastDisplayedRpm)
			{
				_lastDisplayedRpm = num;
				TextMeshTachometer.text = num.ToString("F1");
			}
		}
	}

	private void UpdateRpmNeedle()
	{
		if ((bool)TachometerNeedle)
		{
			_lerpedRpm = Utils.GetFixedVariable(Mathf.Lerp(_lerpedRpm, _valueRpm, Time.deltaTime * 5f));
			float value = Mathf.Clamp(_lerpedRpm, 0f, MaxRpm);
			_tachoAngle = MapValueToAngle(value, TachoCalibration, MaxRpm, MinRpmAngle, MaxRpmAngle, ClockwiseRpm);
			TachometerNeedle.transform.localEulerAngles = new Vector3(_cachedTachometerNeedleRotation.x, _cachedTachometerNeedleRotation.y, _tachoAngle);
		}
	}

	private void UpdateSpeedoNeedle()
	{
		if ((bool)SpeedometerNeedle)
		{
			float num = (Mph ? _powertrain.SpeedMph : _powertrain.SpeedKmh);
			float num2 = 0.3f;
			float maxDelta = Mathf.Abs(num - _lerpedSpeedo) * (Time.deltaTime / num2);
			_lerpedSpeedo = Mathf.MoveTowards(_lerpedSpeedo, num, maxDelta);
			float value = Mathf.Clamp(_lerpedSpeedo, 0f, MaxSpeed);
			_speedoAngle = MapValueToAngle(value, SpeedoCalibration, MaxSpeed, MinSpeedometerAngle, MaxSpeedometerAngle, ClockwiseSpeed);
			SpeedometerNeedle.transform.localEulerAngles = new Vector3(_cachedSpeedometerNeedleRotation.x, _cachedSpeedometerNeedleRotation.y, _speedoAngle);
		}
	}

	private static float MapValueToAngle(float value, List<SpeedAnglePoint> points, float maxValue, float minAngle, float maxAngle, bool clockwise)
	{
		if (points != null && points.Count >= 2)
		{
			if (value <= points[0].Value)
			{
				return points[0].Angle;
			}
			int num = points.Count - 1;
			if (value >= points[num].Value)
			{
				return points[num].Angle;
			}
			for (int i = 0; i < num; i++)
			{
				SpeedAnglePoint speedAnglePoint = points[i];
				SpeedAnglePoint speedAnglePoint2 = points[i + 1];
				if (value >= speedAnglePoint.Value && value <= speedAnglePoint2.Value)
				{
					float t = (Mathf.Approximately(speedAnglePoint.Value, speedAnglePoint2.Value) ? 0f : ((value - speedAnglePoint.Value) / (speedAnglePoint2.Value - speedAnglePoint.Value)));
					return Mathf.Lerp(speedAnglePoint.Angle, speedAnglePoint2.Angle, t);
				}
			}
		}
		float t2 = ((maxValue > 0f) ? Mathf.Clamp01(value / maxValue) : 0f);
		return Mathf.LerpUnclamped(minAngle, maxAngle, t2) * (clockwise ? 1f : (-1f));
	}

	private bool HasTachoCalibration()
	{
		if (TachoCalibration != null)
		{
			return TachoCalibration.Count >= 2;
		}
		return false;
	}

	private bool HasSpeedoCalibration()
	{
		if (SpeedoCalibration != null)
		{
			return SpeedoCalibration.Count >= 2;
		}
		return false;
	}

	private void UpdateSpeed(float newSpeed)
	{
		if ((bool)TextMeshSpeedo)
		{
			float num = (FlagsStorage.Kph ? newSpeed : Utils.Km2Mile(newSpeed));
			float num2 = 0.3f;
			float maxDelta = Mathf.Abs(num - _speedInt) * (Time.deltaTime / num2);
			_speedInt = Mathf.MoveTowards(_speedInt, num, maxDelta);
			int num3 = Mathf.FloorToInt(_speedInt);
			if (num3 != _lastDisplayedSpeed)
			{
				_lastDisplayedSpeed = num3;
				TextMeshSpeedo.text = num3.ToString();
			}
		}
	}
}

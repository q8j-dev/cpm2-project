using System;
using System.Collections.Generic;
using System.Linq;
using CPMEngine;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;
using _Car_Parking.Scripts.EngineTuning;
using _Car_Parking.Scripts.EngineTuning.Instances;

namespace _Car_Parking.Scripts.Drag.Engine
{
	[Serializable]
	public class EngineInstance : PartInstance<EngineId>, IReturnablePart<EngineOutput>
	{
		[Header("Dyno Tuning")]
		public DynamicTuningSettings DynamicTuningSettings;

		[Header("Components")]
		public BlockInstance Block;

		public RotorHousingInstance RotorHousing;

		[SerializeReference]
		public CylinderHeadInstance CylinderHead;

		public IntakeManifoldInstance IntakeManifold;

		public ThrottleValveInstance ThrottleValve;

		public FuelRailInstance FuelRail;

		public FuelInjectorsInstance FuelInjectors;

		public ExhaustHeaderInstance ExhaustHeader;

		public ExhaustPipeInstance ExhaustPipe;

		public ClutchInstance Clutch;

		public ChargerInstance Charger;

		public EcuInstance ECU;

		public FuelPumpInstance FuelPump;

		public PartInstance<RadiatorPart> Radiator;

		public PartInstance<IntercoolerPart> Intercooler;

		public float Displacement;

		public float GetC;

		public bool isRichMixture;

		private float _bTr;

		private float _cN = 1.5f;

		private float _eE;

		private ObscuredInt _engineType;

		private float _fcAcs = 8f;

		private float _fCs = 120f;

		private float _iE;

		private float _inMax;

		private float _inMin;

		private float _iR;

		private float _iTl = 110f;

		private float _mAc;

		private float _mass;

		private float _mFc;

		private float _mH = 1000000f;

		private float _outMax;

		private float _outMin;

		private List<PartInstanceBase> _subParts;

		private float _tB = 0.003f;

		private int _obd;

		private float _vMin;

		private const float PI = (float)Math.PI;

		public float GetMr { get; private set; }

		public float GetOMr { get; private set; }

		public override List<PartInstanceBase> SubParts
		{
			get
			{
				if (_subParts == null || _subParts.Count == 0)
				{
					_subParts = new List<PartInstanceBase>();
					if ((bool)RotorHousing?.Part)
					{
						_subParts.Add(RotorHousing);
					}
					else
					{
						_subParts.Add(Block);
						_subParts.Add(CylinderHead);
					}
					_subParts.Add(IntakeManifold);
					_subParts.Add(FuelRail);
					_subParts.Add(FuelInjectors);
					_subParts.Add(ExhaustHeader);
					_subParts.Add(ExhaustPipe);
					_subParts.Add(Clutch);
					_subParts.Add(ECU);
					_subParts.Add(FuelPump);
					_subParts.Add(Charger);
					_subParts.Add(ThrottleValve);
					_subParts.Add(Radiator);
					_subParts.Add(Intercooler);
				}
				return _subParts;
			}
		}

		private int NumberOfHead => (EngineType)(int)_engineType switch
		{
			EngineType.I4 => 1, 
			EngineType.I5 => 1, 
			EngineType.I6 => 1, 
			EngineType.H4 => 2, 
			EngineType.H6 => 2, 
			EngineType.V6 => 2, 
			EngineType.V8 => 2, 
			EngineType.V10 => 2, 
			EngineType.V12 => 2, 
			EngineType.W16 => 4, 
			EngineType.None => 0, 
			_ => 1, 
		};

		private int Pot => (EngineType)(int)_engineType switch
		{
			EngineType.I3 => 3, 
			EngineType.I4 => 4, 
			EngineType.I5 => 5, 
			EngineType.I6 => 6, 
			EngineType.H4 => 4, 
			EngineType.H6 => 6, 
			EngineType.V6 => 6, 
			EngineType.V8 => 8, 
			EngineType.V10 => 10, 
			EngineType.V12 => 12, 
			EngineType.W16 => 16, 
			_ => 1, 
		};

		public FuelType GetBenzin
		{
			get
			{
				if (DynamicTuningSettings.FuelType == FuelType.None)
				{
					return FuelRail.GetFuelType;
				}
				return DynamicTuningSettings.FuelType;
			}
		}

		public ObscuredInt GetEngineType
		{
			get
			{
				ObscuredInt engineType = _engineType;
				engineType.RandomizeCryptoKey();
				return engineType;
			}
		}

		public EngineOutput GetValue()
		{
			return Ltc();
		}

		public void SetEt(int val)
		{
			_engineType = val;
		}

		private void CheckEngine(out bool res)
		{
			res = false;
			if (_obd > 0)
			{
				res = true;
				return;
			}
			List<(float, float)> list = new List<(float, float)>
			{
				(Block.Pistons.Condition, 0.3f),
				(Block.Crankshaft.Condition, 0.3f),
				(FuelRail.Condition, 0.5f),
				(FuelPump.Condition, 0.4f),
				(FuelInjectors.Condition, 0.6f),
				(IntakeManifold.Condition, 0.2f),
				(ExhaustHeader.Condition, 0.1f),
				(ExhaustPipe.Condition, 0.2f),
				(Clutch.Condition, 0.5f),
				(ThrottleValve.Condition, 0.5f)
			};
			if (Charger != null && (bool)Charger.Part)
			{
				list.Add((Charger.Condition, 0.5f));
			}
			foreach (var item in list)
			{
				if (item.Item1 <= item.Item2)
				{
					res = true;
					break;
				}
			}
		}

		private void CheckWankelEngine(out bool res)
		{
			res = false;
			if (_obd > 0)
			{
				res = true;
				return;
			}
			List<(float, float)> list = new List<(float, float)>
			{
				(RotorHousing.Rotors.Condition, 0.3f),
				(RotorHousing.EccentricShaft.Condition, 0.3f),
				(RotorHousing.RotorTrackingGears.Condition, 0.3f),
				(FuelRail.Condition, 0.5f),
				(FuelPump.Condition, 0.4f),
				(FuelInjectors.Condition, 0.6f),
				(IntakeManifold.Condition, 0.2f),
				(ExhaustHeader.Condition, 0.1f),
				(ExhaustPipe.Condition, 0.2f),
				(Clutch.Condition, 0.5f),
				(ThrottleValve.Condition, 0.5f)
			};
			if (Charger != null && (bool)Charger.Part)
			{
				list.Add((Charger.Condition, 0.5f));
			}
			foreach (var item in list)
			{
				if (item.Item1 <= item.Item2)
				{
					res = true;
					break;
				}
			}
		}

		private void CheckBroken(out int res)
		{
			res = 0;
			if ((bool)RotorHousing?.Part)
			{
				RotorHousingInstance rotorHousing = RotorHousing;
				if (rotorHousing != null && rotorHousing.Condition <= 0f)
				{
					res++;
				}
				RotorHousingInstance rotorHousing2 = RotorHousing;
				if (rotorHousing2 != null && rotorHousing2.Rotors?.Condition <= 0f)
				{
					res++;
				}
				RotorHousingInstance rotorHousing3 = RotorHousing;
				if (rotorHousing3 != null && rotorHousing3.RotorTrackingGears?.Condition <= 0f)
				{
					res++;
				}
				RotorHousingInstance rotorHousing4 = RotorHousing;
				if (rotorHousing4 != null && rotorHousing4.EccentricShaft?.Condition <= 0f)
				{
					res++;
				}
				RotorHousingInstance rotorHousing5 = RotorHousing;
				if (rotorHousing5 != null && rotorHousing5.Flywheel?.Condition <= 0f)
				{
					res++;
				}
			}
			else
			{
				BlockInstance block = Block;
				if (block != null && block.Condition <= 0f)
				{
					res++;
				}
				BlockInstance block2 = Block;
				if (block2 != null && block2.Pistons?.Condition <= 0f)
				{
					res++;
				}
				BlockInstance block3 = Block;
				if (block3 != null && block3.Crankshaft?.Condition <= 0f)
				{
					res++;
				}
				BlockInstance block4 = Block;
				if (block4 != null && block4.ConnectionRod?.Condition <= 0f)
				{
					res++;
				}
				BlockInstance block5 = Block;
				if (block5 != null && block5.Flywheel?.Condition <= 0f)
				{
					res++;
				}
			}
			if ((bool)CylinderHead?.Part)
			{
				CylinderHeadInstance cylinderHead = CylinderHead;
				if (!(cylinderHead is DohcCylinderHeadInstance dohcCylinderHeadInstance))
				{
					if (!(cylinderHead is SohcCylinderHeadInstance sohcCylinderHeadInstance))
					{
						if (cylinderHead is OhvCylinderHeadInstance ohvCylinderHeadInstance)
						{
							if (ohvCylinderHeadInstance.Condition <= 0f)
							{
								res++;
							}
							if (!ohvCylinderHeadInstance.Camshaft.Part || ohvCylinderHeadInstance.Camshaft.Condition <= 0f)
							{
								res++;
							}
						}
						else
						{
							res++;
						}
					}
					else
					{
						if (sohcCylinderHeadInstance.Condition <= 0f)
						{
							res++;
						}
						if (!sohcCylinderHeadInstance.Camshaft.Part || sohcCylinderHeadInstance.Camshaft.Condition <= 0f)
						{
							res++;
						}
					}
				}
				else
				{
					if (dohcCylinderHeadInstance.Condition <= 0f)
					{
						res++;
					}
					if (!dohcCylinderHeadInstance.IntakeCamshaft.Part || dohcCylinderHeadInstance.IntakeCamshaft.Condition <= 0f)
					{
						res++;
					}
					if (!dohcCylinderHeadInstance.ExhaustCamshaft.Part || dohcCylinderHeadInstance.ExhaustCamshaft.Condition <= 0f)
					{
						res++;
					}
				}
			}
			if (Clutch.Condition <= 0f)
			{
				res++;
			}
			if (FuelRail.Condition <= 0f)
			{
				res++;
			}
			if (IntakeManifold.Condition <= 0f)
			{
				res++;
			}
			if (FuelInjectors.Condition <= 0f)
			{
				res++;
			}
		}

		public override void UpdateWear(Pv value)
		{
			Mileage += value.P;
			foreach (PartInstanceBase subPart in SubParts)
			{
				subPart?.UpdateWear(value);
			}
		}

		private FuelMap GetFuelCorrection(float rpm)
		{
			List<FuelMap> getFuelMap = ECU.GetFuelMap;
			if (getFuelMap == null || getFuelMap.Count <= 0)
			{
				return new FuelMap(rpm);
			}
			using (IEnumerator<FuelMap> enumerator = ECU.GetFuelMap.Where((FuelMap corr) => rpm.Equals(corr.RPM)).GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					return enumerator.Current;
				}
			}
			return new FuelMap(rpm);
		}

		private EngineOutput Ltc()
		{
			EngineOutput result = default;
			CheckBroken(out _obd);
			List<TorqueCurveSample> list = new List<TorqueCurveSample>();
			if ((bool)RotorHousing?.Part)
			{
				ApplyWankelSpec(out result.MaxRpm);
				ApplyEcu(out result.IdleRpm, out result.RedlineRpm);
				ApplyCharger(out result.TurboType, out result.TurboPressure, out result.MinTurboRpm, out result.MaxTurboRpm);
				list = GenerateWankelTorque(result);
				CheckWankelEngine(out result.CheckEngine);
			}
			else
			{
				ApplyEngineSpec(out result.MaxRpm);
				ApplyEcu(out result.IdleRpm, out result.RedlineRpm);
				ApplyCharger(out result.TurboType, out result.TurboPressure, out result.MinTurboRpm, out result.MaxTurboRpm);
				list = GenerateTorque(result);
				CheckEngine(out result.CheckEngine);
			}
			result.Torque = list.ToArray();
			result.Displacement = Displacement;
			result.Inertia = _iR;
			result.BrakeInertia = _bTr;
			result.FuelConsumption = _fcAcs;
			result.FuelConsumptionSpeed = _fCs;
			result.IsBroken = _obd > 0;
			result.ShiftDelay = Clutch.GetGearShiftDelay;
			result.MaxClutchF = Mathf.Clamp(Clutch.GetMaxTorque, 0f, 3500f);
			result.SoundId = Part.EngineSound;
			return result;
		}

		private List<TorqueCurveSample> GenerateTorque(EngineOutput result)
		{
			List<TorqueCurveSample> list = new List<TorqueCurveSample>();
			float num = Displacement / (float)Pot;
			float num2 = num + _vMin;
			float num3 = CylinderHead.TimeInOpen * (float)NumberOfHead;
			float num4 = CylinderHead.TimeInClose * (float)NumberOfHead;
			float num5 = CylinderHead.TimeOutOpen * (float)NumberOfHead;
			float num6 = CylinderHead.TimeOutClose * (float)NumberOfHead;
			float num7 = Mathf.Abs(num2 * (num3 + num6 - (num5 + num4)));
			float num8 = (_inMax - _inMin) / (_outMax - _outMin);
			float num9 = num / num7;
			float num10 = num9 * num2 / (num9 + 1f) / GetMr;
			float num11 = 0.46f;
			float num12 = 0f;
			if (ThrottleValve != null)
			{
				num12 = ThrottleValve.GetDiameter;
			}
			float num13 = _iE / (float)Math.PI * Mathf.Pow(num12 / 2f, 2f) / num2 * 100f;
			float num14 = 0f;
			if (ExhaustPipe != null)
			{
				num14 = ExhaustPipe.GetDiameter;
			}
			float num15 = _eE / (float)Math.PI * Mathf.Pow(num14 / 2f, 2f) / num2 * 100f;
			float num16 = (num13 + num15) / 2f;
			if (result.TurboPressure > 0)
			{
				num11 = 0.61f;
			}
			int num17 = ((ECU != null) ? ECU.GetFuelMap.Count : 10);
			for (int i = 1; i <= num17; i++)
			{
				float num18 = i * 500;
				float num19 = num18 / 60f / 2f;
				FuelMap fuelCorrection = GetFuelCorrection(num18);
				if (fuelCorrection.Fuel == 0f && fuelCorrection.Time == 0f)
				{
					TorqueCurveSample item = new TorqueCurveSample
					{
						Rpm = num18,
						Torque = 0f
					};
					list.Add(item);
				}
				else if (FuelInjectors != null && (bool)FuelInjectors.Part && (bool)FuelPump.Part && (bool)FuelRail.Part && ECU != null)
				{
					float max = Mathf.Clamp(FuelPump.GetOutputEfficiency / (float)Pot, 0f, 410f) * FuelRail.GetOutputEfficiency;
					float num20 = (FuelPump.GetPressure + fuelCorrection.pressure) / 2f / FuelPump.GetPressure;
					float num21 = Mathf.Clamp(num20 * num20 * FuelInjectors.GetPerformance, 0f, max) * (num19 * num3) * fuelCorrection.Fuel;
					isRichMixture = num21 > num10;
					float num22 = num21 * GetMr;
					_ = num22 / num21;
					float num23 = (num21 + num22) * 1E-06f * num4 / 1.2f / (GetC - 1f) / (num4 - num3) * _mH * num8 / _tB;
					if (FuelRail.GetFuelType == FuelType.Diesel)
					{
						num11 = 0.38f;
					}
					float num24 = (num23 / (num6 - num3) * num11 + num7 * num15) / num8;
					float num25 = (num23 - num24) / 1000f * num16 * (float)Pot * fuelCorrection.Time;
					if (float.IsNaN(num25))
					{
						break;
					}
					TorqueCurveSample item2 = new TorqueCurveSample
					{
						Rpm = num18,
						Torque = num25
					};
					list.Add(item2);
				}
			}
			return list;
		}

		private List<TorqueCurveSample> GenerateWankelTorque(EngineOutput result)
		{
			List<TorqueCurveSample> list = new List<TorqueCurveSample>();
			RotorHousingResult getParams = RotorHousing.GetParams;
			float num = Displacement / (float)getParams.Rotors;
			float num2 = num + _vMin;
			float intakeDuration = getParams.IntakeDuration;
			float exhaustDuration = getParams.ExhaustDuration;
			float intakeCloseDuration = getParams.IntakeCloseDuration;
			float exhaustOpenDuration = getParams.ExhaustOpenDuration;
			float num3 = intakeCloseDuration + 360f - exhaustOpenDuration;
			float num4 = 100f;
			float value = 1f - Mathf.Abs(num3 - num4) / num4;
			float num5 = Mathf.Abs(num2 * Mathf.Clamp(value, 0.1f, 1f));
			float num6 = _inMax / (_outMax + 1E-06f);
			float num7 = num / num5;
			float num8 = num7 * num2 / (num7 + 1f) / GetMr;
			float num9 = 0.46f;
			float num10 = 0f;
			if (ThrottleValve != null)
			{
				num10 = ThrottleValve.GetDiameter;
			}
			float num11 = _iE / (float)Math.PI * Mathf.Pow(num10 / 2f, 2f) / num2 * 100f;
			float num12 = 0f;
			if (ExhaustPipe != null)
			{
				num12 = ExhaustPipe.GetDiameter;
			}
			float num13 = _eE / (float)Math.PI * Mathf.Pow(num12 / 2f, 2f) / num2 * 100f;
			float num14 = (num11 + num13) / 2f;
			if (result.TurboPressure > 0)
			{
				num9 = 0.61f;
			}
			int num15 = ((ECU != null) ? ECU.GetFuelMap.Count : 10);
			for (int i = 1; i <= num15; i++)
			{
				float num16 = i * 500;
				float num17 = num16 / 60f / 2f;
				FuelMap fuelCorrection = GetFuelCorrection(num16);
				if (fuelCorrection.Fuel == 0f && fuelCorrection.Time == 0f)
				{
					TorqueCurveSample item = new TorqueCurveSample
					{
						Rpm = num16,
						Torque = 0f
					};
					list.Add(item);
				}
				else if (FuelInjectors != null && (bool)FuelInjectors.Part && (bool)FuelPump.Part && (bool)FuelRail.Part && ECU != null)
				{
					float max = Mathf.Clamp(FuelPump.GetOutputEfficiency / (float)getParams.Rotors, 0f, 410f) * FuelRail.GetOutputEfficiency;
					float num18 = (FuelPump.GetPressure + fuelCorrection.pressure) / 2f / FuelPump.GetPressure;
					float num19 = Mathf.Clamp(num18 * num18 * FuelInjectors.GetPerformance, 0f, max) * (num17 * intakeDuration) * fuelCorrection.Fuel;
					isRichMixture = num19 > num8;
					float num20 = num19 * GetMr;
					float num21 = (num19 + num20) * 1E-06f * intakeDuration / 1.2f / (GetC - 1f) / (intakeDuration + intakeDuration) * _mH * num6 / _tB;
					float num22 = (num21 / exhaustDuration * num9 + num5 * num13) / num6;
					float num23 = (num21 - num22) / 1000f * num14 * (float)getParams.Rotors * fuelCorrection.Time;
					if (float.IsNaN(num23))
					{
						break;
					}
					TorqueCurveSample item2 = new TorqueCurveSample
					{
						Rpm = num16,
						Torque = num23
					};
					list.Add(item2);
				}
			}
			return list;
		}

		private void ApplyWankelSpec(out float maxRpm)
		{
			if (_obd > 0)
			{
				maxRpm = 0f;
				return;
			}
			_mass = 0f;
			_iR = 0f;
			RotorHousingResult getParams = RotorHousing.GetParams;
			maxRpm = getParams.MaxRPM;
			float num = 1E+16f;
			if ((bool)RotorHousing.Rotors.Part && (bool)RotorHousing.EccentricShaft.Part)
			{
				float generatingRadius = getParams.GeneratingRadius;
				float eccentricity = RotorHousing.EccentricShaft.Eccentricity;
				float housingWidth = getParams.HousingWidth;
				float compressionRatioFactor = RotorHousing.Rotors.CompressionRatioFactor;
				float num2 = Utils.Mm2Meter(3f * Mathf.Sqrt(3f) * getParams.GeneratingRadius * RotorHousing.EccentricShaft.Eccentricity * getParams.HousingWidth);
				_vMin = Utils.Mm2Meter(Mathf.Sqrt(3f) * eccentricity * eccentricity * housingWidth * compressionRatioFactor);
				float value = (Utils.Mm2Meter(3f * Mathf.Sqrt(3f) * generatingRadius * eccentricity * housingWidth) + _vMin) / _vMin * getParams.IntakeDuration;
				num = ((RotorHousing.Condition > 0f) ? num2 : 0f);
				Displacement = Utils.RoundTo(num * (float)getParams.Rotors, 0);
				GetC = Utils.RoundTo(value, 1);
			}
			else
			{
				_vMin = 1E+15f;
				GetC = 0f;
				Displacement = 0f;
			}
			FlywheelInstance flywheel = RotorHousing.Flywheel;
			if (flywheel != null && flywheel.Condition > 0f)
			{
				_iR += RotorHousing.Flywheel.GetInertia;
			}
			ExhaustHeaderInstance exhaustHeader = ExhaustHeader;
			_eE = ((exhaustHeader != null && exhaustHeader.Condition > 0f) ? ExhaustHeader.GetBlowing : 1.2f);
			IntakeManifoldInstance intakeManifold = IntakeManifold;
			if (intakeManifold != null && intakeManifold.Condition > 0f)
			{
				if (IntakeManifold.AirFilter != null)
				{
					float value2 = IntakeManifold.AirFilter.GetAirConsumption * IntakeManifold.AirFilter.Efficiency + 1E-05f;
					_mAc = Mathf.Clamp(value2, IntakeManifold.AirFilter.GetAirConsumption * 0.5f, IntakeManifold.AirFilter.GetAirConsumption);
					_iE = (IntakeManifold.Efficiency + IntakeManifold.AirFilter.Efficiency) / 2f;
				}
			}
			else
			{
				_iE = 0.2f;
				_mAc = 1E-06f;
			}
			if (num > 0f && _iE > 0f)
			{
				_inMin = 0f;
				_inMax = RotorHousing.InMax * getParams.IntakeDuration;
				_outMin = 0f;
				_outMax = RotorHousing.OutMax * getParams.ExhaustDuration;
				_inMin = Mathf.Clamp(_inMin, 0f, 1f);
				_inMax = Mathf.Clamp(_inMax, 0f, 1f);
				_outMin = Mathf.Clamp(_outMin, 0f, 1f);
				_outMax = Mathf.Clamp(_outMax, 0f, 1f);
			}
			else
			{
				_inMin = (_inMax = (_outMin = (_outMax = 0f)));
				_obd++;
			}
			ApplyFuelInjectorSystem();
			_mass = GetWeight();
			_iR += RotorHousing.Rotors.GetInertia();
			_iR = Utils.RoundTo(_iR, 2);
		}

		private void ApplyEngineSpec(out float maxRpm)
		{
			if (_obd > 0)
			{
				maxRpm = 0f;
				return;
			}
			_mass = 0f;
			_iR = 0f;
			maxRpm = (Block.maxRPM + CylinderHead.Part.maxRPM) / 2f;
			float num = 1E+16f;
			float num2 = (Block.CrankCenter - Block.Crankshaft.Stroke * 0.5f - Block.ConnectionRod.GetLength - Block.Pistons.Height * 0.5f) * 0.1f;
			if ((bool)Block.Pistons.Part && (bool)Block.Crankshaft.Part)
			{
				float num3 = Block.Pistons.Bore * Block.Pistons.Bore * ((float)Math.PI / 4f);
				float num4 = num3 * num2 * 0.01f;
				float getCrownVolume = Block.Pistons.GetCrownVolume;
				float num5 = CylinderHead.Part.Vmin - getCrownVolume + num4;
				num = ((Block.Condition > 0f) ? (num3 * Block.Crankshaft.Stroke * 0.001f) : 0f);
				Displacement = Utils.RoundTo(num * (float)Pot, 0);
				_vMin = num5;
				GetC = Utils.RoundTo((num + num5) / num5, 1);
			}
			else
			{
				_vMin = 1E+15f;
				GetC = 0f;
				Displacement = 0f;
			}
			FlywheelInstance flywheel = Block.Flywheel;
			if (flywheel != null && flywheel.Condition > 0f)
			{
				_iR += Block.Flywheel.GetInertia;
			}
			ExhaustHeaderInstance exhaustHeader = ExhaustHeader;
			_eE = ((exhaustHeader != null && exhaustHeader.Condition > 0f) ? ExhaustHeader.GetBlowing : 1.2f);
			IntakeManifoldInstance intakeManifold = IntakeManifold;
			if (intakeManifold != null && intakeManifold.Condition > 0f)
			{
				if (IntakeManifold.AirFilter != null)
				{
					float value = IntakeManifold.AirFilter.GetAirConsumption * IntakeManifold.AirFilter.Efficiency + 1E-05f;
					_mAc = Mathf.Clamp(value, IntakeManifold.AirFilter.GetAirConsumption * 0.5f, IntakeManifold.AirFilter.GetAirConsumption);
					_iE = (IntakeManifold.Efficiency + IntakeManifold.AirFilter.Efficiency) / 2f;
				}
			}
			else
			{
				_iE = 0.2f;
				_mAc = 1E-06f;
			}
			if ((bool)CylinderHead.Part)
			{
				CylinderHead.UpdateParams();
				if (num > 0f && _iE > 0f)
				{
					_inMin = CylinderHead.InMin * CylinderHead.InMin / num / _iE;
					_inMax = CylinderHead.InMax * CylinderHead.InMax / num * _iE;
					_outMin = CylinderHead.OutMin * CylinderHead.OutMin / num / _eE;
					_outMax = CylinderHead.OutMax * CylinderHead.OutMax / num * _eE;
					_inMin = Mathf.Clamp(_inMin, 0f, 1f);
					_inMax = Mathf.Clamp(_inMax, 0f, 1f);
					_outMin = Mathf.Clamp(_outMin, 0f, 1f);
					_outMax = Mathf.Clamp(_outMax, 0f, 1f);
				}
				else
				{
					_inMin = (_inMax = (_outMin = (_outMax = 0f)));
					_obd++;
				}
			}
			ApplyFuelInjectorSystem();
			_mass = GetWeight();
			_iR += Block.Crankshaft.GetInertia;
			_iR = Utils.RoundTo(_iR, 2);
		}

		private void GetFuelParams(FuelType fuelType, out float heat, out float burnTimeBase, out float burnTimeOffset, out float optimalMixtureRatio)
		{
			switch (fuelType)
			{
			case FuelType.Gas95:
				heat = 1099500f;
				burnTimeOffset = 0.0008f;
				optimalMixtureRatio = 12.5f;
				burnTimeBase = 0.0025f;
				break;
			case FuelType.Gas98:
				heat = 1100000f;
				burnTimeOffset = 0.00075f;
				optimalMixtureRatio = 13.5f;
				burnTimeBase = 0.00247f;
				break;
			case FuelType.Gas100:
				heat = 1100250f;
				burnTimeOffset = 0.00065f;
				optimalMixtureRatio = 14f;
				burnTimeBase = 0.00242f;
				break;
			case FuelType.GasE85:
				heat = 1101000f;
				burnTimeOffset = 0.00052f;
				optimalMixtureRatio = 16f;
				burnTimeBase = 0.00235f;
				break;
			case FuelType.Diesel:
				heat = 1600000f;
				burnTimeOffset = 0.0002f;
				optimalMixtureRatio = 11f;
				burnTimeBase = 0.0034f;
				break;
			case FuelType.None:
				heat = 0f;
				burnTimeOffset = 0f;
				optimalMixtureRatio = 0f;
				burnTimeBase = 0f;
				break;
			default:
				heat = 1099500f;
				burnTimeOffset = 0.0008f;
				optimalMixtureRatio = 12.5f;
				burnTimeBase = 0.0025f;
				break;
			}
		}

		private void ApplyFuelInjectorSystem()
		{
			_mFc = FuelRail.GetOutputMFC;
			float num = 0f;
			GetFuelParams(GetBenzin, out var heat, out var burnTimeBase, out var burnTimeOffset, out var optimalMixtureRatio);
			GetFuelParams(FuelRail.GetFuelType, out var heat2, out var burnTimeBase2, out var burnTimeOffset2, out var optimalMixtureRatio2);
			num = (heat + heat2) * 0.5f;
			float num2 = (burnTimeOffset + burnTimeOffset2) * 0.5f;
			GetOMr = (optimalMixtureRatio + optimalMixtureRatio2) * 0.5f;
			float num3 = (burnTimeBase + burnTimeBase2) * 0.5f;
			GetMr = ((DynamicTuningSettings.MixtureRatio > 0f) ? DynamicTuningSettings.MixtureRatio : GetOMr);
			float num4 = Mathf.Clamp(GetMr, 8f, 20f);
			float num5 = ((!(num4 < GetOMr)) ? (GetOMr / num4) : (num4 / GetOMr));
			num5 = 0.45f * num5 + 0.55f;
			num5 = Mathf.Pow(num5, 4f);
			num5 = 1f - num5;
			_mH = num * (0.8f + 0.2f * (1f - num5)) * 1.55f;
			_tB = (num3 + num2 * num5) * 0.25f;
			GetMr = num4;
			_fcAcs = _mH * _vMin * _mFc * 2.5E-05f;
		}

		private void ApplyEcu(out float idleRpm, out float redlineRpm)
		{
			if (ECU == null)
			{
				_obd++;
				idleRpm = 0f;
				redlineRpm = 0f;
			}
			else
			{
				idleRpm = ((DynamicTuningSettings.IdleRpm > 0f) ? Mathf.Clamp(DynamicTuningSettings.IdleRpm, ECU.GetMinIdleRPM, ECU.GetMaxIdleRPM) : ECU.GetIdleRPM);
				redlineRpm = ((DynamicTuningSettings.RedlineRpm > 0f) ? Mathf.Clamp(DynamicTuningSettings.RedlineRpm, ECU.GetLimitRPM * 0.7f, ECU.GetMaxLimitRPM) : ECU.GetLimitRPM);
			}
		}

		private void ApplyCharger(out TurboType turboType, out int turboPressure, out float minTurboRpm, out float maxTurboRpm)
		{
			if (Charger == null || !Charger.Part)
			{
				turboType = TurboType.NA;
				turboPressure = 0;
				minTurboRpm = 0f;
				maxTurboRpm = 0f;
				return;
			}
			turboType = Charger.GetTurboType;
			int value;
			if (DynamicTuningSettings.Boost > 20000f)
			{
				value = Mathf.RoundToInt(DynamicTuningSettings.Boost * Charger.GetActualCondition);
			}
			else
			{
				Charger.GetActualPressure.Get(out value);
			}
			Charger.GetMaxSize.Get(out int value2);
			turboPressure = Mathf.Min(value, value2);
			minTurboRpm = Charger.GetMinRange;
			maxTurboRpm = Charger.GetMaxRange;
			if (!float.IsFinite(minTurboRpm) || !float.IsFinite(maxTurboRpm) || maxTurboRpm - minTurboRpm < 100f)
			{
				float num = ((ECU != null) ? ECU.GetIdleRPM : 800f);
				float num2 = ((ECU != null) ? ECU.GetLimitRPM : 7000f);
				minTurboRpm = Mathf.Max(num + 500f, 1500f);
				maxTurboRpm = Mathf.Max(minTurboRpm + 1000f, num2 - 500f);
				Debug.LogWarning($"[ApplyCharger] Charger '{Charger.Part?.name}' has degenerate RPM range; clamped to ({minTurboRpm:F0}, {maxTurboRpm:F0}).");
			}
		}

		public bool InstallPart(PartItem part)
		{
			bool result = false;
			if (GetInstallPartById(part.TypeId) != null)
			{
				PartData[] engineDetailsForUI = GetEngineDetailsForUI();
				for (int i = 0; i < engineDetailsForUI.Length; i++)
				{
				}
			}
			return result;
		}

		private PartData GetInstallPartById(PartType id)
		{
			PartData[] engineDetailsForUI = GetEngineDetailsForUI();
			int num = 0;
			if (num < engineDetailsForUI.Length)
			{
				PartData partData = engineDetailsForUI[num];
				if (partData.PartType == id)
				{
					return partData;
				}
			}
			return null;
		}

		private PartData[] GetEngineDetailsForUI()
		{
			return new List<PartData>
			{
				Block.Crankshaft.GetPartData(),
				Block.ConnectionRod.GetPartData(),
				Block.Pistons.GetPartData(),
				Block.Flywheel.GetPartData(),
				CylinderHead.GetPartData(),
				IntakeManifold.GetPartData(),
				ExhaustHeader.GetPartData()
			}.ToArray();
		}
	}
}

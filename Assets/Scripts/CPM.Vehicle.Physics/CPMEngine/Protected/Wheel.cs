using System;
using Unity.Mathematics;
using UnityEngine;

namespace CPMEngine.Protected
{
	public class Wheel : IWheel
	{
		private const float TransitionDamperVelocity = 0.1f;

		private const float MinTreadThickness = 0.001f;

		private const float WearCoef = 1E-09f;

		private const float TempRef = 60f;

		private const float TempScale = 20f;

		private const float MinTyreWearGrip = 0.15f;

		private const float GripCurveExponent = 1f;

		private const float ArcadeLongStiffness = 9f;

		private const float ArcadeLatStiffness = 12f;

		private const float ArcadeMuFront = 1.1f;

		private const float ArcadeMuRear = 1.3f;

		private const float ArcadeLatFadeSpeed = 2f;

		private const float ArcadeLowSpeedLongGain = 0.24f;

		private const float ArcadePlayerLongStiffness = 24f;

		private const float ArcadePlayerLatStiffnessFront = 22f;

		private const float ArcadePlayerLatStiffnessRear = 15f;

		private const float ArcadeLatPriority = 1.4f;

		private bool _digger;

		private bool _compression;

		private bool _rayTraceResult;

		private float _normalForce;

		private float _slipRatio;

		private float _slipAngle;

		private float _mass;

		private float _invMass;

		private float _tyrePressure = 200000f;

		private float _tyreThickness = 0.1f;

		private float _tyreSpringVertical = 150000f;

		private float _tyreSpringLateral = 50000f;

		private float _tyreSpringForward = 50000f;

		private float _tyreAbsorbVertical;

		private float _tyreAbsorbLateral;

		private float _tyreAbsorbForward;

		private float _maxRadius = 0.5f;

		private float _width = 0.2f;

		private float _rollAngle;

		private float _rollTyreAngle;

		private float _sideOffset;

		private float _sideTyreOffset;

		private float _brakeTorque;

		private float _inputBrakeTorque;

		private float _handBrakeTorque;

		private float _holdBrakeTorque;

		private float _sumBrakeTorque;

		private bool _absActive;

		internal const float EspSlipAllowance = 0.3f;

		private float _curRoadDist = 0.7f;

		private float _displacement;

		private float _tarSrVol;

		private float _tarSaVol;

		private Vector3 _flatTarget = Vector3.zero;

		private Vector3 _flatTarNorm = Vector3.zero;

		private Vector3 _target = Vector3.zero;

		private Vector3 _targetNormal = Vector3.zero;

		private Vector3 _targetVelocity = Vector3.zero;

		private Vector3 _steerVelocity = Vector3.zero;

		private Vector3 _casterDir = Vector3.zero;

		private Vector3 _fNdt = Vector3.zero;

		private Vector3 _sumVertForce = Vector3.zero;

		private Vector3 _topUp = Vector3.zero;

		private Vector3 _topRight = Vector3.zero;

		private Vector3 _topOrigin = Vector3.zero;

		private Matrix4x4 _topMatrix = Matrix4x4.identity;

		private Matrix4x4 _renderMatrix = Matrix4x4.identity;

		private Matrix4x4 _physMatrix = Matrix4x4.identity;

		private Vector3 _physUp = Vector3.zero;

		private Vector3 _physFront = Vector3.zero;

		private Vector3 _physRight = Vector3.zero;

		private Vector3 _physOrigin = Vector3.zero;

		private RayInfo _rayInfo;

		private readonly RayInfo[] _rayInfos = new RayInfo[5];

		private readonly bool[] _rayHits = new bool[5];

		private readonly float[] _rayDistances = new float[5];

		private readonly float[] _raySagittas = new float[5];

		private float _visualRoadDist;

		private Vector3 _visualRoadNormal = Vector3.up;

		private int _prevContactRay = -1;

		private float _w;

		private float _transTorque;

		private float _eps;

		private float _initialTreadThickness = 0.01f;

		private float _lastValue;

		private float _maxSpringLen = 0.2f;

		private float _treadThickness;

		private readonly PhysEngine _car;

		private readonly RB _body;

		private float _tyreHealth = 1f;

		private static readonly DisplacementMap DisplacementMap;

		private float _curSpringLen;

		private float _damperSpeed;

		private Matrix4x4 _rimColliderMatrix = Matrix4x4.identity;

		private const float SidewallDroopKneePa = 80000f;

		private const float SidewallDroopFloor = 0.35f;

		internal bool EspBraking { get; set; }

		public bool Enabled { get; set; }

		public bool StaticMode { get; private set; } = true;

		internal bool IsLeft { get; set; }

		internal bool IsTouch { get; set; }

		public bool SkidOnline { get; set; }

		public bool Compression => _compression;

		public bool OnGroundDown => IsTouch;

		public bool IsPowered { get; set; }

		public bool IsFront { get; set; }

		public float TyreTemperature { get; set; } = 25f;

		public float GripTemperature { get; set; } = 25f;

		public float SurfaceTemperature { get; set; } = 25f;

		public bool CarDrifting { get; set; }

		public float GetThreadThickness => _treadThickness;

		public float TyreHealth => _tyreHealth;

		public float TyreFriction
		{
			set
			{
				TyreSimulation.SetFricTempTyre(value);
			}
		}

		public float AdditionalRollingFriction { get; set; }

		public float AddSteerAngle { get; set; }

		public float AlignMom { get; set; }

		public float BestSlipAngle => TyreSimulation.GetBestSlipAngle();

		internal float InputBrakeTorque
		{
			get
			{
				return _inputBrakeTorque;
			}
			set
			{
				_inputBrakeTorque = Mathf.Max(0f, value);
			}
		}

		public float Camber { get; set; }

		internal float CamberAddDown { get; set; } = 5f;

		internal float CamberAddTop { get; set; } = -3f;

		public float CamberSusp { get; set; }

		public float Caster { get; set; }

		public Vector3 CasterPoint { get; private set; }

		public Vector3 CasterDir => _casterDir;

		internal float RadiusLoaded { get; private set; } = 0.5f;

		public float CurrentCamber { get; internal set; }

		public float CurSpringLen
		{
			get
			{
				return _curSpringLen;
			}
			private set
			{
				_curSpringLen = ((float.IsFinite(value) && Mathf.Abs(value) < 100f) ? value : 0f);
			}
		}

		public float CurSpringLengthReceived { get; set; }

		public float StaticSag { get; set; }

		private float DamperSpeed
		{
			get
			{
				return _damperSpeed;
			}
			set
			{
				_damperSpeed = ((float.IsFinite(value) && Mathf.Abs(value) < 1000f) ? value : 0f);
			}
		}

		internal float Eps
		{
			get
			{
				return _eps;
			}
			set
			{
				_eps = value;
			}
		}

		internal float ExtStabForce { get; set; }

		public float FrictionMultiplier
		{
			get
			{
				return TyreSimulation.GetFricMult();
			}
			set
			{
				TyreSimulation.SetFricMult(value);
			}
		}

		internal float HandBrakeTorque
		{
			get
			{
				return _handBrakeTorque;
			}
			set
			{
				_handBrakeTorque = Mathf.Max(0f, value);
				_sumBrakeTorque = _brakeTorque + _handBrakeTorque + _holdBrakeTorque;
			}
		}

		internal float HoldBrakeTorque
		{
			get
			{
				return _holdBrakeTorque;
			}
			set
			{
				_holdBrakeTorque = Mathf.Max(0f, value);
				_sumBrakeTorque = _brakeTorque + _handBrakeTorque + _holdBrakeTorque;
			}
		}

		internal float Inertia { get; private set; } = 1f;

		public Vector3 LatForce { get; set; }

		public Vector3 LongForce { get; set; }

		public float LongtitudeForce { get; private set; }

		public float LateralForce { get; private set; }

		public float NormalForce => _normalForce;

		internal float LongtitudeFricForce { get; private set; }

		public float Mass
		{
			get
			{
				return _mass;
			}
			set
			{
				_mass = Mathf.Max(value, 30f);
				_invMass = 1f / _mass;
				Inertia = Mathf.Max(0.7f * _mass * PhysMath.Sqr(_maxRadius), 0.5f);
			}
		}

		public float MaxSpringLen
		{
			get
			{
				return _maxSpringLen;
			}
			set
			{
				_maxSpringLen = Mathf.Clamp(value, 0.0254f, 0.635f);
			}
		}

		internal TyreSimulation TyreSimulation { get; }

		public PhysicMaterial CurrentPhysMat { get; } = new PhysicMaterial();

		public float Radius
		{
			get
			{
				return _maxRadius;
			}
			set
			{
				_maxRadius = value;
				_tyreThickness = Mathf.Max(TyreSimulation.GetProf() * 0.01f * _width, 0.0001f);
				Inertia = Mathf.Max(0.7f * _mass * PhysMath.Sqr(_maxRadius), 0.5f);
			}
		}

		public float RollAngle => _rollAngle;

		public float LongitudinalSlip { get; private set; }

		private float LateralSlip { get; set; }

		public float SlipAngleVolume { get; private set; }

		public float SlipRatioVolume { get; private set; }

		public float2 BumpRate { get; set; } = new float2(2000f, 4000f);

		public float2 ReboundRate { get; set; } = new float2(2000f, 4000f);

		public Vector3 SmokeVelocity { get; private set; }

		public float SpringCoef { get; set; } = 10000f;

		internal float SteerAngle { get; set; }

		internal float SteerSpeed { get; set; }

		internal float SumBrakeTorque => _sumBrakeTorque;

		internal float SuspKLong { get; set; }

		public float ToeIn { get; set; }

		public Vector3 TopPos { get; set; } = Vector3.zero;

		internal float TransTorque
		{
			get
			{
				return _transTorque;
			}
			set
			{
				_transTorque = value;
			}
		}

		public float TyrePressure
		{
			get
			{
				return _tyrePressure;
			}
			set
			{
				_tyrePressure = Mathf.Max(value, 10000f);
				_tyreThickness = Mathf.Max(TyreSimulation.GetProf() * 0.01f * _width, 0.01f);
			}
		}

		public Vector3 VertForce
		{
			get
			{
				return _sumVertForce;
			}
			set
			{
				_sumVertForce = value;
			}
		}

		public float AngularVelocity0 { get; set; }

		public float AngularVelocity
		{
			get
			{
				return _w;
			}
			set
			{
				if (PhysRuntimeFlags.IsCarInService)
				{
					_w = 0f;
				}
				else
				{
					_w = value;
				}
			}
		}

		public float SideVelocity { get; private set; }

		public float ForwardVelocity { get; private set; }

		public float WheelTireVelocity => AngularVelocity * RadiusLoaded;

		public float Width
		{
			get
			{
				return _width;
			}
			set
			{
				_width = Mathf.Max(value, 0.001f);
				_tyreThickness = Mathf.Max(TyreSimulation.GetProf() * 0.01f * _width, 0.0001f);
			}
		}

		public bool DiggerSurface => _digger;

		public bool NeedsOpenWheelCollider { get; set; }

		public float GetSkidVolume
		{
			get
			{
				float num = Mathf.Min(_sumVertForce.magnitude / 3000f, 2f);
				float num2 = SlipAngleVolume + SlipRatioVolume;
				float num3 = Mathf.Clamp01((PhysMath.ContactSlipSpeed(AngularVelocity, RadiusLoaded, ForwardVelocity, SideVelocity) - 1.2f) / 1.8f);
				return Mathf.Min(num2 * num * 0.35f, 1f) * num3;
			}
		}

		public void SetTyreHealth(float value, float defaultValue)
		{
			_initialTreadThickness = defaultValue;
			_tyreHealth = Mathf.Clamp01(value);
			float num = (_initialTreadThickness - 0.001f) * _tyreHealth;
			_treadThickness = num + 0.001f;
		}

		static Wheel()
		{
			DisplacementMap = new DisplacementMap();
		}

		internal Wheel(PhysEngine car)
		{
			_car = car;
			_body = _car.RigidBody;
			TyreSimulation = new TyreSimulation(this);
			CurSpringLen = MaxSpringLen / 3f;
			Mass = 30f;
		}

		private void BezierTess(ref RayInfo rti)
		{
			Vector3 vector = rti.V0;
			Vector3 v = rti.N0;
			Vector3 vector2 = rti.V1;
			Vector3 vector3 = rti.V2;
			Vector3 v2 = rti.N1;
			Vector3 v3 = rti.N2;
			Vector3 a = vector + (vector2 - vector) * rti.U + (vector3 - vector) * rti.V;
			if (rti.U < 0f)
			{
				vector2 = EvalBezierN(vector, vector2, v, v2, -1f);
				v2 = EvalBezierN(v, v2, v, v2, -1f);
				PhysMath.Vector3NormalizeSafe(ref v2);
				rti.U = 0f - rti.U;
			}
			if (rti.V < 0f)
			{
				vector3 = EvalBezierN(vector, vector3, v, v3, -1f);
				v3 = EvalBezierN(v, v3, v, v3, -1f);
				PhysMath.Vector3NormalizeSafe(ref v3);
				rti.V = 0f - rti.V;
			}
			if (Mathf.Abs(rti.U + rti.V) > 0.0001f)
			{
				float num = 1f / (rti.U + rti.V);
				Vector3 vector4 = vector + (vector2 - vector) * (num * rti.U) + (vector3 - vector) * (num * rti.V);
				float p = Vector3.Distance(a, rti.V0) / Vector3.Distance(rti.V0, vector4);
				float p2 = Vector3.Distance(vector4, vector2) / Vector3.Distance(vector2, vector3);
				Vector3 v4 = EvalBezierN(v2, v3, v2, v3, p2);
				PhysMath.Vector3Normalize(ref v4);
				Vector3 v5 = EvalBezierN(vector2, vector3, v2, v3, p2);
				vector = EvalBezierN(rti.V0, v5, rti.N0, v4, p);
				v = EvalBezierN(rti.N0, v4, rti.N0, v4, p);
				PhysMath.Vector3Normalize(ref v);
			}
			rti.Target = vector;
			rti.Normal = v;
		}

		private float CalcCasterAlignMom(Vector3 sumPatchForce)
		{
			float num = 0f - Vector3.Dot(_casterDir, _target);
			float sqrMagnitude = _casterDir.sqrMagnitude;
			if (sqrMagnitude < 1E-06f)
			{
				return 0f;
			}
			float num2 = (0f - (num + Vector3.Dot(_casterDir, CasterPoint))) / sqrMagnitude;
			Vector3 vector = CasterPoint + _casterDir * num2;
			float num3 = Vector3.Dot(_casterDir, Vector3.Cross(_target - vector, sumPatchForce));
			_steerVelocity = Vector3.Cross(vector - _target, _casterDir * SteerSpeed);
			return 0f - num3;
		}

		internal void CalculateFriction(float dt)
		{
			float radiusLoaded = RadiusLoaded;
			float angularVelocity = AngularVelocity;
			TyreSimulation tyreSimulation = TyreSimulation;
			PhysicMaterial currentPhysMat = CurrentPhysMat;
			Vector3 v = _physFront;
			v -= PhysMath.Vector3Mul(_targetNormal, Vector3.Dot(_targetNormal, v));
			if (v.sqrMagnitude < 1E-06f)
			{
				return;
			}
			PhysMath.Vector3NormalizeSafe(ref v);
			Vector3 vector = _targetVelocity - PhysMath.Vector3Mul(_targetNormal, Vector3.Dot(_targetNormal, _targetVelocity));
			Vector3 v2 = vector;
			bool num = vector.sqrMagnitude > 1E-06f;
			if (num)
			{
				PhysMath.Vector3NormalizeSafe(ref v2);
			}
			Vector3 vector2 = Vector3.Cross(v, _targetNormal) * -1f;
			_slipAngle = 0f;
			if (num)
			{
				Vector3 rhs = Vector3.Cross(v, v2) * -1f;
				_slipAngle = Mathf.Atan2(rhs.magnitude, Mathf.Abs(Vector3.Dot(v, v2)));
				if (Vector3.Dot(_targetNormal, rhs) < 0f)
				{
					_slipAngle = 0f - _slipAngle;
				}
			}
			float num2 = Mathf.Atan2(Vector3.Cross(_targetNormal, _physUp).magnitude, Mathf.Abs(Vector3.Dot(_targetNormal, _physUp)));
			if (Vector3.Dot(_physRight, _targetNormal) < 0f)
			{
				num2 = 0f - num2;
			}
			ForwardVelocity = Vector3.Dot(vector, v);
			float num3 = (SideVelocity = Vector3.Dot(vector, vector2));
			float num5 = (AngularVelocity0 = ForwardVelocity / radiusLoaded);
			float num7 = angularVelocity * radiusLoaded;
			SmokeVelocity = -v * num7 - vector;
			float num8 = Mathf.Abs(num5);
			float num9 = (_slipRatio = (angularVelocity - num5) / (num8 + 0.001f));
			bool flag = (StaticMode = !(num8 >= 18f) && !(Mathf.Abs(angularVelocity - num5) >= 6f) && Mathf.Abs(num3) < 1f);
			float num10 = (_normalForce = Mathf.Max(Vector3.Dot(_targetNormal, _fNdt) / dt, 0f));
			tyreSimulation.SetNormalForce(num10);
			tyreSimulation.SetSlipAngle(_slipAngle);
			tyreSimulation.SetSlipRatio(num9);
			tyreSimulation.SetCamber(num2);
			tyreSimulation.CalcForces(currentPhysMat);
			float num20;
			float num21;
			if (flag)
			{
				float fz = tyreSimulation.GetFz();
				Surface surfaceType = currentPhysMat.SurfaceType;
				float muStatLat = tyreSimulation.GetMuStatLat(0f, fz, surfaceType);
				float muStatLong = tyreSimulation.GetMuStatLong(0f, fz, surfaceType);
				_tarSaVol = 0f;
				_tarSrVol = 0f;
				LateralSlip = 0f;
				_sideOffset += num3 * dt;
				float num11 = muStatLat * num10 / _tyreSpringLateral;
				float num12 = muStatLong * num10 / _tyreSpringForward;
				if (_sideOffset > _sideTyreOffset + num11)
				{
					_sideTyreOffset = _sideOffset - num11;
					_tarSaVol = 0.7f;
				}
				else if (_sideOffset < _sideTyreOffset - num11)
				{
					_sideTyreOffset = _sideOffset + num11;
					_tarSaVol = 0.7f;
				}
				float num13 = _sideTyreOffset - _sideOffset;
				float num14 = (0f - Mathf.Abs(angularVelocity)) * radiusLoaded * 0.1f;
				if (num13 < 0f)
				{
					num14 = 0f - num14;
				}
				_sideTyreOffset += num14 * dt;
				float value = num13 * _tyreSpringLateral - num3 * _tyreAbsorbLateral;
				float num15 = num10 * muStatLat;
				float num16 = Mathf.Clamp(value, 0f - num15, num15);
				float num17 = num12 / radiusLoaded;
				LongitudinalSlip = num9;
				if (_rollAngle > _rollTyreAngle + num17)
				{
					_rollTyreAngle = _rollAngle - num17;
					_tarSrVol = 0.7f;
					LongitudinalSlip = 20f;
				}
				else if (_rollAngle < _rollTyreAngle - num17)
				{
					_rollTyreAngle = _rollAngle + num17;
					_tarSrVol = 0.7f;
					LongitudinalSlip = -20f;
				}
				float num18 = (_rollAngle - _rollTyreAngle) * radiusLoaded * _tyreSpringForward;
				float num19 = (angularVelocity - num5) * _tyreAbsorbForward * radiusLoaded;
				num15 = num10 * muStatLong;
				num20 = Mathf.Clamp(num18 + num19, 0f - num15, num15);
				num21 = num16;
				float num22 = Mathf.Sqrt(num20 * num20 + num21 * num21);
				float num23 = num10 * (muStatLat + muStatLong) * 0.5f;
				if (num22 > num23)
				{
					float num24 = num23 / num22;
					num20 *= num24;
					num21 *= num24;
				}
			}
			else
			{
				_tarSaVol = tyreSimulation.GetSaVolume();
				_tarSrVol = tyreSimulation.GetSrVolume();
				_sideOffset = 0f;
				_sideTyreOffset = 0f;
				_rollTyreAngle = _rollAngle;
				LongitudinalSlip = tyreSimulation.GetLongitudinalSlip();
				LateralSlip = tyreSimulation.GetLateralSlip();
				tyreSimulation.CombineForces(vector, v, vector2, num7);
				num20 = tyreSimulation.GetFx();
				num21 = tyreSimulation.GetFy();
			}
			float num25 = (LongtitudeFricForce = (WeatherPhysics.ApplyWetRollingFriction(currentPhysMat.RollingFriction, currentPhysMat.SurfaceType) + AdditionalRollingFriction) * num10);
			bool isCarInService = PhysRuntimeFlags.IsCarInService;
			bool flag3 = IsPowered && _car.GearboxState == GearboxState.Burnout;
			bool flag4 = IsPowered && PhysRuntimeFlags.DynorunMode;
			float num27 = ((isCarInService | flag4 | flag3) ? 0f : num20);
			float num28 = (isCarInService ? 0f : num21);
			LongtitudeForce = num27;
			LateralForce = num28;
			Vector3 vector3 = vector2 * num28;
			Vector3 vector4 = v * num27;
			Vector3 vector5 = vector3 + vector4 - v2 * num25;
			LongForce += vector4;
			LatForce += vector3;
			AlignMom += CalcCasterAlignMom(vector5 + _fNdt / dt);
			CurrentCamber += LateralSlip * num2;
			_car.RigidBody.AddExtForce(_target, vector5 * dt);
		}

		internal void CalcFrictionArcade(float dt)
		{
			Vector3 v = _physFront;
			v -= _targetNormal * Vector3.Dot(_targetNormal, v);
			if (v.magnitude < 0.001f)
			{
				return;
			}
			PhysMath.Vector3NormalizeSafe(ref v);
			Vector3 vector = _targetVelocity - _targetNormal * Vector3.Dot(_targetNormal, _targetVelocity);
			Vector3 v2 = vector;
			float num = vector.magnitude;
			if (num >= 0.001f)
			{
				PhysMath.Vector3NormalizeSafe(ref v2);
			}
			else
			{
				num = 0f;
			}
			Vector3 vector2 = Vector3.Cross(_targetNormal, v);
			_slipAngle = 0f;
			if (num != 0f)
			{
				Vector3 rhs = Vector3.Cross(v2, v);
				_slipAngle = Mathf.Atan2(rhs.magnitude, Mathf.Abs(Vector3.Dot(v, v2)));
				if (Vector3.Dot(_targetNormal, rhs) < 0f)
				{
					_slipAngle = 0f - _slipAngle;
				}
			}
			float num2 = Mathf.Atan2(Vector3.Cross(_targetNormal, _physUp).magnitude, Mathf.Abs(Vector3.Dot(_targetNormal, _physUp)));
			if (Vector3.Dot(_physRight, _targetNormal) < 0f)
			{
				num2 = 0f - num2;
			}
			ForwardVelocity = Vector3.Dot(vector, v);
			float num3 = (SideVelocity = Vector3.Dot(vector, vector2));
			AngularVelocity0 = ForwardVelocity / RadiusLoaded;
			SmokeVelocity = -v * (AngularVelocity * RadiusLoaded) - vector;
			_slipRatio = (AngularVelocity - AngularVelocity0) / (Mathf.Abs(AngularVelocity0) + 0.001f);
			StaticMode = !(Mathf.Abs(AngularVelocity0) >= 18f) && !(Mathf.Abs(AngularVelocity - AngularVelocity0) >= 6f) && Mathf.Abs(num3) < 1f;
			float num5 = (_car.ArcadeMode ? 24f : 9f);
			float num6 = ((!_car.ArcadeMode) ? 12f : (IsFront ? 22f : 15f));
			if (Mathf.Abs(AngularVelocity0) < 4f)
			{
				_slipRatio = (AngularVelocity - AngularVelocity0) * (0.24f / num5);
			}
			_normalForce = Mathf.Clamp(Vector3.Dot(_targetNormal, _fNdt) / dt, 0f, 10000f);
			float num7;
			float num8;
			if (_car.ArcadeMode)
			{
				float verticalLoad = _normalForce * 0.001f;
				Surface surfaceType = CurrentPhysMat.SurfaceType;
				float fricScale = TyreSimulation.GetFricScale(CurrentPhysMat);
				num7 = TyreSimulation.GetMuStatLong(num, verticalLoad, surfaceType) * fricScale;
				num8 = TyreSimulation.GetMuStatLat(num, verticalLoad, surfaceType) * fricScale;
			}
			else
			{
				num7 = (num8 = (IsFront ? 1.1f : 1.3f));
			}
			float num9 = num7 * _normalForce;
			float num10 = num8 * _normalForce;
			float num11 = _slipRatio * _normalForce * num5;
			float num12 = _slipAngle * _normalForce * num6;
			num12 *= Mathf.Clamp01(num / 2f);
			LongtitudeForce = ((num9 > 0.001f) ? (num9 * (float)Math.Tanh(num11 / num9)) : 0f);
			LateralForce = ((num10 > 0.001f) ? (num10 * (float)Math.Tanh(num12 / num10)) : 0f);
			bool flag = num9 > 0.001f && num10 > 0.001f && PhysMath.Sqr(LateralForce / num10) + PhysMath.Sqr(LongtitudeForce / num9) > 1f;
			_tarSaVol = 0f;
			_tarSrVol = 0f;
			LongitudinalSlip = _slipRatio * 2f;
			LateralSlip = _slipAngle * 10f;
			if (Mathf.Abs(LateralSlip) > 1f)
			{
				_tarSaVol = Mathf.Abs(LateralSlip) - 1f;
			}
			if (Mathf.Abs(LongitudinalSlip) > 1f)
			{
				_tarSrVol = Mathf.Abs(LongitudinalSlip) - 1f;
			}
			if (StaticMode)
			{
				_tarSaVol = 0f;
				LateralSlip = 0f;
				_sideOffset += num3 * dt;
				float num13 = num10 / _tyreSpringLateral;
				if (_sideOffset > _sideTyreOffset + num13)
				{
					_sideTyreOffset = _sideOffset - num13;
					_tarSaVol = 0.7f;
				}
				if (_sideOffset < _sideTyreOffset - num13)
				{
					_sideTyreOffset = _sideOffset + num13;
					_tarSaVol = 0.7f;
				}
				float num14 = _sideTyreOffset - _sideOffset;
				float num15 = (0f - Mathf.Abs(AngularVelocity)) * RadiusLoaded * 0.1f;
				if (num14 < 0f)
				{
					num15 = 0f - num15;
				}
				_sideTyreOffset += num15 * dt;
				num13 = num14 * _tyreSpringLateral;
				float num16 = num13 + (0f - num3) * _tyreAbsorbLateral;
				float lateralForce = ((!(num16 <= num10)) ? num10 : ((num16 >= 0f - num10) ? num16 : (0f - num10)));
				LateralForce = lateralForce;
			}
			else
			{
				_sideOffset = 0f;
				_sideTyreOffset = 0f;
				_rollTyreAngle = _rollAngle;
			}
			float num17 = (WeatherPhysics.ApplyWetRollingFriction(CurrentPhysMat.RollingFriction, CurrentPhysMat.SurfaceType) + AdditionalRollingFriction) * _normalForce;
			LongtitudeFricForce = 0f;
			if (StaticMode)
			{
				_tarSrVol = 0f;
				LongitudinalSlip = _slipRatio;
				float num18 = num9 / _tyreSpringForward;
				float num19 = num18 / RadiusLoaded;
				if (_rollAngle > _rollTyreAngle + num19)
				{
					_rollTyreAngle = _rollAngle - num19;
					_tarSrVol = 0.7f;
					LongitudinalSlip = 20f;
				}
				if (_rollAngle < _rollTyreAngle - num19)
				{
					_rollTyreAngle = _rollAngle + num19;
					_tarSrVol = 0.7f;
					LongitudinalSlip = -20f;
				}
				num18 = (_rollAngle - _rollTyreAngle) * RadiusLoaded * _tyreSpringForward;
				float num20 = (AngularVelocity - AngularVelocity0) * RadiusLoaded * _tyreAbsorbForward;
				float longtitudeForce = ((!(num18 + num20 <= num9)) ? num9 : ((num18 + num20 >= 0f - num9) ? (num18 + num20) : (0f - num9)));
				LongtitudeForce = longtitudeForce;
			}
			LateralForce = Mathf.Clamp(LateralForce, 0f - num10, num10);
			LongtitudeForce = Mathf.Clamp(LongtitudeForce, 0f - num9, num9);
			if (StaticMode)
			{
				float num21 = Mathf.Sqrt(LateralForce * LateralForce + LongtitudeForce * LongtitudeForce);
				float num22 = 0.5f * (num10 + num9);
				if (num21 > num22)
				{
					float num23 = num22 / num21;
					LateralForce *= num23;
					LongtitudeForce *= num23;
				}
			}
			else if (flag)
			{
				float num24 = Mathf.Abs(Vector3.Dot(v, vector) - AngularVelocity * RadiusLoaded);
				float num25 = Mathf.Abs(Vector3.Dot(vector2, vector));
				float num26 = Mathf.Sqrt(num24 * num24 + num25 * num25);
				if (num26 > 0.001f)
				{
					float num27 = 1f / num26;
					num24 *= num27;
					num25 *= num27;
					if (_car.ArcadeMode)
					{
						num25 = Mathf.Min(num25 * 1.4f, 1f);
						num24 = Mathf.Sqrt(Mathf.Max(1f - num25 * num25, 0f));
					}
					LongtitudeForce = num9 * Mathf.Sign(LongtitudeForce) * Mathf.Abs(num24);
					LateralForce = num10 * Mathf.Sign(LateralForce) * Mathf.Abs(num25);
				}
			}
			if (PhysRuntimeFlags.DynorunMode)
			{
				LongtitudeForce = 0f;
			}
			Vector3 vector3 = vector2 * LateralForce;
			Vector3 vector4 = v * LongtitudeForce;
			Vector3 vector5 = vector3 + vector4 - v2 * num17;
			LongForce += vector4;
			LatForce += vector3;
			AlignMom += CalcCasterAlignMom(vector5 + _fNdt / dt);
			num2 = LateralSlip * num2;
			CurrentCamber += num2;
			_car.RigidBody.AddExtForce(_target, PhysMath.Vector3Mul(vector5, dt));
		}

		internal void CalcFrictionDigger(float dt)
		{
			if (_digger)
			{
				Vector3 v = _physFront - PhysMath.Vector3Mul(_targetNormal, Vector3.Dot(_targetNormal, _physFront));
				if (!(v.magnitude < 0.001f))
				{
					PhysMath.Vector3NormalizeSafe(ref v);
					Vector3 vector = Vector3.Cross(_targetNormal, v);
					Vector3 vector2 = _targetVelocity - PhysMath.Vector3Mul(_targetNormal, Vector3.Dot(_targetNormal, _targetVelocity));
					AngularVelocity0 = Vector3.Dot(vector2, v) / RadiusLoaded;
					SmokeVelocity = -v * (AngularVelocity * RadiusLoaded) - vector2;
					Vector3 rhs = PhysMath.Vector3Mul(d: 10f * (1f + _displacement * 40f), a: -vector2);
					float d = Vector3.Dot(v, rhs);
					float d2 = Vector3.Dot(vector, rhs);
					Vector3 vector3 = PhysMath.Vector3Mul(v, d);
					Vector3 vector4 = PhysMath.Vector3Mul(vector, d2);
					Vector3 vector5 = (vector4 + vector3) * dt;
					LongForce += vector3;
					LatForce += vector4;
					AlignMom += CalcCasterAlignMom((vector5 + _fNdt) / dt);
					_car.RigidBody.AddExtForce(_target, vector5);
				}
			}
		}

		internal void CalcSound(float dt)
		{
			float maxDelta = 20f * dt;
			SlipAngleVolume = Mathf.MoveTowards(SlipAngleVolume, _tarSaVol, maxDelta);
			SlipRatioVolume = Mathf.MoveTowards(SlipRatioVolume, _tarSrVol, maxDelta);
		}

		private Vector3 EvalBezierN(Vector3 v0, Vector3 v1, Vector3 n0, Vector3 n1, float p)
		{
			float num = 1f - p;
			Vector3 a = PhysMath.Vector3Mul(v0, 2f) + v1 - PhysMath.Vector3Mul(n0, Vector3.Dot(v1 - v0, n0));
			Vector3 a2 = PhysMath.Vector3Mul(v1, 2f) + v0 - PhysMath.Vector3Mul(n1, Vector3.Dot(v0 - v1, n1));
			return PhysMath.Vector3Mul(v0, Mathf.Pow(num, 3f)) + PhysMath.Vector3Mul(v1, Mathf.Pow(p, 3f)) + PhysMath.Vector3Mul(a, PhysMath.Sqr(num) * p) + PhysMath.Vector3Mul(a2, num * PhysMath.Sqr(p));
		}

		public bool GetDeform(out Vector3 patchCnt, out Vector3 latDeformVec, out Vector3 longDeformVec, out Plane vertDeformPlane)
		{
			if (!OnGroundDown)
			{
				longDeformVec = Vector3.zero;
				latDeformVec = Vector3.zero;
				patchCnt = Vector3.zero;
				vertDeformPlane = default;
				return false;
			}
			GetPatch(out var _, out var _, out var cnt, out var n);
			Matrix4x4 inverse = _renderMatrix.inverse;
			patchCnt = inverse.MultiplyPoint3x4(cnt);
			Vector3 vector = inverse.MultiplyVector(n);
			vertDeformPlane = new Plane(vector, patchCnt);
			patchCnt.x = 0f;
			PhysMath.Vector3Normalize(ref patchCnt);
			patchCnt *= _maxRadius;
			Vector3 v = inverse.MultiplyVector(LatForce);
			Vector3 a = inverse.MultiplyVector(LongForce);
			Vector3 a2 = -Vector3.Cross(vector, Vector3.right);
			if (StaticMode)
			{
				float d = _sideTyreOffset - _sideOffset;
				PhysMath.Vector3NormalizeSafe(ref v);
				latDeformVec = PhysMath.Vector3Mul(Vector3.right, d);
				float d2 = (_rollAngle - _rollTyreAngle) * RadiusLoaded;
				longDeformVec = PhysMath.Vector3Mul(a2, d2);
			}
			else
			{
				float magnitude = v.magnitude;
				float magnitude2 = a.magnitude;
				latDeformVec = ((magnitude > 1E-06f) ? PhysMath.Vector3Mul(v, PhysMath.TreadShearMagnitude(magnitude, _tyreSpringLateral, _tyreThickness) / magnitude) : Vector3.zero);
				longDeformVec = ((magnitude2 > 1E-06f) ? PhysMath.Vector3Mul(a, PhysMath.TreadShearMagnitude(magnitude2, _tyreSpringForward, _tyreThickness) / magnitude2) : Vector3.zero);
			}
			return true;
		}

		public bool GetMarkPatch(out Vector3 left, out Vector3 right, out Vector3 cnt, out Vector3 n)
		{
			Vector3 v = _physFront - PhysMath.Vector3Mul(_flatTarNorm, Vector3.Dot(_flatTarNorm, _physFront));
			if (!IsTouch || v.magnitude < 0.001f)
			{
				n = (cnt = (right = (left = Vector3.zero)));
				return false;
			}
			PhysMath.Vector3NormalizeSafe(ref v);
			Vector3 vector = _targetVelocity - PhysMath.Vector3Mul(_flatTarNorm, Vector3.Dot(_flatTarNorm, _targetVelocity));
			vector = ((vector.magnitude >= 0.001f) ? vector.normalized : v);
			float d = (Mathf.Abs(Vector3.Dot(v, vector)) * 0.7f + 0.3f) * _width * 0.5f;
			Vector3 a = Vector3.Cross(_flatTarNorm, vector);
			left = _flatTarget - PhysMath.Vector3Mul(a, d);
			right = _flatTarget + PhysMath.Vector3Mul(a, d);
			cnt = _flatTarget;
			n = _flatTarNorm;
			return true;
		}

		public bool GetPatch(out Vector3 left, out Vector3 right, out Vector3 cnt, out Vector3 n)
		{
			Vector3 v = _physFront - PhysMath.Vector3Mul(_targetNormal, Vector3.Dot(_targetNormal, _physFront));
			if (!IsTouch || v.magnitude < 0.001f)
			{
				n = (cnt = (right = (left = Vector3.zero)));
				return false;
			}
			PhysMath.Vector3NormalizeSafe(ref v);
			Vector3 vector = _targetVelocity - PhysMath.Vector3Mul(_targetNormal, Vector3.Dot(_targetNormal, _targetVelocity));
			Vector3 rhs = ((vector.magnitude >= 0.001f) ? vector.normalized : v);
			float d = (Mathf.Abs(Vector3.Dot(v, rhs)) * 0.7f + 0.3f) * _width * 0.5f;
			Vector3 a = Vector3.Cross(_targetNormal, rhs);
			left = _target - PhysMath.Vector3Mul(a, d);
			right = _target + PhysMath.Vector3Mul(a, d);
			Vector3 origin = PhysMath.GetOrigin(_renderMatrix);
			float d2 = Mathf.Clamp(_visualRoadDist - CurSpringLen, 0.001f, _maxRadius);
			origin -= PhysMath.Vector3Mul(_physUp, d2);
			cnt = origin;
			Vector3 roadNormal = ((_visualRoadNormal.sqrMagnitude > 1E-06f) ? _visualRoadNormal : _targetNormal);
			n = PhysMath.ClampVisualContactNormal(roadNormal, _physUp, 0.57f);
			return true;
		}

		public int GetRayCount()
		{
			if (!_car.TrafficMode && !_car.RaceBotMode)
			{
				return PhysMath.ActiveWheelContactRayCount;
			}
			return 1;
		}

		public bool GetRayTracePoint(int index, out Vector3 p1, out Vector3 p2)
		{
			if (index == 0)
			{
				_rayTraceResult = false;
				for (int i = 0; i < _rayHits.Length; i++)
				{
					_rayHits[i] = false;
				}
			}
			p1 = Vector3.zero;
			p2 = Vector3.zero;
			if (!Enabled)
			{
				return false;
			}
			if (index == 0)
			{
				UpdateTopMatrix();
			}
			PhysMath.WheelContactRayOffset(index, _width, _maxRadius, out var lateral, out var longitudinal);
			_raySagittas[index] = PhysMath.WheelContactRaySagitta(longitudinal, _maxRadius);
			Vector3 v = _topRight;
			PhysMath.Vector3NormalizeSafe(ref v);
			Vector3 v2 = PhysMath.GetForward(_topMatrix);
			PhysMath.Vector3NormalizeSafe(ref v2);
			p1 = _topOrigin + PhysMath.Vector3Mul(v, lateral) + PhysMath.Vector3Mul(v2, longitudinal);
			float num = _maxRadius + MaxSpringLen;
			p2 = p1 - PhysMath.Vector3Mul(_topUp, num + 0.99915f);
			return true;
		}

		public void GetPositionAndRotation(out Vector3 pos, out Quaternion rot)
		{
			pos = PhysMath.GetOrigin(_renderMatrix);
			rot = PhysMath.QuaternionFromMatrix(_renderMatrix);
		}

		public void GetRimColliderPositionAndRotation(out Vector3 pos, out Quaternion rot)
		{
			pos = PhysMath.GetOrigin(_rimColliderMatrix);
			rot = PhysMath.QuaternionFromMatrix(_rimColliderMatrix);
		}

		public bool GetSmoke()
		{
			if (!OnGroundDown)
			{
				return false;
			}
			float a = PhysMath.TyreLongSlidingShare(AngularVelocity, RadiusLoaded, ForwardVelocity);
			float b = (CarDrifting ? PhysMath.TyreLatSlidingShare(ForwardVelocity, SideVelocity) : 0f);
			if (PhysMath.ContactSlipSpeed(AngularVelocity, RadiusLoaded, ForwardVelocity, SideVelocity) > 4f && Mathf.Max(a, b) >= 0.5f)
			{
				return true;
			}
			if (Mathf.Abs(LongitudinalSlip) + Mathf.Abs(LateralSlip) < 4f)
			{
				return false;
			}
			if (Mathf.Abs(AngularVelocity) < 10f && Mathf.Abs(AngularVelocity0) < 10f)
			{
				return false;
			}
			return SlipAngleVolume + SlipRatioVolume > 0.1f;
		}

		public bool GetUserData(out int userData)
		{
			if (!IsTouch)
			{
				userData = 0;
				return false;
			}
			userData = _rayInfo.UserData;
			return true;
		}

		private static bool IsFinite(Vector3 v)
		{
			if (float.IsFinite(v.x) && float.IsFinite(v.y))
			{
				return float.IsFinite(v.z);
			}
			return false;
		}

		private static float LowPressureSidewallDroop(float pressurePa)
		{
			return Mathf.Clamp(pressurePa / 80000f, 0.35f, 1f);
		}

		internal void CalculateSuspensionForce(float dt)
		{
			Vector3 velocity = _body.GetVelocity(_topOrigin);
			float num = (IsTouch ? Vector3.Dot(_targetNormal, velocity) : 0f);
			_curRoadDist += num * dt;
			float num2 = 0f;
			if (IsTouch)
			{
				RadiusLoaded = Mathf.Max(_curRoadDist - CurSpringLen, 0.001f);
				float num3 = _maxRadius - RadiusLoaded;
				IsTouch = num3 > 0.0001f;
				if (IsTouch)
				{
					_targetVelocity = _body.GetVelocity(_target);
					_targetVelocity += _steerVelocity;
					float num4 = _tyrePressure * LowPressureSidewallDroop(_tyrePressure);
					float a = 0.1f * _mass * PhysMath.Sqr(AngularVelocity) * _maxRadius / (6.28318f * _maxRadius * _width);
					a = Mathf.Min(a, 100000f);
					num4 += a;
					float num5 = Mathf.Max(0.5f * _width * num4 / _tyreThickness, 1f);
					_tyreSpringVertical = (_tyreSpringForward = (_tyreSpringLateral = num5));
					float supportedMass = PhysMath.TyreSupportedMass(num5, num3, _tyreThickness, _mass);
					_tyreAbsorbVertical = PhysMath.TyreVerticalDamping(num5, num3, _tyreThickness, _mass, dt);
					float dampingRatio = PhysMath.TyreDampingRatio(num3, _tyreThickness);
					_tyreAbsorbForward = PhysMath.TyreDamping(_tyreSpringForward, supportedMass, dampingRatio);
					_tyreAbsorbLateral = PhysMath.TyreDamping(_tyreSpringLateral, supportedMass, dampingRatio);
					float num6 = (0f - DamperSpeed) * _tyreAbsorbVertical;
					num2 += PhysMath.TyreVerticalSpringForce(num3, _tyreThickness, num5) + num6;
				}
			}
			float num7 = DamperSpeed - num;
			float num8 = ((!(num7 <= 0f)) ? ((num7 >= 0.1f) ? ((0f - (num7 - 0.1f)) * BumpRate.x - 0.1f * BumpRate.y) : ((0f - num7) * BumpRate.y)) : ((num7 <= -0.1f) ? ((0f - (num7 + 0.1f)) * ReboundRate.x + 0.1f * ReboundRate.y) : ((0f - num7) * ReboundRate.y)));
			num8 *= PhysMath.SuspensionBumpDampingScale(MaxSpringLen - CurSpringLen, MaxSpringLen, StaticSag);
			float num9 = 0f - PhysMath.SuspensionSpringForce(MaxSpringLen - CurSpringLen, MaxSpringLen, SpringCoef, StaticSag);
			num9 += ExtStabForce;
			float num10 = 0f;
			if (CurSpringLen <= 0f)
			{
				CurSpringLen = 0f;
				num10 = Mathf.Max(num2 + num9, 0f);
				num8 = 0f;
			}
			float num11 = num9 + num8 - num10;
			float num12 = 0f - num11;
			float num13 = (num11 + num2) * _invMass;
			DamperSpeed += num13 * dt;
			CurSpringLen -= (DamperSpeed - num) * dt;
			if (CurSpringLen > MaxSpringLen)
			{
				CurSpringLen = MaxSpringLen;
				DamperSpeed = num;
				num12 = 0f;
				num2 = 0f;
			}
			if (IsTouch)
			{
				float num14 = Vector3.Dot(_targetNormal, _topUp);
				num12 *= num14;
				_fNdt = PhysMath.Vector3Mul(_targetNormal, num2 * num14 * dt);
				Vector3 vector = PhysMath.Vector3Mul(_targetNormal, num12 * dt);
				Vector3 target = _target;
				if (IsFinite(vector) && IsFinite(target))
				{
					_body.AddExtForce(target, vector);
				}
			}
			else
			{
				_fNdt = PhysMath.Vector3Mul(_targetNormal, num2 * dt);
				Vector3 vector2 = PhysMath.Vector3Mul(_topUp, num12 * dt);
				Vector3 physOrigin = _physOrigin;
				if (IsFinite(vector2) && IsFinite(physOrigin))
				{
					_body.AddExtForce(physOrigin, vector2);
				}
			}
			DoEps(dt);
			_sumVertForce += _fNdt / dt;
			_compression = num8 > num12;
		}

		internal void CalculateSuspensionArcade(float dt)
		{
			if (!float.IsFinite(_curRoadDist))
			{
				_curRoadDist = 0f;
			}
			if (!float.IsFinite(CurSpringLen))
			{
				CurSpringLen = MaxSpringLen;
			}
			if (!float.IsFinite(DamperSpeed))
			{
				DamperSpeed = 0f;
			}
			if (!IsTouch)
			{
				CurSpringLen = MaxSpringLen;
				return;
			}
			Vector3 velocity = _body.GetVelocity(_topOrigin);
			float num = Vector3.Dot(_topUp, velocity);
			_curRoadDist += num * dt;
			RadiusLoaded = Mathf.Max(_curRoadDist - CurSpringLen, 0.001f);
			float deflection = _maxRadius - RadiusLoaded;
			float num2 = _tyrePressure * LowPressureSidewallDroop(_tyrePressure);
			float a = 0.1f * _mass * PhysMath.Sqr(AngularVelocity) * _maxRadius / (6.28318f * _maxRadius * _width);
			a = Mathf.Min(a, 100000f);
			num2 += a;
			float num3 = 0.5f * _width * num2 / _tyreThickness;
			_tyreSpringVertical = (_tyreSpringForward = (_tyreSpringLateral = num3));
			float supportedMass = PhysMath.TyreSupportedMass(num3, deflection, _tyreThickness, _mass);
			float dampingRatio = PhysMath.TyreDampingRatio(deflection, _tyreThickness);
			_tyreAbsorbVertical = PhysMath.TyreVerticalDamping(num3, deflection, _tyreThickness, _mass, dt);
			_tyreAbsorbForward = PhysMath.TyreDamping(_tyreSpringForward, supportedMass, dampingRatio);
			_tyreAbsorbLateral = PhysMath.TyreDamping(_tyreSpringLateral, supportedMass, dampingRatio);
			float num4 = (0f - DamperSpeed) * _tyreAbsorbVertical;
			float num5 = PhysMath.TyreVerticalSpringForce(deflection, _tyreThickness, num3) + num4;
			float num6 = DamperSpeed - num;
			float num7 = ((!(num6 <= 0f)) ? ((num6 >= 0.1f) ? ((0f - (num6 - 0.1f)) * BumpRate.x - 0.1f * BumpRate.y) : ((0f - num6) * BumpRate.y)) : ((num6 <= -0.1f) ? ((0f - (num6 + 0.1f)) * ReboundRate.x + 0.1f * ReboundRate.y) : ((0f - num6) * ReboundRate.y)));
			num7 *= PhysMath.SuspensionBumpDampingScale(MaxSpringLen - CurSpringLen, MaxSpringLen, StaticSag);
			float num8 = 0f - PhysMath.SuspensionSpringForce(MaxSpringLen - CurSpringLen, MaxSpringLen, SpringCoef, StaticSag);
			num8 += ExtStabForce;
			num8 += LongtitudeForce * SuspKLong;
			float num9 = 0f;
			if (CurSpringLen <= 0f)
			{
				CurSpringLen = 0f;
				num9 = Mathf.Max(num5 + num8, 0f);
				num7 = 0f;
			}
			float num10 = num8 + num7 - num9;
			float num11 = 0f - num10;
			float num12 = (num10 + num5) * _invMass;
			DamperSpeed += num12 * dt;
			CurSpringLen -= (DamperSpeed - num) * dt;
			if (CurSpringLen > MaxSpringLen)
			{
				CurSpringLen = MaxSpringLen;
				DamperSpeed = num;
				num11 = 0f;
				num5 = 0f;
			}
			float num13 = Vector3.Dot(_targetNormal, _topUp);
			num11 *= num13;
			_fNdt = PhysMath.Vector3Mul(_targetNormal, num5 * num13 * dt);
			_body.AddExtForce(_target, PhysMath.Vector3Mul(_targetNormal, num11 * dt));
			DoEps(dt);
			_sumVertForce += _fNdt / dt;
			_compression = num7 > num11;
		}

		private void DoEps(float dt)
		{
			float num = _mass * RadiusLoaded * RadiusLoaded * 0.7f * Mathf.Abs(AngularVelocity);
			Vector3 angularVelocity = _body.AngularVelocity;
			angularVelocity -= _topRight * Vector3.Dot(angularVelocity, _topRight);
			Vector3 vector = angularVelocity * (0f - num);
			Vector3 vector2 = _topRight * (Inertia * Eps * -1f);
			Vector3 tdt = (vector + vector2) * dt;
			if (!float.IsFinite(tdt.x) || !float.IsFinite(tdt.y) || !float.IsFinite(tdt.z))
			{
				tdt = Vector3.zero;
			}
			_body.AddExtTorque(tdt);
		}

		public void SetRayTraceResult(int index, bool result, RayInfo info)
		{
			_rayHits[index] = result;
			_rayInfos[index] = (result ? info : default(RayInfo));
			_rayDistances[index] = (result ? Vector3.Dot(-_topUp, info.Target - _topOrigin) : 0f);
		}

		public void SelectContactRay()
		{
			int num = (_prevContactRay = PhysMath.SelectWheelContactRay(_rayDistances, _rayHits, _raySagittas, GetRayCount(), _prevContactRay));
			_rayTraceResult = num >= 0;
			if (_rayTraceResult)
			{
				RayInfo rayInfo = _rayInfos[num];
				Vector3 v = _topUp;
				PhysMath.Vector3NormalizeSafe(ref v);
				rayInfo.Target -= PhysMath.Vector3Mul(v, _raySagittas[num]);
				_rayInfo = rayInfo;
				bool flag = _rayHits[0];
				_visualRoadDist = (flag ? _rayDistances[0] : _rayDistances[num]);
				_visualRoadNormal = (flag ? _rayInfos[0].Normal : rayInfo.Normal);
			}
			else
			{
				_rayInfo = default;
				_visualRoadDist = 0f;
				_visualRoadNormal = Vector3.zero;
			}
			if (_rayInfo.Tessellate)
			{
				BezierTess(ref _rayInfo);
			}
		}

		internal bool UpdateAbs()
		{
			float num = InputBrakeTorque;
			int absLevel = _car.AbsLevel;
			if (!PhysMath.AbsIsArmed(IsTouch, absLevel, _car.ParkingGear, num, AngularVelocity0))
			{
				_brakeTorque = num;
				_sumBrakeTorque = num + _handBrakeTorque + _holdBrakeTorque;
				if (!IsTouch && _sumBrakeTorque > 0f)
				{
					float num2 = PhysMath.AirborneBrakeCap(Inertia, AngularVelocity);
					if (_sumBrakeTorque > num2)
					{
						_brakeTorque *= num2 / _sumBrakeTorque;
						_sumBrakeTorque = num2;
					}
				}
				_absActive = false;
				return false;
			}
			float num3 = Mathf.Abs(AngularVelocity0);
			float num4 = Mathf.Clamp01((num3 - Mathf.Abs(AngularVelocity)) / num3);
			float t = (float)(absLevel - 1) / 9f;
			float num5 = Mathf.Lerp(0.2f, 0.05f, t);
			if (EspBraking)
			{
				num5 = Mathf.Max(num5, 0.3f);
			}
			float num6 = num5 * 0.35f;
			if (!_absActive && num4 > num5)
			{
				_absActive = true;
			}
			else if (_absActive && num4 < num6)
			{
				_absActive = false;
			}
			if (_absActive)
			{
				float num7 = 1f - Mathf.Clamp01((num4 - num6) / (num5 - num6));
				num *= num7 * 0.15f;
			}
			_brakeTorque = num;
			_sumBrakeTorque = num + _handBrakeTorque + _holdBrakeTorque;
			return _absActive;
		}

		public void UpdateTyreWear(float dt)
		{
			if (!(dt <= 0f) && OnGroundDown)
			{
				float num = Mathf.Abs(AngularVelocity) * Mathf.Sqrt(_slipRatio * _slipRatio + _slipAngle * _slipAngle);
				if (!float.IsNaN(_normalForce) && !float.IsNaN(LongtitudeForce) && !float.IsNaN(LateralForce) && !float.IsNaN(num) && !float.IsNaN(_normalForce / _tyrePressure) && !float.IsNaN(GripTemperature))
				{
					float num2 = Mathf.Max(_normalForce / _tyrePressure, 1E-06f);
					num = Mathf.Max(0f, num);
					num = Mathf.Min(num, 5f);
					float num3 = Mathf.Sqrt(LongtitudeForce * LongtitudeForce + LateralForce * LateralForce) * 0.1f * num;
					float num4 = 1E-09f * num3 / num2;
					float value = Mathf.Exp((GripTemperature - 60f) / 20f);
					value = Mathf.Clamp(value, 0.1f, 10f);
					float num5 = 2.5E-11f * _normalForce * num / num2;
					float num6 = Mathf.Abs(PhysMath.Mm2Meter((num4 + num5) * value * dt));
					_treadThickness -= num6;
					_treadThickness = Mathf.Max(_treadThickness, 0.001f);
					float num7 = Mathf.Max(_treadThickness - 0.001f, 0f);
					float num8 = _initialTreadThickness - 0.001f;
					_tyreHealth = ((num8 > 0f) ? Mathf.Clamp01(num7 / num8) : 0f);
					float fricWearTyre = Mathf.Lerp(0.15f, 0f, Mathf.Pow(_tyreHealth, 1f));
					TyreSimulation.SetFricWearTyre(fricWearTyre);
				}
			}
		}

		internal void UpdatePhysGroundMatrix_Advanced()
		{
			Matrix4x4 m = Matrix4x4.identity;
			Matrix4x4 m2 = Matrix4x4.identity;
			float num = CurSpringLen / MaxSpringLen;
			float num2 = CamberSusp + Camber + CamberAddDown * num + CamberAddTop * (1f - num);
			Vector3 right = Vector3.right;
			Vector3 up = Vector3.up;
			Vector3 forward = Vector3.forward;
			float num3 = (IsLeft ? 1f : (-1f));
			Vector3 vector = right * (0.05f * num3);
			PhysMath.MatrixRotateFast(ref m, _rollAngle, right, isDegree: false);
			PhysMath.MatrixTranslate(ref m, -vector);
			PhysMath.MatrixRotateFast(ref m, Caster, right);
			PhysMath.MatrixRotateFast(ref m, 0f - SteerAngle, up, isDegree: false);
			PhysMath.MatrixRotateFast(ref m, 0f - Caster, right);
			PhysMath.MatrixTranslate(ref m, vector);
			PhysMath.MatrixRotateFast(ref m, num2 * num3, forward);
			Vector3 right2 = PhysMath.GetRight(m);
			Vector3 v = up;
			v -= right2 * Vector3.Dot(v, right2);
			PhysMath.Vector3NormalizeSafe(ref v);
			Vector3 oz = Vector3.Cross(right2, v);
			Vector3 vector2 = ((!IsLeft) ? (TopPos + right * (_width * 0.3f)) : (TopPos - right * (_width * 0.3f)));
			vector2 -= up * CurSpringLen;
			PhysMath.MatrixTransform(ref m, vector2, right2, v, oz);
			Matrix4x4 matrix = _body.Matrix;
			_physMatrix = matrix * m;
			_physFront = PhysMath.GetForward(_physMatrix);
			_physUp = PhysMath.GetUp(_physMatrix);
			_physRight = PhysMath.GetRight(_physMatrix);
			_physOrigin = PhysMath.GetOrigin(_physMatrix);
			PhysMath.MatrixRotateFast(ref m2, 0f - Caster, right);
			PhysMath.MatrixRotateFast(ref m2, num2 * num3, forward);
			PhysMath.MatrixTranslate(ref m2, vector2);
			Matrix4x4 res = default;
			PhysMath.MatrixMultiply4x4(matrix, m2, ref res);
			CasterPoint = res.MultiplyPoint3x4(vector);
			_casterDir = res.MultiplyVector(-up);
		}

		internal void UpdatePhysGroundMatrix_Simple()
		{
			Matrix4x4 m = Matrix4x4.identity;
			PhysMath.MatrixSetTranslate(ref m, Vector3.right * (0.05f * (float)((!IsLeft) ? 1 : (-1))));
			PhysMath.MatrixRotateFast(ref m, 0f - SteerAngle, Vector3.up, isDegree: false);
			PhysMath.MatrixTranslate(ref m, Vector3.right * (0.05f * (float)(IsLeft ? 1 : (-1))));
			Vector3 topPos = TopPos;
			topPos = ((!IsLeft) ? (topPos + Vector3.right * (_width * 0.3f)) : (topPos - Vector3.right * (_width * 0.3f)));
			topPos -= Vector3.up * CurSpringLen;
			PhysMath.MatrixTranslate(ref m, topPos);
			Matrix4x4 res = default;
			Matrix4x4 matrix = _body.Matrix;
			PhysMath.MatrixMultiply4x4(matrix, m, ref res);
			_physFront = PhysMath.GetForward(res);
			_physUp = PhysMath.GetUp(res);
			_physRight = PhysMath.GetRight(res);
			_physOrigin = PhysMath.GetOrigin(res);
			Vector3 vector = Vector3.right * (0.05f * (IsLeft ? 1f : (-1f)));
			CasterPoint = matrix.MultiplyPoint3x4(topPos + vector);
			_casterDir = matrix.MultiplyVector(-Vector3.up);
		}

		internal void UpdateRenderMatrixOnline(Matrix4x4 carBodyMatrix, float dt)
		{
			float num = CurSpringLengthReceived + SteerAngle + _rollAngle;
			if (SkidOnline || !(Mathf.Abs(AngularVelocity) < 1f) || !(Mathf.Abs(num - _lastValue) < 0.001f) || Time.frameCount % 100 == 0)
			{
				CurSpringLen = Mathf.Lerp(CurSpringLen, CurSpringLengthReceived, dt * 20f);
				BuildRenderMatrixOnline(carBodyMatrix, _rollAngle, ref _renderMatrix);
				if (NeedsOpenWheelCollider)
				{
					BuildRenderMatrixOnline(carBodyMatrix, 0f, ref _rimColliderMatrix);
				}
				_lastValue = num;
			}
		}

		private void BuildRenderMatrixOnline(Matrix4x4 carBodyMatrix, float rollAngle, ref Matrix4x4 outMatrix)
		{
			Matrix4x4 m = Matrix4x4.identity;
			float num = CurSpringLen / MaxSpringLen;
			float num2 = CamberSusp + Camber + CamberAddDown * num + CamberAddTop * (1f - num);
			PhysMath.MatrixRotateFast(ref m, rollAngle, Vector3.right, isDegree: false);
			PhysMath.MatrixTranslate(ref m, Vector3.right * (0.05f * (float)((!IsLeft) ? 1 : (-1))));
			PhysMath.MatrixRotateFast(ref m, Caster, Vector3.right);
			PhysMath.MatrixRotateFast(ref m, 0f - SteerAngle, Vector3.up, isDegree: false);
			PhysMath.MatrixRotateFast(ref m, 0f - Caster, Vector3.right);
			PhysMath.MatrixTranslate(ref m, Vector3.right * (0.05f * (float)(IsLeft ? 1 : (-1))));
			PhysMath.MatrixRotateFast(ref m, num2 * (float)(IsLeft ? 1 : (-1)), Vector3.forward);
			Vector3 v = TopPos - PhysMath.Vector3Mul(Vector3.up, CurSpringLen);
			PhysMath.MatrixTranslate(ref m, v);
			PhysMath.MatrixMultiply4x4(carBodyMatrix, m, ref outMatrix);
		}

		internal void UpdateRenderMatrix_Advanced(Matrix4x4 carBodyMatrix)
		{
			if (!float.IsFinite(CurSpringLen))
			{
				CurSpringLen = 0f;
			}
			Vector3 topPosSafe = TopPos;
			if (!float.IsFinite(topPosSafe.x) || !float.IsFinite(topPosSafe.y) || !float.IsFinite(topPosSafe.z))
			{
				topPosSafe = Vector3.zero;
			}
			BuildRenderMatrixAdvanced(carBodyMatrix, topPosSafe, _rollAngle, ref _renderMatrix);
			if (NeedsOpenWheelCollider)
			{
				BuildRenderMatrixAdvanced(carBodyMatrix, topPosSafe, 0f, ref _rimColliderMatrix);
			}
		}

		private void BuildRenderMatrixAdvanced(Matrix4x4 carBodyMatrix, Vector3 topPosSafe, float rollAngle, ref Matrix4x4 outMatrix)
		{
			Matrix4x4 m = Matrix4x4.identity;
			float num = CurSpringLen / MaxSpringLen;
			float num2 = CamberSusp + Camber + CamberAddDown * num + CamberAddTop * (1f - num);
			Vector3 right = Vector3.right;
			Vector3 up = Vector3.up;
			Vector3 forward = Vector3.forward;
			float num3 = (IsLeft ? 1f : (-1f));
			Vector3 vector = right * (0.05f * num3);
			PhysMath.MatrixRotateFast(ref m, rollAngle, right, isDegree: false);
			PhysMath.MatrixTranslate(ref m, -vector);
			PhysMath.MatrixRotateFast(ref m, Caster, Vector3.right);
			PhysMath.MatrixRotateFast(ref m, 0f - SteerAngle, up, isDegree: false);
			PhysMath.MatrixRotateFast(ref m, 0f - Caster, right);
			PhysMath.MatrixTranslate(ref m, vector);
			PhysMath.MatrixRotateFast(ref m, num2 * num3, forward);
			Vector3 v = topPosSafe - PhysMath.Vector3Mul(up, CurSpringLen);
			PhysMath.MatrixTranslate(ref m, v);
			PhysMath.MatrixMultiply4x4(carBodyMatrix, m, ref outMatrix);
		}

		internal void UpdateRenderMatrix_Simple(Matrix4x4 carBodyMatrix, float inwardOffset = 0f)
		{
			if (!float.IsFinite(CurSpringLen))
			{
				CurSpringLen = 0f;
			}
			Vector3 topPosSafe = TopPos;
			if (!float.IsFinite(topPosSafe.x) || !float.IsFinite(topPosSafe.y) || !float.IsFinite(topPosSafe.z))
			{
				topPosSafe = Vector3.zero;
			}
			BuildRenderMatrixSimple(carBodyMatrix, topPosSafe, inwardOffset, _rollAngle, ref _renderMatrix);
			if (NeedsOpenWheelCollider)
			{
				BuildRenderMatrixSimple(carBodyMatrix, topPosSafe, inwardOffset, 0f, ref _rimColliderMatrix);
			}
		}

		private void BuildRenderMatrixSimple(Matrix4x4 carBodyMatrix, Vector3 topPosSafe, float inwardOffset, float rollAngle, ref Matrix4x4 outMatrix)
		{
			Matrix4x4 m = Matrix4x4.identity;
			Vector3 right = Vector3.right;
			Vector3 up = Vector3.up;
			float num = (IsLeft ? 1f : (-1f));
			Vector3 vector = right * (0.05f * num);
			PhysMath.MatrixRotateFast(ref m, rollAngle, right, isDegree: false);
			PhysMath.MatrixTranslate(ref m, -vector);
			PhysMath.MatrixRotateFast(ref m, 0f - SteerAngle, up, isDegree: false);
			PhysMath.MatrixTranslate(ref m, vector);
			Vector3 v = topPosSafe - up * CurSpringLen + right * (inwardOffset * num);
			PhysMath.MatrixTranslate(ref m, v);
			PhysMath.MatrixMultiply4x4(carBodyMatrix, m, ref outMatrix);
		}

		internal void UpdateRoadDistance()
		{
			_tarSaVol = 0f;
			_tarSrVol = 0f;
			LongtitudeForce = 0f;
			IsTouch = false;
			_digger = false;
			RadiusLoaded = _maxRadius;
			CurrentPhysMat.SurfaceType = Surface.None;
			if (!_rayTraceResult || !Enabled)
			{
				return;
			}
			Vector3 vector = -_topUp;
			CurrentPhysMat.SurfaceType = _rayInfo.Surface;
			_flatTarget = _rayInfo.Target;
			_flatTarNorm = _rayInfo.Normal;
			Vector2 rangeMax = new Vector2(CurrentPhysMat.BumpScale, CurrentPhysMat.BumpScale);
			float x = _flatTarget.x;
			float z = _flatTarget.z;
			float bumpMin = CurrentPhysMat.BumpMin;
			float bumpMax = CurrentPhysMat.BumpMax;
			Vector2 zero = Vector2.zero;
			float value = DisplacementMap.GetValue(x, z, bumpMin, bumpMax, zero, rangeMax);
			float value2 = DisplacementMap.GetValue(x + 0.01f, z, bumpMin, bumpMax, zero, rangeMax);
			float value3 = DisplacementMap.GetValue(x, z + 0.01f, bumpMin, bumpMax, zero, rangeMax);
			Vector3 v = new Vector3(value - value2, value - value3, 0.005f);
			PhysMath.Vector3NormalizeSafe(ref v);
			_targetNormal = _flatTarNorm + PhysMath.Vector3Mul(Vector3.right, v.x) + PhysMath.Vector3Mul(Vector3.forward, v.y);
			PhysMath.Vector3NormalizeSafe(ref _targetNormal);
			Surface surfaceType = CurrentPhysMat.SurfaceType;
			_digger = surfaceType == Surface.Sand || surfaceType == Surface.Grass || surfaceType == Surface.DirtRoad || surfaceType == Surface.Snow;
			Vector3 vector2 = _flatTarget - PhysMath.Vector3Mul(vector, value);
			float num = Vector3.Dot(vector, vector2 - _topOrigin);
			if (_digger)
			{
				_curRoadDist = num + 0.05f;
				_target = _topOrigin + PhysMath.Vector3Mul(vector, _curRoadDist);
				_displacement = CurSpringLen + RadiusLoaded - num;
				if (num > MaxSpringLen + _maxRadius || _displacement < 0f)
				{
					CurrentPhysMat.SurfaceType = Surface.None;
					return;
				}
			}
			else
			{
				_target = vector2;
				_curRoadDist = num;
				if (_curRoadDist > MaxSpringLen + _maxRadius)
				{
					CurrentPhysMat.SurfaceType = Surface.None;
					return;
				}
			}
			IsTouch = true;
		}

		internal void UpdateRoadDistanceArcade()
		{
			_steerVelocity = Vector3.zero;
			_tarSaVol = 0f;
			_tarSrVol = 0f;
			LongtitudeForce = 0f;
			IsTouch = false;
			_digger = false;
			RadiusLoaded = _maxRadius;
			CurrentPhysMat.SurfaceType = Surface.None;
			if (_rayTraceResult)
			{
				_flatTarget = _rayInfo.Target;
				_flatTarNorm = _rayInfo.Normal;
				CurrentPhysMat.SurfaceType = _rayInfo.Surface;
				_target = _flatTarget;
				_targetNormal = _flatTarNorm;
				_curRoadDist = Vector3.Dot(-_topUp, _target - _topOrigin);
				if (_curRoadDist > MaxSpringLen + _maxRadius)
				{
					CurrentPhysMat.SurfaceType = Surface.None;
					return;
				}
				_targetVelocity = _body.GetVelocity(_target);
				IsTouch = true;
			}
		}

		internal bool UpdateRoadDist_Traffic(float dt, ref Vector3 midNorm, ref Vector3 midTarget)
		{
			SlipAngleVolume = 0f;
			SlipRatioVolume = 0f;
			CurrentPhysMat.SurfaceType = _rayInfos[0].Surface;
			UpdateRollAngle(dt);
			CurSpringLen = MaxSpringLen;
			if (!_rayHits[0])
			{
				return false;
			}
			_flatTarget = _rayInfos[0].Target;
			_flatTarNorm = _rayInfos[0].Normal;
			midNorm += _flatTarNorm;
			midTarget += _flatTarget;
			Vector3 up = PhysMath.GetUp(_body.Matrix);
			Vector3 forward = PhysMath.GetForward(_body.Matrix);
			Vector3 vector = _body.Matrix.MultiplyPoint3x4(TopPos);
			float num = Vector3.Dot(up, vector - _flatTarget) - _maxRadius - MaxSpringLen;
			float num2 = (0f - num) * SpringCoef;
			if (num < 0f)
			{
				_body.AddExtForce(_flatTarget, PhysMath.Vector3Mul(up, num2 * dt));
				CurSpringLen = MaxSpringLen + num;
				Vector3 velocity = _body.GetVelocity(_flatTarget);
				AngularVelocity0 = Vector3.Dot(velocity, forward) / _maxRadius;
				AngularVelocity = AngularVelocity0;
				ForwardVelocity = AngularVelocity * _maxRadius;
				SideVelocity = 0f;
				LongitudinalSlip = 0f;
				LateralSlip = 0f;
			}
			return true;
		}

		internal void UpdateRollAngle(float dt)
		{
			if (!PhysRuntimeFlags.IsCarInService)
			{
				_rollAngle += AngularVelocity * dt;
				if (IsTouch)
				{
					_rollTyreAngle += AngularVelocity0 * dt;
				}
				else
				{
					_rollTyreAngle = _rollAngle;
					_sideTyreOffset = _sideOffset;
				}
				PhysMath.NormalizeAngles(ref _rollAngle, ref _rollTyreAngle);
				PhysMath.NormalizeAngles(ref _sideOffset, ref _sideTyreOffset);
			}
		}

		private void UpdateTopMatrix()
		{
			Matrix4x4 m = Matrix4x4.identity;
			float num = (IsLeft ? 1f : (-1f));
			Vector3 right = Vector3.right;
			Vector3 up = Vector3.up;
			float num2 = 0.05f * num;
			float num3 = _width * 0.3f * num;
			PhysMath.MatrixRotateFast(ref m, 0f - SteerAngle, up, isDegree: false);
			Vector3 vector = TopPos + right * (num2 - num3);
			m.m03 = vector.x - right.x * num2;
			m.m13 = vector.y - right.y * num2;
			m.m23 = vector.z - right.z * num2;
			_topMatrix = _body.Matrix * m;
			_topUp = PhysMath.GetUp(_topMatrix);
			_topRight = PhysMath.GetRight(_topMatrix);
			_topOrigin = PhysMath.GetOrigin(_topMatrix);
		}
	}
}

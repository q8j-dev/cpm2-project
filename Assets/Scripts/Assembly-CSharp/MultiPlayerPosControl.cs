using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using Addler.Runtime.Core.LifetimeBinding;
using CPM.Database;
using CPM.PlayerData;
using CPM.Scripts.VinylDrawer;
using CPMEngine;
using CodeStage.AntiCheat.ObscuredTypes;
using CodeStage.AntiCheat.Storage;
using Cysharp.Threading.Tasks;
using DataEntities.Vinyl;
using ExitGames.Client.Photon;
using MVPFree.MapIcons.Models;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;
using VContainer;
using _Car_Parking;
using _Car_Parking.Scripts.Car;
using _Car_Parking.Scripts.Car.Physics;
using _Car_Parking.Scripts.Car.Sounds;
using _Car_Parking.Scripts.CommonUtility.Contracts;
using _Car_Parking.Scripts.CustomDataTypes;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.GameFacts.Base;
using _Car_Parking.Scripts.GraphicsSettings;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Multiplayer;
using _Car_Parking.Scripts.Multiplayer.CarLegitService;
using _Car_Parking.Scripts.Multiplayer.DataPack;
using _Car_Parking.Scripts.Multiplayer.InterestGroupsImpl;
using _Car_Parking.Scripts.Multiplayer.RpcMethods;
using _Car_Parking.Scripts.Other.FsoStrobeLights;
using _Car_Parking.Scripts.Other.Helpers;
using _Car_Parking.Scripts.Person;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.Scopes;
using _Car_Parking.Scripts.ToRoad;
using _Car_Parking.Scripts.Trade;

public class MultiPlayerPosControl : MonoBehaviourPunCallbacks, IPunInstantiateMagicCallback, IPunObservable, IVinylDrawPerformer
{
	private const int RequestDataTimer = 60;

	private const float MaxVel = 64f;

	private const float MaxRPM = 180f;

	private const float SpeedLimitForDisableCollision = 30f;

	private static readonly PackerBase<Vector3> PrecisePositionPacker = new RangedVectorPacker((Precision: 11, x: -99.99f, y: 99.99f), (Precision: 14, x: -200f, y: 180f), (Precision: 11, x: -99.99f, y: 99.99f));

	private static readonly PackerBase<Quaternion> PreciseRotationPacker = new QuaternionPacker(7, 7, 7, 7);

	private static readonly PackerPipeline<Vector3, Quaternion> PreciseDataPipeline = new PackerPipeline<Vector3, Quaternion>(PrecisePositionPacker, PreciseRotationPacker);

	private static readonly BoolPacker FlagPacker = new BoolPacker();

	private static readonly PackerBase<float> VelocityPacker = new RangedFloatPacker(6, 0f, 64f);

	private static readonly PackerBase<float> RPMPacker = new Float01Packer(8);

	private static readonly PackerBase<float> SteerPacker = new RangedFloatPacker(6, -1f);

	private CustomPacker<byte> _wheelPacker;

	private static readonly PackerPipeline<float, bool> DistancePipeline = new PackerPipeline<float, bool>(new RangedFloatPacker(15, 0f, 2000f), FlagPacker);

	private static readonly RangedFloatPacker MileagePacker = new RangedFloatPacker(19, 0f, 1000000f);

	private static readonly RangedFloatPacker FuelPacker = new RangedFloatPacker(6, 0f, 100f);

	private static readonly RangedFloatPacker TireHealthPacker = new RangedFloatPacker(6);

	public static readonly RangedVectorPacker GlobalOffsetPacker = new RangedVectorPacker((Precision: 30, x: -20000f, y: 20000f), (Precision: 0, x: 0f, y: 0f), (Precision: 30, x: -20000f, y: 20000f));

	private static readonly PackerPipeline<float, float, float> MileageFuelTireHealthPipeline = new PackerPipeline<float, float, float>(MileagePacker, FuelPacker, TireHealthPacker);

	private static readonly PackerBase<int> WheelIndexPacker = new IntPacker(2, signed: false);

	private static readonly PackerBase<float> WheelLenPacker = new Float01Packer(6);

	private static readonly PackerBase<int> WheelSurfacePacker = new IntPacker(5, signed: false);

	private static readonly PackerBase<float> GasPacker = new Float01Packer(3);

	private static readonly PackerPipeline<int, float, int, bool> WheelDataPipeline = new PackerPipeline<int, float, int, bool>(WheelIndexPacker, WheelLenPacker, WheelSurfacePacker, FlagPacker);

	private static readonly RangedFloatPacker HeightPacker = new RangedFloatPacker(15, -200f, 180f);

	private static readonly PackerBase<int> DirtPacker = new IntPacker(24, signed: true);

	private static readonly PackerBase<float> IntensityPacker = new Float01Packer(8);

	private static readonly Float01Packer Dumb = new Float01Packer(0);

	private PackerPipeline<float, float, float, float, byte, bool> _carDataPackPipeline;

	private static readonly PackerPipeline<float, float, int, float, float, float> RequestedDataPipeline = new PackerPipeline<float, float, int, float, float, float>(FuelPacker, TireHealthPacker, DirtPacker, IntensityPacker, HeightPacker, Dumb);

	private static readonly PackerPipeline<float> HeightPipeline = new PackerPipeline<float>(HeightPacker);

	private readonly AdjustableDictionary _requestedGroups = new AdjustableDictionary();

	[SerializeField]
	private float TooFarToPredict = 50f;

	[Inject]
	private InterestGroupManager _interestGroupManager;

	[Inject]
	private CarInputManager _carInputManager;

	private readonly long[] _prevWheels = new long[4];

	private long _prevPrecise;

	private long _prevDistance;

	private long _prevCarData;

	private byte _prevSurfaceGas;

	private byte _currentWheelIndex;

	private const int WheelDataRefreshPeriod = 40;

	private int _wheelDataRefreshCounter;

	private float _lastSentRPM;

	private bool _lastReceivedIsBraking;

	private bool _isSteeringChanged;

	private bool _isIgnoreCarData;

	private bool _forceUpdate;

	private bool _skipPosLerping;

	private float _lag;

	private float _lastRawPositionTime = -1f;

	private float _lastRawSpeed;

	private IDisposable _resetSkipLerp;

	private IDisposable _sendPreciseDataDisposable;

	private bool _forceIgnoreCollisions;

	private const float EngineOffRpm = 150f;

	private const float ThrottleRecoverPerSec = 25f;

	private const float TauUp = 0.02f;

	private const float TauDown = 0.08f;

	private const float MaxRpmRise = 20000f;

	private const float MaxRpmFall = 30000f;

	private const float SpeedLimit = 90f;

	private const float SqueezeHorizontalOverlap = 0.15f;

	private const float SqueezeVerticalOverlap = 0.05f;

	private const float SqueezeOnTopHeight = 0.35f;

	private const float SqueezeGuardReleaseMargin = 0.3f;

	private const float SqueezeGuardMaxDistance = 30f;

	public bool isOnlineCar;

	public bool vinylLoaded;

	public bool FarFromPerson;

	public bool carEnabled = true;

	public bool forceDisableCar;

	public Vector3 correctPlayerPos;

	public Quaternion correctPlayerRot;

	public float Steering;

	public float RPM;

	public float LerpedRPM;

	public bool RevTrigger;

	private float _oldRpm;

	private float _prevRpm;

	public float rotFR;

	public float rotFL;

	public float rotRR;

	public float rotRL;

	public bool isSkidCarFR;

	public bool isSkidCarFL;

	public bool isSkidCarRR;

	public bool isSkidCarRL;

	public float LeftFuel = 50f;

	public SoundController SoundController;

	public VehicleData VehicleData;

	public LightController LightController;

	public MeshRenderer label;

	public DrawQueue DrawQueue;

	public HashSet<string> CachedSpectators = new HashSet<string>();

	private Powertrain _powertrain;

	private Axles _axles;

	private Rigidbody _rigidBody;

	private PlayerDataStorage _playerDataStorage;

	private PlayerDatas.VinylsDataUtility _vinylsDataUtility;

	private VinylDrawer _vinylDrawer;

	private CarFactoryService _carFactoryService;

	private ICarLegitService _carLegitService;

	private IGameFactManager _gameFactManager;

	private UniTaskCompletionSource _applyDataCompletion;

	private AllCarsInfo _allCarsInfo;

	private FreeDriveControl _freeDriveControl;

	private DragRacingControlMultiplayer _dragControlMulti;

	private OptionalDependency<FreeDriveControl> _freeDriveControlDep;

	private OptionalDependency<DragRacingControlMultiplayer> _dragControlMultiDep;

	private CarComponentsMultiplayer _carComponents;

	private RamHelper _ramHelper;

	public float distanceMultiRace;

	public float distance;

	private int _skipFrames;

	private Camera _cam;

	public Color CarColor;

	public Color CarColorReflective;

	public Color CarColorSpecular;

	public Color WheelColorSpecular;

	public Color WheelColor;

	public Color WheelColorReflective;

	public Color Wheel2ColorSpecular;

	public Color Wheel2Color;

	public Color Wheel2ColorReflective;

	public float Shininess;

	public float WShininess;

	public float W2Shininess;

	public float Smoothness;

	public float WSmoothness;

	public float W2Smoothness;

	private Color _smokeColor = Color.white;

	public Color WindowColor = Color.magenta;

	public Color WindowColorReflective = Color.magenta;

	public Color tireTextColor;

	public string tireText;

	public int rimRadius;

	public int tireFont;

	public int tireFontStyle;

	public Color diodColor;

	public Vector2 cog;

	private bool sentFuel;

	private bool sendMileage;

	public float tempAngel;

	public GameObject SpeakingIcon;

	public GameObject BigRankIcon;

	public GameObject SmallRankIcon;

	public GameObject speedLimitIcon;

	public float brakePressTime;

	public bool speeding;

	public Transform loadCarLocalTarget;

	public bool DataApplied;

	public bool closeToPerson = true;

	public byte velo;

	public float lastUpdatedPosTime;

	public float koff3 = 6f;

	public bool steeringChanging;

	private float _calculateOnlineSteering;

	private float _oldSteering;

	private float _lastSendRpm;

	private bool _sendOnlyPosition;

	private int count;

	public byte lastSkidmarkStates;

	public int countLastSkidmarkStateSend;

	private int countToSkip;

	private Vector3 _lastPositionReceived;

	private int _lastWheelRotDirection;

	public long lastLong;

	public int Last_Y_Xq_Zq;

	public short lastFrontWheelY;

	public short lastRearWheelY;

	private int idIntensity;

	public bool isVisible;

	public bool ignoreCollision;

	private bool _squeezeGuardActive;

	private float lastSendY;

	public bool hoodState;

	private string newCarOwnerName;

	public Vector3 globalPointThisPlayer;

	public byte checkByte;

	private int countGlobalPointSend;

	public bool LoadingWheelNow;

	public short frontWShort;

	public short rearWShort;

	private MeshRenderer _colliderRenderer;

	private readonly int _decalTexPropID = Shader.PropertyToID("_DecalTex");

	private bool _vinylsBanned;

	public float createdTime;

	private ObscuredInt? _rankCurrentPlayer;

	public MultiplayerDataSending.CarVinyls CarVinyls;

	public bool WheelsLoaded;

	private bool _referencesWasSet;

	private bool _oneSubscribeBrake;

	private BadCarConfig _badCarConfig;

	private const int LevelsCount = 83;

	private const string LABEL_KEY = "Player_label";

	private RpcCollection _rpcCollection;

	private CurrencyManagerController _currencyManagerController;

	private LevelsProvider _levelsProvider;

	private IDisposable _interestGroupDisposable;

	private IObservable<long> _intervalObs;

	private MapIconsModel _mapIconsModel;

	private OptionalDependency<MapIconsModel> _mapIconModelDep;

	private readonly List<MonoBehaviour> _disabledAtSpawn = new List<MonoBehaviour>();

	private int _enableAtSpawnProgress;

	private const float EnableFrameBudgetSeconds = 0.002f;

	private bool _steamVisualBuildStarted;

	private GameObject ghost;

	private float _visualSteer;

	private float _limiterPhase;

	private float _onlineAccel;

	private bool cutPhase;

	private const float smoothSpeed = 5f;

	public const string RPC_SEND_VALUE_STR = "SendValueStr";

	private static readonly TimeSpan ForceSendCarPositionDelay = TimeSpan.FromSeconds(1.0);

	public Vector3Int LastSentRawPosition { get; set; }

	public Vector3Int PredictedSentRawPosition { get; set; }

	public Vector3Int LastReceivedRawPosition { get; set; }

	public Vector3Int PredictedRawPosition { get; set; }

	public bool ForceIgnoreCollisions
	{
		get
		{
			return _forceIgnoreCollisions;
		}
		set
		{
			_forceIgnoreCollisions = value;
			if (_forceIgnoreCollisions)
			{
				ForceUpdateIgnoreCollisions();
			}
		}
	}

	public byte InterestGroup
	{
		get
		{
			return base.photonView.Group;
		}
		set
		{
			base.photonView.Group = value;
		}
	}

	public float ReceivedSteer { get; set; }

	public CarComponentsMultiplayer CarComponents
	{
		get
		{
			if (_carComponents == null)
			{
				_carComponents = new CarComponentsMultiplayer(base.gameObject);
			}
			return _carComponents;
		}
	}

	public FreeDriveProvider FreeDriveProvider { get; private set; }

	public OnlineCollectionsProvider OnlineCollectionsProvider { get; private set; }

	public bool IsBreaking { get; set; }

	public bool IsVinylsBanned
	{
		get
		{
			return _vinylsBanned;
		}
		set
		{
			_vinylsBanned = value;
			bool isActive = PlayerPrefs.GetInt("hideAllVinyls", 0) == 0;
			UpdateVinylsState(isActive);
		}
	}

	private MapIconsModel MapIconsModel
	{
		get
		{
			if (_mapIconsModel != null)
			{
				return _mapIconsModel;
			}
			if ((bool)V.CoreGameManager)
			{
				return V.CoreGameManager.MapIconsModel;
			}
			return null;
		}
	}

	private bool CanApplyData
	{
		get
		{
			if ((bool)VehicleData && (bool)base.photonView && !FlagsStorage.IsMainMenu && !VehicleData.IsRacingCar)
			{
				return !ScenesHelper.IsParkingLevelActive();
			}
			return false;
		}
	}

	private bool ShouldSkipCarLegitCheck
	{
		get
		{
			if (!(base.gameObject == FreeDriveProvider.FirstCarMultiPlayer) && FlagsStorage.IsMultiFree)
			{
				return FlagsStorage.InGarageFree;
			}
			return true;
		}
	}

	public bool VisualBuildStarted => _steamVisualBuildStarted;

	public bool VisualBuilt
	{
		get
		{
			if ((bool)VehicleData)
			{
				return VehicleData.IsCarGraphicLoaded;
			}
			return false;
		}
	}

	public MonoBehaviour ComponentToCheck => this;

	public void PredictSentPosition(Vector3Int position)
	{
		Vector3Int lastSentRawPosition = LastSentRawPosition;
		LastSentRawPosition = position;
		Vector3Int vector3Int = position - lastSentRawPosition;
		if ((float)vector3Int.sqrMagnitude > TooFarToPredict * TooFarToPredict)
		{
			vector3Int = Vector3Int.zero;
		}
		PredictedSentRawPosition = position + vector3Int;
	}

	public void StreamSend(PhotonStream stream, PhotonMessageInfo info)
	{
		if (_prevPrecise == long.MaxValue || _interestGroupManager.IsOtherInInterestGroups() || CachedSpectators.Count >= 1)
		{
			Steering = 0f - _powertrain.GetSteer();
			UpdateSerializationRate();
			MutateDataOnStreamSend();
			Vector3 t = base.transform.localPosition - LastSentRawPosition.RoundedToHundreds();
			t.y = HeightOffset.ApplyOnSend(base.transform.localPosition, t.y);
			if (!PreciseDataPipeline.TrySendIfChanged(ref _prevPrecise, stream, t, base.transform.rotation, Convert.ToInt64))
			{
				long prev = 32767L;
				HeightPipeline.TrySendIfChanged(ref prev, stream, HeightOffset.ApplyOnSend(base.transform.localPosition, base.transform.localPosition.y), Convert.ToInt16);
			}
			IWheel wheel = _axles.AllWheels[_currentWheelIndex].Wheel;
			WheelDataPipeline.TrySendIfChanged(ref _prevWheels[_currentWheelIndex], stream, _currentWheelIndex, wheel.CurSpringLen, (int)wheel.CurrentPhysMat.SurfaceType, t4: true, Convert.ToInt16);
			if (!_freeDriveControl)
			{
				_freeDriveControl = V.CoreGameManager.FreeDriveProvider.FreeDriveControl;
			}
			DistancePipeline.TrySendIfChanged(ref _prevDistance, stream, _freeDriveControl.DistanceCarsRacingMulti, t2: false, Convert.ToInt16, !FlagsStorage.ChasingRaceModeStarted || _freeDriveControl.IsChasing);
			_carDataPackPipeline.TrySendIfChanged(ref _prevCarData, stream, _powertrain.CurrentRpm / _powertrain.MaxRpm, _rigidBody.velocity.magnitude / 10f, Steering, _powertrain.ThrottleRev, 0, IsBreaking, Convert.ToInt32, _isIgnoreCarData);
		}
	}

	public void StreamReceive(PhotonStream stream, PhotonMessageInfo info)
	{
		_lag = CalculateLag(info);
		LastReceivedRawPosition = Vector3Int.RoundToInt(Vector3.Lerp(LastReceivedRawPosition, PredictedRawPosition, V.CarPosLerpRate));
		if (!PreciseDataPipeline.TryReceive<long>(stream, Convert.ToInt64, ReceivePrecisePosition))
		{
			HeightPipeline.TryReceive<short>(stream, Convert.ToInt64, UnpackHeight);
		}
		if (stream.currentItem != stream.Count)
		{
			if (stream.PeekNext() is short data && CheckShortData(data))
			{
				WheelDataPipeline.TryReceive<short>(stream, Convert.ToInt64, UnpackWheelLen);
			}
			DistancePipeline.TryReceive<short>(stream, Convert.ToInt64, ReceiveDistance);
			_carDataPackPipeline.TryReceive<int>(stream, Convert.ToInt64, UnpackCarData);
		}
	}

	private void UnpackHeight(float obj)
	{
		correctPlayerPos.y = HeightOffset.ApplyOnReceive(correctPlayerPos, obj);
		SkipPosLerp(0.25f);
	}

	private bool CheckShortData(short data)
	{
		return (data & 1) == 1;
	}

	private static float CalculateLag(PhotonMessageInfo info)
	{
		return Mathf.Max((float)(PhotonNetwork.Time - info.SentServerTime), 0f);
	}

	public void SkipPosLerp(float duration = 1f)
	{
		_skipPosLerping = true;
		_resetSkipLerp?.Dispose();
		_resetSkipLerp = Observable.Timer(TimeSpan.FromSeconds(duration)).Subscribe(SetSkipPosLerpingFalse).AddTo(this);
	}

	private void SetSkipPosLerpingFalse(long _ = 0L)
	{
		_skipPosLerping = false;
	}

	private void ReceiveDistance(float obj, bool _)
	{
		distance = obj;
		if (FlagsStorage.ChasingRaceModeStarted && (bool)_freeDriveControl && _freeDriveControl.IsChasing)
		{
			distanceMultiRace = distance;
		}
	}

	private void ReceivePrecisePosition(Vector3 position, Quaternion rotation)
	{
		Vector3 localPos = LastReceivedRawPosition.RoundedToHundreds() + position;
		position.y = HeightOffset.ApplyOnReceive(localPos, position.y);
		Vector3 vector = LastReceivedRawPosition.RoundedToHundreds() + position;
		Vector3 vector2 = vector - correctPlayerPos;
		vector2.y = 0f;
		correctPlayerPos = vector + vector2 * (2f * _lag) / PhotonNetwork.SerializationRate;
		correctPlayerRot = rotation;
	}

	private void UnpackMileageFuelTireHealth(int mileageFuelTireHealth)
	{
		var (mileage, currentFuel, health) = MileageFuelTireHealthPipeline.Unpack(mileageFuelTireHealth);
		_powertrain.SetMileage(mileage);
		foreach (WheelProvider allWheel in _axles.AllWheels)
		{
			allWheel.SetTyreHealth(health, allWheel.TyreType);
		}
		if ((bool)VehicleData.CarChildsReferences.FuelTank)
		{
			VehicleData.CarChildsReferences.FuelTank.CurrentFuel = currentFuel;
		}
	}

	private void MutateDataOnStreamSend()
	{
		velo = (byte)Mathf.Round(_powertrain.SpeedMps);
		_isSteeringChanged = Mathf.Abs(_oldSteering - Steering) > 0.02f;
		_oldSteering = Steering;
		int num = Mathf.Max(Mathf.Min(_axles.AllWheels.Count, 4), 1);
		_currentWheelIndex = (byte)((_currentWheelIndex + 1) % num);
		if (++_wheelDataRefreshCounter >= 40)
		{
			_wheelDataRefreshCounter = 0;
			for (int i = 0; i < _prevWheels.Length; i++)
			{
				_prevWheels[i] = long.MaxValue;
			}
			_prevCarData = long.MaxValue;
			_forceUpdate = true;
		}
		_lastSentRPM = _powertrain.CurrentRpm;
		LastSentRawPosition = Vector3Int.RoundToInt(Vector3.Lerp(LastSentRawPosition, PredictedSentRawPosition, V.CarPosLerpRate));
		bool flag = _forceIgnoreCollisions || (float)(int)velo * 3.62f > 30f;
		if (flag != ignoreCollision)
		{
			PlayerDatas.IgnoreCollisionWithOtherCars(base.gameObject, flag, OnlineCollectionsProvider, out var checkAgain);
			if (!checkAgain)
			{
				ignoreCollision = flag;
			}
		}
	}

	private void ForceUpdateIgnoreCollisions()
	{
		PlayerDatas.IgnoreCollisionWithOtherCars(base.gameObject, ignore: true, OnlineCollectionsProvider, out var checkAgain);
		if (!checkAgain)
		{
			ignoreCollision = true;
		}
	}

	private static bool EngineRunningByRpm(float rpm)
	{
		return rpm >= 150f;
	}

	private void UpdateSerializationRate()
	{
		_isIgnoreCarData = false;
		if (V.Car == base.gameObject && !FlagsStorage.IsPassenger)
		{
			byte b = velo;
			if (b < 15)
			{
				if (b >= 2)
				{
					goto IL_00e9;
				}
				if (b == 0 && _lastSentRPM < _powertrain.EngineIdleRpm && _powertrain.CurrentRpm < _powertrain.EngineIdleRpm && EngineRunningByRpm(_lastSentRPM) == EngineRunningByRpm(_powertrain.CurrentRpm))
				{
					if (_isSteeringChanged || _carInputManager.ThrottleInput > 0f || _forceUpdate)
					{
						PhotonNetwork.SerializationRate = 5;
					}
					else
					{
						_isIgnoreCarData = true;
						PhotonNetwork.SerializationRate = 3;
					}
				}
				else
				{
					if (!(_powertrain.CurrentRpm > 2000f))
					{
						goto IL_00e9;
					}
					PhotonNetwork.SerializationRate = 6;
				}
			}
			else if (b < 20)
			{
				PhotonNetwork.SerializationRate = 6;
			}
			else
			{
				PhotonNetwork.SerializationRate = 8;
			}
		}
		goto IL_0114;
		IL_00e9:
		PhotonNetwork.SerializationRate = ((_powertrain.CurrentRpm > 6000f) ? 6 : 5);
		goto IL_0114;
		IL_0114:
		bool flag = Mathf.Abs(_axles.FrontAxle.LeftWheel.Wheel.AngularVelocity) > 0.1f || Mathf.Abs(_axles.RearAxle.LeftWheel.Wheel.AngularVelocity) > 0.1f;
		if (((FlagsStorage.RacingGameMode && !_freeDriveControl.IsChasing) || FlagsStorage.RaceStarted) | flag)
		{
			_isIgnoreCarData = false;
		}
		_forceUpdate = false;
	}

	private void UnpackWheelLen(int index, float len, int surfaceType, bool _)
	{
		_axles.AllWheels[index].Wheel.CurSpringLengthReceived = len;
		_axles.AllWheels[index].Wheel.CurrentPhysMat.SurfaceType = (Surface)surfaceType;
		switch (index)
		{
		case 2:
			if (_axles.Rear1Axle.Wheels.Count == 2)
			{
				_axles.AllWheels[4].Wheel.CurSpringLengthReceived = len;
				_axles.AllWheels[4].Wheel.CurrentPhysMat.SurfaceType = (Surface)surfaceType;
			}
			break;
		case 3:
			if (_axles.Rear1Axle.Wheels.Count == 2)
			{
				_axles.AllWheels[5].Wheel.CurSpringLengthReceived = len;
				_axles.AllWheels[5].Wheel.CurrentPhysMat.SurfaceType = (Surface)surfaceType;
			}
			break;
		}
	}

	private void InitPackers()
	{
		if (_wheelPacker == null)
		{
			_wheelPacker = new CustomPacker<byte>(PackWheelRotation, UnpackWheelRotation);
		}
		if (_carDataPackPipeline == null)
		{
			_carDataPackPipeline = new PackerPipeline<float, float, float, float, byte, bool>(RPMPacker, VelocityPacker, SteerPacker, GasPacker, _wheelPacker, FlagPacker);
		}
	}

	private long PackWheelRotation(long l, byte _)
	{
		EncodeWheelRotation(ref l);
		return l;
	}

	private (byte, long) UnpackWheelRotation(long l)
	{
		DecodeWheelRotation(ref l);
		return (0, l);
	}

	private void EncodeWheelRotation(ref long current)
	{
		bool isSkidmarkOnline = _axles.FrontAxle.LeftWheel.IsSkidmarkOnline;
		bool isSkidmarkOnline2 = _axles.RearAxle.LeftWheel.IsSkidmarkOnline;
		if (!isSkidmarkOnline && !isSkidmarkOnline2)
		{
			current <<= 6;
			return;
		}
		current <<= 3;
		current += (isSkidmarkOnline ? Mathf.Clamp(Mathf.RoundToInt(Mathf.Sqrt(_axles.FrontAxle.LeftWheel.Wheel.AngularVelocity)), 0, 4) : 7);
		current <<= 3;
		current += (isSkidmarkOnline2 ? Mathf.Clamp(Mathf.RoundToInt(Mathf.Sqrt(_axles.RearAxle.LeftWheel.Wheel.AngularVelocity)), 0, 4) : 7);
	}

	private void UnpackCarData(float receiveRpm, float vel, float steer, float throttle, byte _, bool brake)
	{
		vel *= 10f;
		ReceivedSteer = steer;
		if (ReceivedSteer.AlmostEquals(0.99f, 0.001f))
		{
			ReceivedSteer = 1f;
		}
		if (ReceivedSteer.AlmostEquals(-0.99f, 0.001f))
		{
			ReceivedSteer = -1f;
		}
		if (vel.AlmostEquals(999f, 0.001f))
		{
			vel = 0f;
		}
		if (_lastReceivedIsBraking != brake)
		{
			_lastReceivedIsBraking = brake;
			CarComponents.Axles.IsBrakePressed.Value = _lastReceivedIsBraking;
		}
		receiveRpm *= _powertrain.MaxRpm;
		_powertrain.Throttle = throttle;
		_oldRpm = RPM;
		RPM = receiveRpm;
		_powertrain.SpeedMpsOnline = vel;
		float num = Utils.Mps2Kmh(vel);
		if (!base.photonView.IsMine)
		{
			_rigidBody.isKinematic = true;
		}
		if (VehicleData.Towing)
		{
			num = 0f;
		}
		speeding = num > 90f;
		bool flag = num > 30f;
		if (flag != ignoreCollision)
		{
			PlayerDatas.IgnoreCollisionWithOtherCars(base.gameObject, flag, OnlineCollectionsProvider, out var checkAgain);
			if (!checkAgain)
			{
				ignoreCollision = flag;
			}
		}
	}

	private void DecodeWheelRotation(ref long packed)
	{
		long num = packed % 8;
		packed >>= 3;
		long num2 = packed % 8;
		packed >>= 3;
		if (num == 0L && num2 == 0L)
		{
			SetRotateByTransform(front: true, rear: true);
			return;
		}
		rotFL = num2 * num2;
		rotFR = num2 * num2;
		rotRL = num * num;
		rotRR = num * num;
		SetRotateByTransform(num2 == 7, num == 7);
	}

	public void PredictNextRawPosition(Vector3Int position, float yRotation)
	{
		Vector3Int lastReceivedRawPosition = LastReceivedRawPosition;
		LastReceivedRawPosition = position;
		Vector3Int vector3Int = position - lastReceivedRawPosition;
		if ((float)vector3Int.sqrMagnitude > TooFarToPredict * TooFarToPredict)
		{
			vector3Int = Vector3Int.zero;
		}
		float time = Time.time;
		if (_lastRawPositionTime < 0f)
		{
			_lastRawPositionTime = time;
		}
		float num = time - _lastRawPositionTime;
		_lastRawSpeed = ((num > 0.001f) ? (((Vector3)vector3Int).magnitude / num) : 0f);
		_lastRawPositionTime = time;
		PredictedRawPosition = position + vector3Int;
		if (!_interestGroupManager.IsSubscribedToInterestGroup(InterestGroup))
		{
			correctPlayerPos = new Vector3(PredictedRawPosition.x, correctPlayerPos.y, PredictedRawPosition.z);
			correctPlayerRot = Quaternion.Euler(0f, yRotation, 0f);
		}
	}

	public float PredictRawVelocity()
	{
		return _lastRawSpeed;
	}

	private void PassGlobalPointToPassengers()
	{
		if (!PhotonNetwork.IsConnected || !base.photonView.IsMine || !VehicleData.Driver || !VehicleData.VehicleMembersController.HasPassenger)
		{
			return;
		}
		List<int> list = new List<int>();
		for (int i = 1; i < VehicleData.VehicleMembersController.Members.Count; i++)
		{
			GameObject gameObject = VehicleData.VehicleMembersController.Members[i];
			if ((bool)gameObject)
			{
				MultiPersonControl component = gameObject.GetComponent<MultiPersonControl>();
				if ((bool)component)
				{
					list.Add(component.photonView.ControllerActorNr);
				}
			}
		}
		if (list.Count > 0)
		{
			Vector3 position = V.GlobalPoint.position;
			position.x -= 6000f;
			long current = 0L;
			GlobalOffsetPacker.Pack(ref current, position);
			RaiseEventOptions raiseEventOptions = new RaiseEventOptions
			{
				Receivers = ReceiverGroup.Others,
				TargetActors = list.ToArray()
			};
			PhotonNetwork.RaiseEvent(53, current, raiseEventOptions, SendOptions.SendUnreliable);
			ForceSendNextStream();
		}
	}

	public void RequestDataFromInterestGroup()
	{
		if (!_requestedGroups.Contains(InterestGroup))
		{
			_requestedGroups.Add(InterestGroup, 60f);
			base.photonView.RpcToDefaultGroup("RequestDataOnSubscribe", RpcTarget.Others);
		}
	}

	[PunRPC]
	public void OnReceivedMileageFuelTireHealth(int packed)
	{
		UnpackMileageFuelTireHealth(packed);
	}

	private int PackMileageFuelTireHealth()
	{
		return (int)MileageFuelTireHealthPipeline.Pack(_powertrain.GetMileage, VehicleData.CarChildsReferences.FuelTank.CurrentFuel, GetTireHealthToPack());
	}

	private float GetTireHealthToPack()
	{
		float num = 1f;
		List<WheelProvider> allWheels = _axles.AllWheels;
		for (int i = 0; i < allWheels.Count; i++)
		{
			num = Mathf.Min(num, allWheels[i].Wheel.TyreHealth);
		}
		return num;
	}

	[PunRPC]
	public void RequestDataOnSubscribe(PhotonMessageInfo info)
	{
		Player player = info.Sender;
		if (player != null && base.photonView.IsMine)
		{
			long num = PackRequestedData();
			base.photonView.RPC("ApplyDataOnSubscribe", player, num);
			SendPreciseDataToSubscribedPlayer(0L);
			ForceSendNextStream();
			_sendPreciseDataDisposable = Observable.Timer(TimeSpan.FromSeconds(1.0)).Subscribe(SendPreciseDataToSubscribedPlayer).AddTo(this);
		}
		void SendPreciseDataToSubscribedPlayer(long _ = 0L)
		{
			Vector3 t = base.transform.localPosition - LastSentRawPosition.RoundedToHundreds();
			t.y = HeightOffset.ApplyOnSend(base.transform.localPosition, t.y);
			long num2 = PreciseDataPipeline.Pack(t, base.transform.rotation);
			base.photonView.RPC("SendMissingPreciseData", player, num2);
			ForceSendNextStream();
		}
	}

	private void ForceSendNextStream(long _)
	{
		_prevWheels[0] = long.MaxValue;
		_prevWheels[1] = long.MaxValue;
		_prevWheels[2] = long.MaxValue;
		_prevWheels[3] = long.MaxValue;
		_prevPrecise = long.MaxValue;
		_prevDistance = 32767L;
		_prevCarData = long.MaxValue;
	}

	public void ForceSendNextStream(bool resetIgnore = false)
	{
		_forceUpdate = resetIgnore;
		ForceSendNextStream(0L);
	}

	[PunRPC]
	public void SendMissingPreciseData(long packedPreciseData)
	{
		(Vector3, Quaternion) tuple = PreciseDataPipeline.Unpack(packedPreciseData);
		Vector3 item = tuple.Item1;
		Quaternion item2 = tuple.Item2;
		_lag = 0f;
		ReceivePrecisePosition(item, item2);
	}

	[PunRPC]
	public void ApplyDataOnSubscribe(long packedData)
	{
		UnpackRequestedData(packedData);
	}

	private long PackRequestedData()
	{
		int t = CombineDirt(VehicleData.DirtComposition.DirtColor);
		return RequestedDataPipeline.Pack(VehicleData.CarChildsReferences.FuelTank.CurrentFuel, GetTireHealthToPack(), t, VehicleData.DirtComposition.DirtIntensity, HeightOffset.ApplyOnSend(base.transform.localPosition, base.transform.localPosition.y), 0f);
	}

	private void UnpackRequestedData(long packed)
	{
		(float, float, int, float, float, float) tuple = RequestedDataPipeline.Unpack(packed);
		float item = tuple.Item1;
		float item2 = tuple.Item2;
		int item3 = tuple.Item3;
		float item4 = tuple.Item4;
		float item5 = tuple.Item5;
		Color value = SplitDirt(item3);
		VehicleData.DirtComposition.UpdateDirtMultiplayer(item4, base.photonView.ViewID, value).Forget();
		foreach (WheelProvider allWheel in _axles.AllWheels)
		{
			allWheel.SetTyreHealth(item2, allWheel.TyreType);
		}
		if ((bool)VehicleData.CarChildsReferences.FuelTank)
		{
			VehicleData.CarChildsReferences.FuelTank.CurrentFuel = item;
		}
		SkipPosLerp(0.1f);
		correctPlayerPos.y = HeightOffset.ApplyOnReceive(correctPlayerPos, item5);
	}

	private int CombineDirt(Color color)
	{
		byte num = (byte)(color.r * 100f);
		byte b = (byte)(color.g * 100f);
		byte b2 = (byte)(color.b * 100f);
		return (num << 16) | (b << 8) | b2;
	}

	private Color SplitDirt(int combined)
	{
		byte num = (byte)((combined >> 16) & 0xFF);
		byte b = (byte)((combined >> 8) & 0xFF);
		byte b2 = (byte)(combined & 0xFF);
		return new Color((float)(int)num * 0.01f, (float)(int)b * 0.01f, (float)(int)b2 * 0.01f);
	}

	[Inject]
	public void Construct(PlayerDataStorage playerDataStorage, PlayerDatas.VinylsDataUtility vinylsDataUtility, VinylDrawer vinylDrawer, DrawQueue drawQueue, OnlineCollectionsProvider onlineCollectionsProvider, CarFactoryService carFactoryService, CurrencyManagerController currencyManagerController, RpcCollection rpcCollection, AllCarsInfo allCarsInfo, FreeDriveProvider freeDriveProvider, LevelsProvider levelsProvider, RamHelper ramHelper, OptionalDependency<FreeDriveControl> freeDriveControl, OptionalDependency<DragRacingControlMultiplayer> dragRacingControlMultiplayer, OptionalDependency<MapIconsModel> mapIconsModel, ICarLegitService carLegitService, BadCarConfig badCarConfig, IGameFactManager gameFactManager)
	{
		_levelsProvider = levelsProvider;
		FreeDriveProvider = freeDriveProvider;
		OnlineCollectionsProvider = onlineCollectionsProvider;
		_allCarsInfo = allCarsInfo;
		_rpcCollection = rpcCollection;
		_currencyManagerController = currencyManagerController;
		_carFactoryService = carFactoryService;
		DrawQueue = drawQueue;
		_vinylDrawer = vinylDrawer;
		_playerDataStorage = playerDataStorage;
		_vinylsDataUtility = vinylsDataUtility;
		_ramHelper = ramHelper;
		_freeDriveControlDep = freeDriveControl;
		_dragControlMultiDep = dragRacingControlMultiplayer;
		_mapIconModelDep = mapIconsModel;
		_carLegitService = carLegitService;
		_badCarConfig = badCarConfig;
		_gameFactManager = gameFactManager;
	}

	private void Awake()
	{
		_mapIconsModel = _mapIconModelDep?.Value;
		_freeDriveControl = _freeDriveControlDep?.Value;
		_dragControlMulti = _dragControlMultiDep?.Value;
		InitPackers();
		WindowColor = Color.magenta;
		WindowColorReflective = Color.magenta;
	}

	private void Start()
	{
		if (FlagsStorage.IsMultiFree || PhotonNetwork.IsConnected)
		{
			CachedSpectators = new HashSet<string>();
			_interestGroupDisposable?.Dispose();
			_intervalObs = Observable.Interval(TimeSpan.FromSeconds(45.0)).TakeUntilDestroy(this);
			_interestGroupDisposable = Observable.Timer(TimeSpan.FromSeconds(5.0)).Concat(_intervalObs).TakeUntilDestroy(this)
				.ObserveOnMainThread()
				.Subscribe(ProcessInterestGroup)
				.AddTo(this);
			PersonMultiplayerDataSending.OnPersonDataRemoved -= OnPlayerDataRemoved;
			PersonMultiplayerDataSending.OnPersonDataRemoved += OnPlayerDataRemoved;
		}
		koff3 = 3f;
		idIntensity = Shader.PropertyToID("_Intensity");
		WindowColor = Color.magenta;
		WindowColorReflective = Color.magenta;
		correctPlayerRot = Quaternion.identity;
		SetReferences();
		GetComponent<PhotonView>().OwnershipTransfer = OwnershipOption.Takeover;
		if (!base.photonView.IsMine && base.gameObject != V.Car && (bool)GetComponent<CharacterJoint>())
		{
			UnityEngine.Object.Destroy(GetComponent<CharacterJoint>());
			_rigidBody.ResetCenterOfMass();
			_rigidBody.ResetInertiaTensor();
			_rigidBody.velocity = Vector3.zero;
			_rigidBody.angularVelocity = Vector3.zero;
		}
		if (FlagsStorage.IsMainMenu)
		{
			base.enabled = false;
		}
		CarLegitBuffer();
		MoveElements.GlobalPointMoved += PassGlobalPointToPassengers;
	}

	private void OnPlayerDataRemoved(MultiPersonControl mpc)
	{
		if (mpc != null && !mpc.ownPlayer && CachedSpectators.Contains(mpc.ThisPlayerID))
		{
			CachedSpectators.Remove(mpc.ThisPlayerID);
		}
	}

	private void ProcessInterestGroup(long _ = 0L)
	{
		if ((bool)base.photonView && base.photonView.IsMine)
		{
			base.photonView.InterestGroupRPC("OnReceivedMileageFuelTireHealth", InterestGroup, RpcTarget.Others, PackMileageFuelTireHealth());
		}
	}

	private void OnDestroy()
	{
		_interestGroupDisposable?.Dispose();
		_interestGroupDisposable = null;
		_intervalObs = null;
		_resetSkipLerp?.Dispose();
		_resetSkipLerp = null;
		_sendPreciseDataDisposable?.Dispose();
		_sendPreciseDataDisposable = null;
		GlobalEventSystem.OnOnlineVinylsVisibilityChanged -= UpdateVinylsState;
		MoveElements.GlobalPointMoved -= PassGlobalPointToPassengers;
		PersonMultiplayerDataSending.OnPersonDataRemoved -= OnPlayerDataRemoved;
		if ((bool)base.photonView)
		{
			OnlineCollectionsProvider.UnregisterCar(this);
		}
		if ((bool)label && FreeDriveProvider != null && (bool)FreeDriveProvider.MultiPlayer && FreeDriveProvider.MultiPlayer.allLabels != null)
		{
			FreeDriveProvider.MultiPlayer.allLabels.Remove(label.gameObject);
		}
		if (TryGetComponent<VehicleData>(out var component))
		{
			component.mppc = null;
		}
		if (TryGetComponent<LightController>(out var component2))
		{
			component2.NullMppcRef();
		}
		SteeringWheel[] componentsInChildren = GetComponentsInChildren<SteeringWheel>(includeInactive: true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].NullMppcRef();
		}
	}

	private void SetReferences()
	{
		TryGetComponent<Powertrain>(out _powertrain);
		TryGetComponent<Axles>(out _axles);
		TryGetComponent<Rigidbody>(out _rigidBody);
		TryGetComponent<VehicleData>(out VehicleData);
		TryGetComponent<LightController>(out LightController);
		TryGetComponent<SoundController>(out SoundController);
		_cam = V.CoreGameManager.gameObject.GetComponent<Camera>();
		if (!_referencesWasSet)
		{
			_referencesWasSet = true;
		}
	}

	private void OnPhotonInstantiate2(long _ = 0L)
	{
		WindowColor = Color.magenta;
		WindowColorReflective = Color.magenta;
		createdTime = Time.time;
		if (FlagsStorage.IsMainMenu)
		{
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
			if ((bool)base.photonView && !base.photonView.IsMine)
			{
				DisableComponentsForSpawn();
				Rigidbody component = GetComponent<Rigidbody>();
				if ((bool)component)
				{
					component.isKinematic = true;
					component.detectCollisions = false;
				}
			}
			Observable.Timer(TimeSpan.FromSeconds(2.0)).Subscribe(OnPhotonInstantiate2).AddTo(this);
			return;
		}
		SetReferences();
		Observable.Timer(TimeSpan.FromSeconds(2.0)).Subscribe(OnPhotonInstantiate1).AddTo(this);
		if (!base.photonView.IsMine)
		{
			GlobalEventSystem.OnOnlineVinylsVisibilityChanged += UpdateVinylsState;
			isOnlineCar = true;
			OnlineCollectionsProvider.RegisterCar(this);
			_enableAtSpawnProgress = 0;
			DisableComponentsForSpawn();
			V.RaiseNewCarCreated(this);
			ApplyPendingCarData();
			Rigidbody component2 = GetComponent<Rigidbody>();
			component2.isKinematic = true;
			component2.detectCollisions = false;
			Observable.Timer(TimeSpan.FromSeconds(0.5)).Subscribe(EnableAllComponent).AddTo(this);
			Observable.Timer(TimeSpan.FromSeconds(3.0)).Subscribe(ResetIgnorCollision).AddTo(this);
			FindFirstAndOwnCar();
			MapIconsModel?.RegisterCar(VehicleData);
			BadCarsChecker.CarDataCheck(VehicleData);
		}
		else
		{
			DataApplied = true;
			FreeDriveProvider.FirstCarMultiPlayer = base.gameObject;
			V.LastUsedCar = base.gameObject;
			FlagsStorage.ForceSendValues = true;
			FreeDriveProvider.MultiPlayer.ResetForceSend();
			Color defaultValue = Color.white;
			Color gray = Color.gray;
			if ((bool)VehicleData.StockMaterial)
			{
				defaultValue = VehicleData.StockMaterial.color;
			}
			CarColor = PlayerDatas.GetColor("CarColor" + VehicleData.CarId, defaultValue);
			CarColorReflective = PlayerDatas.GetColor("CarColorReflective" + VehicleData.CarId, gray);
			if ((bool)VehicleData.WheelComposition.WheelMatFront1)
			{
				WheelColor = PlayerDatas.GetColor("Car" + VehicleData.CarId + "W", VehicleData.WheelComposition.WheelMatFront1.color);
				WheelColorReflective = PlayerDatas.GetColor("WheelColorReflective" + VehicleData.CarId, VehicleData.WheelComposition.WheelMatFront1.GetColor("_ReflectColor"));
				VehicleData.WheelComposition.WheelMatFront1.SetColor("_SpecColor", WheelColorSpecular);
			}
			if ((bool)VehicleData.WheelComposition.WheelMatFront2)
			{
				Wheel2Color = PlayerDatas.GetColor("Car" + VehicleData.CarId + "W2", VehicleData.WheelComposition.WheelMatFront1.color);
				Wheel2ColorReflective = PlayerDatas.GetColor("WheelColorReflective2" + VehicleData.CarId, VehicleData.WheelComposition.WheelMatFront2.GetColor("_ReflectColor"));
				VehicleData.WheelComposition.WheelMatFront2.SetColor("_SpecColor", WheelColorSpecular);
			}
			VehicleData.generatedID = ObscuredPrefs.GetString("carID" + VehicleData.CarId);
		}
		if ((bool)GetComponent<CharacterJoint>())
		{
			UnityEngine.Object.Destroy(GetComponent<CharacterJoint>());
		}
		Scene sceneByBuildIndex = SceneManager.GetSceneByBuildIndex(13);
		SceneManager.MoveGameObjectToScene(base.gameObject, sceneByBuildIndex);
		base.gameObject.transform.parent = V.CoreGameManager.globalPoint.transform;
	}

	public void FindFirstAndOwnCar()
	{
		MultiPersonControl[] playersOnlineNow = OnlineCollectionsProvider.PlayersOnlineNow;
		foreach (MultiPersonControl multiPersonControl in playersOnlineNow)
		{
			if ((bool)multiPersonControl && multiPersonControl.photonView.CreatorActorNr == base.photonView.CreatorActorNr)
			{
				multiPersonControl.FirstAndOwnCar = base.gameObject;
				break;
			}
		}
	}

	public void OnPhotonInstantiate(PhotonMessageInfo info)
	{
		if (GetComponent<PhotonView>().Owner.CustomProperties.TryGetValue("IG", out var value) && InterestGroup == 0)
		{
			InterestGroup = (byte)Mathf.Clamp((byte)value * 2 - 1, 0, 255);
		}
		GetComponent<VehicleData>().CarOwnerID = GetComponent<PhotonView>().Owner.ActorNumber;
		OnPhotonInstantiate2(0L);
	}

	private void OnPhotonInstantiate1(long _)
	{
		if (!base.photonView.IsMine)
		{
			GetComponent<Powertrain>().OnLineCar = true;
			CarTriggers carTriggers = base.gameObject.AddComponent<CarTriggers>();
			carTriggers.LevelScript = _levelsProvider.LevelScript;
			carTriggers.FreeDriveProvider = FreeDriveProvider;
			carTriggers.LevelsProvider = _levelsProvider;
			ApplyPendingCarData();
		}
		startCheckSkiping();
	}

	private void DisableComponentsForSpawn()
	{
		MonoBehaviour[] components = GetComponents<MonoBehaviour>();
		foreach (MonoBehaviour monoBehaviour in components)
		{
			if ((bool)monoBehaviour && !_disabledAtSpawn.Contains(monoBehaviour))
			{
				monoBehaviour.enabled = false;
				_disabledAtSpawn.Add(monoBehaviour);
			}
		}
	}

	private void EnableAllComponent(long _)
	{
		Powertrain component = GetComponent<Powertrain>();
		float num = Time.realtimeSinceStartup + 0.002f;
		while (_enableAtSpawnProgress < _disabledAtSpawn.Count)
		{
			MonoBehaviour monoBehaviour = _disabledAtSpawn[_enableAtSpawnProgress];
			_enableAtSpawnProgress++;
			if ((bool)monoBehaviour && !(monoBehaviour == component))
			{
				monoBehaviour.enabled = true;
				if (Time.realtimeSinceStartup >= num && _enableAtSpawnProgress < _disabledAtSpawn.Count)
				{
					Observable.Timer(TimeSpan.FromSeconds(0.05000000074505806)).Subscribe(EnableAllComponent).AddTo(this);
					return;
				}
			}
		}
		_disabledAtSpawn.Clear();
		_enableAtSpawnProgress = 0;
		DriverController component2 = GetComponent<DriverController>();
		if ((bool)component2)
		{
			component2.enabled = true;
		}
	}

	public void startCheckSkiping()
	{
		Observable.Timer(TimeSpan.FromSeconds(1.0)).Concat(Observable.Interval(TimeSpan.FromSeconds(0.30000001192092896))).Subscribe(CheckDistance)
			.AddTo(this);
		Observable.Timer(TimeSpan.FromSeconds(1.0)).Concat(Observable.Interval(TimeSpan.FromSeconds(1.0))).Subscribe(SetPos3Sec)
			.AddTo(this);
	}

	public void ResetIgnorCollisionDelayed(float delay)
	{
		Observable.Timer(TimeSpan.FromSeconds(delay)).Subscribe(ResetIgnorCollision).AddTo(this);
	}

	private void ResetIgnorCollision(long __)
	{
		Rigidbody component = GetComponent<Rigidbody>();
		if (!base.photonView.IsMine)
		{
			PlayerDatas.IgnoreCollisionWithOtherCars(base.gameObject, ignore: true, OnlineCollectionsProvider, out var checkAgain);
			ignoreCollision = true;
			if ((bool)V.Car && V.Car != base.gameObject && V.Car.TryGetComponent<MultiPlayerPosControl>(out var component2) && component2.ignoreCollision)
			{
				PlayerDatas.IgnoreCollisionWithOtherCars(V.Car, ignore: true, OnlineCollectionsProvider, out checkAgain);
			}
		}
		component.detectCollisions = true;
		if (VehicleData.Towing || ((bool)V.Car && V.Car == base.gameObject))
		{
			component.isKinematic = VehicleData.Towing;
		}
	}

	private void UpdateSqueezeGuard()
	{
		GameObject car = V.Car;
		if (!car || car == base.gameObject)
		{
			_squeezeGuardActive = false;
		}
		else
		{
			if (!_squeezeGuardActive && (distance > 30f || ignoreCollision))
			{
				return;
			}
			VehicleData getVehicleData = _carInputManager.GetVehicleData;
			Collider collider = (getVehicleData ? getVehicleData.Collider : null);
			Collider collider2 = (VehicleData ? VehicleData.Collider : null);
			if (!collider || !collider2)
			{
				return;
			}
			if (_squeezeGuardActive)
			{
				Bounds bounds = collider.bounds;
				bounds.Expand(0.3f);
				if (!bounds.Intersects(collider2.bounds))
				{
					_squeezeGuardActive = false;
					bool flag = (bool)getVehicleData.mppc && getVehicleData.mppc.ignoreCollision;
					CarColliders.SetIgnore(getVehicleData, VehicleData, ignoreCollision | flag);
				}
			}
			else if (IsSqueezedVertically(collider.bounds, collider2.bounds))
			{
				_squeezeGuardActive = true;
				CarColliders.SetIgnore(getVehicleData, VehicleData, ignore: true);
			}
		}
	}

	private static bool IsSqueezedVertically(Bounds a, Bounds b)
	{
		if (!IsSqueezedFromAbove(a, b))
		{
			return IsSqueezedFromAbove(b, a);
		}
		return true;
	}

	private static bool IsSqueezedFromAbove(Bounds bottom, Bounds top)
	{
		if (top.min.y < bottom.min.y + 0.35f)
		{
			return false;
		}
		float num = Mathf.Min(bottom.max.x, top.max.x) - Mathf.Max(bottom.min.x, top.min.x);
		float num2 = Mathf.Min(bottom.max.y, top.max.y) - Mathf.Max(bottom.min.y, top.min.y);
		float num3 = Mathf.Min(bottom.max.z, top.max.z) - Mathf.Max(bottom.min.z, top.min.z);
		if (num > 0.15f && num3 > 0.15f)
		{
			return num2 > 0.05f;
		}
		return false;
	}

	private void CheckDistance(long _)
	{
		if (V.Car != base.gameObject)
		{
			if (forceDisableCar || V.CoreGameManager.MenuLevelParent.FPSDisplay2.CurrentFPS < 5f)
			{
				DisableCar();
			}
			else
			{
				if (!V.DriverPerson)
				{
					return;
				}
				distance = _interestGroupManager.GetDistanceToCar(this);
				Vector3 vector = V.CoreGameManager.transform.InverseTransformPoint(base.gameObject.transform.position);
				if (FlagsStorage.IsPhotoMode && (bool)V.CoreGameManager.ManagerUI && (bool)V.CoreGameManager.ManagerUI.PhotoModeWindow && (bool)V.CoreGameManager.ManagerUI.PhotoModeWindow.PhotoModePrototype && (bool)V.CoreGameManager.ManagerUI.PhotoModeWindow.PhotoModePrototype.captureCamera)
				{
					vector = V.CoreGameManager.ManagerUI.PhotoModeWindow.PhotoModePrototype.captureCamera.transform.InverseTransformPoint(base.gameObject.transform.position);
					distance = Vector3.Distance(base.transform.position, V.CoreGameManager.ManagerUI.PhotoModeWindow.PhotoModePrototype.captureCamera.transform.position);
				}
				tempAngel = Mathf.Atan2(vector.x, vector.z) * 57.29578f;
				tempAngel = Mathf.Abs(tempAngel);
				ShowHideTopLabel(distance);
				if ((bool)speedLimitIcon)
				{
					if (FlagsStorage.NowPolice)
					{
						speedLimitIcon.SetActive(_powertrain.SpeedMpsOnline > 90f);
					}
					else if (speedLimitIcon.activeSelf)
					{
						speedLimitIcon.SetActive(value: false);
					}
				}
				float num = _cam.farClipPlane;
				if (num > 300f)
				{
					num = 300f;
				}
				if ((distance > num || (tempAngel > 90f && distance > 20f)) && base.gameObject != FreeDriveProvider.RacingCar2)
				{
					FarFromPerson = true;
					DisableCar();
				}
				else
				{
					FarFromPerson = false;
					EnableCar();
				}
				if (distance < _cam.farClipPlane + 10f && tempAngel < 100f && !VehicleData.MainModel)
				{
					VehicleData.GetPowertrain.enabled = true;
				}
				if (carEnabled && base.enabled)
				{
					int quality = CPMGraphicsSettingsManager.Instance.Quality;
					if (base.photonView.IsMine)
					{
						closeToPerson = true;
					}
					else
					{
						closeToPerson = distance < (float)(40 + 20 * quality);
					}
					foreach (WheelProvider allWheel in _axles.AllWheels)
					{
						allWheel.enabled = true;
					}
					bool flag = distance < (float)(50 + 10 * quality);
					SoundController.enabled = flag;
				}
				if (carEnabled)
				{
					bool flag2 = false;
					if (!VehicleData.MainModel)
					{
						flag2 = _powertrain.SpeedMpsOnline > 70f;
						if ((bool)V.Car && !flag2)
						{
							Rigidbody getRigidbody = _carInputManager.GetRigidbody;
							if ((bool)getRigidbody)
							{
								flag2 = getRigidbody.velocity.magnitude * 3.6f > 70f;
							}
						}
					}
					if (base.photonView.IsMine)
					{
						_rigidBody.isKinematic = Mathf.Abs(_rigidBody.velocity.y) < 5f && distance > 70f;
					}
					if (((distance > V.DetailDistance) | flag2) || VehicleData.BodyLoadingComposition.LODModel == null)
					{
						if ((bool)VehicleData.BodyLoadingComposition.LODModel)
						{
							VehicleData.BodyLoadingComposition.LODModel.SetActive(value: true);
							if ((bool)VehicleData.MainModel)
							{
								VehicleData.BodyLoadingComposition.SetActiveMainModel(isOn: false);
							}
						}
						else
						{
							VehicleData.BodyLoadingComposition.StartLODLoad(immediately: false, base.gameObject != FreeDriveProvider.FirstCarMultiPlayer).Forget();
						}
					}
					else if ((bool)VehicleData.MainModel)
					{
						VehicleData.BodyLoadingComposition.ShowLod(val: false);
					}
					else if (MustDownloadMainModel())
					{
						if (V.SceneControl != null && !V.SceneControl.TryStartMainBodyLoad())
						{
							VehicleData.BodyLoadingComposition.ShowLod(val: true);
						}
						else
						{
							MultiplayerDataSending.ApplyAllVisual(this, _carFactoryService);
						}
					}
					else
					{
						VehicleData.BodyLoadingComposition.ShowLod(val: true);
					}
					if ((bool)_axles && _axles.AllWheels.Count > 0)
					{
						bool lodWheel = distance > V.DetailDistance + 5f;
						VehicleData.BodyLoadingComposition.SetLodWheel(lodWheel);
					}
				}
				CheckToUnloadBody();
			}
			return;
		}
		EnableCar();
		FlagsStorage.RaycastForSpeeding = false;
		MultiPlayerPosControl[] carsOnlineNow = OnlineCollectionsProvider.CarsOnlineNow;
		foreach (MultiPlayerPosControl multiPlayerPosControl in carsOnlineNow)
		{
			if ((bool)multiPlayerPosControl && !(multiPlayerPosControl == this) && multiPlayerPosControl.speeding && multiPlayerPosControl.carEnabled)
			{
				FlagsStorage.RaycastForSpeeding = true;
				break;
			}
		}
	}

	private void CheckToUnloadBody()
	{
		if (base.gameObject != FreeDriveProvider.RacingCar2 && base.gameObject != FreeDriveProvider.FirstCarMultiPlayer)
		{
			int num = 5000;
			if (_ramHelper.ReachedRamLimit)
			{
				num = 70;
			}
			if (distance > (float)num && !carEnabled)
			{
				VehicleData.BodyLoadingComposition.Unload();
				vinylLoaded = false;
			}
		}
	}

	private void SetPos3Sec(long _)
	{
		if (base.photonView.IsMine)
		{
			correctPlayerPos = base.transform.localPosition;
			correctPlayerRot = base.transform.localRotation;
		}
		else if (FarFromPerson)
		{
			base.transform.SetLocalPositionAndRotation(correctPlayerPos, correctPlayerRot);
			if ((bool)VehicleData.TowingComposition.LoadCar)
			{
				MoveLoadCar();
			}
		}
	}

	private void DisableCar()
	{
		if (!(V.Car != base.gameObject) || !(base.gameObject != FreeDriveProvider.RacingCar2) || LoadingWheelNow || !carEnabled)
		{
			return;
		}
		if (_rigidBody.inertiaTensor.magnitude == 0f)
		{
			_rigidBody.inertiaTensor = Vector3.one;
		}
		base.gameObject.SetActive(value: false);
		for (int i = 0; i < VehicleData.VehicleMembersController.Members.Count; i++)
		{
			if ((bool)VehicleData.VehicleMembersController.Members[i])
			{
				VehicleData.VehicleMembersController.Members[i].SetActive(value: false);
				MultiPersonControl component = VehicleData.VehicleMembersController.Members[i].GetComponent<MultiPersonControl>();
				if ((bool)component)
				{
					component.enabled = true;
				}
			}
		}
		base.enabled = true;
		carEnabled = false;
	}

	public void EnableCar()
	{
		if (!carEnabled)
		{
			base.gameObject.SetActive(value: true);
			base.enabled = true;
			carEnabled = true;
			VehicleData.VehicleMembersController.SetActiveMember(isActive: true);
			if ((bool)VehicleData.MainModel && VehicleData.MainModel.TryGetComponent<BodyMarker>(out var component) && (bool)component.hood)
			{
				component.OpenHood(hoodState, fast: true);
			}
		}
	}

	private void ClampPositions()
	{
		Vector3 vector = correctPlayerPos;
		vector.x = Mathf.Clamp(vector.x, -20000f, 20000f);
		vector.z = Mathf.Clamp(vector.z, -20000f, 20000f);
		vector.y = Mathf.Clamp(vector.y, -500f, 500f);
		correctPlayerPos = vector;
		vector = base.transform.position;
		vector.x = Mathf.Clamp(vector.x, -20000f, 20000f);
		vector.z = Mathf.Clamp(vector.z, -20000f, 20000f);
		vector.y = Mathf.Clamp(vector.y, -500f, 500f);
		base.transform.position = vector;
		if (base.transform.rotation == new Quaternion(0f, 0f, 0f, 0f))
		{
			base.transform.rotation = Quaternion.identity;
		}
		if (!(base.transform.position.y < -499f))
		{
			return;
		}
		if (!FlagsStorage.OutOfCar)
		{
			if (FlagsStorage.IsFreeDriveMode)
			{
				FreeDriveProvider.FreeDriveControl.ToRoad(ToRoadSource.FellThroughWorld);
			}
			else if (FlagsStorage.IsLevelScene && base.gameObject == V.Car)
			{
				_levelsProvider.LevelScript.StartLevel();
			}
		}
		else if (base.photonView.IsMine)
		{
			Vector3 position = base.transform.position;
			position.y = 5f;
			base.transform.position = position;
			_rigidBody.velocity = Vector3.zero;
		}
	}

	private bool MustDownloadMainModel()
	{
		if (base.gameObject == V.Car)
		{
			return true;
		}
		if (V.CoreGameManager.MenuLevelParent.FPSDisplay2.CurrentFPS < 5f)
		{
			return false;
		}
		if ((bool)VehicleData.BodyLoadingComposition.BodyLod && !VehicleData.BodyLoadingComposition.BodyLod.mainLODBody.isVisible)
		{
			return false;
		}
		return CheckIsVisible();
	}

	private bool CheckIsVisible()
	{
		Vector3 vector = V.SceneControl.mainCamera.WorldToViewportPoint(base.transform.position);
		if (vector.x < 1f && vector.x > 0f)
		{
			float y = vector.y;
			if (y < 1f && y > 0f && vector.z > 0f)
			{
				return vector.z < V.SceneControl.mainCamera.farClipPlane;
			}
		}
		return false;
	}

	public void ForceBuildVisual()
	{
		if (!_steamVisualBuildStarted && !(base.gameObject == V.Car) && DataApplied)
		{
			_steamVisualBuildStarted = true;
			if (!VehicleData.MainModel)
			{
				MultiplayerDataSending.ApplyAllVisual(this, _carFactoryService);
			}
		}
	}

	private void SmoothPedal(float time, float release, bool isAnalog, float input, ref float output)
	{
		if (isAnalog)
		{
			if (input > output)
			{
				float num = 1f / (time + 1E-06f);
				output = Mathf.MoveTowards(output, input, Time.deltaTime * num);
			}
			else
			{
				float num2 = output / (release + 1E-06f);
				output = Mathf.MoveTowards(output, input, Time.deltaTime * num2);
			}
		}
		else
		{
			float num3 = ((input > output) ? (1f / time) : (output / (release + 1E-06f)));
			output = Mathf.MoveTowards(output, input, Time.deltaTime * num3);
		}
	}

	private void createGhostcreateGhost()
	{
		GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
		gameObject.GetComponent<Collider>().enabled = false;
		gameObject.transform.parent = V.GlobalPoint;
		gameObject.transform.name = "Ghost";
		ghost = gameObject;
	}

	private void Update()
	{
		if (!PhotonNetwork.IsConnected || !FlagsStorage.IsMultiFree || !base.photonView)
		{
			return;
		}
		ClampPositions();
		if (!_powertrain.OnLineCar)
		{
			RPM = _powertrain.CurrentRpm;
		}
		if (base.photonView.IsMine)
		{
			correctPlayerPos = base.transform.localPosition;
			correctPlayerRot = base.transform.localRotation;
		}
		else
		{
			UpdateSqueezeGuard();
		}
		if (!base.photonView.IsMine && !FarFromPerson)
		{
			float deltaTime = Time.deltaTime;
			if ((bool)VehicleData.renderBody)
			{
				isVisible = VehicleData.renderBody;
			}
			_visualSteer = Mathf.Lerp(_visualSteer, ReceivedSteer, 5f * Time.deltaTime);
			if ((bool)_powertrain)
			{
				_powertrain.SetSteer(_visualSteer, useAssistent: false);
			}
			if ((bool)_axles)
			{
				_axles.FrontAxle.LeftWheel.AngularMultiplayerVelocity = Mathf.Lerp(_axles.FrontAxle.LeftWheel.AngularMultiplayerVelocity, rotFL, Time.deltaTime * (2f + Mathf.Abs(rotFL)));
				_axles.FrontAxle.RightWheel.AngularMultiplayerVelocity = Mathf.Lerp(_axles.FrontAxle.RightWheel.AngularMultiplayerVelocity, rotFR, Time.deltaTime * (2f + Mathf.Abs(rotFR)));
				_axles.RearAxle.LeftWheel.AngularMultiplayerVelocity = Mathf.Lerp(_axles.RearAxle.LeftWheel.AngularMultiplayerVelocity, rotRL, Time.deltaTime * (2f + Mathf.Abs(rotRL)));
				_axles.RearAxle.RightWheel.AngularMultiplayerVelocity = Mathf.Lerp(_axles.RearAxle.RightWheel.AngularMultiplayerVelocity, rotRR, Time.deltaTime * (2f + Mathf.Abs(rotRR)));
				_axles.FrontAxle.LeftWheel.IsSkidmarkOnline = isSkidCarFL;
				_axles.FrontAxle.RightWheel.IsSkidmarkOnline = isSkidCarFR;
				_axles.RearAxle.LeftWheel.IsSkidmarkOnline = isSkidCarRL;
				_axles.RearAxle.RightWheel.IsSkidmarkOnline = isSkidCarRR;
				if (_axles.Rear1Axle.Wheels.Count > 0)
				{
					_axles.Rear1Axle.LeftWheel.AngularMultiplayerVelocity = Mathf.Lerp(_axles.Rear1Axle.LeftWheel.AngularMultiplayerVelocity, rotRL, Time.deltaTime * (2f + Mathf.Abs(rotRL)));
					_axles.Rear1Axle.RightWheel.AngularMultiplayerVelocity = Mathf.Lerp(_axles.Rear1Axle.RightWheel.AngularMultiplayerVelocity, rotRR, Time.deltaTime * (2f + Mathf.Abs(rotRR)));
					_axles.Rear1Axle.LeftWheel.IsSkidmarkOnline = isSkidCarRL;
					_axles.Rear1Axle.RightWheel.IsSkidmarkOnline = isSkidCarRR;
				}
			}
			float num = Vector3.Distance(base.transform.localPosition, correctPlayerPos);
			RevTrigger = RPM > _powertrain.RedlineRpm;
			float num2 = ((RPM > LerpedRPM) ? 0.02f : 0.08f);
			float num3 = deltaTime / (num2 + deltaTime);
			LerpedRPM += (RPM - LerpedRPM) * num3;
			float num4 = LerpedRPM - RPM;
			float f = ((num4 > 0f) ? 30000f : 20000f) * deltaTime;
			LerpedRPM = RPM + Mathf.Clamp(num4, 0f - Mathf.Abs(f), Mathf.Abs(f));
			float limiterFlutterHz = _powertrain.LimiterFlutterHz;
			if (RevTrigger && limiterFlutterHz > 0f)
			{
				_limiterPhase += deltaTime * limiterFlutterHz;
				_limiterPhase -= Mathf.Floor(_limiterPhase);
				cutPhase = _limiterPhase < _powertrain.LimiterCutDuty;
			}
			else
			{
				_limiterPhase = 0f;
				cutPhase = false;
			}
			float num5 = 0f;
			if (RevTrigger)
			{
				num5 = _powertrain.LimiterBounceDepthRpm * (0.5f - 0.5f * Mathf.Cos(_limiterPhase * 2f * (float)Math.PI));
			}
			_powertrain.CurrentRpm = LerpedRPM - num5;
			float num6 = (cutPhase ? 0f : _powertrain.Throttle);
			_onlineAccel = ((_onlineAccel >= num6) ? num6 : Mathf.Min(_onlineAccel + 25f * deltaTime, num6));
			_powertrain.SetOnlineAccel(_onlineAccel);
			_powertrain.SetOnlineRevLimiterTriggered(cutPhase);
			if (_powertrain.MaxRpm > 0f)
			{
				bool flag = RPM >= 150f;
				_powertrain.SetOnlineEngineRunning(flag);
				if (!flag)
				{
					LerpedRPM = 0f;
				}
			}
			float num7 = _powertrain.SpeedMpsOnline / 100f;
			if (VehicleData.Towing)
			{
				return;
			}
			float num8 = 40f + _powertrain.SpeedMpsOnline / 5f;
			if (!FlagsStorage.InGarageFree)
			{
				if (num < num8)
				{
					if (Mathf.Abs(base.transform.localPosition.y - correctPlayerPos.y) > 2f)
					{
						base.transform.localPosition = Vector3.Lerp(base.transform.localPosition, correctPlayerPos, Time.deltaTime * (koff3 + 5f + num7));
					}
					base.transform.localPosition = Vector3.Lerp(base.transform.localPosition, correctPlayerPos, deltaTime * (koff3 + num7));
				}
				else
				{
					base.transform.localPosition = correctPlayerPos;
				}
				if (FlagsStorage.RacingGameMode && !FlagsStorage.RaceStarted)
				{
					base.transform.localRotation = correctPlayerRot;
				}
				else
				{
					base.transform.localRotation = Quaternion.Lerp(base.transform.localRotation, correctPlayerRot, deltaTime * koff3);
				}
			}
		}
		if ((bool)VehicleData.TowingComposition.LoadCar)
		{
			MoveLoadCar();
		}
	}

	private void MoveLoadCar()
	{
		Vector3 position = loadCarLocalTarget.position;
		position.y = VehicleData.TowingComposition.LoadCar.transform.position.y;
		position.y = Mathf.Clamp(position.y, loadCarLocalTarget.position.y - 0.25f, loadCarLocalTarget.position.y + 0.25f);
		position.y = Mathf.Lerp(position.y, loadCarLocalTarget.position.y, (Mathf.Abs(_rigidBody.velocity.y) * 5f + 5f) * Time.deltaTime);
		VehicleData.TowingComposition.LoadCar.transform.SetPositionAndRotation(position, loadCarLocalTarget.rotation);
	}

	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (_referencesWasSet)
		{
			if (stream.IsWriting)
			{
				StreamSend(stream, info);
			}
			else
			{
				StreamReceive(stream, info);
			}
		}
	}

	private void SetRotateByTransform(bool front, bool rear)
	{
		if ((bool)_axles)
		{
			_axles.FrontAxle.LeftWheel.RotateModelByTransform = front;
			_axles.FrontAxle.RightWheel.RotateModelByTransform = front;
			_axles.RearAxle.LeftWheel.RotateModelByTransform = rear;
			_axles.RearAxle.RightWheel.RotateModelByTransform = rear;
			if (_axles.Rear1Axle.Wheels.Count > 0)
			{
				_axles.Rear1Axle.LeftWheel.RotateModelByTransform = rear;
				_axles.Rear1Axle.RightWheel.RotateModelByTransform = rear;
			}
			if (_axles.Rear2Axle.Wheels.Count > 0)
			{
				_axles.Rear2Axle.LeftWheel.RotateModelByTransform = rear;
				_axles.Rear2Axle.RightWheel.RotateModelByTransform = rear;
			}
		}
		isSkidCarFL = (isSkidCarFR = !front);
		isSkidCarRL = (isSkidCarRR = !rear);
	}

	public void SetHeadAndIndicator(int iState, int HState, bool forceSet)
	{
		if ((LightController.IndicatorState.Value != (TurnSignalState)iState) | forceSet)
		{
			switch (iState)
			{
			case 0:
				LightController.TurnOffAllIndicators();
				break;
			case 1:
				LightController.TurnOnLeftIndicators();
				break;
			case 2:
				LightController.TurnOnRightIndicators();
				break;
			case 3:
				LightController.EmergencyLightsOn();
				break;
			default:
				LightController.TurnOffAllIndicators();
				break;
			}
			if (iState != 9)
			{
				LightController.IndicatorState.Value = (TurnSignalState)iState;
			}
		}
		if (!((LightController.HState != HState) | forceSet))
		{
			return;
		}
		switch (HState)
		{
		case 0:
			LightController.turnOffHeadLights();
			break;
		case 1:
			LightController.turnOnNearLights();
			break;
		case 2:
		case 3:
			LightController.turnOnFarLights();
			break;
		case 4:
			if ((bool)VehicleData.LightsComposition.PoliceLights)
			{
				VehicleData.LightsComposition.PoliceLights.TurnOnOffLights(enable: true);
			}
			break;
		case 5:
			if (VehicleData.LightsComposition.PoliceLights != null)
			{
				VehicleData.LightsComposition.PoliceLights.TurnOnOffLights(enable: false);
			}
			break;
		case 6:
			hoodState = true;
			if ((bool)VehicleData.MainModel)
			{
				BodyMarker component2 = VehicleData.MainModel.GetComponent<BodyMarker>();
				if ((bool)component2 && (bool)component2.hood)
				{
					component2.OpenHood(open: true);
				}
			}
			break;
		case 7:
			hoodState = false;
			if ((bool)VehicleData.MainModel)
			{
				BodyMarker component = VehicleData.MainModel.GetComponent<BodyMarker>();
				if ((bool)component && (bool)component.hood)
				{
					component.OpenHood(open: false);
				}
			}
			break;
		}
		LightController.HState = HState;
	}

	public UniTask ApplyDataIfNeedAsync(CancellationToken ct = default(CancellationToken))
	{
		return ApplyDataAsync(ct);
	}

	public UniTask ApplyDataAsync(CancellationToken ct = default(CancellationToken))
	{
		if (DataApplied)
		{
			return UniTask.CompletedTask;
		}
		if (!CanApplyData)
		{
			return UniTask.CompletedTask;
		}
		UniTaskCompletionSource uniTaskCompletionSource = _applyDataCompletion;
		if (uniTaskCompletionSource == null)
		{
			uniTaskCompletionSource = (_applyDataCompletion = new UniTaskCompletionSource());
			RunApplyDataPipeline(this.GetCancellationTokenOnDestroy()).Forget();
		}
		return uniTaskCompletionSource.Task.AttachExternalCancellation(ct);
	}

	private async UniTask RunApplyDataPipeline(CancellationToken ct)
	{
		_ = 1;
		try
		{
			if (ShouldSkipCarLegitCheck)
			{
				MultiplayerDataSending.CarApplyData(this, _allCarsInfo, _badCarConfig);
				return;
			}
			bool flag = await _carLegitService.IsCheckEnabledAsync(ct);
			if (!CanApplyData || ct.IsCancellationRequested)
			{
				return;
			}
			if (!flag)
			{
				MultiplayerDataSending.CarApplyData(this, _allCarsInfo, _badCarConfig);
				return;
			}
			var (flag2, carInstanceId, carId, firebaseId) = ResolveCarLegitInputs();
			if (!flag2)
			{
				MultiplayerDataSending.CarApplyData(this, _allCarsInfo, _badCarConfig, isCarLegit: false);
				return;
			}
			bool isCarLegit = await _carLegitService.IsCarLegitAsync(firebaseId, carInstanceId, carId, ct, flushImmediately: true);
			if (CanApplyData && !ct.IsCancellationRequested)
			{
				MultiplayerDataSending.CarApplyData(this, _allCarsInfo, _badCarConfig, isCarLegit);
			}
		}
		catch (OperationCanceledException)
		{
		}
		catch (Exception ex2)
		{
			Debug.LogError("ApplyData Exception on car:" + base.name + ":" + ex2.Message, base.gameObject);
		}
		finally
		{
			UniTaskCompletionSource applyDataCompletion = _applyDataCompletion;
			_applyDataCompletion = null;
			applyDataCompletion?.TrySetResult();
		}
	}

	private (bool success, string carInstanceId, int carId, string firebaseId) ResolveCarLegitInputs()
	{
		string item;
		int item2;
		string text;
		try
		{
			object[] instantiationData = base.photonView.InstantiationData;
			item = (string)instantiationData[0];
			item2 = (int)instantiationData[1] >> 8;
			text = ((instantiationData.Length > 7) ? (instantiationData[7] as string) : null);
		}
		catch (Exception)
		{
			return (success: false, carInstanceId: null, carId: 0, firebaseId: null);
		}
		if (string.IsNullOrEmpty(text))
		{
			return (success: false, carInstanceId: item, carId: item2, firebaseId: null);
		}
		return (success: true, carInstanceId: item, carId: item2, firebaseId: text);
	}

	private void CarLegitBuffer()
	{
		if (!ShouldSkipCarLegitCheck)
		{
			var (flag, carInstanceId, carId, firebaseId) = ResolveCarLegitInputs();
			if (flag)
			{
				_carLegitService.IsCarLegitAsync(firebaseId, carInstanceId, carId, base.destroyCancellationToken, flushImmediately: false).Forget();
			}
		}
	}

	public void UpdateVinylsState(bool isActive)
	{
		bool flag = isActive && !_vinylsBanned;
		if ((bool)VehicleData.CarMat && (bool)VehicleData.VinylsComposition.RenderTexture)
		{
			VehicleData.CarMat.mainTexture = (flag ? VehicleData.VinylsComposition.RenderTexture : null);
		}
		if ((bool)VehicleData.WindowComposition.WindowMat && (bool)VehicleData.WindowComposition.WindowRT)
		{
			VehicleData.WindowComposition.WindowMat.mainTexture = (flag ? VehicleData.WindowComposition.WindowRT : null);
		}
		if ((bool)VehicleData.PlatesComposition.FrontPlateMat && (bool)VehicleData.PlatesComposition.FrontPlateRT)
		{
			VehicleData.PlatesComposition.FrontPlateMat.mainTexture = (flag ? VehicleData.PlatesComposition.FrontPlateRT : null);
		}
		if ((bool)VehicleData.PlatesComposition.RearPlateMat && (bool)VehicleData.PlatesComposition.RearPlateRT)
		{
			VehicleData.PlatesComposition.RearPlateMat.mainTexture = (flag ? VehicleData.PlatesComposition.RearPlateRT : null);
		}
	}

	public async UniTaskVoid CreateSkidAfter(Color colorsSmokeColor)
	{
		await UniTask.Delay(TimeSpan.FromSeconds(1.0));
		colorsSmokeColor.a = 1f;
		if ((bool)this && (bool)base.gameObject)
		{
			PlayerDatas.CreateSkid(base.gameObject);
			if ((bool)_axles)
			{
				_smokeColor = colorsSmokeColor;
				PlayerDatas.SetSmokeColor(_axles, _smokeColor);
			}
		}
	}

	public void AfterWheelsLoaded()
	{
		LoadingWheelNow = false;
		AssignMats();
	}

	private void AssignMats()
	{
		if ((bool)VehicleData.WheelComposition.WheelMatFront1)
		{
			VehicleData.WheelComposition.WheelMatFront1.color = WheelColor;
			VehicleData.WheelComposition.WheelMatFront1.SetColor("_ReflectColor", WheelColorReflective);
			VehicleData.WheelComposition.WheelMatFront1.SetColor("_SpecColor", WheelColorSpecular);
			VehicleData.WheelComposition.WheelMatFront1.SetFloat("_Shininess", WShininess);
			VehicleData.WheelComposition.WheelMatFront1.SetFloat("_Smoothness", WSmoothness);
			if ((bool)VehicleData.WheelComposition.WheelMatLod)
			{
				VehicleData.WheelComposition.WheelMatLod.color = WheelColor;
			}
		}
		if ((bool)VehicleData.WheelComposition.WheelMatFront2)
		{
			VehicleData.WheelComposition.WheelMatFront2.color = Wheel2Color;
			VehicleData.WheelComposition.WheelMatFront2.SetColor("_ReflectColor", Wheel2ColorReflective);
			VehicleData.WheelComposition.WheelMatFront2.SetColor("_SpecColor", Wheel2ColorSpecular);
			VehicleData.WheelComposition.WheelMatFront2.SetFloat("_Shininess", W2Shininess);
			VehicleData.WheelComposition.WheelMatFront2.SetFloat("_Smoothness", W2Smoothness);
		}
	}

	public void SetAllMaterialColor()
	{
		if (!CanApplyData)
		{
			return;
		}
		if ((bool)VehicleData.LightsComposition.DiodMat)
		{
			VehicleData.LightsComposition.DiodMat.color = diodColor;
			LightController.IndicatorDiodMat = VehicleData.LightsComposition.DiodMat;
			LightController.indicatorColorDiod = diodColor;
		}
		if ((bool)VehicleData.LightsComposition.DiodMat2)
		{
			VehicleData.LightsComposition.DiodMat2.color = diodColor;
			LightController.turnLeftDiodMat = VehicleData.LightsComposition.DiodMat2;
			LightController.indicatorColorDiod = diodColor;
		}
		if ((bool)VehicleData.LightsComposition.DiodMat3)
		{
			VehicleData.LightsComposition.DiodMat3.color = diodColor;
			LightController.turnRightDiodMat = VehicleData.LightsComposition.DiodMat3;
			LightController.indicatorColorDiod = diodColor;
		}
		if ((bool)VehicleData.WheelComposition.WheelMatFront1)
		{
			VehicleData.WheelComposition.WheelMatFront1.color = WheelColor;
			VehicleData.WheelComposition.WheelMatFront1.SetColor("_ReflectColor", WheelColorReflective);
			VehicleData.WheelComposition.WheelMatFront1.SetColor("_SpecColor", WheelColorSpecular);
			VehicleData.WheelComposition.WheelMatFront1.SetFloat("_Shininess", WShininess);
			VehicleData.WheelComposition.WheelMatFront1.SetFloat("_Smoothness", WSmoothness);
			if ((bool)VehicleData.WheelComposition.WheelMatLod)
			{
				VehicleData.WheelComposition.WheelMatLod.color = WheelColor;
			}
		}
		if ((bool)VehicleData.WheelComposition.WheelMatFront2)
		{
			VehicleData.WheelComposition.WheelMatFront2.color = Wheel2Color;
			VehicleData.WheelComposition.WheelMatFront2.SetColor("_ReflectColor", Wheel2ColorReflective);
			VehicleData.WheelComposition.WheelMatFront2.SetColor("_SpecColor", Wheel2ColorSpecular);
			VehicleData.WheelComposition.WheelMatFront2.SetFloat("_Shininess", W2Shininess);
			VehicleData.WheelComposition.WheelMatFront2.SetFloat("_Smoothness", W2Smoothness);
		}
		if ((bool)VehicleData.WindowComposition.WindowMat)
		{
			if (WindowColorReflective != Color.magenta)
			{
				VehicleData.WindowComposition.WindowMat.SetColor("_SpecColor", WindowColorReflective);
			}
			if (WindowColor != Color.magenta)
			{
				VehicleData.WindowComposition.WindowMat.color = WindowColor;
			}
		}
		if ((bool)VehicleData.CarMat)
		{
			VehicleData.CarMat.SetColor("_SpecColor", CarColorSpecular);
			VehicleData.CarMat.SetFloat("_Shininess", Shininess);
			VehicleData.CarMat.SetFloat("_Smoothness", Smoothness);
			VehicleData.CarMat.SetColor("_ReflectColor", CarColorReflective);
		}
	}

	private async UniTaskVoid RenderVinyls(VinylsType type, object[] vinylsInstantiationData, bool isFrontPlate)
	{
		if (!VehicleData)
		{
			Debug.LogError("Missing carData");
			return;
		}
		try
		{
			Material material = null;
			RenderTexture renderTexture = _vinylDrawer.GetRenderTexture(type);
			switch (type)
			{
			case VinylsType.CarVinyls:
				VehicleData.VinylsComposition.RenderTexture = renderTexture;
				material = VehicleData.CarMat;
				break;
			case VinylsType.PlateVinyls:
				if (isFrontPlate)
				{
					VehicleData.PlatesComposition.FrontPlateRT = renderTexture;
					material = VehicleData.PlatesComposition.FrontPlateMat;
				}
				else
				{
					VehicleData.PlatesComposition.RearPlateRT = renderTexture;
					material = VehicleData.PlatesComposition.RearPlateMat;
				}
				break;
			case VinylsType.WindowVinyls:
				VehicleData.WindowComposition.WindowRT = renderTexture;
				material = VehicleData.WindowComposition.WindowMat;
				break;
			}
			if (!material)
			{
				Debug.LogError($"No material for {type} on {VehicleData.photonView.ViewID}");
				_vinylDrawer.DeleteVinylContext(type);
				return;
			}
			material.mainTexture = renderTexture;
			await UnpackVinyls(vinylsInstantiationData, type);
			_vinylDrawer.FocusOnVinyls(type);
			_vinylDrawer.TakeSnapshot();
		}
		catch (Exception message)
		{
			Debug.LogError(message);
		}
		finally
		{
			_vinylDrawer.DeleteVinylContext(type);
			bool isActive = PlayerPrefs.GetInt("hideAllVinyls", 0) == 0;
			UpdateVinylsState(isActive);
		}
	}

	private async UniTask<bool> UnpackVinyls(object[] photonVinylsData, VinylsType type = VinylsType.CarVinyls)
	{
		int countWait = 0;
		foreach (object onePhotonVinyl in photonVinylsData)
		{
			if (countWait % 10 == 0)
			{
				await UniTask.WaitForFixedUpdate();
			}
			object[] array = (object[])onePhotonVinyl;
			if (array != null)
			{
				VinylData vinylData = VinylsUtility.UnpackVinylInstantiationData(array);
				_vinylsDataUtility.CreateVinyl(vinylData, type);
				countWait++;
			}
		}
		return true;
	}

	public void updateLabel(GameObject Person, bool Show, bool hideLabel = false)
	{
		UpdateLabelInternal(Person, Show, hideLabel).Forget();
	}

	private async UniTaskVoid UpdateLabelInternal(GameObject Person, bool Show, bool hideLabel = false)
	{
		if (FlagsStorage.HideAllTopLabel && (bool)label)
		{
			label.gameObject.SetActive(value: false);
		}
		if (Show)
		{
			if (!label)
			{
				AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>("Player_label").BindTo(base.gameObject);
				await handle;
				if (!this || !Person)
				{
					return;
				}
				GameObject gameObject = UnityEngine.Object.Instantiate(handle.Result);
				label = gameObject.GetComponent<MeshRenderer>();
				SpeakingIcon = label.transform.Find("SpeakingIcon").gameObject;
				speedLimitIcon = label.transform.Find("speed limit").gameObject;
				VehicleData = GetComponent<VehicleData>();
				label.transform.parent = base.gameObject.transform;
				label.transform.localPosition = new Vector3(0f, VehicleData.RoofPosition + 0.5f, 0f);
				label.GetComponent<AllwaysLookAtCam>().cam = V.CoreGameManager.gameObject.transform;
				FreeDriveProvider.MultiPlayer.allLabels.Add(gameObject);
			}
			if (!Person)
			{
				return;
			}
			MultiPersonControl multiPersonControl = Person.GetComponent<MultiPersonControl>();
			while ((bool)multiPersonControl && !multiPersonControl.rank.HasValue)
			{
				await UniTask.Delay(TimeSpan.FromMilliseconds(10.0));
			}
			if (!this || !Person || !label)
			{
				return;
			}
			_rankCurrentPlayer = Person.GetComponent<MultiPersonControl>().rank;
			SmallRankIcon = label.transform.GetChild(3).gameObject;
			BigRankIcon = label.transform.GetChild(4).gameObject;
			Material material = UtilsRank.LoadRankMaterialFromResources((UtilsRank.RankType)(int)_rankCurrentPlayer.GetValueOrDefault());
			SmallRankIcon.GetComponent<MeshRenderer>().material = material;
			SmallRankIcon.GetComponent<MeshRenderer>().enabled = true;
			BigRankIcon.GetComponent<MeshRenderer>().material = material;
			BigRankIcon.GetComponent<MeshRenderer>().enabled = true;
			string nickName = Person.GetComponent<PhotonView>().Owner.NickName;
			if (TryGetComponent<Powertrain>(out var component))
			{
				component.CalcMaxPowerMaxTorque(out var maxHp, out var _, out var maxTq, out var _, out var _);
				TMP_FontAsset_Placeholder.EnsureFor(nickName);
				if (label.TryGetComponent<TextMeshPro>(out var component2))
				{
					component2.text = (nickName + "\n\n" + Utils.GetPower(maxHp) + "\n" + Utils.GetTorque(maxTq) + " " + Utils.GetTorquePostfix).ToUpper();
				}
			}
			VehicleData.CarMirrorController.SetMirror(val: false);
			SpeakingIcon.SetActive(value: false);
			speedLimitIcon.SetActive(value: false);
		}
		if (hideLabel && (bool)label)
		{
			label.gameObject.SetActive(value: false);
		}
		else if ((bool)label && Person.activeInHierarchy)
		{
			if ((bool)SmallRankIcon)
			{
				SmallRankIcon.SetActive(value: false);
			}
			if ((bool)BigRankIcon)
			{
				BigRankIcon.SetActive(value: true);
			}
			label.enabled = false;
			label.gameObject.SetActive(value: true);
		}
	}

	private void ShowHideTopLabel(float d)
	{
		if (!label)
		{
			return;
		}
		if (!VehicleData.Driver)
		{
			label.gameObject.SetActive(value: false);
		}
		else if (FlagsStorage.HideAllTopLabel)
		{
			label.gameObject.SetActive(value: false);
		}
		else if (d < 12f)
		{
			if ((bool)SmallRankIcon)
			{
				SmallRankIcon.SetActive(value: true);
			}
			if ((bool)BigRankIcon)
			{
				BigRankIcon.SetActive(value: false);
			}
			label.enabled = true;
			label.gameObject.SetActive(value: true);
		}
		else if (d > 12f && d < 35f && (int?)_rankCurrentPlayer > 4)
		{
			if ((bool)SmallRankIcon)
			{
				SmallRankIcon.SetActive(value: false);
			}
			if ((bool)BigRankIcon)
			{
				BigRankIcon.SetActive(value: true);
			}
			label.enabled = false;
			label.gameObject.SetActive(value: true);
		}
		else
		{
			label.gameObject.SetActive(value: false);
		}
	}

	private static bool IsDragMessageForUs(string b)
	{
		if (string.IsNullOrEmpty(b))
		{
			return true;
		}
		if (!int.TryParse(b, out var result))
		{
			return false;
		}
		if (PhotonNetwork.LocalPlayer != null)
		{
			return result == PhotonNetwork.LocalPlayer.ActorNumber;
		}
		return false;
	}

	private void ApplyPendingCarData()
	{
		if (FreeDriveProvider == null || !FreeDriveProvider.MultiPlayer || FlagsStorage.IsMainMenu || (FreeDriveProvider.FirstCarMultiPlayer == null && !_gameFactManager.IsTrue(GameFact.Carless)))
		{
			return;
		}
		MultiPlayer.PendingCarData pendingCarData = FreeDriveProvider.MultiPlayer.TakePendingCarData(base.photonView.ViewID);
		if (pendingCarData == null)
		{
			return;
		}
		if (pendingCarData.HasRawPosition)
		{
			PredictNextRawPosition(pendingCarData.RawPosition, pendingCarData.RawYRotation);
		}
		foreach (var (a, b, text) in pendingCarData.Rpc)
		{
			HandleValueStr(a, b, text);
		}
	}

	[PunRPC]
	private void SendValueStr(byte a, string b, string name)
	{
		if (!FlagsStorage.IsMainMenu && (!(FreeDriveProvider.FirstCarMultiPlayer == null) || _gameFactManager.IsTrue(GameFact.Carless)) && !FreeDriveProvider.MultiPlayer.TryStorePendingCarRpc(base.photonView.ViewID, a, b, name))
		{
			HandleValueStr(a, b, name);
		}
	}

	private void HandleValueStr(byte a, string b, string name)
	{
		switch (a)
		{
		case 8:
			if ((bool)V.Car && !FlagsStorage.IsPassenger)
			{
				int actorNumber4 = V.Car.GetComponent<PhotonView>().Owner.ActorNumber;
				if (int.Parse(b) == actorNumber4)
				{
					V.CoreGameManager.SetMessage(new LocalizedString("MultiPlayerPosControl.InvitationRejected"), 4f);
					MonoBehaviour.print("make car 2 null");
					FreeDriveProvider.RacingCar2 = null;
					FreeDriveProvider.FreeDriveControl.ResetInviteButton();
					FreeDriveProvider.FreeDriveControl.ClearInvitationId();
				}
			}
			break;
		case 9:
			if ((bool)V.Car && !FlagsStorage.IsPassenger)
			{
				int actorNumber11 = V.Car.GetComponent<PhotonView>().Owner.ActorNumber;
				if (int.Parse(b) == actorNumber11)
				{
					FreeDriveProvider.RacingCar2 = base.gameObject;
					FreeDriveProvider.FreeDriveControl.InvitationAccepted();
				}
			}
			break;
		case 10:
			if ((bool)V.Car && !FlagsStorage.IsPassenger)
			{
				int actorNumber14 = V.Car.GetComponent<PhotonView>().Owner.ActorNumber;
				if (int.Parse(b) == actorNumber14)
				{
					distanceMultiRace = 252f;
				}
			}
			break;
		case 11:
			if ((bool)V.Car && !FlagsStorage.IsPassenger)
			{
				int actorNumber10 = V.Car.GetComponent<PhotonView>().Owner.ActorNumber;
				if (int.Parse(b) == actorNumber10)
				{
					V.CoreGameManager.SetMessage(new LocalizedString("MultiPlayerPosControl.PlayerIsBusy"), 5f);
					MonoBehaviour.print("make car 2 null");
					FreeDriveProvider.RacingCar2 = null;
					FreeDriveProvider.FreeDriveControl.ResetInviteButton();
				}
			}
			break;
		case 12:
			if ((bool)V.Car && !FlagsStorage.IsPassenger && IsDragMessageForUs(b))
			{
				FreeDriveProvider.DragRacingControlMultiplayer.EnableGuestDistance(int.Parse(name));
			}
			break;
		case 13:
			if ((bool)V.Car && !FlagsStorage.IsPassenger)
			{
				int actorNumber = V.Car.GetComponent<PhotonView>().Owner.ActorNumber;
				if (int.Parse(b) == actorNumber)
				{
					FreeDriveProvider.DragRacingControlMultiplayer.Reset();
					FreeDriveProvider.FreeDriveControl.ShowNegotiationWindow();
					FreeDriveProvider.RacingCar2 = base.gameObject;
					FreeDriveProvider.FreeDriveControl.InvitationHost = true;
				}
			}
			break;
		case 14:
		{
			if (!V.Car || FlagsStorage.IsPassenger)
			{
				break;
			}
			int actorNumber7 = V.Car.GetComponent<PhotonView>().Owner.ActorNumber;
			if (int.Parse(b) == actorNumber7)
			{
				FreeDriveProvider.FreeDriveControl.ShowMainDrivingUI();
				MonoBehaviour.print("make car 2 null");
				FreeDriveProvider.RacingCar2 = null;
				FreeDriveProvider.FreeDriveControl.inviteToRaceButton.SetActive(active: true);
				if ((bool)FreeDriveProvider.FreeDriveControl.newNegotiationWindow)
				{
					FreeDriveProvider.FreeDriveControl.newNegotiationWindow.Close();
				}
			}
			break;
		}
		case 16:
			if ((bool)V.Car)
			{
				int actorNumber12 = V.Car.GetComponent<PhotonView>().Owner.ActorNumber;
				if (int.Parse(b) == actorNumber12)
				{
					FlagsStorage.IsReadyMultRaceCar2 = true;
				}
			}
			break;
		case 17:
			if ((bool)FreeDriveProvider.RacingCar2)
			{
				_dragControlMulti.PlayerNameMultiCar2 = FreeDriveProvider.RacingCar2.GetComponent<VehicleData>().Driver.GetComponent<PhotonView>().Owner.NickName;
			}
			SeparateResult(name);
			break;
		case 18:
			if ((bool)V.Car && !FlagsStorage.IsPassenger)
			{
				_ = V.Car.GetComponent<PhotonView>().Owner.ActorNumber;
				FreeDriveProvider.DragRacingControlMultiplayer.ReplayReceived(int.Parse(b));
			}
			break;
		case 19:
			if ((bool)V.Car && !FlagsStorage.IsPassenger)
			{
				int actorNumber13 = V.Car.GetComponent<PhotonView>().Owner.ActorNumber;
				if (int.Parse(b) == actorNumber13)
				{
					FreeDriveProvider.DragRacingControlMultiplayer.ReplayReceived(3);
				}
			}
			break;
		case 22:
		{
			int sound = ((!int.TryParse(name, out sound)) ? 1 : 0);
			V.CoreGameManager.GetComponent<MultiPlayer>().LockCar(int.Parse(b), sound);
			break;
		}
		case 23:
		{
			int sound2 = ((!int.TryParse(name, out sound2)) ? 1 : 0);
			V.CoreGameManager.GetComponent<MultiPlayer>().UnlockCar(int.Parse(b), sound2);
			break;
		}
		case 26:
		{
			if (FreeDriveProvider.MultiPlayer.ignoreFriendRequest)
			{
				break;
			}
			int actorNumber5 = V.DriverPerson.GetComponent<PhotonView>().Owner.ActorNumber;
			if (int.Parse(b) == actorNumber5)
			{
				string[] array2 = name.Split("?"[0]);
				string playerId = array2[0];
				TradeOfferState tradeOfferState = (TradeOfferState)int.Parse(array2[1]);
				string offerId = array2[2];
				switch (tradeOfferState)
				{
				case TradeOfferState.Enabled:
					FreeDriveProvider.AddFriendService.RequestToFriendReceived(playerId, offerId);
					break;
				case TradeOfferState.Cancelled:
					FreeDriveProvider.AddFriendService.OfferCanceled(offerId);
					break;
				case TradeOfferState.Pending:
					FreeDriveProvider.AddFriendService.OfferPending(offerId);
					break;
				case TradeOfferState.Completed:
					FreeDriveProvider.AddFriendService.OnOfferCompleted(offerId);
					break;
				}
			}
			break;
		}
		case 27:
		{
			int actorNumber2 = V.DriverPerson.GetComponent<PhotonView>().Owner.ActorNumber;
			if (int.Parse(b) == actorNumber2)
			{
				FreeDriveProvider.AddFriendService.OfferConfirmed(name);
			}
			break;
		}
		case 28:
		{
			int actorNumber15 = V.DriverPerson.GetComponent<PhotonView>().Owner.ActorNumber;
			if (int.Parse(b) == actorNumber15)
			{
				FreeDriveProvider.AddFriendService.OfferDeclined(name);
			}
			break;
		}
		case 34:
		{
			int actorNumber8 = V.DriverPerson.GetComponent<PhotonView>().Owner.ActorNumber;
			if (int.Parse(b) == actorNumber8)
			{
				int num6 = 0;
				num6 = ((FreeDriveProvider.RacingCar2 != null) ? 1 : (FreeDriveProvider.FreeDriveControl.JustPenalized ? 2 : (((int)CurrencyManagerController.Money < 2000) ? 3 : 0)));
				PhotonView component2 = OnlineCollectionsProvider.GetPlayerByPlayerID(name).GetComponent<PhotonView>();
				int actorNumber9 = component2.Owner.ActorNumber;
				if (num6 == 0)
				{
					string nickName = component2.Owner.NickName;
					V.CoreGameManager.SetMessage(string.Format(new LocalizedString("MultiPlayerPosControl.PoliceFinedYou"), nickName), 4f);
					AdvancedInt price = new AdvancedInt(2000);
					_currencyManagerController.TryMoneySpend(out var _, price);
					FreeDriveProvider.FreeDriveControl.PenalizedReset();
					V.Car.GetComponent<VehicleData>().StopCarCompletely(0.017f);
				}
				string text2 = num6 + "?" + _playerDataStorage.PlayerName;
				FreeDriveProvider.MultiPlayer.SendValueStrToPlayer(35, actorNumber9.ToString(), text2, component2.Owner);
			}
			break;
		}
		case 35:
		{
			int actorNumber6 = V.DriverPerson.GetComponent<PhotonView>().Owner.ActorNumber;
			if (int.Parse(b) == actorNumber6)
			{
				string[] array3 = name.Split("?"[0]);
				int num4 = int.Parse(array3[0]);
				string text = array3[1];
				switch (num4)
				{
				case 0:
				{
					V.CoreGameManager.SetMessage("You fined '" + text + "' for speeding!\n+2000", 3f);
					ObscuredInt value2 = 2000;
					_currencyManagerController.AddMoney(new AdvancedInt(value2));
					int num5 = ObscuredPrefs.GetInt("FineCount");
					ObscuredPrefs.SetInt("FineCount", num5 + 1);
					++FreeDriveProvider.FreeDriveDB.police;
					break;
				}
				case 1:
					V.CoreGameManager.SetMessage("'" + text + "' is  racing!\n", 3f);
					break;
				case 2:
					V.CoreGameManager.SetMessage("'" + text + "' was already fined!\n", 3f);
					break;
				case 3:
					V.CoreGameManager.SetMessage("'" + text + "'  has no money!\n", 3f);
					break;
				}
			}
			break;
		}
		case 36:
		{
			int viewID4 = base.gameObject.GetComponent<PhotonView>().ViewID;
			if (int.Parse(b) == viewID4)
			{
				bool value = int.Parse(name) == 1;
				if (!_powertrain.CheckEngineStatus)
				{
					_powertrain.SetCheckEngine(value, sendRpc: false);
				}
			}
			break;
		}
		case 40:
			if (VehicleData.CarId == 101)
			{
				int viewID = base.gameObject.GetComponent<PhotonView>().ViewID;
				if (int.Parse(b) == viewID)
				{
					bool flag = int.Parse(name) == 1;
					VehicleData.SpacerOpen = flag;
					VehicleData.spacerMove1.ExtendSpacer(flag);
					VehicleData.spacerMove2.ExtendSpacer(flag);
				}
				break;
			}
			goto default;
		default:
			switch (a)
			{
			case 41:
			{
				int viewID3 = base.gameObject.GetComponent<PhotonView>().ViewID;
				if (int.Parse(b) == viewID3)
				{
					if (name.Length == 2)
					{
						string s = name[0].ToString();
						string s2 = name[1].ToString();
						SetHeadAndIndicator(int.Parse(s), int.Parse(s2), forceSet: false);
					}
					else
					{
						Debug.LogWarning("Length data not correct: " + name);
					}
				}
				break;
			}
			case 44:
				if ((bool)V.Car && (bool)FreeDriveProvider.RacingCar2)
				{
					bool raceStarted = FlagsStorage.RaceStarted;
					FreeDriveProvider.RacingCar2 = null;
					V.CoreGameManager.SetMessage(new LocalizedString("MultiPlayerPosControl.PlayerCanceledRace"), 3f);
					NewRacingMultiplayerController newRacingMultiplayerController = FreeDriveProvider.DragRacingControlMultiplayer.newRacingMultiplayerController;
					if (raceStarted)
					{
						newRacingMultiplayerController.OtherCarCancelledRaceDuringRun();
					}
					else
					{
						newRacingMultiplayerController.OtherCarCancelledRace();
					}
				}
				break;
			case 49:
				if ((ObscuredString)b == _playerDataStorage.PlayerID)
				{
					FreeDriveProvider.MultiPlayer.OnReport(name);
				}
				break;
			case 50:
				if ((ObscuredString)b == _playerDataStorage.PlayerID)
				{
					FreeDriveProvider.MultiPlayer.OnRemoveFromRoom();
				}
				break;
			case 51:
				V.CoreGameManager.SetMessage("Player '" + b + "' will be removed from the room.", 5f);
				break;
			case 53:
			{
				int viewID2 = base.gameObject.GetComponent<PhotonView>().ViewID;
				if (int.Parse(b) == viewID2 && base.gameObject.TryGetComponentInChildren<FsoStrobeLights>(out var component, includeInactive: true))
				{
					component.ReceiveRpcForFsoToggle(int.Parse(name));
				}
				break;
			}
			case 54:
				if ((bool)V.Car && !FlagsStorage.IsPassenger)
				{
					int actorNumber3 = V.Car.GetComponent<PhotonView>().Owner.ActorNumber;
					if (int.Parse(b) == actorNumber3)
					{
						FreeDriveProvider.DragRacingControlMultiplayer.StartRaceByHostDistance(int.Parse(name));
					}
				}
				break;
			case 58:
				FreeDriveProvider.MultiPlayer.KickedFromCar();
				break;
			case 59:
			{
				RagdollCPM personToRagdoll2 = GetPersonToRagdoll(int.Parse(b));
				if ((bool)personToRagdoll2)
				{
					MonoBehaviour.print(name);
					string[] array = name.Split("?"[0]);
					int num = int.Parse(array[0]);
					int num2 = int.Parse(array[1]);
					int num3 = int.Parse(array[2]);
					Vector3 origin = new Vector3(int.Parse(array[3]), int.Parse(array[4]), int.Parse(array[5]));
					origin *= 0.1f;
					Vector3 force = new Vector3(num, num2, num3);
					if (!personToRagdoll2.isRagdoll)
					{
						personToRagdoll2.RagdollEnabler(force, origin);
					}
				}
				break;
			}
			case 60:
			{
				RagdollCPM personToRagdoll = GetPersonToRagdoll(int.Parse(b));
				if ((bool)personToRagdoll)
				{
					personToRagdoll.DisableRagdoll();
				}
				break;
			}
			default:
				_rpcCollection.GetRpcObject(a)?.Execute(this, b, name);
				break;
			}
			break;
		case 5:
			break;
		}
	}

	private RagdollCPM GetPersonToRagdoll(int id)
	{
		GameObject gameObject = null;
		MultiPersonControl[] playersOnlineNow = V.CoreGameManager._onlineCollectionsProvider.PlayersOnlineNow;
		foreach (MultiPersonControl multiPersonControl in playersOnlineNow)
		{
			if (multiPersonControl == null)
			{
				continue;
			}
			MonoBehaviour.print(multiPersonControl.GetComponent<PhotonView>().ViewID + " " + id);
			if (multiPersonControl.GetComponent<PhotonView>().ViewID == id)
			{
				RagdollCPM ragdollCpm = multiPersonControl.GetComponent<DriverActionController>().ragdollCpm;
				if (ragdollCpm == null)
				{
					return null;
				}
				if (multiPersonControl.DistanceToDriverPerson > 200f && !ragdollCpm.isRagdoll)
				{
					return null;
				}
				if (multiPersonControl.GetComponent<PhotonView>().ViewID == id)
				{
					gameObject = multiPersonControl.gameObject;
				}
			}
			if (gameObject == null && V.DriverPerson.GetComponent<PhotonView>().ViewID == id)
			{
				gameObject = V.DriverPerson;
			}
		}
		if (gameObject == null)
		{
			return null;
		}
		return gameObject.GetComponent<DriverActionController>().ragdollCpm;
	}

	private void SeparateResult(string toSeparate)
	{
		string[] array = toSeparate.Split("?"[0]);
		for (int i = 0; i < array.Length; i++)
		{
			switch (i)
			{
			case 0:
			{
				string s6 = array[i].Replace(",", ".");
				_dragControlMulti.TimeMultiRaceCarFinish = float.Parse(s6, CultureInfo.InvariantCulture);
				break;
			}
			case 1:
			{
				string s5 = array[i].Replace(",", ".");
				_dragControlMulti.MultiRaceCar100Km = float.Parse(s5, CultureInfo.InvariantCulture);
				break;
			}
			case 2:
			{
				string s4 = array[i].Replace(",", ".");
				_dragControlMulti.MultiRaceCar200Km = float.Parse(s4, CultureInfo.InvariantCulture);
				break;
			}
			case 3:
			{
				string s3 = array[i].Replace(",", ".");
				_dragControlMulti.MultiRaceCar500Km = float.Parse(s3, CultureInfo.InvariantCulture);
				break;
			}
			case 4:
			{
				string s2 = array[i].Replace(",", ".");
				_dragControlMulti.MultiRaceCarMaxSpeed = float.Parse(s2, CultureInfo.InvariantCulture);
				break;
			}
			case 5:
			{
				string s = array[i].Replace(",", ".");
				_dragControlMulti.MultiRaceCar200Km = float.Parse(s, CultureInfo.InvariantCulture);
				break;
			}
			case 6:
				_dragControlMulti.MultiRaceWon = float.Parse(array[i], CultureInfo.InvariantCulture);
				break;
			case 7:
				_dragControlMulti.MultiRaceOverallRace = float.Parse(array[i], CultureInfo.InvariantCulture);
				break;
			}
		}
	}

	public static void MessageNewCar2()
	{
		V.CoreGameManager.SetMessage(new LocalizedString("MultiPlayerPosControl.CongratulationsYouOwnANewCar"), 5f);
	}

	public int GetVinylsCount(VinylsType type, bool isFrontPlate)
	{
		object[] vinylsInstantiationDataByType = GetVinylsInstantiationDataByType(type, isFrontPlate);
		if (vinylsInstantiationDataByType == null)
		{
			return 0;
		}
		return vinylsInstantiationDataByType.Length;
	}

	public void RenderVinyls(VinylsType type, bool isFrontPlate)
	{
		RenderVinyls(type, GetVinylsInstantiationDataByType(type, isFrontPlate), isFrontPlate).Forget();
		CarInfoContainer component = GetComponent<CarInfoContainer>();
		switch (type)
		{
		case VinylsType.CarVinyls:
			CarVinyls.CarBodyVinyls = null;
			if ((bool)component)
			{
				component.VinylsData.CarBodyVinyls = null;
			}
			break;
		case VinylsType.PlateVinyls:
			if (isFrontPlate)
			{
				CarVinyls.FrontPlateVinyls = null;
				if ((bool)component)
				{
					component.VinylsData.FrontPlateVinyls = null;
				}
			}
			else
			{
				CarVinyls.RearPlateVinyls = null;
				if ((bool)component)
				{
					component.VinylsData.RearPlateVinyls = null;
				}
			}
			break;
		case VinylsType.WindowVinyls:
			CarVinyls.WindowVinyls = null;
			if ((bool)component)
			{
				component.VinylsData.WindowVinyls = null;
			}
			break;
		case VinylsType.FlagVinyls:
			break;
		}
	}

	private object[] GetVinylsInstantiationDataByType(VinylsType type, bool isFrontPlate)
	{
		return type switch
		{
			VinylsType.CarVinyls => CarVinyls.CarBodyVinyls, 
			VinylsType.PlateVinyls => isFrontPlate ? CarVinyls.FrontPlateVinyls : CarVinyls.RearPlateVinyls, 
			VinylsType.WindowVinyls => CarVinyls.WindowVinyls, 
		};
	}

	public void TeleportToOwner()
	{
		if (!(base.gameObject != FreeDriveProvider.FirstCarMultiPlayer) && (bool)V.DriverPerson)
		{
			FreeDriveProvider.FreeDriveControl?.CarToRoad(base.gameObject, ToRoadSource.TeleportToOwner);
			ForceSendCarPosition().Forget();
		}
	}

	private async UniTaskVoid ForceSendCarPosition()
	{
		if (FlagsStorage.IsMultiFree && PhotonNetwork.IsConnected && PhotonNetwork.CurrentRoom != null && (bool)FreeDriveProvider.MultiPlayer && !(base.gameObject != FreeDriveProvider.FirstCarMultiPlayer))
		{
			while (!base.photonView.IsMine)
			{
				await UniTask.Delay(ForceSendCarPositionDelay, ignoreTimeScale: false, PlayerLoopTiming.Update, base.destroyCancellationToken);
			}
			await UniTask.Delay(ForceSendCarPositionDelay, ignoreTimeScale: false, PlayerLoopTiming.Update, base.destroyCancellationToken);
			FreeDriveProvider.MultiPlayer.RaiseAllControlledCarsRawPosition();
			ForceSendNextStream();
		}
	}
}

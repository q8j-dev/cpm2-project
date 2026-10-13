using System;
using System.Collections.Generic;
using System.Linq;
using CarInteractionAnimationsScripts;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;
using _Car_Parking;
using _Car_Parking.Scripts.Audio.SoundBanks;
using _Car_Parking.Scripts.Car;
using _Car_Parking.Scripts.Car.car_animations_scripts;
using _Car_Parking.Scripts.Drag;

namespace CarAnimationScripts
{
	public class CarAnimationReferences : MonoBehaviour
	{
		private const string Front = "Front";

		[SerializeField]
		private bool IsRightHanded;

		private bool _isTrunkAnimateInitial;

		private bool _isHoodAnimateInitial;

		private Animator _defaultRightSkirtsAnimator;

		private Animator _defaultLeftSkirtsAnimator;

		private Animator _defaultRightBackSkirtsAnimator;

		private Animator _defaultLeftBackSkirtsAnimator;

		private bool _defaultEveryDoor;

		public List<CarPartAnimationSequence> TrunkAnimationsOpen;

		public List<CarPartAnimationSequence> TrunkAnimationsClose;

		public List<CarPartAnimationSequence> RoofAnimationsOpen;

		public List<CarPartAnimationSequence> RoofAnimationsClose;

		[SerializeField]
		private float DelayOpenFrontDoorOutside;

		[SerializeField]
		private float DelayCloseFrontDoorInsideSeatIntoCar;

		[SerializeField]
		private float DelayCloseFrontDoorInside;

		[SerializeField]
		private float DelayOpenFrontDoorInside;

		[SerializeField]
		private float DelayCloseFrontDoorOutsideGetOut;

		[SerializeField]
		private float DelayCloseFrontDoorOutside;

		[SerializeField]
		private float DelayOpenBackDoorOutside;

		[SerializeField]
		private float DelayCloseBackDoorInsideSeatIntoCar;

		[SerializeField]
		private float DelayCloseBackDoorInside;

		[SerializeField]
		private float DelayOpenBackDoorInside;

		[SerializeField]
		private float DelayCloseBackDoorOutsideGetOut;

		[SerializeField]
		private float DelayCloseBackDoorOutside;

		private float _delayOpenFrontDoorOutsideInitial;

		private float _delayCloseFrontDoorInsideSeatIntoCarInitial;

		private float _delayCloseFrontDoorInsideInitial;

		private float _delayOpenFrontDoorInsideInitial;

		private float _delayCloseFrontDoorOutsideGetOutInitial;

		private float _delayCloseFrontDoorOutsideInitial;

		private float _delayOpenBackDoorOutsideInitial;

		private float _delayCloseBackDoorInsideSeatIntoCarInitial;

		private float _delayCloseBackDoorInsideInitial;

		private float _delayOpenBackDoorInsideInitial;

		private float _delayCloseBackDoorOutsideGetOutInitial;

		private float _delayCloseBackDoorOutsideInitial;

		[Range(0f, 2f)]
		public float OpenFrontDoorOutsideAnimationSpeed = 1f;

		[Range(0f, 2f)]
		public float CloseFrontDoorInsideSeatIntoCarAnimationSpeed = 1f;

		[Range(0f, 2f)]
		public float CloseFrontDoorInsideAnimationSpeed = 1f;

		[Range(0f, 2f)]
		public float OpenFrontDoorInsideAnimationSpeed = 1f;

		[Range(0f, 2f)]
		public float CloseFrontDoorOutsideGetOutAnimationSpeed = 1f;

		[Range(0f, 2f)]
		public float CloseFrontDoorOutsideAnimationSpeed = 1f;

		[Range(0f, 2f)]
		public float OpenBackDoorOutsideAnimationSpeed = 1f;

		[Range(0f, 2f)]
		public float CloseBackDoorInsideSeatIntoCarAnimationSpeed = 1f;

		[Range(0f, 2f)]
		public float CloseBackDoorInsideAnimationSpeed = 1f;

		[Range(0f, 2f)]
		public float OpenBackDoorInsideAnimationSpeed = 1f;

		[Range(0f, 2f)]
		public float CloseBackDoorOutsideGetOutAnimationSpeed = 1f;

		[Range(0f, 2f)]
		public float CloseBackDoorOutsideAnimationSpeed = 1f;

		public Vector3 SizePlayerInCar = new Vector3(1.1f, 1.1f, 1.2f);

		public CarAnimationsPoints CarAnimationsPoints;

		private IDisposable _setOldInteriorDisposable;

		private IDisposable _bodyKitTimerSub;

		private VehicleData _vehicleData;

		private bool _roofBanksHeld;

		private CarAnimationsController _carAnimationController;

		private List<GameObject> _oldInterior;

		private List<GameObject> _newInterior;

		private GameObject _newInteriorMain;

		private List<GameObject> BodyKitsToIgnoreOpenTrunkObjects = new List<GameObject>();

		private List<GameObject> BodyKitsToIgnoreOpenHoodObjects = new List<GameObject>();

		private Transform _initialPlateHoodParent;

		private Transform _initialPlateTrunkParent;

		public CarBodyReferences CarBodyReferences { get; private set; }

		public bool RightHanded => IsRightHanded;

		[field: SerializeField]
		public bool IsF1Car { get; private set; }

		[field: SerializeField]
		public bool IsTrunkAnimate { get; private set; } = true;

		[field: SerializeField]
		public Transform TrunkParent { get; private set; }

		[field: SerializeField]
		public bool IsHoodAnimate { get; private set; } = true;

		[field: SerializeField]
		public bool IsPlateInTrunk { get; private set; }

		[field: SerializeField]
		public Transform PlateTrunkParent { get; private set; }

		[field: SerializeField]
		public bool IsPlateInHood { get; private set; }

		[field: SerializeField]
		public Transform PlateHoodParent { get; private set; }

		[field: SerializeField]
		public bool IsOpenCloseSideDoor { get; private set; }

		[field: SerializeField]
		public bool IsOpenCloseRoof { get; private set; }

		[field: SerializeField]
		[field: FormerlySerializedAs("_countSeats")]
		public int CountSeats { get; private set; }

		[field: SerializeField]
		public Animator RightSkirtsAnimator { get; private set; }

		[field: SerializeField]
		public Animator LeftSkirtsAnimator { get; private set; }

		[field: SerializeField]
		public bool EveryDoor { get; private set; }

		[field: SerializeField]
		public Animator RightBackSkirtsAnimator { get; private set; }

		[field: SerializeField]
		public Animator LeftBackSkirtsAnimator { get; private set; }

		public Dictionary<CarDoorType, Animator> CarDoorAnimators { get; private set; }

		public GameObject[] CurrentAnimateSeats { get; private set; }

		public bool IsAnimatingSeats => CurrentAnimateSeats.Any((GameObject value) => value != null);

		[field: SerializeField]
		public List<string> BodyKitsToIgnoreOpenHoodNames { get; private set; } = new List<string>();

		[field: SerializeField]
		public List<string> BodyKitsToIgnoreOpenTrunkNames { get; private set; } = new List<string>();

		[field: SerializeField]
		public List<string> BodyKitsToIgnoreForMoveKit { get; private set; } = new List<string>();

		[field: SerializeField]
		public List<string> BodyKitsToSetAirIntakeKit { get; private set; } = new List<string>();

		[field: SerializeField]
		public List<string> BodyKitsToSetSpoilerKit { get; private set; } = new List<string>();

		public CarWithoutDoorsBodyKit CarWithoutDoorsBodyKit { get; set; }

		public event Action OnEnabled;

		public void SaveInitialValues()
		{
			_delayOpenFrontDoorOutsideInitial = DelayOpenFrontDoorOutside;
			_delayCloseFrontDoorInsideSeatIntoCarInitial = DelayCloseFrontDoorInsideSeatIntoCar;
			_delayCloseFrontDoorInsideInitial = DelayCloseFrontDoorInside;
			_delayOpenFrontDoorInsideInitial = DelayOpenFrontDoorInside;
			_delayCloseFrontDoorOutsideGetOutInitial = DelayCloseFrontDoorOutsideGetOut;
			_delayCloseFrontDoorOutsideInitial = DelayCloseFrontDoorOutside;
			_delayOpenBackDoorOutsideInitial = DelayOpenBackDoorOutside;
			_delayCloseBackDoorInsideSeatIntoCarInitial = DelayCloseBackDoorInsideSeatIntoCar;
			_delayCloseBackDoorInsideInitial = DelayCloseBackDoorInside;
			_delayOpenBackDoorInsideInitial = DelayOpenBackDoorInside;
			_delayCloseBackDoorOutsideGetOutInitial = DelayCloseBackDoorOutsideGetOut;
			_delayCloseBackDoorOutsideInitial = DelayCloseBackDoorOutside;
			Debug.Log("Initial values saved.");
		}

		public void ApplySpeedMultipliers()
		{
			DelayOpenFrontDoorOutside = _delayOpenFrontDoorOutsideInitial / OpenFrontDoorOutsideAnimationSpeed;
			DelayCloseFrontDoorInsideSeatIntoCar = _delayCloseFrontDoorInsideSeatIntoCarInitial / CloseFrontDoorInsideSeatIntoCarAnimationSpeed;
			DelayCloseFrontDoorInside = _delayCloseFrontDoorInsideInitial / CloseFrontDoorInsideAnimationSpeed;
			DelayOpenFrontDoorInside = _delayOpenFrontDoorInsideInitial / OpenFrontDoorInsideAnimationSpeed;
			DelayCloseFrontDoorOutsideGetOut = _delayCloseFrontDoorOutsideGetOutInitial / CloseFrontDoorOutsideGetOutAnimationSpeed;
			DelayCloseFrontDoorOutside = _delayCloseFrontDoorOutsideInitial / CloseFrontDoorOutsideAnimationSpeed;
			DelayOpenBackDoorOutside = _delayOpenBackDoorOutsideInitial / OpenBackDoorOutsideAnimationSpeed;
			DelayCloseBackDoorInsideSeatIntoCar = _delayCloseBackDoorInsideSeatIntoCarInitial / CloseBackDoorInsideSeatIntoCarAnimationSpeed;
			DelayCloseBackDoorInside = _delayCloseBackDoorInsideInitial / CloseBackDoorInsideAnimationSpeed;
			DelayOpenBackDoorInside = _delayOpenBackDoorInsideInitial / OpenBackDoorInsideAnimationSpeed;
			DelayCloseBackDoorOutsideGetOut = _delayCloseBackDoorOutsideGetOutInitial / CloseBackDoorOutsideGetOutAnimationSpeed;
			DelayCloseBackDoorOutside = _delayCloseBackDoorOutsideInitial / CloseBackDoorOutsideAnimationSpeed;
		}

		private void Awake()
		{
			_isTrunkAnimateInitial = IsTrunkAnimate;
			_isHoodAnimateInitial = IsHoodAnimate;
			_defaultLeftSkirtsAnimator = LeftSkirtsAnimator;
			_defaultRightSkirtsAnimator = RightSkirtsAnimator;
			_defaultLeftBackSkirtsAnimator = LeftBackSkirtsAnimator;
			_defaultRightBackSkirtsAnimator = RightBackSkirtsAnimator;
			_defaultEveryDoor = EveryDoor;
			CarBodyReferences = GetComponent<CarBodyReferences>();
			InitAnimators();
			if ((bool)CarAnimationsPoints)
			{
				CarAnimationsPoints.InitPoints(IsRightHanded);
			}
			CurrentAnimateSeats = new GameObject[CountSeats];
			_bodyKitTimerSub = Observable.Timer(TimeSpan.FromSeconds(0.0), TimeSpan.FromSeconds(2.0)).Subscribe((long _) =>
			{
				CheckBodyKitsToActiveButtons();
			}).AddTo(this);
		}

		public void CheckBodyKitsToActiveButtons()
		{
			if (BodyKitsToIgnoreOpenTrunkObjects.Count > 0 && BodyKitsToIgnoreOpenTrunkObjects.All((GameObject value) => !value))
			{
				IsTrunkAnimate = _isTrunkAnimateInitial;
				BodyKitsToIgnoreOpenTrunkObjects.Clear();
			}
			if (BodyKitsToIgnoreOpenHoodObjects.Count > 0 && BodyKitsToIgnoreOpenHoodObjects.All((GameObject value) => !value))
			{
				IsHoodAnimate = _isHoodAnimateInitial;
				BodyKitsToIgnoreOpenHoodObjects.Clear();
			}
		}

		private void IgnoreBodyKit(GameObject bodyKit)
		{
			string text = bodyKit.name.Replace("(Clone)", "").Trim();
			foreach (string bodyKitsToIgnoreOpenHoodName in BodyKitsToIgnoreOpenHoodNames)
			{
				if (text == bodyKitsToIgnoreOpenHoodName)
				{
					IsHoodAnimate = false;
					BodyKitsToIgnoreOpenHoodObjects.Add(bodyKit);
					break;
				}
			}
			foreach (string bodyKitsToIgnoreOpenTrunkName in BodyKitsToIgnoreOpenTrunkNames)
			{
				if (text == bodyKitsToIgnoreOpenTrunkName)
				{
					IsTrunkAnimate = false;
					BodyKitsToIgnoreOpenTrunkObjects.Add(bodyKit);
					break;
				}
			}
		}

		private void InitAnimators()
		{
			Animator animator = (CarBodyReferences.FrontLeftDoor ? CarBodyReferences.FrontLeftDoor.GetComponent<Animator>() : null);
			Animator animator2 = (CarBodyReferences.FrontRightDoor ? CarBodyReferences.FrontRightDoor.GetComponent<Animator>() : null);
			Animator animator3 = (CarBodyReferences.BackLeftDoor ? CarBodyReferences.BackLeftDoor.GetComponent<Animator>() : null);
			Animator animator4 = (CarBodyReferences.BackRightDoor ? CarBodyReferences.BackRightDoor.GetComponent<Animator>() : null);
			Animator value = (CarBodyReferences.Trunk ? CarBodyReferences.Trunk.GetComponent<Animator>() : null);
			Animator value2 = (CarBodyReferences.Hood ? CarBodyReferences.Hood.GetComponent<Animator>() : null);
			Animator value3 = (CarBodyReferences.Roof ? CarBodyReferences.Roof.GetComponent<Animator>() : null);
			Animator value4 = (CarBodyReferences.CarFrontLeftDoorWindow ? CarBodyReferences.CarFrontLeftDoorWindow.GetComponent<Animator>() : null);
			Animator value5 = (CarBodyReferences.CarFrontRightDoorWindow ? CarBodyReferences.CarFrontRightDoorWindow.GetComponent<Animator>() : null);
			Animator value6 = (CarBodyReferences.RoofPlatformHondaDelSol ? CarBodyReferences.RoofPlatformHondaDelSol.GetComponent<Animator>() : null);
			CarDoorAnimators = new Dictionary<CarDoorType, Animator>
			{
				{
					CarDoorType.DriverDoor,
					IsRightHanded ? animator2 : animator
				},
				{
					CarDoorType.PassengerFrontDoor,
					IsRightHanded ? animator : animator2
				},
				{
					CarDoorType.PassengerBackDoor1,
					IsRightHanded ? animator4 : animator3
				},
				{
					CarDoorType.PassengerBackDoor2,
					IsRightHanded ? animator3 : animator4
				},
				{
					CarDoorType.Hood,
					value2
				},
				{
					CarDoorType.Trunk,
					value
				},
				{
					CarDoorType.Roof,
					value3
				},
				{
					CarDoorType.FrontLeftDoorWindow,
					value4
				},
				{
					CarDoorType.FrontRightDoorWindow,
					value5
				},
				{
					CarDoorType.RoofPlatformHondaDelSol,
					value6
				}
			};
		}

		private void Start()
		{
			bool isMultiFree = FlagsStorage.IsMultiFree;
			bool flag = (bool)base.transform.parent && base.transform.parent.gameObject.TryGetComponent<CarAnimationsController>(out _carAnimationController);
			bool flag2 = (bool)_carAnimationController && (bool)_carAnimationController.PhotonViewCar && _carAnimationController.PhotonViewCar.IsMine;
			CheckRoof(_carAnimationController, isMultiFree, flag2).Forget();
			_vehicleData = (_carAnimationController ? _carAnimationController.VehicleData : null);
			_vehicleData = (_vehicleData ? _vehicleData : GetComponentInParent<VehicleData>());
			if ((isMultiFree & flag) || (flag && !flag2))
			{
				SubscribeUpdateDoors().Forget();
			}
			if (IsOpenCloseRoof)
			{
				HoldRoofBanks();
			}
		}

		private async UniTask SubscribeUpdateDoors()
		{
			await UniTask.WaitUntil(() => (bool)_vehicleData && (bool)_vehicleData.BodyKits && _vehicleData.BodyKits.AllBodyKitsWasLoaded.Value, PlayerLoopTiming.Update, this.GetCancellationTokenOnDestroy()).TimeoutWithoutException(TimeSpan.FromSeconds(5.0));
			if ((bool)_carAnimationController)
			{
				OnEnabled += _carAnimationController.UpdateOpenDoors;
				OnEnabled?.Invoke();
			}
		}

		private void OnEnable()
		{
			OnEnabled?.Invoke();
		}

		private async UniTask CheckRoof(CarAnimationsController carAnimationsController, bool isMultiPlayer, bool isPhotonViewCarMine)
		{
			await UniTask.DelayFrame(3);
			if ((bool)carAnimationsController && PlayerPrefs.GetInt("OpenedRoof" + carAnimationsController.VehicleData.CarId, 0) == 1 && (!isMultiPlayer | isPhotonViewCarMine))
			{
				carAnimationsController.OpenDoors[CarDoorType.Roof] = true;
				carAnimationsController.OpenDoors[CarDoorType.FrontLeftDoorWindow] = true;
				carAnimationsController.OpenDoors[CarDoorType.FrontRightDoorWindow] = true;
				carAnimationsController.UpdateOpenDoors();
			}
		}

		private void OnDestroy()
		{
			if ((bool)_carAnimationController)
			{
				OnEnabled -= _carAnimationController.UpdateOpenDoors;
			}
			_carAnimationController = null;
			OnEnabled = null;
			_setOldInteriorDisposable?.Dispose();
			_setOldInteriorDisposable = null;
			_bodyKitTimerSub?.Dispose();
			_bodyKitTimerSub = null;
			ReleaseRoofBanks();
		}

		public void SetInteriorParents(GameObject car, CarInteriorReferences interior)
		{
			if ((bool)car && (bool)interior && (bool)base.transform.parent && !(car != base.transform.parent.gameObject) && (bool)CarBodyReferences)
			{
				TrySetParent(interior.FrontLeftDoorInteriorPart, CarBodyReferences.FrontLeftDoor);
				TrySetParent(interior.FrontRightDoorInteriorPart, CarBodyReferences.FrontRightDoor);
				TrySetParent(interior.BackLeftDoorInteriorPart, CarBodyReferences.BackLeftDoor);
				TrySetParent(interior.BackRightDoorInteriorPart, CarBodyReferences.BackRightDoor);
				TrySetParent(interior.TrunkFirstDoorInteriorPart, CarBodyReferences.FirstTrunkDoor);
				TrySetParent(interior.TrunkSecondDoorInteriorPart, CarBodyReferences.SecondTrunkDoor);
			}
		}

		private void TrySetParent(Transform target, Transform parent)
		{
			if ((bool)target && (bool)parent)
			{
				target.transform.parent = parent;
				target.transform.localRotation = Quaternion.identity;
			}
		}

		public void SetHoodParent(Transform hood)
		{
			CarBodyReferences.Hood = hood;
		}

		public void SetTrunkParent(GameObject trunk, Transform parentToMoveableObjects, Transform plateTrunkParent)
		{
			CarBodyReferences.Trunk = trunk.transform;
			TrunkParent = parentToMoveableObjects;
			PlateTrunkParent = plateTrunkParent;
			Animator component = trunk.GetComponent<Animator>();
			CarDoorAnimators[CarDoorType.Trunk] = component;
		}

		public void SetInitialParentForPlate(Transform plate, bool isFront)
		{
			if ((bool)plate)
			{
				if (isFront)
				{
					_initialPlateHoodParent = plate.parent;
				}
				else
				{
					_initialPlateTrunkParent = plate.parent;
				}
			}
		}

		public void SetParentForPlate(GameObject plate, bool isFront)
		{
			if (!plate)
			{
				return;
			}
			if (!isFront)
			{
				if (IsPlateInTrunk && (bool)CarBodyReferences.Trunk)
				{
					plate.transform.parent = (PlateTrunkParent ? PlateTrunkParent : CarBodyReferences.Trunk);
				}
			}
			else if (IsPlateInHood && (bool)CarBodyReferences.Hood)
			{
				plate.transform.parent = (PlateHoodParent ? PlateHoodParent : CarBodyReferences.Hood);
			}
		}

		public void ResetPlateParent(GameObject plate, bool isFront)
		{
			if ((bool)plate & isFront)
			{
				plate.transform.parent = _initialPlateHoodParent;
			}
			if ((bool)plate && !isFront)
			{
				plate.transform.parent = _initialPlateTrunkParent;
			}
		}

		private bool IsIgnoreBodyKit(string bodyKitType)
		{
			return BodyKitsToIgnoreForMoveKit.Any((string s) => bodyKitType == s);
		}

		private bool IsSetAirIntakeBodyKit(string bodyKitType)
		{
			return BodyKitsToSetAirIntakeKit.Any((string s) => bodyKitType == s);
		}

		private bool IsSetSpoilerBodyKit(string bodyKitType)
		{
			return BodyKitsToSetSpoilerKit.Any((string s) => bodyKitType == s);
		}

		public void SetParentBodyKit(bodyKitValues bodyKit, BodyKitType bodyKitType)
		{
			if (bodyKitType == BodyKitType.Hood)
			{
				SetHoodParent(bodyKit.transform);
				return;
			}
			string bodyKitType2 = bodyKit.name.Replace("(Clone)", "").Trim();
			if (IsIgnoreBodyKit(bodyKitType2))
			{
				return;
			}
			if (IsSetAirIntakeBodyKit(bodyKitType2))
			{
				bodyKitType = BodyKitType.AirIntakeHood;
			}
			if (IsSetSpoilerBodyKit(bodyKitType2))
			{
				bodyKitType = BodyKitType.Spoiler;
			}
			IgnoreBodyKit(bodyKit.gameObject);
			if ((bodyKitType != BodyKitType.Spoiler && bodyKitType != BodyKitType.AirIntakeHood) || (bodyKitType == BodyKitType.Spoiler && !IsTrunkAnimate) || (bodyKitType == BodyKitType.AirIntakeHood && !IsHoodAnimate))
			{
				return;
			}
			if ((bool)bodyKit && bodyKitType == BodyKitType.AirIntakeHood && (bool)_vehicleData && (bool)_vehicleData.BodyKits)
			{
				_vehicleData.BodyKits.AllBodyKitsWasLoaded.Where((bool loaded) => loaded).First().Subscribe((bool _) =>
				{
					ParentKit(bodyKit, CarBodyReferences.Hood);
				})
					.AddTo(bodyKit.gameObject);
			}
			if ((bool)bodyKit && bodyKitType == BodyKitType.Spoiler && (bool)_vehicleData && (bool)_vehicleData.BodyKits)
			{
				_vehicleData.BodyKits.AllBodyKitsWasLoaded.Where((bool loaded) => loaded).First().Subscribe((bool _) =>
				{
					ParentKit(bodyKit, TrunkParent ? TrunkParent : CarBodyReferences.Trunk);
				})
					.AddTo(bodyKit.gameObject);
			}
		}

		public void EnsureSpoilerOnTrunk()
		{
			if (!IsTrunkAnimate)
			{
				return;
			}
			Transform transform = (TrunkParent ? TrunkParent : (CarBodyReferences ? CarBodyReferences.Trunk : null));
			if (!transform)
			{
				return;
			}
			if (!_vehicleData)
			{
				_vehicleData = GetComponentInParent<VehicleData>();
			}
			if (!_vehicleData || !_vehicleData.BodyKits)
			{
				return;
			}
			bodyKitValues installedBodyKitValues = _vehicleData.BodyKits.GetInstalledBodyKitValues(BodyKitType.Spoiler);
			if ((bool)installedBodyKitValues)
			{
				string bodyKitType = installedBodyKitValues.name.Replace("(Clone)", "").Trim();
				if (!IsIgnoreBodyKit(bodyKitType) && !installedBodyKitValues.transform.IsChildOf(transform))
				{
					installedBodyKitValues.transform.parent = transform;
				}
			}
		}

		private void ParentKit(bodyKitValues bodyKit, Transform parent)
		{
			if ((bool)bodyKit && (bool)parent)
			{
				bodyKit.gameObject.transform.parent = parent;
			}
		}

		public float GetDelayOpenCloseDoor(CarAnimationType animationType, string frontOrBack)
		{
			float num = 0f;
			return animationType switch
			{
				CarAnimationType.OpenOutside => (frontOrBack == "Front") ? DelayOpenFrontDoorOutside : DelayOpenBackDoorOutside, 
				CarAnimationType.SeatIntoCarOutside => (frontOrBack == "Front") ? DelayCloseFrontDoorInsideSeatIntoCar : DelayCloseBackDoorInsideSeatIntoCar, 
				CarAnimationType.OpenInside => (frontOrBack == "Front") ? DelayOpenFrontDoorInside : DelayOpenBackDoorInside, 
				CarAnimationType.GetOutOfTheCar => (frontOrBack == "Front") ? DelayCloseFrontDoorOutsideGetOut : DelayCloseBackDoorOutsideGetOut, 
				CarAnimationType.CloseInside => (frontOrBack == "Front") ? DelayCloseFrontDoorInside : DelayCloseBackDoorInside, 
				CarAnimationType.CloseOutside => (frontOrBack == "Front") ? DelayCloseFrontDoorOutside : DelayCloseBackDoorOutside, 
				_ => num, 
			};
		}

		public void SetupRoof(List<CarPartAnimationSequence> openSequence, List<CarPartAnimationSequence> closeSequence)
		{
			SwitchRoofAnimation(enable: true);
			RoofAnimationsOpen = openSequence;
			RoofAnimationsClose = closeSequence;
		}

		public void SetupSkirts(Animator leftAnimator, Animator rightAnimator, bool everyDoor = false, Animator leftBackAnimator = null, Animator rightBackAnimator = null)
		{
			if ((bool)leftAnimator)
			{
				LeftSkirtsAnimator = leftAnimator;
			}
			if ((bool)rightAnimator)
			{
				RightSkirtsAnimator = rightAnimator;
			}
			EveryDoor = everyDoor;
			if ((bool)leftBackAnimator)
			{
				LeftBackSkirtsAnimator = leftBackAnimator;
			}
			if ((bool)rightBackAnimator)
			{
				RightBackSkirtsAnimator = rightBackAnimator;
			}
		}

		public void ResetSkirts(Animator leftAnimator, Animator rightAnimator, Animator leftBackAnimator = null, Animator rightBackAnimator = null)
		{
			if (LeftSkirtsAnimator == leftAnimator)
			{
				LeftSkirtsAnimator = _defaultLeftSkirtsAnimator;
			}
			if (RightSkirtsAnimator == rightAnimator)
			{
				RightSkirtsAnimator = _defaultRightSkirtsAnimator;
			}
			if (LeftBackSkirtsAnimator == leftBackAnimator)
			{
				LeftBackSkirtsAnimator = _defaultLeftBackSkirtsAnimator;
			}
			if (RightBackSkirtsAnimator == rightBackAnimator)
			{
				RightBackSkirtsAnimator = _defaultRightBackSkirtsAnimator;
			}
			EveryDoor = _defaultEveryDoor;
		}

		public bool IsLeftSideDoor(CarDoorType doorType)
		{
			if (IsRightHanded)
			{
				if (doorType != CarDoorType.PassengerFrontDoor)
				{
					return doorType == CarDoorType.PassengerBackDoor2;
				}
				return true;
			}
			if (doorType != CarDoorType.DriverDoor)
			{
				return doorType == CarDoorType.PassengerBackDoor1;
			}
			return true;
		}

		public Animator GetSkirtsAnimator(CarDoorType doorType)
		{
			bool flag = IsLeftSideDoor(doorType);
			Animator result = (flag ? LeftSkirtsAnimator : RightSkirtsAnimator);
			if (!EveryDoor || (doorType != CarDoorType.PassengerBackDoor1 && doorType != CarDoorType.PassengerBackDoor2))
			{
				return result;
			}
			Animator animator = (flag ? LeftBackSkirtsAnimator : RightBackSkirtsAnimator);
			if (!animator)
			{
				return result;
			}
			return animator;
		}

		public void UpdateWindows()
		{
			Animator animator = (CarBodyReferences.CarFrontLeftDoorWindow ? CarBodyReferences.CarFrontLeftDoorWindow.GetComponent<Animator>() : null);
			Animator animator2 = (CarBodyReferences.CarFrontRightDoorWindow ? CarBodyReferences.CarFrontRightDoorWindow.GetComponent<Animator>() : null);
			if ((bool)animator)
			{
				CarDoorAnimators[CarDoorType.FrontLeftDoorWindow] = animator;
			}
			if ((bool)animator2)
			{
				CarDoorAnimators[CarDoorType.FrontRightDoorWindow] = animator2;
			}
		}

		public void SwitchRoofAnimation(bool enable)
		{
			IsOpenCloseRoof = enable;
		}

		private void HoldRoofBanks()
		{
			if (!_roofBanksHeld && (bool)_vehicleData && _vehicleData.SoundBankService != null)
			{
				_roofBanksHeld = true;
				_vehicleData.SoundBankService.TryLoadBank(SoundBankType.CarRooftop).Forget();
				_vehicleData.SoundBankService.TryLoadBank(SoundBankType.CarWindow).Forget();
			}
		}

		private void ReleaseRoofBanks()
		{
			if (_roofBanksHeld)
			{
				_roofBanksHeld = false;
				if ((bool)_vehicleData && _vehicleData.SoundBankService != null)
				{
					_vehicleData.SoundBankService.TryUnloadBank(SoundBankType.CarRooftop).Forget();
					_vehicleData.SoundBankService.TryUnloadBank(SoundBankType.CarWindow).Forget();
				}
			}
		}

		public static void SetAnimateSeatSafe(CarAnimationsController controller, int place, GameObject person)
		{
			if ((bool)controller)
			{
				CarAnimationReferences carAnimationReferences = controller.CarAnimationReferences;
				if ((bool)carAnimationReferences && carAnimationReferences.CurrentAnimateSeats != null && place >= 0 && place < carAnimationReferences.CurrentAnimateSeats.Length)
				{
					carAnimationReferences.CurrentAnimateSeats[place] = person;
				}
			}
		}

		public static Vector3 GetSizePlayerInCarSafe(CarAnimationsController controller, Vector3 fallback)
		{
			if (!controller)
			{
				return fallback;
			}
			CarAnimationReferences carAnimationReferences = controller.CarAnimationReferences;
			if (!carAnimationReferences)
			{
				return fallback;
			}
			return carAnimationReferences.SizePlayerInCar;
		}
	}
}

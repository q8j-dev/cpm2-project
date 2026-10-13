using System;
using CPMEngine;
using CodeStage.AntiCheat.Storage;
using UniRx;
using UnityEngine;
using VContainer;
using _Car_Parking;
using _Car_Parking.Scripts;
using _Car_Parking.Scripts.Car;
using _Car_Parking.Scripts.Car.Sounds;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.Scopes;
using _Car_Parking.Scripts.UIElement;
using _Car_Parking.Scripts.WindowSystem.Mediators;

public class CarCamerasController : MonoBehaviour
{
	private const float InsideCameraClippingNear = 0.01f;

	private const float InitCameraClippingNear = 0.1f;

	public GameObject activator;

	private CarCameras carCameras;

	private float _sizex;

	private float _sizey;

	private float _sizez;

	[HideInInspector]
	public float externalSizex;

	[HideInInspector]
	public float externalSizey;

	[HideInInspector]
	public float externalSizez;

	public int i = -1;

	private int _i2;

	private int _i3;

	private int _i5;

	private bool _reverse2;

	private int _pageNumber;

	private bool _menu23;

	private GUISkin _guiSkin;

	private Vector3 _velocity;

	private int _i4;

	private bool _resumeText;

	private bool _load;

	private int _sizeLoad;

	private string _scene1;

	private string _scene2;

	private string _scene3;

	public Vector3 fixedCamModePosition;

	private float _startLerpTime;

	private CameraDatas _cameraDatas;

	public bool DoNotUpdatePersonCam;

	private bool _justTouched;

	private float _timeJustTouch;

	private Camera _mainCam;

	private UIReferencesMediator _uiReferencesMediator;

	private OptionalDependency<UIReferencesMediator> _uiReferencesMediatorDep;

	private ParkingMenuUI _parkingMenuUI;

	private OptionalDependency<ParkingMenuUI> _parkingMenuUIDep;

	private IDisposable _disposablesetAfterSoundKoff;

	private IDisposable _disposablehideCounter;

	private VehicleData _cachedVehicleData;

	private Rigidbody _cachedRigidBody;

	private GameObject _cachedCar;

	private CoreGameManager _coreGameManager;

	private LevelsProvider _levelsProvider;

	private FreeDriveProvider _freeDriveProvider;

	private MenuLevelParent _menuLevelParent;

	private CarCamerasParametersConfig _carCamerasParametersConfig;

	private GameObject _cachedDriverPerson;

	private MainPerson _spectatedHiddenHead;

	private SoundControllerExtension _carInOutSound;

	public GameObject CachedCar => _cachedCar;

	public float maxDistanceToDynamicCam => FlagsStorage.IsLevelScene ? 30 : 50;

	public GameObject CachedDriverPerson
	{
		get
		{
			if (!_cachedDriverPerson)
			{
				return V.DriverPerson;
			}
			return _cachedDriverPerson;
		}
		set
		{
			if (_cachedDriverPerson != value)
			{
				RestoreSpectatedHead();
			}
			_cachedDriverPerson = value;
		}
	}

	public bool IsFirstPersonView => i == 0;

	public bool IsSpectatorCam => i == 5;

	private void HideSpectatedHead()
	{
		if ((bool)_cachedDriverPerson && _cachedDriverPerson != V.DriverPerson && _cachedDriverPerson.TryGetComponent<MainPerson>(out var component))
		{
			if ((bool)_spectatedHiddenHead && _spectatedHiddenHead != component)
			{
				_spectatedHiddenHead.SwitchHeadVisible(visible: true);
			}
			component.SwitchHeadVisible(visible: false);
			_spectatedHiddenHead = component;
		}
	}

	private void RestoreSpectatedHead()
	{
		if ((bool)_spectatedHiddenHead)
		{
			_spectatedHiddenHead.SwitchHeadVisible(visible: true);
		}
		_spectatedHiddenHead = null;
	}

	private void Awake()
	{
		_parkingMenuUI = _parkingMenuUIDep?.Value;
		_uiReferencesMediator = _uiReferencesMediatorDep?.Value;
		_mainCam = GetComponent<Camera>();
	}

	[Inject]
	public void Construct(LevelsProvider levelsProvider, CoreGameManager coreGameManager, FreeDriveProvider freeDriveProvider, MenuLevelParent menuLevelParent, CarCamerasParametersConfig camerasParametersConfig, OptionalDependency<UIReferencesMediator> freeUIMediator, OptionalDependency<ParkingMenuUI> parkingMenuUI, SoundControllerExtension carInOutSound)
	{
		_carInOutSound = carInOutSound;
		_menuLevelParent = menuLevelParent;
		_freeDriveProvider = freeDriveProvider;
		_levelsProvider = levelsProvider;
		_coreGameManager = coreGameManager;
		_parkingMenuUIDep = parkingMenuUI;
		_uiReferencesMediatorDep = freeUIMediator;
		_carCamerasParametersConfig = camerasParametersConfig;
	}

	private void Start()
	{
		carCameras = GetComponent<CarCameras>();
	}

	private void OnEnable()
	{
		V.OnCarEntered += SetCachedCar;
		V.OnCarExit += ClearCache;
	}

	private void OnDisable()
	{
		_disposablesetAfterSoundKoff?.Dispose();
		V.OnCarEntered -= SetCachedCar;
		V.OnCarExit -= ClearCache;
	}

	private void ClearCache(GameObject person, VehicleData vehicleData)
	{
		_cachedCar = null;
		_cachedVehicleData = null;
		_cachedRigidBody = null;
	}

	public void SetCachedCar(GameObject person, VehicleData vehicleData)
	{
		if ((bool)V.Car)
		{
			_cachedCar = V.Car;
			_cachedVehicleData = vehicleData;
			V.Car.TryGetComponent<Rigidbody>(out _cachedRigidBody);
		}
	}

	public void SetCachedCarForSpectator(VehicleData vehicleData)
	{
		if ((bool)vehicleData)
		{
			_cachedCar = vehicleData.gameObject;
			_cachedVehicleData = vehicleData;
			vehicleData.gameObject.TryGetComponent<Rigidbody>(out _cachedRigidBody);
		}
	}

	public void SetCamera(int index, Transform target, bool newTarget, bool temporary = false)
	{
		if ((newTarget && (bool)V.Car && V.Car != _cachedCar) || ((bool)V.Car && !_cachedCar))
		{
			_cachedCar = V.Car;
			V.Car.TryGetComponent<VehicleData>(out _cachedVehicleData);
			V.Car.TryGetComponent<Rigidbody>(out _cachedRigidBody);
		}
		if (!_mainCam)
		{
			_mainCam = GetComponent<Camera>();
		}
		if (!FlagsStorage.OutOfCar)
		{
			if (!this.carCameras)
			{
				this.carCameras = GetComponent<CarCameras>();
			}
			if (CPMGraphicsSettingsManager.Instance.AdditionalEffects == CPMGraphicsSettings.AdditionalEffects.Low)
			{
				if (index == 0)
				{
					if (!_cachedVehicleData.InteriorsComposition.Interior)
					{
						_cachedVehicleData.InteriorsComposition.TryRestoreSavedInterior();
					}
					else
					{
						_cachedVehicleData.InteriorsComposition.ShowHighInterior();
					}
				}
				else
				{
					_cachedVehicleData.InteriorsComposition.ShowLowInterior();
				}
			}
			else
			{
				_cachedVehicleData.InteriorsComposition.ShowHighInterior();
			}
			FlagsStorage.CamStaticMode = false;
			FlagsStorage.FixedCameraAndLookAt = false;
			this.carCameras.FollowPosRot = false;
			SetCamMoveJoysticks(index);
			if ((bool)_parkingMenuUI)
			{
				_parkingMenuUI.CloseCameraSettingsWindowWithButton();
			}
			if (!temporary && (bool)_uiReferencesMediator && (bool)_uiReferencesMediator.CameraTypeText)
			{
				_uiReferencesMediator.CameraTypeText.text = $"#{index + 1} {GetCamName(index)}";
				_uiReferencesMediator.CameraTypeText.gameObject.SetActive(value: true);
				_disposablehideCounter = Observable.Timer(TimeSpan.FromSeconds(3.0)).Subscribe((long _) =>
				{
					HideCounter();
				}).AddTo(this);
				if (index < 5)
				{
					PlayerPrefs.SetInt("LastCam", index);
				}
			}
			i = index;
			if (newTarget)
			{
				this.carCameras.mtarget = target;
			}
			this.carCameras.dampFixedCamera = false;
			this.carCameras.mouseOrbitFixedCamera = false;
			this.carCameras.driverView = false;
			_disposablesetAfterSoundKoff?.Dispose();
			FlagsStorage.SmoothFollow = false;
			FlagsStorage.ChangeCamAuto = false;
			this.carCameras.pitchAngleOffset = 0f;
			float num = 0f;
			this.carCameras.SmoothFollowAdd = 4.8f;
			_mainCam.nearClipPlane = 0.2f;
			float num2 = 0f;
			if ((bool)_cachedVehicleData && (bool)_cachedVehicleData.Collider)
			{
				num2 = 0f - (_cachedVehicleData.Collider.bounds.extents.y - 0.9f);
			}
			if ((bool)_cachedRigidBody && _cachedRigidBody.velocity.magnitude < 20f)
			{
				this.carCameras.lerpPosition = true;
			}
			bool flag = false;
			bool flag2 = false;
			if ((bool)_levelsProvider.LevelScript)
			{
				_cameraDatas = null;
				if (_levelsProvider.LevelScript.cameraDatas != null && !_levelsProvider.LevelScript.IsCarCameraSettingForLevel && _levelsProvider.LevelScript.cameraDatas.Length > 5 && index != 6)
				{
					flag2 = true;
					_cameraDatas = _levelsProvider.LevelScript.cameraDatas[index];
				}
				if (_cachedVehicleData.cameraDatas != null && _cameraDatas == null && _cachedVehicleData.cameraDatas.Length != 0)
				{
					this.carCameras.pitchAngleOffset = 0.5f;
					if (_cachedVehicleData.cameraDatas.Length > 5 && index != 6)
					{
						_cameraDatas = _cachedVehicleData.cameraDatas[index];
					}
				}
			}
			if (index != 0 && (bool)target)
			{
				_carInOutSound.SetOutCarSound();
			}
			if (index == 0 && (bool)target)
			{
				_carInOutSound.UpdateInCarSound(_cachedVehicleData.CarId);
				FlagsStorage.UpdatedToReverse = false;
				_mainCam.nearClipPlane = 0.1f;
				this.carCameras.dampFixedCamera = false;
				this.carCameras.mouseOrbitFixedCamera = false;
				_disposablesetAfterSoundKoff?.Dispose();
				_disposablesetAfterSoundKoff = Observable.Timer(TimeSpan.FromSeconds(0.10000000149011612)).Subscribe((long _) =>
				{
					SetAfterSoundKoff();
				}).AddTo(this);
				this.carCameras.FpvDefaultPitch = _carCamerasParametersConfig.GetPitch(0, _cachedVehicleData.isTruck, isTall: false);
			}
			else if (index == 1 && (bool)target)
			{
				bool isTall = false;
				this.carCameras.mycamera = CarCameras.Cameras.SmoothLookAt;
				this.carCameras.Target = target;
				FlagsStorage.SmoothFollow = true;
				if (FlagsStorage.IsFreeDriveMode)
				{
					int carId = _cachedVehicleData.CarId;
					isTall = CarIdStorage.TallCars.Contains(carId);
				}
				_carCamerasParametersConfig.Apply(this.carCameras, index, _cachedVehicleData.isTruck, isTall);
				SetValuesFromCarData();
				this.carCameras.pitchAngleOffset = this.carCameras.pitchAngle + this.carCameras.height;
			}
			else if (index == 2 && (bool)target)
			{
				this.carCameras.mycamera = CarCameras.Cameras.SmoothLookAt;
				this.carCameras.Target = target;
				FlagsStorage.SmoothFollow = true;
				_carCamerasParametersConfig.Apply(this.carCameras, index, _cachedVehicleData.isTruck, isTall: false);
				SetValuesFromCarData();
				this.carCameras.DefaultHeight = this.carCameras.height;
				this.carCameras.pitchAngleOffset = this.carCameras.pitchAngle + this.carCameras.height;
				this.carCameras.yawAngle = 0f;
			}
			else if (index == 3 && (bool)target)
			{
				this.carCameras.mycamera = CarCameras.Cameras.SmoothLookAt;
				this.carCameras.Target = target;
				_carCamerasParametersConfig.Apply(this.carCameras, index, _cachedVehicleData.isTruck, isTall: false);
				FlagsStorage.SmoothFollow = true;
				SetValuesFromCarData();
			}
			else if (index == 4 && (bool)target)
			{
				this.carCameras.mycamera = CarCameras.Cameras.SmoothLookAt;
				this.carCameras.Target = target;
				_carCamerasParametersConfig.Apply(this.carCameras, index, _cachedVehicleData.isTruck, isTall: false);
				FlagsStorage.UpdatedToReverse = false;
				SetValuesFromCarData();
				if (FlagsStorage.IsFreeDriveMode)
				{
					int carId2 = _cachedVehicleData.CarId;
					if ((bool)_parkingMenuUI)
					{
						_parkingMenuUI.ShowCustomCamButton(isActive: true);
					}
					FlagsStorage.SmoothFollow = false;
					float defaultValue = 0f;
					float defaultValue2 = 0f;
					float defaultValue3 = 0f;
					if (_cameraDatas != null)
					{
						defaultValue = _cameraDatas.distance;
						defaultValue2 = _cameraDatas.height;
						defaultValue3 = _cameraDatas.pitchAngleOffset;
					}
					float num3 = ObscuredPrefs.GetFloat($"camDistance{carId2}", ObscuredPrefs.GetFloat("camDistance", defaultValue));
					if (num3 != 0f)
					{
						this.carCameras.distance = num3;
					}
					float num4 = ObscuredPrefs.GetFloat($"camHeight{carId2}", ObscuredPrefs.GetFloat("camHeight", defaultValue2));
					if (num4 != 0f)
					{
						this.carCameras.height = num4;
					}
					float num5 = ObscuredPrefs.GetFloat($"camPitch{carId2}", ObscuredPrefs.GetFloat("camPitch", defaultValue3));
					if (num5 != 0f)
					{
						this.carCameras.pitchAngle = num5;
					}
					float num6 = ObscuredPrefs.GetFloat($"camDamp{carId2}", ObscuredPrefs.GetFloat("camDamp", 4f));
					if (num6 != 0f)
					{
						this.carCameras.rotationDamping = num6;
					}
					float num7 = ObscuredPrefs.GetFloat($"camField{carId2}", ObscuredPrefs.GetFloat("camField", 50f));
					if (num7 != 0f)
					{
						_mainCam.fieldOfView = num7;
					}
					flag = true;
				}
			}
			else if (index == 5 && (bool)target)
			{
				this.carCameras.mycamera = CarCameras.Cameras.SmoothLookAt;
				_carCamerasParametersConfig.Apply(this.carCameras, index, _cachedVehicleData.isTruck, isTall: false);
				SetValuesFromCarData();
				this.carCameras.rotationAngle = 0f;
			}
			else if (index == 6 && (bool)target)
			{
				FlagsStorage.FixedCameraAndLookAt = true;
				if ((bool)_levelsProvider.CurrentLevelData)
				{
					base.transform.position = _levelsProvider.CurrentLevelData.FixedCamPos;
				}
			}
			if (index != 0 && (bool)target)
			{
				if ((bool)V.DriverPerson && V.DriverPerson.TryGetComponent<MainPerson>(out var component))
				{
					component.SwitchHeadVisible(visible: true);
				}
				RestoreSpectatedHead();
				_mainCam.nearClipPlane = 0.1f;
			}
			if (FlagsStorage.IsLevelScene && _cameraDatas == null)
			{
				if (this.carCameras.height != 0f)
				{
					this.carCameras.height += num2;
				}
				this.carCameras.pitchAngle = 0f - this.carCameras.height;
			}
			if ((bool)_levelsProvider.LevelScript && _cameraDatas != null)
			{
				if (_cameraDatas.YawAngle != 0f)
				{
					this.carCameras.yawAngle = _cameraDatas.YawAngle;
				}
				if (_cameraDatas.SmoothFollowAdd != 0f)
				{
					this.carCameras.SmoothFollowAdd = _cameraDatas.SmoothFollowAdd;
				}
				if (_cameraDatas.pitchAngleOffset != 0f)
				{
					if (flag2)
					{
						this.carCameras.pitchAngle = _cameraDatas.pitchAngleOffset * 2f;
					}
					if (index == 0)
					{
						this.carCameras.pitchAngle = _cameraDatas.pitchAngleOffset;
						this.carCameras.distance = 0f;
						this.carCameras.height = 0f;
					}
				}
			}
			if (!FlagsStorage.IsLevelScene && (bool)_cachedCar && _cachedVehicleData.isTruck)
			{
				this.carCameras.SmoothFollowAdd = 9.5f;
			}
			if (!flag)
			{
				if (this.carCameras.distance > 0.2f)
				{
					this.carCameras.distance += num;
				}
				_mainCam.fieldOfView = 50f;
			}
			CarCameras carCameras = this.carCameras;
			carCameras.SteamLookMode = index switch
			{
				1 => CarCameras.SteamLook.Free, 
				3 => CarCameras.SteamLook.Fixed, 
				_ => CarCameras.SteamLook.Default, 
			};
			if (this.carCameras.SteamLookMode == CarCameras.SteamLook.Free && (bool)target)
			{
				this.carCameras.freeOrbitYaw = target.eulerAngles.y;
			}
			if (index == 0)
			{
				SetAfterSoundKoff();
			}
			this.carCameras.camChangedByTouch = false;
		}
		else
		{
			_mainCam.nearClipPlane = 0.2f;
			SetCamForPerson();
			_carInOutSound.SetOutCarSound();
		}
	}

	private void SetValuesFromCarData()
	{
		if (_cameraDatas != null)
		{
			if (_cameraDatas.distance != 0f)
			{
				carCameras.distance = _cameraDatas.distance;
			}
			if (_cameraDatas.height != 0f)
			{
				carCameras.height = _cameraDatas.height;
			}
			if (_cameraDatas.pitchAngleOffset != 0f)
			{
				carCameras.pitchAngle = _cameraDatas.pitchAngleOffset;
			}
		}
	}

	private string GetCamName(int index)
	{
		return index switch
		{
			0 => new LocalizedString("CarCamerasController.Inside"), 
			1 => new LocalizedString("CarCamerasController.Free"), 
			2 => new LocalizedString("CarCamerasController.DirectionView"), 
			3 => new LocalizedString("CarCamerasController.Fixed"), 
			5 => new LocalizedString("CarCamerasController.Dynamic"), 
			_ => "", 
		};
	}

	public void HideCounter()
	{
		_disposablehideCounter?.Dispose();
		if ((bool)_uiReferencesMediator && (bool)_uiReferencesMediator.CameraTypeText)
		{
			_uiReferencesMediator.CameraTypeText.gameObject.SetActive(value: false);
		}
	}

	private void SetCamForPerson()
	{
		_mainCam.fieldOfView = 60f;
		V.PersonCamIndex++;
		if (DoNotUpdatePersonCam)
		{
			DoNotUpdatePersonCam = false;
			return;
		}
		if ((bool)V.DriverPerson && V.DriverPerson.TryGetComponent<NewMoveController>(out var component))
		{
			component.ChangeCameraPosition();
		}
		PlayerPrefs.SetInt("PersonCamIndex", V.PersonCamIndex);
	}

	public void SetPersonCameraIndex(int index)
	{
		if (!_mainCam)
		{
			_mainCam = GetComponent<Camera>();
		}
		_mainCam.fieldOfView = 60f;
		V.PersonCamIndex = index;
		if ((bool)V.DriverPerson && V.DriverPerson.TryGetComponent<NewMoveController>(out var component))
		{
			component.SetCameraPosition(index);
		}
		PlayerPrefs.SetInt("PersonCamIndex", V.PersonCamIndex);
	}

	private void SetCamMoveJoysticks(int index)
	{
		if (i == 5 && index != 5)
		{
			CamStaticMode(on: false);
			FlagsStorage.FixedCameraAndLookAt = false;
			if ((bool)_menuLevelParent.InsideCamControllParent)
			{
				_menuLevelParent.InsideCamControllParent.SetActive(value: false);
			}
		}
		if (i != 5 && index == 5)
		{
			CamStaticMode(on: true);
			if ((bool)_menuLevelParent.InsideCamControllParent)
			{
				_menuLevelParent.InsideCamControllParent.SetActive(value: true);
				SetCamElementsActive(_menuLevelParent.FixedCamElements, active: true);
				SetCamElementsActive(_menuLevelParent.InsideCamElements, active: false);
			}
		}
		if (i == 0 && index != 0 && (bool)_menuLevelParent.InsideCamControllParent)
		{
			_menuLevelParent.InsideCamControllParent.SetActive(value: false);
		}
		if ((i != 0 && index == 0) || (i == 0 && index == 0))
		{
			if ((bool)_menuLevelParent.InsideCamControllParent)
			{
				_menuLevelParent.InsideCamControllParent.SetActive(value: true);
			}
			SetCamElementsActive(_menuLevelParent.FixedCamElements, active: false);
			SetCamElementsActive(_menuLevelParent.InsideCamElements, active: true);
		}
	}

	private void SetCamElementsActive(GameObject camElements, bool active)
	{
		if (!camElements)
		{
			return;
		}
		camElements.SetActive(active);
		if (!active)
		{
			return;
		}
		foreach (Transform item in camElements.transform)
		{
			item.gameObject.SetActive(value: true);
		}
	}

	private void SetAfterSoundKoff()
	{
		_disposablesetAfterSoundKoff?.Dispose();
		if ((bool)carCameras)
		{
			carCameras.mycamera = CarCameras.Cameras.FixedTo;
			carCameras.distance = 0f;
			carCameras.height = 0f;
			carCameras.pitchAngle = 0f;
			carCameras.yawAngle = 0f;
			carCameras.rotationAngle = 0f;
			carCameras.x = 0f;
			carCameras.y = 0f;
			if (_cameraDatas != null)
			{
				if (_cameraDatas.distance != 0f)
				{
					carCameras.distance = _cameraDatas.distance;
				}
				if (_cameraDatas.height != 0f)
				{
					carCameras.height = _cameraDatas.height;
				}
				if (_cameraDatas.YawAngle != 0f)
				{
					carCameras.yawAngle = _cameraDatas.YawAngle;
				}
				if (_cameraDatas.SmoothFollowAdd != 0f)
				{
					carCameras.SmoothFollowAdd = _cameraDatas.SmoothFollowAdd;
				}
				if (_cameraDatas.pitchAngleOffset != 0f)
				{
					carCameras.pitchAngle = _cameraDatas.pitchAngleOffset;
				}
			}
		}
		if (!CachedDriverPerson)
		{
			_disposablesetAfterSoundKoff = Observable.Timer(TimeSpan.FromSeconds(0.10000000149011612)).Subscribe((long _) =>
			{
				SetAfterSoundKoff();
			}).AddTo(this);
		}
		else
		{
			if (!CachedDriverPerson.TryGetComponent<MultiPersonControl>(out var component))
			{
				return;
			}
			int placeOnCar = component.placeOnCar;
			if (!_cachedCar && (bool)V.Car && V.Car.TryGetComponent<VehicleData>(out var component2))
			{
				SetCachedCar(CachedDriverPerson, component2);
			}
			if (!_cachedCar || !_cachedCar.TryGetComponent<DriverController>(out var component3))
			{
				return;
			}
			component3.SetCamHeadPoint(CachedDriverPerson, placeOnCar, skipUSerCheck: true);
			if ((bool)carCameras && (bool)component3.camInsideView)
			{
				Vector3 localEulerAngles = component3.camInsideView.transform.localEulerAngles;
				localEulerAngles.x = carCameras.FpvDefaultPitch;
				localEulerAngles.y = 0f;
				component3.camInsideView.transform.localEulerAngles = localEulerAngles;
			}
			if ((bool)V.DriverPerson && V.DriverPerson.TryGetComponent<MainPerson>(out var component4))
			{
				component4.TrySwitchVisibleHeadPerson(component3.camInsideView.transform);
				_mainCam.nearClipPlane = 0.01f;
			}
			HideSpectatedHead();
			if ((bool)carCameras)
			{
				if ((bool)component3.camInsideView)
				{
					carCameras.Target = component3.camInsideView.transform;
				}
				carCameras.driverView = true;
			}
		}
	}

	public void CamStaticMode(bool on)
	{
		FlagsStorage.CamStaticMode = on;
		FlagsStorage.FixedCameraAndLookAt = on;
		if (on)
		{
			SetPostionForCamStaticMode();
		}
		if ((bool)_menuLevelParent.focusToCar)
		{
			_freeDriveProvider.CarCameras.Target = ((!_menuLevelParent.focusToCar.isOn) ? null : _cachedCar.transform);
		}
	}

	public void SetPostionForCamStaticMode()
	{
		if ((bool)_cachedCar)
		{
			Transform carTransform = _cachedCar.transform;
			Powertrain component = _cachedCar.GetComponent<Powertrain>();
			float num = (FlagsStorage.IsPassenger ? Mathf.RoundToInt(component.SpeedMpsOnline) : Mathf.RoundToInt(component.SpeedMps));
			int showBack = 1;
			if (FlagsStorage.IsFreeDriveMode && num == 0f && UnityEngine.Random.Range(0, 2) == 0)
			{
				showBack = -1;
			}
			Vector3 pos = CalculateNewPos(carTransform, num, showBack);
			MakeRaycast(pos);
			_startLerpTime = Time.time;
			base.transform.position = fixedCamModePosition;
		}
		Vector3 CalculateNewPos(Transform transform, float add, int num2)
		{
			Vector3 vector = transform.position + transform.forward * (UnityEngine.Random.Range(6f + add, 15f + add) * (float)num2) + transform.right * UnityEngine.Random.Range(-10, 10);
			vector.y = transform.position.y + (float)UnityEngine.Random.Range(3, 10);
			for (int i = 0; i < 5; i++)
			{
				if (Vector3.Distance(_cachedCar.transform.position, vector) < maxDistanceToDynamicCam)
				{
					return vector;
				}
				vector = transform.position + transform.forward * (UnityEngine.Random.Range(6f + add, 15f + add) * (float)num2) + transform.right * UnityEngine.Random.Range(-10, 10);
			}
			Vector3 vector2 = (_cachedCar.transform.position - vector).normalized * maxDistanceToDynamicCam;
			vector = _cachedCar.transform.position - vector2;
			vector.y = transform.position.y + (float)UnityEngine.Random.Range(3, 10);
			return vector;
		}
	}

	public Vector3 MakeRaycast(Vector3 pos)
	{
		Transform transform = _cachedCar.transform;
		Vector3 position = transform.position;
		Vector3 vector = position - pos;
		LayerMask layerMask = -1;
		float maxDistance = Vector3.Distance(pos, position);
		if (Physics.Raycast(position, -vector, out var hitInfo, maxDistance, layerMask.value, QueryTriggerInteraction.Ignore) && !hitInfo.transform.CompareTag("Car") && !hitInfo.transform.IsChildOf(transform))
		{
			fixedCamModePosition = transform.position - vector.normalized * (hitInfo.distance - 1f);
			return fixedCamModePosition;
		}
		fixedCamModePosition = pos;
		return fixedCamModePosition;
	}

	private void Update()
	{
		_ = carCameras.mycamera;
		_ = 3;
		if (FlagsStorage.CamStaticMode && (bool)_cachedCar && Vector3.Distance(_cachedCar.transform.position, base.transform.position) > maxDistanceToDynamicCam && !IsPointInFrontOfVelocity(_cachedCar.GetComponent<Rigidbody>(), base.transform.position))
		{
			SetPostionForCamStaticMode();
		}
	}

	private bool IsPointInFrontOfVelocity(Rigidbody rb, Vector3 point)
	{
		Vector3 lhs = Vector3.Normalize(rb.velocity);
		Vector3 rhs = Vector3.Normalize(point - rb.position);
		return Vector3.Dot(lhs, rhs) > 0f;
	}
}

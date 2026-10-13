using CPMEngine;
using UniRx;
using UnityEngine;

namespace _Car_Parking
{
	public class FlagsStorage
	{
		public static bool IsFreeFuelWindow;

		public static bool CanEnterGarage = true;

		public static bool IsInGarageTunnel;

		public static bool CamFollowCar;

		public static bool MoveByTouch;

		public static bool IgnoreTouchesOnMenu;

		public static bool LevelStarted;

		public static bool FixedCameraAndLookAt;

		public static bool IsEnabledJoy;

		public static bool ShowCarMirror;

		public static bool IsMultiFree;

		public static bool DebugChatInSinglePlayer;

		public static bool CarlessMode;

		public static bool ChasingRaceModeStarted;

		public static bool ShowParkingSensors;

		public static bool PRND = true;

		public static bool IsCarDamaged;

		public static bool IsReadyMultRaceCar2;

		public static bool RacingGameMode;

		public static bool RaceStarted = false;

		public static bool Car2Finished;

		public static bool IsLevelScene;

		public static bool IsMainMenu;

		public static bool KeepRow = false;

		public static bool StartInHome = false;

		public static bool IsFreeDriveMode = false;

		public static bool MovingInsideCamPos = false;

		public static bool SolverCheckStarted = false;

		public static bool CloserMountain;

		public static bool CloserCity2;

		public static bool NowPolice;

		public static bool LoadCollidedInvoked;

		public static bool OnExteriorMenu;

		public static bool OnVisualTuning;

		public static bool StoreInitialized;

		public static bool ForceSendValues;

		public static bool KeepCityParts = true;

		public static bool SmoothFollow;

		public static bool ChangeCamAuto;

		public static bool UpdatedToReverse;

		public static bool CreateForFriendOnly = false;

		public static bool IsLoadingGarageCars;

		public static bool RaycastForSpeeding;

		public static bool CamStaticMode;

		public static readonly ReactiveProperty<bool> FirstMainSceneLoaded = new ReactiveProperty<bool>();

		public static bool HideAllTopLabel;

		public static bool HideSkidSmoke;

		public static bool HideSkidMarks;

		public static bool HideWheelDirt;

		public static bool LightTransformChangeMode = false;

		public static bool AutoStartWwReview = false;

		public static bool IsPurchaserInitialized;

		public static bool SingeDragRacingMode;

		public static bool InGarageMenu = false;

		public static bool EndlessCrashTime = false;

		public static bool IsCheckLoadCarDebug = false;

		public static bool EnteredForwardInGarageTunnel = true;

		public static bool LoginEventSend = false;

		public static bool AnalyticsInitialized = false;

		public static bool UseNewLocomotion = true;

		public static bool IsPrivateRoom = false;

		public static bool IsSpectatorMode = false;

		public static bool FreeWalk = false;

		public static bool IsDoorOpenAnimation = false;

		public static bool IsLoadingFromEvent = false;

		public static bool IsSteamDeck = false;

		public static bool IgnoreTriggersAfterExit = false;

		public const string CasualModePrefsKey = "SteamControls.CasualMode";

		private static bool _casualLoaded;

		public static bool LoadedFromGarage = false;

		public static bool IsAutomatic = false;

		private static bool _inGarageFreeNow = false;

		private static bool _outOfCar;

		private static bool _isPassenger = false;

		private static bool _isPhotoMode = false;

		private static bool _isInputEnabledInThisWindow = true;

		public static bool IsChatAllowed
		{
			get
			{
				if (!IsMultiFree)
				{
					return DebugChatInSinglePlayer;
				}
				return true;
			}
		}

		public static bool DynorunMode
		{
			get
			{
				return PhysRuntimeFlags.DynorunMode;
			}
			set
			{
				PhysRuntimeFlags.DynorunMode = value;
			}
		}

		public static bool IsCarInService
		{
			get
			{
				return PhysRuntimeFlags.IsCarInService;
			}
			set
			{
				PhysRuntimeFlags.IsCarInService = value;
			}
		}

		public static bool Kph
		{
			get
			{
				return PhysRuntimeFlags.Kph;
			}
			set
			{
				PhysRuntimeFlags.Kph = value;
			}
		}

		public static bool IsCasualControl
		{
			get
			{
				if (!_casualLoaded)
				{
					PhysRuntimeFlags.IsCasualControl = PlayerPrefs.GetInt("SteamControls.CasualMode", 0) == 1;
					_casualLoaded = true;
				}
				return PhysRuntimeFlags.IsCasualControl;
			}
			set
			{
				if (!_casualLoaded || PhysRuntimeFlags.IsCasualControl != value)
				{
					PhysRuntimeFlags.IsCasualControl = value;
					_casualLoaded = true;
					PlayerPrefs.SetInt("SteamControls.CasualMode", value ? 1 : 0);
				}
			}
		}

		public static bool IsInputEnabledInThisWindow
		{
			get
			{
				return _isInputEnabledInThisWindow;
			}
			set
			{
				if (_isInputEnabledInThisWindow != value)
				{
					_isInputEnabledInThisWindow = value;
				}
			}
		}

		public static bool InGarageFree
		{
			get
			{
				return _inGarageFreeNow;
			}
			set
			{
				if (_inGarageFreeNow != value)
				{
					_inGarageFreeNow = value;
					V.OnInGarageFreeChanged?.Invoke(value);
				}
			}
		}

		public static bool OutOfCar
		{
			get
			{
				return _outOfCar;
			}
			set
			{
				_outOfCar = value;
				V.OnOutOfCarChanged?.Invoke(value);
			}
		}

		public static bool Interacting { get; set; }

		public static bool IsPassenger
		{
			get
			{
				return _isPassenger;
			}
			set
			{
				_isPassenger = value;
				V.OnIsPassengerChanged?.Invoke(value);
			}
		}

		public static bool IsPhotoMode
		{
			get
			{
				return _isPhotoMode;
			}
			set
			{
				_isPhotoMode = value;
				V.OnPhotoModeChanged?.Invoke(value);
			}
		}
	}
}

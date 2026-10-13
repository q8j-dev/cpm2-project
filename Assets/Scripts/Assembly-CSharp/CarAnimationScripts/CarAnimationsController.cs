using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CPMEngine;
using CarInteractionAnimationsScripts;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using Photon.Realtime;
using UniRx;
using UnityEngine;
using VContainer;
using _Car_Parking;
using _Car_Parking.Scripts.Audio;
using _Car_Parking.Scripts.Car;
using _Car_Parking.Scripts.Multiplayer.RpcMethods.RpcObjects;
using _Car_Parking.Scripts.Other.Helpers;
using _Car_Parking.Scripts.Police;
using _Car_Parking.Scripts.Scopes;

namespace CarAnimationScripts
{
	public class CarAnimationsController : MonoBehaviour
	{
		private const float MaxSpeedToCloseDoors = 20f;

		private const string TrueString = "1";

		private const string FalseString = "0";

		private const string PlayCarDoorOpen = "Play_Car_Door_Open";

		private const string PlayCarDoorClose = "Play_Car_Door_Close";

		private const string PlayTrunkOpen = "Play_trunk_open";

		private const string PlayTrunkClose = "Play_trunk_close";

		private const string PlaySideDoorOpen = "Play_Van_Slide_Door_Open";

		private const string PlaySideDoorClose = "Play_Van_Slide_Door_Close";

		private const float DelayForPlayCloseDoorSound = 0.2f;

		private const float TimePeriodCheckOpenedDoors = 1f;

		public const string OpenedRoofKey = "OpenedRoof";

		private readonly int _openCarElement = Animator.StringToHash("Open");

		private readonly int _closeCarElement = Animator.StringToHash("Close");

		private readonly int _fastOpen = Animator.StringToHash("FastOpen");

		private readonly int _fastClose = Animator.StringToHash("FastClose");

		private List<CarDoorType> _openedDoorsKeys;

		private Powertrain _powertrain;

		private VehicleData _vehicleData;

		private PhotonView _photonViewCar;

		private MultiPlayer _multiPlayer;

		private IDisposable _resetPlateParentDisposable;

		private IDisposable _resetPoliceTopLightParent;

		public GameObject ThisCar => base.gameObject;

		public Dictionary<CarDoorType, bool> OpenDoors { get; private set; }

		public VehicleData VehicleData
		{
			get
			{
				CommonExtensions.NullIfDestroyed(ref _vehicleData);
				return _vehicleData;
			}
			private set
			{
				_vehicleData = value;
			}
		}

		public PhotonView PhotonViewCar
		{
			get
			{
				CommonExtensions.NullIfDestroyed(ref _photonViewCar);
				return _photonViewCar;
			}
			private set
			{
				_photonViewCar = value;
			}
		}

		public CarAnimationsPoints CarAnimationsPoints => CarAnimationReferences.CarAnimationsPoints;

		public CarAnimationReferences CarAnimationReferences { get; private set; }

		public bool IsMoveSideDoor { get; private set; }

		public bool IsMoveRoof { get; set; }

		public bool IsAnimatingTrunk { get; set; }

		public bool IsAnimatingHood { get; set; }

		[Inject]
		public void Construct(OptionalDependency<MultiPlayer> multiPlayer)
		{
			_multiPlayer = multiPlayer.Value;
		}

		private void Awake()
		{
			InitOpenDoorsDictionaryIfNeeds();
			TryGetComponent<Powertrain>(out _powertrain);
			VehicleData = GetComponent<VehicleData>();
			PhotonViewCar = GetComponent<PhotonView>();
			_openedDoorsKeys = new List<CarDoorType>();
			Observable.Timer(TimeSpan.FromSeconds(0.0), TimeSpan.FromSeconds(1.0)).Subscribe(CheckOpenedDoors).AddTo(this);
		}

		private void Start()
		{
			V.CoreGameManager.CarInteractionAnimationsCache.AddCar(VehicleData.CarId);
			if (!_multiPlayer && (bool)V.CoreGameManager && V.CoreGameManager.FreeDriveProvider != null)
			{
				_multiPlayer = V.CoreGameManager.FreeDriveProvider.MultiPlayer;
			}
		}

		private void OnDestroy()
		{
			if ((bool)V.CoreGameManager && (bool)VehicleData)
			{
				V.CoreGameManager.CarInteractionAnimationsCache.RemoveCar(VehicleData.CarId);
			}
			PhotonViewCar = null;
			CarAnimationReferences = null;
			VehicleData = null;
		}

		public void SetMoveSideDoor(bool isMoving)
		{
			IsMoveSideDoor = isMoving;
		}

		public void SetCarAnimationReference(CarAnimationReferences reference, bool loadLocalInterior = false)
		{
			CarAnimationReferences = reference;
		}

		public void SendDataOpenDoors(Player player)
		{
			InitOpenDoorsDictionaryIfNeeds();
			StringBuilder stringBuilder = new StringBuilder();
			foreach (KeyValuePair<CarDoorType, bool> openDoor in OpenDoors)
			{
				stringBuilder.Append(openDoor.Value ? "1" : "0");
			}
			if ((bool)_multiPlayer)
			{
				_multiPlayer.SendValueStrToPlayer<RpcSendDataOpenDoors>(PhotonViewCar.ViewID.ToString(), stringBuilder.ToString(), player);
			}
		}

		private void CheckOpenedDoors(long _)
		{
			CommonExtensions.NullIfDestroyed(ref _powertrain);
			CommonExtensions.NullIfDestroyed(ref _photonViewCar);
			CommonExtensions.NullIfDestroyed(ref _vehicleData);
			if ((bool)_powertrain && !_powertrain.OnLineCar && _powertrain.SpeedKmh > 20f && !OpenDoors.Values.All((bool value) => !value))
			{
				CloseMainOpenedDoors();
			}
		}

		public void CloseMainOpenedDoors()
		{
			foreach (CarDoorType door in OpenDoors.Keys)
			{
				bool num = IsDoorElement(door);
				bool flag = IsMoveRoof && CarAnimationReferences != null && (CarAnimationReferences.RoofAnimationsOpen.Any((CarPartAnimationSequence a) => a.AnimType == door) || CarAnimationReferences.RoofAnimationsClose.Any((CarPartAnimationSequence a) => a.AnimType == door));
				if (!((!num || !OpenDoors[door]) | flag))
				{
					_openedDoorsKeys.Add(door);
				}
			}
			foreach (CarDoorType item in _openedDoorsKeys.ToList())
			{
				FastOpenCloseDoor(item, isOpen: false);
				_openedDoorsKeys.Remove(item);
			}
		}

		private bool IsDoorElement(CarDoorType doorType)
		{
			if (doorType != CarDoorType.DriverDoor && doorType != CarDoorType.PassengerFrontDoor && doorType != CarDoorType.PassengerBackDoor1 && doorType != CarDoorType.PassengerBackDoor2 && doorType != CarDoorType.Hood)
			{
				return doorType == CarDoorType.Trunk;
			}
			return true;
		}

		public void OpenCloseCarElement(CarDoorType elementType, bool isOpen, bool isSendData = false, bool isSideDoor = false, bool playSound = true)
		{
			if (elementType == CarDoorType.None)
			{
				return;
			}
			if (((bool)_multiPlayer & isSendData) && !ScenesHelper.GarageIsActive() && (bool)PhotonViewCar)
			{
				int num = (int)elementType;
				string text = num.ToString();
				string text2 = (isOpen ? "1" : "0");
				string text3 = text + "," + text2;
				string b = PhotonViewCar.ViewID.ToString();
				_multiPlayer.SendValueStr<RpcOpenCloseDoorCar>(b, text3);
			}
			if (isOpen && (elementType == CarDoorType.DriverDoor || elementType == CarDoorType.PassengerFrontDoor))
			{
				GetDoorState(isDoorOpen: true);
			}
			BodyMarker componentInChildren = base.transform.GetComponentInChildren<BodyMarker>(includeInactive: true);
			CarAnimationReferences componentInChildren2 = base.transform.GetComponentInChildren<CarAnimationReferences>(includeInactive: true);
			if (!componentInChildren)
			{
				return;
			}
			Animator animator = (componentInChildren2.CarDoorAnimators.ContainsKey(elementType) ? componentInChildren2.CarDoorAnimators[elementType] : null);
			if (!animator && elementType != CarDoorType.Hood)
			{
				return;
			}
			if (elementType == CarDoorType.Hood)
			{
				componentInChildren.OpenHood(isOpen);
			}
			else if (animator != null)
			{
				animator.SetTrigger(isOpen ? _openCarElement : _closeCarElement);
				TryOpenCloseSkirts(elementType, isOpen, isFast: false);
			}
			if ((bool)VehicleData && !VehicleData.PlatesComposition.Plates.IsNullOrEmpty() && (elementType == CarDoorType.Trunk || elementType == CarDoorType.Hood))
			{
				GameObject plate = ((elementType == CarDoorType.Hood) ? VehicleData.PlatesComposition.Plates[0] : VehicleData.PlatesComposition.Plates[1]);
				if (isOpen)
				{
					_resetPlateParentDisposable?.Dispose();
					_resetPlateParentDisposable = null;
					CarAnimationReferences.SetParentForPlate(plate, elementType == CarDoorType.Hood);
				}
				else
				{
					_resetPlateParentDisposable = Observable.Timer(TimeSpan.FromSeconds(1.5)).Subscribe((long _) =>
					{
						CarAnimationReferences.ResetPlateParent(plate, elementType == CarDoorType.Hood);
					}).AddTo(this);
				}
			}
			TrySetPoliceLightParent(isOpen, elementType);
			OpenDoors[elementType] = isOpen;
			if (elementType == CarDoorType.Hood || !playSound)
			{
				return;
			}
			string eventName = "Play_Car_Door_Open";
			string closeDoorSound = "Play_Car_Door_Close";
			if (isSideDoor)
			{
				eventName = "Play_Van_Slide_Door_Open";
				closeDoorSound = "Play_Van_Slide_Door_Close";
			}
			if (elementType == CarDoorType.Trunk)
			{
				eventName = "Play_trunk_open";
				closeDoorSound = "Play_trunk_close";
			}
			if (isOpen)
			{
				if (base.gameObject.activeInHierarchy)
				{
					AudioWrapper.PostEvent(eventName, base.gameObject);
				}
			}
			else
			{
				if (!(animator != null))
				{
					return;
				}
				animator.Update(0f);
				float num2 = ((elementType == CarDoorType.Trunk) ? (GetCurrentAnimationLength(animator) - 0.3f) : 0.2f);
				num2 = (isSideDoor ? 0f : num2);
				num2 = ((num2 < 0f) ? 0f : num2);
				Observable.Timer(TimeSpan.FromSeconds(num2)).Subscribe((long _) =>
				{
					if (base.gameObject.activeInHierarchy)
					{
						AudioWrapper.PostEvent(closeDoorSound, base.gameObject);
					}
					if (elementType == CarDoorType.DriverDoor || elementType == CarDoorType.PassengerFrontDoor)
					{
						GetDoorState(isDoorOpen: false);
					}
				}).AddTo(this);
			}
		}

		private void TryOpenCloseSkirts(CarDoorType elementType, bool isOpen, bool isFast)
		{
			if (IsDoorElement(elementType) && (bool)CarAnimationReferences)
			{
				Animator skirtsAnimator = CarAnimationReferences.GetSkirtsAnimator(elementType);
				if ((bool)skirtsAnimator)
				{
					int trigger = ((!isOpen) ? (isFast ? _fastClose : _closeCarElement) : (isFast ? _fastOpen : _openCarElement));
					skirtsAnimator.SetTrigger(trigger);
				}
			}
		}

		public float GetCurrentAnimationLength(Animator animator)
		{
			if (animator == null || animator.runtimeAnimatorController == null)
			{
				return 0f;
			}
			AnimatorClipInfo[] currentAnimatorClipInfo = animator.GetCurrentAnimatorClipInfo(0);
			if (currentAnimatorClipInfo.Length == 0)
			{
				return 0f;
			}
			return currentAnimatorClipInfo[0].clip.length;
		}

		private void TrySetPoliceLightParent(bool isOpen, CarDoorType doorType)
		{
			if (!VehicleData || !VehicleData.PoliceCarController || !base.gameObject.TryGetComponent<SetterPoliceParentController>(out var setterPoliceParentController) || setterPoliceParentController.ParentType != doorType || !VehicleData.PoliceCarController.BodyKitsUpdater)
			{
				return;
			}
			if (!isOpen)
			{
				_resetPoliceTopLightParent?.Dispose();
				_resetPoliceTopLightParent = null;
				_resetPoliceTopLightParent = Observable.Timer(TimeSpan.FromSeconds(1.5)).Subscribe((long _) =>
				{
					setterPoliceParentController.ResetParent();
				}).AddTo(this);
				return;
			}
			_resetPoliceTopLightParent?.Dispose();
			_resetPoliceTopLightParent = null;
			PoliceCarBodyKit bodyKit = VehicleData.PoliceCarController.BodyKitsUpdater.GetBodyKit(PoliceBodyKitType.TopLight);
			if (!bodyKit)
			{
				return;
			}
			Transform transform = null;
			if (setterPoliceParentController.ParentType == CarDoorType.Trunk)
			{
				if ((bool)CarAnimationReferences.TrunkParent)
				{
					transform = CarAnimationReferences.TrunkParent;
				}
				else if ((bool)CarAnimationReferences.CarDoorAnimators[CarDoorType.Trunk])
				{
					transform = CarAnimationReferences.CarDoorAnimators[CarDoorType.Trunk].transform;
				}
			}
			if ((bool)transform)
			{
				setterPoliceParentController.SetParent(transform, bodyKit.gameObject);
			}
		}

		public async UniTask OpenCloseTrunk(bool isOpen, bool sendData)
		{
			if (IsAnimatingTrunk)
			{
				return;
			}
			if (((bool)_multiPlayer & sendData) && !ScenesHelper.GarageIsActive() && (bool)PhotonViewCar)
			{
				string text = (isOpen ? "1" : "0");
				_multiPlayer.SendValueStr<RpcOpenCloseTrunk>(PhotonViewCar.ViewID.ToString(), text);
			}
			if (!CarAnimationReferences)
			{
				return;
			}
			if (isOpen)
			{
				CarAnimationReferences.EnsureSpoilerOnTrunk();
			}
			IsAnimatingTrunk = true;
			if (CarAnimationReferences.TrunkAnimationsOpen.IsNullOrEmpty() && CarAnimationReferences.TrunkAnimationsClose.IsNullOrEmpty())
			{
				OpenCloseCarElement(CarDoorType.Trunk, isOpen, sendData);
				if (CarAnimationReferences.CarDoorAnimators.TryGetValue(CarDoorType.Trunk, out var value))
				{
					Observable.Timer(TimeSpan.FromSeconds(GetCurrentAnimationLength(value))).Subscribe((long _) =>
					{
						IsAnimatingTrunk = false;
					}).AddTo(this);
				}
				return;
			}
			List<CarPartAnimationSequence> carPartAnims = (isOpen ? CarAnimationReferences.TrunkAnimationsOpen : CarAnimationReferences.TrunkAnimationsClose);
			for (int i = 0; i < carPartAnims.Count; i++)
			{
				await PlayAnimation(carPartAnims, i);
				if (!V.DriverPerson || !this)
				{
					return;
				}
			}
			IsAnimatingTrunk = false;
		}

		private bool TryFastOpenCloseTrunk(bool isOpen)
		{
			List<CarPartAnimationSequence> list = (isOpen ? CarAnimationReferences.TrunkAnimationsOpen : CarAnimationReferences.TrunkAnimationsClose);
			if (list.IsNullOrEmpty())
			{
				return false;
			}
			foreach (CarPartAnimationSequence item in list)
			{
				FastOpenCloseCarElement(item.AnimType, item.IsOpen);
			}
			return true;
		}

		public async UniTask OpenCloseRoof(bool isOpen, bool isSendData)
		{
			if (!CarAnimationReferences || CarAnimationReferences.RoofAnimationsOpen == null || (CarAnimationReferences.RoofAnimationsOpen.Any((CarPartAnimationSequence a) => a.AnimType == CarDoorType.Trunk) && IsAnimatingTrunk))
			{
				return;
			}
			if (isSendData && (bool)_multiPlayer)
			{
				string text = (isOpen ? "1" : "0") ?? "";
				string b = PhotonViewCar.ViewID.ToString();
				_multiPlayer.SendValueStr<RpcOpenCloseRoof>(b, text);
				PlayerPrefs.SetInt("OpenedRoof" + VehicleData.CarId, isOpen ? 1 : 0);
			}
			else if (!FlagsStorage.IsMultiFree)
			{
				PlayerPrefs.SetInt("OpenedRoof" + VehicleData.CarId, isOpen ? 1 : 0);
			}
			IsMoveRoof = true;
			List<CarPartAnimationSequence> carPartAnims = (isOpen ? CarAnimationReferences.RoofAnimationsOpen : CarAnimationReferences.RoofAnimationsClose);
			for (int i = 0; i < carPartAnims.Count; i++)
			{
				if (VehicleData.CarId != 225)
				{
					await PlayAnimation(carPartAnims, i);
				}
				else
				{
					await PlayRoofAnimationsHondaDelSol225(carPartAnims, i, isOpen);
				}
				if (!V.DriverPerson || !this)
				{
					return;
				}
			}
			if ((bool)V.DriverPerson && (bool)this)
			{
				IsMoveRoof = false;
			}
		}

		private async UniTask PlayAnimation(List<CarPartAnimationSequence> carPartAnims, int index)
		{
			CarPartAnimationSequence carPartAnim = carPartAnims[index];
			if (!CarAnimationReferences.CarDoorAnimators.TryGetValue(carPartAnim.AnimType, out var value) || !value)
			{
				return;
			}
			OpenCloseCarElement(carPartAnim.AnimType, carPartAnim.IsOpen, isSendData: false, isSideDoor: false, playSound: false);
			if (!string.IsNullOrEmpty(carPartAnim.SoundEvent) && carPartAnim.CanPlaySound(VehicleData))
			{
				Observable.Timer(TimeSpan.FromSeconds(carPartAnim.DelayPlaySound)).Subscribe((long _) =>
				{
					if (base.gameObject.activeInHierarchy)
					{
						AudioWrapper.PostEvent(carPartAnim.SoundEvent, base.gameObject);
					}
				}).AddTo(this);
			}
			await UniTask.Delay(TimeSpan.FromSeconds(carPartAnim.DelayAfterPlay));
		}

		private async UniTask PlayRoofAnimationsHondaDelSol225(List<CarPartAnimationSequence> carPartAnims, int index, bool isOpen)
		{
			CarPartAnimationSequence carPartAnimationSequence = carPartAnims[index];
			if (carPartAnimationSequence.AnimType != CarDoorType.Trunk || (carPartAnimationSequence.IsOpen && !OpenDoors[CarDoorType.Trunk]) || (!carPartAnimationSequence.IsOpen && OpenDoors[CarDoorType.Trunk]))
			{
				OpenCloseCarElement(carPartAnimationSequence.AnimType, carPartAnimationSequence.IsOpen, isSendData: false, isSideDoor: false, playSound: false);
			}
			if (!string.IsNullOrEmpty(carPartAnimationSequence.SoundEvent) && base.gameObject.activeInHierarchy)
			{
				AudioWrapper.PostEvent(carPartAnimationSequence.SoundEvent, base.gameObject);
			}
			if (index == carPartAnims.Count - (isOpen ? 1 : 3) && CarAnimationReferences.CarDoorAnimators.ContainsKey(CarDoorType.Roof) && (bool)CarAnimationReferences.CarDoorAnimators[CarDoorType.Roof])
			{
				CarAnimationReferences.CarDoorAnimators[CarDoorType.Roof].transform.parent = ((!isOpen) ? CarAnimationReferences.transform : CarAnimationReferences.CarDoorAnimators[CarDoorType.Trunk].transform);
			}
			await UniTask.Delay(TimeSpan.FromSeconds(carPartAnimationSequence.DelayAfterPlay));
		}

		private void SendRpcOpenCloseCarElement(CarDoorType elementType, bool isOpen)
		{
			if (elementType == CarDoorType.Hood)
			{
				string text = (isOpen ? "1" : "0");
				_multiPlayer.SendValueStr<RpcOpenCloseHood>(PhotonViewCar.ViewID.ToString(), text);
			}
		}

		public void FastOpenCloseDoor(CarDoorType elementType, bool isOpen)
		{
			OpenDoors[elementType] = isOpen;
			if (elementType == CarDoorType.Hood)
			{
				BodyMarker componentInChildren = base.transform.GetComponentInChildren<BodyMarker>();
				if ((bool)componentInChildren)
				{
					componentInChildren.OpenHood(isOpen, fast: true);
				}
			}
			else if ((bool)CarAnimationReferences && (elementType != CarDoorType.Trunk || !TryFastOpenCloseTrunk(isOpen)))
			{
				FastOpenCloseCarElement(elementType, isOpen);
			}
		}

		private void FastOpenCloseCarElement(CarDoorType elementType, bool isOpen)
		{
			if (CarAnimationReferences.CarDoorAnimators.TryGetValue(elementType, out var value) && (bool)value)
			{
				OpenDoors[elementType] = isOpen;
				if (elementType == CarDoorType.Roof && VehicleData.CarId == 225)
				{
					FastOpenCloseRoofHondaDelSol(value, isOpen);
					return;
				}
				value.SetTrigger(isOpen ? _fastOpen : _fastClose);
				TryOpenCloseSkirts(elementType, isOpen, isFast: true);
			}
		}

		public void UpdateOpenDoors()
		{
			foreach (KeyValuePair<CarDoorType, bool> item in new Dictionary<CarDoorType, bool>(OpenDoors))
			{
				FastOpenCloseDoor(item.Key, item.Value);
			}
		}

		private void GetDoorState(bool isDoorOpen)
		{
			if ((bool)VehicleData && CarIdStorage.IsCarWithHiddenPoliceLight(VehicleData.CarId))
			{
				VehicleData.OnDoorStateChanged(isDoorOpen);
			}
		}

		private void FastOpenCloseRoofHondaDelSol(Animator roofAnimator, bool isOpen)
		{
			CarAnimationReferences.CarDoorAnimators[CarDoorType.Roof].transform.parent = ((!isOpen) ? CarAnimationReferences.transform : CarAnimationReferences.CarDoorAnimators[CarDoorType.Trunk].transform);
			roofAnimator.SetTrigger(isOpen ? _fastOpen : _fastClose);
			CarAnimationReferences.CarDoorAnimators[CarDoorType.Roof].transform.localPosition = (isOpen ? new Vector3(0f, -0.395f, 0f) : Vector3.zero);
		}

		private void InitOpenDoorsDictionaryIfNeeds()
		{
			Dictionary<CarDoorType, bool> openDoors = OpenDoors;
			if (openDoors == null || openDoors.Count <= 0)
			{
				OpenDoors = new Dictionary<CarDoorType, bool>
				{
					{
						CarDoorType.DriverDoor,
						false
					},
					{
						CarDoorType.PassengerFrontDoor,
						false
					},
					{
						CarDoorType.PassengerBackDoor1,
						false
					},
					{
						CarDoorType.PassengerBackDoor2,
						false
					},
					{
						CarDoorType.Hood,
						false
					},
					{
						CarDoorType.Trunk,
						false
					},
					{
						CarDoorType.Roof,
						false
					},
					{
						CarDoorType.FrontLeftDoorWindow,
						false
					},
					{
						CarDoorType.FrontRightDoorWindow,
						false
					},
					{
						CarDoorType.RoofPlatformHondaDelSol,
						false
					}
				};
			}
		}
	}
}

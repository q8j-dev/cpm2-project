using CarAnimationScripts;
using CarInteractionAnimationsScripts;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.Car.Sounds;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Other.Helpers;
using _Car_Parking.Scripts.Scopes;

namespace _Car_Parking.Scripts.MenuUI
{
	public class LockCarButtonController : MonoBehaviour
	{
		public enum LockStateEnum
		{
			None = 0,
			Unlock = 1,
			Lock = 2
		}

		public const int KickPassengersRpcCode = 58;

		public const int RagdollRpcCode = 59;

		public const int LockCarRpcCode = 22;

		public const int UnLockCarRpcCode = 23;

		private OptionalDependency<MultiPlayer> _multiplayer;

		private LockCarButtonReference _lockCarButtonReference;

		private VehicleData _vehicleToOpen;

		[Inject]
		public void Construct(OptionalDependency<MultiPlayer> multiplayer)
		{
			_multiplayer = multiplayer;
		}

		public void SetButtonReference(LockCarButtonReference lockCarButtonReference)
		{
			_lockCarButtonReference = lockCarButtonReference;
			if (_lockCarButtonReference.TryGetComponent<Button>(out var component))
			{
				component.onClick.AddListener(KickPassengersButtonPressed);
			}
		}

		public bool GetLockButtonActiveState()
		{
			return _lockCarButtonReference.gameObject.activeSelf;
		}

		public void UpdateLockButtonImage(Image image)
		{
			image.sprite = _lockCarButtonReference.GetLockButtonSprite();
		}

		public void ShowKickPassengersButton(VehicleData vehicleData)
		{
			if ((bool)_lockCarButtonReference)
			{
				int num = 5;
				if (V.CurrentCamIndex != num || FlagsStorage.OutOfCar)
				{
					_vehicleToOpen = vehicleData;
					_lockCarButtonReference.gameObject.SetActive(value: true);
					_lockCarButtonReference.ChangeSprite(vehicleData.CarLocked);
				}
			}
		}

		public void HideKickPassengersButton()
		{
			_vehicleToOpen = null;
			if ((bool)_lockCarButtonReference)
			{
				_lockCarButtonReference.gameObject.SetActive(value: false);
			}
		}

		public void SetActiveRoot(bool isActive)
		{
			if ((bool)_lockCarButtonReference)
			{
				_lockCarButtonReference.SetActiveRoot(isActive);
			}
		}

		public void KickPassengersButtonPressed()
		{
			KickPassengersLogic(_vehicleToOpen);
		}

		private void KickPassengersLogic(VehicleData vehicleData)
		{
			if (!vehicleData || vehicleData.gameObject != vehicleData.FreeDriveProvider.FirstCarMultiPlayer)
			{
				return;
			}
			bool num = (bool)V.DriverPerson && V.DriverPerson.TryGetComponentInChildren<PersonAnimationsController>(out var component) && (component.SitGetOutCarSystem.SitGetOutData.IsSittingIntoCarAnimate || component.SitGetOutCarSystem.SitGetOutData.IsGettingOutOfTheCarAnimate);
			bool flag = vehicleData.TryGetComponent<CarAnimationsController>(out var component2) && component2.CarAnimationReferences.IsAnimatingSeats;
			if (!(num | flag))
			{
				if (!vehicleData.CarLocked)
				{
					LockCar(vehicleData);
				}
				else
				{
					UnLockCar(vehicleData);
				}
			}
		}

		private void SyncLockButtonSprite(VehicleData vehicleData)
		{
			if ((bool)_lockCarButtonReference && (bool)vehicleData && vehicleData.FreeDriveProvider != null && vehicleData.gameObject == vehicleData.FreeDriveProvider.FirstCarMultiPlayer)
			{
				_lockCarButtonReference.ChangeSprite(vehicleData.CarLocked);
			}
		}

		public void LockCar(VehicleData vehicleData, bool showMessage = true)
		{
			vehicleData.CarLocked = true;
			SyncLockButtonSprite(vehicleData);
			if (PhotonNetwork.IsConnected && (bool)_multiplayer.Value && !FlagsStorage.InGarageFree)
			{
				_multiplayer.Value.SendValueStr(22, vehicleData.GetComponent<PhotonView>().ViewID.ToString(), "");
			}
			vehicleData.GetComponent<SoundController>().LockCar();
			vehicleData.GetComponent<LightController>().EmergencyLightOnForTime(0.76f);
			if (showMessage)
			{
				V.CoreGameManager.SetMessage(new LocalizedString("LockCarButtonController.YourCarIsLocked"), 3f);
			}
		}

		public void UnLockCar(VehicleData vehicleData, bool showMessage = true)
		{
			vehicleData.CarLocked = false;
			SyncLockButtonSprite(vehicleData);
			if (PhotonNetwork.IsConnected && (bool)_multiplayer.Value && !FlagsStorage.InGarageFree)
			{
				_multiplayer.Value.SendValueStr(23, vehicleData.GetComponent<PhotonView>().ViewID.ToString(), "");
			}
			vehicleData.GetComponent<SoundController>().UnlockCar();
			vehicleData.GetComponent<LightController>().EmergencyLightOnForTime(1.52f);
			if (showMessage)
			{
				V.CoreGameManager.SetMessage(new LocalizedString("LockCarButtonController.YourCarIsUnlocked"), 3f);
			}
		}
	}
}

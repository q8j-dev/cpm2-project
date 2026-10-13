using System.Threading;
using CarAnimationScripts;
using Cysharp.Threading.Tasks;
using RootMotion.FinalIK;
using UnityEngine;
using UnityEngine.Scripting;
using _Car_Parking;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Localization;

namespace CarInteractionAnimationsScripts
{
	[Preserve]
	public class AnimationService
	{
		private const string IgnoreLayerToCheckObstacles = "parktronic";

		private const string PlayerLayerToCheckObstacles = "DriverPerson";

		private const float DistanceToCamera = 3f;

		private const float DoorClearanceRight = 0.5f;

		private const float DoorClearanceLeft = -0.5f;

		private const float DistanceToCheckObstaclesRight = 0.3f;

		private const float DistanceToCheckObstaclesLeft = -0.3f;

		private const string Front = "Front";

		private const string Back = "Back";

		private const float DurationLerpIk = 0.2f;

		private PersonAnimationsController _personAnimationsController;

		private readonly FirstPersonCameraSuspender _firstPersonCameraSuspender;

		public AnimationService(PersonAnimationsController personAnimationsController)
		{
			_personAnimationsController = personAnimationsController;
			_firstPersonCameraSuspender = new FirstPersonCameraSuspender(personAnimationsController);
		}

		public async UniTask<AnimationClip> GetAnimationClip(CarAnimationsController carAnimController, CarAnimationType carAnimationType, CarInteractionAnimationsCache animationCache, string frontOrBack)
		{
			string path = $"anim{carAnimController.VehicleData.CarId}/{frontOrBack}{carAnimationType.ToString()}.anim";
			return await animationCache.GetAnimation(path, carAnimController.ThisCar);
		}

		public float GetDelayToOpenCloseDoor(CarAnimationsController carAnimController, CarAnimationType carAnimationType, int currentPlace)
		{
			string frontOrBackDoor = GetFrontOrBackDoor(currentPlace);
			return carAnimController.CarAnimationReferences.GetDelayOpenCloseDoor(carAnimationType, frontOrBackDoor);
		}

		public async UniTask<float> PlayCarInteractionAnimation(CarAnimationType carAnimationType, int currentPlace, CarAnimationsController carAnimController, CarInteractionAnimationsCache animationCache, CancellationToken cancellationToken)
		{
			CheckerStopAction checkerStopAction = new CheckerStopAction();
			string frontOrBack = GetFrontOrBackDoor(currentPlace);
			bool isMirror = GetIsMirror(carAnimController, currentPlace);
			AnimationClip animationClip = await GetAnimationClip(carAnimController, carAnimationType, animationCache, frontOrBack);
			if (checkerStopAction.CheckStop(_personAnimationsController, carAnimController, cancellationToken) || !animationClip)
			{
				return 0f;
			}
			float num = animationClip.length;
			float num2 = SetAnimationSpeed(carAnimController, carAnimationType, frontOrBack);
			_personAnimationsController.PlayCarInteractionAnimation(animationClip, isMirror);
			if ((bool)carAnimController.CarAnimationReferences.CarWithoutDoorsBodyKit && carAnimController.CarAnimationReferences.CarWithoutDoorsBodyKit.DoorsAnimationsLength.TryGetValue(carAnimationType, out var value))
			{
				num = value.CarAnimationNewLengthFront;
			}
			return num / num2;
		}

		private float SetAnimationSpeed(CarAnimationsController carAnimController, CarAnimationType carAnimationType, string frontOrBack)
		{
			float num = 1f;
			switch (carAnimationType)
			{
			case CarAnimationType.OpenOutside:
				num = ((frontOrBack == "Front") ? carAnimController.CarAnimationReferences.OpenFrontDoorOutsideAnimationSpeed : carAnimController.CarAnimationReferences.OpenBackDoorOutsideAnimationSpeed);
				break;
			case CarAnimationType.SeatIntoCarOutside:
				num = ((frontOrBack == "Front") ? carAnimController.CarAnimationReferences.CloseFrontDoorInsideSeatIntoCarAnimationSpeed : carAnimController.CarAnimationReferences.CloseBackDoorInsideSeatIntoCarAnimationSpeed);
				break;
			case CarAnimationType.OpenInside:
				num = ((frontOrBack == "Front") ? carAnimController.CarAnimationReferences.OpenFrontDoorInsideAnimationSpeed : carAnimController.CarAnimationReferences.OpenBackDoorInsideAnimationSpeed);
				break;
			case CarAnimationType.GetOutOfTheCar:
				num = ((frontOrBack == "Front") ? carAnimController.CarAnimationReferences.CloseFrontDoorOutsideGetOutAnimationSpeed : carAnimController.CarAnimationReferences.CloseBackDoorOutsideGetOutAnimationSpeed);
				break;
			case CarAnimationType.CloseOutside:
				num = ((frontOrBack == "Front") ? carAnimController.CarAnimationReferences.CloseFrontDoorOutsideAnimationSpeed : carAnimController.CarAnimationReferences.CloseBackDoorOutsideAnimationSpeed);
				break;
			}
			_personAnimationsController.Animator.speed = num;
			return num;
		}

		private string GetFrontOrBackDoor(int currentPlace)
		{
			if (currentPlace != 0 && currentPlace != 1)
			{
				return "Back";
			}
			return "Front";
		}

		private bool GetIsMirror(CarAnimationsController carAnimController, int currentPlace)
		{
			if (carAnimController.CarAnimationReferences.IsF1Car)
			{
				Transform parent = _personAnimationsController.transform.parent;
				Transform transform = carAnimController.transform;
				return IsPlayerRightSideFromCar(transform, parent);
			}
			if (!carAnimController.VehicleData.RightHanded)
			{
				return currentPlace == 1 || currentPlace == 3;
			}
			return currentPlace == 0 || currentPlace == 2;
		}

		public bool IsPlayerRightSideFromCar(Transform car, Transform player)
		{
			return car.InverseTransformPoint(player.position).x > 0f;
		}

		public void EnableDisableIk(bool isOn)
		{
			MainPerson component = _personAnimationsController.transform.parent.GetComponent<MainPerson>();
			FullBodyBipedIK ik = component.ik;
			if ((bool)ik)
			{
				component.ik.enabled = isOn;
				if (isOn)
				{
					ik.solver.IKPositionWeight = 1f;
				}
				else
				{
					ik.solver.IKPositionWeight = 0f;
				}
			}
		}

		public async UniTask EnableIkLerp(CancellationToken cancellationToken, CheckerStopAction checkerStopAction, CarAnimationsController carAnimController)
		{
			FullBodyBipedIK ik = _personAnimationsController.transform.parent.GetComponent<MainPerson>().ik;
			if (!ik)
			{
				return;
			}
			ik.enabled = true;
			for (float t = 0f; t < 0.2f; t += Time.deltaTime)
			{
				ik.solver.IKPositionWeight = t / 0.2f;
				await UniTask.Yield(cancellationToken).SuppressCancellationThrow();
				if (checkerStopAction.CheckStop(_personAnimationsController, carAnimController, cancellationToken))
				{
					return;
				}
			}
			ik.solver.IKPositionWeight = 1f;
		}

		public bool CheckObstacles(GameObject car)
		{
			CarAnimationsController component = car.GetComponent<CarAnimationsController>();
			VehicleData component2 = car.GetComponent<VehicleData>();
			if ((bool)component && (bool)component2)
			{
				int num = ((_personAnimationsController.SitGetOutCarSystem.SitGetOutData.CurrentPlace != -1) ? _personAnimationsController.SitGetOutCarSystem.SitGetOutData.CurrentPlace : 0);
				if (_personAnimationsController.SitGetOutCarSystem.SitGetOutData.CurrentPlace >= component.CarAnimationReferences.CountSeats)
				{
					return false;
				}
				CarDoorType carDoorType = (CarDoorType)num;
				if (!component.CarAnimationsPoints || !component.CarAnimationsPoints.TryGetSeatPoint(carDoorType, out var seatPoint))
				{
					return false;
				}
				bool num2 = ((carDoorType == CarDoorType.DriverDoor || carDoorType == CarDoorType.PassengerBackDoor1) && component2.RightHanded) || ((carDoorType == CarDoorType.PassengerFrontDoor || carDoorType == CarDoorType.PassengerBackDoor2) && !component2.RightHanded);
				Vector3 localPosition = seatPoint.localPosition;
				float x = (num2 ? 0.5f : (-0.5f));
				seatPoint.localPosition += new Vector3(x, 0f, 0f);
				Vector3 position = seatPoint.position;
				float x2 = (num2 ? 0.3f : (-0.3f));
				seatPoint.localPosition += new Vector3(x2, 0f, 0f);
				Vector3 normalized = (seatPoint.position - position).normalized;
				seatPoint.localPosition = localPosition;
				int mask = LayerMask.GetMask("parktronic", "DriverPerson");
				if (Physics.Raycast(position, normalized, out var hitInfo, 1f, ~mask) && !hitInfo.collider.isTrigger)
				{
					V.CoreGameManager.SetMessage(new LocalizedString("CarBlock.TheDoorIsBlocked"), 2f);
					return true;
				}
			}
			return false;
		}

		public bool CheckOverTurnedCar(GameObject car)
		{
			Vector3 eulerAngles = car.transform.rotation.eulerAngles;
			float num = Mathf.Repeat(eulerAngles.x, 360f);
			float num2 = Mathf.Repeat(eulerAngles.z, 360f);
			bool num3 = num > 90f && num < 270f;
			bool flag = num2 > 90f && num2 < 270f;
			if (num3 | flag)
			{
				return true;
			}
			return false;
		}

		public void TurnOnOffCollider(GameObject person, bool isOn)
		{
			if (!person)
			{
				return;
			}
			if (person.TryGetComponent<Collider>(out var component))
			{
				component.enabled = isOn;
			}
			if (person.TryGetComponent<Rigidbody>(out var component2))
			{
				if ((bool)V.CoreGameManager && (bool)V.CoreGameManager.CarOutInControl)
				{
					V.CoreGameManager.CarOutInControl.SetPersonKinematicWithIgnoreTrigger(component2, !isOn).Forget();
				}
				else
				{
					component2.isKinematic = !isOn;
				}
			}
		}

		public void SuspendFirstPersonCamera()
		{
			if (FlagsStorage.InGarageFree || FlagsStorage.InGarageMenu)
			{
				WindowScript window = V.CoreGameManager.ManagerUI.GetWindow("MenuControllerHUD");
				if ((bool)window && window is MenuGarageNavigationWindow menuGarageNavigationWindow)
				{
					if ((V.DriverPerson.transform.position - V.CoreGameManager.transform.position).sqrMagnitude < 9f)
					{
						menuGarageNavigationWindow.ChangeCameraPosition();
					}
				}
				else if (FlagsStorage.InGarageFree && !FlagsStorage.OutOfCar && V.CurrentCamIndex == 0 && (bool)V.Car)
				{
					V.CoreGameManager.GetComponent<CarCamerasController>().SetCamera(1, V.Car.transform, newTarget: true);
				}
			}
			else
			{
				_firstPersonCameraSuspender.Suspend();
			}
		}

		public void RestoreFirstPersonCamera()
		{
			_firstPersonCameraSuspender.Restore();
		}

		public bool IsAnimatingPlayer()
		{
			if (!_personAnimationsController.SitGetOutCarSystem.SitGetOutData.IsSittingIntoCarAnimate)
			{
				return _personAnimationsController.SitGetOutCarSystem.SitGetOutData.IsGettingOutOfTheCarAnimate;
			}
			return true;
		}
	}
}

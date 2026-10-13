using System;
using System.Collections.Generic;
using CarAnimationScripts;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using _Car_Parking;
using _Car_Parking.Scripts.Multiplayer.RpcMethods.RpcObjects;
using _Car_Parking.Scripts.Person.Locomotion;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.WorldInteraction;

namespace CarInteractionAnimationsScripts
{
	public class SeatGetOutCarSystem : IDisposable
	{
		private const float MinDistanceToCheckMoveCar = 0.040000003f;

		private const float LocomotionResumeDelay = 1f;

		private PersonAnimationsController _personAnimationsController;

		private FreeDriveProvider _freeDriveProvider;

		public CarInteractionAnimationsCache AnimationCache { get; private set; }

		public SitGetOutData SitGetOutData { get; set; }

		public CarAnimationStopper CarAnimationStopper { get; private set; }

		public OpenCloseCarDoorSystem OpenCloseCarDoorSystem { get; private set; }

		public MultiPlayerSendRpcCarAnimations MultiPlayerSendRpcCarAnimations { get; private set; }

		public SeatGetOutCarSystem(PersonAnimationsController personAnimationsController)
		{
			_personAnimationsController = personAnimationsController;
			AnimationCache = V.CoreGameManager.CarInteractionAnimationsCache;
			_freeDriveProvider = V.CoreGameManager.FreeDriveProvider;
			SitGetOutData = new SitGetOutData();
			CarAnimationStopper = new CarAnimationStopper(_personAnimationsController);
			OpenCloseCarDoorSystem = new OpenCloseCarDoorSystem(_personAnimationsController);
			MultiPlayerSendRpcCarAnimations = new MultiPlayerSendRpcCarAnimations();
		}

		public void Dispose()
		{
		}

		public void SitIntoCarDriving(GameObject car, bool playAnimation = true)
		{
			if (!SitGetOutData.IsSittingIntoCarAnimate && !SitGetOutData.IsGettingOutOfTheCarAnimate)
			{
				if (FlagsStorage.IsMultiFree && car.TryGetComponent<PhotonView>(out var component) && _freeDriveProvider != null && (bool)_freeDriveProvider.MultiPlayer && !FlagsStorage.InGarageFree)
				{
					ResolveDriverLocomotion()?.CancelSprintLock();
					RpcSitToCarRequest.Send(_freeDriveProvider.MultiPlayer, 0, component.CreatorActorNr);
				}
				else
				{
					SitIntoCarDrivingForce(car, playAnimation);
				}
			}
		}

		public void SitIntoCarDrivingForce(GameObject car, bool playAnimation = true)
		{
			if (!SitGetOutData.IsSittingIntoCarAnimate && !SitGetOutData.IsGettingOutOfTheCarAnimate)
			{
				StopWorldInteractionPose();
				ISetupHandler setupHandler = new SeatDrivingSetup(_personAnimationsController, car, playAnimation);
				WalkInputCancelWatcher cancelWatcher = CreateWalkInputCancelWatcher();
				List<IAction> list = new List<IAction>();
				list.Add(CreateMoveToDoorAction(cancelWatcher));
				list.Add(new RotateToDoorAction(_personAnimationsController, isOnlyOpenCloseDoor: false, cancelWatcher));
				if (car.TryGetComponent<CarAnimationsController>(out var component) && (bool)component.CarAnimationReferences && (bool)component.CarAnimationReferences.CarDoorAnimators[CarDoorType.DriverDoor] && !component.CarAnimationReferences.CarWithoutDoorsBodyKit)
				{
					list.Add(new PlayOpenOutsideAnimationAction(_personAnimationsController));
				}
				list.Add(new PlaySeatIntoCarAnimationAction(_personAnimationsController));
				FlagsStorage.IsDoorOpenAnimation = true;
				PlayActions(list, component, setupHandler).Forget();
			}
		}

		public void SitIntoCarPassenger(GameObject car, bool playAnimation = true, int passengerPlace = -1)
		{
			if (SitGetOutData.IsSittingIntoCarAnimate || SitGetOutData.IsGettingOutOfTheCarAnimate)
			{
				return;
			}
			if (FlagsStorage.IsMultiFree && car.TryGetComponent<PhotonView>(out var component) && _freeDriveProvider != null && (bool)_freeDriveProvider.MultiPlayer && !FlagsStorage.InGarageFree)
			{
				if (passengerPlace < 0)
				{
					passengerPlace = _personAnimationsController.SitGetOutCarSystem.GetPlaceForPassenger(car);
				}
				if (passengerPlace < 0)
				{
					if ((bool)V.CoreGameManager && (bool)V.CoreGameManager.CarOutInControl)
					{
						V.CoreGameManager.CarOutInControl.ShowBusyMessage(0);
					}
				}
				else
				{
					ResolveDriverLocomotion()?.CancelSprintLock();
					RpcSitToCarRequest.Send(_freeDriveProvider.MultiPlayer, passengerPlace, component.CreatorActorNr);
				}
			}
			else
			{
				SitIntoCarPassengerForce(car, playAnimation, passengerPlace);
			}
		}

		public void SitIntoCarPassengerForce(GameObject car, bool playAnimation = true, int passengerPlace = -1)
		{
			if (!SitGetOutData.IsSittingIntoCarAnimate && !SitGetOutData.IsGettingOutOfTheCarAnimate)
			{
				StopWorldInteractionPose();
				ISetupHandler setupHandler = new SeatPassengerSetup(_personAnimationsController, car, playAnimation, passengerPlace);
				WalkInputCancelWatcher cancelWatcher = CreateWalkInputCancelWatcher();
				List<IAction> list = new List<IAction>();
				list.Add(CreateMoveToDoorAction(cancelWatcher));
				list.Add(new RotateToDoorAction(_personAnimationsController, isOnlyOpenCloseDoor: false, cancelWatcher));
				if (car.TryGetComponent<CarAnimationsController>(out var component) && (bool)component.CarAnimationReferences && !component.CarAnimationReferences.CarWithoutDoorsBodyKit)
				{
					list.Add(new PlayOpenOutsideAnimationAction(_personAnimationsController));
				}
				list.Add(new PlaySeatIntoCarAnimationAction(_personAnimationsController));
				PlayActions(list, component, setupHandler).Forget();
			}
		}

		public void SitIntoCarOtherPlayer(GameObject car, int place)
		{
			if (!SitGetOutData.IsSittingIntoCarAnimate && !SitGetOutData.IsGettingOutOfTheCarAnimate)
			{
				ISetupHandler setupHandler = new SeatOtherPlayerSetup(_personAnimationsController, car, place);
				List<IAction> list = new List<IAction>();
				list.Add(new MoveToDoorPointAction(_personAnimationsController));
				list.Add(new RotateToDoorAction(_personAnimationsController));
				if (car.TryGetComponent<CarAnimationsController>(out var component) && (bool)component.CarAnimationReferences && component.CarAnimationReferences.CarDoorAnimators[(CarDoorType)place] != null && !component.CarAnimationReferences.CarWithoutDoorsBodyKit)
				{
					list.Add(new PlayOpenOutsideAnimationAction(_personAnimationsController));
				}
				list.Add(new PlaySeatIntoCarAnimationAction(_personAnimationsController));
				PlayActions(list, component, setupHandler).Forget();
			}
		}

		public void GetOutCar(GameObject car, bool playAnimation = true)
		{
			if (!SitGetOutData.IsSittingIntoCarAnimate && !SitGetOutData.IsGettingOutOfTheCarAnimate)
			{
				ISetupHandler setupHandler = new GetOutSetup(_personAnimationsController, car, playAnimation);
				CarAnimationsController component = car.GetComponent<CarAnimationsController>();
				List<IAction> actions = new List<IAction>
				{
					new PlayOpenInsideAnimationAction(_personAnimationsController),
					new PlayGetOutOfTheCarAnimationAction(_personAnimationsController)
				};
				FlagsStorage.IsDoorOpenAnimation = true;
				PlayActions(actions, component, setupHandler).Forget();
			}
		}

		public void GetOutCarOtherPlayer(GameObject car, int place)
		{
			if (!SitGetOutData.IsSittingIntoCarAnimate && !SitGetOutData.IsGettingOutOfTheCarAnimate)
			{
				ISetupHandler setupHandler = new GetOutOtherPlayerSetup(_personAnimationsController, car, place);
				CarAnimationsController component = car.GetComponent<CarAnimationsController>();
				List<IAction> actions = new List<IAction>
				{
					new PlayOpenInsideAnimationAction(_personAnimationsController),
					new PlayGetOutOfTheCarAnimationAction(_personAnimationsController)
				};
				PlayActions(actions, component, setupHandler).Forget();
			}
		}

		public void OpenCloseDoor(GameObject car, bool isDriverDoor)
		{
			if (!SitGetOutData.IsSittingIntoCarAnimate && !SitGetOutData.IsGettingOutOfTheCarAnimate)
			{
				CarAnimationsController component = car.GetComponent<CarAnimationsController>();
				if ((bool)component && !component.CarAnimationReferences.IsF1Car)
				{
					var (actions, setupHandler) = OpenCloseCarDoorSystem.GetActionsOpenCloseDoor(car, isDriverDoor);
					PlayActions(actions, component, setupHandler).Forget();
				}
			}
		}

		public void OpenCloseDoorOtherPlayer(GameObject car, int place, CarAnimationType animationType)
		{
			CarAnimationsController component = car.GetComponent<CarAnimationsController>();
			if ((bool)component)
			{
				var (actions, setupHandler) = OpenCloseCarDoorSystem.GetActionsOpenCloseDoorOtherPlayer(car, place, animationType);
				PlayActions(actions, component, setupHandler).Forget();
			}
		}

		private async UniTask PlayActions(List<IAction> actions, CarAnimationsController car, ISetupHandler setupHandler)
		{
			PersonLocomotion locomotion = ResolveLocomotion();
			RemoteLocomotionDriver remoteDriver = ResolveRemoteDriver();
			bool num = actions.Count > 0 && actions[0] is MoveToDoorPointLocomotionAction;
			bool suspended = false;
			bool canceled = false;
			remoteDriver?.Suspend();
			if (!num)
			{
				locomotion?.Suspend();
				suspended = true;
			}
			try
			{
				setupHandler.Enter();
				foreach (IAction action in actions)
				{
					bool flag = SitGetOutData.IsForceSeatIntoCar || SitGetOutData.IsForceGetOutCar;
					if (((setupHandler.IsBusy || !setupHandler.IsAnimate) | flag) || !car || !_personAnimationsController || SitGetOutData.IsCanceled)
					{
						if (!suspended)
						{
							locomotion?.Suspend();
							suspended = true;
						}
						continue;
					}
					if (SitGetOutData.IsSittingIntoCarAnimate)
					{
						CarAnimationStopper.StartTimerForForceSeat();
					}
					else if (SitGetOutData.IsGettingOutOfTheCarAnimate)
					{
						CarAnimationStopper.StartTimerForForceGetOut();
					}
					await action.Run(car, SitGetOutData.CancellationTokenSource.Token).SuppressCancellationThrow();
					if (!suspended && action is MoveToDoorPointLocomotionAction)
					{
						suspended = true;
					}
				}
				CarAnimationStopper.StopTimerForceSeat();
				CarAnimationStopper.StopTimerForceGetOut();
				if (!suspended)
				{
					locomotion?.Suspend();
					suspended = true;
				}
				canceled = SitGetOutData != null && SitGetOutData.IsCanceled;
				setupHandler.Exit();
				for (int i = 0; i < actions.Count; i++)
				{
					actions[i]?.Dispose();
				}
			}
			finally
			{
				if (SitGetOutData != null)
				{
					SitGetOutData.IsKitPoseSuppressed = false;
				}
				if ((bool)_personAnimationsController && _personAnimationsController.AnimationService != null)
				{
					_personAnimationsController.AnimationService.RestoreFirstPersonCamera();
				}
				remoteDriver?.Resume(canceled ? 0f : 1f);
				if (suspended)
				{
					if (canceled)
					{
						RestorePersonPhysicsForCancel();
					}
					locomotion?.Resume(canceled ? 0f : 1f);
				}
			}
		}

		private RemoteLocomotionDriver ResolveRemoteDriver()
		{
			if (!_personAnimationsController || !_personAnimationsController.transform.parent)
			{
				return null;
			}
			_personAnimationsController.transform.parent.TryGetComponent<RemoteLocomotionDriver>(out var component);
			return component;
		}

		private IAction CreateMoveToDoorAction(WalkInputCancelWatcher cancelWatcher, bool isOnlyOpenCloseDoor = false)
		{
			PersonLocomotion personLocomotion = ResolveDriverLocomotion();
			if (!personLocomotion)
			{
				return new MoveToDoorPointAction(_personAnimationsController, isOnlyOpenCloseDoor, cancelWatcher);
			}
			return new MoveToDoorPointLocomotionAction(_personAnimationsController, personLocomotion, isOnlyOpenCloseDoor, cancelWatcher);
		}

		private void RestorePersonPhysicsForCancel()
		{
			if ((bool)_personAnimationsController && (bool)_personAnimationsController.transform.parent)
			{
				Transform parent = _personAnimationsController.transform.parent;
				if (parent.TryGetComponent<Collider>(out var component))
				{
					component.enabled = true;
				}
				if (parent.TryGetComponent<Rigidbody>(out var component2))
				{
					component2.isKinematic = false;
				}
			}
		}

		private WalkInputCancelWatcher CreateWalkInputCancelWatcher()
		{
			if (!_personAnimationsController || !_personAnimationsController.transform.parent)
			{
				return null;
			}
			if (!(_personAnimationsController.transform.parent.gameObject == V.DriverPerson))
			{
				return null;
			}
			return new WalkInputCancelWatcher(_personAnimationsController);
		}

		private PersonLocomotion ResolveDriverLocomotion()
		{
			if (!FlagsStorage.UseNewLocomotion)
			{
				return null;
			}
			PersonLocomotion personLocomotion = ResolveLocomotion();
			if (!personLocomotion || personLocomotion.gameObject != V.DriverPerson)
			{
				return null;
			}
			return personLocomotion;
		}

		private PersonLocomotion ResolveLocomotion()
		{
			if (!_personAnimationsController || !_personAnimationsController.transform.parent)
			{
				return null;
			}
			_personAnimationsController.transform.parent.TryGetComponent<PersonLocomotion>(out var component);
			return component;
		}

		public async UniTask CheckMoveCar(GameObject car)
		{
			Vector3 startPositionCar = (car ? car.transform.position : Vector3.zero);
			while (SitGetOutData != null && (SitGetOutData.IsSittingIntoCarAnimate || SitGetOutData.IsGettingOutOfTheCarAnimate))
			{
				float num = 0f;
				bool isSittingIntoCarAnimate = SitGetOutData.IsSittingIntoCarAnimate;
				if ((bool)car)
				{
					num = (car.transform.position - startPositionCar).sqrMagnitude;
				}
				if (!car || num > 0.040000003f)
				{
					if (isSittingIntoCarAnimate)
					{
						CarAnimationStopper.ForceSit();
					}
					else
					{
						CarAnimationStopper.ForceGetOut();
					}
					break;
				}
				await UniTask.Yield();
			}
		}

		public int GetPlaceForPassenger(GameObject car)
		{
			VehicleData component = car.GetComponent<VehicleData>();
			CarAnimationsController component2 = car.GetComponent<CarAnimationsController>();
			int result = -1;
			float num = float.MaxValue;
			for (int i = 1; i < component.VehicleMembersController.Members.Count; i++)
			{
				if ((bool)component2 && (bool)component2.CarAnimationReferences)
				{
					if (i < component2.CarAnimationsPoints.SeatPointsToSit.Count && (bool)component2.CarAnimationsPoints.SeatPointsToSit[i] && component.VehicleMembersController.IsPlaceFree(i))
					{
						float sqrMagnitude = (V.DriverPerson.transform.position - component2.CarAnimationsPoints.SeatPointsToSit[i].transform.position).sqrMagnitude;
						if (sqrMagnitude < num)
						{
							result = i;
							num = sqrMagnitude;
						}
					}
				}
				else if (!component.VehicleMembersController.Members[i])
				{
					result = i;
					break;
				}
			}
			return result;
		}

		public int GetDoorPlaceForPassenger(GameObject car)
		{
			VehicleData component = car.GetComponent<VehicleData>();
			CarAnimationsController component2 = car.GetComponent<CarAnimationsController>();
			if (!component || !component2 || !component2.CarAnimationReferences || !component2.CarAnimationsPoints || component2.CarAnimationsPoints.OpenCloseDoorsPointsToMove == null)
			{
				return GetPlaceForPassenger(car);
			}
			List<Transform> openCloseDoorsPointsToMove = component2.CarAnimationsPoints.OpenCloseDoorsPointsToMove;
			int num = Mathf.Min(component2.CarAnimationReferences.CountSeats, openCloseDoorsPointsToMove.Count, component.VehicleMembersController.Members.Count);
			int num2 = -1;
			float num3 = float.MaxValue;
			for (int i = 1; i < num; i++)
			{
				if ((bool)openCloseDoorsPointsToMove[i] && component.VehicleMembersController.IsPlaceFree(i))
				{
					float sqrMagnitude = (V.DriverPerson.transform.position - openCloseDoorsPointsToMove[i].position).sqrMagnitude;
					if (sqrMagnitude < num3)
					{
						num2 = i;
						num3 = sqrMagnitude;
					}
				}
			}
			if (num2 != -1)
			{
				return num2;
			}
			return GetPlaceForPassenger(car);
		}

		private void StopWorldInteractionPose()
		{
			if ((bool)_personAnimationsController)
			{
				WorldInteractionController componentInParent = _personAnimationsController.GetComponentInParent<WorldInteractionController>();
				if ((bool)componentInParent)
				{
					componentInParent.StopForCarEnter();
				}
			}
		}

		public void Subscribe()
		{
			if ((bool)_freeDriveProvider.MultiPlayer)
			{
				_freeDriveProvider.MultiPlayer.OnPlayerConnected += SendIsAnimateToNewPlayer;
			}
		}

		public void Unsubscribe()
		{
			if ((bool)_freeDriveProvider.MultiPlayer)
			{
				_freeDriveProvider.MultiPlayer.OnPlayerConnected -= SendIsAnimateToNewPlayer;
			}
		}

		private void SendIsAnimateToNewPlayer(Player player)
		{
			if ((bool)_freeDriveProvider.MultiPlayer)
			{
				string text = _personAnimationsController.transform.parent.GetComponent<PhotonView>().ViewID.ToString();
				string text2 = SitGetOutData.CurrentCar.PhotonViewCar.ViewID.ToString();
				if (SitGetOutData.IsSittingIntoCarAnimate)
				{
					string name = SitGetOutData.CurrentPlace.ToString();
					_freeDriveProvider.MultiPlayer.SendValueStrToPlayer<RpcSitIntoCar>(text + "," + text2, name, player);
				}
				if (SitGetOutData.IsGettingOutOfTheCarAnimate)
				{
					_freeDriveProvider.MultiPlayer.SendValueStrToPlayer<RpcGetOutCar>(text + "," + text2, SitGetOutData.CurrentPlace.ToString(), player);
				}
			}
		}
	}
}

using System;
using System.Collections.Generic;
using System.Threading;
using CarAnimationScripts;
using CarInteractionAnimationsScripts;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.Infrastructure;

namespace _Car_Parking.Scripts.Racing_Scripts
{
	public class RallyPassengerNpc : MonoBehaviour
	{
		private sealed class MoveNpcToDoorAction : IAction, IDisposable
		{
			private const float MinDistanceSqr = 0.09f;

			private const float WalkSpeed = 1.5f;

			private const float RotateSpeed = 10f;

			private readonly PersonAnimationsController _pac;

			private readonly SitGetOutData _sitData;

			public MoveNpcToDoorAction(PersonAnimationsController pac)
			{
				_pac = pac;
				_sitData = pac.SitGetOutCarSystem.SitGetOutData;
			}

			public void Dispose()
			{
			}

			public async UniTask Run(CarAnimationsController cac, CancellationToken token)
			{
				Transform person = _pac.transform.parent;
				Transform target = cac.CarAnimationsPoints.DoorsPointsToMove[_sitData.CurrentPlace];
				_pac.Animator.applyRootMotion = false;
				_pac.PlayMoveAnimation(0f, 0.2f);
				_pac.SetBoolIsMoving(value: true);
				_pac.Animator.SetBool("walk", value: true);
				while (!token.IsCancellationRequested)
				{
					Vector2 vector = new Vector2(person.position.x, person.position.z);
					if ((new Vector2(target.position.x, target.position.z) - vector).sqrMagnitude <= 0.09f)
					{
						break;
					}
					Vector3 forward = target.position - person.position;
					forward.y = 0f;
					person.rotation = Quaternion.Slerp(person.rotation, Quaternion.LookRotation(forward), Time.deltaTime * 10f);
					person.position += person.forward * (Time.deltaTime * 1.5f);
					await UniTask.Yield(token).SuppressCancellationThrow();
				}
				if ((bool)_pac && (bool)_pac.Animator)
				{
					_pac.PlayMoveAnimation(0f, 0f);
					_pac.SetBoolIsMoving(value: false);
					_pac.Animator.SetBool("walk", value: false);
					person.position = target.position;
					_pac.Animator.applyRootMotion = true;
				}
			}
		}

		private const int PassengerPlace = 1;

		private const float SpawnOffset = 5f;

		[Inject]
		private IPrefabFactory _prefabFactory;

		private GameObject _npc;

		private PersonAnimationsController _pac;

		[SerializeField]
		private NpcSkinChanger NpcSkinChanger;

		public async UniTask SpawnPassengerNpc(GameObject car)
		{
			if (_npc != null)
			{
				return;
			}
			GameObject obj = Resources.Load<GameObject>("FreeParts/Client");
			_npc = _prefabFactory.Instantiate(obj, V.GlobalPoint);
			if (!_npc.GetComponent<StepSounds>())
			{
				_npc.AddComponent<StepSounds>();
			}
			if (_npc.TryGetComponent<Rigidbody>(out var component))
			{
				component.isKinematic = true;
			}
			await UniTask.DelayFrame(2, PlayerLoopTiming.Update, base.destroyCancellationToken).SuppressCancellationThrow();
			if (base.destroyCancellationToken.IsCancellationRequested || !_npc)
			{
				return;
			}
			MainPerson mainPerson = _npc.GetComponent<MainPerson>();
			mainPerson.Gender = 1;
			if (mainPerson.Animator == null)
			{
				mainPerson.LoadRandomSkin(useHelmet: true);
				NpcSkinChanger.Init(mainPerson);
				await UniTask.DelayFrame(1, PlayerLoopTiming.Update, base.destroyCancellationToken).SuppressCancellationThrow();
				if (base.destroyCancellationToken.IsCancellationRequested || !_npc)
				{
					return;
				}
			}
			CarAnimationsController component2 = car.GetComponent<CarAnimationsController>();
			if ((bool)component2 && !(component2.CarAnimationReferences == null) && 1 < component2.CarAnimationReferences.CountSeats)
			{
				_pac = CreatePacForNpc(_npc, mainPerson);
				Transform transform = component2.CarAnimationsPoints.DoorsPointsToMove[1];
				float num = (component2.CarAnimationReferences.RightHanded ? (-1f) : 1f);
				Vector3 vector = component2.transform.right * num;
				_npc.transform.position = transform.position + vector * 5f;
				_npc.transform.rotation = Quaternion.LookRotation(-vector);
				await RunSitAnimationSequenceAsync(_pac, component2, car.GetComponent<VehicleData>());
			}
		}

		public void DespawnPassengerNpc(GameObject car)
		{
			if ((bool)_npc)
			{
				_pac?.SitGetOutCarSystem.SitGetOutData.CancellationTokenSource?.Cancel();
				car.GetComponent<VehicleData>()?.VehicleMembersController.SetMember(1, null);
				_npc.transform.SetParent(null);
				UnityEngine.Object.Destroy(_npc);
				_npc = null;
				_pac = null;
			}
		}

		public void ForcePassengerSeat(GameObject car)
		{
			if (!_npc)
			{
				return;
			}
			if (_pac != null)
			{
				SitGetOutData sitGetOutData = _pac.SitGetOutCarSystem.SitGetOutData;
				_pac.SitGetOutCarSystem.CarAnimationStopper.StopTimerForceSeat();
				sitGetOutData.IsForceSeatIntoCar = true;
				sitGetOutData.CancellationTokenSource?.Cancel();
				if (car.TryGetComponent<CarAnimationsController>(out var component))
				{
					component.FastOpenCloseDoor(CarDoorType.PassengerFrontDoor, isOpen: false);
				}
				_pac.StopAnimation(withoutTransition: true);
				_pac.SetAnimatorCullingModeAlways(value: false);
			}
			if (car.TryGetComponent<DriverController>(out var component2))
			{
				component2.PreSetPerson(_npc, 1);
			}
			MainPerson component3 = _npc.GetComponent<MainPerson>();
			if (component3?.ik != null)
			{
				component3.ik.enabled = true;
				component3.ik.solver.IKPositionWeight = 1f;
			}
		}

		private PersonAnimationsController CreatePacForNpc(GameObject npc, MainPerson mainPerson)
		{
			PersonAnimationsController component = UnityEngine.Object.Instantiate(Resources.Load<GameObject>("PersonAnimationsController/PersonAnimationsController"), npc.transform).GetComponent<PersonAnimationsController>();
			component.Animator = mainPerson.Animator;
			GameObject gameObject = Resources.Load<GameObject>(((int)mainPerson.Gender == 1) ? "MenuPerson/MaleKit/body/Body_Male" : "MenuPerson/FemaleKit/body/Body_Female");
			if (gameObject != null)
			{
				Animator component2 = gameObject.GetComponent<Animator>();
				if (component2 != null)
				{
					if (component2.runtimeAnimatorController != null)
					{
						component.Animator.runtimeAnimatorController = component2.runtimeAnimatorController;
					}
					if (component2.avatar != null)
					{
						component.Animator.avatar = component2.avatar;
					}
				}
			}
			component.Init(null);
			mainPerson.PersonAnimationsController = component;
			if (npc.TryGetComponent<DriverActionController>(out var component3))
			{
				component3.PersonAnimationsController = component;
			}
			return component;
		}

		private async UniTask RunSitAnimationSequenceAsync(PersonAnimationsController pac, CarAnimationsController cac, VehicleData vd)
		{
			SitGetOutData sitData = pac.SitGetOutCarSystem.SitGetOutData;
			GameObject gameObject = pac.transform.parent.gameObject;
			if (pac.AnimationService.CheckOverTurnedCar(cac.gameObject))
			{
				return;
			}
			sitData.CurrentPlace = 1;
			sitData.SetCarSitData(cac.gameObject, 1, isSitting: true);
			using CancellationTokenSource linkedCts = CancellationTokenSource.CreateLinkedTokenSource(sitData.CancellationTokenSource.Token, base.destroyCancellationToken);
			if ((bool)vd.BodyLoadingComposition.LODModel && vd.BodyLoadingComposition.LODModel.activeInHierarchy)
			{
				vd.BodyLoadingComposition.ShowLod(val: false);
			}
			pac.SetAnimatorCullingModeAlways(value: true);
			cac.CarAnimationReferences.CurrentAnimateSeats[1] = gameObject;
			vd.VehicleMembersController.SetMember(1, gameObject);
			pac.AnimationService.TurnOnOffCollider(gameObject, isOn: false);
			pac.SitGetOutCarSystem.CarAnimationStopper.StartTimerForForceSeat();
			List<IAction> actions = new List<IAction>
			{
				new MoveNpcToDoorAction(pac),
				new RotateToDoorAction(pac)
			};
			if (!cac.CarAnimationReferences.CarWithoutDoorsBodyKit)
			{
				actions.Add(new PlayOpenOutsideAnimationAction(pac));
			}
			actions.Add(new PlaySeatIntoCarAnimationAction(pac));
			bool wasForced = false;
			foreach (IAction item in actions)
			{
				if (sitData.IsCanceled || sitData.IsForceSeatIntoCar || linkedCts.Token.IsCancellationRequested)
				{
					wasForced = sitData.IsForceSeatIntoCar && !sitData.IsCanceled;
					break;
				}
				await item.Run(cac, linkedCts.Token).SuppressCancellationThrow();
			}
			foreach (IAction item2 in actions)
			{
				item2?.Dispose();
			}
			if ((bool)cac)
			{
				sitData.SetCarOpenCloseDoorOutsideExitData(isOpenClose: false);
			}
			if ((bool)pac)
			{
				pac.SitGetOutCarSystem.CarAnimationStopper.StopTimerForceSeat();
			}
			if ((bool)pac)
			{
				pac.StopAnimation(withoutTransition: true);
			}
			if ((bool)pac)
			{
				pac.SetAnimatorCullingModeAlways(value: false);
			}
			if ((bool)cac)
			{
				cac.CarAnimationReferences.CurrentAnimateSeats[1] = null;
			}
			if (wasForced && !base.destroyCancellationToken.IsCancellationRequested && (bool)_npc && (bool)cac)
			{
				ForcePassengerSeat(cac.gameObject);
			}
		}
	}
}

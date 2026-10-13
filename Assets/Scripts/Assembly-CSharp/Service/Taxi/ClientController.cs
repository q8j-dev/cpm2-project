using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using RootMotion;
using RootMotion.FinalIK;
using Service.Abstract;
using UnityEngine;

namespace Service.Taxi
{
	public class ClientController : Service.Abstract.ClientController
	{
		[SerializeField]
		[Header("Monitoring")]
		private GameObject groundLight;

		[SerializeField]
		private Animator animator;

		[SerializeField]
		private AudioClip clip;

		private Queue<CancellationTokenSource> tokens = new Queue<CancellationTokenSource>();

		private CancellationTokenSource shashkaToken = new CancellationTokenSource();

		private LayerMask mask;

		private FullBodyBipedIK _frozenLegsIk;

		private Transform[] _frozenLegBones;

		private Quaternion[] _frozenLegRotations;

		private CancellationTokenSource _legFreezeToken;

		private void Awake()
		{
			animator = GetComponent<Animator>();
			UpdateShashka().Forget();
		}

		private async UniTaskVoid UpdateShashka()
		{
			while (!owner || !car)
			{
				if (shashkaToken.IsCancellationRequested)
				{
					return;
				}
				await UniTask.Delay(TimeSpan.FromMilliseconds(100.0));
			}
			while (!shashkaToken.IsCancellationRequested && !(owner == null))
			{
				TaxiController taxiController = owner?.GetComponent<TaxiController>();
				GameObject gameObject = taxiController?.Shashka;
				if ((bool)gameObject)
				{
					Transform transform = taxiController?.GetPoliceTransform(car?.transform);
					transform?.gameObject.SetActive(value: false);
					gameObject?.SetActive(value: true);
					gameObject?.transform.SetPositionAndRotation(transform.position, (car?.transform.rotation).Value);
				}
				await UniTask.Delay(TimeSpan.FromMilliseconds(10.0));
			}
		}

		private async UniTask LookAt(Transform car, CLIENT_STATE state)
		{
			if (tokens.Count > 0)
			{
				tokens.Dequeue().Cancel();
			}
			CancellationTokenSource token = new CancellationTokenSource();
			tokens.Enqueue(token);
			while ((bool)car && !token.IsCancellationRequested)
			{
				Vector3 euler = new Vector3(0f, Quaternion.LookRotation(car.position - base.transform.position).eulerAngles.y, 0f);
				base.transform.rotation = Quaternion.Lerp(base.transform.rotation, Quaternion.Euler(euler), Time.deltaTime * 1f);
				if (state == CLIENT_STATE.GO_TO_CAR)
				{
					if (car.GetComponent<VehicleData>().Driver != owner)
					{
						ChangeState(CLIENT_STATE.WAIT).Forget();
						WaitForPlayer().Forget();
					}
					if ((int)mask == 0)
					{
						mask = LayerMask.GetMask("Default") | LayerMask.GetMask("Road");
					}
					if (Physics.Raycast(base.transform.position + Vector3.up * 2.5f, Vector3.down, out var hitInfo, 5.5f, mask))
					{
						base.transform.Translate(Vector3.up * (2.5f - hitInfo.distance));
					}
				}
				await UniTask.Delay(TimeSpan.FromMilliseconds(10.0));
			}
		}

		private async UniTaskVoid WaitForPlayer()
		{
			while ((bool)this && (bool)car && GetState() == CLIENT_STATE.WAIT && car.GetComponent<VehicleData>().Driver != owner)
			{
				await UniTask.Delay(TimeSpan.FromMilliseconds(10.0));
			}
			if ((bool)this && (bool)car && GetState() == CLIENT_STATE.WAIT)
			{
				ChangeState(CLIENT_STATE.GO_TO_CAR).Forget();
			}
		}

		public void Walk()
		{
			if (ClientStateTransitions.IsAllowed(GetState(), CLIENT_STATE.GO_TO_CAR))
			{
				if (V.Car.GetComponent<Rigidbody>().velocity.magnitude > 2f)
				{
					ChangeState(CLIENT_STATE.NONE).Forget();
				}
				ChangeState(CLIENT_STATE.GO_TO_CAR).Forget();
			}
		}

		public void StopLookAt()
		{
			while (tokens.Count > 0)
			{
				tokens.Dequeue().Cancel();
			}
		}

		public void OnDestroy()
		{
			shashkaToken.Cancel();
			StopLookAt();
			SetLegAnimationFrozen(frozen: false);
			try
			{
				owner.GetComponent<TaxiController>().Shashka.SetActive(value: false);
				VehicleData vehicleData = car?.GetComponent<VehicleData>();
				if (vehicleData.LightsComposition.IsPolice)
				{
					vehicleData.LightsComposition.IsTaxi = false;
					vehicleData.LightsComposition.MakePoliceCar();
				}
			}
			catch (Exception)
			{
			}
		}

		public async UniTaskVoid ChangeState(CLIENT_STATE state, bool animate = true)
		{
			if (!ClientStateTransitions.IsAllowed(GetState(), state))
			{
				return;
			}
			ResetAnimation();
			base.ChangeState(state);
			SetLegAnimationFrozen(state == CLIENT_STATE.SIT_IN);
			if (state != CLIENT_STATE.NONE)
			{
				LookAt(car.transform, state);
			}
			PhotonView photonView = PhotonView.Get(Global.TaskManager);
			while (V.DriverPerson == null)
			{
				await UniTask.Delay(TimeSpan.FromMilliseconds(10.0));
			}
			PhotonView photonView2 = PhotonView.Get(V.DriverPerson);
			if (PhotonNetwork.IsConnected && V.DriverPerson == owner)
			{
				byte b = (byte)((animate ? 1 : 0) | ((int)state << 1));
				photonView.RpcToDefaultGroup("Taxi_ChangeState", RpcTarget.Others, photonView2.ViewID, b);
			}
			if (!animate)
			{
				return;
			}
			switch (state)
			{
			case CLIENT_STATE.NONE:
				StopLookAt();
				break;
			case CLIENT_STATE.SIT_IN:
				StopLookAt();
				if (V.DriverPerson == owner)
				{
					Global.TaskManager.Task.SetState(TASK_STATE.START);
				}
				break;
			case CLIENT_STATE.WAIT:
				animator?.SetBool("wait", value: true);
				break;
			case CLIENT_STATE.GO_TO_CAR:
				animator?.SetBool("walk", value: true);
				break;
			case CLIENT_STATE.GO_OUT:
				animator?.SetBool("realwalk", value: true);
				break;
			}
		}

		private void SetLegAnimationFrozen(bool frozen)
		{
			_legFreezeToken?.Cancel();
			_legFreezeToken = null;
			if (frozen)
			{
				_legFreezeToken = new CancellationTokenSource();
				FreezeLegsWhenIkReady(_legFreezeToken.Token).Forget();
			}
			else
			{
				UnfreezeLegs();
			}
		}

		private async UniTaskVoid FreezeLegsWhenIkReady(CancellationToken cancellationToken)
		{
			while ((bool)this && !cancellationToken.IsCancellationRequested)
			{
				FullBodyBipedIK componentInChildren = GetComponentInChildren<FullBodyBipedIK>();
				if ((bool)componentInChildren)
				{
					FreezeLegs(componentInChildren);
					break;
				}
				if (await UniTask.Delay(TimeSpan.FromMilliseconds(50.0), ignoreTimeScale: false, PlayerLoopTiming.Update, cancellationToken).SuppressCancellationThrow())
				{
					break;
				}
			}
		}

		private void FreezeLegs(FullBodyBipedIK ik)
		{
			UnfreezeLegs();
			BipedReferences references = ik.references;
			_frozenLegBones = new Transform[6] { references.leftThigh, references.leftCalf, references.leftFoot, references.rightThigh, references.rightCalf, references.rightFoot };
			_frozenLegRotations = new Quaternion[_frozenLegBones.Length];
			for (int i = 0; i < _frozenLegBones.Length; i++)
			{
				if ((bool)_frozenLegBones[i])
				{
					_frozenLegRotations[i] = _frozenLegBones[i].localRotation;
				}
			}
			_frozenLegsIk = ik;
			IKSolverFullBodyBiped solver = ik.solver;
			solver.OnPreRead = (IKSolver.UpdateDelegate)Delegate.Combine(solver.OnPreRead, new IKSolver.UpdateDelegate(ApplyFrozenLegPose));
		}

		private void ApplyFrozenLegPose()
		{
			if (_frozenLegBones == null)
			{
				return;
			}
			for (int i = 0; i < _frozenLegBones.Length; i++)
			{
				if ((bool)_frozenLegBones[i])
				{
					_frozenLegBones[i].localRotation = _frozenLegRotations[i];
				}
			}
		}

		private void UnfreezeLegs()
		{
			if ((bool)_frozenLegsIk)
			{
				IKSolverFullBodyBiped solver = _frozenLegsIk.solver;
				solver.OnPreRead = (IKSolver.UpdateDelegate)Delegate.Remove(solver.OnPreRead, new IKSolver.UpdateDelegate(ApplyFrozenLegPose));
			}
			_frozenLegsIk = null;
			_frozenLegBones = null;
			_frozenLegRotations = null;
		}

		public void ResetAnimation()
		{
			if (!(animator == null))
			{
				animator.SetBool("walk", value: false);
				animator.SetBool("wait", value: false);
				animator.SetBool("realwalk", value: false);
			}
		}

		public override void Set(GameObject owner, GameObject car)
		{
			base.Set(owner, car);
			SetInternal(owner);
		}

		private void SetInternal(GameObject owner)
		{
			if (!PhotonNetwork.IsConnected || PhotonView.Get(owner).IsMine)
			{
				GameObject original = Resources.Load<GameObject>("FreeParts/Triggers");
				groundLight = UnityEngine.Object.Instantiate(original, base.transform);
			}
		}
	}
}

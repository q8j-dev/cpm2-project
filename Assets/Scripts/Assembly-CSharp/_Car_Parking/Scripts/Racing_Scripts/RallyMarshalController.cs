using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using MTAssets.SkinnedMeshCombiner;
using UnityEngine;

namespace _Car_Parking.Scripts.Racing_Scripts
{
	public class RallyMarshalController : MonoBehaviour
	{
		[SerializeField]
		private Transform GoAwayTarget;

		[SerializeField]
		private Animator MarshalAnimator;

		[SerializeField]
		private float MoveSpeed = 1.5f;

		[SerializeField]
		private SkinnedMeshCombiner SkinnedMeshCombiner;

		private static readonly int WalkHash = Animator.StringToHash("walk");

		private static readonly int StartHash = Animator.StringToHash("start");

		private static readonly int StopHash = Animator.StringToHash("stop");

		private static readonly int PrepareStartHash = Animator.StringToHash("prepareStart");

		private Vector3 _roadPosition;

		private Quaternion _roadRotation;

		private CancellationTokenSource _goAwayCts;

		private Collider _collider;

		public void SetRoadPosition(Vector3 pos, Quaternion rot)
		{
			_roadPosition = pos;
			_roadRotation = rot;
		}

		private void Start()
		{
			if ((bool)SkinnedMeshCombiner)
			{
				SkinnedMeshCombiner.CombineMeshes();
			}
			EnsureCollider();
		}

		private Collider EnsureCollider()
		{
			if (_collider == null)
			{
				_collider = base.gameObject.GetComponentInChildren<Collider>(includeInactive: true);
			}
			return _collider;
		}

		public void ResetToInitialPosition()
		{
			Collider collider = EnsureCollider();
			if ((bool)collider)
			{
				collider.enabled = true;
			}
			_goAwayCts?.Cancel();
			_goAwayCts?.Dispose();
			_goAwayCts = null;
			base.transform.SetPositionAndRotation(_roadPosition, _roadRotation);
			MarshalAnimator.applyRootMotion = true;
			MarshalAnimator.SetBool(WalkHash, value: false);
			MarshalAnimator.SetBool(StartHash, value: false);
			MarshalAnimator.SetBool(StopHash, value: false);
			MarshalAnimator.SetBool(PrepareStartHash, value: false);
			MarshalAnimator.Play("Cw01_Ontrack_work03", 0, 0f);
		}

		public async UniTaskVoid GoAway()
		{
			_goAwayCts?.Cancel();
			_goAwayCts?.Dispose();
			_goAwayCts = new CancellationTokenSource();
			CancellationTokenSource linkedCts = CancellationTokenSource.CreateLinkedTokenSource(_goAwayCts.Token, this.GetCancellationTokenOnDestroy());
			Collider collider = EnsureCollider();
			if ((bool)collider)
			{
				collider.enabled = false;
			}
			if (await UniTask.Delay(TimeSpan.FromSeconds(1.0), ignoreTimeScale: false, PlayerLoopTiming.Update, linkedCts.Token).SuppressCancellationThrow())
			{
				linkedCts.Dispose();
				return;
			}
			MarshalAnimator.applyRootMotion = false;
			MarshalAnimator.SetBool(WalkHash, value: true);
			MarshalAnimator.Play("Walk", 0, 0f);
			while (!linkedCts.Token.IsCancellationRequested)
			{
				Vector3 vector = new Vector3(GoAwayTarget.position.x, base.transform.position.y, GoAwayTarget.position.z);
				base.transform.LookAt(vector);
				base.transform.position = Vector3.MoveTowards(base.transform.position, vector, MoveSpeed * Time.deltaTime);
				if (Vector3.Distance(base.transform.position, vector) < 0.05f)
				{
					SetAnimation("walk", state: false);
					base.transform.rotation = GoAwayTarget.transform.rotation;
					break;
				}
				await UniTask.Yield(PlayerLoopTiming.Update, linkedCts.Token);
			}
			linkedCts.Dispose();
		}

		private void OnDestroy()
		{
			_goAwayCts?.Cancel();
			_goAwayCts?.Dispose();
		}

		public void PlayAnimation(string animName)
		{
			MarshalAnimator.Play(animName, 0, 0f);
		}

		public void SetAnimation(string trigger, bool state, bool ignoreState = false)
		{
			if (state && !ignoreState)
			{
				MarshalAnimator.SetBool(WalkHash, value: false);
				MarshalAnimator.SetBool(StartHash, value: false);
				MarshalAnimator.SetBool(StopHash, value: false);
				MarshalAnimator.SetBool(PrepareStartHash, value: false);
			}
			MarshalAnimator.SetBool(Animator.StringToHash(trigger), state);
		}
	}
}

using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Car_Parking.Scripts.Racing_Scripts.RallyCrowd
{
	public class CPMCrowdNpc : MonoBehaviour
	{
		[SerializeField]
		private Animator Animator;

		[SerializeField]
		private CPMCrowdNpcConfig Config;

		private CancellationTokenSource _cts;

		private Vector3 _animatorLocalPosition;

		private Quaternion _animatorLocalRotation;

		public int PrefabIndex { get; private set; }

		private void Awake()
		{
			if (!Animator)
			{
				Animator = GetComponentInChildren<Animator>();
			}
			Animator.cullingMode = Config.CullingMode;
			Animator.applyRootMotion = false;
			Transform transform = Animator.transform;
			_animatorLocalPosition = transform.localPosition;
			_animatorLocalRotation = transform.localRotation;
		}

		public void OnSpawn(int prefabIndex)
		{
			PrefabIndex = prefabIndex;
			Transform transform = Animator.transform;
			if (transform != base.transform)
			{
				transform.SetLocalPositionAndRotation(_animatorLocalPosition, _animatorLocalRotation);
			}
			bool startAsAction = UnityEngine.Random.value < Config.StartAsActionChance && Config.Actions != null && Config.Actions.Length != 0;
			CancelLoop();
			_cts = new CancellationTokenSource();
			RunLoop(startAsAction, _cts.Token).Forget();
		}

		public void OnDespawn()
		{
			CancelLoop();
		}

		private void OnDestroy()
		{
			CancelLoop();
		}

		private void CancelLoop()
		{
			CommonExtensions.CancelAndDisposeToken(ref _cts);
		}

		private async UniTaskVoid RunLoop(bool startAsAction, CancellationToken ct)
		{
			_ = 2;
			try
			{
				if (startAsAction && TryGetRandomAction(out var action))
				{
					float value = UnityEngine.Random.value;
					Animator.Play(action.StateName, 0, value);
					await UniTask.Delay(TimeSpan.FromSeconds(action.Duration * (1f - value)), ignoreTimeScale: false, PlayerLoopTiming.Update, ct);
					Animator.CrossFadeInFixedTime(Config.IdleStateName, Config.CrossFadeDuration);
				}
				else
				{
					Animator.Play(Config.IdleStateName, 0, UnityEngine.Random.value);
				}
				while (true)
				{
					await UniTask.Delay(TimeSpan.FromSeconds(NextIdleDelay()), ignoreTimeScale: false, PlayerLoopTiming.Update, ct);
					if (TryGetRandomAction(out var action2))
					{
						Animator.CrossFadeInFixedTime(action2.StateName, Config.CrossFadeDuration);
						await UniTask.Delay(TimeSpan.FromSeconds(action2.Duration), ignoreTimeScale: false, PlayerLoopTiming.Update, ct);
						Animator.CrossFadeInFixedTime(Config.IdleStateName, Config.CrossFadeDuration);
					}
				}
			}
			catch (OperationCanceledException)
			{
			}
		}

		private bool TryGetRandomAction(out CPMCrowdNpcConfig.ActionClip action)
		{
			CPMCrowdNpcConfig.ActionClip[] actions = Config.Actions;
			if (actions == null || actions.Length == 0)
			{
				action = default;
				return false;
			}
			action = actions[UnityEngine.Random.Range(0, actions.Length)];
			return true;
		}

		private float NextIdleDelay()
		{
			return UnityEngine.Random.Range(Config.IdleDelayRange.x, Config.IdleDelayRange.y);
		}

		private void SetRefs()
		{
			Animator = GetComponentInChildren<Animator>();
		}
	}
}

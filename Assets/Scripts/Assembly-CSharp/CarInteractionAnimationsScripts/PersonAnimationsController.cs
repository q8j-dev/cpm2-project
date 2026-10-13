using System;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using _Car_Parking;

namespace CarInteractionAnimationsScripts
{
	public class PersonAnimationsController : MonoBehaviour
	{
		private const float RareMoveAnimationChance = 0.05f;

		private readonly int _playSpecialAnimationTrigger = Animator.StringToHash("PlaySpecialAnimation");

		private readonly int _stopAnimationTrigger = Animator.StringToHash("StopAnimation");

		private readonly int _stopAnimationWithoutTransitionTrigger = Animator.StringToHash("StopAnimationWithoutTransition");

		private readonly int _playCarInteraction = Animator.StringToHash("PlayCarInteractionWithoutTransition");

		private readonly int _playCarInteractionMirror = Animator.StringToHash("PlayCarInteractionWithoutTransitionMirror");

		private readonly int _movePlayerXAnimation = Animator.StringToHash("X");

		private readonly int _movePlayerZAnimation = Animator.StringToHash("Z");

		private readonly int _offset = Animator.StringToHash("offset");

		private readonly int _isMovePlayerAnimation = Animator.StringToHash("IsMoving");

		private readonly int _playOneShot = Animator.StringToHash("PlayOneShot");

		private readonly int _playOneShotWithoutTransition = Animator.StringToHash("PlayOneShotWithoutTransition");

		private readonly int _specialAnimationsState = Animator.StringToHash("SpecialAnimations");

		private readonly int _oneShotState = Animator.StringToHash("OneShot");

		private readonly int _carInteractionState = Animator.StringToHash("CarInteractionAnimation");

		private readonly int _carInteractionMirrorState = Animator.StringToHash("CarInteractionAnimationMirror");

		[SerializeField]
		private AnimationDataSO SpecialAnimationsData;

		private AnimatorOverrideController _overrideController;

		private IDisposable _currentSpecialAnimationDisposable;

		private NewMoveController _newMoveController;

		private bool _isMoving;

		private bool _isSpecialAnimating;

		private bool _isDestroyed;

		public AnimationDataSO AnimationData => SpecialAnimationsData;

		public bool IsThereMoveAnimations => !SpecialAnimationsData.StartFirstSetupPersonAnimations.IsNullOrEmpty();

		public int CurrentSpecialAnimationId { get; private set; }

		public bool IsCurrentSpecialAnimationLooping
		{
			get
			{
				if (CurrentSpecialAnimationId != 0)
				{
					return _currentSpecialAnimationDisposable == null;
				}
				return false;
			}
		}

		public bool IsEmoteAnimating
		{
			get
			{
				if (CurrentSpecialAnimationId == 0)
				{
					return InSpecialAnimationsState();
				}
				return true;
			}
		}

		public Vector3 ScalePersonOutCar { get; private set; } = new Vector3(1f, 1f, 1f);

		public Animator Animator { get; set; }

		public SeatGetOutCarSystem SitGetOutCarSystem { get; private set; }

		public AnimationService AnimationService { get; private set; }

		public bool IsOneShotAnimating { get; set; }

		private void Awake()
		{
			InitIdleAnimation().Forget();
		}

		public void Init(NewMoveController newMoveController)
		{
			SitGetOutCarSystem = new SeatGetOutCarSystem(this);
			AnimationService = new AnimationService(this);
			_newMoveController = newMoveController;
		}

		public void SetBoolIsMoving(bool value)
		{
			_isMoving = value;
			Animator.SetBool(_isMovePlayerAnimation, value);
		}

		public void PlayMoveAnimation(float x, float z)
		{
			Animator.SetFloat(_movePlayerXAnimation, x);
			Animator.SetFloat(_movePlayerZAnimation, z);
		}

		public void StopMoveAnimation()
		{
			Animator.SetFloat(_movePlayerXAnimation, 0f);
			Animator.SetFloat(_movePlayerZAnimation, 0f);
		}

		public void PlaySpecialAnimation(int id)
		{
			if (id == 0 || CurrentSpecialAnimationId == id || SitGetOutCarSystem.SitGetOutData.IsSittingIntoCarAnimate || SitGetOutCarSystem.SitGetOutData.IsGettingOutOfTheCarAnimate)
			{
				return;
			}
			AnimationDataSO.AnimationDataItem byY = SpecialAnimationsData.GetByY(id);
			if (byY == null || byY.animationClip == null)
			{
				return;
			}
			AnimationClip animationClip = byY.animationClip;
			float duration = byY.duration;
			CurrentSpecialAnimationId = id;
			if (!_overrideController)
			{
				_overrideController = new AnimatorOverrideController(Animator.runtimeAnimatorController);
				Animator.runtimeAnimatorController = _overrideController;
			}
			_overrideController["SpecialAnimations"] = animationClip;
			_isSpecialAnimating = true;
			Animator.ResetTrigger(_stopAnimationTrigger);
			Animator.ResetTrigger(_stopAnimationWithoutTransitionTrigger);
			if (InSpecialAnimationsState())
			{
				Animator.ResetTrigger(_playSpecialAnimationTrigger);
				Animator.Play(_specialAnimationsState, 0, 0f);
			}
			else
			{
				Animator.SetTrigger(_playSpecialAnimationTrigger);
			}
			if (_currentSpecialAnimationDisposable != null)
			{
				_currentSpecialAnimationDisposable.Dispose();
				_currentSpecialAnimationDisposable = null;
			}
			if (!animationClip.isLooping)
			{
				_currentSpecialAnimationDisposable = Observable.Timer(TimeSpan.FromSeconds(duration)).Subscribe((long _) =>
				{
					StopSpecialAnimation();
				}).AddTo(this);
			}
		}

		public void StopSpecialAnimation()
		{
			if (_currentSpecialAnimationDisposable != null)
			{
				_currentSpecialAnimationDisposable.Dispose();
				_currentSpecialAnimationDisposable = null;
			}
			if ((bool)_newMoveController)
			{
				_newMoveController.Input.Emotion = 0;
			}
			CurrentSpecialAnimationId = 0;
			_isSpecialAnimating = false;
			if ((bool)Animator)
			{
				Animator.speed = 1f;
			}
			if (InSpecialAnimationsState() || SpecialEntryPending())
			{
				StopAnimation();
			}
		}

		public void StopAnimation(bool withoutTransition = false)
		{
			if ((bool)Animator)
			{
				Animator.speed = 1f;
				if (InEmoteState() || AnyEmoteEntryPending())
				{
					Animator.ResetTrigger(_playSpecialAnimationTrigger);
					int trigger = (withoutTransition ? _stopAnimationWithoutTransitionTrigger : _stopAnimationTrigger);
					Animator.SetTrigger(trigger);
				}
			}
		}

		private bool AnimatorStatePollable()
		{
			if ((bool)Animator && (bool)Animator.runtimeAnimatorController)
			{
				return Animator.isActiveAndEnabled;
			}
			return false;
		}

		private bool SpecialEntryPending()
		{
			if (AnimatorStatePollable())
			{
				return Animator.GetBool(_playSpecialAnimationTrigger);
			}
			return false;
		}

		private bool AnyEmoteEntryPending()
		{
			if (!AnimatorStatePollable())
			{
				return false;
			}
			if (!Animator.GetBool(_playSpecialAnimationTrigger) && !Animator.GetBool(_playOneShot) && !Animator.GetBool(_playOneShotWithoutTransition) && !Animator.GetBool(_playCarInteraction))
			{
				return Animator.GetBool(_playCarInteractionMirror);
			}
			return true;
		}

		private bool StateIsAnyEmote(int shortNameHash)
		{
			if (shortNameHash != _specialAnimationsState && shortNameHash != _oneShotState && shortNameHash != _carInteractionState)
			{
				return shortNameHash == _carInteractionMirrorState;
			}
			return true;
		}

		private bool InEmoteState()
		{
			if (!AnimatorStatePollable())
			{
				return false;
			}
			if (StateIsAnyEmote(Animator.GetCurrentAnimatorStateInfo(0).shortNameHash))
			{
				return true;
			}
			if (Animator.IsInTransition(0))
			{
				return StateIsAnyEmote(Animator.GetNextAnimatorStateInfo(0).shortNameHash);
			}
			return false;
		}

		private bool InSpecialAnimationsState()
		{
			if (!AnimatorStatePollable())
			{
				return false;
			}
			if (Animator.GetCurrentAnimatorStateInfo(0).shortNameHash == _specialAnimationsState)
			{
				return true;
			}
			if (Animator.IsInTransition(0))
			{
				return Animator.GetNextAnimatorStateInfo(0).shortNameHash == _specialAnimationsState;
			}
			return false;
		}

		public void SetAnimatorCullingModeAlways(bool value)
		{
			Animator.applyRootMotion = true;
			Animator.cullingMode = ((!value) ? AnimatorCullingMode.CullCompletely : AnimatorCullingMode.AlwaysAnimate);
		}

		public void SetFloatOffset(float value)
		{
			Animator.SetFloat(_offset, value);
		}

		public void PlayCarInteractionAnimation(AnimationClip anim, bool isMirror)
		{
			if ((bool)anim)
			{
				if (!_overrideController)
				{
					_overrideController = new AnimatorOverrideController(Animator.runtimeAnimatorController);
					Animator.runtimeAnimatorController = _overrideController;
				}
				_overrideController["CarInteractionAnimation"] = anim;
				_overrideController["CarInteractionAnimationMirror"] = anim;
				if (isMirror)
				{
					Animator.SetTrigger(_playCarInteractionMirror);
				}
				else
				{
					Animator.SetTrigger(_playCarInteraction);
				}
			}
		}

		public void EnableAnimator()
		{
			if (!SitGetOutCarSystem.SitGetOutData.IsSittingIntoCarAnimate && !SitGetOutCarSystem.SitGetOutData.IsGettingOutOfTheCarAnimate)
			{
				Animator.enabled = true;
			}
		}

		public void DisableAnimator()
		{
			if (!SitGetOutCarSystem.SitGetOutData.IsSittingIntoCarAnimate && !SitGetOutCarSystem.SitGetOutData.IsGettingOutOfTheCarAnimate)
			{
				Animator.enabled = false;
			}
		}

		public async UniTask InitIdleAnimation()
		{
			if (!SpecialAnimationsData.OneShotIdleAnimations.IsNullOrEmpty())
			{
				while (!_isDestroyed)
				{
					await UniTask.Delay(TimeSpan.FromSeconds(UnityEngine.Random.Range(5f, 10f)));
					await PlayRandomIdleAnimation();
				}
			}
		}

		private async UniTask PlayRandomIdleAnimation()
		{
			if ((bool)Animator && !AnimationService.IsAnimatingPlayer() && !_isMoving && !_isSpecialAnimating && FlagsStorage.OutOfCar && !IsOneShotAnimating && !SitGetOutCarSystem.SitGetOutData.IsIKPoseActive && (bool)SpecialAnimationsData && !SpecialAnimationsData.OneShotIdleAnimations.IsNullOrEmpty())
			{
				int index = UnityEngine.Random.Range(0, SpecialAnimationsData.OneShotIdleAnimations.Count - 1);
				AnimationClip animClip = (SpecialAnimationsData.OneShotIdleAnimations.IsNullOrEmpty() ? null : SpecialAnimationsData.OneShotIdleAnimations[index]);
				await UniTask.Delay(TimeSpan.FromSeconds(PlayOneShotAnimation(animClip)));
				IsOneShotAnimating = false;
			}
		}

		public float PlayRandomMoveOneShotAnimation(bool withTransition = true)
		{
			if (SpecialAnimationsData.StartFirstSetupPersonAnimations.IsNullOrEmpty())
			{
				return -1f;
			}
			AnimationClip animClip = SpecialAnimationsData.StartFirstSetupPersonAnimations[0];
			if (UnityEngine.Random.value <= 0.05f && SpecialAnimationsData.StartFirstSetupPersonAnimations.Count > 1)
			{
				animClip = SpecialAnimationsData.StartFirstSetupPersonAnimations[UnityEngine.Random.Range(1, SpecialAnimationsData.StartFirstSetupPersonAnimations.Count - 1)];
			}
			return PlayOneShotAnimation(animClip, withTransition);
		}

		public float PlayOneShotAnimation(AnimationClip animClip, bool withTransition = true)
		{
			if (!animClip)
			{
				return -1f;
			}
			IsOneShotAnimating = true;
			if (!_overrideController)
			{
				_overrideController = new AnimatorOverrideController(Animator.runtimeAnimatorController);
				Animator.runtimeAnimatorController = _overrideController;
			}
			_overrideController["OneShot"] = animClip;
			int trigger = (withTransition ? _playOneShot : _playOneShotWithoutTransition);
			Animator.ResetTrigger(_stopAnimationTrigger);
			Animator.ResetTrigger(_stopAnimationWithoutTransitionTrigger);
			Animator.SetTrigger(trigger);
			return animClip.length;
		}

		public void ResetOneShotTrigger()
		{
			Animator?.ResetTrigger(_playOneShot);
			Animator?.ResetTrigger(_playOneShotWithoutTransition);
		}

		private void OnDestroy()
		{
			if (FlagsStorage.IsMultiFree)
			{
				SitGetOutCarSystem.Unsubscribe();
			}
			SitGetOutCarSystem?.Dispose();
			_isDestroyed = true;
		}
	}
}

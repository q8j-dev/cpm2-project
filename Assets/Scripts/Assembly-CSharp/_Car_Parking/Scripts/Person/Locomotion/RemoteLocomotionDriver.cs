using System.Collections.Generic;
using CarInteractionAnimationsScripts;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using UnityEngine;
using _Car_Parking.Scripts.Multiplayer.DataPack;
using _Car_Parking.Scripts.Person.Locomotion.Config;

namespace _Car_Parking.Scripts.Person.Locomotion
{
	[RequireComponent(typeof(MainPerson))]
	public class RemoteLocomotionDriver : MonoBehaviour
	{
		public static bool UseLightweight;

		[SerializeField]
		private PersonLocomotionConfig _config;

		private MainPerson _mainPerson;

		private PersonLocomotion _locomotion;

		private MultiPersonControl _mpc;

		private PhotonView _photonView;

		private PersonAnimationsController _animations;

		private Animator _animator;

		private bool _setupDone;

		private readonly HashSet<int> _paramHashes = new HashSet<int>();

		private PersonLocoState _state;

		private bool _hasState;

		private bool _prevJumping;

		private const float RagdollRetryInterval = 0.5f;

		private float _ragdollRetry;

		private int _lastStartedEmotion;

		private int _lastGait;

		private float _fallingDuration;

		private float _idleDwellTimer;

		private float _nextBreakDelay;

		private PersonKitAnimationLayer[] _kitLayers;

		private const int MaxParamRebuildAttempts = 10;

		private int _paramRebuildAttempts;

		private int _suspendCount;

		private float _resumeDelayTimer;

		private RuntimeAnimatorController _legacyBase;

		private bool _legacyCaptured;

		private bool _releasedForCar;

		public bool IsSuspended
		{
			get
			{
				if (_suspendCount <= 0)
				{
					return _resumeDelayTimer > 0f;
				}
				return true;
			}
		}

		private void Awake()
		{
			TryGetComponent<MainPerson>(out _mainPerson);
			TryGetComponent<PersonLocomotion>(out _locomotion);
			TryGetComponent<MultiPersonControl>(out _mpc);
			TryGetComponent<PhotonView>(out _photonView);
			_kitLayers = GetComponents<PersonKitAnimationLayer>();
			if (!PhotonNetwork.IsConnected || ((bool)_photonView && _photonView.IsMine))
			{
				base.enabled = false;
			}
		}

		public void ApplyState(in PersonLocoState state)
		{
			_state = state;
			_hasState = true;
		}

		public void ForceResetup()
		{
			_setupDone = false;
			_paramRebuildAttempts = 0;
		}

		private RuntimeAnimatorController ResolveTargetController()
		{
			if (!_locomotion)
			{
				return null;
			}
			bool flag = (bool)_mainPerson && (int)_mainPerson.Gender == 1;
			RuntimeAnimatorController result = (flag ? _locomotion.MaleController : _locomotion.FemaleController);
			if (!UseLightweight)
			{
				return result;
			}
			RuntimeAnimatorController runtimeAnimatorController = (flag ? _locomotion.MaleControllerLite : _locomotion.FemaleControllerLite);
			if (!runtimeAnimatorController)
			{
				return result;
			}
			return runtimeAnimatorController;
		}

		public void Suspend()
		{
			_suspendCount++;
			_resumeDelayTimer = 0f;
			ReleaseToLegacyController();
		}

		public void Resume(float delaySeconds)
		{
			if (_suspendCount > 0)
			{
				_suspendCount--;
			}
			if (_suspendCount <= 0)
			{
				_resumeDelayTimer = Mathf.Max(delaySeconds, 0f);
				_fallingDuration = 0f;
				_lastGait = 0;
				_prevJumping = false;
				_idleDwellTimer = 0f;
				_nextBreakDelay = 0f;
			}
		}

		private void ReleaseToLegacyController()
		{
			if (!_legacyCaptured || !_animator)
			{
				return;
			}
			if (_animator.runtimeAnimatorController is AnimatorOverrideController animatorOverrideController)
			{
				if ((object)animatorOverrideController.runtimeAnimatorController != _legacyBase)
				{
					animatorOverrideController.runtimeAnimatorController = _legacyBase;
				}
			}
			else if ((object)_animator.runtimeAnimatorController != _legacyBase)
			{
				_animator.runtimeAnimatorController = _legacyBase;
			}
			_animator.applyRootMotion = true;
			if (_animator.updateMode != AnimatorUpdateMode.Normal)
			{
				_animator.updateMode = AnimatorUpdateMode.Normal;
			}
			_setupDone = false;
			_paramRebuildAttempts = 0;
		}

		private bool IsRemoteActive()
		{
			if (!_hasState || _photonView == null || _photonView.IsMine)
			{
				return false;
			}
			if (_mpc != null && (bool)_mpc.currentCar)
			{
				return false;
			}
			if (IsSuspended)
			{
				return false;
			}
			return true;
		}

		private void LateUpdate()
		{
			if (_resumeDelayTimer > 0f)
			{
				_resumeDelayTimer -= Time.deltaTime;
			}
			if ((bool)_mpc && (bool)_mpc.currentCar)
			{
				if (!_releasedForCar)
				{
					_releasedForCar = true;
					ReleaseToLegacyController();
				}
				return;
			}
			_releasedForCar = false;
			if (IsRemoteActive() && ResolveAnimator())
			{
				EnsureSetupOnce();
				ApplyRagdoll();
				ApplyAnimator();
				ApplyEmotion();
			}
		}

		private bool ResolveAnimator()
		{
			Animator animator = null;
			if ((bool)_mainPerson)
			{
				_animations = _mainPerson.PersonAnimationsController;
				animator = (((bool)_animations && (bool)_animations.Animator) ? _animations.Animator : _mainPerson.Animator);
			}
			if ((object)animator != _animator)
			{
				_animator = animator;
				_setupDone = false;
			}
			return _animator;
		}

		private void EnsureSetupOnce()
		{
			if (_setupDone || !_locomotion)
			{
				return;
			}
			RagdollCPM ragdollCPM = (((bool)_mpc && (bool)_mpc.driverActionController) ? _mpc.driverActionController.ragdollCpm : null);
			if ((bool)ragdollCPM && (ragdollCPM.isRagdoll || ragdollCPM.isStadningUp))
			{
				return;
			}
			if (_locomotion.enabled)
			{
				_locomotion.enabled = false;
			}
			RuntimeAnimatorController runtimeAnimatorController = ResolveTargetController();
			if (!runtimeAnimatorController)
			{
				return;
			}
			RuntimeAnimatorController runtimeAnimatorController2 = _animator.runtimeAnimatorController;
			if (runtimeAnimatorController2 is AnimatorOverrideController animatorOverrideController)
			{
				if ((object)animatorOverrideController.runtimeAnimatorController != runtimeAnimatorController)
				{
					if (!_legacyCaptured)
					{
						_legacyBase = animatorOverrideController.runtimeAnimatorController;
						_legacyCaptured = true;
					}
					animatorOverrideController.runtimeAnimatorController = runtimeAnimatorController;
				}
			}
			else if ((object)runtimeAnimatorController2 != runtimeAnimatorController)
			{
				if (!_legacyCaptured)
				{
					_legacyBase = runtimeAnimatorController2;
					_legacyCaptured = true;
				}
				_animator.runtimeAnimatorController = runtimeAnimatorController;
			}
			if (_animator.updateMode != AnimatorUpdateMode.Normal)
			{
				_animator.updateMode = AnimatorUpdateMode.Normal;
			}
			_animator.applyRootMotion = false;
			_animator.cullingMode = AnimatorCullingMode.CullCompletely;
			for (int i = 1; i < _animator.layerCount; i++)
			{
				_animator.SetLayerWeight(i, 0f);
			}
			RebuildParamHashes();
			if (_paramHashes.Count == 0 && _paramRebuildAttempts < 10)
			{
				_paramRebuildAttempts++;
				return;
			}
			if (_paramHashes.Count == 0)
			{
				Debug.LogWarning($"RemoteLocomotionDriver: controller '{runtimeAnimatorController.name}' exposed no animator parameters after {10} rebuild attempts on {base.name}");
			}
			_paramRebuildAttempts = 0;
			_setupDone = true;
		}

		private void RebuildParamHashes()
		{
			_paramHashes.Clear();
			AnimatorControllerParameter[] parameters = _animator.parameters;
			for (int i = 0; i < parameters.Length; i++)
			{
				_paramHashes.Add(parameters[i].nameHash);
			}
		}

		private void SetFloatSafe(int hash, float value)
		{
			if (_paramHashes.Contains(hash))
			{
				_animator.SetFloat(hash, value);
			}
		}

		private void SetBoolSafe(int hash, bool value)
		{
			if (_paramHashes.Contains(hash))
			{
				_animator.SetBool(hash, value);
			}
		}

		private void SetIntSafe(int hash, int value)
		{
			if (_paramHashes.Contains(hash))
			{
				_animator.SetInteger(hash, value);
			}
		}

		private void SetTriggerSafe(int hash)
		{
			if (_paramHashes.Contains(hash))
			{
				_animator.SetTrigger(hash);
			}
		}

		private void ApplyAnimator()
		{
			bool flag = _state.Has(PersonNetFlags.Grounded);
			bool flag2 = _state.Has(PersonNetFlags.Jumping);
			bool flag3 = _state.Has(PersonNetFlags.Crouching);
			bool num = flag3 && _state.Has(PersonNetFlags.Sliding);
			_fallingDuration = (flag ? 0f : (_fallingDuration + Time.deltaTime));
			int num2 = (_lastGait = RemoteGait.Derive(walkSpeed: _config ? _config.WalkSpeed : 1.4f, runSpeed: _config ? _config.RunSpeed : 2.5f, sprintSpeed: _config ? _config.SprintSpeed : 6f, moveSpeed: _state.MoveSpeed, lastGait: _lastGait, crouching: flag3));
			int num3 = (num ? 3 : num2);
			bool flag4 = flag && _state.MoveSpeed >= 0.05f;
			SetIntSafe(PersonAnimatorParams.CurrentGait, num3);
			SetFloatSafe(PersonAnimatorParams.MoveSpeed, _state.MoveSpeed);
			SetFloatSafe(PersonAnimatorParams.FallingDuration, _fallingDuration);
			SetFloatSafe(PersonAnimatorParams.IsStrafing, 0f);
			SetFloatSafe(PersonAnimatorParams.StrafeDirectionX, 0f);
			SetFloatSafe(PersonAnimatorParams.StrafeDirectionZ, 1f);
			SetFloatSafe(PersonAnimatorParams.ForwardStrafe, 1f);
			SetBoolSafe(PersonAnimatorParams.IsGrounded, flag && !flag2);
			SetBoolSafe(PersonAnimatorParams.IsCrouching, flag3);
			SetBoolSafe(PersonAnimatorParams.IsWalking, num3 == 1);
			SetBoolSafe(PersonAnimatorParams.IsSprinting, num3 == 3);
			SetBoolSafe(PersonAnimatorParams.IsStopped, !flag4);
			SetBoolSafe(PersonAnimatorParams.MovementInputHeld, flag4);
			SetBoolSafe(PersonAnimatorParams.MovementInputPressed, flag4);
			SetBoolSafe(PersonAnimatorParams.MovementInputTapped, value: false);
			SetBoolSafe(PersonAnimatorParams.IsStarting, _state.Has(PersonNetFlags.Starting));
			SetBoolSafe(PersonAnimatorParams.IsQuickTurning, _state.Has(PersonNetFlags.QuickTurning));
			SetBoolSafe(PersonAnimatorParams.QuickTurnLeft, _state.Has(PersonNetFlags.QuickTurnLeft));
			SetBoolSafe(PersonAnimatorParams.IsJumping, flag2);
			if (flag2 && !_prevJumping)
			{
				SetTriggerSafe(PersonAnimatorParams.DoJump);
			}
			_prevJumping = flag2;
			UpdateIdleBreaks(flag, flag2, flag3, num3, flag4);
		}

		private void UpdateIdleBreaks(bool grounded, bool jumping, bool crouching, int gait, bool moving)
		{
			if (_config == null || !_config.IdleBreaksEnabled)
			{
				return;
			}
			if (AnyKitWorn())
			{
				_idleDwellTimer = 0f;
				_nextBreakDelay = 0f;
				return;
			}
			if (!grounded || moving || jumping || crouching || gait != 0 || _state.Has(PersonNetFlags.Starting) || _state.Has(PersonNetFlags.QuickTurning))
			{
				_idleDwellTimer = 0f;
				_nextBreakDelay = 0f;
				return;
			}
			if (_animator.IsInTransition(0) || !_animator.GetCurrentAnimatorStateInfo(0).IsName("Idle_Standing"))
			{
				_idleDwellTimer = 0f;
				return;
			}
			if (_nextBreakDelay <= 0f)
			{
				_nextBreakDelay = Random.Range(_config.IdleBreakFirstDelayMin, _config.IdleBreakFirstDelayMax);
			}
			_idleDwellTimer += Time.deltaTime;
			if (!(_idleDwellTimer < _nextBreakDelay))
			{
				int maxExclusive = Mathf.Max(1, _config.IdleBreakCount);
				SetIntSafe(PersonAnimatorParams.IdleBreakIndex, Random.Range(0, maxExclusive));
				SetTriggerSafe(PersonAnimatorParams.PlayIdleBreak);
				_idleDwellTimer = 0f;
				_nextBreakDelay = Random.Range(_config.IdleBreakMinInterval, _config.IdleBreakMaxInterval);
			}
		}

		private bool AnyKitWorn()
		{
			if (_kitLayers == null)
			{
				return false;
			}
			for (int i = 0; i < _kitLayers.Length; i++)
			{
				if ((bool)_kitLayers[i] && _kitLayers[i].IsWorn)
				{
					return true;
				}
			}
			return false;
		}

		private void ApplyRagdoll()
		{
			RagdollCPM ragdollCPM = (((bool)_mpc && (bool)_mpc.driverActionController) ? _mpc.driverActionController.ragdollCpm : null);
			if (ragdollCPM == null)
			{
				return;
			}
			if (_state.Has(PersonNetFlags.Ragdoll))
			{
				if (!ragdollCPM.isRagdoll)
				{
					_ragdollRetry -= Time.deltaTime;
					if (_ragdollRetry <= 0f)
					{
						ragdollCPM.RagdollEnabler(Vector3.zero).Forget();
						_ragdollRetry = 0.5f;
					}
				}
			}
			else
			{
				_ragdollRetry = 0f;
				if (ragdollCPM.isRagdoll && !ragdollCPM.isStadningUp)
				{
					ragdollCPM.DisableRagdoll();
				}
			}
		}

		private void ApplyEmotion()
		{
			if (!(_animations == null))
			{
				if (ShouldStartSpecialAnimation(_state.Emotion, _lastStartedEmotion))
				{
					_lastStartedEmotion = _state.Emotion;
					_animations.PlaySpecialAnimation(_state.Emotion);
				}
				else if (_state.Emotion == 0 && ShouldStopSpecialAnimation(_state.Emotion, _animations.CurrentSpecialAnimationId, _animations.IsCurrentSpecialAnimationLooping, _state.MoveSpeed, _config ? _config.WalkSpeed : 1.4f))
				{
					_animations.StopSpecialAnimation();
				}
				if (_state.Emotion == 0)
				{
					_lastStartedEmotion = 0;
				}
			}
		}

		public static bool ShouldStartSpecialAnimation(int emotion, int lastStartedEmotion)
		{
			if (emotion != 0)
			{
				return emotion != lastStartedEmotion;
			}
			return false;
		}

		public static bool ShouldStopSpecialAnimation(int emotion, int currentSpecialAnimationId, bool isLooping, float moveSpeed, float walkSpeed)
		{
			if (emotion != 0 || currentSpecialAnimationId == 0)
			{
				return false;
			}
			if (!isLooping)
			{
				return moveSpeed > walkSpeed * 0.5f;
			}
			return true;
		}
	}
}

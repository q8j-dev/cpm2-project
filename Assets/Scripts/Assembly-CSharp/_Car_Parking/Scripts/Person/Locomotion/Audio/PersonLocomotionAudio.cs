using CarInteractionAnimationsScripts;
using Cysharp.Threading.Tasks;
using UnityEngine;
using _Car_Parking.Scripts.Audio;
using _Car_Parking.Scripts.Audio.SoundBanks;
using _Car_Parking.Scripts.Person.Locomotion.Config;

namespace _Car_Parking.Scripts.Person.Locomotion.Audio
{
	public class PersonLocomotionAudio
	{
		private static readonly uint FootstepWalk = AudioWrapper.GetId("Play_Footstep_Type_Surface");

		private static readonly uint FootstepTurn = AudioWrapper.GetId("Play_Scuffsteps_Type_Surface");

		private static readonly uint FootstepCrawl = AudioWrapper.GetId("Play_Footstep_Crawl_Surface");

		private static readonly uint FootstepCrawlIn = AudioWrapper.GetId("Play_Footstep_Crawl_In_Surface");

		private static readonly uint FootstepCrawlOut = AudioWrapper.GetId("Play_Footstep_Crawl_Out_Surface");

		private static readonly uint FootstepJump = AudioWrapper.GetId("Play_Footstep_Jump_Surface");

		private static readonly uint FootstepLand = AudioWrapper.GetId("Play_Footstep_Land_Surface");

		private static readonly uint FootstepSlide = AudioWrapper.GetId("Play_Footsteps_Slide_Surface");

		private static readonly uint Clothes = AudioWrapper.GetId("Play_P_L_Clothes");

		private static readonly uint Vocalization = AudioWrapper.GetId("Play_P_Voice_Vocalization");

		private static readonly uint GenderGroup = AudioWrapper.GetId("P_Voice_Gender");

		private static readonly uint GenderMale = AudioWrapper.GetId("male");

		private static readonly uint GenderFemale = AudioWrapper.GetId("female");

		private static readonly uint VocalizationGroup = AudioWrapper.GetId("P_Voice_Vocalizations");

		private static readonly uint VocalizationEffort = AudioWrapper.GetId("Effort");

		private static readonly uint VocalizationShoutHappy = AudioWrapper.GetId("Shout_Happy");

		private static readonly uint PlayerSpeedRtpc = AudioWrapper.GetId("RTPC_Player_Speed");

		private readonly Transform _transform;

		private readonly MainPerson _mainPerson;

		private readonly PersonLocomotionState _state;

		private readonly PersonLocomotionConfig _config;

		private readonly SoundBankLoadService _bankService;

		private readonly LocomotionSurfaceResolver _surface = new LocomotionSurfaceResolver();

		private FootPlantDetector _leftDetector;

		private FootPlantDetector _rightDetector;

		private Animator _animator;

		private Transform _leftFoot;

		private Transform _rightFoot;

		private Transform _torso;

		private Transform _head;

		private static readonly int StandToCrouchHash = Animator.StringToHash("Stand_ToCrouch");

		private static readonly int CrouchToStandHash = Animator.StringToHash("Crouch_ToStand");

		private static readonly int SprintToCrouchHash = Animator.StringToHash("Sprint_ToCrouch");

		private const float MinAirTimeForLanding = 0.15f;

		private const float FallShoutMinDescentTime = 0.5f;

		private const bool DiagFootsteps = false;

		private const bool DiagCrouchAudio = false;

		private bool _fallShoutFired;

		private bool _wasJumping;

		private bool _wasGrounded = true;

		private int _lastStateHash;

		private bool _diagWasCrouching;

		private float _lastFallingDuration;

		private Vector3 _previousRootPosition;

		private bool _hasPreviousRootPosition;

		public PersonLocomotionAudio(Transform transform, MainPerson mainPerson, PersonLocomotionState state, PersonLocomotionConfig config, SoundBankLoadService bankService)
		{
			_transform = transform;
			_mainPerson = mainPerson;
			_state = state;
			_config = config;
			_bankService = bankService;
		}

		public void Tick()
		{
			if (_state != null && !(_config == null) && _bankService != null && _state.IsActive && ResolveRig())
			{
				UpdateFallShout(_state.Snapshot);
				if (_config.FootstepAudioEnabled)
				{
					UpdateDiscreteEvents(_state.Snapshot);
				}
			}
		}

		public void PhysicsTick()
		{
			if (_state != null && !(_config == null) && _bankService != null && _config.FootstepAudioEnabled && _state.IsActive && ResolveRig())
			{
				UpdateFootsteps(_state.Snapshot, Time.fixedDeltaTime);
			}
		}

		public void Stop()
		{
			_leftDetector?.Reset();
			_rightDetector?.Reset();
			_wasJumping = false;
			_wasGrounded = true;
			_lastStateHash = 0;
			_lastFallingDuration = 0f;
			_hasPreviousRootPosition = false;
			_fallShoutFired = false;
		}

		private void UpdateFallShout(LocomotionStateSnapshot snapshot)
		{
			if (snapshot.IsGrounded)
			{
				_fallShoutFired = false;
			}
			else if (!_fallShoutFired && !(snapshot.DescendingDuration < 0.5f))
			{
				_fallShoutFired = true;
				if (_bankService.TryTakeFallShoutSlot())
				{
					PostVocalization(VocalizationShoutHappy);
				}
			}
		}

		private void UpdateDiscreteEvents(LocomotionStateSnapshot snapshot)
		{
			if (_config.FootstepAudioEnabled)
			{
				if (snapshot.IsJumping && !_wasJumping)
				{
					PostFootstep(FootstepJump, snapshot, LowerFoot());
					PostVocalization(VocalizationEffort);
				}
				if (snapshot.IsGrounded && !_wasGrounded && _lastFallingDuration >= 0.15f)
				{
					PostFootstep(FootstepLand, snapshot, LowerFoot());
				}
				UpdateCrouchStateEvents(snapshot);
			}
			_wasJumping = snapshot.IsJumping;
			_wasGrounded = snapshot.IsGrounded;
			_lastFallingDuration = snapshot.FallingDuration;
		}

		private void UpdateCrouchStateEvents(LocomotionStateSnapshot snapshot)
		{
			int num = ResolveActiveStateHash();
			if (num != _lastStateHash)
			{
				_lastStateHash = num;
				if (num == SprintToCrouchHash)
				{
					PostFootstep(FootstepSlide, snapshot, LowerFoot(), _config.PlayerSpeedRtpcMax);
					PostVocalization(VocalizationEffort);
				}
				else if (num == StandToCrouchHash)
				{
					PostFootstep(FootstepCrawlIn, snapshot, LowerFoot());
				}
				else if (num == CrouchToStandHash)
				{
					PostFootstep(FootstepCrawlOut, snapshot, LowerFoot());
				}
			}
		}

		private static bool IsCrouchRelatedState(int hash)
		{
			if (hash != StandToCrouchHash && hash != CrouchToStandHash)
			{
				return hash == SprintToCrouchHash;
			}
			return true;
		}

		private static string StateName(int hash)
		{
			if (hash == StandToCrouchHash)
			{
				return "Stand_ToCrouch";
			}
			if (hash == CrouchToStandHash)
			{
				return "Crouch_ToStand";
			}
			if (hash == SprintToCrouchHash)
			{
				return "Sprint_ToCrouch";
			}
			return hash.ToString();
		}

		private static string GaitName(int gait)
		{
			return (Gait)gait switch
			{
				Gait.Walk => "Walk", 
				Gait.Run => "Run", 
				Gait.Sprint => "Sprint", 
				_ => "Idle", 
			};
		}

		private static string EventName(uint soundEvent)
		{
			if (soundEvent == FootstepWalk)
			{
				return "Play_Footstep_Walk_Type_Surface";
			}
			if (soundEvent == FootstepTurn)
			{
				return "Play_Footstep_Turn_Type_Surface";
			}
			if (soundEvent == FootstepCrawl)
			{
				return "Play_Footstep_Crawl_Surface";
			}
			if (soundEvent == FootstepCrawlIn)
			{
				return "Play_Footstep_Crawl_In_Surface";
			}
			if (soundEvent == FootstepCrawlOut)
			{
				return "Play_Footstep_Crawl_Out_Surface";
			}
			if (soundEvent == FootstepJump)
			{
				return "Play_Footstep_Jump_Surface";
			}
			if (soundEvent == FootstepLand)
			{
				return "Play_Footstep_Land_Surface";
			}
			if (soundEvent == FootstepSlide)
			{
				return "Play_Footsteps_Slide_Surface";
			}
			return soundEvent.ToString();
		}

		private int ResolveActiveStateHash()
		{
			if (!_animator.IsInTransition(0))
			{
				return _animator.GetCurrentAnimatorStateInfo(0).shortNameHash;
			}
			return _animator.GetNextAnimatorStateInfo(0).shortNameHash;
		}

		private void UpdateFootsteps(LocomotionStateSnapshot snapshot, float deltaTime)
		{
			GroundInfo ground = _state.Ground;
			if (!snapshot.IsGrounded || snapshot.IsJumping || snapshot.IsSliding || !ground.Grounded)
			{
				_leftDetector.Reset();
				_rightDetector.Reset();
				_hasPreviousRootPosition = false;
				return;
			}
			if (snapshot.Gait == 0 && !snapshot.IsQuickTurning && !snapshot.IsTurningInPlace)
			{
				_leftDetector.Reset();
				_rightDetector.Reset();
				_hasPreviousRootPosition = false;
				return;
			}
			float referenceSpeed = ResolveReferenceSpeed(deltaTime);
			uint soundEvent = ResolveCycleEvent(snapshot);
			bool num = _leftDetector.Tick(_leftFoot.position, ground.Point, ground.Normal, referenceSpeed, deltaTime);
			bool flag = _rightDetector.Tick(_rightFoot.position, ground.Point, ground.Normal, referenceSpeed, deltaTime);
			if (num)
			{
				PostFootstep(soundEvent, snapshot, _leftFoot);
			}
			if (flag)
			{
				PostFootstep(soundEvent, snapshot, _rightFoot);
			}
		}

		private float ResolveReferenceSpeed(float deltaTime)
		{
			Vector3 position = _transform.position;
			float a = 0f;
			if (_hasPreviousRootPosition && deltaTime > 0f)
			{
				Vector3 vector = position - _previousRootPosition;
				vector.y = 0f;
				a = vector.magnitude / deltaTime;
			}
			_previousRootPosition = position;
			_hasPreviousRootPosition = true;
			return Mathf.Max(a, _config.FootReferenceMinSpeed);
		}

		private static uint ResolveCycleEvent(LocomotionStateSnapshot snapshot)
		{
			if (snapshot.IsCrouching)
			{
				return FootstepCrawl;
			}
			if (!snapshot.IsQuickTurning)
			{
				return FootstepWalk;
			}
			return FootstepTurn;
		}

		private void PostFootstep(uint soundEvent, LocomotionStateSnapshot snapshot, Transform foot, float speedOverride = -1f)
		{
			if ((bool)foot)
			{
				GameObject gameObject = foot.gameObject;
				float in_value = ((speedOverride >= 0f) ? speedOverride : PlayerSpeed(snapshot));
				AudioWrapper.SetSwitch(_surface.SwitchGroup, _surface.Resolve(foot.position), gameObject);
				AudioWrapper.SetRTPCValue(PlayerSpeedRtpc, in_value, gameObject);
				_bankService.PostEvent(soundEvent, SoundBankType.SfxMain, gameObject).Forget();
				if (_config.ClothesWithFootsteps && (bool)_torso)
				{
					_bankService.PostEvent(Clothes, SoundBankType.SfxMain, _torso.gameObject).Forget();
				}
			}
		}

		private void PostVocalization(uint vocalization)
		{
			if ((bool)_head)
			{
				GameObject gameObject = _head.gameObject;
				ApplyGender(gameObject);
				AudioWrapper.SetSwitch(VocalizationGroup, vocalization, gameObject);
				_bankService.PostEvent(Vocalization, SoundBankType.CpmSoundBank, gameObject).Forget();
			}
		}

		private void ApplyGender(GameObject emitter)
		{
			bool flag = (bool)_mainPerson && (int)_mainPerson.Gender == 1;
			AudioWrapper.SetSwitch(GenderGroup, flag ? GenderMale : GenderFemale, emitter);
		}

		private float PlayerSpeed(LocomotionStateSnapshot snapshot)
		{
			return Mathf.Clamp(snapshot.MoveSpeed, 0f, _config.PlayerSpeedRtpcMax);
		}

		private FootPlantDetector CreateDetector()
		{
			return new FootPlantDetector(_config.FootPlantSpeedRatio, _config.FootLiftSpeedRatio, _config.FootGroundedMaxHeight, _config.FootstepMinInterval);
		}

		private Transform LowerFoot()
		{
			if (!_leftFoot)
			{
				return _rightFoot;
			}
			if (!_rightFoot)
			{
				return _leftFoot;
			}
			if (!(_leftFoot.position.y <= _rightFoot.position.y))
			{
				return _rightFoot;
			}
			return _leftFoot;
		}

		private bool ResolveRig()
		{
			Animator animator = ResolveAnimator();
			if (!animator || !animator.isHuman)
			{
				return false;
			}
			if ((object)animator != _animator)
			{
				_animator = animator;
				_leftFoot = null;
				_rightFoot = null;
				_torso = null;
				_head = null;
			}
			if (!_leftFoot || !_rightFoot)
			{
				_leftFoot = animator.GetBoneTransform(HumanBodyBones.LeftFoot);
				_rightFoot = animator.GetBoneTransform(HumanBodyBones.RightFoot);
				if (!_leftFoot || !_rightFoot)
				{
					return false;
				}
				WiseHelper.SetAudioWiseComponents(_leftFoot.gameObject, addColliderTrigger: false);
				WiseHelper.SetAudioWiseComponents(_rightFoot.gameObject, addColliderTrigger: false);
				_leftDetector = CreateDetector();
				_rightDetector = CreateDetector();
			}
			if (!_torso)
			{
				Transform boneTransform = animator.GetBoneTransform(HumanBodyBones.Chest);
				if (!boneTransform)
				{
					boneTransform = animator.GetBoneTransform(HumanBodyBones.Spine);
				}
				_torso = (boneTransform ? boneTransform : _transform);
				WiseHelper.SetAudioWiseComponents(_torso.gameObject, addColliderTrigger: false);
			}
			if (!_head)
			{
				Transform boneTransform2 = animator.GetBoneTransform(HumanBodyBones.Head);
				_head = (boneTransform2 ? boneTransform2 : _torso);
				WiseHelper.SetAudioWiseComponents(_head.gameObject, addColliderTrigger: false);
			}
			return true;
		}

		private Animator ResolveAnimator()
		{
			if (!_mainPerson)
			{
				return null;
			}
			PersonAnimationsController personAnimationsController = _mainPerson.PersonAnimationsController;
			if (!personAnimationsController || !personAnimationsController.Animator)
			{
				return _mainPerson.Animator;
			}
			return personAnimationsController.Animator;
		}
	}
}

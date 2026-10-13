using CarInteractionAnimationsScripts;
using Cysharp.Threading.Tasks;
using UnityEngine;
using _Car_Parking.Scripts.Audio;
using _Car_Parking.Scripts.Audio.SoundBanks;
using _Car_Parking.Scripts.Person.Locomotion.Config;
using _Car_Parking.Scripts.Person.Locomotion.Stamina;

namespace _Car_Parking.Scripts.Person.Locomotion.Audio
{
	public class PersonBreathAudio
	{
		private static readonly uint BreathPlay = AudioWrapper.GetId("Play_Breath");

		private static readonly uint BreathStop = AudioWrapper.GetId("Stop_Breath");

		private static readonly uint StaminaRtpc = AudioWrapper.GetId("RTPC_Stamina");

		private static readonly uint GenderGroup = AudioWrapper.GetId("P_Voice_Gender");

		private static readonly uint GenderMale = AudioWrapper.GetId("male");

		private static readonly uint GenderFemale = AudioWrapper.GetId("female");

		private readonly Transform _transform;

		private readonly MainPerson _mainPerson;

		private readonly PersonLocomotionState _state;

		private readonly StaminaService _stamina;

		private readonly PersonLocomotionConfig _config;

		private readonly SoundBankLoadService _bankService;

		private Animator _animator;

		private Transform _head;

		private bool _playing;

		public PersonBreathAudio(Transform transform, MainPerson mainPerson, PersonLocomotionState state, StaminaService stamina, PersonLocomotionConfig config, SoundBankLoadService bankService)
		{
			_transform = transform;
			_mainPerson = mainPerson;
			_state = state;
			_stamina = stamina;
			_config = config;
			_bankService = bankService;
		}

		public void Tick()
		{
			if (_state == null || _config == null || _bankService == null)
			{
				return;
			}
			if (!_config.BreathAudioEnabled || !_state.IsActive)
			{
				Stop();
				return;
			}
			LocomotionStateSnapshot snapshot = _state.Snapshot;
			if (snapshot.IsJumping || snapshot.IsSliding)
			{
				Stop();
				return;
			}
			GameObject gameObject = ResolveEmitter();
			if ((bool)gameObject)
			{
				float num = ((_stamina != null) ? _stamina.Normalized.Value : 1f);
				AudioWrapper.SetRTPCValue(StaminaRtpc, num, gameObject);
				if (IsMoving(snapshot))
				{
					Play(gameObject);
				}
				else if (num >= _config.BreathStaminaFullThreshold)
				{
					Stop();
				}
			}
		}

		public void Stop()
		{
			if (_playing)
			{
				_playing = false;
				if ((bool)_head && _bankService != null)
				{
					_bankService.PostEvent(BreathStop, SoundBankType.CpmSoundBank, _head.gameObject).Forget();
				}
			}
		}

		private bool IsMoving(LocomotionStateSnapshot snapshot)
		{
			if (snapshot.Gait > 0)
			{
				return snapshot.MoveSpeed > _config.BreathMoveSpeedThreshold;
			}
			return false;
		}

		private void Play(GameObject emitter)
		{
			if (!_playing)
			{
				_playing = true;
				ApplyGender(emitter);
				_bankService.PostEvent(BreathPlay, SoundBankType.CpmSoundBank, emitter).Forget();
			}
		}

		private void ApplyGender(GameObject emitter)
		{
			bool flag = (bool)_mainPerson && (int)_mainPerson.Gender == 1;
			AudioWrapper.SetSwitch(GenderGroup, flag ? GenderMale : GenderFemale, emitter);
		}

		private GameObject ResolveEmitter()
		{
			Animator animator = ResolveAnimator();
			if ((bool)animator && animator.isHuman && (object)animator != _animator)
			{
				Stop();
				_animator = animator;
				_head = null;
			}
			if (!_head)
			{
				if (!animator || !animator.isHuman)
				{
					return null;
				}
				Transform boneTransform = animator.GetBoneTransform(HumanBodyBones.Head);
				_head = (boneTransform ? boneTransform : _transform);
				if (!_head)
				{
					return null;
				}
				WiseHelper.SetAudioWiseComponents(_head.gameObject, addColliderTrigger: false);
			}
			return _head.gameObject;
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

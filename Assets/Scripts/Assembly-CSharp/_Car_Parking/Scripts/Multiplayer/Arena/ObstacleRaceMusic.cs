using Cysharp.Threading.Tasks;
using UnityEngine;
using _Car_Parking.Scripts.Audio;
using _Car_Parking.Scripts.Audio.SoundBanks;

namespace _Car_Parking.Scripts.Multiplayer.Arena
{
	public sealed class ObstacleRaceMusic
	{
		private const string EmitterName = "ObstacleRaceMusicEmitter";

		private const string HitTrigger = "player_hit";

		private readonly uint _playEvent = AudioWrapper.GetId("Play_Wipeout_Cpm_Event_Music");

		private readonly uint _stopEvent = AudioWrapper.GetId("Stop_Wipeout_Cpm_Event_Music");

		private readonly uint _stateGroup = AudioWrapper.GetId("Event_Music_State_Wipeout");

		private readonly uint _introState = AudioWrapper.GetId("intro");

		private readonly uint _lowState = AudioWrapper.GetId("low_intencity");

		private readonly uint _highState = AudioWrapper.GetId("high_intencity");

		private readonly uint _winState = AudioWrapper.GetId("win");

		private readonly uint _looseState = AudioWrapper.GetId("loose");

		private readonly SoundBankLoadService _bankService;

		private GameObject _emitter;

		private uint _appliedState;

		private bool _running;

		private bool _playing;

		private bool _bankRequested;

		public ObstacleRaceMusic(SoundBankLoadService bankService)
		{
			_bankService = bankService;
		}

		public void Play()
		{
			if (!_running)
			{
				_running = true;
				_bankRequested = true;
				ApplyState(_introState);
				PlayAsync().Forget();
			}
		}

		public void SetLowIntensity()
		{
			ApplyState(_lowState);
		}

		public void SetHighIntensity()
		{
			ApplyState(_highState);
		}

		public void SetOutcome(bool prizePlace)
		{
			ApplyState(prizePlace ? _winState : _looseState);
		}

		public void PlayerHit()
		{
			if (_playing && (bool)_emitter)
			{
				AudioWrapper.PostTrigger("player_hit", _emitter);
			}
		}

		public void Stop()
		{
			if (_playing && (bool)_emitter)
			{
				AudioWrapper.PostEvent(_stopEvent, _emitter);
			}
			_running = false;
			_playing = false;
			_appliedState = 0u;
			if (_bankRequested)
			{
				_bankRequested = false;
				_bankService.TryUnloadBank(SoundBankType.CpmMusic).Forget();
			}
		}

		private async UniTaskVoid PlayAsync()
		{
			await _bankService.TryLoadBank(SoundBankType.CpmMusic);
			if (_running)
			{
				AudioWrapper.PostEvent(_playEvent, Emitter());
				_playing = true;
			}
		}

		private GameObject Emitter()
		{
			if (!_emitter)
			{
				_emitter = new GameObject("ObstacleRaceMusicEmitter");
				Object.DontDestroyOnLoad(_emitter);
			}
			return _emitter;
		}

		private void ApplyState(uint state)
		{
			if (_running && state != _appliedState)
			{
				_appliedState = state;
				AudioWrapper.SetState(_stateGroup, state);
			}
		}
	}
}

using Cysharp.Threading.Tasks;
using UnityEngine;
using _Car_Parking.Scripts.Audio.SoundBanks;

namespace _Car_Parking.Scripts.Audio
{
	public class RainSound
	{
		private const float ThunderDistantThreshold = 0.1f;

		private const float ThunderCloseThreshold = 0.6f;

		private const float ThunderThresholdTolerance = 0.005f;

		private readonly uint _playRainEvent = AudioWrapper.GetId("Play_Rain");

		private readonly uint _stopRainEvent = AudioWrapper.GetId("Stop_Rain");

		private readonly uint _rainIntensityRtpc = AudioWrapper.GetId("RTPC_Rain_Intencity");

		private readonly uint _playThunderDistantEvent = AudioWrapper.GetId("Play_Thunder_Distant");

		private readonly uint _stopThunderDistantEvent = AudioWrapper.GetId("Stop_Thunder_Distant");

		private readonly uint _playThunderCloseEvent = AudioWrapper.GetId("Play_Thunder_Close");

		private readonly uint _stopThunderCloseEvent = AudioWrapper.GetId("Stop_Thunder_Close");

		private readonly SoundBankLoadService _bankService;

		private readonly RainSoundState _rainSoundState;

		private bool _rainPlaying;

		private bool _thunderDistantPlaying;

		private bool _thunderClosePlaying;

		private bool _bankRequested;

		private int _pauseDepth;

		private float _prevIntensity = -1f;

		private GameObject _emitter;

		public RainSound(SoundBankLoadService bankService, RainSoundState rainSoundState)
		{
			_bankService = bankService;
			_rainSoundState = rainSoundState;
		}

		public void Tick(bool rainActive, float intensity)
		{
			if (rainActive && _pauseDepth == 0 && !_rainPlaying)
			{
				StartRain();
			}
			else if (!rainActive && (_rainPlaying || _bankRequested))
			{
				StopAll();
			}
			if (!_rainPlaying)
			{
				return;
			}
			if (!_emitter)
			{
				_rainPlaying = (_thunderDistantPlaying = (_thunderClosePlaying = false));
				return;
			}
			if (!Mathf.Approximately(intensity, _prevIntensity))
			{
				AudioWrapper.SetRTPCValue(_rainIntensityRtpc, intensity);
				_prevIntensity = intensity;
			}
			UpdateThunder(intensity);
		}

		public void Pause()
		{
			_pauseDepth++;
			if (_pauseDepth == 1)
			{
				Stop(releaseBank: false);
			}
		}

		public void Resume()
		{
			if (_pauseDepth != 0)
			{
				_pauseDepth--;
			}
		}

		public void StopAll()
		{
			Stop(releaseBank: true);
		}

		private void Stop(bool releaseBank)
		{
			if ((bool)_emitter)
			{
				if (_rainPlaying)
				{
					AudioWrapper.PostEvent(_stopRainEvent, _emitter);
				}
				if (_thunderDistantPlaying)
				{
					AudioWrapper.PostEvent(_stopThunderDistantEvent, _emitter);
				}
				if (_thunderClosePlaying)
				{
					AudioWrapper.PostEvent(_stopThunderCloseEvent, _emitter);
				}
			}
			_rainPlaying = (_thunderDistantPlaying = (_thunderClosePlaying = false));
			_emitter = null;
			_prevIntensity = -1f;
			if (releaseBank && _bankRequested)
			{
				_bankRequested = false;
				_bankService.TryUnloadBank(SoundBankType.CpmRain).Forget();
			}
		}

		private void StartRain()
		{
			if (!_bankRequested)
			{
				_bankRequested = true;
				_bankService.TryLoadBank(SoundBankType.CpmRain).Forget();
			}
			if (_bankService.IsLoaded(SoundBankType.CpmRain))
			{
				GameObject gameObject = (((bool)V.CoreGameManager && (bool)V.CoreGameManager.MainCamera) ? V.CoreGameManager.MainCamera.gameObject : null);
				if ((bool)gameObject)
				{
					_emitter = gameObject;
					_rainSoundState.EnsureApplied();
					AudioWrapper.PostEvent(_playRainEvent, _emitter);
					_rainPlaying = true;
					_prevIntensity = -1f;
				}
			}
		}

		private void UpdateThunder(float intensity)
		{
			bool flag = intensity >= 0.095f;
			if (flag != _thunderDistantPlaying)
			{
				AudioWrapper.PostEvent(flag ? _playThunderDistantEvent : _stopThunderDistantEvent, _emitter);
				_thunderDistantPlaying = flag;
			}
			bool flag2 = intensity > 0.605f;
			if (flag2 != _thunderClosePlaying)
			{
				AudioWrapper.PostEvent(flag2 ? _playThunderCloseEvent : _stopThunderCloseEvent, _emitter);
				_thunderClosePlaying = flag2;
			}
		}
	}
}

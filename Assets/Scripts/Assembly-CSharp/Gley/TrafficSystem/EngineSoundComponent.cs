using UnityEngine;

namespace Gley.TrafficSystem
{
	[RequireComponent(typeof(AudioSource))]
	public class EngineSoundComponent : MonoBehaviour
	{
		[Tooltip("Pitch used when vehicle is stationary")]
		public float minPitch = 0.6f;

		[Tooltip("Pitch used when vehicle is at max speed")]
		public float maxPitch = 1f;

		[Tooltip("Volume used when vehicle is stationary")]
		public float minVolume = 0.5f;

		[Tooltip("Volume used when vehicle is at max speed")]
		public float maxVolume = 1f;

		private AudioSource _audioSource;

		public void Initialize()
		{
			_audioSource = GetComponent<AudioSource>();
			_audioSource.loop = true;
		}

		public void Play(float masterVolume)
		{
			_audioSource.volume = masterVolume;
			_audioSource.Play();
		}

		public void Stop()
		{
			_audioSource.Stop();
		}

		public void UpdateEngineSound(float velocity, float maxVelocity, float masterVolume)
		{
			float num = velocity / maxVelocity;
			_audioSource.volume = (minVolume + (maxVolume - minVolume) * num) * masterVolume;
			float pitch = minPitch + (maxPitch - minPitch) * num;
			_audioSource.pitch = pitch;
		}
	}
}

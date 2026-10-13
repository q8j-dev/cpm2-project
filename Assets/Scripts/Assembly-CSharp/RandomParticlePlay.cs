using System;
using UniRx;
using UnityEngine;

public class RandomParticlePlay : MonoBehaviour
{
	private ParticleSystem _particleSystem;

	public AudioSource _AudioSource;

	private void Start()
	{
		_particleSystem = GetComponent<ParticleSystem>();
		Observable.Timer(TimeSpan.FromSeconds(2.0), TimeSpan.FromSeconds(5.0)).Subscribe((long _) =>
		{
			PlayParticle();
		}).AddTo(this);
	}

	private void PlayParticle()
	{
		if ((bool)_particleSystem && UnityEngine.Random.Range(0, 12) == 0)
		{
			_particleSystem.Play(withChildren: true);
			if ((bool)_AudioSource)
			{
				_AudioSource.Play();
			}
		}
	}
}

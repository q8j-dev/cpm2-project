using System.Collections.Generic;
using UnityEngine;
using _Car_Parking.Scripts.Audio;

namespace _Car_Parking.Scripts.Other.ParticlesZone
{
	public class ParticleSystemZone : MonoBehaviour
	{
		[SerializeField]
		private ParticleSystem[] ParticleSystems = new ParticleSystem[0];

		private List<SandFallSound> _sandFallSounds = new List<SandFallSound>();

		private const string TagToReact = "MainCamera";

		private void Awake()
		{
			SetParticles(isOn: false);
			if (ParticleSystems == null)
			{
				return;
			}
			ParticleSystem[] particleSystems = ParticleSystems;
			foreach (ParticleSystem particleSystem in particleSystems)
			{
				if ((bool)particleSystem && particleSystem.TryGetComponent<SandFallSound>(out var component))
				{
					_sandFallSounds.Add(component);
				}
			}
		}

		private void OnTriggerEnter(Collider other)
		{
			if (other.CompareTag("MainCamera"))
			{
				SetParticles(isOn: true);
			}
		}

		private void OnTriggerExit(Collider other)
		{
			if (other.CompareTag("MainCamera"))
			{
				SetParticles(isOn: false);
			}
		}

		private void SetParticles(bool isOn)
		{
			if (ParticleSystems == null || ParticleSystems.Length == 0)
			{
				return;
			}
			ParticleSystem[] particleSystems = ParticleSystems;
			foreach (ParticleSystem particleSystem in particleSystems)
			{
				if (!particleSystem)
				{
					continue;
				}
				if (isOn)
				{
					particleSystem.Play(withChildren: true);
					if (_sandFallSounds == null || _sandFallSounds.Count <= 0)
					{
						continue;
					}
					foreach (SandFallSound sandFallSound in _sandFallSounds)
					{
						sandFallSound.StartCheckParticles();
					}
					continue;
				}
				particleSystem.Stop(withChildren: true);
				if (_sandFallSounds == null || _sandFallSounds.Count <= 0)
				{
					continue;
				}
				foreach (SandFallSound sandFallSound2 in _sandFallSounds)
				{
					sandFallSound2.StopCheckParticles();
				}
			}
		}
	}
}

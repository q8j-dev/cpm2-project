using UnityEngine;
using _Car_Parking.Scripts.ArtistTools.Enums;

namespace _Car_Parking.Scripts.ArtistTools
{
	public class VfxTriggerActivator : MonoBehaviour
	{
		[SerializeField]
		[Header("Main")]
		private ParticleSystem VfxEffect = default;

		[SerializeField]
		private bool IsSpawnParticle = default;

		[Header("Activator settings")]
		[SerializeField]
		private TriggerActivator Activator = default;

		[Header("VFX states")]
		[SerializeField]
		private VfxActivatingState InitialMode = default;

		[SerializeField]
		private VfxActivatingState OnEnterVfxMode = default;

		[SerializeField]
		private VfxActivatingState OnExitVfxMode = default;

		private ParticleSystem _spawnedParticle;

		private void Awake()
		{
			ChangeVfxState(InitialMode);
		}

		private void OnTriggerEnter(Collider other)
		{
			if (IsValidActivator(other))
			{
				ChangeVfxState(OnEnterVfxMode);
			}
		}

		private void OnTriggerExit(Collider other)
		{
			if (IsValidActivator(other))
			{
				ChangeVfxState(OnExitVfxMode);
			}
		}

		private void CheckSpawnedParticle()
		{
			if (IsSpawnParticle && !_spawnedParticle)
			{
				_spawnedParticle = Object.Instantiate(VfxEffect, base.transform.position, base.transform.rotation);
			}
		}

		private bool IsValidActivator(Collider other)
		{
			if (!other)
			{
				return false;
			}
			bool flag = other.gameObject == V.DriverPerson;
			bool flag2 = other.gameObject == V.Car;
			Transform parent = other.transform.parent;
			if ((bool)parent && !flag2)
			{
				flag2 = parent.gameObject == V.Car;
			}
			return Activator switch
			{
				TriggerActivator.Person => flag, 
				TriggerActivator.Car => flag2, 
				TriggerActivator.PersonOrCar => flag | flag2, 
				_ => false, 
			};
		}

		private void ChangeVfxState(VfxActivatingState state)
		{
			CheckSpawnedParticle();
			ParticleSystem particleSystem = (IsSpawnParticle ? _spawnedParticle : VfxEffect);
			if ((bool)particleSystem)
			{
				switch (state)
				{
				case VfxActivatingState.StopInstantly:
					particleSystem.Stop(withChildren: true, ParticleSystemStopBehavior.StopEmittingAndClear);
					break;
				case VfxActivatingState.StopGradually:
					particleSystem.Stop();
					break;
				case VfxActivatingState.PlayOnce:
					particleSystem.loop = false;
					particleSystem.Play();
					break;
				case VfxActivatingState.PlayLoop:
					particleSystem.loop = true;
					particleSystem.Play();
					break;
				}
			}
		}
	}
}

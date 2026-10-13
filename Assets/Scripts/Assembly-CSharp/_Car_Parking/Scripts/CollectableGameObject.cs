using AK.Wwise;
using UnityEngine;
using _Car_Parking.Scripts.ArtistTools;
using _Car_Parking.Scripts.Audio;

namespace _Car_Parking.Scripts
{
	public class CollectableGameObject : MonoBehaviour
	{
		[SerializeField]
		private VfxSpawner VfxSpawner;

		[SerializeField]
		private AK.Wwise.Event CollisionSoundEvent;

		[SerializeField]
		private CollectableGameObjectClaim[] Claims;

		private bool _wasTriggerClaim;

		private void OnTriggerEnter(Collider other)
		{
			if (_wasTriggerClaim || other.isTrigger || !other.attachedRigidbody)
			{
				return;
			}
			GameObject gameObject = other.attachedRigidbody.gameObject;
			if (gameObject.CompareTag("Car") || gameObject.CompareTag("OnlineCar"))
			{
				if (V.Car != gameObject)
				{
					return;
				}
			}
			else if (!gameObject.CompareTag("DriverPerson") || V.DriverPerson != gameObject)
			{
				return;
			}
			_wasTriggerClaim = true;
			if ((bool)VfxSpawner)
			{
				VfxSpawner.TryActivateVfxEffect();
			}
			AudioWrapper.PostProcess(CollisionSoundEvent, base.gameObject);
			if (Claims == null)
			{
				return;
			}
			for (int i = 0; i < Claims.Length; i++)
			{
				if ((bool)Claims[i])
				{
					Claims[i].Claim(this);
				}
			}
		}
	}
}

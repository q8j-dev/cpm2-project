using UnityEngine;
using _Car_Parking.Scripts.Audio;

namespace Animals
{
	public class AnimalSoundStop : MonoBehaviour
	{
		private void OnDisable()
		{
			if (AudioWrapper.IsInitialized())
			{
				AudioWrapper.StopAll(base.gameObject);
			}
		}
	}
}

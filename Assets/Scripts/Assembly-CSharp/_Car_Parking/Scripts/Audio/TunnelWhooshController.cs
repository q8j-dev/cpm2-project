using System.Collections.Generic;
using System.Linq;
using CPMEngine;
using UnityEngine;

namespace _Car_Parking.Scripts.Audio
{
	public class TunnelWhooshController : MonoBehaviour
	{
		[SerializeField]
		private List<IgnoreWhooshSoundController> IgnoreWhooshSoundsControllers = default;

		private const string ObjectsTagToPlayWhoosh = "MainCamera";

		private const string PlayPassTunnelWhoosh = "Play_Pass_Tunel_Whoosh";

		private const float SpeedToPlayWhoosh = 150f;

		private const int DynamicCamIndex = 5;

		private bool _ignoreWhoosh => IgnoreWhooshSoundsControllers.Any((IgnoreWhooshSoundController a) => a.IgnorePlayWhoosh);

		private void OnTriggerEnter(Collider other)
		{
			PlayWhoosh(other);
		}

		private void PlayWhoosh(Collider other)
		{
			if (other.CompareTag("MainCamera") && !_ignoreWhoosh && V.CurrentCamIndex != 5 && GetSpeedCar(V.Car) >= 150f)
			{
				AudioWrapper.PostEvent("Play_Pass_Tunel_Whoosh", base.gameObject);
			}
		}

		private float GetSpeedCar(GameObject car)
		{
			if ((bool)car && car.TryGetComponent<Powertrain>(out var component))
			{
				if (FlagsStorage.IsPassenger)
				{
					return component.SpeedMpsOnline * 3.6f;
				}
				return component.SpeedKmh;
			}
			return 0f;
		}
	}
}

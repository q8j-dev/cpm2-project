using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Car_Parking.Scripts.Steam.World
{
	public static class SteamCarTeleportSnap
	{
		public static void SnapCar()
		{
			GameObject car = V.Car;
			if ((bool)car && car.TryGetComponent<Rigidbody>(out var component))
			{
				RigidbodyInterpolation interpolation = component.interpolation;
				if (interpolation != RigidbodyInterpolation.None)
				{
					component.interpolation = RigidbodyInterpolation.None;
					component.position = car.transform.position;
					component.rotation = car.transform.rotation;
					RestoreInterpolation(component, interpolation).Forget();
				}
			}
		}

		private static async UniTaskVoid RestoreInterpolation(Rigidbody body, RigidbodyInterpolation saved)
		{
			await UniTask.DelayFrame(1);
			if ((bool)body)
			{
				body.interpolation = saved;
			}
		}
	}
}

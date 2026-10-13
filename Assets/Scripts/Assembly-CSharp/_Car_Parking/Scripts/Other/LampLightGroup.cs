using DG.Tweening;
using UnityEngine;

namespace _Car_Parking.Scripts.Other
{
	public class LampLightGroup : MonoBehaviour
	{
		[SerializeField]
		private LampLight[] LampLights;

		public LampLight GetLampLight(int index)
		{
			if (index >= 0 && index < LampLights.Length)
			{
				return LampLights[index];
			}
			return null;
		}

		public void EnableDisableAnimation(int indexLamp, float timeToEnable, float duration, float timeToDisable, TweenCallback onComplete = null)
		{
			LampLight lampLight = GetLampLight(indexLamp);
			if ((bool)lampLight)
			{
				lampLight.EnableDisableAnimation(timeToEnable, duration, timeToDisable, onComplete);
			}
		}

		public void Enable(int indexLamp, float time, TweenCallback onComplete = null)
		{
			UpdateIntensity(indexLamp, 1f, time, onComplete);
		}

		public void DisableAll(float time)
		{
			for (int i = 0; i < LampLights.Length; i++)
			{
				Disable(i, time);
			}
		}

		public void Disable(int indexLamp, float time, TweenCallback onComplete = null)
		{
			UpdateIntensity(indexLamp, 0f, time, onComplete);
		}

		public void UpdateIntensity(int indexLamp, float intensity, float time, TweenCallback onComplete = null)
		{
			LampLight lampLight = GetLampLight(indexLamp);
			if ((bool)lampLight)
			{
				lampLight.UpdateIntensity(intensity, time, onComplete);
			}
		}

		private void SetRefs()
		{
			LampLights = GetComponentsInChildren<LampLight>(includeInactive: true);
		}
	}
}

using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using _Car_Parking.Scripts.Audio;

namespace Car_Parking.Scripts.Racing
{
	public class StartTrafficLightObject : MonoBehaviour
	{
		public GameObject[] RedLights;

		public GameObject GreenLight;

		private int _currentRedIndex;

		private void OnEnable()
		{
			_currentRedIndex = 0;
		}

		public async UniTask ShowRedLights(float duration)
		{
			int lastIndex = _currentRedIndex;
			AudioWrapper.PostEvent("Play_count_ready", base.gameObject);
			RedLights[_currentRedIndex].SetActive(value: true);
			_currentRedIndex = (_currentRedIndex + 1) % RedLights.Length;
			await UniTask.Delay(TimeSpan.FromSeconds(duration), ignoreTimeScale: false, PlayerLoopTiming.Update, base.destroyCancellationToken);
			RedLights[lastIndex].SetActive(value: false);
		}

		public void ShowGreenLight()
		{
			AudioWrapper.PostEvent("Play_count_go", base.gameObject);
			GreenLight.SetActive(value: true);
		}

		public void Reset()
		{
			_currentRedIndex = 0;
			GreenLight.SetActive(value: false);
		}
	}
}

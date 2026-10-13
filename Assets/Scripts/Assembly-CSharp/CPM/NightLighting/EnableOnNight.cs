using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using _Car_Parking.Scripts.Infrastructure;

namespace CPM.NightLighting
{
	public class EnableOnNight : MonoBehaviour
	{
		private SunAndSkyController _sunAndSkyController;

		private const float BindTimeoutSeconds = 30f;

		private const int BindRetryMs = 250;

		private void Start()
		{
			BindToSunControllerAsync(this.GetCancellationTokenOnDestroy()).Forget();
		}

		private async UniTaskVoid BindToSunControllerAsync(CancellationToken cancellationToken)
		{
			float deadline = Time.realtimeSinceStartup + 30f;
			while (true)
			{
				_sunAndSkyController = GlobalEventSystem.RequestSunController();
				if ((bool)_sunAndSkyController)
				{
					break;
				}
				if (Time.realtimeSinceStartup >= deadline || await UniTask.Delay(250, DelayType.UnscaledDeltaTime, PlayerLoopTiming.Update, cancellationToken).SuppressCancellationThrow())
				{
					return;
				}
			}
			_sunAndSkyController.OnDayTimeChanged += UpdateGOState;
			UpdateGOState(_sunAndSkyController.IsNight);
		}

		private void OnDestroy()
		{
			if ((bool)_sunAndSkyController)
			{
				_sunAndSkyController.OnDayTimeChanged -= UpdateGOState;
			}
		}

		private void UpdateGOState(bool isNightTime)
		{
			base.gameObject.SetActive(isNightTime);
		}
	}
}

using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using _Car_Parking.Scripts.Audio;

public class LapRaceTrafficLightUI : MonoBehaviour
{
	[SerializeField]
	private CanvasGroup[] RedLights;

	[SerializeField]
	private CanvasGroup GreenLight;

	private int _currentRedIndex;

	private bool _isDestroyed;

	public void Reset()
	{
		_currentRedIndex = 0;
		ResetRedLights();
		SetGreenLight(state: false);
	}

	public void ResetRedLights()
	{
		if (RedLights.IsNullOrEmptyCollection())
		{
			return;
		}
		for (int i = 0; i < RedLights.Length; i++)
		{
			if ((bool)RedLights[i])
			{
				RedLights[i].alpha = 0f;
			}
		}
	}

	public async UniTaskVoid ShowRedLights(float duration)
	{
		int lastIndex = _currentRedIndex;
		if ((bool)RedLights[_currentRedIndex])
		{
			RedLights[_currentRedIndex].alpha = 1f;
			_currentRedIndex = (_currentRedIndex + 1) % RedLights.Length;
		}
		await UniTask.Delay(TimeSpan.FromSeconds(duration));
		if (!_isDestroyed)
		{
			RedLights[lastIndex].alpha = 0f;
		}
	}

	public async UniTask StartLights()
	{
		Reset();
		int duration = 1;
		CancellationToken token = this.GetCancellationTokenOnDestroy();
		for (int i = 0; i < RedLights.Length; i++)
		{
			ShowRedLights(duration).Forget();
			AudioWrapper.PostEvent("Play_count_ready", base.gameObject);
			await UniTask.Delay(TimeSpan.FromSeconds(duration), ignoreTimeScale: false, PlayerLoopTiming.Update, token);
		}
		AudioWrapper.PostEvent("Play_count_go", base.gameObject);
		SetGreenLight(state: true);
	}

	public void SetRedLight(int index)
	{
		ResetRedLights();
		RedLights[index].alpha = 1f;
	}

	public void SetGreenLight(bool state)
	{
		GreenLight.alpha = (state ? 1 : 0);
	}

	private void OnDestroy()
	{
		_isDestroyed = true;
	}
}

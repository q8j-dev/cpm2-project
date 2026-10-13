using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class DecalIcon : MonoBehaviour
{
	private Renderer _renderer;

	public float maxAlpha = 1f;

	public float minAlpha;

	private CancellationTokenSource _cts;

	private void Start()
	{
		_renderer = GetComponent<Renderer>();
	}

	public void Hide()
	{
		RestartCts();
		HideMeshAsync(_cts.Token).Forget();
	}

	public void Show()
	{
		RestartCts();
		ShowMeshAsync(_cts.Token).Forget();
	}

	private void RestartCts()
	{
		_cts?.Cancel();
		_cts?.Dispose();
		_cts = CancellationTokenSource.CreateLinkedTokenSource(this.GetCancellationTokenOnDestroy());
	}

	private void OnDestroy()
	{
		_cts?.Cancel();
		_cts?.Dispose();
		_cts = null;
	}

	private async UniTask HideMeshAsync(CancellationToken ct)
	{
		try
		{
			for (float f = _renderer.material.color.a; f >= minAlpha; f -= 0.08f)
			{
				Color color = _renderer.material.color;
				color.a = f;
				_renderer.material.color = color;
				await UniTask.Delay(TimeSpan.FromSeconds(0.019999999552965164), ignoreTimeScale: false, PlayerLoopTiming.Update, ct);
			}
		}
		catch (OperationCanceledException)
		{
		}
	}

	private async UniTask ShowMeshAsync(CancellationToken ct)
	{
		try
		{
			for (float f = _renderer.material.color.a; f <= maxAlpha; f = 0.08f + f)
			{
				Color color = _renderer.material.color;
				color.a = f;
				_renderer.material.color = color;
				await UniTask.Delay(TimeSpan.FromSeconds(0.019999999552965164), ignoreTimeScale: false, PlayerLoopTiming.Update, ct);
			}
		}
		catch (OperationCanceledException)
		{
		}
	}
}

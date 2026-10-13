using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class DelayCamFrameRate : MonoBehaviour
{
	public Camera cam;

	public float delay = 0.05f;

	public GameObject mirror;

	private CancellationTokenSource _cts;

	private void Awake()
	{
		BindCamera();
	}

	public void BindCamera()
	{
		if (TryGetComponent<Camera>(out cam))
		{
			cam.enabled = false;
		}
	}

	public void RestartCoroutine()
	{
		if (base.gameObject.activeSelf)
		{
			StopRendering();
			_cts = CancellationTokenSource.CreateLinkedTokenSource(this.GetCancellationTokenOnDestroy());
			DelayedRenderingAsync(_cts.Token).Forget();
		}
	}

	private void StopRendering()
	{
		_cts?.Cancel();
		_cts?.Dispose();
		_cts = null;
	}

	private void OnDestroy()
	{
		StopRendering();
	}

	public async UniTask DelayedRenderingAsync(CancellationToken ct = default(CancellationToken))
	{
		try
		{
			while (!ct.IsCancellationRequested)
			{
				if ((bool)cam && (bool)mirror)
				{
					float num = Vector3.Distance(base.transform.position, V.CoreGameManager.transform.position);
					if (num < 50f)
					{
						delay = num * 0.04f;
						cam.Render();
					}
				}
				await UniTask.Delay(TimeSpan.FromSeconds(delay), ignoreTimeScale: false, PlayerLoopTiming.Update, ct);
			}
		}
		catch (OperationCanceledException)
		{
		}
	}
}

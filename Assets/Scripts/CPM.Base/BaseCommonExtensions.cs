using System;
using System.Threading;
using UnityEngine;

public static class BaseCommonExtensions
{
	public static void CancelAndDisposeToken(ref CancellationTokenSource tokenSource)
	{
		CancellationTokenSource cancellationTokenSource = tokenSource;
		tokenSource = null;
		if (cancellationTokenSource == null)
		{
			return;
		}
		try
		{
			cancellationTokenSource.Cancel();
		}
		finally
		{
			cancellationTokenSource.Dispose();
		}
	}

	public static void DisposeAndClear<T>(ref T disposable) where T : class, IDisposable
	{
		T val = disposable;
		disposable = null;
		val?.Dispose();
	}

	public static void DestroyMat(ref Material mat)
	{
		if ((bool)mat)
		{
			UnityEngine.Object.Destroy(mat);
			mat = null;
		}
	}

	public static T EnsureComponent<T>(this GameObject go) where T : Component
	{
		if (!go.TryGetComponent<T>(out var component))
		{
			return go.AddComponent<T>();
		}
		return component;
	}

	public static T EnsureComponent<T>(this GameObject go, out T t) where T : Component
	{
		if (!go.TryGetComponent<T>(out t))
		{
			return t = go.AddComponent<T>();
		}
		return t;
	}
}

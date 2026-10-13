using System;
using System.Collections.Generic;
using System.Threading;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;

public static class CommonExtensions
{
	public static TimeSpan AsSeconds(this float seconds)
	{
		return TimeSpan.FromSeconds(seconds);
	}

	public static TimeSpan AsSeconds(this int seconds)
	{
		return TimeSpan.FromSeconds(seconds);
	}

	public static TimeSpan AsSeconds(this ObscuredFloat seconds)
	{
		return TimeSpan.FromSeconds((float)seconds);
	}

	public static TimeSpan AsSeconds(this ObscuredInt seconds)
	{
		return TimeSpan.FromSeconds((int)seconds);
	}

	public static int AsSign(this bool b)
	{
		if (!b)
		{
			return -1;
		}
		return 1;
	}

	public static float AsSign(this bool b, float f)
	{
		if (!b)
		{
			return 0f - f;
		}
		return f;
	}

	public static int AsInt(this bool b)
	{
		if (!b)
		{
			return 0;
		}
		return 1;
	}

	public static float AsFloat(this bool b, float f)
	{
		if (!b)
		{
			return 0f;
		}
		return f;
	}

	public static float AsFloat(this bool b, float trueResult, float falseResult)
	{
		if (!b)
		{
			return falseResult;
		}
		return trueResult;
	}

	public static void DestroyMat(ref Material mat)
	{
		if ((bool)mat)
		{
			UnityEngine.Object.Destroy(mat);
			mat = null;
		}
	}

	public static void CopyColor(Material from, Material to, string fromColorName = "_Color", string toColorName = "_Color")
	{
		if ((bool)from && (bool)to && from.HasProperty(fromColorName) && to.HasProperty(toColorName))
		{
			to.SetColor(fromColorName, from.GetColor(toColorName));
		}
	}

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

	public static bool NullIfDestroyed<T>(ref T field) where T : UnityEngine.Object
	{
		if ((object)field != null && field == null)
		{
			field = null;
			return true;
		}
		return false;
	}

	public static T GetRandomEnum<T>() where T : Enum
	{
		Array values = Enum.GetValues(typeof(T));
		if (values.Length == 0)
		{
			return default;
		}
		return (T)values.GetValue(UnityEngine.Random.Range(0, values.Length));
	}

	public static List<int> GenerateRandomListInt(int countMin, int countMax, int valueMin, int valueMax)
	{
		List<int> list = new List<int>();
		int num = UnityEngine.Random.Range(countMin, countMax);
		for (int i = 0; i < num; i++)
		{
			list.Add(UnityEngine.Random.Range(valueMin, valueMax));
		}
		return list;
	}

	public static List<float> GenerateRandomListFloat(int countMin, int countMax, float valueMin, float valueMax)
	{
		List<float> list = new List<float>();
		int num = UnityEngine.Random.Range(countMin, countMax);
		for (int i = 0; i < num; i++)
		{
			list.Add(UnityEngine.Random.Range(valueMin, valueMax));
		}
		return list;
	}

	public static void ClearGOArray(ref GameObject[] array)
	{
		if (array != null)
		{
			GameObject[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				UnityEngine.Object.Destroy(array2[i]);
			}
			array = null;
		}
	}

	public static void ClearGOArray<T>(ref T[] array) where T : Component
	{
		if (array == null)
		{
			return;
		}
		T[] array2 = array;
		foreach (T val in array2)
		{
			if ((bool)val)
			{
				UnityEngine.Object.Destroy(val.gameObject);
			}
		}
		array = null;
	}

	public static void ClearGOCollection(ICollection<GameObject> collection)
	{
		if (collection == null)
		{
			return;
		}
		foreach (GameObject item in collection)
		{
			UnityEngine.Object.Destroy(item);
		}
		collection.Clear();
	}

	public static void ClearGOCollection<T>(ICollection<T> collection) where T : Component
	{
		if (collection == null)
		{
			return;
		}
		foreach (T item in collection)
		{
			if ((bool)item)
			{
				UnityEngine.Object.Destroy(item.gameObject);
			}
		}
		collection.Clear();
	}

	public static void ClearObjectCollection<T>(ICollection<T> collection) where T : UnityEngine.Object
	{
		if (collection == null)
		{
			return;
		}
		foreach (T item in collection)
		{
			UnityEngine.Object.Destroy(item);
		}
		collection.Clear();
	}

	public static void ClearObjectArray<T>(ref T[] array) where T : UnityEngine.Object
	{
		if (array != null)
		{
			T[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				UnityEngine.Object.Destroy(array2[i]);
			}
			array = null;
		}
	}

	public static void DeleteRenderTex(RenderTexture renderTexture)
	{
		if ((bool)renderTexture)
		{
			renderTexture.Release();
			UnityEngine.Object.Destroy(renderTexture);
		}
	}

	public static Texture2D ToTexture2D(this RenderTexture rTex)
	{
		Texture2D texture2D = new Texture2D(rTex.width, rTex.height, TextureFormat.RGB24, mipChain: false);
		RenderTexture active = RenderTexture.active;
		RenderTexture.active = rTex;
		texture2D.ReadPixels(new Rect(0f, 0f, rTex.width, rTex.height), 0, 0);
		texture2D.Apply();
		RenderTexture.active = active;
		return texture2D;
	}

	public static void DestroyComponent<T>(this GameObject gameObject) where T : Component
	{
		if ((bool)gameObject && gameObject.TryGetComponent<T>(out var component))
		{
			UnityEngine.Object.Destroy(component);
		}
	}
}

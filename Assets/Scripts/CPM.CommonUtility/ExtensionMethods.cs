using System;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

public static class ExtensionMethods
{
	public static void SetActive(this MonoBehaviour mono, bool active)
	{
		if ((bool)mono)
		{
			mono.gameObject.SetActive(active);
		}
		else
		{
			Debug.LogWarning("[MissingRef] components not exist");
		}
	}

	public static Vector3 ToVector3(this Color color)
	{
		return new Vector3(color.r, color.g, color.b);
	}

	public static Color ToColor(this Vector3 vector)
	{
		return new Color(vector.x, vector.y, vector.z, 1f);
	}

	public static bool TryJson<T>(this string json, out T value, JsonSerializerSettings serializerSettings = null)
	{
		try
		{
			if (serializerSettings == null)
			{
				value = JsonConvert.DeserializeObject<T>(json);
			}
			else
			{
				value = JsonConvert.DeserializeObject<T>(json, serializerSettings);
			}
			return value != null;
		}
		catch (Exception)
		{
		}
		try
		{
			value = JsonUtility.FromJson<T>(json);
			return value != null;
		}
		catch (Exception)
		{
			value = default;
			return false;
		}
	}

	public static async UniTask Rebuild(this Transform transform)
	{
		await UniTask.DelayFrame(1);
		transform.gameObject.SetActive(value: false);
		await UniTask.DelayFrame(1);
		transform.gameObject.SetActive(value: true);
	}
}

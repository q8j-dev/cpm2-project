using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ExitGames.Client.Photon;
using Photon.Pun;
using UnityEngine;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.VisualTuningFavourite;

public static class Extensions
{
	public static Dictionary<MethodInfo, ParameterInfo[]> ParametersOfMethods = new Dictionary<MethodInfo, ParameterInfo[]>();

	private static readonly int Smoothness = Shader.PropertyToID("_Smoothness");

	private static readonly int Metalness = Shader.PropertyToID("_Metalness");

	private static readonly int CoatMask = Shader.PropertyToID("_CoatMask");

	private static readonly int Iridescence = Shader.PropertyToID("_Iridescence");

	private static readonly int FlakesPower = Shader.PropertyToID("_FlakesPower");

	private static readonly int OrangePeelPower = Shader.PropertyToID("_OrangePeelPower");

	public static ParameterInfo[] GetCachedParemeters(this MethodInfo mo)
	{
		if (!ParametersOfMethods.TryGetValue(mo, out var value))
		{
			value = mo.GetParameters();
			ParametersOfMethods[mo] = value;
		}
		return value;
	}

	public static PhotonView[] GetPhotonViewsInChildren(this GameObject go)
	{
		return go.GetComponentsInChildren<PhotonView>(includeInactive: true);
	}

	public static PhotonView GetPhotonView(this GameObject go)
	{
		return go.GetComponent<PhotonView>();
	}

	public static bool TryGetPhotonView(this GameObject go, out PhotonView photonView)
	{
		return go.TryGetComponent<PhotonView>(out photonView);
	}

	public static bool ApproximatelyEqual(this Vector3 me, Vector3 other, float precision)
	{
		if (Mathf.Abs(me.x - other.x) > precision)
		{
			return false;
		}
		if (Mathf.Abs(me.y - other.y) > precision)
		{
			return false;
		}
		return Mathf.Abs(me.z - other.z) <= precision;
	}

	public static float DistanceSquare(Vector3 position01, Vector3 position02)
	{
		return (position01 - position02).sqrMagnitude;
	}

	public static bool AlmostEquals(this Vector3 target, Vector3 second, float sqrMagnitudePrecision)
	{
		return (target - second).sqrMagnitude < sqrMagnitudePrecision;
	}

	public static bool AlmostEquals(this Vector2 target, Vector2 second, float sqrMagnitudePrecision)
	{
		return (target - second).sqrMagnitude < sqrMagnitudePrecision;
	}

	public static bool AlmostEquals(this Quaternion target, Quaternion second, float maxAngle)
	{
		return Quaternion.Angle(target, second) < maxAngle;
	}

	public static bool AlmostEquals(this float target, float second, float floatDiff)
	{
		return Mathf.Abs(target - second) < floatDiff;
	}

	public static void Merge(this IDictionary target, IDictionary addHash)
	{
		if (addHash == null || target.Equals(addHash))
		{
			return;
		}
		foreach (object key in addHash.Keys)
		{
			target[key] = addHash[key];
		}
	}

	public static void MergeStringKeys(this IDictionary target, IDictionary addHash)
	{
		if (addHash == null || target.Equals(addHash))
		{
			return;
		}
		foreach (object key in addHash.Keys)
		{
			if (key is string)
			{
				target[key] = addHash[key];
			}
		}
	}

	public static string ToStringFull(this IDictionary origin)
	{
		return SupportClass.DictionaryToString(origin, includeTypes: false);
	}

	public static string ToStringFull(this object[] data)
	{
		if (data == null)
		{
			return "null";
		}
		string[] array = new string[data.Length];
		for (int i = 0; i < data.Length; i++)
		{
			object obj = data[i];
			array[i] = ((obj != null) ? obj.ToString() : "null");
		}
		return string.Join(", ", array);
	}

	public static ExitGames.Client.Photon.Hashtable StripToStringKeys(this IDictionary original)
	{
		ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
		if (original != null)
		{
			foreach (object key in original.Keys)
			{
				if (key is string)
				{
					hashtable[key] = original[key];
				}
			}
		}
		return hashtable;
	}

	public static void StripKeysWithNullValues(this IDictionary original)
	{
		object[] array = new object[original.Count];
		int num = 0;
		foreach (object key2 in original.Keys)
		{
			array[num++] = key2;
		}
		foreach (object key in array)
		{
			if (original[key] == null)
			{
				original.Remove(key);
			}
		}
	}

	public static bool Contains(this int[] target, int nr)
	{
		if (target == null)
		{
			return false;
		}
		for (int i = 0; i < target.Length; i++)
		{
			if (target[i] == nr)
			{
				return true;
			}
		}
		return false;
	}

	public static bool Contains<T>(this T[] target, T nr) where T : IEquatable<T>
	{
		if (target == null)
		{
			return false;
		}
		for (int i = 0; i < target.Length; i++)
		{
			if (target[i].Equals(nr))
			{
				return true;
			}
		}
		return false;
	}

	public static string GetMatKeyword(this CarMatType type)
	{
		string text = "_PAINT_TYPE_";
		return type switch
		{
			CarMatType.Glossy => text + "GLOSS", 
			CarMatType.Metallic => text + "METALLIC", 
			CarMatType.Iridescent => text + "IRIDESCENT", 
			CarMatType.Chrome => text + "CHROME", 
			CarMatType.ChromeIridescent => text + "CHROME_IRIDESCENT", 
			CarMatType.Matte => text + "MATTE", 
			CarMatType.MatteIridescent => text + "MATTE_IRIDESCENT", 
			_ => throw new ArgumentOutOfRangeException("type", type, null), 
		};
	}

	public static void ApplyTypeOnMat(this CarMatType type, Material material)
	{
		float value = 0f;
		float value2 = 0f;
		float value3 = 0f;
		float value4 = 0f;
		float value5 = 0f;
		float value6 = 0f;
		switch (type)
		{
		case CarMatType.Glossy:
			value = 1f;
			value2 = 0f;
			value3 = 0f;
			value4 = 0f;
			value5 = 0f;
			value6 = 0.05f;
			break;
		case CarMatType.Metallic:
			value = 0.5f;
			value2 = 1f;
			value3 = 1f;
			value4 = 0f;
			value5 = 0.2f;
			value6 = 0.05f;
			break;
		case CarMatType.Iridescent:
			value = 0.5f;
			value2 = 1f;
			value3 = 1f;
			value4 = 1f;
			value5 = 0.2f;
			value6 = 0.05f;
			break;
		case CarMatType.Chrome:
			value = 0.95f;
			value2 = 1f;
			value3 = 0f;
			value4 = 0f;
			value5 = 0f;
			value6 = 0f;
			break;
		case CarMatType.ChromeIridescent:
			value = 0.95f;
			value2 = 1f;
			value3 = 0f;
			value4 = 1f;
			value5 = 0f;
			value6 = 0f;
			break;
		case CarMatType.Matte:
			value = 0.3f;
			value2 = 0f;
			value3 = 0f;
			value4 = 0f;
			value5 = 0f;
			value6 = 0f;
			break;
		case CarMatType.MatteIridescent:
			value = 0.3f;
			value2 = 1f;
			value3 = 0f;
			value4 = 1f;
			value5 = 0f;
			value6 = 0f;
			break;
		}
		material.SetFloat(Smoothness, value);
		material.SetFloat(Metalness, value2);
		material.SetFloat(CoatMask, value3);
		material.SetFloat(Iridescence, value4);
		material.SetFloat(FlakesPower, value5);
		material.SetFloat(OrangePeelPower, value6);
	}

	public static List<int> GetFavouriteVinyls(OneVinylData[] AllVinyls)
	{
		if (AllVinyls == null || AllVinyls.Length == 0)
		{
			return new List<int>();
		}
		return (from vinylData in AllVinyls
			where PlayerPrefs.GetInt("FavouriteVinyl" + vinylData.index, 0) == 1
			select vinylData.index).ToList();
	}

	public static List<int> GetFavouriteWheels(this AllWheelsCounterConfig config)
	{
		List<int> list = new List<int>();
		if (config.AllWheelsIndexes == null)
		{
			return list;
		}
		foreach (int allWheelsIndex in config.AllWheelsIndexes)
		{
			if (PlayerPrefs.GetInt("FavouriteWheel" + allWheelsIndex, 0) == 1)
			{
				list.Add(allWheelsIndex);
			}
		}
		return list;
	}
}

using System.Collections.Generic;
using UnityEngine;

public static class TransformExtensions
{
	public static void SetLocalData(this Transform transform, Vector3 position, Quaternion rotation, Vector3 scale)
	{
		if ((bool)transform)
		{
			transform.SetLocalPositionAndRotation(position, rotation);
			transform.localScale = scale;
		}
	}

	public static void SetLocalData(this Transform transform, Transform source)
	{
		if ((bool)transform && (bool)source)
		{
			transform.SetLocalData(source.localPosition, source.localRotation, source.localScale);
		}
	}

	public static Transform FindChildRecursive(this Transform parent, string n)
	{
		Queue<Transform> queue = new Queue<Transform>();
		queue.Enqueue(parent);
		while (queue.Count > 0)
		{
			Transform transform = queue.Dequeue();
			if (!transform)
			{
				continue;
			}
			if (transform.name == n)
			{
				return transform;
			}
			foreach (Transform item in transform)
			{
				queue.Enqueue(item);
			}
		}
		return null;
	}

	public static Transform FindDeepChildV2(this Transform parent, string name)
	{
		if ((bool)parent)
		{
			Transform transform = parent.Find(name);
			if ((bool)transform)
			{
				return transform;
			}
			foreach (Transform item in parent)
			{
				transform = item.FindDeepChildV2(name);
				if ((bool)transform)
				{
					return transform;
				}
			}
		}
		return null;
	}

	public static T FindDeepChildV2<T>(this Transform parent, string name) where T : Component
	{
		Transform transform = parent.FindDeepChildV2(name);
		if ((bool)transform)
		{
			return transform.GetComponent<T>();
		}
		return null;
	}

	public static Transform FindChildContainsName(this Transform parent, string n)
	{
		if ((bool)parent)
		{
			for (int i = 0; i < parent.childCount; i++)
			{
				if (parent.GetChild(i).name.Contains(n))
				{
					return parent.GetChild(i);
				}
			}
		}
		return null;
	}

	public static float OffsetAlongForward(this Transform A, Transform B)
	{
		return Vector3.Dot(B.position - A.position, A.forward);
	}

	public static void RemoveAllChild(this Transform transform)
	{
		int childCount = transform.childCount;
		for (int i = 0; i < childCount; i++)
		{
			Object.Destroy(transform.GetChild(i).gameObject);
		}
	}
}

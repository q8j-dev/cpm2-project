using System;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;

public static class AddressableExtensions
{
	private static readonly Dictionary<(object key, Type type), bool> _existsCache = new Dictionary<(object, Type), bool>();

	public static bool AddressableResourceExists(object key, Type type)
	{
		(object, Type) key2 = (key, type);
		if (_existsCache.TryGetValue(key2, out var value))
		{
			return value;
		}
		bool flag = false;
		bool flag2 = false;
		foreach (IResourceLocator resourceLocator in Addressables.ResourceLocators)
		{
			flag2 = true;
			if (resourceLocator.Locate(key, type, out var _))
			{
				flag = true;
				break;
			}
		}
		if (flag2)
		{
			_existsCache[key2] = flag;
		}
		return flag;
	}
}

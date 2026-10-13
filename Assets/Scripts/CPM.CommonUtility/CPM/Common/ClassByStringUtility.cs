using System;
using UnityEngine;

namespace CPM.Common
{
	public static class ClassByStringUtility
	{
		public static T Create<T>(string typeName) where T : class
		{
			try
			{
				return Activator.CreateInstance(Type.GetType(typeName)) as T;
			}
			catch (Exception arg)
			{
				Debug.LogError($"Could not create type from string {typeName}. Exception:{arg}");
				return null;
			}
		}
	}
}

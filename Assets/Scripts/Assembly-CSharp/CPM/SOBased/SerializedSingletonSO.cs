using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace CPM.SOBased
{
	public class SerializedSingletonSO<T> : SerializedScriptableObject where T : SerializedSingletonSO<T>
	{
		private static T instance;

		public static T Instance
		{
			get
			{
				if (instance == null)
				{
					T[] array = Resources.LoadAll<T>("SingletonSO/");
					if (array == null || array.Length < 1)
					{
						throw new Exception($"Could not found {typeof(T)} scriptable object instance in resources.");
					}
					if (array.Length > 1)
					{
						throw new Exception($"Found multiple {typeof(T)} scriptable object instances in resources");
					}
					instance = array[0];
				}
				return instance;
			}
		}
	}
}

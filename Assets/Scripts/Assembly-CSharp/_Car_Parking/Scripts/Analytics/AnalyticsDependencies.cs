using System;
using System.Collections.Generic;

namespace _Car_Parking.Scripts.Analytics
{
	public static class AnalyticsDependencies
	{
		private static readonly Dictionary<Type, object> Dependencies = new Dictionary<Type, object>();

		public static void RegisterDependence<T>(T service) where T : class
		{
			Type typeFromHandle = typeof(T);
			Dependencies.ContainsKey(typeFromHandle);
			Dependencies[typeFromHandle] = service;
		}

		public static void Unregister<T>(T service) where T : class
		{
			Type typeFromHandle = typeof(T);
			if (Dependencies.ContainsKey(typeFromHandle))
			{
				Dependencies.Remove(typeFromHandle);
			}
		}

		public static T Get<T>() where T : class
		{
			Type typeFromHandle = typeof(T);
			if (Dependencies.TryGetValue(typeFromHandle, out var value))
			{
				return (T)value;
			}
			return null;
		}

		public static bool TryGet<T>(out T service) where T : class
		{
			if (Dependencies.TryGetValue(typeof(T), out var value))
			{
				service = (T)value;
				return service != null;
			}
			service = null;
			return false;
		}
	}
}

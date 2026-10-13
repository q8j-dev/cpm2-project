using System;
using UnityEngine;

namespace FunctionalExtensions
{
	public static class F
	{
		public static TOut Get<TIn, TOut>(this TIn source, Func<TIn, TOut> modification = null) where TIn : class where TOut : class
		{
			if (source != null)
			{
				if (modification == null)
				{
					return null;
				}
				return modification(source);
			}
			return null;
		}

		public static T With<T>(this T source, Action<T> action = null) where T : class
		{
			if (source == null)
			{
				return null;
			}
			action?.Invoke(source);
			return source;
		}

		public static T Identity<T>(T t)
		{
			return t;
		}

		public static bool Exists<T>(T arg) where T : UnityEngine.Object
		{
			return arg;
		}
	}
}

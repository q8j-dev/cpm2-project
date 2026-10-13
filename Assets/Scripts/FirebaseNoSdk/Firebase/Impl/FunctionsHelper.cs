using System;
using System.Collections.Generic;

namespace Firebase.Impl
{
	public static class FunctionsHelper
	{
		private static readonly HashSet<Type> SimpleTypes = new HashSet<Type>
		{
			typeof(bool),
			typeof(int),
			typeof(string),
			typeof(object)
		};

		public static bool IsNullableValueType<TRes>()
		{
			Type typeFromHandle = typeof(TRes);
			if (typeFromHandle.IsGenericType && typeFromHandle.GetGenericTypeDefinition() == typeof(Nullable<>))
			{
				return typeFromHandle.GetGenericArguments()[0].IsValueType;
			}
			return false;
		}

		public static bool UseOneStep<TRes>()
		{
			if (!SimpleTypes.Contains(typeof(TRes)))
			{
				return IsNullableValueType<TRes>();
			}
			return true;
		}
	}
}

using System;
using System.Reflection;
using System.Runtime.Serialization;

namespace _Car_Parking.Scripts
{
	public static class EnumMemberExtensions
	{
		public static string GetEnumMemberValue(this Enum value)
		{
			FieldInfo field = value.GetType().GetField(value.ToString());
			if (field == null)
			{
				return value.ToString();
			}
			return field.GetCustomAttribute<EnumMemberAttribute>()?.Value ?? value.ToString();
		}

		public static T GetEnumFromMemberValue<T>(string enumMemberValue) where T : struct, Enum
		{
			foreach (T value in Enum.GetValues(typeof(T)))
			{
				if (value.GetEnumMemberValue() == enumMemberValue)
				{
					return value;
				}
			}
			if (Enum.TryParse<T>(enumMemberValue, out var result))
			{
				return result;
			}
			throw new ArgumentException("No enum value found for EnumMember value: " + enumMemberValue);
		}
	}
}

using System;
using System.Collections;
using System.Reflection;
using System.Runtime.InteropServices;

namespace _Car_Parking.Scripts.CommonUtility.Contracts
{
	public static class ObjectArraySizeCalculator
	{
		public static long CalculateObjectSize(object obj)
		{
			if (obj == null)
			{
				return IntPtr.Size;
			}
			Type type = obj.GetType();
			if (type.IsValueType)
			{
				return Marshal.SizeOf(type);
			}
			if (type.IsArray)
			{
				long num = IntPtr.Size;
				{
					foreach (object item in (IEnumerable)obj)
					{
						num += CalculateObjectSize(item);
					}
					return num;
				}
			}
			FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			long num2 = IntPtr.Size;
			FieldInfo[] array = fields;
			foreach (FieldInfo fieldInfo in array)
			{
				num2 = ((!fieldInfo.FieldType.IsValueType) ? (num2 + CalculateObjectSize(fieldInfo.GetValue(obj))) : (num2 + Marshal.SizeOf(fieldInfo.FieldType)));
			}
			return num2;
		}

		public static long CalculateArraySize(object[] objects)
		{
			long num = 0L;
			foreach (object obj in objects)
			{
				num += CalculateObjectSize(obj);
			}
			return num;
		}
	}
}

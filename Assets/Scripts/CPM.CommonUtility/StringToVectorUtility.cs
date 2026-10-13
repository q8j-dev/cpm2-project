using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public static class StringToVectorUtility
{
	public static List<Vector3> stringToVectors(List<string> stringList)
	{
		List<Vector3> list = new List<Vector3>();
		foreach (string @string in stringList)
		{
			list.Add(StringToVector3(@string));
		}
		return list;
	}

	public static Vector3 StringToVector3(string sVector)
	{
		string[] array = sVector.Split(',');
		if (array.Length == 0)
		{
			return Vector3.zero;
		}
		float result = 0f;
		if (array.Length != 0)
		{
			float.TryParse(array[0], NumberStyles.Any, CultureInfo.InvariantCulture, out result);
		}
		float result2 = 0f;
		if (array.Length > 1)
		{
			float.TryParse(array[1], NumberStyles.Any, CultureInfo.InvariantCulture, out result2);
		}
		float result3 = 0f;
		if (array.Length > 2)
		{
			float.TryParse(array[2], NumberStyles.Any, CultureInfo.InvariantCulture, out result3);
		}
		return new Vector3(result, result2, result3);
	}
}

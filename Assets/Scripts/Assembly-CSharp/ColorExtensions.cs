using UnityEngine;

public static class ColorExtensions
{
	public static bool AlmostEqual(this Color me, Color other, float precision)
	{
		if (Mathf.Abs(me.r - other.r) > precision)
		{
			return false;
		}
		if (Mathf.Abs(me.g - other.g) > precision)
		{
			return false;
		}
		if (Mathf.Abs(me.b - other.b) > precision)
		{
			return false;
		}
		return Mathf.Abs(me.a - other.a) <= precision;
	}

	public static string Colored(this string s, Color color)
	{
		return "<color=#" + ColorUtility.ToHtmlStringRGB(color) + ">" + s + "</color>";
	}
}

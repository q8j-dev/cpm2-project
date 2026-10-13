using UnityEngine;

public class ColorHelperUtility : MonoBehaviour
{
	public static Color SetColorWithAlpha(Color color, float alpha)
	{
		color.a = alpha;
		return color;
	}

	public static Color GetColor(string colorKey)
	{
		if (ColorUtility.TryParseHtmlString(colorKey, out var color))
		{
			return color;
		}
		return Color.white;
	}
}

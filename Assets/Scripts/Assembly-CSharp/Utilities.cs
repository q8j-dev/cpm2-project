using System.Text.RegularExpressions;
using UnityEngine;

public class Utilities : MonoBehaviour
{
	public static GameObject CreateInstance(GameObject original, GameObject parent, bool isActive)
	{
		GameObject obj = Object.Instantiate(original, parent.transform, worldPositionStays: false);
		obj.SetActive(isActive);
		return obj;
	}

	public static float ClockwiseAngleOf3Points(Vector2 A, Vector2 B, Vector2 C)
	{
		Vector2 vector = A - B;
		Vector2 to = C - B;
		float num = Mathf.Sign(vector.x * to.y - vector.y * to.x) * -1f;
		float num2 = Vector2.Angle(vector, to) * num;
		if (num2 < 0f)
		{
			num2 = 360f + num2;
		}
		return num2;
	}

	public static string CleanStringForFloat(string input)
	{
		if (string.IsNullOrEmpty(input))
		{
			return "";
		}
		if (Regex.Match(input, "^-?[0-9]*(?:\\.[0-9]*)?$").Success)
		{
			return input;
		}
		return "0";
	}

	public static string CleanStringForInt(string input)
	{
		if (string.IsNullOrEmpty(input))
		{
			return "";
		}
		if (Regex.Match(input, "([-+]?[0-9]+)").Success)
		{
			return input;
		}
		return "0";
	}
}

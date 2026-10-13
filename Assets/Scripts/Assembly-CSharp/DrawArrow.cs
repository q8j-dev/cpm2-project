using UnityEngine;

public static class DrawArrow
{
	public static void ForGizmo(Vector3 pos, Vector3 direction, Color? color = null, bool doubled = false, float arrowHeadLength = 0.2f, float arrowHeadAngle = 20f)
	{
		Gizmos.color = color ?? Color.white;
		Gizmos.DrawRay(pos, direction);
		if (direction != Vector3.zero)
		{
			Vector3 vector = Quaternion.LookRotation(direction) * Quaternion.Euler(0f, 180f + arrowHeadAngle, 0f) * new Vector3(0f, 0f, 1f);
			Vector3 vector2 = Quaternion.LookRotation(direction) * Quaternion.Euler(0f, 180f - arrowHeadAngle, 0f) * new Vector3(0f, 0f, 1f);
			Gizmos.DrawRay(pos + direction, vector * arrowHeadLength);
			Gizmos.DrawRay(pos + direction, vector2 * arrowHeadLength);
		}
	}

	public static void ForDebug(Vector3 pos, Vector3 direction, float duration = 0.5f, Color? color = null, ArrowType type = ArrowType.Default, float arrowHeadLength = 0.2f, float arrowHeadAngle = 30f, bool sceneCamFollows = false)
	{
		Color color2 = color ?? Color.white;
		duration /= Time.timeScale;
		float num = 0.02f;
		Vector3 vector = Vector3.zero;
		Vector3 vector2 = Vector3.zero;
		_ = Vector3.zero;
		Vector3 vector3 = Vector3.zero;
		Vector3 vector4 = Vector3.zero;
		if (direction != Vector3.zero)
		{
			vector = Quaternion.LookRotation(direction) * Quaternion.Euler(0f, 270f, 0f) * new Vector3(0f, 0f, 1f);
			vector2 = Quaternion.LookRotation(direction) * Quaternion.Euler(0f, 90f, 0f) * new Vector3(0f, 0f, 1f);
			_ = Quaternion.LookRotation(direction) * Quaternion.Euler(0f, 180f, 0f) * new Vector3(0f, 0f, 1f);
			vector3 = Quaternion.LookRotation(direction) * Quaternion.Euler(0f, 180f + arrowHeadAngle, 0f) * new Vector3(0f, 0f, 1f);
			vector4 = Quaternion.LookRotation(direction) * Quaternion.Euler(0f, 180f - arrowHeadAngle, 0f) * new Vector3(0f, 0f, 1f);
		}
		Debug.DrawRay(pos + direction, vector3 * arrowHeadLength, color2, duration);
		Debug.DrawRay(pos + direction, vector4 * arrowHeadLength, color2, duration);
		switch (type)
		{
		case ArrowType.Default:
			Debug.DrawRay(pos, direction, color2, duration);
			break;
		case ArrowType.Double:
		{
			Debug.DrawRay(pos + vector * num, direction * (1f - num), color2, duration);
			Debug.DrawRay(pos + vector2 * num, direction * (1f - num), color2, duration);
			int num4 = 5;
			for (int j = 0; j < num4; j++)
			{
				float num5 = Mathf.Lerp(0f - num, num, (float)j / (float)num4);
				Debug.DrawRay(pos + direction + vector * num5, vector3 * arrowHeadLength, color2, duration);
				Debug.DrawRay(pos + direction + vector * num5, vector4 * arrowHeadLength, color2, duration);
			}
			break;
		}
		case ArrowType.Triple:
			Debug.DrawRay(pos, direction, color2, duration);
			Debug.DrawRay(pos + vector * num, direction * (1f - num), color2, duration);
			Debug.DrawRay(pos + vector2 * num, direction * (1f - num), color2, duration);
			break;
		case ArrowType.Solid:
		{
			int num2 = 20;
			for (int i = 0; i < num2; i++)
			{
				float num3 = Mathf.Lerp(0f - num, num, (float)i / (float)num2);
				Debug.DrawRay(pos + vector * num3, direction, color2, duration);
				Debug.DrawRay(pos + vector2 * num3, direction, color2, duration);
				Debug.DrawRay(pos + direction + vector * num3, vector3 * arrowHeadLength, color2, duration);
				Debug.DrawRay(pos + direction + vector * num3, vector4 * arrowHeadLength, color2, duration);
			}
			break;
		}
		case ArrowType.Thin:
			Debug.DrawRay(pos, direction, color2, duration);
			break;
		case ArrowType.Fat:
		case ArrowType.ThreeD:
			break;
		}
	}

	public static void randomStar(Vector3 center, Color color)
	{
		if (!(center == Vector3.zero))
		{
			for (int i = 0; i < 2; i++)
			{
				ForGizmo(center, Random.onUnitSphere * 1f, color, doubled: false, 0.1f, 30f);
			}
		}
	}

	public static void comparePositions(Transform t1, Transform t2)
	{
		ForDebug(t1.position, t2.position - t1.position);
	}
}

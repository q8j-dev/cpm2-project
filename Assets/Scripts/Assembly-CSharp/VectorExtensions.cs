using CPMEngine;
using UnityEngine;

public static class VectorExtensions
{
	public static float Clamp(this Vector2 source, float value)
	{
		if (!(value < source.x))
		{
			if (!(value > source.y))
			{
				return value;
			}
			return source.y;
		}
		return source.x;
	}

	public static float Lerp(this Vector2 source, float value)
	{
		return Mathf.Lerp(source.x, source.y, value);
	}

	public static float InverseLerp(this Vector2 source, float value)
	{
		return Mathf.InverseLerp(source.x, source.y, value);
	}

	public static bool Exceeds(this Vector3 source, float distance)
	{
		if (!(Mathf.Abs(source.x) > distance) && !(Mathf.Abs(source.y) > distance))
		{
			return Mathf.Abs(source.z) > distance;
		}
		return true;
	}

	public static float Distance(this Vector3 source, Vector3 other)
	{
		return (source - other).magnitude;
	}

	public static float Distance(this Vector3 source, Vector3 other, out Vector3 delta)
	{
		Vector3 vector = (delta = source - other);
		return vector.magnitude;
	}

	public static bool CloserThan(this Vector3 source, Vector3 other, float radius)
	{
		return (source - other).sqrMagnitude < radius * radius;
	}

	public static bool CloserThanWithSqrRadius(this Vector3 source, Vector3 other, float sqrRadius)
	{
		return (source - other).sqrMagnitude < sqrRadius;
	}

	public static bool CloserThan(this Vector3 source, Vector3 other, float radius, out Vector3 delta)
	{
		Vector3 vector = (delta = source - other);
		return vector.sqrMagnitude < radius * radius;
	}

	public static bool FurtherThan(this Vector3 source, Vector3 other, float radius)
	{
		return (source - other).sqrMagnitude > radius * radius;
	}

	public static bool FurtherThan(this Vector3 source, Vector3 other, float radius, out Vector3 delta)
	{
		Vector3 vector = (delta = source - other);
		return vector.sqrMagnitude > radius * radius;
	}

	public static Vector3Int RoundedToTens(this Vector3Int vector)
	{
		return new Vector3Int(vector.x / 10 * 10, vector.y / 10 * 10, vector.z / 10 * 10);
	}

	public static Vector3Int RoundedToHundreds(this Vector3Int vector)
	{
		return new Vector3Int(vector.x / 100 * 100, vector.y / 100 * 100, vector.z / 100 * 100);
	}

	public static Vector3 Clamp(this Vector3 source, Vector3 min, Vector3 max)
	{
		return Vector3.Max(Vector3.Min(source, max), min);
	}

	public static bool IsDistanceMoreThan(Vector3 a, Vector3 b, float d)
	{
		float num = a.x - b.x;
		float num2 = a.y - b.y;
		float num3 = a.z - b.z;
		return (double)num * (double)num + (double)num2 * (double)num2 + (double)num3 * (double)num3 > (double)(d * d);
	}

	public static bool IsDistanceLessThan(Vector3 a, Vector3 b, float d)
	{
		float num = a.x - b.x;
		float num2 = a.y - b.y;
		float num3 = a.z - b.z;
		return num * num + num2 * num2 + num3 * num3 < d * d;
	}

	public static Vector3 GetPosByWheel(GameObject car, Vector3 origin)
	{
		if (!car.TryGetComponent<Axles>(out var component))
		{
			origin.y += 0.4f;
			return origin;
		}
		Transform transform = (component.AllWheels[0].WheelModel ? component.AllWheels[0].WheelModel.transform : component.AllWheels[0].transform);
		float num = Vector3.Dot(car.transform.position - transform.position, car.transform.up);
		origin.y += num + component.AllWheels[0].Wheel.Radius;
		return origin;
	}
}

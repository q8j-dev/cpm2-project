using System;
using UnityEngine;

[ExecuteAlways]
public class SamplingDebug : MonoBehaviour
{
	public Vector3 Dir = Vector3.up;

	public int Samples = 512;

	private Vector3[] SphereSamples;

	private void Awake()
	{
		SphereSamples = FibonacciSphere(0, Samples);
	}

	private void LateUpdate()
	{
	}

	public static Vector3[] FibonacciSphere(int startIndex, int n)
	{
		Vector3[] array = new Vector3[n];
		float num = (float)Math.PI * (3f - Mathf.Sqrt(5f));
		for (int i = 0; i < n; i++)
		{
			float num2 = 1f - (float)i / (float)(n - 1) * 2f;
			float num3 = Mathf.Sqrt(1f - num2 * num2);
			float f = num * (float)i;
			float x = Mathf.Cos(f) * num3;
			float z = Mathf.Sin(f) * num3;
			array[startIndex + i] = Vector3.Normalize(new Vector3(x, num2, z));
		}
		return array;
	}
}

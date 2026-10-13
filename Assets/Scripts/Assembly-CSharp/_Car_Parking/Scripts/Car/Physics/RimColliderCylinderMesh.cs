using System;
using UnityEngine;

namespace _Car_Parking.Scripts.Car.Physics
{
	public static class RimColliderCylinderMesh
	{
		private const int Sides = 16;

		private const float Radius = 0.5f;

		private const float HalfLength = 0.5f;

		private static Mesh s_mesh;

		public static Mesh Get()
		{
			if ((bool)s_mesh)
			{
				return s_mesh;
			}
			int num = 0;
			int num2 = 1;
			int num3 = 17;
			int num4 = num3 + 1;
			Vector3[] array = new Vector3[34];
			array[num] = new Vector3(-0.5f, 0f, 0f);
			array[num3] = new Vector3(0.5f, 0f, 0f);
			for (int i = 0; i < 16; i++)
			{
				float f = (float)i * (float)Math.PI * 2f / 16f;
				float y = Mathf.Cos(f) * 0.5f;
				float z = Mathf.Sin(f) * 0.5f;
				array[num2 + i] = new Vector3(-0.5f, y, z);
				array[num4 + i] = new Vector3(0.5f, y, z);
			}
			int[] array2 = new int[192];
			int num5 = 0;
			for (int j = 0; j < 16; j++)
			{
				int num6 = (j + 1) % 16;
				array2[num5++] = num;
				array2[num5++] = num2 + num6;
				array2[num5++] = num2 + j;
				array2[num5++] = num3;
				array2[num5++] = num4 + j;
				array2[num5++] = num4 + num6;
				int num7 = num2 + j;
				int num8 = num2 + num6;
				int num9 = num4 + j;
				int num10 = num4 + num6;
				array2[num5++] = num7;
				array2[num5++] = num8;
				array2[num5++] = num10;
				array2[num5++] = num7;
				array2[num5++] = num10;
				array2[num5++] = num9;
			}
			Mesh mesh = new Mesh();
			mesh.name = "OpenWheelRimColliderCylinder";
			mesh.SetVertices(array);
			mesh.SetTriangles(array2, 0);
			mesh.RecalculateNormals();
			mesh.RecalculateBounds();
			s_mesh = mesh;
			return s_mesh;
		}
	}
}

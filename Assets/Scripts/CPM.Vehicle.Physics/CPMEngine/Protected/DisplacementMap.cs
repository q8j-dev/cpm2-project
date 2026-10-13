using System;
using UnityEngine;

namespace CPMEngine.Protected
{
	internal class DisplacementMap
	{
		private float[] _map;

		private int _sizeX;

		private int _sizeY;

		private readonly MTRandom _random;

		public DisplacementMap()
		{
			_random = new MTRandom();
			RegenerateMap(128, 128);
		}

		private float GetVal(int i, int j)
		{
			WrapInt(ref i, _sizeY);
			WrapInt(ref j, _sizeX);
			return _map[i * _sizeX + j];
		}

		public float GetValue(float x, float y, float ampMin, float ampMax, Vector2 rangeMin, Vector2 rangeMax)
		{
			float num = (x - rangeMin.x) / (rangeMax.x - rangeMin.x);
			float num2 = (y - rangeMin.y) / (rangeMax.y - rangeMin.y);
			float num3 = (num - 0.5f / (float)_sizeX) * (float)_sizeX;
			float num4 = (num2 - 0.5f / (float)_sizeY) * (float)_sizeY;
			int num5 = Mathf.FloorToInt(num3);
			int num6 = Mathf.FloorToInt(num4);
			float t = num3 - (float)num5;
			float t2 = num4 - (float)num6;
			int j = num5 + 1;
			int i = num6 + 1;
			float val = GetVal(num6, num5);
			float val2 = GetVal(i, num5);
			float val3 = GetVal(num6, j);
			float val4 = GetVal(i, j);
			float a = Mathf.Lerp(val, val2, t2);
			float b = Mathf.Lerp(val3, val4, t2);
			float num7 = Mathf.Lerp(a, b, t);
			return ampMin + num7 * (ampMax - ampMin);
		}

		private void PutHill(int hx, int hy, float height, int size)
		{
			for (int i = hy - size; i <= hy + size; i++)
			{
				for (int j = hx - size; j <= hx + size; j++)
				{
					float num = Mathf.Sqrt((hx - j) * (hx - j) + (hy - i) * (hy - i)) / (float)size;
					if (num <= 1.001f)
					{
						float num2 = height * (1f - num * num);
						float val = GetVal(i, j);
						SetVal(i, j, val + num2);
					}
				}
			}
		}

		private void RegenerateMap(int sizeX, int sizeY)
		{
			_sizeX = sizeX;
			_sizeY = sizeY;
			_map = new float[_sizeX * _sizeY];
			Array.Clear(_map, 0, _map.Length);
			_random.Seed(5489u);
			int num = 10;
			int num2 = 20;
			int num3 = 50;
			for (int i = 0; i < num; i++)
			{
				int hx = (int)(_random.Next() % (uint)_sizeX);
				int hy = (int)(_random.Next() % (uint)_sizeY);
				float height = (float)(_random.Next() % 2000) / 999f;
				PutHill(hx, hy, height, 20);
			}
			for (int j = 0; j < num2; j++)
			{
				int hx2 = (int)(_random.Next() % (uint)_sizeX);
				int hy2 = (int)(_random.Next() % (uint)_sizeY);
				float height2 = (float)(_random.Next() % 1500) / 999f;
				PutHill(hx2, hy2, height2, 15);
			}
			for (int k = 0; k < num3; k++)
			{
				int hx3 = (int)(_random.Next() % (uint)_sizeX);
				int hy3 = (int)(_random.Next() % (uint)_sizeY);
				float height3 = (float)(_random.Next() % 600) / 999f;
				PutHill(hx3, hy3, height3, 5);
			}
			float[] array = new float[_sizeX * _sizeY];
			for (int l = 0; l < _sizeY; l++)
			{
				for (int m = 0; m < _sizeX; m++)
				{
					array[l * _sizeX + m] = Smooth(m, l, 5);
				}
			}
			_map = array;
		}

		private void SetVal(int i, int j, float val)
		{
			WrapInt(ref i, _sizeY);
			WrapInt(ref j, _sizeX);
			_map[i * _sizeX + j] = val;
		}

		private float Smooth(int hx, int hy, int size)
		{
			float num = 0f;
			float num2 = 0f;
			for (int i = hy - size; i <= hy + size; i++)
			{
				for (int j = hx - size; j <= hx + size; j++)
				{
					float num3 = Mathf.Sqrt((hx - j) * (hx - j) + (hy - i) * (hy - i)) / (float)size;
					if (num3 <= 1.001f)
					{
						float val = GetVal(i, j);
						float num4 = 1f - num3;
						num2 += num4;
						num += val * num4;
					}
				}
			}
			return Mathf.Min(num / num2, 1f);
		}

		private void WrapInt(ref int val, int size)
		{
			val = ((val < 0) ? (size - 1 - -val % size) : (val % size));
		}
	}
}

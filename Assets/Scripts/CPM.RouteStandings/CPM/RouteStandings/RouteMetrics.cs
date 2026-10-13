using System;
using System.Collections.Generic;
using UnityEngine;

namespace CPM.RouteStandings
{
	public sealed class RouteMetrics
	{
		private readonly Vector3[] _points;

		private readonly float[] _remaining;

		private readonly float[] _segmentLength;

		private const float SearchWindowMeters = 150f;

		private const float ReacquireDistance = 50f;

		public bool IsValid => _points.Length >= 2;

		public float TotalLength
		{
			get
			{
				if (!IsValid)
				{
					return 0f;
				}
				return _remaining[0];
			}
		}

		public RouteMetrics(IReadOnlyList<Vector3> points)
		{
			int num = points?.Count ?? 0;
			if (num < 2)
			{
				_points = Array.Empty<Vector3>();
				_remaining = Array.Empty<float>();
				_segmentLength = Array.Empty<float>();
				return;
			}
			_points = new Vector3[num];
			for (int i = 0; i < num; i++)
			{
				_points[i] = points[i];
			}
			_segmentLength = new float[num - 1];
			for (int j = 0; j < num - 1; j++)
			{
				_segmentLength[j] = Vector3.Distance(_points[j], _points[j + 1]);
			}
			_remaining = new float[num];
			_remaining[num - 1] = 0f;
			for (int num2 = num - 2; num2 >= 0; num2--)
			{
				_remaining[num2] = _remaining[num2 + 1] + _segmentLength[num2];
			}
		}

		public float RemainingFrom(Vector3 position, ref int cursor)
		{
			if (!IsValid)
			{
				return 0f;
			}
			int num = _segmentLength.Length - 1;
			float bestSqr = float.MaxValue;
			int bestSegment = 0;
			float bestT = 0f;
			int num2;
			if (cursor >= 0)
			{
				num2 = ((cursor <= num) ? 1 : 0);
				if (num2 != 0)
				{
					Search(position, WindowStart(cursor), WindowEnd(cursor), ref bestSqr, ref bestSegment, ref bestT);
				}
			}
			else
			{
				num2 = 0;
			}
			if (num2 == 0 || bestSqr > 2500f)
			{
				Search(position, 0, num, ref bestSqr, ref bestSegment, ref bestT);
			}
			cursor = bestSegment;
			return Mathf.Max(0f, _remaining[bestSegment] - bestT * _segmentLength[bestSegment]);
		}

		private void Search(Vector3 position, int from, int to, ref float bestSqr, ref int bestSegment, ref float bestT)
		{
			for (int i = from; i <= to; i++)
			{
				float num = ClosestT(position, i);
				Vector3 vector = Vector3.Lerp(_points[i], _points[i + 1], num);
				float sqrMagnitude = (position - vector).sqrMagnitude;
				if (!(sqrMagnitude >= bestSqr))
				{
					bestSqr = sqrMagnitude;
					bestSegment = i;
					bestT = num;
				}
			}
		}

		private float ClosestT(Vector3 position, int segment)
		{
			Vector3 vector = _points[segment];
			Vector3 rhs = _points[segment + 1] - vector;
			float sqrMagnitude = rhs.sqrMagnitude;
			if (sqrMagnitude <= 1E-06f)
			{
				return 0f;
			}
			return Mathf.Clamp01(Vector3.Dot(position - vector, rhs) / sqrMagnitude);
		}

		private int WindowStart(int cursor)
		{
			float num = _remaining[cursor] + 150f;
			int num2 = cursor;
			while (num2 > 0 && _remaining[num2] < num)
			{
				num2--;
			}
			return num2;
		}

		private int WindowEnd(int cursor)
		{
			float num = _remaining[cursor] - 150f;
			int num2 = _segmentLength.Length - 1;
			int i;
			for (i = cursor; i < num2 && _remaining[i] > num; i++)
			{
			}
			return i;
		}
	}
}

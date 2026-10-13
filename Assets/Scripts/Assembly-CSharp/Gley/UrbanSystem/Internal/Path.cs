using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gley.UrbanSystem.Internal
{
	[Serializable]
	public class Path
	{
		[SerializeField]
		private List<Vector3> points;

		[SerializeField]
		private bool isClosed;

		[SerializeField]
		private bool autoSetControlPoints;

		public Vector3 this[int i]
		{
			get
			{
				return points[i];
			}
			set
			{
				points[i] = value;
			}
		}

		public bool IsClosed
		{
			get
			{
				return isClosed;
			}
			set
			{
				if (isClosed == value)
				{
					return;
				}
				isClosed = value;
				if (isClosed)
				{
					points.Add(points[points.Count - 1] * 2f - points[points.Count - 2]);
					points.Add(points[0] * 2f - points[1]);
					if (autoSetControlPoints)
					{
						AutoSetAnchorControlPoints(0);
						AutoSetAnchorControlPoints(points.Count - 3);
					}
				}
				else
				{
					points.RemoveRange(points.Count - 2, 2);
					if (autoSetControlPoints)
					{
						AutoSetStartAndEndControls();
					}
				}
			}
		}

		public bool AutoSetControlPoints
		{
			get
			{
				return autoSetControlPoints;
			}
			set
			{
				if (autoSetControlPoints != value)
				{
					autoSetControlPoints = value;
					if (autoSetControlPoints)
					{
						AutoSetAllControlPoints();
					}
				}
			}
		}

		public int NumPoints => points.Count;

		public int NumSegments => points.Count / 3;

		public Path(Vector3 startPosition, Vector3 endPosition)
		{
			points = new List<Vector3>
			{
				startPosition,
				(startPosition + endPosition) / 2f,
				(startPosition + endPosition) / 2f,
				endPosition
			};
		}

		public Path(Vector3 startPosition, Vector3 startTangent, Vector3 endPosition, Vector3 endTangent)
		{
			float num = Vector3.Distance(startPosition, endPosition);
			points = new List<Vector3>
			{
				startPosition,
				startPosition + startTangent.normalized * (num / 2f),
				endPosition - endTangent.normalized * (num / 2f),
				endPosition
			};
		}

		public void AddSegment(Vector3 anchorPos)
		{
			points.Add(points[points.Count - 1] * 2f - points[points.Count - 2]);
			points.Add((points[points.Count - 1] + anchorPos) * 0.5f);
			points.Add(anchorPos);
			if (autoSetControlPoints)
			{
				AutoSetAllAffectedControlPoints(points.Count - 1);
			}
		}

		public void SplitSegment(Vector3 anchorPos, int segmentIndex)
		{
			points.InsertRange(segmentIndex * 3 + 2, new Vector3[3]
			{
				Vector3.zero,
				anchorPos,
				Vector3.zero
			});
			if (autoSetControlPoints)
			{
				AutoSetAllAffectedControlPoints(segmentIndex * 3 + 3);
			}
			else
			{
				AutoSetAnchorControlPoints(segmentIndex * 3 + 3);
			}
		}

		public void DeleteSegment(int anchorIndex)
		{
			if (NumSegments <= 2 && (isClosed || NumSegments <= 1))
			{
				return;
			}
			if (anchorIndex == 0)
			{
				if (isClosed)
				{
					points[points.Count - 1] = points[2];
				}
				points.RemoveRange(0, 3);
			}
			else if (anchorIndex == points.Count - 1 && !isClosed)
			{
				points.RemoveRange(anchorIndex - 2, 3);
			}
			else
			{
				points.RemoveRange(anchorIndex - 1, 3);
			}
		}

		public Vector3[] GetPointsInSegment(int i, Vector3 offset)
		{
			return new Vector3[4]
			{
				points[i * 3] + offset,
				points[i * 3 + 1] + offset,
				points[i * 3 + 2] + offset,
				points[LoopIndex(i * 3 + 3)] + offset
			};
		}

		public Vector3 GetPoint(int i, Vector3 offset)
		{
			return points[i] + offset;
		}

		public void MovePoint(int i, Vector3 pos)
		{
			Vector3 vector = pos - points[i];
			if (i % 3 != 0 && autoSetControlPoints)
			{
				return;
			}
			points[i] = pos;
			if (autoSetControlPoints)
			{
				AutoSetAllAffectedControlPoints(i);
				return;
			}
			if (i % 3 == 0)
			{
				if (i + 1 < points.Count || isClosed)
				{
					points[LoopIndex(i + 1)] += vector;
				}
				if (i - 1 >= 0 || isClosed)
				{
					points[LoopIndex(i - 1)] += vector;
				}
				return;
			}
			bool num = (i + 1) % 3 == 0;
			int num2 = (num ? (i + 2) : (i - 2));
			int i2 = (num ? (i + 1) : (i - 1));
			if ((num2 >= 0 && num2 < points.Count) || isClosed)
			{
				float magnitude = (points[LoopIndex(i2)] - points[LoopIndex(num2)]).magnitude;
				Vector3 normalized = (points[LoopIndex(i2)] - pos).normalized;
				points[LoopIndex(num2)] = points[LoopIndex(i2)] + normalized * magnitude;
			}
		}

		private void AutoSetAllAffectedControlPoints(int updatedAnchorIndex)
		{
			for (int i = updatedAnchorIndex - 3; i <= updatedAnchorIndex + 3; i += 3)
			{
				if ((i >= 0 && i < points.Count) || isClosed)
				{
					AutoSetAnchorControlPoints(LoopIndex(i));
				}
			}
			AutoSetStartAndEndControls();
		}

		private void AutoSetAllControlPoints()
		{
			for (int i = 0; i < points.Count; i += 3)
			{
				AutoSetAnchorControlPoints(i);
			}
			AutoSetStartAndEndControls();
		}

		private void AutoSetAnchorControlPoints(int anchorIndex)
		{
			Vector3 vector = points[anchorIndex];
			Vector3 zero = Vector3.zero;
			float[] array = new float[2];
			if (anchorIndex - 3 >= 0 || isClosed)
			{
				Vector3 vector2 = points[LoopIndex(anchorIndex - 3)] - vector;
				zero += vector2.normalized;
				array[0] = vector2.magnitude;
			}
			if (anchorIndex + 3 >= 0 || isClosed)
			{
				Vector3 vector3 = points[LoopIndex(anchorIndex + 3)] - vector;
				zero -= vector3.normalized;
				array[1] = 0f - vector3.magnitude;
			}
			zero.Normalize();
			for (int i = 0; i < 2; i++)
			{
				int num = anchorIndex + i * 2 - 1;
				if ((num >= 0 && num < points.Count) || isClosed)
				{
					points[LoopIndex(num)] = vector + zero * array[i] * 0.5f;
				}
			}
		}

		private void AutoSetStartAndEndControls()
		{
			if (!isClosed)
			{
				points[1] = (points[0] + points[2]) * 0.5f;
				points[points.Count - 2] = (points[points.Count - 1] + points[points.Count - 3]) * 0.5f;
			}
		}

		private int LoopIndex(int i)
		{
			return (i + points.Count) % points.Count;
		}
	}
}

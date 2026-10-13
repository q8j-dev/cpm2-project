using System;
using System.Collections.Generic;
using ChartAndGraph;
using UnityEngine;

public class GraphEditable : MonoBehaviour
{
	public GraphChart Graph;

	public string Category;

	public RectTransform LastPoint;

	private List<DoubleVector3> mDataArray = new List<DoubleVector3>();

	private const double MaxDistSqr = 0.0006250000000000001;

	private int mMovingPoint = -1;

	private int mLastMovingPoint = -1;

	private void Start()
	{
		if (Graph != null)
		{
			Graph.OnRedraw.AddListener(Redraw);
		}
	}

	private void Redraw()
	{
		if (mLastMovingPoint < 0)
		{
			if (LastPoint != null && LastPoint.gameObject.activeSelf)
			{
				LastPoint.gameObject.SetActive(value: false);
			}
			return;
		}
		DoubleVector3 doubleVector = mDataArray[mLastMovingPoint];
		if (Graph.PointToWorldSpace(out var result, doubleVector.x, doubleVector.y, Category) && LastPoint != null)
		{
			if (!LastPoint.gameObject.activeSelf)
			{
				LastPoint.gameObject.SetActive(value: true);
			}
			LastPoint.transform.position = result;
		}
	}

	private int FindNearPoint(DoubleVector3 position)
	{
		int result = -1;
		double num = double.PositiveInfinity;
		for (int i = 0; i < mDataArray.Count; i++)
		{
			DoubleVector3 doubleVector = mDataArray[i] - position;
			doubleVector.x /= Graph.DataSource.HorizontalViewSize;
			doubleVector.y /= Graph.DataSource.VerticalViewSize;
			double num2 = doubleVector.x * doubleVector.x + doubleVector.y * doubleVector.y;
			if (num2 < 0.0006250000000000001 && num2 < num)
			{
				num = num2;
				result = i;
			}
		}
		return result;
	}

	private int InsertPoint(DoubleVector3 point)
	{
		int i;
		for (i = 0; i < mDataArray.Count && !(point.x <= mDataArray[i].x); i++)
		{
		}
		mDataArray.Insert(i, point);
		return i;
	}

	private void Update()
	{
		Vector2 vector = Input.mousePosition;
		Graph.PointToClient((Vector3)vector, out double x, out double y);
		DoubleVector3 doubleVector = new DoubleVector3(x, y, 0.0);
		if (Input.GetMouseButtonDown(0))
		{
			mMovingPoint = FindNearPoint(doubleVector);
			if (mMovingPoint == -1)
			{
				mMovingPoint = InsertPoint(doubleVector);
			}
			mLastMovingPoint = mMovingPoint;
		}
		else if (Input.GetMouseButtonUp(0))
		{
			mMovingPoint = -1;
		}
		if (Input.GetKeyDown(KeyCode.Delete) && mLastMovingPoint >= 0)
		{
			mDataArray.RemoveAt(mLastMovingPoint);
			mLastMovingPoint = -1;
			mMovingPoint = -1;
		}
		if (mMovingPoint != -1)
		{
			DoubleVector3 value = doubleVector;
			if (mMovingPoint > 0)
			{
				value.x = Math.Max(mDataArray[mMovingPoint - 1].x, value.x);
			}
			if (mMovingPoint + 1 < mDataArray.Count)
			{
				value.x = Math.Min(mDataArray[mMovingPoint + 1].x, value.x);
			}
			mDataArray[mMovingPoint] = value;
		}
		Graph.DataSource.SetCategoryArray(Category, mDataArray, 0, mDataArray.Count);
	}
}

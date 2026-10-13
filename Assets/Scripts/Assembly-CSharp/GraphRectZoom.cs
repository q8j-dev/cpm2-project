using System;
using ChartAndGraph;
using UnityEngine;

[RequireComponent(typeof(CustomChartPointer))]
public class GraphRectZoom : MonoBehaviour
{
	public RectTransform Marker;

	public int MinMarkerPixels = 20;

	private CustomChartPointer mPointer;

	private RectTransform mRect;

	private bool mIsDown;

	private Vector2 mStart;

	private bool mIsZoomed;

	private bool mIsUp = true;

	private void Start()
	{
		mRect = GetComponent<RectTransform>();
		mPointer = GetComponent<CustomChartPointer>();
		Marker.gameObject.SetActive(value: false);
	}

	private void OnMarkerStart()
	{
		mIsDown = true;
		mStart = mPointer.ScreenPosition;
		Marker.gameObject.SetActive(value: true);
	}

	private void OnMarkerMove()
	{
		Vector3 vector = mPointer.ScreenPosition;
		Vector3 vector2 = new Vector3(Mathf.Min(mStart.x, vector.x), Mathf.Min(mStart.y, vector.y));
		Vector3 vector3 = new Vector3(Mathf.Max(mStart.x, vector.x), Mathf.Max(mStart.y, vector.y));
		Marker.position = vector2;
		Marker.sizeDelta = vector3 - vector2;
	}

	private void OnMarkerEnd()
	{
		mIsDown = false;
		Marker.gameObject.SetActive(value: false);
	}

	private bool CheckBounds()
	{
		Vector2 vector = mPointer.ScreenPosition - mStart;
		if (Math.Abs(vector.x) < (float)MinMarkerPixels || Math.Abs(vector.y) < (float)MinMarkerPixels)
		{
			return false;
		}
		return true;
	}

	private void SetGraphZoom()
	{
		if (CheckBounds())
		{
			GraphChart component = GetComponent<GraphChart>();
			if (component.PointToClient((Vector3)mStart, out double x, out double y) && component.PointToClient((Vector3)mPointer.ScreenPosition, out double x2, out double y2))
			{
				mIsZoomed = true;
				component.DataSource.AutomaticHorizontalView = false;
				component.DataSource.AutomaticVerticallView = false;
				DoubleVector2 doubleVector = new DoubleVector2(Math.Min(x, x2), Math.Min(y, y2));
				DoubleVector2 doubleVector2 = new DoubleVector2(Math.Max(x, x2), Math.Max(y, y2));
				component.HorizontalScrolling = doubleVector.x;
				component.VerticalScrolling = doubleVector.y;
				component.DataSource.HorizontalViewSize = doubleVector2.x - doubleVector.x;
				component.DataSource.VerticalViewSize = doubleVector2.y - doubleVector.y;
				component.DataSource.HorizontalViewOrigin = 0.0;
				component.DataSource.VerticalViewOrigin = 0.0;
			}
		}
	}

	private void Update()
	{
		if (mIsZoomed && mPointer.IsMouseDown)
		{
			mIsZoomed = false;
			GraphChart component = GetComponent<GraphChart>();
			component.DataSource.AutomaticHorizontalView = true;
			component.DataSource.AutomaticVerticallView = true;
			component.HorizontalScrolling = 0.0;
			component.VerticalScrolling = 0.0;
			mIsUp = false;
		}
		else
		{
			if (!mIsUp && mPointer.IsMouseDown)
			{
				return;
			}
			mIsUp = true;
			_ = mPointer.ScreenPosition;
			if (!mIsDown)
			{
				if (mPointer.IsMouseDown)
				{
					OnMarkerStart();
					OnMarkerMove();
				}
			}
			else if (!mPointer.IsMouseDown)
			{
				OnMarkerEnd();
				if (!mPointer.IsOut)
				{
					SetGraphZoom();
				}
			}
			else
			{
				OnMarkerMove();
			}
		}
	}
}

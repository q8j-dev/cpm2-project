using System;
using ChartAndGraph;
using UnityEngine;

[RequireComponent(typeof(GraphChart))]
public class GraphZoom : MonoBehaviour
{
	private GraphChart graph;

	private Vector3 mZoomBasePosition;

	private DoubleVector3 mZoomBaseChartSpace;

	private DoubleVector3 InitalScrolling;

	private DoubleVector3 InitalViewSize;

	private DoubleVector3 InitalViewDirection;

	private DoubleVector3 InitialOrigin;

	public float errorMargin = 5f;

	public float ZoomSpeed = 20f;

	public float MaxViewSize = 10f;

	public float MinViewSize = 0.1f;

	private float totalZoom;

	private void Start()
	{
	}

	private void OnEnable()
	{
		graph = GetComponent<GraphChart>();
	}

	private void ResetZoomAnchor()
	{
		totalZoom = 0f;
		InitalScrolling = new DoubleVector3(graph.HorizontalScrolling, graph.VerticalScrolling);
		InitalViewSize = new DoubleVector3(graph.DataSource.HorizontalViewSize, graph.DataSource.VerticalViewSize);
		InitalViewDirection = new DoubleVector3(Math.Sign(InitalViewSize.x), Math.Sign(InitalViewSize.y));
		InitialOrigin = new DoubleVector3(graph.DataSource.HorizontalViewOrigin, graph.DataSource.VerticalViewOrigin);
	}

	private bool CompareWithError(Vector3 a, Vector3 b)
	{
		if (Mathf.Abs(a.x - b.x) > errorMargin)
		{
			return false;
		}
		if (Mathf.Abs(a.y - b.y) > errorMargin)
		{
			return false;
		}
		return true;
	}

	private void Update()
	{
		if (graph == null)
		{
			return;
		}
		Vector2 vector = Input.mousePosition;
		graph.PointToClient((Vector3)vector, out double x, out double y);
		if (!CompareWithError(vector, mZoomBasePosition))
		{
			mZoomBasePosition = vector;
			graph.PointToClient((Vector3)vector, out x, out y);
			mZoomBaseChartSpace = new DoubleVector3(x, y);
			ResetZoomAnchor();
		}
		else
		{
			vector = mZoomBasePosition;
		}
		float y2 = Input.mouseScrollDelta.y;
		totalZoom += y2;
		if (y2 != 0f && graph.PointToClient((Vector3)vector, out x, out y))
		{
			DoubleVector3 doubleVector = InitialOrigin + InitalScrolling;
			DoubleVector3 doubleVector2 = new DoubleVector3(mZoomBaseChartSpace.x - doubleVector.x, mZoomBaseChartSpace.y - doubleVector.y);
			float num = Mathf.Pow(2f, totalZoom / ZoomSpeed);
			double num2 = InitalViewSize.x * (double)num;
			double num3 = InitalViewSize.y * (double)num;
			if (num2 * InitalViewDirection.x < (double)MaxViewSize && num2 * InitalViewDirection.x > (double)MinViewSize && num3 * InitalViewDirection.y < (double)MaxViewSize && num3 * InitalViewDirection.y > (double)MinViewSize)
			{
				graph.HorizontalScrolling = InitalScrolling.x + doubleVector2.x - doubleVector2.x * (double)num;
				graph.VerticalScrolling = InitalScrolling.y + doubleVector2.y - doubleVector2.y * (double)num;
				graph.DataSource.HorizontalViewSize = num2;
				graph.DataSource.VerticalViewSize = num3;
			}
		}
	}
}

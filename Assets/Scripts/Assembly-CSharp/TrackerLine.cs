using System;
using ChartAndGraph;
using UnityEngine;

public class TrackerLine : MonoBehaviour
{
	public GraphChartBase Chart;

	public RectTransform Area;

	public float lineThickness = 1f;

	public bool TrackLast = true;

	public float yPosition;

	private void Start()
	{
		if (Chart != null)
		{
			Chart.OnRedraw.AddListener(Redraw);
		}
	}

	private void Redraw()
	{
		if (Chart == null || !Chart.DataSource.GetLastPoint("Player 1", out var point))
		{
			return;
		}
		if (!TrackLast)
		{
			point.y = yPosition;
		}
		DoubleRect rect = new DoubleRect(point.x - Chart.DataSource.HorizontalViewSize, point.y - (double)(lineThickness * 0.5f), Chart.DataSource.HorizontalViewSize, lineThickness);
		if (!(Area != null))
		{
			return;
		}
		if (Chart.PointToWorldSpace(out var result, rect.min.x, rect.min.y))
		{
			result.y += lineThickness;
			if (Chart.PointToClient(result, out double _, out double y))
			{
				double num = Math.Abs(rect.min.y - y);
				rect = new DoubleRect(point.x - Chart.DataSource.HorizontalViewSize, point.y - num * 0.5, Chart.DataSource.HorizontalViewSize, num);
			}
		}
		if (Chart.TrimRect(rect, out var trimmed))
		{
			if (!Area.gameObject.activeSelf)
			{
				Area.gameObject.SetActive(value: true);
			}
			Chart.RectToCanvas(Area, trimmed);
		}
		else if (Area.gameObject.activeSelf)
		{
			Area.gameObject.SetActive(value: false);
		}
	}

	private void Update()
	{
	}
}

using ChartAndGraph;
using UnityEngine;
using UnityEngine.UI;

public class Marker : MonoBehaviour
{
	public GraphChartBase Chart;

	public RectTransform LastPoint;

	public RectTransform Area;

	public Text MouseText;

	private DoubleRect currentRect;

	private void Start()
	{
		if (Chart != null)
		{
			Chart.OnRedraw.AddListener(Redraw);
		}
	}

	private void Redraw()
	{
		if (!(Chart == null))
		{
			if (!Chart.IsRectVisible(currentRect))
			{
				double num = (float)(Chart.HorizontalScrolling + Chart.DataSource.HorizontalViewSize);
				double num2 = num - 1.0;
				double num3 = (float)Chart.VerticalScrolling;
				double num4 = (float)Chart.DataSource.GetMaxValue(1, dataValue: false);
				currentRect = new DoubleRect(num2, num3, num - num2, num4 - num3);
			}
			if (Chart.TrimRect(currentRect, out var trimmed))
			{
				Chart.RectToCanvas(Area, trimmed);
			}
			if (Chart.DataSource.GetLastPoint("Player 1", out var point) && Chart.PointToWorldSpace(out var result, point.x, point.y, "Player 1") && LastPoint != null)
			{
				LastPoint.transform.position = result;
			}
		}
	}

	private void Update()
	{
		if (MouseText != null)
		{
			if (Chart.MouseToClient(out var x, out var y))
			{
				MouseText.text = $"{x:0.00} , {y:0.00}";
			}
		}
		else
		{
			MouseText.text = "";
		}
	}
}

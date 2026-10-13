using ChartAndGraph;
using UnityEngine;

public class MarkerSampleDataInitializer : MonoBehaviour
{
	public GraphChartBase graph;

	private void Start()
	{
		graph.DataSource.ClearCategory("Player 1");
		MarkerText markerText = Object.FindObjectOfType<MarkerText>();
		MarkerTextLine markerTextLine = Object.FindObjectOfType<MarkerTextLine>();
		double y = 0.0;
		for (int i = 0; i < 10; i++)
		{
			double num = Random.value * 10f;
			graph.DataSource.AddPointToCategory("Player 1", i, num);
			if (i == 5)
			{
				markerText.point = new DoubleVector2(i, num);
			}
			if (i == 7)
			{
				markerTextLine.point1 = new DoubleVector2(i - 1, y);
				markerTextLine.point2 = new DoubleVector2(i, num);
			}
			y = num;
		}
	}

	private void Update()
	{
	}
}

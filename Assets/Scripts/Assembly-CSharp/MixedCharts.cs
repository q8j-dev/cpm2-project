using System.Collections;
using ChartAndGraph;
using UnityEngine;

public class MixedCharts : MonoBehaviour
{
	public BarChart Bar;

	public GraphChartBase Graph;

	private void Start()
	{
		StartCoroutine(FillGraphWait());
	}

	private IEnumerator FillGraphWait()
	{
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();
		FillGraph();
	}

	private void FillGraph()
	{
		Graph.DataSource.ClearCategory("Category1");
		for (int i = 0; i < Bar.DataSource.TotalCategories; i++)
		{
			string categoryName = Bar.DataSource.GetCategoryName(i);
			for (int j = 0; j < Bar.DataSource.TotalGroups; j++)
			{
				string groupName = Bar.DataSource.GetGroupName(j);
				Bar.GetBarTrackPosition(categoryName, groupName, out var trackPosition);
				Graph.PointToClient(trackPosition, out double x, out double _);
				Graph.DataSource.AddPointToCategory("Category1", x, Random.value * 10f);
			}
		}
	}

	private void Update()
	{
	}
}

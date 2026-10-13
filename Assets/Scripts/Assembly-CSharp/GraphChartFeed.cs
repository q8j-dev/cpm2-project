using System.Collections;
using ChartAndGraph;
using UnityEngine;

public class GraphChartFeed : MonoBehaviour
{
	private void Start()
	{
		GraphChartBase component = GetComponent<GraphChartBase>();
		if (!(component != null))
		{
			return;
		}
		component.Scrollable = false;
		component.HorizontalValueToStringMap[0.0] = "Zero";
		component.DataSource.StartBatch();
		component.DataSource.ClearCategory("Player 1");
		component.DataSource.ClearAndMakeBezierCurve("Player 2");
		for (int i = 0; i < 5; i++)
		{
			component.DataSource.AddPointToCategory("Player 1", i * 5, Random.value * 10f + 20f);
			if (i == 0)
			{
				component.DataSource.SetCurveInitialPoint("Player 2", i * 5, Random.value * 10f + 10f);
			}
			else
			{
				component.DataSource.AddLinearCurveToCategory("Player 2", new DoubleVector2(i * 5, Random.value * 10f + 10f));
			}
		}
		component.DataSource.MakeCurveCategorySmooth("Player 2");
		component.DataSource.EndBatch();
	}

	private IEnumerator ClearAll()
	{
		yield return new WaitForSeconds(5f);
		GetComponent<GraphChartBase>().DataSource.Clear();
	}
}

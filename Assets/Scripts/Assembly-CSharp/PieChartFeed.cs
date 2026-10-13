using ChartAndGraph;
using UnityEngine;

public class PieChartFeed : MonoBehaviour
{
	private void Start()
	{
		PieChart component = GetComponent<PieChart>();
		if (component != null)
		{
			component.DataSource.SlideValue("Player 1", 50.0, 10f);
			component.DataSource.SetValue("Player 2", Random.value * 10f);
		}
	}
}

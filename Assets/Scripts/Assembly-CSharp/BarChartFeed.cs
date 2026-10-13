using ChartAndGraph;
using UnityEngine;

public class BarChartFeed : MonoBehaviour
{
	private void Start()
	{
		BarChart component = GetComponent<BarChart>();
		if (component != null)
		{
			component.DataSource.SetValue("Player 1", "Value 1", Random.value * 20f);
			component.DataSource.SlideValue("Player 2", "Value 1", Random.value * 20f, 40f);
		}
	}

	private void Update()
	{
	}
}

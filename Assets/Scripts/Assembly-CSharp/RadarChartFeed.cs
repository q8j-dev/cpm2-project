using ChartAndGraph;
using UnityEngine;

public class RadarChartFeed : MonoBehaviour
{
	private void Start()
	{
		RadarChart component = GetComponent<RadarChart>();
		if (component != null)
		{
			component.DataSource.SetValue("Player 1", "A", 10.0);
		}
	}

	private void Update()
	{
	}
}

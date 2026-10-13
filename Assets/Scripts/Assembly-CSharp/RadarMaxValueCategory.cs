using ChartAndGraph;
using UnityEngine;

public class RadarMaxValueCategory : MonoBehaviour
{
	private void Start()
	{
		RadarChart component = GetComponent<RadarChart>();
		if (component != null)
		{
			component.DataSource.SetCategoryMaxValue("Player 1", 20.0);
			component.DataSource.SetValue("Player 1", "A", 10.0);
		}
	}

	private void Update()
	{
	}
}

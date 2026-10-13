using UnityEngine;

namespace ChartAndGraph
{
	internal class BubbleGraphFeed : MonoBehaviour
	{
		private string[] items = new string[11]
		{
			"A", "B", "C", "D", "E", "F", "G", "H", "I", "J",
			"K"
		};

		private void Start()
		{
			GraphChartBase component = GetComponent<GraphChartBase>();
			if (component != null)
			{
				component.DataSource.ClearCategory("Player 1");
				component.DataSource.ClearCategory("Player 2");
				for (int i = 0; i < 10; i++)
				{
					GraphData.AddPointToCategoryWithLabel(component, "Player 1", Random.value * 10f, Random.value * 10f, Random.value * 3f, items[i], i.ToString());
				}
			}
		}
	}
}

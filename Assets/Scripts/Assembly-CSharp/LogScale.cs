using ChartAndGraph;
using UnityEngine;

public class LogScale : MonoBehaviour
{
	public GraphChart Chart;

	private void Start()
	{
		Chart.GetComponent<VerticalAxis>().WithEdges = false;
		string[] array = new string[9] { "100", "1K", "10K", "100K", "1M", "10M", "100M", "1B", "10B" };
		for (int i = 0; i < array.Length; i++)
		{
			Chart.VerticalValueToStringMap[10 + i * 10] = array[i];
			Chart.VerticalValueToStringMap[-(10 + i * 10)] = "-" + array[i];
		}
	}

	private void Update()
	{
	}
}

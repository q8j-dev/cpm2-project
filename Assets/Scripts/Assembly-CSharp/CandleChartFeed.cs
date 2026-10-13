using ChartAndGraph;
using UnityEngine;

public class CandleChartFeed : MonoBehaviour
{
	private float time = 3f;

	private float x = 10f;

	private void Start()
	{
		CandleChart component = GetComponent<CandleChart>();
		if (component != null)
		{
			component.DataSource.StartBatch();
			component.DataSource.ClearCategory("Player 1");
			for (int i = 0; i < 30; i++)
			{
				component.DataSource.AddCandleToCategory("Player 1", new CandleChartData.CandleValue(Random.Range(5f, 10f), Random.Range(10f, 15f), Random.Range(0f, 5f), Random.Range(5f, 10f), (float)i * 10f / 30f, Random.value * 10f / 30f));
			}
			component.DataSource.EndBatch();
		}
	}

	private void Update()
	{
		time -= Time.deltaTime;
		if (time <= 0f)
		{
			time = 3f;
			x += 0.3f;
			GetComponent<CandleChart>().DataSource.AddCandleToCategory("Player 1", new CandleChartData.CandleValue(Random.Range(5f, 10f), Random.Range(10f, 15f), Random.Range(0f, 5f), Random.Range(5f, 10f), x, Random.value * 10f / 30f), 1f);
		}
	}
}

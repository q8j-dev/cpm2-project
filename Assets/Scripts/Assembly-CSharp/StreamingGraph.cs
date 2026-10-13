using System;
using ChartAndGraph;
using UnityEngine;

public class StreamingGraph : MonoBehaviour
{
	public GraphChart Graph;

	public int TotalPoints = 5;

	private float lastTime;

	private float lastX;

	private void Start()
	{
		if (!(Graph == null))
		{
			float num = 3f * (float)TotalPoints;
			Graph.DataSource.StartBatch();
			Graph.DataSource.ClearCategory("Player 1");
			Graph.DataSource.ClearCategory("Player 2");
			for (int i = 0; i < TotalPoints; i++)
			{
				Graph.DataSource.AddPointToCategory("Player 1", DateTime.Now - TimeSpan.FromSeconds(num), UnityEngine.Random.value * 20f + 10f);
				Graph.DataSource.AddPointToCategory("Player 2", DateTime.Now - TimeSpan.FromSeconds(num), UnityEngine.Random.value * 10f);
				num = (lastX = num - UnityEngine.Random.value * 3f);
			}
			Graph.DataSource.EndBatch();
		}
	}

	private void Update()
	{
		float time = Time.time;
		if (lastTime + 5f < time)
		{
			lastTime = time;
			lastX += UnityEngine.Random.value * 3f;
			Graph.DataSource.AddPointToCategoryRealtime("Player 1", DateTime.Now, UnityEngine.Random.value * 20f + 10f, 1.0);
			Graph.DataSource.AddPointToCategoryRealtime("Player 2", DateTime.Now, UnityEngine.Random.value * 10f, 1.0);
		}
	}
}

using System;
using ChartAndGraph;
using UnityEngine;

public class TestMonthWeek : MonoBehaviour
{
	public GraphChart chart;

	private void Start()
	{
		chart.DataSource.ClearCategory("Player 1");
		DateTime now = DateTime.Now;
		for (int i = 0; i < 60; i++)
		{
			chart.DataSource.AddPointToCategory("Player 1", now.AddMonths(i), UnityEngine.Random.Range(0f, 10f));
		}
	}

	private void Update()
	{
	}
}

using System;
using ChartAndGraph;
using UnityEngine;

public class customAxisDivisions : MonoBehaviour
{
	public GraphChart chart;

	private void Start()
	{
		chart.DataSource.ClearCategory("Player 1");
		DateTime now = DateTime.Now;
		for (int i = 0; i < 36; i++)
		{
			chart.DataSource.AddPointToCategory("Player 1", now + TimeSpan.FromDays(i * 10), UnityEngine.Random.Range(0f, 10f));
		}
		DateTime dateTime = now;
		chart.ClearHorizontalCustomDivisions();
		for (int j = 0; j < 12; j++)
		{
			chart.AddHorizontalAxisDivision(ChartDateUtility.DateToValue(dateTime));
			chart.AddHorizontalAxisDivision(ChartDateUtility.DateToValue(dateTime.AddDays(15.0)), subDivision: true);
			dateTime = dateTime.AddMonths(1);
		}
	}

	private void Update()
	{
	}
}

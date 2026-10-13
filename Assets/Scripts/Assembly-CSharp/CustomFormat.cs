using System;
using ChartAndGraph;
using UnityEngine;

public class CustomFormat : MonoBehaviour
{
	public GraphChart chart;

	private void Start()
	{
		chart.CustomNumberFormat = (double nubmer, int fractionDigits) => (int)(nubmer / 1000.0) + "K";
		chart.CustomDateTimeFormat = (DateTime date) => date.ToString("MMM");
	}

	private void Update()
	{
	}
}

using System;
using System.Collections.Generic;
using ChartAndGraph;
using UnityEngine;

public class BarRunChart : MonoBehaviour
{
	private class RunChartEntry
	{
		public string Name;

		public double Amount;

		public RunChartEntry(string name, double amount)
		{
			Name = name;
			Amount = amount;
		}
	}

	public float switchTime = 0.1f;

	private float switchTimeCounter;

	private List<RunChartEntry> mEntries = new List<RunChartEntry>();

	public Material SourceMaterial;

	private void Start()
	{
		switchTimeCounter = switchTime;
		BarChart component = GetComponent<BarChart>();
		component.TransitionTimeBetaFeature = switchTime;
		component.DataSource.ClearCategories();
		component.DataSource.ClearGroups();
		component.DataSource.AddGroup("Default");
		for (int i = 0; i < 10; i++)
		{
			string text = "Item " + i;
			mEntries.Add(new RunChartEntry(text, UnityEngine.Random.value * 10f));
			Material material = new Material(SourceMaterial);
			material.color = new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f));
			component.DataSource.AddCategory(text, material);
		}
	}

	private void AddValuesToCategories()
	{
		for (int i = 0; i < mEntries.Count; i++)
		{
			mEntries[i].Amount += UnityEngine.Random.Range(-0.3f, 0.3f);
		}
	}

	private void Update()
	{
		switchTimeCounter -= Time.deltaTime;
		if (switchTimeCounter < 0f)
		{
			switchTimeCounter = switchTime;
			BarChart component = GetComponent<BarChart>();
			for (int i = 0; i < mEntries.Count; i++)
			{
				component.DataSource.MoveCategory(mEntries[i].Name, i);
			}
			AddValuesToCategories();
			mEntries.Sort((RunChartEntry x, RunChartEntry y) => Math.Sign(x.Amount - y.Amount));
			for (int num = 0; num < mEntries.Count; num++)
			{
				component.DataSource.SlideValue(mEntries[num].Name, "Default", mEntries[num].Amount, switchTime);
			}
		}
	}
}

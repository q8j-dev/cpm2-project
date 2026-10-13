using System;
using System.Collections.Generic;
using System.Linq;
using ChartAndGraph;
using UnityEngine;

public class StackedGraphManager : MonoBehaviour
{
	private class CategoryEntry
	{
		public List<double> mYValues = new List<double>();

		public List<DoubleVector2> mVectors = new List<DoubleVector2>();

		public LargeDataFeed mFeed;

		public bool mEnabled = true;
	}

	public int RealtimeDownSampleCount = 10;

	public int DownSampleToPoints = 100;

	public GraphChart Chart;

	private Dictionary<string, CategoryEntry> mData = new Dictionary<string, CategoryEntry>();

	private List<double> mXValues = new List<double>();

	private List<double> mAccumilated = new List<double>();

	private void Start()
	{
	}

	public DoubleVector2 GetPointValue(string category, int inGraphIndex)
	{
		if (!mData.ContainsKey(category))
		{
			throw new ArgumentException("Category does not exist");
		}
		CategoryEntry categoryEntry = mData[category];
		int index = categoryEntry.mFeed.GetIndex(inGraphIndex);
		double y = categoryEntry.mYValues[index];
		return new DoubleVector2(mXValues[index], y);
	}

	public void ToggleCategoryEnabled(string category)
	{
		VerifyCategories();
		if (!mData.ContainsKey(category))
		{
			throw new ArgumentException("no such category");
		}
		CategoryEntry categoryEntry = mData[category];
		categoryEntry.mEnabled = !categoryEntry.mEnabled;
		Chart.DataSource.SetCategoryEnabled(category, categoryEntry.mEnabled);
		ApplyData();
	}

	public void SetCategoryEnabled(string category, bool isEnabled)
	{
		VerifyCategories();
		if (!mData.ContainsKey(category))
		{
			throw new ArgumentException("no such category");
		}
		mData[category].mEnabled = isEnabled;
		Chart.DataSource.SetCategoryEnabled(category, isEnabled);
		ApplyData();
	}

	private void VerifyNewCategory(string name)
	{
		CategoryEntry categoryEntry = null;
		if (mData.ContainsKey(name))
		{
			categoryEntry = mData[name];
		}
		if (categoryEntry == null)
		{
			categoryEntry = new CategoryEntry();
			categoryEntry.mYValues = new List<double>(mXValues.Count);
			categoryEntry.mVectors = new List<DoubleVector2>(mXValues.Count);
			categoryEntry.mFeed = base.gameObject.AddComponent<LargeDataFeed>();
			categoryEntry.mFeed.LoadExample = false;
			categoryEntry.mFeed.AlternativeGraph = Chart;
			categoryEntry.mFeed.Category = name;
			mData[name] = categoryEntry;
		}
		categoryEntry.mVectors.Clear();
		categoryEntry.mYValues.Clear();
		for (int i = 0; i < mXValues.Count; i++)
		{
			categoryEntry.mVectors.Add(new DoubleVector2(mXValues[i], 0.0));
			categoryEntry.mYValues.Add(0.0);
		}
		categoryEntry.mFeed.DownSampleToPoints = DownSampleToPoints;
		categoryEntry.mFeed.RealtimeDownSampleCount = RealtimeDownSampleCount;
	}

	private void OnValidate()
	{
		LargeDataFeed[] components = base.gameObject.GetComponents<LargeDataFeed>();
		foreach (LargeDataFeed obj in components)
		{
			obj.DownSampleToPoints = DownSampleToPoints;
			obj.RealtimeDownSampleCount = RealtimeDownSampleCount;
		}
	}

	private void VerifyRemoveCategory(string name)
	{
		if (mData.ContainsKey(name))
		{
			CategoryEntry categoryEntry = mData[name];
			if (categoryEntry != null)
			{
				categoryEntry.mVectors = null;
				UnityEngine.Object.Destroy(categoryEntry.mFeed);
			}
			mData.Remove(name);
		}
	}

	private void VerifyCategories()
	{
		IEnumerable<string> categoryNames = Chart.DataSource.CategoryNames;
		foreach (string item in categoryNames)
		{
			if (!mData.ContainsKey(item))
			{
				VerifyNewCategory(item);
			}
		}
		foreach (string key in mData.Keys)
		{
			if (!categoryNames.Contains(key))
			{
				VerifyRemoveCategory(key);
			}
		}
	}

	private void ClearEntries()
	{
		mXValues.Clear();
		foreach (string categoryName in Chart.DataSource.CategoryNames)
		{
			CategoryEntry categoryEntry = mData[categoryName];
			categoryEntry.mVectors.Clear();
			categoryEntry.mFeed.SetData(new List<DoubleVector2>());
		}
	}

	public void InitialData(double[] x, double[,] y)
	{
		VerifyCategories();
		ClearEntries();
		if (x.Length != y.GetLength(0))
		{
			throw new ArgumentException("x and y size should match");
		}
		mXValues.Clear();
		mXValues.AddRange(x);
		mAccumilated.Clear();
		mAccumilated.AddRange(Enumerable.Repeat(0.0, mXValues.Count));
		int num = Chart.DataSource.CategoryNames.Count() - 1;
		foreach (string item in Chart.DataSource.CategoryNames.Reverse())
		{
			CategoryEntry categoryEntry = mData[item];
			if (categoryEntry.mEnabled)
			{
				for (int i = 0; i < x.Length; i++)
				{
					mAccumilated[i] += y[i, num];
				}
			}
			categoryEntry.mYValues.Clear();
			categoryEntry.mVectors.Clear();
			for (int j = 0; j < mXValues.Count; j++)
			{
				categoryEntry.mYValues.Add(y[j, num]);
				categoryEntry.mVectors.Add(new DoubleVector2(mXValues[j], mAccumilated[j]));
			}
			categoryEntry.mFeed.SetData(categoryEntry.mVectors);
			num--;
		}
	}

	private void ApplyData()
	{
		mAccumilated.Clear();
		mAccumilated.AddRange(Enumerable.Repeat(0.0, mXValues.Count));
		int num = Chart.DataSource.CategoryNames.Count() - 1;
		foreach (string item in Chart.DataSource.CategoryNames.Reverse())
		{
			CategoryEntry categoryEntry = mData[item];
			categoryEntry.mVectors.Clear();
			if (categoryEntry.mEnabled)
			{
				for (int i = 0; i < mXValues.Count; i++)
				{
					mAccumilated[i] += categoryEntry.mYValues[i];
				}
			}
			categoryEntry.mVectors.Clear();
			for (int j = 0; j < mXValues.Count; j++)
			{
				categoryEntry.mVectors.Add(new DoubleVector2(mXValues[j], mAccumilated[j]));
			}
			categoryEntry.mFeed.SetData(categoryEntry.mVectors);
			num--;
		}
	}

	public void AddPointRealtime(double x, double[] y, double slideTime = 0.0)
	{
		VerifyCategories();
		mXValues.Add(x);
		int num = Chart.DataSource.CategoryNames.Count() - 1;
		double num2 = 0.0;
		foreach (string item in Chart.DataSource.CategoryNames.Reverse())
		{
			CategoryEntry categoryEntry = mData[item];
			double num3 = 0.0;
			if (num < y.Length)
			{
				num3 = y[num];
			}
			if (categoryEntry.mEnabled)
			{
				num2 += num3;
			}
			categoryEntry.mYValues.Add(num3);
			categoryEntry.mFeed.AppendPointRealtime(x, num2, slideTime);
			num--;
		}
	}

	private void Update()
	{
	}
}

using System;
using System.Collections.Generic;
using System.IO;
using ChartAndGraph;
using UnityEngine;

public class LargeDataFeed : MonoBehaviour, IComparer<DoubleVector2>
{
	public string Category = "Player 1";

	public int DownSampleToPoints = 100;

	private List<DoubleVector2> mData = new List<DoubleVector2>();

	private double pageSize = 2.0;

	private double currentPagePosition;

	private double currentZoom;

	private GraphChartBase graph;

	private double mCurrentPageSizeFactor = double.NegativeInfinity;

	public bool LoadExample = true;

	public bool ControlViewPortion;

	private double? mSlideEndThreshold;

	private int mStart;

	[HideInInspector]
	public GraphChartBase AlternativeGraph;

	public int RealtimeDownSampleCount = 10;

	private int currentDownSampleCount;

	private DoubleVector3 DownSampleSum;

	private double PageSizeFactor => pageSize * graph.DataSource.HorizontalViewSize;

	private void Start()
	{
		graph = GetComponent<GraphChartBase>();
		if (AlternativeGraph != null)
		{
			graph = AlternativeGraph;
		}
		if (LoadExample)
		{
			SetInitialData();
		}
		else
		{
			SetData(mData);
		}
	}

	public DoubleVector2 GetLastPoint()
	{
		if (mData.Count == 0)
		{
			return default;
		}
		return mData[mData.Count - 1];
	}

	private void SetInitialData()
	{
		List<DoubleVector2> list = new List<DoubleVector2>(250000);
		double num = 0.0;
		double num2 = 200.0;
		for (int i = 0; i < 25000; i++)
		{
			list.Add(new DoubleVector2(num, num2));
			num2 += (double)(UnityEngine.Random.value * 10f - 5f);
			num += (double)UnityEngine.Random.value;
		}
		SetData(list);
	}

	public void SaveToFile(string path)
	{
		using StreamWriter streamWriter = new StreamWriter(path);
		streamWriter.WriteLine(mData.Count);
		for (int i = 0; i < mData.Count; i++)
		{
			DoubleVector2 doubleVector = mData[i];
			streamWriter.WriteLine(doubleVector.x);
			streamWriter.WriteLine(doubleVector.y);
		}
	}

	public void LoadFromFile(string path)
	{
		try
		{
			List<DoubleVector2> list = new List<DoubleVector2>();
			using (StreamReader streamReader = new StreamReader(path))
			{
				int num = int.Parse(streamReader.ReadLine());
				for (int i = 0; i < num; i++)
				{
					double x = double.Parse(streamReader.ReadLine());
					double y = double.Parse(streamReader.ReadLine());
					list.Add(new DoubleVector2(x, y));
				}
			}
			SetData(list);
		}
		catch (Exception)
		{
			throw new Exception("Invalid file format");
		}
	}

	private bool VerifySorted(List<DoubleVector2> data)
	{
		if (data == null)
		{
			return true;
		}
		for (int i = 1; i < data.Count; i++)
		{
			if (data[i].x < data[i - 1].x)
			{
				return false;
			}
		}
		return true;
	}

	private void OnDataLoaded()
	{
		RestartDownSampleCount();
	}

	public void SetData(List<DoubleVector2> data)
	{
		if (data == null)
		{
			data = new List<DoubleVector2>();
		}
		if (!VerifySorted(data))
		{
			Debug.LogWarning("The data used with large data feed must be sorted acoording to the x value, aborting operation");
			return;
		}
		mData = data;
		OnDataLoaded();
		LoadPage(currentPagePosition);
	}

	private int FindClosestIndex(double position)
	{
		int num = mData.BinarySearch(new DoubleVector2(position, 0.0), this);
		if (num >= 0)
		{
			return num;
		}
		return ~num;
	}

	private void AdjustVerticalView()
	{
		findAdjustPosition(graph.HorizontalScrolling, graph.DataSource.HorizontalViewSize, out var start, out var end);
		double num = double.MaxValue;
		double num2 = double.MinValue;
		bool flag = graph.AutoScrollHorizontally;
		if (mData.Count == 0)
		{
			flag = true;
		}
		else
		{
			double x = mData[mData.Count - 1].x;
			double num3 = currentPagePosition - mCurrentPageSizeFactor;
			double num4 = currentPagePosition + mCurrentPageSizeFactor - graph.DataSource.HorizontalViewSize;
			if (x >= num3 && x <= num4)
			{
				flag = true;
			}
		}
		if (flag)
		{
			end--;
		}
		for (int i = start; i <= end; i++)
		{
			double y = mData[i].y;
			num = Math.Min(y, num);
			num2 = Math.Max(y, num2);
		}
		if (flag && graph.DataSource.GetLastPoint(Category, out var point))
		{
			num = Math.Min(point.y, num);
			num2 = Math.Max(point.y, num2);
		}
		graph.VerticalScrolling = num;
		graph.DataSource.VerticalViewSize = num2 - num;
	}

	private void findAdjustPosition(double position, double size, out int start, out int end)
	{
		int num = FindClosestIndex(position);
		double num2 = position + size;
		start = num;
		end = num;
		while (end < mData.Count && !(mData[end].x > num2))
		{
			end++;
		}
	}

	private void findPointsForPage(double position, out int start, out int end)
	{
		int num = FindClosestIndex(position);
		double num2 = position + PageSizeFactor;
		double num3 = position - PageSizeFactor;
		start = num;
		while (start > 0 && !(mData[start].x < num3))
		{
			start--;
		}
		end = num;
		while (end < mData.Count && !(mData[end].x > num2))
		{
			end++;
		}
	}

	public void Update()
	{
		if (graph != null)
		{
			double num = currentPagePosition - mCurrentPageSizeFactor;
			double num2 = currentPagePosition + mCurrentPageSizeFactor;
			if (mSlideEndThreshold.HasValue)
			{
				num2 = Math.Max(mSlideEndThreshold.Value, num2);
			}
			double num3 = num2 - graph.DataSource.HorizontalViewSize * 1.0001;
			if (graph.HorizontalScrolling < num || graph.HorizontalScrolling > num3 || currentZoom >= graph.DataSource.HorizontalViewSize * 2.0)
			{
				currentZoom = graph.DataSource.HorizontalViewSize;
				mCurrentPageSizeFactor = PageSizeFactor * 0.8999999761581421;
				LoadPage(graph.HorizontalScrolling);
			}
			if (ControlViewPortion)
			{
				AdjustVerticalView();
			}
		}
	}

	private void LoadWithoutDownSampling(int start, int end)
	{
		for (int i = start; i < end; i++)
		{
			graph.DataSource.AddPointToCategory(Category, mData[i].x, mData[i].y);
		}
	}

	private void LoadWithDownSampling(int start, int end)
	{
		int num = end - start;
		if (DownSampleToPoints >= num)
		{
			LoadWithoutDownSampling(start, end);
			return;
		}
		double num2 = (double)num / (double)DownSampleToPoints;
		for (int i = 0; i < DownSampleToPoints; i++)
		{
			int num3 = start + (int)((double)i * num2);
			int val = start + (int)((double)(i + 1) * num2);
			val = Math.Min(val, mData.Count - 1);
			double num4 = 0.0;
			double num5 = 0.0;
			double num6 = 0.0;
			for (int j = num3; j < val; j++)
			{
				num4 += mData[j].x;
				num5 += mData[j].y;
				num6++;
			}
			if (num6 > 0.0)
			{
				num4 /= num6;
				num5 /= num6;
				graph.DataSource.AddPointToCategory(Category, num4, num5);
			}
			else
			{
				Debug.Log("error");
			}
		}
	}

	public int GetIndex(int inGraphIndex)
	{
		return mStart + inGraphIndex;
	}

	private void LoadPage(double pagePosition)
	{
		mSlideEndThreshold = null;
		if (graph != null)
		{
			Debug.Log("Loading page :" + pagePosition);
			graph.DataSource.StartBatch();
			graph.DataSource.HorizontalViewOrigin = 0.0;
			findPointsForPage(pagePosition, out var start, out var end);
			graph.DataSource.ClearCategory(Category);
			mStart = start;
			if (DownSampleToPoints <= 0)
			{
				LoadWithoutDownSampling(start, end);
			}
			else
			{
				LoadWithDownSampling(start, end);
			}
			graph.DataSource.EndBatch();
			graph.HorizontalScrolling = pagePosition;
		}
		currentPagePosition = pagePosition;
	}

	public int Compare(DoubleVector2 x, DoubleVector2 y)
	{
		if (x.x < y.x)
		{
			return -1;
		}
		if (x.x > y.x)
		{
			return 1;
		}
		return 0;
	}

	public void AppendRealtimeWithDownSampling(double x, double y, double slideTime = 0.0)
	{
		if (graph == null)
		{
			return;
		}
		bool flag = graph.AutoScrollHorizontally;
		if (mData.Count == 0)
		{
			flag = true;
		}
		else
		{
			double x2 = mData[mData.Count - 1].x;
			double num = currentPagePosition - mCurrentPageSizeFactor;
			double num2 = currentPagePosition + mCurrentPageSizeFactor - graph.DataSource.HorizontalViewSize;
			if (x2 >= num && x2 <= num2)
			{
				flag = true;
			}
		}
		DoubleVector2 item = new DoubleVector2(x, y);
		mData.Add(item);
		if (flag)
		{
			if (currentDownSampleCount >= RealtimeDownSampleCount)
			{
				RestartDownSampleCount();
				graph.DataSource.AddPointToCategoryRealtime(Category, x, y, slideTime);
			}
			DownSampleSum += item.ToDoubleVector3();
			currentDownSampleCount++;
			DoubleVector3 doubleVector = DownSampleSum * 1.0 / currentDownSampleCount;
			if (currentDownSampleCount != 1)
			{
				graph.DataSource.UpdateLastPointInCategoryRealtime(Category, doubleVector.x, doubleVector.y, slideTime);
			}
		}
		else
		{
			RestartDownSampleCount();
		}
	}

	private void RestartDownSampleCount()
	{
		currentDownSampleCount = 0;
		DownSampleSum = default;
	}

	public void AppendPointRealtime(double x, double y, double slideTime = 0.0)
	{
		if (graph == null)
		{
			return;
		}
		bool flag = graph.AutoScrollHorizontally;
		if (mData.Count == 0)
		{
			flag = true;
		}
		else
		{
			double x2 = mData[mData.Count - 1].x;
			double num = currentPagePosition - mCurrentPageSizeFactor;
			double num2 = currentPagePosition + mCurrentPageSizeFactor - graph.DataSource.HorizontalViewSize;
			if (x2 >= num && x2 <= num2)
			{
				flag = true;
			}
		}
		mData.Add(new DoubleVector2(x, y));
		if (flag)
		{
			mSlideEndThreshold = x;
			graph.DataSource.AddPointToCategoryRealtime(Category, x, y, slideTime);
		}
	}
}

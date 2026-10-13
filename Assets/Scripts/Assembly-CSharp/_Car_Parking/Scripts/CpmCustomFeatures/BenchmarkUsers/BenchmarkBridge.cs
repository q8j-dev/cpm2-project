using ChartAndGraph;
using UnityEngine;
using UnityEngine.Scripting;
using _Car_Parking.Scripts.CpmCustomFeatures.Benchmarking;

namespace _Car_Parking.Scripts.CpmCustomFeatures.BenchmarkUsers
{
	[Preserve]
	public sealed class BenchmarkBridge
	{
		[RuntimeInitializeOnLoadMethod]
		private static void InitializeBenchmarkBridge()
		{
			BenchmarkRunner.OnReportReady += HandleReport;
		}

		private static void HandleReport(BenchmarkReport benchmarkReport)
		{
			BenchmarkUi benchmarkUi = Object.FindAnyObjectByType<BenchmarkUi>();
			GraphChart graphChart = benchmarkUi.GraphChart;
			graphChart.DataSource.ClearCategory("Fps");
			graphChart.DataSource.StartBatch();
			for (int i = 0; i < benchmarkReport.FpsData.AllFps.Count; i++)
			{
				graphChart.DataSource.AddPointToCategory("Fps", i, benchmarkReport.FpsData.AllFps[i]);
			}
			graphChart.DataSource.EndBatch();
			Vector2 sizeDelta = new Vector2(benchmarkReport.FpsData.AllFps.Count + 60, 120f);
			graphChart.GetComponent<RectTransform>().sizeDelta = sizeDelta;
			graphChart.WidthRatio = benchmarkReport.FpsData.AllFps.Count;
			graphChart.HeightRatio = 120f;
			benchmarkUi.Text.text = benchmarkReport.ToString();
			benchmarkUi.LastReport = benchmarkReport;
			Debug.Log(benchmarkReport.ToString());
		}
	}
}

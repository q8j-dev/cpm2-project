using System;
using System.Linq;
using System.Threading;
using ChartAndGraph;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class StackedGraphExample : MonoBehaviour
{
	public int NumCategories = 3;

	public Text infoText;

	private CancellationTokenSource _cts;

	private void Start()
	{
		_cts = new CancellationTokenSource();
		AddPoints(_cts.Token).Forget();
	}

	private void OnDestroy()
	{
		_cts?.Cancel();
		_cts?.Dispose();
	}

	private async UniTaskVoid AddPoints(CancellationToken cancellationToken)
	{
		int x = 100;
		StackedGraphManager manager = GetComponent<StackedGraphManager>();
		manager.Chart.PointHovered.AddListener(GraphHoverd);
		manager.Chart.NonHovered.AddListener(NonHovered);
		double[] array = Enumerable.Range(0, x).Select((Func<int, double>)((int num4) => num4)).ToArray();
		double[,] array2 = new double[array.Length, NumCategories];
		for (int num = 0; num < array2.GetLength(0); num++)
		{
			for (int num2 = 0; num2 < array2.GetLength(1); num2++)
			{
				array2[num, num2] = UnityEngine.Random.value * 3f;
			}
		}
		manager.InitialData(array, array2);
		double[] newPointYArr = new double[NumCategories];
		while (!cancellationToken.IsCancellationRequested)
		{
			await UniTask.Delay(5000, ignoreTimeScale: false, PlayerLoopTiming.Update, cancellationToken);
			x++;
			for (int num3 = 0; num3 < newPointYArr.Length; num3++)
			{
				newPointYArr[num3] = UnityEngine.Random.value * 3f;
			}
			manager.AddPointRealtime(x, newPointYArr, 1.0);
		}
	}

	public void Toogle(string name)
	{
		StackedGraphManager component = GetComponent<StackedGraphManager>();
		if (component != null)
		{
			component.ToggleCategoryEnabled(name);
		}
	}

	public void GraphHoverd(GraphChartBase.GraphEventArgs args)
	{
		if (!(infoText == null))
		{
			DoubleVector2 pointValue = GetComponent<StackedGraphManager>().GetPointValue(args.Category, args.Index);
			infoText.text = $"{args.Category} : {pointValue.x},{pointValue.y:0.##}";
		}
	}

	public void NonHovered()
	{
		if (!(infoText == null))
		{
			infoText.text = "";
		}
	}
}

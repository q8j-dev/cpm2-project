using System.Threading;
using ChartAndGraph;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class colorRandomizer : MonoBehaviour
{
	public string category;

	public GraphChart chart;

	public Material baseMaterial;

	public GameObject textPrefab;

	private CancellationTokenSource _cts;

	private void Start()
	{
		_cts = new CancellationTokenSource();
		SwitchColor(_cts.Token).Forget();
	}

	private void OnDestroy()
	{
		_cts?.Cancel();
		_cts?.Dispose();
	}

	private async UniTaskVoid SwitchColor(CancellationToken cancellationToken)
	{
		while (!cancellationToken.IsCancellationRequested)
		{
			baseMaterial.color = Random.ColorHSV();
			chart.DataSource.SetCategoryPoint(category, baseMaterial, 5.0);
			Text component = textPrefab.GetComponent<Text>();
			if (component != null)
			{
				component.color = Random.ColorHSV();
				VerticalAxis component2 = chart.GetComponent<VerticalAxis>();
				if (component2 != null)
				{
					component2.MainDivisions.TextPrefab = component;
					component2.SubDivisions.TextPrefab = component;
				}
			}
			await UniTask.Delay(1000, ignoreTimeScale: false, PlayerLoopTiming.Update, cancellationToken);
		}
	}
}

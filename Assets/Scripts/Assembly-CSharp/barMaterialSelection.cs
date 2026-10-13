using ChartAndGraph;
using UnityEngine;

public class barMaterialSelection : MonoBehaviour
{
	public BarChart bar;

	private void Start()
	{
		bar.BarClicked.AddListener(BarClicked);
	}

	private void BarClicked(BarChart.BarEventArgs args)
	{
		masterBarSelection component = bar.GetComponent<masterBarSelection>();
		if (component != null)
		{
			component.ToogleBar(args.Category, args.Group);
		}
	}

	private void Update()
	{
	}
}

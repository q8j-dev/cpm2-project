using ChartAndGraph;
using UnityEngine;

public class WaveGraph : MonoBehaviour
{
	public GraphChart Graph;

	public int PointCount = 100;

	private DoubleVector3[] array;

	private void Start()
	{
		array = new DoubleVector3[PointCount];
		if (!(Graph == null))
		{
			Graph.DataSource.StartBatch();
			Graph.DataSource.ClearCategory("Player 1");
			for (int i = 0; i < array.Length; i++)
			{
				Graph.DataSource.AddPointToCategory("Player 1", i, Random.value * 20f + 10f);
			}
			Graph.DataSource.EndBatch();
		}
	}

	private void Update()
	{
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = new DoubleVector3(i, Random.value * 20f, -1.0);
		}
		Graph.DataSource.SetCategoryArray("Player 1", array, 0, array.Length);
	}
}

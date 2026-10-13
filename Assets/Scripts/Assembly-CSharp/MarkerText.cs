using ChartAndGraph;
using UnityEngine;
using UnityEngine.UI;

public class MarkerText : MonoBehaviour
{
	public GraphChartBase Chart;

	public Text TextObject;

	public DoubleVector2 point;

	public float SeperationOffset;

	public float ElevationOffset;

	private void Start()
	{
	}

	private void Update()
	{
		if (!(Chart == null) && Chart.PointToWorldSpace(out var result, point.x, point.y))
		{
			TextObject.transform.position = result + new Vector3(SeperationOffset, ElevationOffset, 0f);
		}
	}
}

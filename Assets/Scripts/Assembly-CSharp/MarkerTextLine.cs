using ChartAndGraph;
using UnityEngine;
using UnityEngine.UI;

public class MarkerTextLine : MonoBehaviour
{
	public GraphChartBase Chart;

	public Text TextObject;

	public DoubleVector2 point1;

	public DoubleVector2 point2;

	public float SeperationOffset;

	public float ElevationOffset;

	public bool Rotate = true;

	private void Start()
	{
	}

	private void Update()
	{
		if (!(Chart == null) && Chart.PointToWorldSpace(out var result, point1.x, point1.y) && Chart.PointToWorldSpace(out var result2, point2.x, point2.y))
		{
			Vector2 vector = (result2 + result) * 0.5f;
			Vector2 vector2 = (result2 - result).normalized;
			Vector2 vector3 = ChartCommon.Perpendicular(vector2);
			TextObject.transform.position = vector + SeperationOffset * vector2 + ElevationOffset * vector3;
			if (Rotate)
			{
				TextObject.transform.localRotation = Quaternion.Euler(0f, 0f, 57.29578f * Mathf.Atan2(vector2.y, vector2.x));
			}
		}
	}
}

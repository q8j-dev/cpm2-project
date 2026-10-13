using ChartAndGraph;
using UnityEngine;

public class RealtimeAppend : MonoBehaviour
{
	private float time = 1f;

	private double x = -1.0;

	private double y = -1.0;

	private void Start()
	{
	}

	private void Update()
	{
		time -= Time.deltaTime;
		if (!(time <= 0f))
		{
			return;
		}
		time = 1f;
		LargeDataFeed component = GetComponent<LargeDataFeed>();
		if (component != null)
		{
			if (x == -1.0)
			{
				DoubleVector2 lastPoint = component.GetLastPoint();
				x = lastPoint.x;
				y = lastPoint.y;
			}
			y += Random.value * 10f - 5f;
			x += Random.value;
			component.AppendRealtimeWithDownSampling(x, y, 1.0);
		}
	}
}

using ChartAndGraph;
using UnityEngine;

public class ChangeOrder : MonoBehaviour
{
	public float slideTime = 5f;

	public float switchTime = 7f;

	private float timeCounter = 1f;

	private float switchTimeCounter = 0.3f;

	private void Start()
	{
		timeCounter = 0f;
		switchTimeCounter = switchTime;
	}

	private void Update()
	{
		timeCounter -= Time.deltaTime;
		switchTimeCounter -= Time.deltaTime;
		if (switchTimeCounter < 0f)
		{
			switchTimeCounter = switchTime;
			GetComponent<BarChart>().DataSource.SwitchCategoryPositions("Category 1", "Category 2");
		}
		if (!(timeCounter < 0f))
		{
			return;
		}
		BarChart component = GetComponent<BarChart>();
		timeCounter = slideTime;
		for (int i = 1; i <= 3; i++)
		{
			string category = "Category " + i;
			for (int j = 1; j <= 3; j++)
			{
				string text = "Group " + j;
				component.DataSource.SlideValue(category, text, Random.value * 10f, slideTime);
			}
		}
	}
}

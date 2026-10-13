using ChartAndGraph;
using UnityEngine;

public class PyramidFeed : MonoBehaviour
{
	public Material newCategory;

	private void Start()
	{
		PyramidChart component = GetComponent<PyramidChart>();
		component.DataSource.AddCategory("New category", new ChartDynamicMaterial(newCategory, Color.blue, Color.white), "New", "and Fresh", null);
		component.DataSource.SetCategoryHeightRatio("New category", 3f);
	}

	private void Update()
	{
	}
}

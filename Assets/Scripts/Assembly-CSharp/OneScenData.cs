using UnityEngine;
using UnityEngine.Rendering;

public class OneScenData : MonoBehaviour
{
	public GameObject[] treeToCopy;

	public GameObject cityMainMesh;

	private void Start()
	{
		if (CPMGraphicsSettingsManager.Instance.Quality > 1)
		{
			CreateShowTrees();
		}
	}

	private void CreateShowTrees()
	{
		GameObject[] array = treeToCopy;
		foreach (GameObject gameObject in array)
		{
			if (!gameObject)
			{
				break;
			}
			GameObject obj = Object.Instantiate(gameObject);
			Renderer component = obj.GetComponent<MeshRenderer>();
			component.shadowCastingMode = ShadowCastingMode.ShadowsOnly;
			obj.transform.parent = gameObject.transform.parent;
			obj.transform.position = gameObject.transform.position;
			obj.transform.eulerAngles = gameObject.transform.eulerAngles;
			Material material = new Material(gameObject.GetComponent<MeshRenderer>().material);
			material.shader = Shader.Find("Legacy Shaders/Transparent/Cutout/Diffuse");
			component.material = material;
		}
	}
}

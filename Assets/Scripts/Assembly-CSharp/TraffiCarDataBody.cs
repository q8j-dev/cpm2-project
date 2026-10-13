using UnityEngine;

public class TraffiCarDataBody : MonoBehaviour
{
	public Color[] Colors;

	public Material bodyMat;

	public GameObject body;

	private void Start()
	{
		if ((bool)body)
		{
			Material material = new Material(bodyMat.shader);
			material.CopyPropertiesFromMaterial(bodyMat);
			bodyMat = material;
			int num = Colors.Length;
			if (num > 0)
			{
				bodyMat.color = Colors[Random.Range(0, num)];
			}
			body.GetComponent<Renderer>().material = bodyMat;
		}
	}

	public void ChangeColor()
	{
		int num = Colors.Length;
		if (num > 0)
		{
			bodyMat.color = Colors[Random.Range(0, num)];
		}
	}
}

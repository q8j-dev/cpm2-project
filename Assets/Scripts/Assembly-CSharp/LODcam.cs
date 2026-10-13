using UnityEngine;

public class LODcam : MonoBehaviour
{
	public float[] lodDistances = new float[32];

	private void Start()
	{
	}

	private void Update()
	{
		Camera.main.layerCullDistances = lodDistances;
	}
}

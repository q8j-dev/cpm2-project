using UnityEngine;

public class TempLogger : MonoBehaviour
{
	private Material mat;

	private void Start()
	{
		mat = GetComponent<MeshRenderer>().material;
		MonoBehaviour.print(mat.GetTexture("_T2M_SplatMap_1"));
	}
}

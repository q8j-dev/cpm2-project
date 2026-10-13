using UnityEngine;

public class DisabeMeshRenderer : MonoBehaviour
{
	private void Awake()
	{
		if ((bool)GetComponent<MeshRenderer>())
		{
			GetComponent<MeshRenderer>().enabled = false;
		}
	}
}

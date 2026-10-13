using UnityEngine;

public class DeactivatePhysics : MonoBehaviour
{
	public Mesh Mesh;

	private void OnBecameInvisible()
	{
		Object.Destroy(base.gameObject);
	}

	private void OnDestroy()
	{
		Object.Destroy(Mesh);
		Mesh = null;
	}
}

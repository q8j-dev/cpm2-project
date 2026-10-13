using UnityEngine;

public class FogController : MonoBehaviour
{
	public bool fogOn;

	public int colorVal = -1;

	private void OnTriggerEnter(Collider other)
	{
		_ = other.attachedRigidbody.gameObject;
	}
}

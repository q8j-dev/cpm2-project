using UnityEngine;

public class LoadObject : MonoBehaviour
{
	public int partNumber;

	public bool loadingNow;

	public new Collider collider;

	public float LoadDistance = 10f;

	public float disableDistance = 30f;

	private void Start()
	{
		collider = GetComponent<Collider>();
	}
}

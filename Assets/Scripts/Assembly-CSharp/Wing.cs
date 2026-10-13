using UnityEngine;

public class Wing : MonoBehaviour
{
	public float dragCoefficient = 0.05f;

	public int angleOfAttack = 1;

	public float area = 1f;

	public float downForce;

	public float dragForce;

	private Rigidbody body;

	private Transform myTransform;

	private void Start()
	{
		Transform parent = base.transform;
		myTransform = base.transform;
		while (parent != null && parent.GetComponent<Rigidbody>() == null)
		{
			parent = parent.parent;
		}
		if (parent != null)
		{
			body = parent.GetComponent<Rigidbody>();
		}
		parent = base.transform;
	}
}

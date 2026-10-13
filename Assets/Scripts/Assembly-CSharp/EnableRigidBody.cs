using CPMEngine;
using UnityEngine;

public class EnableRigidBody : MonoBehaviour
{
	private Rigidbody rb;

	private MeshCollider mechCollider;

	private void Start()
	{
		rb = GetComponent<Rigidbody>();
		mechCollider = GetComponent<MeshCollider>();
	}

	private void OnCollisionEnter(Collision other)
	{
		if (other.gameObject == V.Car && V.Car.GetComponent<Powertrain>().SpeedMps > 10f && (bool)mechCollider && (bool)rb)
		{
			mechCollider.convex = true;
			rb.isKinematic = false;
			Object.Destroy(base.gameObject, 5f);
		}
	}
}

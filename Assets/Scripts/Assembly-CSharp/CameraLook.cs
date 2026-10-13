using UnityEngine;

public class CameraLook : MonoBehaviour
{
	public Transform target;

	private void Start()
	{
		target = GameObject.FindWithTag("Player").transform;
	}

	private void Update()
	{
		Vector3 vector = target.transform.position - base.transform.position;
		vector.x = (vector.z = 0f);
		base.transform.LookAt(target.transform.position - vector);
	}
}

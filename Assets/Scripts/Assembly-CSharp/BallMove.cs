using TouchControlsKit;
using UnityEngine;

public class BallMove : MonoBehaviour
{
	private Rigidbody myRigidbody;

	private Vector3 tilt = Vector3.zero;

	private Transform myTransform;

	private Transform cameraTransform;

	private void Awake()
	{
		myRigidbody = GetComponent<Rigidbody>();
		myRigidbody.maxAngularVelocity = 25f;
		myTransform = GetComponent<Transform>();
		cameraTransform = Camera.main.GetComponent<Transform>();
	}

	private void Update()
	{
		tilt.x = Tilt.forwardAxis + TCKInput.GetAxis("dPad", "Horizontal");
		tilt.z = 0f - Tilt.sidewaysAxis + TCKInput.GetAxis("dPad", "Vertical");
		cameraTransform.position = new Vector3(myTransform.position.x, cameraTransform.position.y, myTransform.position.z - 5f);
	}

	private void FixedUpdate()
	{
		myRigidbody.AddForce(tilt * 5f * Time.fixedDeltaTime, ForceMode.Impulse);
	}
}

using TouchControlsKit;
using UnityEngine;

public class CarMove : MonoBehaviour
{
	private CharacterController controller;

	private void Awake()
	{
		controller = GetComponent<CharacterController>();
	}

	private void Update()
	{
		float yAngle = TCKInput.GetAxis("steeringWheel", "Horizontal") + Input.GetAxis("Horizontal");
		float num = TCKInput.GetAxis("moveJoystick", "Vertical") + Input.GetAxis("Vertical");
		if (num != 0f)
		{
			base.transform.Rotate(0f, yAngle, 0f);
		}
		Vector3 motion = base.transform.forward * num * 5f;
		motion *= Time.deltaTime;
		controller.Move(motion);
	}
}

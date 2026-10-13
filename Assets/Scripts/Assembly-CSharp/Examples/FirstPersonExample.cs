using TouchControlsKit;
using UnityEngine;

namespace Examples
{
	public class FirstPersonExample : MonoBehaviour
	{
		private Transform myTransform;

		private Transform cameraTransform;

		private CharacterController controller;

		private float rotation;

		private Vector3 moveDirection = Vector3.zero;

		private bool jump;

		private bool grounded;

		private bool prevGrounded;

		private float weapReadyTime;

		private bool weapReady = true;

		private void Awake()
		{
			myTransform = base.transform;
			cameraTransform = Camera.main.transform;
			controller = GetComponent<CharacterController>();
		}

		private void Update()
		{
			if (!weapReady)
			{
				weapReadyTime += Time.deltaTime;
				if (weapReadyTime > 0.15f)
				{
					weapReady = true;
					weapReadyTime = 0f;
				}
			}
			if (grounded && TCKInput.GetButtonDown("jumpBtn"))
			{
				jump = true;
			}
		}

		private void FixedUpdate()
		{
			grounded = controller.isGrounded;
			float axis = TCKInput.GetAxis("Joystick", "Horizontal");
			float axis2 = TCKInput.GetAxis("Joystick", "Vertical");
			PlayerMovement(axis, axis2);
			if (!prevGrounded && grounded)
			{
				moveDirection.y = 0f;
			}
			prevGrounded = grounded;
		}

		private void PlayerMovement(float horizontal, float vertical)
		{
			moveDirection = myTransform.forward * vertical;
			moveDirection += myTransform.right * horizontal;
			if (grounded)
			{
				moveDirection *= 7f;
				moveDirection.y = -10f;
				if (jump)
				{
					jump = false;
					moveDirection.y = 5f;
				}
			}
			else
			{
				moveDirection += Physics.gravity * 2f * Time.fixedDeltaTime;
			}
			moveDirection.y *= 20f;
			controller.Move(moveDirection * Time.fixedDeltaTime);
		}

		public void PlayerRotation(float horizontal, float vertical)
		{
			myTransform.Rotate(0f, horizontal * 12f, 0f);
			rotation += vertical * 12f;
			rotation = Mathf.Clamp(rotation, -60f, 60f);
			cameraTransform.localEulerAngles = new Vector3(0f - rotation, cameraTransform.localEulerAngles.y, 0f);
		}

		public void PlayerFiring()
		{
			if (weapReady)
			{
				weapReady = false;
				GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
				obj.transform.position = myTransform.position + myTransform.right;
				obj.transform.localScale = Vector3.one * 0.15f;
				Rigidbody rigidbody = obj.AddComponent<Rigidbody>();
				Transform transform = Camera.main.transform;
				rigidbody.AddForce(transform.forward * Random.Range(25f, 35f) + transform.right * Random.Range(-2f, 2f) + transform.up * Random.Range(-2f, 2f), ForceMode.Impulse);
				Object.Destroy(obj, 3.5f);
			}
		}

		public void PlayerClicked()
		{
		}
	}
}

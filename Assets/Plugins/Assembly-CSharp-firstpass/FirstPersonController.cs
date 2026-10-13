using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class FirstPersonController : MonoBehaviour
{
	[SerializeField]
	private float m_MouseSensitivity = 100f;

	[SerializeField]
	private float m_MovementSpeed = 5f;

	[SerializeField]
	private Transform m_PlayerCamera = default;

	[SerializeField]
	private bool m_MoveWithMouse = true;

	private CharacterController m_CharacterController;

	private float m_XRotation;

	private void Start()
	{
		Debug.Log("The FirstPersonController uses the legacy input system. Please set it in Project Settings");
		m_MoveWithMouse = false;
		if (m_MoveWithMouse)
		{
			Cursor.lockState = CursorLockMode.Locked;
		}
		m_CharacterController = GetComponent<CharacterController>();
	}

	private void Update()
	{
		Look();
		Move();
	}

	private void Look()
	{
		Vector2 lookInput = GetLookInput();
		m_XRotation -= lookInput.y;
		m_XRotation = Mathf.Clamp(m_XRotation, -90f, 90f);
		m_PlayerCamera.localRotation = Quaternion.Euler(m_XRotation, 0f, 0f);
		base.transform.Rotate(Vector3.up * lookInput.x, Space.World);
	}

	private void Move()
	{
		Vector3 movementInput = GetMovementInput();
		Vector3 vector = base.transform.right * movementInput.x + base.transform.forward * movementInput.z;
		m_CharacterController.Move(vector * m_MovementSpeed * Time.deltaTime);
	}

	private Vector2 GetLookInput()
	{
		float x = 0f;
		float y = 0f;
		if (m_MoveWithMouse)
		{
			x = Input.GetAxis("Mouse X") * m_MouseSensitivity * Time.deltaTime;
			y = Input.GetAxis("Mouse Y") * m_MouseSensitivity * Time.deltaTime;
		}
		return new Vector2(x, y);
	}

	private Vector3 GetMovementInput()
	{
		float x = 0f;
		float z = 0f;
		if (m_MoveWithMouse)
		{
			x = Input.GetAxis("Horizontal");
			z = Input.GetAxis("Vertical");
		}
		return new Vector3(x, 0f, z);
	}
}

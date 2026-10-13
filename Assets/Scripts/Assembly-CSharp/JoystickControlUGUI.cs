using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using _Car_Parking;

public class JoystickControlUGUI : MonoBehaviour, IPointerDownHandler, IEventSystemHandler, IPointerUpHandler
{
	public Transform target;

	public Transform rotateTransform;

	public Vector3 targetOffset;

	public float distance = 5f;

	public float maxDistance = 20f;

	public float minDistance = 0.6f;

	public float xSpeed = 5f;

	public float ySpeed = 5f;

	public int yMinLimit = -80;

	public int yMaxLimit = 80;

	public float zoomRate = 10f;

	public float panSpeed = 0.3f;

	public float zoomDampening = 5f;

	private float xDeg;

	private float yDeg;

	private float currentDistance;

	private float desiredDistance;

	private Quaternion currentRotation;

	private Quaternion desiredRotation;

	private Quaternion rotation;

	private Vector3 FirstPosition;

	private Vector3 SecondPosition;

	private Vector3 delta;

	private Vector3 lastOffset;

	private Vector3 lastOffsettemp;

	private Touchscreen screen;

	private bool Pressed { get; set; }

	private void Start()
	{
		Init();
	}

	private void OnEnable()
	{
		Init();
	}

	public void Init()
	{
		if (!target)
		{
			target = V.Car.transform;
		}
		distance = Vector3.Distance(rotateTransform.position, rotateTransform.position);
		currentDistance = distance;
		desiredDistance = distance;
		rotation = rotateTransform.rotation;
		currentRotation = rotateTransform.rotation;
		desiredRotation = rotateTransform.rotation;
		xDeg = Vector3.Angle(Vector3.right, rotateTransform.right);
		yDeg = Vector3.Angle(Vector3.up, rotateTransform.up);
		Debug.Log("CAMERA MOVEMENT INIT SUCCESS\nTOUCHSCREEN INPUT FOUND: " + (Touchscreen.current != null).ToString().ToUpper());
		screen = Touchscreen.current;
	}

	private void LateUpdate()
	{
		if (Pressed && !FlagsStorage.OutOfCar)
		{
			List<TouchControl> list = ActiveTouches(screen);
			Debug.Log("touches: " + list.Count);
			if (list.Count == 2)
			{
				Debug.Log("CAM_MOVE Two Touches");
				TouchControl touchControl = list[0];
				TouchControl touchControl2 = list[1];
				Vector2 vector = touchControl.position.ReadValue() - touchControl.delta.ReadValue();
				Vector2 vector2 = touchControl2.position.ReadValue() - touchControl2.delta.ReadValue();
				float magnitude = (vector - vector2).magnitude;
				float magnitude2 = (touchControl.position.ReadValue() - touchControl2.position.ReadValue()).magnitude;
				float num = magnitude - magnitude2;
				desiredDistance += num * Time.deltaTime * zoomRate * 0.0025f * Mathf.Abs(desiredDistance);
				Vector2 vector3 = (vector + vector2) / 2f;
				Vector2 vector4 = (touchControl.position.ReadValue() + touchControl2.position.ReadValue()) / 2f - vector3;
				lastOffset = targetOffset;
				float num2 = 0.003f * panSpeed * currentDistance;
				targetOffset = lastOffset + rotateTransform.right * vector4.x * num2 + Quaternion.Euler(0f, -90f, 0f) * base.transform.right * vector4.y * num2;
			}
			if (list.Count == 1 && list[0].phase.ReadValue() == UnityEngine.InputSystem.TouchPhase.Moved)
			{
				Debug.Log("CAM_MOVE Moving Single Touch");
				Vector2 vector5 = list[0].delta.ReadValue();
				xDeg += vector5.x * xSpeed * 0.002f;
				yDeg -= vector5.y * ySpeed * 0.002f;
				yDeg = ClampAngle(yDeg, yMinLimit, yMaxLimit);
			}
			desiredRotation = Quaternion.Euler(yDeg, xDeg, 0f);
			currentRotation = rotateTransform.rotation;
			rotation = Quaternion.Lerp(currentRotation, desiredRotation, Time.deltaTime * zoomDampening);
			rotateTransform.rotation = rotation;
			desiredDistance = Mathf.Clamp(desiredDistance, minDistance, maxDistance);
			currentDistance = Mathf.Lerp(currentDistance, desiredDistance, Time.deltaTime * zoomDampening);
		}
	}

	private static float ClampAngle(float angle, float min, float max)
	{
		if (angle < -360f)
		{
			angle += 360f;
		}
		if (angle > 360f)
		{
			angle -= 360f;
		}
		return Mathf.Clamp(angle, min, max);
	}

	private List<TouchControl> ActiveTouches(Touchscreen screen)
	{
		List<TouchControl> list = new List<TouchControl>();
		foreach (TouchControl touch in screen.touches)
		{
			if (touch.isInProgress)
			{
				list.Add(touch);
			}
		}
		return list;
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		Pressed = true;
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		Pressed = false;
	}
}

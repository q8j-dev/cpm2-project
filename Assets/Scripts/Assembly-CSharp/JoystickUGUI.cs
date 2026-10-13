using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class JoystickUGUI : MonoBehaviour, IPointerDownHandler, IEventSystemHandler, IDragHandler, IPointerUpHandler
{
	public enum Constraints
	{
		None = 0,
		Horizontal = 1,
		Vertical = 2
	}

	public RectTransform background;

	public RectTransform center;

	public float radius = 1f;

	public bool isFloating = true;

	public bool isSteeringWheel;

	public bool isBorder;

	public Constraints constraint;

	public float smoothReverseTime = 0.3f;

	public bool isKeyboardForSteeringWheel = true;

	public string axisSteeringWheel = "Horizontal";

	public List<UnityEvent<Vector2>> onJoystick = new List<UnityEvent<Vector2>>();

	public List<UnityEvent<float>> onSteeringWheel = new List<UnityEvent<float>>();

	private bool isDrag;

	private bool isReverse;

	private float timer;

	private Vector2 startPos = Vector2.zero;

	private Vector2 prevPos = Vector2.zero;

	private Vector2 lastPos = Vector2.zero;

	private float axisLastValue;

	public Vector2 ParameterDelta = Vector2.zero;

	private void Start()
	{
		if (isFloating)
		{
			background.gameObject.SetActive(value: false);
			center.gameObject.SetActive(value: false);
		}
		else if (isSteeringWheel)
		{
			center.gameObject.SetActive(value: false);
		}
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		PointerDown(eventData.position);
	}

	public void OnDrag(PointerEventData eventData)
	{
		PointerMove(eventData.position);
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		PointerUp(eventData.position);
	}

	private void PointerDown(Vector2 pos)
	{
		startPos = pos;
		center.position = pos;
		background.gameObject.SetActive(value: true);
		center.gameObject.SetActive(value: true);
		prevPos = pos;
		isDrag = true;
		isReverse = false;
	}

	private void PointerMove(Vector2 pos)
	{
		prevPos = pos;
	}

	private void PointerUp(Vector2 pos)
	{
		isDrag = false;
		isReverse = true;
		timer = 0f;
	}

	private void Update()
	{
		if (isDrag)
		{
			timer += Time.deltaTime;
			CalcPositionCenter(prevPos);
		}
		else if (isReverse)
		{
			timer += Time.deltaTime;
			if (timer > smoothReverseTime)
			{
				ReverseEnd();
			}
			else
			{
				CalcPositionCenter(prevPos);
			}
		}
		else if (isSteeringWheel && isKeyboardForSteeringWheel && Input.GetAxisRaw(axisSteeringWheel) != 0f)
		{
			float num = Mathf.Clamp01(Time.deltaTime * 3f) * Mathf.Sign(Input.GetAxisRaw(axisSteeringWheel));
			axisLastValue = Mathf.Clamp(axisLastValue + num, -1f, 1f);
			Vector2 position = GetPosition(axisLastValue);
			Vector2 pos = new Vector2(background.position.x + position.x, background.position.y + position.y);
			PointerDown(pos);
		}
		lastPos = prevPos;
	}

	private void ReverseEnd()
	{
		if (isFloating)
		{
			background.gameObject.SetActive(value: false);
			center.gameObject.SetActive(value: false);
		}
		else
		{
			center.localPosition = Vector3.zero;
		}
		SendMessageOnJoystick(Vector3.zero);
		isReverse = false;
	}

	private void CalcPositionCenter(Vector2 pos)
	{
		Vector2 vector = pos;
		Debug.Log("pos:" + vector.ToString());
		Vector2 b = pos - startPos;
		b = Vector2.Lerp(Vector2.zero, b, timer / smoothReverseTime);
		if (b.magnitude > radius)
		{
			b = b.normalized * radius;
		}
		center.localPosition = b;
		SendMessageOnJoystick(b);
	}

	private void SendMessageOnJoystick(Vector2 newPos)
	{
		float num = newPos.x / radius;
		float num2 = newPos.y / radius;
		Vector2 vector = new Vector2((num > 1f) ? 1f : num, (num2 > 1f) ? 1f : num2);
		Vector2 vector2 = vector;
		Debug.Log("delta:" + vector2.ToString());
		ParameterDelta = vector;
		foreach (UnityEvent<Vector2> item in onJoystick)
		{
			item.Invoke(vector);
		}
	}

	private Vector2 GetPosition(float i)
	{
		float f = i * (float)Math.PI;
		float x = Mathf.Cos(f);
		float y = Mathf.Sin(f);
		return new Vector2(x, y);
	}
}

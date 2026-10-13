using UnityEngine;

public class SmoothFieldOfView : MonoBehaviour
{
	public Camera camera3;

	public float targetField;

	private float fieldNow = 50f;

	public float speed = 1f;

	public Camera mainCamera;

	private void Awake()
	{
		base.enabled = false;
	}

	private void Update()
	{
		if (targetField - fieldNow != 0f)
		{
			fieldNow = Mathf.MoveTowards(fieldNow, targetField, speed * Time.deltaTime);
		}
		if ((bool)camera3)
		{
			camera3.fieldOfView = fieldNow;
		}
		if ((bool)mainCamera)
		{
			mainCamera.fieldOfView = fieldNow;
		}
	}
}

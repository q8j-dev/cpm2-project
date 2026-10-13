using UnityEngine;
using _Car_Parking;
using _Car_Parking.Scripts;

public class PersonCameraControll : MonoBehaviour
{
	public Transform target;

	public Transform rotationSpace;

	public bool lockCursor = true;

	public bool smoothFollow;

	public float followSpeed = 10f;

	private float distance = 5f;

	public float minDistance = 3f;

	public float maxDistance = 4f;

	public float zoomSpeed = 10f;

	public float zoomSensitivity = 1f;

	public float rotationSensitivity = 4.5f;

	public float yMinLimit = -10f;

	public float yMaxLimit = 80f;

	public Vector3 offset = new Vector3(0f, 1.5f, -0.7f);

	public bool rotateAlways = true;

	public bool rotateOnLeftButton;

	public bool rotateOnRightButton;

	public bool rotateOnMiddleButton;

	public float x;

	public float y;

	public Vector3 targetDistance;

	public Vector3 position;

	public Quaternion rotation = Quaternion.identity;

	public Vector3 smoothPosition;

	public bool RaycastHitted;

	public float RaycastDistace;

	private float RaycastDistaceLerp;

	private Camera cam;

	private Quaternion r = Quaternion.identity;

	private Vector3 lastUp;

	public float distanceTarget { get; private set; }

	private float zoomAdd
	{
		get
		{
			float axis = Input.GetAxis("Mouse ScrollWheel");
			if (axis > 0f)
			{
				return 0f - zoomSensitivity;
			}
			if (axis < 0f)
			{
				return zoomSensitivity;
			}
			return 0f;
		}
	}

	private void Start()
	{
		Vector3 eulerAngles = base.transform.eulerAngles;
		x = eulerAngles.y;
		y = eulerAngles.x;
		distanceTarget = distance;
		smoothPosition = base.transform.position;
		cam = V.CoreGameManager.transform.GetComponent<Camera>();
		lastUp = ((rotationSpace != null) ? rotationSpace.up : Vector3.up);
	}

	private void Update()
	{
		if (!target || !FlagsStorage.OutOfCar)
		{
			return;
		}
		Vector3 vector = target.position + rotation * (offset - Vector3.forward * 0f);
		Vector3 vector2 = vector - base.transform.position;
		LayerMask layerMask = -1;
		Debug.DrawRay(vector, -vector2, Color.green);
		if (V.PersonCamIndex == 0)
		{
			if (Physics.Raycast(vector, -vector2, out var hitInfo, maxDistance - 0.5f, layerMask.value, QueryTriggerInteraction.Ignore))
			{
				if (hitInfo.transform.gameObject.layer != 17)
				{
					RaycastHitted = true;
					distance = hitInfo.distance;
					RaycastDistace = distance - 0.3f;
				}
			}
			else if (RaycastHitted)
			{
				smoothPosition = target.position;
				RaycastDistace = distanceTarget;
				RaycastHitted = false;
				RaycastDistaceLerp = distance;
			}
		}
		UpdateTransform();
	}

	public void UpdateTransform()
	{
		if (!target || !FlagsStorage.OutOfCar)
		{
			return;
		}
		if (rotationSpace != null)
		{
			r = Quaternion.FromToRotation(lastUp, rotationSpace.up) * r;
			rotation = r * rotation;
			lastUp = rotationSpace.up;
		}
		if (V.PersonCamIndex == 0)
		{
			if (RaycastHitted)
			{
				RaycastDistaceLerp = Mathf.Lerp(RaycastDistaceLerp, RaycastDistace, Time.deltaTime * 3f);
				position = target.position + rotation * (offset - Vector3.forward * RaycastDistaceLerp);
			}
			else
			{
				distance += (distanceTarget - distance) * zoomSpeed * Time.deltaTime;
				smoothPosition = Vector3.Lerp(smoothPosition, target.position, Time.deltaTime * followSpeed);
				position = smoothPosition + rotation * (offset - Vector3.forward * distance);
			}
		}
		else
		{
			distance += (distanceTarget - distance) * zoomSpeed * Time.deltaTime;
			smoothPosition = Vector3.Lerp(smoothPosition, target.position, Time.deltaTime * followSpeed);
			position = smoothPosition + new Vector3(0f, 1.7f, 0f) + rotation * (offset - Vector3.forward * distance);
		}
		rotation = Quaternion.AngleAxis(x, Vector3.up) * Quaternion.AngleAxis(y, Vector3.right);
		base.transform.position = position;
		base.transform.rotation = rotation;
	}

	public void UpdateInput()
	{
		if (FlagsStorage.OutOfCar)
		{
			x += InputStorage.RightJoyInputX * rotationSensitivity;
			y = ClampAngle(y - InputStorage.RightJoyInputY * rotationSensitivity, yMinLimit, yMaxLimit);
			distanceTarget = Mathf.Clamp(distanceTarget, minDistance, maxDistance);
		}
	}

	private float ClampAngle(float angle, float min, float max)
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
}

using CarInteractionAnimationsScripts;
using UnityEngine;
using _Car_Parking;
using _Car_Parking.Scripts;

public class MovePlayer : MonoBehaviour
{
	public float rotateSpeed = 7f;

	public float blendSpeed = 10f;

	public float maxAngle = 90f;

	public float moveSpeed = 1.5f;

	public float rootMotionWeight;

	public PersonCameraControll pcc;

	public Transform cam;

	protected Vector3 moveBlend;

	protected Vector3 moveInput;

	protected Vector3 velocity;

	public bool OwnPlayer;

	public Vector3 input;

	public int emotions;

	public int Y;

	private PersonAnimationsController _personAnimationsController;

	private static Vector3 inputVector => new Vector3(InputStorage.LeftJoyInputX * 1.3f, 0f, InputStorage.LeftJoyInputY * 1.3f);

	private static Vector3 inputVectorRaw => new Vector3(InputStorage.LeftJoyInputX * 1.3f, 0f, InputStorage.LeftJoyInputY * 1.3f);

	private void Start()
	{
		_personAnimationsController = GetComponent<PersonAnimationsController>();
		pcc = V.CoreGameManager.GetComponent<PersonCameraControll>();
		cam = V.CoreGameManager.transform;
	}

	private void Update()
	{
		velocity = Vector3.Lerp(velocity, base.transform.rotation * Vector3.ClampMagnitude(moveInput, 1f) * moveSpeed, Time.deltaTime * blendSpeed);
		base.transform.position += Vector3.Lerp(velocity * Time.deltaTime, _personAnimationsController.Animator.deltaPosition, rootMotionWeight);
	}

	private void LateUpdate()
	{
		if ((bool)pcc && pcc.enabled && base.gameObject == V.DriverPerson && FlagsStorage.OutOfCar && !FlagsStorage.UseNewLocomotion)
		{
			pcc.UpdateInput();
			pcc.UpdateTransform();
			input = inputVector;
			input.y = Y;
			bool flag = inputVector != Vector3.zero || inputVectorRaw != Vector3.zero;
			Vector3 forward = cam.forward;
			Vector3 aimTarget = cam.position + forward * 10f;
			if (flag)
			{
				Y = 0;
				input.y = 0f;
				_personAnimationsController.StopSpecialAnimation();
			}
			Move(input, flag, forward, aimTarget);
		}
	}

	public virtual void Move(Vector3 moveInput, bool isMoving, Vector3 faceDirection, Vector3 aimTarget)
	{
		moveInput.y = 0f;
		this.moveInput = moveInput;
		Vector3 vector = base.transform.InverseTransformDirection(faceDirection);
		float num = Mathf.Atan2(vector.x, vector.z) * 57.29578f;
		float num2 = num * Time.deltaTime * rotateSpeed;
		if (num > maxAngle)
		{
			num2 = Mathf.Clamp(num2, num - maxAngle, num2);
		}
		if (num < 0f - maxAngle)
		{
			num2 = Mathf.Clamp(num2, num2, num + maxAngle);
		}
		base.transform.Rotate(Vector3.up, num2);
		moveBlend = Vector3.Lerp(moveBlend, moveInput, Time.deltaTime * blendSpeed);
		_personAnimationsController.PlayMoveAnimation(moveBlend.x, moveBlend.z);
		_personAnimationsController.SetBoolIsMoving(isMoving);
	}
}

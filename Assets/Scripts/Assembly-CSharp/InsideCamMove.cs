using UnityEngine;
using VContainer;
using _Car_Parking;
using _Car_Parking.Scripts.Scopes;

public class InsideCamMove : MonoBehaviour
{
	public bool Touching;

	public bool OnlyHeight;

	public bool MoveCamTransform;

	public float TargetMoveSpeed = 0.5f;

	public FixedJoystick LeftJoy;

	public FixedJoystick RightJoy;

	private CoreGameManager _coreGameManager;

	private CarCamerasController _carCamerasController;

	private CarCameras _carCameras;

	private MainPerson _mainPerson;

	private RacingCameraController _racingCameraController;

	private OptionalDependency<RacingCameraController> _racingCameraControllerDep;

	[Inject]
	public void Construct(OptionalDependency<RacingCameraController> racingCameraControllerDep, CoreGameManager coreGameManager)
	{
		_racingCameraControllerDep = racingCameraControllerDep;
		_coreGameManager = coreGameManager;
	}

	private void Start()
	{
		_carCamerasController = _coreGameManager.GetComponent<CarCamerasController>();
		_carCameras = _coreGameManager.GetComponent<CarCameras>();
		_racingCameraController = _racingCameraControllerDep?.Value;
		if (FlagsStorage.IsLevelScene)
		{
			TargetMoveSpeed = 5f;
		}
	}

	public void OnTouched()
	{
		Touching = true;
		FlagsStorage.MovingInsideCamPos = true;
	}

	public void OnRelease()
	{
		Touching = false;
		FlagsStorage.MovingInsideCamPos = false;
	}

	private void MoveCamTransformMethod(Vector3 input)
	{
		Transform transform = _coreGameManager.transform;
		Vector3 position = V.Car.transform.position;
		Vector3 vector = ((!OnlyHeight) ? (transform.position + transform.right * input.x + transform.up * input.y) : (transform.position + transform.forward * (input.y * 1.5f)));
		vector.y = Mathf.Clamp(vector.y, position.y, position.y + 20f);
		vector.x = Mathf.Clamp(vector.x, position.x - 20f, position.x + 20f);
		vector.z = Mathf.Clamp(vector.z, position.z - 20f, position.z + 20f);
		if (Vector3.Distance(vector, position) > 1f)
		{
			_coreGameManager.transform.position = _carCamerasController.MakeRaycast(vector);
		}
	}

	private void RelativeMoveMethod(Vector3 input)
	{
		if (V.Car.TryGetComponent<DriverController>(out var component) && (bool)component.camInsideView)
		{
			Transform transform = component.camInsideView.transform;
			if (OnlyHeight)
			{
				transform.localPosition += new Vector3(0f, input.y, 0f);
			}
			else
			{
				transform.localPosition += V.Car.transform.InverseTransformDirection(transform.right) * input.x + V.Car.transform.InverseTransformDirection(transform.forward) * input.y;
			}
			_racingCameraController?.MarkCameraSettingsChanged();
			Vector3 localPosition = transform.localPosition;
			localPosition.y = Mathf.Clamp(localPosition.y, -0.3f, 2f);
			localPosition.x = Mathf.Clamp(localPosition.x, -10f, 10f);
			localPosition.z = Mathf.Clamp(localPosition.z, -10f, 10f);
			transform.localPosition = localPosition;
			component.camPosInitialInsideView = localPosition;
		}
	}

	private void LeftJoyMove()
	{
		if ((bool)LeftJoy && LeftJoy.Direction == Vector2.zero)
		{
			Touching = false;
			return;
		}
		Touching = true;
		if ((bool)LeftJoy && (bool)V.Car)
		{
			Vector3 input = LeftJoy.Direction / 20f * (TargetMoveSpeed * Time.deltaTime);
			if (MoveCamTransform)
			{
				MoveCamTransformMethod(input);
			}
			else
			{
				RelativeMoveMethod(input);
			}
		}
	}

	private void RightJoyMove()
	{
		if ((bool)RightJoy && RightJoy.Direction == Vector2.zero)
		{
			Touching = false;
			return;
		}
		Touching = true;
		if ((bool)RightJoy)
		{
			_carCameras.rotationAngle = Mathf.Clamp(_carCameras.rotationAngle + RightJoy.Horizontal, -90f, 90f);
		}
	}

	private void Update()
	{
		LeftJoyMove();
		RightJoyMove();
		TrySwitchVisibleHeadPerson(_coreGameManager.transform);
	}

	private void TrySwitchVisibleHeadPerson(Transform cameraTransform)
	{
		if ((bool)_mainPerson || ((bool)V.DriverPerson && V.DriverPerson.TryGetComponent<MainPerson>(out _mainPerson)))
		{
			_mainPerson.TrySwitchVisibleHeadPerson(cameraTransform);
		}
	}
}

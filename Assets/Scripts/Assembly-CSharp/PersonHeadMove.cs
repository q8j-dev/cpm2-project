using System.Threading;
using Cysharp.Threading.Tasks;
using RootMotion.FinalIK;
using UnityEngine;
using _Car_Parking.Scripts.Person.Locomotion;

public class PersonHeadMove : MonoBehaviour
{
	public DriverActionController driverActionController;

	public FullBodyBipedIK ik;

	public LookAtIK lookAt;

	public MultiPersonControl multiPersonControl;

	public Transform toLookAt;

	public Transform goal;

	public float speed = 3f;

	public float speedCorrection = 1f;

	private FPSDisplay2 _fpsDisplay;

	private CancellationTokenSource _token;

	private bool lookAtVelo;

	public Transform closetestTarget;

	private float lastClosestUpdateTime;

	private Vector3 _velo;

	public Vector3 lastPosGoal;

	private float _lastTime;

	public bool disableAimIk;

	private readonly float _minFps = 25f;

	private PersonLocomotion _locomotion;

	public float idleLookMaxSpeed = 0.5f;

	public float maxAngle = 85f;

	public float minCameraLookAngle = 28f;

	public Vector3 yOffset;

	private void Start()
	{
		ik = driverActionController.ik;
		_token = new CancellationTokenSource();
		_ = AddIkHead(_token);
		_fpsDisplay = V.CoreGameManager.gameObject.GetComponent<FPSDisplay2>();
	}

	private void OnDestroy()
	{
		if ((bool)toLookAt)
		{
			Object.Destroy(toLookAt.gameObject);
		}
		if ((bool)goal)
		{
			Object.Destroy(goal.gameObject);
		}
		_token?.Cancel();
	}

	private async UniTask AddIkHead(CancellationTokenSource t)
	{
		if (!ik)
		{
			return;
		}
		while (multiPersonControl.DistanceToDriverPerson > 50f)
		{
			await UniTask.Delay(1000);
			if (t.IsCancellationRequested)
			{
				return;
			}
		}
		FullBodyBipedIK fullBodyBipedIK = ik;
		lookAt = GetComponent<LookAtIK>();
		if (!lookAt)
		{
			lookAt = base.gameObject.AddComponent<LookAtIK>();
		}
		if (lookAt.solver == null)
		{
			lookAt.solver = new IKSolverLookAt();
		}
		Transform head = fullBodyBipedIK.references.head;
		Transform transform = (((bool)head && (bool)head.parent) ? head.parent : ((fullBodyBipedIK.references.spine != null && fullBodyBipedIK.references.spine.Length != 0) ? fullBodyBipedIK.references.spine[fullBodyBipedIK.references.spine.Length - 1] : null));
		lookAt.solver.SetChain((!transform) ? null : new Transform[1] { transform }, head, null, ik.references.root);
		lookAt.solver.bodyWeight = 0f;
		lookAt.solver.headWeight = 0.8f;
		lookAt.solver.eyesWeight = 0f;
	}

	private void CreateHeadTarget()
	{
		toLookAt = new GameObject("Head target" + multiPersonControl.ThisPlayerName).transform;
		if ((bool)V.GlobalPoint)
		{
			toLookAt.transform.parent = V.GlobalPoint.transform;
		}
		goal = new GameObject("Head goal").transform;
		if ((bool)V.GlobalPoint)
		{
			goal.transform.parent = V.GlobalPoint.transform;
		}
		lookAt.solver.target = toLookAt;
	}

	private void Update()
	{
		if ((bool)lookAt)
		{
			MoveHead();
		}
	}

	private bool IsMovingOnFoot()
	{
		if (!_locomotion)
		{
			_locomotion = GetComponent<PersonLocomotion>();
		}
		if (!_locomotion)
		{
			return false;
		}
		return _locomotion.Snapshot.MoveSpeed > idleLookMaxSpeed;
	}

	private void MoveHead()
	{
		if (!lookAt || Time.timeScale == 0f)
		{
			return;
		}
		if (disableAimIk)
		{
			if (lookAt.enabled)
			{
				if (lookAt.solver.headWeight < 0.2f)
				{
					lookAt.enabled = false;
				}
				lookAt.solver.headWeight = Mathf.Lerp(lookAt.solver.headWeight, 0f, Time.deltaTime * 2f);
			}
		}
		else
		{
			lookAt.enabled = true;
			if (lookAt.solver.headWeight < 0.8f)
			{
				lookAt.solver.headWeight = Mathf.Lerp(lookAt.solver.headWeight, 1f, Time.deltaTime * 2f);
			}
			else
			{
				lookAt.solver.headWeight = 0.85f;
			}
		}
		if (!goal)
		{
			CreateHeadTarget();
		}
		if (Time.frameCount % 3 == 0)
		{
			speed = 1f + Vector3.Distance(goal.transform.position, lastPosGoal) * speedCorrection / (Time.time - _lastTime);
			lastPosGoal = goal.transform.position;
			_lastTime = Time.time;
		}
		if ((bool)multiPersonControl.currentCar)
		{
			if ((bool)driverActionController.carRb)
			{
				_velo = driverActionController.carRb.velocity;
				if (_velo.magnitude > 5f)
				{
					disableAimIk = true;
				}
				else
				{
					LookAtClosest();
				}
			}
		}
		else if (IsMovingOnFoot())
		{
			disableAimIk = true;
		}
		else
		{
			LookAtClosest();
		}
	}

	private bool IsCameraHeadOn(Transform cameraTransform)
	{
		Vector3 to = Vector3.ProjectOnPlane(cameraTransform.position - base.transform.position, Vector3.up);
		if (to.sqrMagnitude < 0.0001f)
		{
			return false;
		}
		return Vector3.Angle(Vector3.ProjectOnPlane(base.transform.forward, Vector3.up), to) < minCameraLookAngle;
	}

	private bool InFront(Transform self, Transform target)
	{
		Vector3 to = target.position - self.position;
		if (to.sqrMagnitude < 0.0001f)
		{
			return false;
		}
		return Vector3.Angle(self.forward, to) <= maxAngle;
	}

	private void UpdateClosestTarget()
	{
		if (!V.DriverPerson || !this.multiPersonControl || this.multiPersonControl.DistanceToDriverPerson > 40f)
		{
			return;
		}
		closetestTarget = null;
		yOffset = new Vector3(0f, 0f, 0f);
		float num = 200f;
		GameObject currentCar = this.multiPersonControl.currentCar;
		if ((bool)currentCar)
		{
			MultiPersonControl component = V.DriverPerson.GetComponent<MultiPersonControl>();
			if (component.isPassanger)
			{
				return;
			}
			GameObject currentCar2 = component.currentCar;
			if (!currentCar2 || currentCar2 != this.multiPersonControl.currentCar)
			{
				return;
			}
		}
		if (Random.Range(0, 100) < 80)
		{
			if (!V.CoreGameManager || !V.CoreGameManager.MainCamera || (!closetestTarget && Mathf.Abs(base.transform.position.y - V.CoreGameManager.MainCamera.transform.position.y) > 5f))
			{
				return;
			}
			Transform transform = V.CoreGameManager.MainCamera.transform;
			if (InFront(base.transform, transform) && speed < 20f && Mathf.Abs(base.transform.position.y - transform.position.y) < 5f && (bool)currentCar)
			{
				closetestTarget = transform;
				num = Vector3.Distance(base.transform.position, closetestTarget.position);
				if ((bool)currentCar)
				{
					yOffset = new Vector3(0f, -2f, 0f);
				}
			}
			if (this.multiPersonControl.isPassanger)
			{
				return;
			}
			if (base.gameObject != V.DriverPerson && InFront(base.transform, V.DriverPerson.transform))
			{
				float num2 = Vector3.Distance(base.transform.position, V.DriverPerson.transform.position);
				DriverActionController component2 = V.DriverPerson.GetComponent<DriverActionController>();
				if (num2 < num && (bool)component2 && (bool)component2.ik)
				{
					closetestTarget = component2.ik.references.head;
					num = num2;
				}
			}
			MultiPersonControl[] playersOnlineNow = V.CoreGameManager._onlineCollectionsProvider.PlayersOnlineNow;
			foreach (MultiPersonControl multiPersonControl in playersOnlineNow)
			{
				if ((bool)multiPersonControl && !(multiPersonControl.gameObject == base.gameObject) && (!currentCar || !multiPersonControl.currentCar) && (!multiPersonControl || InFront(base.transform, multiPersonControl.transform)))
				{
					float num3 = Vector3.Distance(multiPersonControl.transform.position, base.transform.position);
					if (num3 < num && (bool)multiPersonControl.driverActionController.ik)
					{
						closetestTarget = multiPersonControl.driverActionController.ik.references.head;
						num = num3;
					}
				}
			}
			MultiPlayerPosControl[] carsOnlineNow = V.CoreGameManager._onlineCollectionsProvider.CarsOnlineNow;
			foreach (MultiPlayerPosControl multiPlayerPosControl in carsOnlineNow)
			{
				if (!multiPlayerPosControl || this.multiPersonControl.currentCar == multiPlayerPosControl.gameObject)
				{
					continue;
				}
				Transform target = multiPlayerPosControl.VehicleData.Collider.transform;
				if (InFront(base.transform, target))
				{
					float num4 = Vector3.Distance(multiPlayerPosControl.gameObject.transform.position, base.transform.position);
					if (num4 < num)
					{
						closetestTarget = target;
						num = num4;
						yOffset = new Vector3(0f, 1.2f, 0f);
					}
				}
			}
		}
		if ((bool)currentCar)
		{
			yOffset = new Vector3(0f, -2f, 0f);
		}
	}

	private void LookAtClosest()
	{
		if (Time.time - lastClosestUpdateTime > 5f)
		{
			UpdateClosestTarget();
			lastClosestUpdateTime = Time.time;
		}
		if (_fpsDisplay.averageFps < _minFps)
		{
			disableAimIk = true;
		}
		if (!closetestTarget)
		{
			disableAimIk = true;
			return;
		}
		disableAimIk = false;
		goal.transform.position = closetestTarget.position;
		toLookAt.transform.position = Vector3.Lerp(toLookAt.position, goal.position + yOffset, Time.deltaTime * speed);
		if (!InFront(base.transform, goal.transform))
		{
			closetestTarget = null;
			disableAimIk = true;
		}
		else if (!multiPersonControl.currentCar && (bool)V.CoreGameManager && (bool)V.CoreGameManager.MainCamera && closetestTarget == V.CoreGameManager.MainCamera.transform)
		{
			closetestTarget = null;
			disableAimIk = true;
		}
	}
}

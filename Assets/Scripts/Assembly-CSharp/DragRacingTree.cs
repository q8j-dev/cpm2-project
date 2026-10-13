using System;
using System.Collections.Generic;
using System.Threading;
using CPMEngine;
using Cysharp.Threading.Tasks;
using ElectronicRoadSign;
using UniRx;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Racing_Scripts;

public class DragRacingTree : MonoBehaviour
{
	[Serializable]
	public class OneRacingTreeLight
	{
		public MeshRenderer lightMesh;

		public Material mat;

		public MeshRenderer[] glow;

		public Color enabledC = Color.white;

		public Color disabledC = Color.gray;
	}

	public enum PositionOnTree
	{
		left = 0,
		right = 1,
		None = 2
	}

	public OneRacingTreeLight[] LMats;

	public OneRacingTreeLight[] RMats;

	public GameObject CarLeft;

	public GameObject CarRight;

	public BoxCollider StartCollider;

	public int intensity;

	public bool LeftCarStaged;

	public bool RightCarStaged;

	public bool LeftCarFalseStart;

	public bool RightCarFalseStart;

	public bool GreenLightWillOn;

	public float DistanceToFinishOnStartLeft;

	public float DistanceToFinishOnStarRight;

	public Transform finish;

	public float reactionTimeLeft;

	public float reactionTimeRight;

	public float startTimeLeft;

	public float startTimeRight;

	public float launchTime;

	public float LTimeOnStage;

	public float RTimeOnStage;

	public bool AfterStage;

	public float prestageDistance = 0.1f;

	public float stageRange = 0.1f;

	public float leftDistance;

	public float rightDistance;

	public float LeftCarTime;

	public int leftBlinkCount;

	public int rightBlinkCount;

	public float RightCarTime;

	public Collider car1InsideCheck;

	public Collider car2InsideCheck;

	public Collider carLeftInsideOwnPath;

	public Collider carRightInsideOwnPath;

	public Transform LeftSideStartPoint;

	public Transform RightSideStartPoint;

	public Collider finishCollider1_4;

	public Collider finishCollider1_2;

	public Transform toLookForCam;

	public float distanceCheck;

	public float distanceCarLeftToFinish;

	public float distanceCarRightToFinish;

	public ElectronicRoadSignScript leftCarBanner;

	public ElectronicRoadSignScript rightCarBanner;

	private bool camOnFinishShowed;

	public bool outOfRoad;

	private float timeOfFixedTime;

	public DragRacingController controller;

	public bool leftCar1_4reached;

	public bool rightCar1_4reached;

	public Transform leftSideTarget;

	public Transform rightSideTarget;

	private Rigidbody ownRigidbody;

	public Action<float> LeftCarDistance;

	public Action<float> RightCarDistance;

	private CarComponents _carLeftComponents;

	private CarComponents _carRightComponents;

	private FalseStartBotController _falseStartBotController;

	private const float PlaySlowMoEventThreshold = 30f;

	private const float SlowMoDistanceThreshold = 5f;

	private List<Material> _glowMats = new List<Material>();

	private List<Material> _colorsMats = new List<Material>();

	private readonly MaterialAssetStateGuard _materialGuard = new MaterialAssetStateGuard();

	private CarInputManager _carInputManager;

	private IDisposable _onOutOfRoadDisposable;

	private IDisposable _resetOwnRigidBodyDisposable;

	private const float SlowMoDistanceToFinish = 10f;

	private const float CarsDistanceToEnableSlowMo = 10f;

	private DragRacingConfig DragRacingConfig => controller.DragRacingConfig;

	public DragRacingTree()
	{
		RightCarFalseStart = false;
		GreenLightWillOn = false;
		RightCarStaged = false;
	}

	[Inject]
	public void Construct(CarInputManager carInputManager)
	{
		_carInputManager = carInputManager;
	}

	private void Awake()
	{
		_falseStartBotController = new FalseStartBotController(DragRacingConfig);
	}

	private void Start()
	{
		intensity = Shader.PropertyToID("_Intensity");
		OneRacingTreeLight[] lMats = LMats;
		foreach (OneRacingTreeLight oneRacingTreeLight in lMats)
		{
			_materialGuard.Capture(oneRacingTreeLight.mat);
		}
		lMats = RMats;
		foreach (OneRacingTreeLight oneRacingTreeLight2 in lMats)
		{
			_materialGuard.Capture(oneRacingTreeLight2.mat);
		}
		lMats = LMats;
		foreach (OneRacingTreeLight oneRacingTreeLight3 in lMats)
		{
			if (oneRacingTreeLight3.glow.Length != 0)
			{
				MeshRenderer[] glow = oneRacingTreeLight3.glow;
				foreach (MeshRenderer meshRenderer in glow)
				{
					Material material = new Material(Shader.Find("Legacy Shaders/Particles/Additive"));
					material.SetTexture("_MainTex", meshRenderer.sharedMaterial.GetTexture("_MainTex"));
					material.SetColor("_TintColor", oneRacingTreeLight3.enabledC);
					meshRenderer.sharedMaterial = material;
					meshRenderer.enabled = false;
					_glowMats.Add(material);
				}
			}
		}
		lMats = RMats;
		foreach (OneRacingTreeLight oneRacingTreeLight4 in lMats)
		{
			if (oneRacingTreeLight4.glow.Length != 0)
			{
				MeshRenderer[] glow = oneRacingTreeLight4.glow;
				foreach (MeshRenderer meshRenderer2 in glow)
				{
					Material material2 = new Material(Shader.Find("Legacy Shaders/Particles/Additive"));
					material2.SetTexture("_MainTex", meshRenderer2.sharedMaterial.GetTexture("_MainTex"));
					material2.SetColor("_TintColor", oneRacingTreeLight4.enabledC);
					meshRenderer2.sharedMaterial = material2;
					meshRenderer2.enabled = false;
					_glowMats.Add(material2);
				}
			}
		}
		ResetAllLights();
	}

	private void OnDestroy()
	{
		ResetAllLights();
		foreach (Material glowMat in _glowMats)
		{
			Material mat = glowMat;
			CommonExtensions.DestroyMat(ref mat);
		}
		foreach (Material colorsMat in _colorsMats)
		{
			Material mat2 = colorsMat;
			CommonExtensions.DestroyMat(ref mat2);
		}
		OneRacingTreeLight[] lMats = LMats;
		for (int i = 0; i < lMats.Length; i++)
		{
			lMats[i].mat.SetFloat(intensity, 0f);
		}
		lMats = RMats;
		for (int i = 0; i < lMats.Length; i++)
		{
			lMats[i].mat.SetFloat(intensity, 0f);
		}
		_materialGuard.Restore();
	}

	private void FixedUpdate()
	{
		CheckCarsMove();
	}

	public void OnStarNewRace()
	{
		_falseStartBotController.Clear();
	}

	public void ResetTree()
	{
		StopMeasuringSplits(_carLeftComponents);
		StopMeasuringSplits(_carRightComponents);
		ResetAllLights();
		outOfRoad = false;
		LeftCarStaged = false;
		RightCarStaged = false;
		LeftCarFalseStart = false;
		RightCarFalseStart = false;
		CarLeft = null;
		CarRight = null;
		_carLeftComponents = new CarComponents(null);
		_carRightComponents = new CarComponents(null);
		launchTime = 0f;
		reactionTimeLeft = 0f;
		reactionTimeRight = 0f;
		startTimeLeft = 0f;
		startTimeRight = 0f;
		DistanceToFinishOnStarRight = 0f;
		DistanceToFinishOnStartLeft = 0f;
		distanceCarRightToFinish = 0f;
		distanceCarLeftToFinish = 0f;
		AfterStage = false;
		GreenLightWillOn = false;
		LeftCarTime = 0f;
		RightCarTime = 0f;
		camOnFinishShowed = false;
		_onOutOfRoadDisposable?.Dispose();
		leftCar1_4reached = false;
		rightCar1_4reached = false;
		Time.timeScale = 1f;
	}

	private static void StopMeasuringSplits(CarComponents car)
	{
		if ((bool)car.Dragy)
		{
			car.Dragy.MeasureSplits = false;
		}
	}

	public bool carOnRacing(GameObject car)
	{
		if (!(CarLeft == car))
		{
			return CarRight == car;
		}
		return true;
	}

	private void ResetAllLights()
	{
		OneRacingTreeLight[] lMats = LMats;
		foreach (OneRacingTreeLight oneLight in lMats)
		{
			SetMat(oneLight, on: false);
		}
		lMats = RMats;
		foreach (OneRacingTreeLight oneLight2 in lMats)
		{
			SetMat(oneLight2, on: false);
		}
	}

	private void SetMat(Color c, float i, Material m)
	{
		m.SetFloat(intensity, i);
		m.color = c;
	}

	private void SetMat(OneRacingTreeLight oneLight, bool on)
	{
		oneLight.mat.SetFloat(intensity, on ? 3 : 0);
		oneLight.mat.SetColor("_Color", on ? oneLight.enabledC : oneLight.disabledC);
		MeshRenderer[] glow = oneLight.glow;
		foreach (MeshRenderer meshRenderer in glow)
		{
			if ((bool)meshRenderer)
			{
				meshRenderer.enabled = on;
			}
		}
	}

	private void CancelPreStageRightCar()
	{
		SetMat(RMats[0], on: false);
	}

	private void CancelPreStageLeftCar()
	{
		SetMat(LMats[0], on: false);
	}

	private void CancelStageLeftCar()
	{
		SetMat(LMats[1], on: false);
	}

	private void CancelStageRightCar()
	{
		SetMat(RMats[1], on: false);
	}

	private async UniTask willTurnOnGreenAsync(CancellationToken ct = default(CancellationToken))
	{
		_ = 3;
		try
		{
			float randomTime = UnityEngine.Random.Range(1f, 3.8f);
			float timing = DragRacingConfig.DelayToTurnTree;
			if (controller.alwaysWin)
			{
				randomTime = 0.5f;
				timing = 0.1f;
			}
			GreenLightWillOn = true;
			while (randomTime > 0f)
			{
				await UniTask.Delay(TimeSpan.FromSeconds(0.10000000149011612), ignoreTimeScale: false, PlayerLoopTiming.Update, ct);
				randomTime -= 0.1f;
				bool num = !controller.testRun && (!LeftCarStaged || !RightCarStaged);
				bool flag = controller.testRun && !LeftCarStaged && !RightCarStaged;
				if (num | flag)
				{
					GreenLightWillOn = false;
					MonoBehaviour.print("cancelled green");
					return;
				}
			}
			AfterStage = true;
			beforeStart(_carLeftComponents);
			beforeStart(_carRightComponents);
			CancelPreStageLeftCar();
			CancelPreStageRightCar();
			CancelStageLeftCar();
			CancelStageRightCar();
			SetMat(LMats[2], on: true);
			SetMat(RMats[2], on: true);
			await UniTask.Delay(TimeSpan.FromSeconds(timing), ignoreTimeScale: false, PlayerLoopTiming.Update, ct);
			SetMat(LMats[2], on: false);
			SetMat(LMats[3], on: true);
			SetMat(RMats[2], on: false);
			SetMat(RMats[3], on: true);
			await UniTask.Delay(TimeSpan.FromSeconds(timing), ignoreTimeScale: false, PlayerLoopTiming.Update, ct);
			SetMat(LMats[3], on: false);
			SetMat(LMats[4], on: true);
			SetMat(RMats[3], on: false);
			SetMat(RMats[4], on: true);
			CarCanStartAsync(_carLeftComponents, ct).Forget();
			CarCanStartAsync(_carRightComponents, ct).Forget();
			await UniTask.Delay(TimeSpan.FromSeconds(timing), ignoreTimeScale: false, PlayerLoopTiming.Update, ct);
			SetMat(LMats[4], on: false);
			SetMat(LMats[5], on: true);
			SetMat(RMats[4], on: false);
			SetMat(RMats[5], on: true);
			launchTime = Time.time;
			controller.recordFinishResult = true;
			Vibrate(0.05f, 0.3f);
			if ((!controller.testRun && !controller.qualification) || controller.IsRandomCarsRace)
			{
				StartRaceDragSpeakerEvent().Forget();
			}
		}
		catch (OperationCanceledException)
		{
		}
	}

	private async UniTaskVoid StartRaceDragSpeakerEvent()
	{
		controller.SoundEventChannel.InvokeDragSpeakerEvent("Green_Light");
		await UniTask.Delay(TimeSpan.FromSeconds(2.5));
		controller.SoundEventChannel.InvokeDragSpeakerEvent("After_Launch");
	}

	private void beforeStart(CarComponents car)
	{
		if (!(car.GameObject == null))
		{
			car.VehicleData.racingState = RacingState.preLaunch;
			if (car.GameObject == V.Car)
			{
				_carInputManager.State = 3;
			}
		}
	}

	private async UniTask CarCanStartAsync(CarComponents car, CancellationToken ct = default(CancellationToken))
	{
		float num = UnityEngine.Random.Range(0.1f, 1.2f);
		if ((bool)car.GameObject && controller.OtherCar == car.GameObject)
		{
			num = ((!_falseStartBotController.GetNewIsFalseStart()) ? (DragRacingConfig.DelayToTurnTree + DragRacingConfig.Bot.DelayToStartBot.GetRandom()) : UnityEngine.Random.Range(0.1f, DragRacingConfig.DelayToTurnTree - 0.1f));
		}
		if (controller.alwaysWin)
		{
			num = 0.2f;
		}
		await UniTask.Delay(TimeSpan.FromSeconds(num), ignoreTimeScale: false, PlayerLoopTiming.Update, ct);
		LaunchCarNow(car);
	}

	private void OnOutOfRoad()
	{
		if (V.LastUsedCar.GetComponent<VehicleData>().racingState == RacingState.OnRacing)
		{
			controller.OnGarageOpen();
		}
	}

	private void CheckFinishDistance(CarComponents car)
	{
		VehicleData vehicleData = car.VehicleData;
		if (car.GameObject == null || vehicleData.racingState != RacingState.OnRacing)
		{
			return;
		}
		Axles axles = car.Axles;
		Vector3 vector = 0.5f * (axles.FrontAxle.LeftWheel.ModelTransform.position + axles.FrontAxle.RightWheel.ModelTransform.position);
		if (Time.frameCount % 5 == 0 && car.GameObject == V.Car && !outOfRoad)
		{
			float num = ((!(car.GameObject == CarLeft)) ? Vector3.Distance(carRightInsideOwnPath.ClosestPoint(vector), vector) : Vector3.Distance(carLeftInsideOwnPath.ClosestPoint(vector), vector));
			if (num > 2f)
			{
				car.Dragy.lost = true;
				car.Dragy.reason = Dragy.LostReason.outOfRoad;
				outOfRoad = true;
				_onOutOfRoadDisposable = Observable.Timer(TimeSpan.FromSeconds(5.0)).Subscribe((long _) =>
				{
					OnOutOfRoad();
				}).AddTo(this);
			}
		}
		float num2 = Vector3.Distance(finishCollider1_4.ClosestPoint(vector), vector);
		float num3 = Vector3.Distance(finishCollider1_2.ClosestPoint(vector), vector);
		float num4 = 1f / Time.fixedDeltaTime;
		distanceCheck = car.Rigidbody.velocity.magnitude / num4;
		if (controller.runDistance == DragRacingController.RunDistance.QuaterMile)
		{
			if (car.GameObject == CarLeft)
			{
				distanceCarLeftToFinish = num2;
			}
			else
			{
				distanceCarRightToFinish = num2;
			}
		}
		else if (car.GameObject == CarLeft)
		{
			distanceCarLeftToFinish = num3;
		}
		else
		{
			distanceCarRightToFinish = num3;
		}
		if (num2 < distanceCheck)
		{
			if (controller.runDistance == DragRacingController.RunDistance.QuaterMile)
			{
				controller.OnCarFinish(car);
				if (car.GameObject == CarLeft)
				{
					distanceCarLeftToFinish = 0f;
					SetBannerLeft(car);
				}
				else
				{
					distanceCarRightToFinish = 0f;
					SetBannerRight(car);
				}
			}
			else if (car.GameObject == CarLeft)
			{
				if (!leftCar1_4reached)
				{
					car.Dragy.on402Reached();
					leftCar1_4reached = true;
				}
			}
			else if (!rightCar1_4reached)
			{
				car.Dragy.on402Reached();
				rightCar1_4reached = true;
			}
		}
		if ((controller.runDistance == DragRacingController.RunDistance.OneMile || controller.runDistance == DragRacingController.RunDistance.HalfMile) && num3 < distanceCheck)
		{
			controller.OnCarFinish(car);
			if (car.GameObject == CarLeft)
			{
				distanceCarLeftToFinish = 0f;
				SetBannerLeft(car);
			}
			else
			{
				distanceCarRightToFinish = 0f;
				SetBannerRight(car);
			}
		}
	}

	public void Vibrate(float duration, float intensity)
	{
		Utils.Vibrate(duration, intensity);
	}

	private void CheckForCloseFinish()
	{
		if (!camOnFinishShowed && distanceCarRightToFinish != 0f && distanceCarLeftToFinish != 0f && (CarLeft == V.Car || CarRight == V.Car) && CarsCloseToFinish(10f) && CarsCloseToEachOther(10f))
		{
			camOnFinishShowed = true;
			controller.dragRacingCamera.OnCloseFinish();
		}
	}

	private bool CarsCloseToFinish(float distance)
	{
		if (distanceCarRightToFinish < distance)
		{
			return distanceCarLeftToFinish < distance;
		}
		return false;
	}

	private bool CarsCloseToEachOther(float distance)
	{
		return Mathf.Abs(distanceCarLeftToFinish - distanceCarRightToFinish) < distance;
	}

	private void CheckCarsMove()
	{
		if ((bool)CarLeft)
		{
			CheckLeftApproachingCar(_carLeftComponents);
			CheckFinishDistance(_carLeftComponents);
		}
		if ((bool)CarRight)
		{
			CheckRightApproachingCar(_carRightComponents);
			CheckFinishDistance(_carRightComponents);
		}
		if ((bool)CarRight && (bool)CarLeft)
		{
			CheckForCloseFinish();
		}
		if (controller.testRun)
		{
			if ((LeftCarStaged || RightCarStaged) && !GreenLightWillOn && (LTimeOnStage > 1f || RTimeOnStage > 1f))
			{
				willTurnOnGreenAsync(this.GetCancellationTokenOnDestroy()).Forget();
			}
		}
		else if (LeftCarStaged && RightCarStaged && !GreenLightWillOn && LTimeOnStage > 1f && RTimeOnStage > 1f)
		{
			willTurnOnGreenAsync(this.GetCancellationTokenOnDestroy()).Forget();
		}
	}

	public GameObject getOpponentCar(GameObject car)
	{
		if (CarLeft == car)
		{
			return CarRight;
		}
		return CarLeft;
	}

	private void LaunchCarNow(CarComponents car)
	{
		if ((bool)car.GameObject)
		{
			car.VehicleData.racingState = RacingState.OnRacing;
		}
	}

	private void CheckLeftApproachingCar(CarComponents car)
	{
		Axles axles = car.Axles;
		VehicleData vehicleData = car.VehicleData;
		Vector3 position = vehicleData.Collider.transform.transform.position;
		Vector3 b = car1InsideCheck.ClosestPoint(position);
		float num = Vector3.Distance(position, b);
		bool flag = V.Car != car.GameObject;
		if (num > 1f)
		{
			if (!flag)
			{
				SetCircleUI(10f);
			}
			if (!LeftCarFalseStart || reactionTimeLeft != 0f)
			{
				return;
			}
		}
		Dragy dragy = car.Dragy;
		int racingState = (int)vehicleData.racingState;
		Vector3 vector = 0.5f * (axles.FrontAxle.LeftWheel.ModelTransform.position + axles.FrontAxle.RightWheel.ModelTransform.position);
		Vector3 a = StartCollider.ClosestPoint(vector);
		Vector3 to = finish.TransformDirection(Vector3.forward);
		if (Vector3.Angle(car.Transform.forward, to) < 120f)
		{
			return;
		}
		float num2 = (leftDistance = Vector3.Distance(a, vector));
		if (vehicleData.racingState == RacingState.staged)
		{
			LTimeOnStage = Time.deltaTime + LTimeOnStage;
			DistanceToFinishOnStartLeft = Vector3.Distance(vector, finish.transform.position);
			if (flag)
			{
				controller.dragRacingUI.setCircleStage(LTimeOnStage);
			}
		}
		else
		{
			LTimeOnStage = 0f;
		}
		if (num2 == 0f && !AfterStage)
		{
			controller.ResetCarPositions();
			return;
		}
		if (!AfterStage)
		{
			if (!LeftCarStaged)
			{
				if (num2 < 5f + prestageDistance && flag && (bool)car.CarAIController)
				{
					car.CarAIController.TargetSpeed = 5f + (float)UnityEngine.Random.Range(-1, 2);
				}
				if (num2 < 2f + prestageDistance && !GreenLightWillOn)
				{
					Rigidbody rigidbody = car.Rigidbody;
					if (rigidbody.velocity.magnitude > 2f)
					{
						rigidbody.velocity = Vector3.ClampMagnitude(rigidbody.velocity, 2f);
						if (!flag)
						{
							_carInputManager.TargetSpeed = 2f;
						}
					}
				}
			}
			float num3 = num2 - prestageDistance;
			LeftCarDistance?.Invoke(num3);
			leftDistance = num3;
			if (!flag)
			{
				SetCircleUI(num3);
			}
			if (num2 < prestageDistance + stageRange * 2f && num2 > prestageDistance - stageRange * 2f)
			{
				SetMat(LMats[0], on: true);
				if (Mathf.Abs(num3) < stageRange && !LeftCarStaged)
				{
					SetMat(LMats[1], on: true);
					LeftCarStaged = true;
					onCarStage(car);
				}
			}
			else if (!flag)
			{
				CancelPreStageLeftCar();
				if (LeftCarStaged)
				{
					if (!(num3 > 0f))
					{
						V.CoreGameManager.SetMessage(new LocalizedString("DragRacingMultiplayerTree.MoveBackward"), 3f);
						MoveBackCar(car.Transform);
					}
					CancelStageLeftCar();
					vehicleData.racingState = RacingState.rollingToStart;
					LeftCarStaged = false;
				}
			}
		}
		if (AfterStage && startTimeLeft == 0f && LeftCarStaged)
		{
			float num4 = Vector3.Distance(vector, finish.transform.position);
			if (Mathf.Abs(DistanceToFinishOnStartLeft - num4) > 0.02f)
			{
				startTimeLeft = Time.time;
				car.Dragy.StartTime = Time.time;
				if (launchTime == 0f)
				{
					SetMat(LMats[6], on: true);
					LeftCarFalseStart = true;
				}
				else if (reactionTimeLeft == 0f)
				{
					reactionTimeLeft = Time.time - launchTime;
					dragy.SetReactionTime(reactionTimeLeft);
				}
			}
		}
		if (LeftCarFalseStart && reactionTimeLeft == 0f && launchTime != 0f)
		{
			reactionTimeLeft = 0f - launchTime + startTimeLeft;
			if (reactionTimeLeft == 0f)
			{
				Debug.LogError("reactionTimeLeft  time is 0," + launchTime + " " + startTimeLeft);
			}
			dragy.SetReactionTime(reactionTimeLeft);
		}
		_ = 2;
	}

	private void CheckRightApproachingCar(CarComponents car)
	{
		Axles axles = car.Axles;
		VehicleData vehicleData = car.VehicleData;
		if (!vehicleData.CarBodyReferences || !vehicleData.CarBodyReferences.Body)
		{
			return;
		}
		Vector3 position = vehicleData.CarBodyReferences.Body.transform.position;
		Vector3 b = car2InsideCheck.ClosestPoint(position);
		float num = Vector3.Distance(position, b);
		bool flag = V.Car != car.GameObject;
		if (num > 1f)
		{
			if (!flag)
			{
				SetCircleUI(10f);
			}
			if (!RightCarFalseStart || reactionTimeRight != 0f)
			{
				return;
			}
		}
		Dragy dragy = car.Dragy;
		int racingState = (int)vehicleData.racingState;
		Vector3 vector = 0.5f * (axles.FrontAxle.LeftWheel.ModelTransform.position + axles.FrontAxle.RightWheel.ModelTransform.position);
		Vector3 a = StartCollider.ClosestPoint(vector);
		Vector3 to = finish.TransformDirection(Vector3.forward);
		if (Vector3.Angle(car.Transform.forward, to) < 120f)
		{
			return;
		}
		float num2 = (rightDistance = Vector3.Distance(a, vector));
		if (vehicleData.racingState == RacingState.staged)
		{
			DistanceToFinishOnStarRight = Vector3.Distance(vector, finish.transform.position);
			if (flag)
			{
				controller.dragRacingUI.setCircleStage(RTimeOnStage);
			}
			RTimeOnStage = Time.deltaTime + RTimeOnStage;
		}
		else
		{
			RTimeOnStage = 0f;
		}
		if (!AfterStage)
		{
			if (!RightCarStaged)
			{
				if (num2 < 5f + prestageDistance && flag)
				{
					car.CarAIController.TargetSpeed = 5f + (float)UnityEngine.Random.Range(-1, 2);
				}
				if (num2 < 2f + prestageDistance)
				{
					Rigidbody rigidbody = car.Rigidbody;
					if (rigidbody.velocity.magnitude > 2f)
					{
						rigidbody.velocity = Vector3.ClampMagnitude(rigidbody.velocity, 2f);
						if (!flag)
						{
							_carInputManager.TargetSpeed = 2f;
						}
					}
				}
			}
			float num3 = num2 - prestageDistance;
			RightCarDistance?.Invoke(num3);
			if (!flag)
			{
				SetCircleUI(num3);
			}
			if (num2 == 0f && !AfterStage)
			{
				controller.ResetCarPositions();
				return;
			}
			if (num2 < prestageDistance + stageRange * 2f && num2 > prestageDistance - stageRange * 2f)
			{
				SetMat(RMats[0], on: true);
				if (Mathf.Abs(num3) < stageRange && !RightCarStaged)
				{
					SetMat(RMats[1], on: true);
					RightCarStaged = true;
					onCarStage(car);
				}
			}
			else if (!flag)
			{
				CancelPreStageRightCar();
				if (RightCarStaged)
				{
					if (num3 > 0f)
					{
						V.CoreGameManager.SetMessage(new LocalizedString("DragRacingMultiplayerTree.MoveForward"), 3f);
					}
					else
					{
						V.CoreGameManager.SetMessage(new LocalizedString("DragRacingMultiplayerTree.MoveBackward"), 3f);
						MoveBackCar(car.Transform);
					}
					CancelStageRightCar();
					RightCarStaged = false;
					vehicleData.racingState = RacingState.rollingToStart;
				}
			}
		}
		if (AfterStage && RightCarStaged && startTimeRight == 0f)
		{
			float num4 = Vector3.Distance(vector, finish.transform.position);
			if (Mathf.Abs(DistanceToFinishOnStarRight - num4) > 0.02f)
			{
				startTimeRight = Time.time;
				car.Dragy.StartTime = Time.time;
				if (launchTime == 0f)
				{
					SetMat(RMats[6], on: true);
					RightCarFalseStart = true;
				}
				else if (reactionTimeRight == 0f)
				{
					reactionTimeRight = Time.time - launchTime;
					dragy.SetReactionTime(reactionTimeRight);
				}
			}
		}
		if (RightCarFalseStart && reactionTimeRight == 0f && launchTime != 0f)
		{
			reactionTimeRight = 0f - launchTime + startTimeRight;
			if (reactionTimeRight == 0f)
			{
				Debug.LogError("reactionTimeRight  time is 0," + launchTime + " " + startTimeRight);
			}
			dragy.SetReactionTime(reactionTimeRight);
		}
		_ = 2;
	}

	public void MoveBackCar(Transform car)
	{
		car.transform.position += StartCollider.transform.forward * 10f;
	}

	public void StopCar(GameObject carGameObject)
	{
		CarComponents carComponents = new CarComponents(carGameObject);
		MonoBehaviour.print("stop car");
		Powertrain powertrain = carComponents.Powertrain;
		_carInputManager.PressThrottle(0f);
		if (carComponents.GameObject == controller.ourCar)
		{
			_carInputManager.PressThrottle(0f);
			_carInputManager.OnStopCarOnStaged = true;
		}
		CarAIController carAIController = carComponents.CarAIController;
		if ((bool)carAIController)
		{
			carAIController.onStopCarOnStaged = true;
			carAIController.TargetSpeed = 0f;
		}
		powertrain.ResetTransmission();
	}

	private void SetCircleUI(float distance)
	{
		if (GreenLightWillOn)
		{
			controller.dragRacingUI.HideCircle();
		}
		else
		{
			controller.dragRacingUI.setCircle(distance, stageRange);
		}
	}

	private void SetCircleToStageUI(float distance)
	{
		if (GreenLightWillOn)
		{
			controller.dragRacingUI.HideCircle();
		}
		else
		{
			controller.dragRacingUI.setCircle(distance, stageRange);
		}
	}

	private void onCarStage(CarComponents car)
	{
		VehicleData vehicleData = car.VehicleData;
		Dragy dragy = car.Dragy;
		bool flag = car.GameObject != V.Car;
		GameObject opponentCar = getOpponentCar(car.GameObject);
		dragy.Reset();
		dragy.MeasureSplits = true;
		vehicleData.racingState = RacingState.staged;
		if (flag)
		{
			if (opponentCar == V.Car)
			{
				controller.dragRacingUI.OnOtherCarStaged();
			}
		}
		else
		{
			controller.dragRacingUI.onOwnCarStaged();
			_carInputManager.TargetSpeed = 1f;
			_carInputManager.State = 2;
			ownRigidbody = car.Rigidbody;
			ownRigidbody.isKinematic = true;
			_resetOwnRigidBodyDisposable?.Dispose();
			_resetOwnRigidBodyDisposable = Observable.Timer(TimeSpan.FromSeconds(2.0)).Subscribe((long _) =>
			{
				ResetOwnRigidBody();
			}).AddTo(this);
		}
		StopCar(car.GameObject);
	}

	private void ResetOwnRigidBody()
	{
		ownRigidbody.isKinematic = false;
	}

	public PositionOnTree GetPhysicalSide(GameObject car)
	{
		if (!carLeftInsideOwnPath || !carRightInsideOwnPath)
		{
			return PositionOnTree.None;
		}
		Vector3 position = car.transform.position;
		float num = Vector3.Distance(LeftSideStartPoint.position, position);
		float num2 = Vector3.Distance(RightSideStartPoint.position, position);
		if (!(num <= num2))
		{
			return PositionOnTree.right;
		}
		return PositionOnTree.left;
	}

	public void SetCarToSide(GameObject car, PositionOnTree side)
	{
		leftBlinkCount = 0;
		rightBlinkCount = 0;
		if (side == PositionOnTree.left)
		{
			CarLeft = car;
			_carLeftComponents = new CarComponents(CarLeft);
		}
		else
		{
			CarRight = car;
			_carRightComponents = new CarComponents(CarRight);
		}
	}

	public async UniTask BlinkLeftSideMats(CancellationToken ct = default(CancellationToken))
	{
		_ = 1;
		try
		{
			if (ct == default(CancellationToken))
			{
				ct = this.GetCancellationTokenOnDestroy();
			}
			if (leftBlinkCount == 0)
			{
				leftBlinkCount = 20;
			}
			OneRacingTreeLight[] lMats = LMats;
			OneRacingTreeLight[] lMats2;
			foreach (OneRacingTreeLight oneLight in lMats)
			{
				lMats2 = LMats;
				foreach (OneRacingTreeLight oneLight2 in lMats2)
				{
					SetMat(oneLight2, on: true);
				}
				SetMat(oneLight, on: false);
				await UniTask.Delay(TimeSpan.FromSeconds(0.10000000149011612), ignoreTimeScale: false, PlayerLoopTiming.Update, ct);
				if (leftBlinkCount == 0)
				{
					return;
				}
			}
			lMats2 = LMats;
			foreach (OneRacingTreeLight oneLight3 in lMats2)
			{
				SetMat(oneLight3, on: false);
			}
			await UniTask.Delay(TimeSpan.FromSeconds(0.20000000298023224), ignoreTimeScale: false, PlayerLoopTiming.Update, ct);
			leftBlinkCount--;
			if (leftBlinkCount > 1)
			{
				BlinkLeftSideMats(ct).Forget();
			}
		}
		catch (OperationCanceledException)
		{
		}
	}

	public async UniTask BlinkRightSideMats(CancellationToken ct = default(CancellationToken))
	{
		_ = 1;
		try
		{
			if (ct == default(CancellationToken))
			{
				ct = this.GetCancellationTokenOnDestroy();
			}
			if (rightBlinkCount == 0)
			{
				rightBlinkCount = 20;
			}
			OneRacingTreeLight[] rMats = RMats;
			OneRacingTreeLight[] rMats2;
			foreach (OneRacingTreeLight oneLight in rMats)
			{
				rMats2 = RMats;
				foreach (OneRacingTreeLight oneLight2 in rMats2)
				{
					SetMat(oneLight2, on: true);
				}
				SetMat(oneLight, on: false);
				await UniTask.Delay(TimeSpan.FromSeconds(0.10000000149011612), ignoreTimeScale: false, PlayerLoopTiming.Update, ct);
				if (rightBlinkCount == 0)
				{
					return;
				}
			}
			rMats2 = RMats;
			foreach (OneRacingTreeLight oneLight3 in rMats2)
			{
				SetMat(oneLight3, on: false);
			}
			await UniTask.Delay(TimeSpan.FromSeconds(0.20000000298023224), ignoreTimeScale: false, PlayerLoopTiming.Update, ct);
			rightBlinkCount--;
			if (rightBlinkCount > 1)
			{
				BlinkRightSideMats(ct).Forget();
			}
		}
		catch (OperationCanceledException)
		{
		}
	}

	public void OnCarFinished(CarComponents car)
	{
		controller.SoundEventChannel.InvokeOnFinishSound();
		Dragy dragy = car.Dragy;
		dragy.MeasureSplits = false;
		if (controller.runDistance == DragRacingController.RunDistance.QuaterMile)
		{
			if (car.GameObject == CarLeft)
			{
				LeftCarTime = dragy.on402Reached();
			}
			else if (car.GameObject == CarRight)
			{
				RightCarTime = dragy.on402Reached();
			}
		}
		else if (car.GameObject == CarLeft)
		{
			LeftCarTime = dragy.on800mReached();
		}
		else if (car.GameObject == CarRight)
		{
			RightCarTime = dragy.on800mReached();
		}
		if (LeftCarTime != 0f && RightCarTime != 0f)
		{
			PositionOnTree winnerSide = getWinnerSide();
			MonoBehaviour.print(winnerSide);
			switch (winnerSide)
			{
			case PositionOnTree.left:
				leftBlinkCount = 20;
				BlinkLeftSideMats(this.GetCancellationTokenOnDestroy()).Forget();
				break;
			case PositionOnTree.right:
				rightBlinkCount = 20;
				BlinkRightSideMats(this.GetCancellationTokenOnDestroy()).Forget();
				break;
			}
			ResetTree();
		}
		if (controller.testRun)
		{
			ResetTree();
		}
	}

	private PositionOnTree getWinnerSide()
	{
		if (CarLeft == null)
		{
			return PositionOnTree.right;
		}
		if (CarRight == null)
		{
			return PositionOnTree.left;
		}
		if (LeftCarTime == 0f)
		{
			return PositionOnTree.right;
		}
		if (RightCarTime == 0f)
		{
			return PositionOnTree.left;
		}
		if (reactionTimeLeft < 0f || reactionTimeRight < 0f)
		{
			if (reactionTimeLeft < 0f && reactionTimeRight < 0f)
			{
				return PositionOnTree.None;
			}
			if (reactionTimeLeft < 0f)
			{
				return PositionOnTree.right;
			}
			return PositionOnTree.left;
		}
		if (LeftCarTime + reactionTimeLeft < RightCarTime + reactionTimeRight)
		{
			return PositionOnTree.left;
		}
		return PositionOnTree.right;
	}

	public void SetBannerLeft(CarComponents car)
	{
		MonoBehaviour.print("SetBannerLeft");
		Dragy dragy = car.Dragy;
		float reactionTime = dragy.reactionTime;
		float quarterMileResult = dragy.quarterMileResult;
		float num = 0f;
		if (reactionTime > 0f)
		{
			num = reactionTime + quarterMileResult;
		}
		if ((bool)leftCarBanner)
		{
			leftCarBanner.MessageText = num.ToString("F2") + "s";
		}
	}

	public void SetBannerRight(CarComponents car)
	{
		Dragy dragy = car.Dragy;
		float reactionTime = dragy.reactionTime;
		float quarterMileResult = dragy.quarterMileResult;
		float num = 0f;
		if (reactionTime > 0f)
		{
			num = reactionTime + quarterMileResult;
		}
		if ((bool)rightCarBanner)
		{
			rightCarBanner.MessageText = num.ToString("F2") + "s";
		}
	}

	public void ClearRacingPath()
	{
		foreach (GameObject allCar in controller.allCars)
		{
			if ((bool)allCar && allCar != V.LastUsedCar && Vector3.Distance(carLeftInsideOwnPath.ClosestPoint(allCar.transform.position), allCar.transform.position) < 1f)
			{
				controller.DestroyRacingCar(allCar);
				MonoBehaviour.print("car destroyed: " + allCar);
			}
		}
	}
}

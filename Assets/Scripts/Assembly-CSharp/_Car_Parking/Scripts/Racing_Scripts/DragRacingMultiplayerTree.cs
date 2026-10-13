using System;
using System.Collections.Generic;
using System.Threading;
using CPMEngine;
using Cysharp.Threading.Tasks;
using ElectronicRoadSign;
using Photon.Pun;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Providers;

namespace _Car_Parking.Scripts.Racing_Scripts
{
	public class DragRacingMultiplayerTree : MonoBehaviour
	{
		[Serializable]
		public class OneRacingTreeLight
		{
			public Material mat;

			public Color enabledC = Color.white;

			public Color disabledC = Color.gray;

			public GameObject lightObject;
		}

		[Serializable]
		public class PropsToEnableOnRacing
		{
			public int indexRunDistance;

			public List<GameObject> props;
		}

		[Serializable]
		public class RaceEntry
		{
			public DragRacingController.RunDistance Type;

			public Transform LeftCarPos;

			public Transform RightCarPos;

			public BoxCollider StartCollider;

			public Collider Car1InsideCheck;

			public Collider Car2InsideCheck;

			public bool UseOwnPathCheck;

			public Collider CarLeftInsideOwnPath;

			public Collider CarRightInsideOwnPath;

			public List<RacingCheckPoint> Checkpoints;

			public int LoopOverAll;

			public BoxCollider Finish;

			private bool UsesOwnPath
			{
				get
				{
					if (Type != DragRacingController.RunDistance.QuaterMile && Type != DragRacingController.RunDistance.OneMile)
					{
						return Type == DragRacingController.RunDistance.HalfMile;
					}
					return true;
				}
			}

			private bool UsesCheckpoints
			{
				get
				{
					if (Type != DragRacingController.RunDistance.Rally)
					{
						return Type == DragRacingController.RunDistance.Lap;
					}
					return true;
				}
			}

			private bool ShowOwnPathColliders
			{
				get
				{
					if (UsesOwnPath)
					{
						return UseOwnPathCheck;
					}
					return false;
				}
			}

			private bool IsPursuit => Type == DragRacingController.RunDistance.Pursuit;

			private bool IsLap => Type == DragRacingController.RunDistance.Lap;
		}

		public enum PositionOnTree
		{
			left = 0,
			right = 1,
			None = 2
		}

		private readonly MaterialAssetStateGuard _materialGuard = new MaterialAssetStateGuard();

		public V.placeToSpawn PlaceToSpawn;

		public DragRacingController.RunDistance[] RacingRunDistances;

		public List<RaceEntry> Races;

		public List<PropsToEnableOnRacing> propsToEnable;

		public DragRacingMultiplayerTree otherDistanceTree;

		public OneRacingTreeLight[] LMats;

		public OneRacingTreeLight[] RMats;

		public OneRacingTreeLight MiddleLight;

		public GameObject ownCar;

		public GameObject otherCar;

		public GameObject[] carWaitingNewRace;

		public BoxCollider StartCollider;

		public int intensity;

		public bool CarStaged;

		public bool otherCarStaged;

		public bool FalseStart;

		public bool GreenLightWillOn;

		public float DistanceToFinishOnStart;

		public Transform finish;

		public float reactionTime;

		public float startTime;

		public float launchTime;

		public float TimeOnStage;

		public bool AfterStage;

		public float prestageDistance = 0.1f;

		public float stageRange = 0.1f;

		private int leftBlinkCount;

		private int rightBlinkCount;

		public Collider car1InsideCheck;

		public Collider car2InsideCheck;

		public Collider carLeftInsideOwnPath;

		public Collider carRightInsideOwnPath;

		public Collider finishCollider1_4;

		public Collider finishCollider1_2;

		public float distanceCheck;

		public float distanceCarToFinish;

		public ElectronicRoadSignScript leftCarBanner;

		public ElectronicRoadSignScript rightCarBanner;

		private bool camOnFinishShowed;

		public bool outOfRoad;

		private float timeOfFixedTime;

		public NewRacingMultiplayerController controller;

		public bool Car1_4reached;

		private bool _activeSelfByDefault = true;

		private FreeDriveProvider _freeDriveProvider;

		private Axles axles;

		private VehicleData _vehicleData;

		private Dragy dragy;

		private Transform body;

		private Axles axlesOther;

		private VehicleData _vehicleDataOther;

		private Dragy dragyOther;

		private Rigidbody rb;

		private float angle;

		public JoinDragRacingTrigger LeftTrigger;

		public JoinDragRacingTrigger RightTrigger;

		public Transform leftCarPos;

		public Transform RightCarPos;

		public List<RacingCheckPoint> RacingCheckPoints;

		private AnimationDataSO.AnimationDataItem[] podiumAnimations;

		private AnimationDataSO.AnimationDataItem[] crowdAnimations;

		public Transform[] crowdPos;

		public Transform treeTransform;

		[SerializeField]
		private bool IgnoreOutOfRoadColliders;

		private CarInputManager _carInputManager;

		public RaceEntry CurrentRaceEntry { get; private set; }

		[Inject]
		public void Construct(CarInputManager carInputManager)
		{
			_carInputManager = carInputManager;
		}

		public DragRacingController.RunDistance[] GetDistinctRaceTypes()
		{
			if (Races == null || Races.Count == 0)
			{
				return RacingRunDistances;
			}
			List<DragRacingController.RunDistance> list = new List<DragRacingController.RunDistance>();
			foreach (RaceEntry race in Races)
			{
				if (!list.Contains(race.Type))
				{
					list.Add(race.Type);
				}
			}
			return list.ToArray();
		}

		public int MenuSlotForType(DragRacingController.RunDistance type)
		{
			DragRacingController.RunDistance[] distinctRaceTypes = GetDistinctRaceTypes();
			for (int i = 0; i < distinctRaceTypes.Length; i++)
			{
				if (distinctRaceTypes[i] == type)
				{
					return i;
				}
			}
			return 0;
		}

		public int ResolveRaceIndex(int menuSlot, Vector3 fromPos)
		{
			if (Races == null || Races.Count == 0)
			{
				return menuSlot;
			}
			DragRacingController.RunDistance[] distinctRaceTypes = GetDistinctRaceTypes();
			if (menuSlot < 0 || menuSlot >= distinctRaceTypes.Length)
			{
				return 0;
			}
			DragRacingController.RunDistance runDistance = distinctRaceTypes[menuSlot];
			int num = -1;
			float num2 = float.MaxValue;
			for (int i = 0; i < Races.Count; i++)
			{
				if (Races[i].Type == runDistance)
				{
					float sqrMagnitude = (GetEntryStartPosition(Races[i]) - fromPos).sqrMagnitude;
					if (sqrMagnitude < num2)
					{
						num2 = sqrMagnitude;
						num = i;
					}
				}
			}
			if (num < 0)
			{
				return 0;
			}
			return num;
		}

		private Vector3 GetEntryStartPosition(RaceEntry entry)
		{
			if (entry.StartCollider != null)
			{
				return entry.StartCollider.transform.position;
			}
			if (entry.LeftCarPos != null)
			{
				return entry.LeftCarPos.position;
			}
			return base.transform.position;
		}

		public void ApplyRaceEntry(int index)
		{
			if (Races != null && index >= 0 && index < Races.Count)
			{
				RaceEntry raceEntry = (CurrentRaceEntry = Races[index]);
				leftCarPos = raceEntry.LeftCarPos;
				RightCarPos = raceEntry.RightCarPos;
				StartCollider = raceEntry.StartCollider;
				car1InsideCheck = raceEntry.Car1InsideCheck;
				car2InsideCheck = raceEntry.Car2InsideCheck;
				if (raceEntry.UseOwnPathCheck)
				{
					carLeftInsideOwnPath = raceEntry.CarLeftInsideOwnPath;
					carRightInsideOwnPath = raceEntry.CarRightInsideOwnPath;
				}
				else
				{
					carLeftInsideOwnPath = null;
					carRightInsideOwnPath = null;
				}
				DisableAllCheckPoints();
				RacingCheckPoints = raceEntry.Checkpoints;
				if (raceEntry.Finish != null)
				{
					finishCollider1_4 = raceEntry.Finish;
					finishCollider1_2 = raceEntry.Finish;
					finish = raceEntry.Finish.transform;
				}
				else
				{
					finishCollider1_4 = null;
					finishCollider1_2 = null;
					finish = null;
				}
			}
		}

		public bool IsFinishCollider(Collider other)
		{
			if (CurrentRaceEntry != null && CurrentRaceEntry.Finish != null)
			{
				return CurrentRaceEntry.Finish == other;
			}
			return other.name == "FinishRace";
		}

		private void Start()
		{
			_freeDriveProvider = V.CoreGameManager.FreeDriveProvider;
			carWaitingNewRace = new GameObject[2];
			intensity = Shader.PropertyToID("_Intensity");
			if ((bool)LeftTrigger)
			{
				LeftTrigger.tree = this;
			}
			if ((bool)RightTrigger)
			{
				RightTrigger.tree = this;
			}
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
			if (MiddleLight != null)
			{
				_materialGuard.Capture(MiddleLight.mat);
			}
			ResetAllLights();
			ResetTree();
		}

		private void OnDestroy()
		{
			_materialGuard.Restore();
		}

		public static int CountUniqueCheckpoints(List<RacingCheckPoint> checkpoints)
		{
			if (checkpoints == null)
			{
				return 0;
			}
			HashSet<RacingCheckPoint> hashSet = new HashSet<RacingCheckPoint>();
			foreach (RacingCheckPoint checkpoint in checkpoints)
			{
				if ((bool)checkpoint)
				{
					hashSet.Add(checkpoint);
				}
			}
			return hashSet.Count;
		}

		public void DisableAllCheckPoints()
		{
			if (RacingCheckPoints == null)
			{
				return;
			}
			foreach (RacingCheckPoint racingCheckPoint in RacingCheckPoints)
			{
				if ((bool)racingCheckPoint)
				{
					racingCheckPoint.carPassed = false;
					racingCheckPoint.gameObject.SetActive(value: false);
				}
			}
		}

		public void EnableCheckPoints()
		{
			if (RacingCheckPoints == null)
			{
				return;
			}
			foreach (RacingCheckPoint racingCheckPoint in RacingCheckPoints)
			{
				if ((bool)racingCheckPoint)
				{
					racingCheckPoint.carPassed = false;
					racingCheckPoint.gameObject.SetActive(value: true);
				}
			}
		}

		public void ResetTree()
		{
			ResetAllLights();
			outOfRoad = false;
			CarStaged = false;
			FalseStart = false;
			launchTime = 0f;
			reactionTime = 0f;
			startTime = 0f;
			DistanceToFinishOnStart = 0f;
			distanceCarToFinish = 0f;
			AfterStage = false;
			GreenLightWillOn = false;
			camOnFinishShowed = false;
			Car1_4reached = false;
			Time.timeScale = 1f;
			ClearCrowd();
		}

		public void Activate()
		{
			_activeSelfByDefault = base.gameObject.activeSelf;
			base.gameObject.SetActive(value: true);
		}

		public void SetDefaultActiveSelf()
		{
			base.gameObject.SetActive(_activeSelfByDefault);
		}

		private void ResetAllLights()
		{
			OneRacingTreeLight[] lMats = LMats;
			foreach (OneRacingTreeLight oneRacingTreeLight in lMats)
			{
				SetMat(oneRacingTreeLight, on: false);
				if ((bool)oneRacingTreeLight.lightObject)
				{
					oneRacingTreeLight.lightObject.SetActive(value: false);
				}
			}
			lMats = RMats;
			foreach (OneRacingTreeLight oneRacingTreeLight2 in lMats)
			{
				SetMat(oneRacingTreeLight2, on: false);
				if ((bool)oneRacingTreeLight2.lightObject)
				{
					oneRacingTreeLight2.lightObject.SetActive(value: false);
				}
			}
			SetMiddleLight(on: false);
		}

		public void OnRaceStarted()
		{
			carWaitingNewRace[0] = null;
			carWaitingNewRace[1] = null;
			SetCarRef(V.LastUsedCar);
			EnableCheckPoints();
			SetMiddleLight(on: true);
			CreateCrowd();
		}

		private void SetMat(PositionOnTree side, int index, bool on)
		{
			OneRacingTreeLight oneRacingTreeLight = LMats[index];
			if (side == PositionOnTree.right)
			{
				oneRacingTreeLight = RMats[index];
			}
			oneRacingTreeLight.mat.SetFloat(intensity, on ? 3 : 0);
			if ((bool)oneRacingTreeLight.lightObject)
			{
				oneRacingTreeLight.lightObject.SetActive(on);
			}
		}

		private void SetMat(OneRacingTreeLight oneLight, bool on)
		{
			oneLight.mat.SetFloat(intensity, on ? 3 : 0);
			if ((bool)oneLight.lightObject)
			{
				oneLight.lightObject.SetActive(on);
			}
		}

		private void SetMiddleLight(bool on)
		{
			if (!(MiddleLight.mat == null))
			{
				MiddleLight.mat.SetFloat(intensity, on ? 3 : 0);
			}
		}

		private void cancelPreStageRightCar()
		{
			SetMat(RMats[0], on: false);
		}

		private void cancelPreStageLeftCar()
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

		public async UniTaskVoid willTurnOnGreen(bool sendForOther, CancellationToken token)
		{
			float timing = 0.1f;
			if (controller.runDistance == DragRacingController.RunDistance.OneMile || controller.runDistance == DragRacingController.RunDistance.QuaterMile || controller.runDistance == DragRacingController.RunDistance.HalfMile)
			{
				timing = 0.4f;
			}
			if (sendForOther)
			{
				float num = UnityEngine.Random.Range(1f, 3.8f);
				GreenLightWillOn = true;
				await UniTask.Delay(TimeSpan.FromSeconds(num), ignoreTimeScale: false, PlayerLoopTiming.Update, token);
				controller.OnGreenLightWillOn();
				await UniTask.Delay(TimeSpan.FromSeconds((float)PhotonNetwork.GetPing() * 0.001f), ignoreTimeScale: false, PlayerLoopTiming.Update, token);
			}
			otherCarStaged = false;
			AfterStage = true;
			beforeStart(ownCar);
			cancelPreStageLeftCar();
			cancelPreStageRightCar();
			CancelStageLeftCar();
			CancelStageRightCar();
			SetMat(LMats[2], on: true);
			SetMat(RMats[2], on: true);
			await UniTask.Delay(TimeSpan.FromSeconds(timing), ignoreTimeScale: false, PlayerLoopTiming.Update, token);
			ResetOwnRigidBody();
			SetMat(LMats[2], on: false);
			SetMat(LMats[3], on: true);
			SetMat(RMats[2], on: false);
			SetMat(RMats[3], on: true);
			await UniTask.Delay(TimeSpan.FromSeconds(timing), ignoreTimeScale: false, PlayerLoopTiming.Update, token);
			SetMat(LMats[3], on: false);
			SetMat(LMats[4], on: true);
			SetMat(RMats[3], on: false);
			SetMat(RMats[4], on: true);
			CarCanStart(token).Forget();
			await UniTask.Delay(TimeSpan.FromSeconds(timing), ignoreTimeScale: false, PlayerLoopTiming.Update, token);
			SetMat(LMats[4], on: false);
			SetMat(LMats[5], on: true);
			SetMat(RMats[4], on: false);
			SetMat(RMats[5], on: true);
			launchTime = Time.time;
			controller.recordFinishResult = true;
			FlagsStorage.RaceStarted = true;
			Vibrate(0.05f, 0.3f);
		}

		private void beforeStart(GameObject car)
		{
			if (!(car == null))
			{
				Powertrain component = car.GetComponent<Powertrain>();
				VehicleData component2 = car.GetComponent<VehicleData>();
				component.CalcMaxPowerMaxTorque(out var _, out var _, out var _, out var _, out var _);
				component2.racingState = RacingState.preLaunch;
				if (car == V.Car)
				{
					_carInputManager.State = 3;
				}
			}
		}

		private async UniTaskVoid CarCanStart(CancellationToken token)
		{
			await UniTask.Delay(TimeSpan.FromSeconds(UnityEngine.Random.Range(0.1f, 1.2f)), ignoreTimeScale: false, PlayerLoopTiming.Update, token);
			LaunchCarNow(ownCar);
		}

		private bool ShouldCheckOwnPath()
		{
			if (CurrentRaceEntry != null)
			{
				return CurrentRaceEntry.UseOwnPathCheck;
			}
			return !IgnoreOutOfRoadColliders;
		}

		private void checkIsInOwnPath(Vector3 pos)
		{
			if (Time.frameCount % 5 != 0 || !(ownCar == V.Car) || outOfRoad)
			{
				return;
			}
			float num;
			if (controller.ourPositionOnTree == PositionOnTree.left)
			{
				if (carLeftInsideOwnPath == null)
				{
					return;
				}
				num = Vector3.Distance(carLeftInsideOwnPath.ClosestPoint(pos), pos);
			}
			else
			{
				if (carRightInsideOwnPath == null)
				{
					return;
				}
				num = Vector3.Distance(carRightInsideOwnPath.ClosestPoint(pos), pos);
			}
			if (num > 2f)
			{
				dragy.reason = Dragy.LostReason.outOfRoad;
				outOfRoad = true;
			}
		}

		private void CheckFinishDistance(GameObject car, VehicleData vehicleData1, Axles axles)
		{
			DragRacingController.RunDistance runDistance = controller.runDistance;
			if (runDistance == DragRacingController.RunDistance.Pursuit || vehicleData1.racingState != RacingState.OnRacing)
			{
				return;
			}
			if (runDistance == DragRacingController.RunDistance.Lap || runDistance == DragRacingController.RunDistance.Rally)
			{
				if (Time.frameCount % 2 == 0 && startTime != 0f)
				{
					float num = Time.time - startTime + (float)controller.fineToAdd;
					_freeDriveProvider.DragRacingControlMultiplayer.TimerOverAll = num;
					controller.racingTimerLabel.text = ConvertToMinutesSecondsMilliseconds(num);
				}
			}
			else if (!(car == null))
			{
				Vector3 vector = 0.5f * (axles.FrontAxle.LeftWheel.ModelTransform.position + axles.FrontAxle.RightWheel.ModelTransform.position);
				if (ShouldCheckOwnPath())
				{
					checkIsInOwnPath(vector);
				}
				float num2 = Vector3.Distance(finishCollider1_4.ClosestPoint(vector), vector);
				float num3 = Vector3.Distance(finishCollider1_2.ClosestPoint(vector), vector);
				float num4 = 1f / Time.fixedDeltaTime;
				distanceCheck = car.GetComponent<Rigidbody>().velocity.magnitude / num4;
				if (controller.runDistance == DragRacingController.RunDistance.QuaterMile)
				{
					distanceCarToFinish = num2;
				}
				else
				{
					distanceCarToFinish = num3;
				}
				if (num2 <= distanceCheck)
				{
					On402Reached(car);
				}
				if ((controller.runDistance == DragRacingController.RunDistance.OneMile || controller.runDistance == DragRacingController.RunDistance.HalfMile) && num3 < distanceCheck)
				{
					onMileReached(car);
				}
			}
		}

		public void onMileReached(GameObject car)
		{
			dragy.on800mReached();
			_ = controller.OnCarFinish(car);
			distanceCarToFinish = 0f;
			SetBannerLeft(car);
		}

		public void On402Reached(GameObject car)
		{
			if (controller.runDistance == DragRacingController.RunDistance.QuaterMile)
			{
				dragy.on402Reached();
				_ = controller.OnCarFinish(car);
				distanceCarToFinish = 0f;
				SetBannerLeft(car);
			}
			else if (!Car1_4reached)
			{
				dragy.on402Reached();
				Car1_4reached = true;
			}
		}

		private void CheckForCloseFinish()
		{
			if (!camOnFinishShowed && distanceCarToFinish != 0f && distanceCarToFinish < 10f)
			{
				camOnFinishShowed = true;
			}
		}

		public void CheckCarsMove()
		{
			if (FlagsStorage.ChasingRaceModeStarted || !FlagsStorage.RacingGameMode || controller.CurrentRacingTree != this)
			{
				return;
			}
			if ((bool)ownCar)
			{
				CheckApproachingCar(ownCar, controller.ourPositionOnTree);
				if ((bool)otherCar)
				{
					CheckApproachingOtherCar(otherCar, controller.otherCarPositionOnTree);
				}
				CheckFinishDistance(ownCar, _vehicleData, axles);
			}
			if ((bool)ownCar)
			{
				CheckForCloseFinish();
			}
			CancellationToken cancellationTokenOnDestroy = this.GetCancellationTokenOnDestroy();
			if (controller.testRun)
			{
				if (CarStaged && !GreenLightWillOn)
				{
					willTurnOnGreen(sendForOther: true, cancellationTokenOnDestroy).Forget();
				}
			}
			else if (CarStaged && otherCarStaged && !GreenLightWillOn && controller.ourPositionOnTree == PositionOnTree.left)
			{
				willTurnOnGreen(sendForOther: true, cancellationTokenOnDestroy).Forget();
			}
		}

		private string ConvertToMinutesSecondsMilliseconds(float seconds)
		{
			int num = (int)seconds;
			int num2 = num / 60;
			int num3 = num % 60;
			int num4 = (int)((seconds - (float)num) * 100f);
			return $"{num2:D2}:{num3:D2}:{num4:D2}";
		}

		private void LaunchCarNow(GameObject car)
		{
			if ((bool)car)
			{
				car.GetComponent<VehicleData>().racingState = RacingState.OnRacing;
			}
		}

		private void CheckApproachingCar(GameObject car, PositionOnTree side)
		{
			Vector3 position = body.transform.position;
			Collider collider = car1InsideCheck;
			if (side == PositionOnTree.right)
			{
				collider = car2InsideCheck;
			}
			if (_vehicleData.racingState == RacingState.rollingToStart)
			{
				Vector3 b = collider.ClosestPoint(position);
				float num = Vector3.Distance(position, b);
				if (num > 2f)
				{
					MonoBehaviour.print($"{num} {controller.CurrentRacingTree} {collider} {body}");
					controller.outOfTriggerBeforeStage();
					return;
				}
			}
			if (_vehicleData.racingState == RacingState.Finished || _vehicleData.racingState == RacingState.None)
			{
				return;
			}
			Vector3 vector = 0.5f * (axles.FrontAxle.LeftWheel.ModelTransform.position + axles.FrontAxle.RightWheel.ModelTransform.position);
			Vector3 a = StartCollider.ClosestPoint(vector);
			Vector3 to = StartCollider.transform.TransformDirection(Vector3.forward);
			angle = Vector3.Angle(car.transform.forward, to);
			if (angle < 120f)
			{
				return;
			}
			float num2 = Vector3.Distance(a, vector);
			if (_vehicleData.racingState == RacingState.staged)
			{
				TimeOnStage = Time.deltaTime + TimeOnStage;
				DistanceToFinishOnStart = Vector3.Distance(vector, finish.transform.position);
			}
			else
			{
				TimeOnStage = 0f;
			}
			if (num2 == 0f)
			{
				_ = AfterStage;
			}
			if (!AfterStage)
			{
				if (!CarStaged && num2 < 2f + prestageDistance && !GreenLightWillOn && rb.velocity.magnitude > 2f)
				{
					rb.velocity = Vector3.ClampMagnitude(rb.velocity, 2f);
					_carInputManager.TargetSpeed = 2f;
				}
				float num3 = num2 - prestageDistance;
				SetCircleUI(num3);
				if (num2 < prestageDistance + stageRange * 2f && num2 > prestageDistance - stageRange * 2f)
				{
					SetMat(side, 0, on: true);
					if (Mathf.Abs(num3) < stageRange && !CarStaged)
					{
						SetMat(side, 1, on: true);
						CarStaged = true;
						OnCarStage(car);
					}
				}
				else
				{
					SetMat(side, 0, on: false);
					if (CarStaged)
					{
						if (!(num3 > 0f))
						{
							V.CoreGameManager.SetMessage(new LocalizedString("DragRacingMultiplayerTree.MoveBackward"), 3f);
							moveBackCar(car.transform);
						}
						SetMat(side, 1, on: false);
						_vehicleData.racingState = RacingState.rollingToStart;
						CarStaged = false;
					}
				}
			}
			if (AfterStage && startTime == 0f && CarStaged)
			{
				float num4 = Vector3.Distance(vector, finish.transform.position);
				if (Mathf.Abs(DistanceToFinishOnStart - num4) > 0.1f)
				{
					startTime = Time.time;
					car.GetComponent<Dragy>().StartTime = Time.time;
					car.GetComponent<Dragy>().onCarStop();
					if (launchTime == 0f)
					{
						FalseStart = true;
					}
					else if (reactionTime == 0f)
					{
						reactionTime = Time.time - launchTime;
						dragy.SetReactionTime(reactionTime);
					}
				}
			}
			if (FalseStart && reactionTime == 0f && launchTime != 0f)
			{
				reactionTime = 0f - launchTime + startTime;
				if (reactionTime == 0f)
				{
					Debug.LogError("reactionTimeLeft  time is 0," + launchTime + " " + startTime);
				}
				dragy.SetReactionTime(reactionTime);
			}
		}

		private void CheckApproachingOtherCar(GameObject car, PositionOnTree side)
		{
			if (car == null)
			{
				return;
			}
			Vector3 vector = ((!axlesOther) ? car.transform.position : (0.5f * (axlesOther.FrontAxle.LeftWheel.ModelTransform.position + axlesOther.FrontAxle.RightWheel.ModelTransform.position)));
			float num = Vector3.Distance(StartCollider.ClosestPoint(vector), vector);
			if (!AfterStage)
			{
				if ((num < prestageDistance + stageRange * 10f && num > prestageDistance - stageRange * 10f) || otherCarStaged)
				{
					SetMat(side, 0, on: true);
				}
				else
				{
					SetMat(side, 0, on: false);
				}
			}
		}

		public void moveBackCar(Transform car)
		{
			car.transform.position = car.transform.position + StartCollider.transform.forward * 10f;
		}

		public void StopCar(GameObject car)
		{
			MonoBehaviour.print("stop car");
			Powertrain component = car.GetComponent<Powertrain>();
			CarAIController component2 = car.GetComponent<CarAIController>();
			if ((bool)component2)
			{
				component2.onStopCarOnStaged = true;
				component2.TargetSpeed = 0f;
			}
			component.ResetTransmission();
		}

		private void SetCircleUI(float distance)
		{
			_ = GreenLightWillOn;
		}

		private void OnCarStage(GameObject car)
		{
			VehicleData component = car.GetComponent<VehicleData>();
			Dragy component2 = car.GetComponent<Dragy>();
			component2.Reset();
			component2.reason = Dragy.LostReason.none;
			component.racingState = RacingState.staged;
			_carInputManager.TargetSpeed = 1f;
			_carInputManager.State = 2;
			rb = car.GetComponent<Rigidbody>();
			rb.constraints = (RigidbodyConstraints)10;
			StopCar(car);
			controller.OnOurCarStaged();
		}

		public void onOtherCarStaged()
		{
			otherCarStaged = true;
			SetMat(controller.otherCarPositionOnTree, 0, on: true);
			SetMat(controller.otherCarPositionOnTree, 1, on: true);
		}

		private void ResetOwnRigidBody()
		{
			rb.isKinematic = false;
			rb.constraints = RigidbodyConstraints.None;
		}

		public void SetCarRef(GameObject car)
		{
			leftBlinkCount = 0;
			rightBlinkCount = 0;
			_vehicleData = car.GetComponent<VehicleData>();
			axles = car.GetComponent<Axles>();
			body = _vehicleData.Collider.transform;
			dragy = car.GetComponent<Dragy>();
			rb = car.GetComponent<Rigidbody>();
			_vehicleDataOther = otherCar.GetComponent<VehicleData>();
			axlesOther = otherCar.GetComponent<Axles>();
			_vehicleData.racingState = RacingState.rollingToStart;
			StopCar(car);
			if (controller.runDistance == DragRacingController.RunDistance.Lap || controller.runDistance == DragRacingController.RunDistance.Rally)
			{
				dragy.CheckMaxSpeed = true;
			}
			_carInputManager.State = 3;
		}

		private async UniTask BlinkLeftSideMatsAsync(CancellationToken ct = default(CancellationToken))
		{
			_ = 1;
			try
			{
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
					BlinkLeftSideMatsAsync(ct).Forget();
				}
			}
			catch (OperationCanceledException)
			{
			}
		}

		private async UniTask BlinkRightSideMatsAsync(CancellationToken ct = default(CancellationToken))
		{
			_ = 1;
			try
			{
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
					BlinkRightSideMatsAsync(ct).Forget();
				}
			}
			catch (OperationCanceledException)
			{
			}
		}

		public void SetBannerLeft(GameObject car)
		{
			MonoBehaviour.print("SetBannerLeft");
			Dragy component = car.GetComponent<Dragy>();
			float num = component.reactionTime;
			float quarterMileResult = component.quarterMileResult;
			float num2 = 0f;
			if (num > 0f)
			{
				num2 = num + quarterMileResult;
			}
			if ((bool)leftCarBanner)
			{
				leftCarBanner.MessageText = num2.ToString("F2") + "s";
			}
		}

		public void SetBannerRight(GameObject car)
		{
			Dragy component = car.GetComponent<Dragy>();
			float num = component.reactionTime;
			float quarterMileResult = component.quarterMileResult;
			float num2 = 0f;
			if (num > 0f)
			{
				num2 = num + quarterMileResult;
			}
			if ((bool)rightCarBanner)
			{
				rightCarBanner.MessageText = num2.ToString("F2") + "s";
			}
		}

		public void CreateCrowd()
		{
			if (V.CoreGameManager.gameObject.GetComponent<FPSDisplay2>().averageFps < 30f)
			{
				return;
			}
			Transform[] array = crowdPos;
			foreach (Transform square in array)
			{
				Vector3 toLook = (leftCarPos.transform.position + RightCarPos.transform.position) * 0.5f;
				if ((bool)treeTransform)
				{
					toLook = treeTransform.transform.position;
				}
				_ = controller.Podium.PopulateCrowd(square, toLook, myPersonLose: false);
			}
		}

		public void ClearCrowd()
		{
			controller.Podium.ClearCrowd();
		}

		public void Vibrate(float duration, float intensity)
		{
			Utils.Vibrate(duration, intensity);
		}
	}
}

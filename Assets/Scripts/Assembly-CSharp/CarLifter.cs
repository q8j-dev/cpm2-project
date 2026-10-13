using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;
using _Car_Parking;
using _Car_Parking.Scripts.Audio;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Other.Helpers;
using _Car_Parking.Scripts.Scopes;

public class CarLifter : MonoBehaviour
{
	public Vector3 UpPos;

	public Vector3 DownPos;

	public Vector3 target;

	public float speed = 1f;

	public bool Lifting;

	public Transform raycastPos;

	public bool CarUnder;

	public bool CarStuckAbove;

	public bool useLocalPos;

	public Vector3 initialPos;

	[SerializeField]
	private float isCarAwayCheckDelay = 1.5f;

	[SerializeField]
	private float distanceAway = 22.5f;

	[SerializeField]
	private float CarUnderCheckTime = 0.5f;

	private Vector3 _raycastPosOffset = new Vector3(0f, 0.2f, 0f);

	private Vector3 _stuckCheckPosOffset = new Vector3(0f, 1f, 0f);

	private bool _waitingUntilMovesAway;

	private bool _canMove;

	private bool _isSoundPlaying;

	private LevelScript _levelScript;

	private bool _isDestroyed;

	private ManagerUI _managerUi;

	private CancellationTokenSource _lifetimeCts;

	private CancellationTokenSource _carUnderCts;

	public static CarLifter ClosestLifter;

	private static readonly List<CarLifter> CurrLifters = new List<CarLifter>();

	private static readonly Vector3 CarCheckBoxSize = new Vector3(2f, 0.3f, 0.7f);

	private const string PlaySfxHydraulicPress = "Play_SFX_Hydraulic_Press";

	private const string StopSfxHydraulicPress = "Stop_SFX_Hydraulic_Press";

	private const float CheckDistanceFirstDelay = 1f;

	private const float CheckDistanceRepeatInterval = 10f;

	[Inject]
	public void Construct(ManagerUI managerUI, OptionalDependency<LevelScript> level)
	{
		_levelScript = level.Value;
		_managerUi = managerUI;
	}

	private void Awake()
	{
		_isDestroyed = false;
		if (ScenesHelper.IsParkingLevelActive() || !FlagsStorage.IsFreeDriveMode)
		{
			ClosestLifter = this;
		}
		target = (useLocalPos ? base.transform.localPosition : base.transform.position);
		if (useLocalPos)
		{
			initialPos = base.transform.localPosition;
		}
		CurrLifters.Add(this);
		_lifetimeCts = new CancellationTokenSource();
	}

	private void Start()
	{
		if (FlagsStorage.IsLevelScene || !FlagsStorage.IsFreeDriveMode)
		{
			ClosestLifter = this;
		}
		if (FlagsStorage.IsFreeDriveMode)
		{
			CheckDistanceLoop(_lifetimeCts.Token).Forget();
		}
		if ((bool)V.SceneControl)
		{
			V.SceneControl.carLifters.Add(this);
		}
	}

	private void OnEnable()
	{
		StartCheckCarUnder();
	}

	private void OnDisable()
	{
		StopCarUnderCheck();
	}

	private async UniTaskVoid CheckDistanceLoop(CancellationToken ct)
	{
		try
		{
			await UniTask.Delay(TimeSpan.FromSeconds(1.0), ignoreTimeScale: false, PlayerLoopTiming.Update, ct);
		}
		catch (OperationCanceledException)
		{
			return;
		}
		while (!ct.IsCancellationRequested)
		{
			CheckDistance();
			try
			{
				await UniTask.Delay(TimeSpan.FromSeconds(10.0), ignoreTimeScale: false, PlayerLoopTiming.Update, ct);
			}
			catch (OperationCanceledException)
			{
				break;
			}
		}
	}

	private void CheckDistance()
	{
		if ((bool)V.CoreGameManager)
		{
			float num = Vector3.Distance(V.CoreGameManager.transform.position, base.transform.position);
			base.enabled = num < 100f;
		}
	}

	private void SetDownPos()
	{
		Lifting = false;
		if (useLocalPos)
		{
			target = initialPos + new Vector3(0f, -2f, 0f);
			return;
		}
		target = DownPos;
		_managerUi.ShowActionButton(new LocalizedString("CarLifter.Lift")).Forget();
		StartCheckCarUnder();
	}

	private void StartCheckCarUnder()
	{
		StopCarUnderCheck();
		if (_lifetimeCts != null && !_lifetimeCts.IsCancellationRequested)
		{
			_carUnderCts = CancellationTokenSource.CreateLinkedTokenSource(_lifetimeCts.Token);
			CarUnderCheckLoop(_carUnderCts.Token).Forget();
		}
	}

	private void StopCarUnderCheck()
	{
		if (_carUnderCts != null)
		{
			_carUnderCts.Cancel();
			_carUnderCts.Dispose();
			_carUnderCts = null;
		}
	}

	private async UniTaskVoid CarUnderCheckLoop(CancellationToken ct)
	{
		while (!ct.IsCancellationRequested)
		{
			try
			{
				await UniTask.Delay(TimeSpan.FromSeconds(CarUnderCheckTime), ignoreTimeScale: false, PlayerLoopTiming.Update, ct);
			}
			catch (OperationCanceledException)
			{
				break;
			}
			CheckForCarUnder();
		}
	}

	private void SetUpPos()
	{
		Lifting = true;
		if (useLocalPos)
		{
			target = initialPos;
			return;
		}
		target = UpPos;
		_managerUi.ShowActionButton(new LocalizedString("CarLifter.Down")).Forget();
		if (FlagsStorage.IsLevelScene)
		{
			_levelScript.ResetTopMessage();
		}
	}

	private void CheckForCarUnder()
	{
		if (!(ClosestLifter != this))
		{
			Vector3 vector = raycastPos.position - _raycastPosOffset;
			Vector3 center = vector + _stuckCheckPosOffset;
			bool flag = Physics.CheckBox(vector, CarCheckBoxSize, raycastPos.rotation, int.MinValue, QueryTriggerInteraction.Ignore);
			CarStuckAbove = Physics.CheckBox(center, CarCheckBoxSize, raycastPos.rotation, int.MinValue, QueryTriggerInteraction.Ignore);
			CarUnder = flag && !CarStuckAbove;
		}
	}

	public void ChangeMoveDirection()
	{
		_canMove = true;
		if (Lifting)
		{
			SetDownPos();
		}
		else
		{
			SetUpPos();
		}
	}

	private void Update()
	{
		float num = 0f;
		if (_canMove)
		{
			if (!Lifting && CarUnder)
			{
				if (_isSoundPlaying)
				{
					StopHydraulicPressSound();
					if (FlagsStorage.IsFreeDriveMode)
					{
						V.CoreGameManager.SetMessage(new LocalizedString("CarLifter.Removethecar"), 3f);
					}
					else
					{
						_levelScript.ShowMessage("CarLifter.Removethecar", isImportant: true);
					}
				}
				return;
			}
			float maxDistanceDelta = speed * Time.deltaTime;
			if (useLocalPos)
			{
				num = Vector3.Distance(base.transform.localPosition, target);
				if (num > 0.01f)
				{
					base.transform.localPosition = Vector3.MoveTowards(base.transform.localPosition, target, maxDistanceDelta);
				}
			}
			else
			{
				num = Vector3.Distance(base.transform.position, target + V.GlobalOffset);
				if (num > 0.01f)
				{
					base.transform.position = Vector3.MoveTowards(base.transform.position, target + V.GlobalOffset, maxDistanceDelta);
				}
			}
		}
		if (num > 0.1f)
		{
			if (!_isSoundPlaying)
			{
				PlayHydraulicPressSound();
			}
		}
		else if (_isSoundPlaying)
		{
			StopHydraulicPressSound();
		}
	}

	public static CarLifter GetClosestLifter(Transform tr)
	{
		CarLifter carLifter = null;
		float num = float.MaxValue;
		foreach (CarLifter currLifter in CurrLifters)
		{
			float num2 = Vector3.Distance(tr.position, currLifter.transform.position);
			if (num2 < num)
			{
				num = num2;
				carLifter = currLifter;
			}
		}
		if ((bool)carLifter)
		{
			ClosestLifter = carLifter;
		}
		return carLifter;
	}

	private void OnDestroy()
	{
		StopCarUnderCheck();
		if (_lifetimeCts != null)
		{
			_lifetimeCts.Cancel();
			_lifetimeCts.Dispose();
			_lifetimeCts = null;
		}
		_isDestroyed = true;
		StopHydraulicPressSound();
		CurrLifters.Remove(this);
		if (ClosestLifter == this)
		{
			ClosestLifter = null;
		}
		if ((bool)V.SceneControl)
		{
			V.SceneControl.carLifters.Remove(this);
		}
	}

	private void PlayHydraulicPressSound()
	{
		_isSoundPlaying = true;
		AudioWrapper.PostEvent("Play_SFX_Hydraulic_Press", base.gameObject);
	}

	private void StopHydraulicPressSound()
	{
		_canMove = false;
		_isSoundPlaying = false;
		AudioWrapper.PostEvent("Stop_SFX_Hydraulic_Press", base.gameObject);
	}

	public void StartWaitUntilCarMovesAway()
	{
		if (!_waitingUntilMovesAway)
		{
			_waitingUntilMovesAway = true;
			WaitUntilCarMovesAway().Forget();
		}
	}

	private async UniTaskVoid WaitUntilCarMovesAway()
	{
		while (!_isDestroyed)
		{
			Vector3 b = (V.Car ? V.Car.transform.position : V.DriverPerson.transform.position);
			if (Vector3.Distance(base.transform.position, b) > distanceAway)
			{
				break;
			}
			await UniTask.Delay(TimeSpan.FromSeconds(isCarAwayCheckDelay));
		}
		_managerUi.HideActionButton();
		_waitingUntilMovesAway = false;
	}
}

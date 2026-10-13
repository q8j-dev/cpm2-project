using System;
using System.Threading;
using CPM.Shifter.Contracts;
using CPM.Shifter.Logic;
using CPMEngine;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.MenuUI.Shifter;
using _Car_Parking.Scripts.SOAP;
using _Car_Parking.Scripts.SOAP_Registry;

[RequireComponent(typeof(RectTransform))]
public class ManualHShifterController : MonoBehaviour, IShifterMode
{
	[Header("Wiring")]
	[SerializeField]
	private ManualHShifterView _view;

	[SerializeField]
	private ManualHShifterInput _input;

	[SerializeField]
	private TextMeshProUGUI _gearLabel;

	private ShifterGamepadBridge _shifterGamepadBridge;

	private CarInputManager _carInputManager;

	private CancellationTokenSource _gripWheelCts;

	private const float KnobMoveDuration = 0.15f;

	private SteeringWheelValueEvent _steeringWheelValueEvent;

	private EventRegistry _eventRegistry;

	private RegistryHandle<GearVariable> _gearHandle = new RegistryHandle<GearVariable>(EventRegistryIDs.Gear);

	private const float MinHandSequenceSeconds = 0.4f;

	public bool IsTouching
	{
		get
		{
			if (_input != null)
			{
				return _input.IsTouching;
			}
			return false;
		}
	}

	[Inject]
	public void Construct(CarInputManager carInputManager, EventRegistry eventRegistry, SteeringWheelValueEvent steeringWheelValueEvent)
	{
		_carInputManager = carInputManager;
		_eventRegistry = eventRegistry;
		_steeringWheelValueEvent = steeringWheelValueEvent;
	}

	private void Awake()
	{
		if (_view == null)
		{
			_view = GetComponentInChildren<ManualHShifterView>(includeInactive: true);
		}
		if (_input == null)
		{
			_input = GetComponentInChildren<ManualHShifterInput>(includeInactive: true);
		}
	}

	private void OnEnable()
	{
		if (_carInputManager != null && _carInputManager.GetPowertrain != null)
		{
			Activate();
		}
	}

	private void OnDisable()
	{
		if (_input != null)
		{
			_input.OnGearRequested -= HandleGearRequest;
			_input.OnPreviewGearChanged -= HandlePreviewGearChanged;
			_input.OnGrabStarted -= HandleGrabStarted;
			_input.OnGrabReleased -= HandleGrabReleased;
			_input.Unbind();
		}
		CommonExtensions.CancelAndDisposeToken(ref _gripWheelCts);
		if ((bool)_shifterGamepadBridge)
		{
			_shifterGamepadBridge.UnsubscribeFromActions();
		}
	}

	public void Activate()
	{
		Powertrain powertrain = (_carInputManager ? _carInputManager.GetPowertrain : null);
		if ((bool)powertrain)
		{
			ShifterLayoutAsset shifterLayoutAsset = _carInputManager.GetVehicleEntityProvider?.CarEntity?.GearBox?.Part?.ShifterLayout;
			ShifterLayoutAsset shifterLayoutAsset2 = ShifterLayoutResolver.Resolve(shifterLayoutAsset, powertrain.NumGears, ShifterLayoutDefaults.Instance);
			Debug.Log($"[Shifter] NumGears={powertrain.NumGears} → layout='{shifterLayoutAsset2.name}' " + "(explicit=" + (shifterLayoutAsset ? shifterLayoutAsset.name : "<none>") + ", " + $"R={shifterLayoutAsset2.ReversePos}, dogleg={shifterLayoutAsset2.DoglegFirstGear})");
			_view.ApplyLayout(shifterLayoutAsset2);
			_input.Bind(_view, () => _carInputManager.ClutchInput);
			_input.OnGearRequested -= HandleGearRequest;
			_input.OnGearRequested += HandleGearRequest;
			_input.OnPreviewGearChanged -= HandlePreviewGearChanged;
			_input.OnPreviewGearChanged += HandlePreviewGearChanged;
			_input.OnGrabStarted -= HandleGrabStarted;
			_input.OnGrabStarted += HandleGrabStarted;
			_input.OnGrabReleased -= HandleGrabReleased;
			_input.OnGrabReleased += HandleGrabReleased;
			int gear = powertrain.Gear;
			if (_steeringWheelValueEvent.StoredValue)
			{
				GearEnum storedValue = _gearHandle.Get(_eventRegistry).StoredValue;
				gear = (powertrain.Gear = GetGear(storedValue));
			}
			if (!_shifterGamepadBridge)
			{
				_shifterGamepadBridge = GetComponentInParent<ShifterGamepadBridge>();
			}
			if ((bool)_shifterGamepadBridge)
			{
				_shifterGamepadBridge.SubscribeToActions(isManual: true);
			}
			_view.MoveKnobTo(_view.GetSlotAnchoredPos(gear), 0f);
			SetGearLabelText(GearLabelFor(gear));
			_view.SetGearTraceAlpha(0.3f);
		}
	}

	private int GetGear(GearEnum gear)
	{
		return gear switch
		{
			GearEnum.R => -1, 
			GearEnum.N => 0, 
			GearEnum.G1 => 1, 
			GearEnum.G2 => 2, 
			GearEnum.G3 => 3, 
			GearEnum.G4 => 4, 
			GearEnum.G5 => 5, 
			GearEnum.G6 => 6, 
			GearEnum.G7 => 7, 
			_ => 0, 
		};
	}

	public void Deactivate()
	{
		if ((bool)_input)
		{
			_input.OnGearRequested -= HandleGearRequest;
			_input.OnPreviewGearChanged -= HandlePreviewGearChanged;
			_input.OnGrabStarted -= HandleGrabStarted;
			_input.OnGrabReleased -= HandleGrabReleased;
		}
	}

	public void SyncToCurrentGear()
	{
		if (base.isActiveAndEnabled && (!_input || !_input.IsTouching) && (bool)_carInputManager?.GetPowertrain && (bool)_view)
		{
			int gear = _carInputManager.GetPowertrain.Gear;
			_view.MoveKnobTo(_view.GetSlotAnchoredPos(gear), 0.15f);
			_view.RevealAndHideWithin(0.15f);
			SetGearLabelText(GearLabelFor(gear));
		}
	}

	public void ResetToNeutral()
	{
		if ((bool)_view)
		{
			_view.MoveKnobTo(_view.GetSlotAnchoredPos(0), 0.15f);
			SetGearLabelText(GearLabelFor(0));
		}
	}

	public void SetPosByGear(int gear)
	{
		if ((bool)_view)
		{
			_view.MoveKnobTo(_view.GetSlotAnchoredPos(gear), 0.15f);
		}
	}

	public void SetGearTraceAlpha(float a)
	{
		if ((bool)_view)
		{
			_view.SetGearTraceAlpha(a);
		}
	}

	public void SetGearLabelText(string text)
	{
		if ((bool)_gearLabel)
		{
			_gearLabel.text = text;
		}
	}

	public void PlayDriverShiftAnimation()
	{
		CommonExtensions.CancelAndDisposeToken(ref _gripWheelCts);
		DriverHandHelpers.Shiftgear();
		_gripWheelCts = new CancellationTokenSource();
		DelayedGripWheel(_gripWheelCts.Token).Forget();
	}

	private void HandleGrabStarted()
	{
		_view?.RevealTrace();
		_view?.ScaleKnobGrabbed();
		CommonExtensions.CancelAndDisposeToken(ref _gripWheelCts);
		DriverHandHelpers.Shiftgear();
	}

	private void HandleGrabReleased()
	{
		_view?.HideTraceWithDelay(0f, 0.15f);
		_view?.ScaleKnobReleased();
		ShifterController.FireReleaseGearEvent();
		CommonExtensions.CancelAndDisposeToken(ref _gripWheelCts);
		_gripWheelCts = new CancellationTokenSource();
		DelayedGripWheel(_gripWheelCts.Token).Forget();
	}

	private async UniTaskVoid DelayedGripWheel(CancellationToken ct)
	{
		try
		{
			await UniTask.Delay(TimeSpan.FromSeconds(0.4000000059604645), ignoreTimeScale: false, PlayerLoopTiming.Update, ct);
		}
		catch (OperationCanceledException)
		{
			return;
		}
		DriverHandHelpers.GripWheel();
	}

	private void HandlePreviewGearChanged(int gear)
	{
		SetGearLabelText(GearLabelFor(gear));
	}

	private void HandleGearRequest(int gear)
	{
		if ((bool)V.Car)
		{
			try
			{
				float intensity = 0.1f;
				Utils.Vibrate(0.1f, intensity);
			}
			catch (Exception ex)
			{
				Debug.LogError("Shifter vibration error: " + ex.Message);
			}
			SoundEventsDataHelper.PlayUISound(SoundEventsDataHelper.GearShiftSwitch, base.gameObject);
			_carInputManager.GetPowertrain.Gear = gear;
			_carInputManager.SelectorState = gear;
			SetGearLabelText(GearLabelFor(gear));
		}
	}

	public void RequestGearChange(int gear)
	{
		if ((_input != null && _input.IsTouching) || (gear != 0 && (_view == null || !_view.IsSlotActive(gear))) || !V.Car)
		{
			return;
		}
		Powertrain powertrain = _carInputManager?.GetPowertrain;
		if (!(powertrain == null))
		{
			try
			{
				float intensity = 0.1f;
				Utils.Vibrate(0.1f, intensity);
			}
			catch (Exception ex)
			{
				Debug.LogError("Shifter vibration error: " + ex.Message);
			}
			SoundEventsDataHelper.PlayUISound(SoundEventsDataHelper.GearShiftSwitch, base.gameObject);
			powertrain.Gear = gear;
			_carInputManager.SelectorState = gear;
			SetGearLabelText(GearLabelFor(gear));
			PlayDriverShiftAnimation();
		}
	}

	private static string GearLabelFor(int gear)
	{
		if (gear != -1)
		{
			if (gear == 0)
			{
				return "N";
			}
			return (gear > 0) ? gear.ToString() : string.Empty;
		}
		return "R";
	}
}

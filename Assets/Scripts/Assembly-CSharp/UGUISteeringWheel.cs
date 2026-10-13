using System.Threading;
using CPMEngine;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using VContainer;
using _Car_Parking.Scripts.EditableHUD;

public class UGUISteeringWheel : MonoBehaviour, IPointerDownHandler, IEventSystemHandler, EditableHUDElement.IHUDUpdated
{
	private const float MaxSteerAngle = 270f;

	public bool Interactable = true;

	public float centerDeadZoneRadius;

	public float wheelFreeSpeed = 1f;

	private int _touchId = -1;

	private float _maxAngle;

	private float _wheelAngle;

	private float _steeringWheelResetPosSpeed = 200f;

	private float _wheelTempAngle;

	private float _wheelNewAngle;

	private bool _isHold;

	private bool _isDestroyed;

	private Vector2 _wheelCenter;

	private Vector2 _touchPos;

	private Transform _wheelRect;

	private CancellationTokenSource _cancellationTokenSourceUpdateWheel;

	private CarInputManager _carInputManager;

	private static float CurrentMaxAngle => 270f * (0.3f + (1f - V.SWsensitivity));

	[Inject]
	public void Construct(CarInputManager carInputManager)
	{
		_carInputManager = carInputManager;
		if ((bool)_carInputManager)
		{
			_carInputManager.UGUISteeringWheel = this;
		}
	}

	private void Awake()
	{
		TryGetComponent<Transform>(out _wheelRect);
	}

	public void Init(CarInputManager carInputManager)
	{
		_carInputManager = carInputManager;
	}

	private void OnEnable()
	{
		UpdateWheelCenter();
	}

	private void OnDestroy()
	{
		_isDestroyed = true;
		CommonExtensions.CancelAndDisposeToken(ref _cancellationTokenSourceUpdateWheel);
		if (_carInputManager != null && _carInputManager.UGUISteeringWheel == this)
		{
			_carInputManager.UGUISteeringWheel = null;
		}
		_wheelRect = null;
		_carInputManager = null;
	}

	public void ResetWheel()
	{
		_wheelAngle = 0f;
	}

	public void SyncToSteerInput(float normalizedSteerInput)
	{
		_maxAngle = CurrentMaxAngle;
		_wheelAngle = SteerRemapMath.WidgetAngleFromSteerInput(normalizedSteerInput, _maxAngle);
		if ((bool)_wheelRect)
		{
			_wheelRect.eulerAngles = new Vector3(0f, 0f, 0f - _wheelAngle);
		}
	}

	private void Update()
	{
		_maxAngle = 270f * (0.3f + (1f - V.SWsensitivity));
		if (!_carInputManager || !_wheelRect || !_wheelRect)
		{
			return;
		}
		_maxAngle = 270f * (0.3f + (1f - V.SWsensitivity));
		if (_isHold)
		{
			if (Interactable)
			{
				Touch[] touches = Input.touches;
				for (int i = 0; i < touches.Length; i++)
				{
					Touch touch = touches[i];
					if (touch.fingerId == _touchId)
					{
						_touchPos = touch.position;
						TouchPhase phase = touch.phase;
						if (phase == TouchPhase.Ended || phase == TouchPhase.Canceled)
						{
							_isHold = false;
							_touchId = -1;
						}
					}
				}
				_wheelNewAngle = Vector2.Angle(Vector2.up, _touchPos - _wheelCenter);
				if (Vector2.Distance(_touchPos, _wheelCenter) > centerDeadZoneRadius)
				{
					if (_touchPos.x > _wheelCenter.x)
					{
						_wheelAngle += _wheelNewAngle - _wheelTempAngle;
					}
					else
					{
						_wheelAngle -= _wheelNewAngle - _wheelTempAngle;
					}
				}
				if (_wheelAngle > _maxAngle)
				{
					_wheelAngle = _maxAngle;
				}
				else if (_wheelAngle < 0f - _maxAngle)
				{
					_wheelAngle = 0f - _maxAngle;
				}
				_wheelTempAngle = _wheelNewAngle;
				_wheelRect.eulerAngles = new Vector3(0f, 0f, 0f - _wheelAngle);
			}
		}
		else
		{
			if (!Mathf.Approximately(0f, _wheelAngle))
			{
				_steeringWheelResetPosSpeed = _carInputManager.GetPowertrain.SpeedMps * 60f;
				float num = _steeringWheelResetPosSpeed * Time.fixedDeltaTime;
				if (Mathf.Abs(num) > Mathf.Abs(_wheelAngle))
				{
					_wheelAngle = 0f;
					return;
				}
				if (_wheelAngle > 0f)
				{
					_wheelAngle -= num;
				}
				else
				{
					_wheelAngle += num;
				}
			}
			_wheelRect.eulerAngles = new Vector3(0f, 0f, 0f - _wheelAngle);
		}
		_carInputManager.SetSteerWheelAngle(0f - _wheelAngle);
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		Touch[] touches = Input.touches;
		for (int i = 0; i < touches.Length; i++)
		{
			Touch touch = touches[i];
			if (touch.phase == TouchPhase.Began)
			{
				_isHold = true;
				_touchPos = touch.position;
				_wheelTempAngle = Vector2.Angle(Vector2.up, eventData.position - _wheelCenter);
				_touchId = touch.fingerId;
			}
		}
	}

	public void OnEditableHUDUpdated(EditableHUDElement element)
	{
		UpdateWheelCenter();
	}

	private void UpdateWheelCenter()
	{
		CommonExtensions.CancelAndDisposeToken(ref _cancellationTokenSourceUpdateWheel);
		_cancellationTokenSourceUpdateWheel = new CancellationTokenSource();
		UpdateWheelCenterAsync(_cancellationTokenSourceUpdateWheel.Token).Forget();
	}

	private async UniTaskVoid UpdateWheelCenterAsync(CancellationToken cancellationToken)
	{
		await UniTask.DelayFrame(1, PlayerLoopTiming.Update, cancellationToken);
		if (!cancellationToken.IsCancellationRequested && (bool)_wheelRect)
		{
			await UniTask.DelayFrame(1, PlayerLoopTiming.Update, cancellationToken);
			if (!_isDestroyed)
			{
				_wheelCenter = _wheelRect.position;
			}
		}
	}
}

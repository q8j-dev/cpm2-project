using CPMEngine;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VContainer;

public class StarterEngineButton : Button
{
	private CanvasGroup _canvasGroup;

	private Tweener _pulseTweener;

	private CarInputManager _carInputManager;

	private bool _isStarting;

	private bool _isSubscribed;

	[Inject]
	public void Construct(CarInputManager carInputManager)
	{
		_carInputManager = carInputManager;
	}

	protected override void Awake()
	{
		TryGetComponent<CanvasGroup>(out _canvasGroup);
	}

	protected override void OnEnable()
	{
		OnEnableAsync().Forget();
	}

	protected override void OnDisable()
	{
		if ((bool)_carInputManager && (bool)_carInputManager.GetPowertrain)
		{
			_carInputManager.GetPowertrain.OnEngineStarted -= OnEngineStarted;
			_carInputManager.GetPowertrain.OnEngineShutdown -= OnEngineShutdown;
			_isSubscribed = false;
		}
		_pulseTweener?.Kill();
	}

	private async UniTaskVoid OnEnableAsync()
	{
		while (!_carInputManager)
		{
			await UniTask.Yield();
		}
		while (!_carInputManager.GetPowertrain)
		{
			await UniTask.Yield();
		}
		SubscribeToEngineEvents();
	}

	private void SubscribeToEngineEvents()
	{
		if (!_isSubscribed)
		{
			Powertrain getPowertrain = _carInputManager.GetPowertrain;
			if ((bool)getPowertrain)
			{
				getPowertrain.OnEngineStarted += OnEngineStarted;
				getPowertrain.OnEngineShutdown += OnEngineShutdown;
				_isSubscribed = true;
			}
		}
		_isStarting = false;
		RestoreAnimation();
	}

	private void OnEngineStarted()
	{
		_pulseTweener?.Kill();
		if ((bool)_canvasGroup)
		{
			_canvasGroup.alpha = 0.3f;
		}
	}

	private void OnEngineShutdown()
	{
		CreatePulseAnimation();
	}

	private void CreatePulseAnimation()
	{
		_pulseTweener?.Kill();
		if ((bool)_canvasGroup)
		{
			_canvasGroup.alpha = 1f;
			_pulseTweener = _canvasGroup.DOFade(0.1f, 0.5f).SetEase(Ease.InOutQuad).SetLoops(-1, LoopType.Yoyo)
				.SetUpdate(UpdateType.Normal, isIndependentUpdate: true);
		}
	}

	private void RestoreAnimation()
	{
		if ((bool)_canvasGroup)
		{
			if (_carInputManager.GetPowertrain.IsEngineWorks)
			{
				_canvasGroup.alpha = 0.3f;
			}
			else
			{
				CreatePulseAnimation();
			}
		}
	}

	public override void OnPointerDown(PointerEventData eventData)
	{
		if (!_isStarting)
		{
			_isStarting = true;
			_carInputManager.SetStarter();
		}
	}

	public override void OnPointerUp(PointerEventData eventData)
	{
		base.OnPointerUp(eventData);
		_isStarting = false;
		_carInputManager.SetStarter(0f);
	}
}

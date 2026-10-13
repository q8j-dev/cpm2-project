using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.SOAP;

public class EngineImageRotator : MonoBehaviour
{
	private StageRender _stageRenderHandler;

	[SerializeField]
	private RawImage RawImage;

	[SerializeField]
	private CanvasGroup[] CanvasGroups;

	private CancellationTokenSource _tokenSource;

	private EventRegistry _eventRegistry;

	private bool _startRotating;

	private RegistryHandle<BoolEvent> _onLookHandle = new RegistryHandle<BoolEvent>(EventRegistryIDs.OnLook);

	private RegistryHandle<Vector2Variable> _lookRotateHandle = new RegistryHandle<Vector2Variable>(EventRegistryIDs.LookRotate);

	private const float RotationSpeed = 25f;

	[Inject]
	public void Construct(StageRender stageRender, EventRegistry eventRegistry)
	{
		_stageRenderHandler = stageRender;
		_eventRegistry = eventRegistry;
	}

	private void SubscribeToActions()
	{
		_onLookHandle.Get(_eventRegistry).Subscribe(Fade);
	}

	private void UnsubscribeFromActions()
	{
		_onLookHandle.Get(_eventRegistry)?.Unsubscribe(Fade);
	}

	private void OnEnable()
	{
		SubscribeToActions();
	}

	private void Update()
	{
		if (_startRotating)
		{
			Vector2 relativeCursorPos = _lookRotateHandle.Get(_eventRegistry).Value * 25f;
			_stageRenderHandler.SetInput(relativeCursorPos);
		}
	}

	public async UniTaskVoid Init(string path)
	{
		CommonExtensions.CancelAndDisposeToken(ref _tokenSource);
		_tokenSource = new CancellationTokenSource();
		RawImage rawImage = RawImage;
		rawImage.texture = await _stageRenderHandler.RenderObjectStage(path, (int)RawImage.rectTransform.rect.width, (int)RawImage.rectTransform.rect.height, _tokenSource.Token);
		RawImage.DOFade(1f, 0.4f);
	}

	private void OnDisable()
	{
		CommonExtensions.CancelAndDisposeToken(ref _tokenSource);
		_stageRenderHandler.Cleanup();
		UnsubscribeFromActions();
	}

	private void Fade(bool fadeOut)
	{
		float endValue = (fadeOut ? 0f : 1f);
		CanvasGroup[] canvasGroups = CanvasGroups;
		for (int i = 0; i < canvasGroups.Length; i++)
		{
			canvasGroups[i].DOFade(endValue, 0.5f);
		}
		_startRotating = fadeOut;
	}
}

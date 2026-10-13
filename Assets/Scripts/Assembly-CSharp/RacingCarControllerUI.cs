using System;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Scripting;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.CommonUtility;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.SOAP;

public class RacingCarControllerUI : MonoBehaviour
{
	private const string CarCameraRacingPrefKey = "CarCameraRacing";

	[SerializeField]
	private GameObject SteeringWheel;

	[SerializeField]
	private GameObject LeftRightArrow;

	[SerializeField]
	private float ToRoadCooldown = 5f;

	[SerializeField]
	private Button CamButton;

	[SerializeField]
	private Button ToRoadButton;

	[SerializeField]
	private Button LightButton;

	[SerializeField]
	private Image LightOnImage;

	[SerializeField]
	private Sprite[] LightOnSprites;

	[SerializeField]
	private TextMeshProUGUI CameraText;

	[Header("SteamUIParentChanger")]
	[SerializeField]
	private SteamUIParentChanger SteamUIParentChanger;

	public EventTrigger ReverseCamButton;

	public Action OnToRoad;

	private QuickCarSettings _quickCarSettings;

	private RacingCameraController _cameraController;

	private LightController _lightController;

	private EventRegistry _eventRegistry;

	private InputItemsProvider _inputItemsProvider;

	private RegistryHandle<ScriptableEvent> _toRoadHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.ToRoad);

	private RegistryHandle<ScriptableEvent> _changeCameraHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.ChangeCamera);

	private RegistryHandle<ScriptableEvent> _onHeadlightsHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.OnHeadlights);

	private bool _toRoadOnCooldown;

	[field: SerializeField]
	public GameObject InsideCamElements { get; private set; }

	private SpeedometerUI Speedometer
	{
		get
		{
			if (_inputItemsProvider == null)
			{
				return null;
			}
			return _inputItemsProvider.SpeedometerUI;
		}
	}

	[UnityEngine.Scripting.Preserve]
	[Inject]
	public void Construct(QuickCarSettings quickCarSettings, RacingCameraController cameraController, EventRegistry eventRegistry, InputItemsProvider inputItemsProvider)
	{
		_quickCarSettings = quickCarSettings;
		_cameraController = cameraController;
		_eventRegistry = eventRegistry;
		_inputItemsProvider = inputItemsProvider;
	}

	private void OnEnable()
	{
		RacingCameraController cameraController = _cameraController;
		cameraController.OnChangeCameraStateAndText = (Action<string, bool>)Delegate.Combine(cameraController.OnChangeCameraStateAndText, new Action<string, bool>(OnChangeCameraStateAndText));
		_toRoadHandle.Get(_eventRegistry).Subscribe(OnToRoadClicked);
		_changeCameraHandle.Get(_eventRegistry).Subscribe(SetActiveCameraElements);
		_onHeadlightsHandle.Get(_eventRegistry).Subscribe(HeadlightsButtonOnly);
	}

	private void OnChangeCameraStateAndText(string text, bool state)
	{
		CameraText.SetActive(state);
		CameraText.text = text;
	}

	private void Start()
	{
		_lightController = V.Car.GetComponent<LightController>();
		Init();
		SetActiveCameraElements();
		SetSteering();
		LightButton.onClick.AddListener(SetLight);
		CamButton.onClick.AddListener(ChangeCam);
		CamButton.onClick.AddListener(SetActiveCameraElements);
		ToRoadButton.onClick.AddListener(OnToRoadClicked);
		SetReverseCamListeners();
		LightOnImage.sprite = LightOnSprites[_lightController.HState];
		if ((bool)SteamUIParentChanger)
		{
			SteamUIParentChanger.ApplyAllChanges();
		}
	}

	private void HeadlightsButtonOnly()
	{
		LightOnImage.sprite = LightOnSprites[_lightController.HState];
	}

	private void SetLight()
	{
		_quickCarSettings.TurnOnLights();
		LightOnImage.sprite = LightOnSprites[_lightController.HState];
	}

	private void SetReverseCamListeners()
	{
		EventTrigger.Entry entry = new EventTrigger.Entry
		{
			eventID = EventTriggerType.PointerUp
		};
		entry.callback.AddListener((BaseEventData _) =>
		{
			_cameraController.CameraReversed(isPressed: false).Forget();
		});
		EventTrigger.Entry entry2 = new EventTrigger.Entry
		{
			eventID = EventTriggerType.PointerDown
		};
		entry2.callback.AddListener((BaseEventData _) =>
		{
			_cameraController.CameraReversed(isPressed: true).Forget();
		});
		ReverseCamButton.triggers.Add(entry);
		ReverseCamButton.triggers.Add(entry2);
	}

	private void SetActiveCameraElements()
	{
		bool active = PlayerPrefs.GetInt("CarCameraRacing", 1) == 0;
		InsideCamElements.SetActive(active);
	}

	private void ChangeCam()
	{
		_cameraController.SetNextCam();
	}

	public void Init()
	{
		SpeedometerUI speedometer = Speedometer;
		if ((bool)speedometer)
		{
			speedometer.SetTestMode(state: false);
		}
	}

	public void Resume()
	{
		SpeedometerUI speedometer = Speedometer;
		if ((bool)speedometer)
		{
			speedometer.ReInit();
		}
		SetSteering();
	}

	private void OnToRoadClicked()
	{
		if ((bool)ToRoadButton && ToRoadButton.isActiveAndEnabled && !_toRoadOnCooldown)
		{
			OnToRoad?.Invoke();
			ToRoadButton.interactable = false;
			_toRoadOnCooldown = true;
			EnableToRoadDelayed().Forget();
		}
	}

	private async UniTaskVoid EnableToRoadDelayed()
	{
		await UniTask.Delay(TimeSpan.FromSeconds(ToRoadCooldown));
		if (ToRoadButton != null)
		{
			ToRoadButton.interactable = true;
		}
		_toRoadOnCooldown = false;
	}

	public void SetVisibleSpeedometer(bool visible)
	{
		SpeedometerUI speedometer = Speedometer;
		if ((bool)speedometer)
		{
			speedometer.SetTestMode(visible);
		}
	}

	public void SetSteering()
	{
		int steering = PlayerPrefs.GetInt("steering", 0);
		SetSteering(steering);
	}

	private void SetSteering(int steeringId)
	{
		switch (steeringId)
		{
		case 0:
			SteeringWheel.SetActive(value: true);
			LeftRightArrow.SetActive(value: false);
			break;
		case 1:
			SteeringWheel.SetActive(value: false);
			LeftRightArrow.SetActive(value: false);
			break;
		case 3:
			SteeringWheel.SetActive(value: false);
			LeftRightArrow.SetActive(value: true);
			break;
		case 2:
			break;
		}
	}

	private void OnDisable()
	{
		_cameraController.CameraReversed(isPressed: false).Forget();
		RacingCameraController cameraController = _cameraController;
		cameraController.OnChangeCameraStateAndText = (Action<string, bool>)Delegate.Remove(cameraController.OnChangeCameraStateAndText, new Action<string, bool>(OnChangeCameraStateAndText));
		_toRoadHandle.Get(_eventRegistry)?.Unsubscribe(OnToRoadClicked);
		_changeCameraHandle.Get(_eventRegistry)?.Unsubscribe(SetActiveCameraElements);
		_onHeadlightsHandle.Get(_eventRegistry)?.Unsubscribe(HeadlightsButtonOnly);
	}

	private void OnDestroy()
	{
		CamButton.onClick.RemoveAllListeners();
	}
}

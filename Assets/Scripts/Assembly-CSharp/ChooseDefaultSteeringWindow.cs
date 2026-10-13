using UnityEngine;
using UnityEngine.Device;
using UnityEngine.UI;
using VContainer;

public class ChooseDefaultSteeringWindow : MonoBehaviour
{
	private int _currentSteeringIndex;

	private CoreGameManager _coreGameManager;

	private MenuLevelParent _menuLevelParent;

	[SerializeField]
	private Toggle[] SteeringTogglesArray = default;

	private CarInputManager _carInputManager;

	[Inject]
	public void Construct(CoreGameManager coreGameManager, MenuLevelParent menuLevelParent, CarInputManager carInputManager)
	{
		_coreGameManager = coreGameManager;
		_menuLevelParent = menuLevelParent;
		_carInputManager = carInputManager;
	}

	private void Start()
	{
		SetSteering();
		base.gameObject.SetActive(value: false);
	}

	public void SetCurrentSteeringIndex(int index)
	{
		_currentSteeringIndex = index;
	}

	private void OnEnable()
	{
		SteeringTogglesArray[_currentSteeringIndex].SetIsOnWithoutNotify(value: true);
	}

	private void SetSteering()
	{
		int value = 0;
		switch (_currentSteeringIndex)
		{
		case 0:
			_coreGameManager.SetSteeringArrow();
			value = 3;
			break;
		case 1:
			_coreGameManager.SetSteeringWheel();
			break;
		case 2:
			_coreGameManager.SetSteeringTilt();
			value = 1;
			break;
		}
		PlayerPrefs.SetInt("steering", value);
		_menuLevelParent.Resume();
		base.gameObject.SetActive(value: false);
		if ((bool)_carInputManager)
		{
			bool flag = UnityEngine.Device.SystemInfo.deviceType == DeviceType.Desktop;
			_carInputManager.SetController((!flag) ? Controller.Mobile : Controller.Desktop);
		}
	}
}

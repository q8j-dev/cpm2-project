using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;
using _Car_Parking;
using _Car_Parking.Scripts.Providers;

public class PhoneController : MonoBehaviour
{
	[SerializeField]
	private GameObject[] pages;

	private GameObject _currentApp;

	private float _volume;

	private FreeDriveProvider _freeDriveProvider;

	private LevelsProvider _levelsProvider;

	private MenuLevelParent _menuLevelParent;

	private CarInputManager _carInputManager;

	[Inject]
	private void Construct(FreeDriveProvider freeDriveProvider, LevelsProvider levelsProvider, MenuLevelParent menuLevelParent, CarInputManager carInputManager)
	{
		_menuLevelParent = menuLevelParent;
		_levelsProvider = levelsProvider;
		_freeDriveProvider = freeDriveProvider;
		_carInputManager = carInputManager;
	}

	private void AppOperation(bool toOpen)
	{
		if ((bool)_currentApp)
		{
			_currentApp.SetActive(toOpen);
		}
		GameObject[] array = pages;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(!toOpen);
		}
	}

	public void OpenApp(GameObject appUIGameObject)
	{
		_currentApp = appUIGameObject;
		AppOperation(toOpen: true);
	}

	public void CloseApp()
	{
		AppOperation(toOpen: false);
		_currentApp = null;
	}

	public void ResumeService()
	{
		GPSTracker.ClearPath(forget: true);
		_freeDriveProvider.MapCamera.HideBigMap();
		if (_levelsProvider.LevelScript.GetComponent<Camera>().enabled || !FlagsStorage.IsFreeDriveMode)
		{
			_levelsProvider.LevelScript.SetActiveFalseEachMenu(1);
			_carInputManager.ClearAllInputs();
			_levelsProvider.LevelScript.UnCleanScreen();
			if (SceneManager.GetActiveScene().buildIndex == 13)
			{
				_menuLevelParent.HideEmoji();
			}
		}
	}

	public void CloseAllApps()
	{
		if (FlagsStorage.IsFreeDriveMode)
		{
			_levelsProvider.LevelScript.SetActiveFalseEachMenu(17);
		}
	}
}

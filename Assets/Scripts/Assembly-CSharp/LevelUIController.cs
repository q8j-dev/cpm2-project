using UnityEngine;
using VContainer;
using _Car_Parking;
using _Car_Parking.Scripts.Providers;

public class LevelUIController : MonoBehaviour
{
	[SerializeField]
	private GameObject[] UIElementsForWalk;

	[SerializeField]
	private GameObject[] UIElementsForDrive;

	private LevelScript _levelScript;

	private LevelsProvider _levelsProvider;

	private MenuLevelParent _menuLevelParent;

	[Inject]
	public void Construct(LevelScript levelScript, LevelsProvider levelsProvider, MenuLevelParent menuLevelParent)
	{
		_menuLevelParent = menuLevelParent;
		_levelsProvider = levelsProvider;
		_levelScript = levelScript;
	}

	private void OnEnable()
	{
		if ((bool)_levelsProvider.CurrentLevelData && _levelsProvider.CurrentLevelData.IndexInScene < _levelScript.levelDatas.Length)
		{
			if (_levelsProvider.CurrentLevelData.DriverOutSideBegin)
			{
				FlagsStorage.OutOfCar = true;
			}
			ShowWalkElements(FlagsStorage.OutOfCar);
		}
	}

	public void ShowWalkElements(bool canShow)
	{
		ChangeUIWalkElementsState(canShow);
		ChangeUIDriveElementsState(!canShow);
	}

	private void ChangeUIWalkElementsState(bool state)
	{
		GameObject[] uIElementsForWalk = UIElementsForWalk;
		foreach (GameObject gameObject in uIElementsForWalk)
		{
			if ((bool)gameObject)
			{
				gameObject.SetActive(state);
			}
		}
	}

	private void ChangeUIDriveElementsState(bool state)
	{
		if (state && FlagsStorage.IsPassenger)
		{
			return;
		}
		GameObject[] uIElementsForDrive = UIElementsForDrive;
		foreach (GameObject gameObject in uIElementsForDrive)
		{
			if ((bool)gameObject)
			{
				gameObject.SetActive(state);
			}
		}
		if ((bool)_menuLevelParent)
		{
			_menuLevelParent.SetSteering();
		}
	}
}

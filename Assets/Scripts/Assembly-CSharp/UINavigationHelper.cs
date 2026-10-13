using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.Providers;

public class UINavigationHelper : MonoBehaviour
{
	[SerializeField]
	private Sprite activeSprite;

	[SerializeField]
	private Sprite deactivateSprite;

	[SerializeField]
	private SpriteSwitcher[] buttons;

	[SerializeField]
	public GameObject[] childsWindows;

	public bool ButtonTouched;

	private RacingProvider _racingProvider;

	[Inject]
	private void Construct(RacingProvider racingProvider)
	{
		_racingProvider = racingProvider;
	}

	private void Start()
	{
		HideAll();
	}

	public void HideAll()
	{
		for (int i = 0; i < buttons.Length; i++)
		{
			buttons[i].ChangeState(state: false);
			childsWindows[i].SetActive(value: false);
		}
	}

	public void FocusOnWindowByIndex(int index)
	{
		ButtonTouched = true;
		bool activeSelf = childsWindows[index].activeSelf;
		activeSelf = !activeSelf;
		for (int i = 0; i < buttons.Length; i++)
		{
			childsWindows[i].SetActive(i == index);
			buttons[i].ChangeState(i == index);
		}
		childsWindows[index].SetActive(activeSelf);
		buttons[index].ChangeState(activeSelf);
		_racingProvider.DragRacingController.dragRacingUI.nowRacingNotificationWindow.SetActive(value: false);
	}
}

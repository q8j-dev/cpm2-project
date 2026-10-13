using CPMEngine;
using TMPro;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Scopes;

public class NextRacePopupWindow : MonoBehaviour
{
	[SerializeField]
	private TextMeshProUGUI nextRaceText;

	[SerializeField]
	private UserCard nextOpponentCard;

	private DragRacingController _dragRacingController;

	[Inject]
	public void Construct(OptionalDependency<DragRacingController> dragRacingController)
	{
		_dragRacingController = dragRacingController.Value;
	}

	public void Init(MainCarValues carValues, Powertrain powertrain)
	{
		nextOpponentCard.Init(carValues, powertrain);
		SetNextRaceTitle();
	}

	private void SetNextRaceTitle()
	{
		nextRaceText.transform.parent.gameObject.SetActive(!_dragRacingController.qualification);
		switch (_dragRacingController.RaceWon)
		{
		case 0:
			nextRaceText.text = "1/16";
			break;
		case 1:
			nextRaceText.text = "1/8";
			break;
		case 2:
			nextRaceText.text = "1/4";
			break;
		case 3:
			nextRaceText.text = "1/2";
			break;
		case 4:
			nextRaceText.text = new LocalizedString("NextRacePopupWindow.Final");
			break;
		case 5:
			nextRaceText.text = "";
			break;
		}
	}
}

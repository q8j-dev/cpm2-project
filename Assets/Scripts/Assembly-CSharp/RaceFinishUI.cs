using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.Audio;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.Scopes;

public class RaceFinishUI : MonoBehaviour
{
	[SerializeField]
	private RaceFinishElement leftElmnt;

	[SerializeField]
	private RaceFinishElement rightElmnt;

	private DragRacingUI _dragRacingUI;

	private SoundEventChannel _soundEventChannel;

	private RacingProvider _racingProvider;

	[Inject]
	public void Construct(OptionalDependency<DragRacingUI> dragRacingUI, SoundEventChannel soundEventChannel, RacingProvider racingProvider)
	{
		_racingProvider = racingProvider;
		_soundEventChannel = soundEventChannel;
		_dragRacingUI = dragRacingUI.Value;
	}

	public async UniTask ShowResults(GameObject carLeft, GameObject carRight, bool raceAgain, int timeClass = 0)
	{
		_ = 1;
		try
		{
			await UniTask.Delay(1000);
			while (_dragRacingUI.controller.dragRacingCamera.virtualCameraOnFinish.enabled)
			{
				await UniTask.Delay(2000);
			}
			_dragRacingUI.disableFinishSign();
			_dragRacingUI.parentResult.gameObject.SetActive(value: true);
			_dragRacingUI.resumeButton.SetActive(value: true);
			bool quaterMile = _dragRacingUI.controller.runDistance == DragRacingController.RunDistance.QuaterMile;
			if ((bool)carLeft && (bool)carRight)
			{
				Dragy component = carLeft.GetComponent<Dragy>();
				Dragy component2 = carRight.GetComponent<Dragy>();
				leftElmnt.ParseDatas(component, component2, quaterMile);
				rightElmnt.ParseDatas(component2, component, quaterMile);
			}
			else
			{
				rightElmnt.ParseDatas(carRight, quaterMile);
				leftElmnt.ParseDatas(carLeft, quaterMile);
			}
			if (carLeft == V.LastUsedCar || carRight == V.LastUsedCar || _racingProvider.DragRacingController.focusOnOtherCar)
			{
				_dragRacingUI.finishTextResult.gameObject.SetActive(value: true);
				Dragy component3 = V.LastUsedCar.GetComponent<Dragy>();
				if (_dragRacingUI.controller.qualification || timeClass != 0)
				{
					switch (timeClass)
					{
					case 8:
					{
						string text2 = new LocalizedString("RaceFinishUI.Youwillbeonsclass");
						_dragRacingUI.finishTextResult.text = text2.Replace("{0}", "'Unlim'");
						break;
					}
					default:
					{
						string text = new LocalizedString("RaceFinishUI.Youwillbeonsclass");
						_dragRacingUI.finishTextResult.text = text.Replace("{0}", timeClass.ToString());
						break;
					}
					case 0:
						break;
					}
					if (component3.reactionTime < 0f)
					{
						_dragRacingUI.finishTextResult.text = new LocalizedString("RaceFinishUI.Falsestart,waitforgreenlight");
						_dragRacingUI.resumeButton.SetActive(value: false);
						raceAgain = true;
					}
				}
				else if (raceAgain)
				{
					_dragRacingUI.finishTextResult.text = new LocalizedString("RaceFinishUI.FalseStartRaceAgain");
				}
				else if (component3.lost)
				{
					_dragRacingUI.finishTextResult.text = new LocalizedString("RaceFinishUI.YouLost");
					if (component3.reason == Dragy.LostReason.falseStart)
					{
						string text3 = new LocalizedString("RaceFinishUI.YouLostFalseStart");
						_dragRacingUI.finishTextResult.text = text3.Replace("{0}", component3.falseStartCount.ToString());
					}
					else if (component3.reason == Dragy.LostReason.fasterClass)
					{
						string text4 = new LocalizedString("RaceFinishUI.YourCarFasterThan");
						_dragRacingUI.finishTextResult.text = text4.Replace("{0}", _racingProvider.DragRacingController.timeClass.ToString());
					}
				}
				else if (component3.lostButFor3place)
				{
					_dragRacingUI.finishTextResult.text = new LocalizedString("RaceFinishUI.Youlost,youwillracefor3rdplace");
				}
				else
				{
					_dragRacingUI.finishTextResult.text = new LocalizedString("RaceFinishUI.YouAreWinner");
				}
			}
			else
			{
				_dragRacingUI.finishTextResult.gameObject.SetActive(value: false);
			}
			_dragRacingUI.restartRaceButtonOnFinish.SetActive(raceAgain);
			_dragRacingUI.toGarageButtonOnFinish.SetActive(!raceAgain);
			_dragRacingUI.SetActiveFalseEachMenu(5);
			if (!_dragRacingUI.controller.autoSteering)
			{
				_dragRacingUI.lastAutoSteeringState = true;
				_dragRacingUI.controller.SetSteering(auto: true);
			}
		}
		finally
		{
			_racingProvider.DragRacingController.OnFinishResultShown();
		}
	}
}

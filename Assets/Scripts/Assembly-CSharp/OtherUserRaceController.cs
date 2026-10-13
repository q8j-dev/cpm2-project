using CPMEngine;
using DG.Tweening;
using UnityEngine;

public class OtherUserRaceController : MonoBehaviour
{
	public UserCard leftCarCard;

	public UserCard rightCarCard;

	[SerializeField]
	private CanvasGroup resultPanel;

	public void Init(MainCarValues leftCar, Powertrain leftDriveTrain, MainCarValues rightCar, Powertrain rightDriveTrain)
	{
		resultPanel.alpha = 0f;
		leftCarCard.Init(leftCar, leftDriveTrain, setFlag: true);
		rightCarCard.Init(rightCar, rightDriveTrain, setFlag: true);
	}

	public void ShowResult()
	{
		resultPanel.DOFade(1f, 1.5f);
	}
}

using CPMEngine;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Localization;

public class OnePlayerWindow : MonoBehaviour
{
	public TextMeshProUGUI nickName;

	public TextMeshProUGUI raceCount;

	public TextMeshProUGUI mileage;

	public TextMeshProUGUI taxiCount;

	public TextMeshProUGUI policeFineCount;

	public TextMeshProUGUI winrateText;

	public RawImage icon;

	public void SetAllLabel(LeaderBoardData leaderBoardData)
	{
		int num = (int)leaderBoardData.racingWin;
		int num2 = (int)(leaderBoardData.racingCount - leaderBoardData.racingWin);
		int num3 = (int)leaderBoardData.racingCount;
		float num4 = 0f;
		if (num3 != 0)
		{
			num4 = (float)num / (float)num3;
		}
		TMP_FontAsset_Placeholder.EnsureFor(leaderBoardData.nickname);
		nickName.text = leaderBoardData.nickname;
		string text = new LocalizedString("OnePlayerWindow.Race");
		string text2 = new LocalizedString("OnePlayerWindow.Win");
		string text3 = new LocalizedString("OnePlayerWindow.Lose");
		raceCount.text = text + num3 + text2 + num + text3 + num2;
		winrateText.text = (num4 * 100f).ToString() ?? "";
		mileage.text = Utils.GetConvertedMileage(leaderBoardData.mileage) + Utils.CurrentMetric;
		taxiCount.text = leaderBoardData.taxiDoneCount.ToString();
		policeFineCount.text = leaderBoardData.policeArrestCount.ToString();
		int personIndex = leaderBoardData.personIndex;
		personIndex = Mathf.Clamp(personIndex, 0, V.CoreGameManager.MenuLevelParent.TexturesOfPersons.Length - 1);
		icon.texture = V.CoreGameManager.MenuLevelParent.TexturesOfPersons[personIndex];
	}
}

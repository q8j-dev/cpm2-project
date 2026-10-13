using TMPro;
using UnityEngine;
using UnityEngine.UI;
using _Car_Parking.Scripts.Localization;

public class achievmentItemUI : MonoBehaviour
{
	[SerializeField]
	private RawImage BackgroundImage;

	[SerializeField]
	private RawImage MiddleImage;

	[SerializeField]
	private TextMeshProUGUI TitleText;

	[SerializeField]
	private TextMeshProUGUI RankText;

	[SerializeField]
	private TextMeshProUGUI ScoreText;

	[SerializeField]
	private Button AchievmentButton;

	[SerializeField]
	private Slider ProgressSlider;

	public Texture backgroundTexture;

	public string score;

	public string title;

	public string rang;

	public Texture middleTexture;

	public bool DoNotHaveAtAll;

	public string mainDescription;

	public string description;

	private AchievementsWindow _achievementsWindow;

	private void Start()
	{
		AchievmentButton.onClick.AddListener(ShowDetails);
	}

	public void SetData(AchievementItemData data, AchievementsWindow achievementsWindow)
	{
		_achievementsWindow = achievementsWindow;
		backgroundTexture = data.backgroundTexture;
		score = data.score;
		title = data.title;
		rang = data.rang;
		middleTexture = data.middleTexture;
		DoNotHaveAtAll = data.DoNotHaveAtAll;
		mainDescription = data.mainDescription;
		description = data.description;
	}

	public void SetItem()
	{
		BackgroundImage.texture = backgroundTexture;
		MiddleImage.texture = middleTexture;
		TitleText.text = new LocalizedString(title);
		RankText.text = string.Concat(new LocalizedString("achievmentItemUI.Rank"), (int.Parse(rang) + 1).ToString());
		if (DoNotHaveAtAll)
		{
			BackgroundImage.color = new Color(1f, 1f, 1f, 0.2f);
			MiddleImage.color = new Color(1f, 1f, 1f, 0.2f);
			ScoreText.text = "";
			RankText.text = "";
			RankText.gameObject.SetActive(value: false);
		}
		else
		{
			ProgressSlider.value = (float)(int.Parse(rang) + 1) / 6f;
			ScoreText.text = score;
			RankText.text = string.Concat(new LocalizedString("achievmentItemUI.Rank"), (int.Parse(rang) + 1).ToString());
		}
	}

	public void ShowDetails()
	{
		_achievementsWindow.ShowDetailDescription(this);
	}
}

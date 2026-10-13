using System;
using CPMEngine;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Other.Helpers;

public class AchievementNotifcationWindow : WindowScript, IBackableWindow
{
	[SerializeField]
	private TextMeshProUGUI LabelName = default;

	[SerializeField]
	private TextMeshProUGUI LabelDescription = default;

	[SerializeField]
	private RawImage BackgroundTexture = default;

	[SerializeField]
	private RawImage MiddleTexture = default;

	[SerializeField]
	private Button CloseButton;

	public void ShowNotification(string name, string description, int rank, Texture midTexture, int order)
	{
		LabelName.text = name;
		LabelDescription.text = description;
		MiddleTexture.texture = midTexture;
		BackgroundTexture.texture = Resources.Load<Texture>("icons/Rating/Ach_BG_" + (rank + 1));
		Observable.Timer(TimeSpan.FromSeconds(3.0)).Subscribe((long _) =>
		{
			ShowWindow();
		}).AddTo(this);
		base.gameObject.SetActive(value: false);
	}

	public void ShowRankUpgrade(int newRank, int order)
	{
		string rankNameByIndex = Utils.GetRankNameByIndex(newRank);
		LabelName.text = rankNameByIndex;
		LabelDescription.text = string.Concat(new LocalizedString("AchievementNotifcationWindow.YourNewRank"), " ", rankNameByIndex);
		Texture texture = UtilsRank.LoadRankTextureFromResources((UtilsRank.RankType)newRank);
		MiddleTexture.texture = texture;
		MonoBehaviour.print(order);
		Observable.Timer(TimeSpan.FromSeconds(2.0)).Subscribe((long _) =>
		{
			ShowWindow();
		}).AddTo(this);
		base.gameObject.SetActive(value: false);
	}

	public void InitBackButton()
	{
		_managerUI.OnBackButtonActivated?.Invoke(obj: true);
		_managerUI.SetCurrentWindow(this);
	}

	public void Back()
	{
		Close();
	}

	protected override void Start()
	{
		InitBackButton();
		base.Start();
	}

	private void ShowWindow()
	{
		base.gameObject.SetActive(value: true);
		UnityEngine.Object.Destroy(base.gameObject, 10f);
	}

	public void CloseThisWindow()
	{
		UnityEngine.Object.Destroy(base.gameObject);
	}
}

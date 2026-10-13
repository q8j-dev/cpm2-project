using System;
using System.Linq;
using System.Threading;
using CPMEngine;
using CodeStage.AntiCheat.Storage;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using _Car_Parking;
using _Car_Parking.Scripts.Levels;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Other.Helpers;
using _Car_Parking.Scripts.Providers;

public class LevelLoader : MonoBehaviour
{
	public TextMeshProUGUI bestTimeLabel;

	[SerializeField]
	private TextMeshProUGUI TitleLabel = default;

	[SerializeField]
	private Image SelectedLine = default;

	[SerializeField]
	private Image LevelNamePanel = default;

	[SerializeField]
	private GameObject lockedPanel = default;

	public GameObject DoneMark;

	[SerializeField]
	private RawImage MainTexture = default;

	[SerializeField]
	private CanvasGroup CanvasGroup = default;

	private Button _button;

	public bool LevelClosed;

	public bool willLoad = true;

	private LevelsMenuWindow _levelsMenuWindow;

	private LevelDataScript _levelDataScript;

	private LevelsProvider _levelsProvider;

	private void OnEnable()
	{
		_button = GetComponent<Button>();
	}

	public void Init(LevelDataScript levelDataScript, LevelsMenuWindow levelsMenuWindow, LevelsProvider levelsProvider, int idBefore, int orderInList)
	{
		_levelsProvider = levelsProvider;
		_levelsMenuWindow = levelsMenuWindow;
		_levelDataScript = levelDataScript;
		OnSelected(selected: false);
		TitleLabel.text = new LocalizedString(levelDataScript.Title);
		LevelPreviewData levelPreviewData = levelsMenuWindow.PreviewsConfig.LevelPreviews.FirstOrDefault((LevelPreviewData preview) => preview.ID == levelDataScript.ID);
		if (levelPreviewData != null)
		{
			MainTexture.texture = levelPreviewData.PreviewTex;
		}
		string text = "";
		float num = ObscuredPrefs.GetFloat($"bestResultID{levelDataScript.ID}");
		if (!ObscuredPrefs.HasKey($"bestResultID{levelDataScript.ID}"))
		{
			text += "-";
			DoneMark.SetActive(value: false);
			bestTimeLabel.gameObject.SetActive(value: false);
			lockedPanel.gameObject.SetActive(value: false);
			CanvasGroup.blocksRaycasts = true;
		}
		else
		{
			text = ((num == 0f || num == 1f) ? ((string)new LocalizedString("LevelLoader.Done")) : $"{text}{num:N1}s");
			if (levelDataScript.Type == LevelType.Chasing || levelDataScript.SceneForLoad == 13 || levelDataScript.ID == 55)
			{
				text = new LocalizedString("LevelLoader.Done");
			}
			DoneMark.SetActive(value: true);
			bestTimeLabel.gameObject.SetActive(value: true);
			lockedPanel.gameObject.SetActive(value: false);
			CanvasGroup.blocksRaycasts = true;
			if (levelDataScript.ID % 10 == 0)
			{
				_ = RateAppHelper.RateAppAsync();
			}
		}
		bestTimeLabel.text = text;
		_button.onClick.RemoveAllListeners();
		_button.onClick.AddListener(LoadLevel);
		if (orderInList > 2 && ObscuredPrefs.GetFloat("bestResultID" + idBefore, -1f) == -1f && num == 0f)
		{
			DoneMark.SetActive(value: false);
			bestTimeLabel.gameObject.SetActive(value: false);
			lockedPanel.gameObject.SetActive(value: true);
			CanvasGroup.blocksRaycasts = false;
		}
	}

	public void OnSelected(bool selected)
	{
		TitleLabel.color = (selected ? _levelsMenuWindow.SelectedTextColor : _levelsMenuWindow.NotSelectedTextColor);
		LevelNamePanel.color = (selected ? _levelsMenuWindow.SelectedTextParentColor : _levelsMenuWindow.NotSelectedTextParentColor);
		SelectedLine.gameObject.SetActive(selected);
	}

	public void NotLoad()
	{
		willLoad = false;
	}

	public void Load()
	{
		willLoad = true;
	}

	private void LoadLevel()
	{
		_levelsMenuWindow.OnLevelClick(this);
		if (!willLoad || LevelClosed)
		{
			return;
		}
		if (FlagsStorage.IsMainMenu)
		{
			V.CurrentIndexArrayPositionCar = 0;
		}
		FlagsStorage.IsMainMenu = false;
		int iD = _levelDataScript.ID;
		LevelDataScript currentLevelData = _levelsProvider.CurrentLevelData;
		_levelsProvider.CurrentLevelData = _levelDataScript;
		int sceneForLoad = _levelsProvider.CurrentLevelData.SceneForLoad;
		TitleLabel.text = new LocalizedString("LevelLoader.Loading");
		int num = -1;
		if (currentLevelData != null)
		{
			num = currentLevelData.ID;
		}
		if (num == _levelsProvider.CurrentLevelData.ID && SceneManager.GetActiveScene().buildIndex != 1 && !ScenesHelper.IsMenuSceneActive())
		{
			_levelsMenuWindow.Back();
			return;
		}
		PlayerPrefs.SetFloat("lastPlayedLevelSibIndex", base.gameObject.transform.GetSiblingIndex());
		if (iD != 13)
		{
			PlayerPrefs.SetFloat("lastPlayedLevel", iD);
		}
		if (sceneForLoad == 13)
		{
			V.PlaceToSpawn = V.placeToSpawn.Desert;
			FlagsStorage.IsMultiFree = false;
		}
		if (sceneForLoad != SceneManager.GetActiveScene().buildIndex)
		{
			SwitchSceneWithCurtain(_levelsMenuWindow, sceneForLoad);
		}
		else
		{
			_levelsProvider.LevelScript.RestartScene();
		}
	}

	private static void SwitchSceneWithCurtain(LevelsMenuWindow levelsMenuWindow, int nextScene)
	{
		UniTask.Void(async () =>
		{
			try
			{
				await levelsMenuWindow.ShowSceneTransitionCurtainAsync();
			}
			catch (Exception ex)
			{
				Debug.LogWarning("[Curtain] шторка не показана: " + ex.Message);
			}
			AsyncSceneSwitcher.Load(nextScene);
		});
	}

	private async UniTask LoadAfterAsync(int nextScene, CancellationToken ct = default(CancellationToken))
	{
		await UniTask.Delay(TimeSpan.FromSeconds(0.5), DelayType.Realtime, PlayerLoopTiming.Update, ct);
		V.CoreGameManager.ClearAssets();
		SceneManager.LoadScene(nextScene);
	}
}

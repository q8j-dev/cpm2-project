using System.Collections.Generic;
using System.Linq;
using Addler.Runtime.Core.LifetimeBinding;
using CPM.Database;
using CodeStage.AntiCheat.Storage;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.Scripting;
using UnityEngine.UI;
using VContainer;
using _Car_Parking;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Levels;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Other.Helpers;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.SOAP_Registry;
using _Car_Parking.Scripts.Scopes;
using _Car_Parking.Scripts.UIElement;

public class LevelsMenuWindow : WindowScript, IBackableWindow
{
	private const string PreviewsConfigPath = "LevelPreviewsConfig";

	[SerializeField]
	private TextMeshProUGUI TitleText = default;

	[SerializeField]
	private LevelLoader PrefabLevel = default;

	[SerializeField]
	private LevelCategoryItem PrefabCategory = default;

	[SerializeField]
	private Transform LevelsParentTransform = default;

	[SerializeField]
	private Transform CategoriesParentTransform = default;

	[SerializeField]
	private ContentSizeFitter ContentSizeFitter = default;

	[SerializeField]
	private ScrollRect LevelssScrollRect = default;

	[SerializeField]
	private Button BackButton = default;

	[SerializeField]
	private int CellCountInOneRow = 1;

	private LevelsConfig _levelsConfig;

	private IPrefabFactory _prefabFactory;

	private readonly List<LevelLoader> _cachedLevelItems = new List<LevelLoader>();

	private List<LevelCategoryItem> _cachedCategoryItems;

	[SerializeField]
	private Slider Slider = default;

	[Header("Colors")]
	public Color SelectedColor;

	public Color NotSelectedColor;

	public Color SelectedTextColor;

	public Color NotSelectedTextColor;

	public Color SelectedTextParentColor;

	public Color NotSelectedTextParentColor;

	private LevelLoader _currentSelectedLevel;

	private int _currentCategoryIndex = -1;

	private int _lastCategoryIndex;

	private LevelScript _levelScript;

	private OptionalDependency<LevelScript> _levelScriptDep;

	private int _previusMenuIndex = -1;

	private LevelsProvider _levelsProvider;

	private int _levelsCount;

	private bool _isDestroyed;

	private AllCarsInfo _allCarsInfo;

	public LevelPreviewsConfig PreviewsConfig { get; private set; }

	protected override InputGroupsEnum WindowInputGroup => InputGroupsEnum.Ui;

	[UnityEngine.Scripting.Preserve]
	[Inject]
	public void Construct(IPrefabFactory prefabFactory, LevelsProvider levelsProvider, LevelsConfig levelsConfig, OptionalDependency<LevelScript> levelScript, AllCarsInfo allCarsInfo)
	{
		_levelsProvider = levelsProvider;
		_prefabFactory = prefabFactory;
		_levelScriptDep = levelScript;
		_levelsConfig = levelsConfig;
		_allCarsInfo = allCarsInfo;
	}

	protected override void Awake()
	{
		base.Awake();
		_levelScript = _levelScriptDep?.Value;
	}

	public void InitBackButton()
	{
		_managerUI.OnBackButtonActivated?.Invoke(obj: true);
		_managerUI.SetCurrentWindow(this);
	}

	public void Back()
	{
		if (ScenesHelper.IsMenuSceneActive())
		{
			_managerUI.ShowGameModeWindow();
		}
		if (FlagsStorage.IsLevelScene)
		{
			if (_levelScript.IsLevelPaused)
			{
				_managerUI.ShowLevelPauseWindow();
			}
			else if (_previusMenuIndex < 0)
			{
				_previusMenuIndex = 0;
				_levelScript.OpenFirstMenu();
			}
			else
			{
				_levelScript.SetActiveFalseEachMenu(_previusMenuIndex);
			}
		}
		Close();
		V.MenuIndex = _previusMenuIndex;
	}

	public void ShowLoading()
	{
		_managerUI.ShowWaitingWindow(set: true).Forget();
	}

	public UniTask ShowSceneTransitionCurtainAsync()
	{
		return _managerUI.ShowWaitingWindowDark(set: true);
	}

	public void CloseLoading()
	{
		_managerUI.CloseWaitingWindow();
	}

	public override void SetAdditionalData(string data)
	{
		base.SetAdditionalData(data);
		if (!string.IsNullOrEmpty(data) && int.TryParse(data, out var result))
		{
			_previusMenuIndex = result;
		}
	}

	protected override void Start()
	{
		InitBackButton();
		base.Start();
		BackButton.onClick.RemoveAllListeners();
		BackButton.onClick.AddListener(Back);
		Slider.gameObject.SetActive(value: false);
		SetLevelsInfo().Forget();
	}

	protected override void OnDestroy()
	{
		_isDestroyed = true;
	}

	private async UniTask SetLevelsInfo()
	{
		PreviewsConfig = await LoadPreviewsConfig();
		CreateCategories();
		LoadParkingLevelsMenu();
		ParseCategory(0).Forget();
	}

	private async UniTask<LevelPreviewsConfig> LoadPreviewsConfig()
	{
		AsyncOperationHandle<LevelPreviewsConfig> asyncOperationHandle = Addressables.LoadAssetAsync<LevelPreviewsConfig>("LevelPreviewsConfig").BindTo(base.gameObject);
		await asyncOperationHandle.Task;
		if (_isDestroyed)
		{
			return null;
		}
		return asyncOperationHandle.Result;
	}

	private void CreateCategories()
	{
		_cachedCategoryItems = new List<LevelCategoryItem>();
		LevelCategory[] levelCategories = _levelsConfig.LevelCategories;
		List<int> activeCategories = new List<int>();
		for (int i = 0; i < levelCategories.Length; i++)
		{
			LevelCategoryItem levelCategoryItem = _prefabFactory.Instantiate(PrefabCategory, CategoriesParentTransform);
			levelCategoryItem.Init(this, levelCategories[i]);
			_cachedCategoryItems.Add(levelCategoryItem);
			if (levelCategories[i].isActive)
			{
				activeCategories.Add(i);
			}
		}
		_levelsCount = _levelsConfig.LevelsDatas.Count((LevelDataScript l) => activeCategories.Contains(l.CategoryIndex) && !IsLevelCarBanned(l));
	}

	private bool IsLevelCarBanned(LevelDataScript level)
	{
		if (_allCarsInfo == null)
		{
			return false;
		}
		if (level.CarId > 0 && _allCarsInfo.IsBannedCar(level.CarId))
		{
			return true;
		}
		if (level.RacingCarId > 0)
		{
			return _allCarsInfo.IsBannedCar(level.RacingCarId);
		}
		return false;
	}

	private void LoadParkingLevelsMenu()
	{
		if (!(_levelsConfig == null) && !FlagsStorage.IsFreeDriveMode)
		{
			UpdateOverAllProgress();
		}
	}

	public async UniTask ParseCategory(int catIndex)
	{
		if (_currentCategoryIndex == catIndex)
		{
			return;
		}
		_lastCategoryIndex = ((_currentCategoryIndex == -1) ? catIndex : _currentCategoryIndex);
		if (_lastCategoryIndex != -1)
		{
			_cachedCategoryItems[_lastCategoryIndex].ChangeState(state: false);
		}
		_currentCategoryIndex = catIndex;
		_cachedCategoryItems[catIndex].ChangeState(state: true);
		if ((bool)_currentSelectedLevel)
		{
			_currentSelectedLevel.OnSelected(selected: false);
		}
		LevelDataScript[] array = (from a in _levelsConfig.LevelsDatas
			where a.CategoryIndex == catIndex
			where !IsLevelCarBanned(a)
			select a).ToArray();
		foreach (LevelLoader cachedLevelItem in _cachedLevelItems)
		{
			cachedLevelItem.gameObject.SetActive(value: false);
		}
		for (int num = 0; num < array.Length; num++)
		{
			int idBefore = ((num > 0) ? array[num - 1].ID : (-1));
			if (_cachedLevelItems.Count > num)
			{
				_cachedLevelItems[num].SetActive(active: true);
				_cachedLevelItems[num].Init(array[num], this, _levelsProvider, idBefore, num);
			}
			else
			{
				LevelLoader levelLoader = _prefabFactory.Instantiate(PrefabLevel, LevelsParentTransform);
				_cachedLevelItems.Add(levelLoader);
				levelLoader.Init(array[num], this, _levelsProvider, idBefore, num);
			}
		}
		await ReloadRebuilder();
	}

	private async UniTask ReloadRebuilder()
	{
		await UniTask.Yield();
		if (this == null)
		{
			return;
		}
		RectTransform content = LevelssScrollRect.content;
		if (content == null)
		{
			return;
		}
		GridLayoutGroup component = content.GetComponent<GridLayoutGroup>();
		if (component == null)
		{
			return;
		}
		component.cellSize = UIHelper.CalculateGridElementWidth(LevelssScrollRect.viewport, component, CellCountInOneRow, 25);
		ContentSizeFitter.enabled = false;
		await UniTask.Yield();
		if (!(this == null))
		{
			LayoutRebuilder.ForceRebuildLayoutImmediate(content);
			ContentSizeFitter.enabled = true;
			if (FlagsStorage.IsLevelScene)
			{
				await SnapTo();
			}
		}
	}

	protected override void OnEnable()
	{
		BackButton.gameObject.SetActive(value: false);
		base.OnEnable();
	}

	private void UpdateOverAllProgress()
	{
		int levelsCount = _levelsCount;
		int num = 0;
		for (int i = 0; i <= levelsCount; i++)
		{
			if (ObscuredPrefs.HasKey($"bestResultID{LevelsConfig.IDOrderLevels[i]}"))
			{
				num++;
			}
		}
		if ((bool)TitleText)
		{
			TitleText.text = ((num >= levelsCount) ? ((string)new LocalizedString("LevelsMenuWindow.PerfectAllDoneHaveAdventureOnF")) : $"{num}/{levelsCount}");
		}
		Slider.value = Mathf.InverseLerp(0f, levelsCount, num);
		Slider.gameObject.SetActive(value: true);
	}

	public void OnLevelClick(LevelLoader levelLoader)
	{
		if (_currentSelectedLevel != null)
		{
			_currentSelectedLevel.OnSelected(selected: false);
		}
		levelLoader.OnSelected(selected: true);
		_currentSelectedLevel = levelLoader;
		if (!ScenesHelper.IsMenuSceneActive())
		{
			Close();
		}
	}

	private UniTask SnapTo()
	{
		if (LevelssScrollRect == null)
		{
			return UniTask.CompletedTask;
		}
		RectTransform content = LevelssScrollRect.content;
		if (content == null)
		{
			return UniTask.CompletedTask;
		}
		int childCount = content.childCount;
		if (childCount == 0)
		{
			return UniTask.CompletedTask;
		}
		float num = PlayerPrefs.GetFloat("lastPlayedLevelSibIndex", 0f);
		if (num >= (float)childCount)
		{
			num = childCount - 1;
		}
		float value = 1f - num / ((float)childCount - 1f);
		value = Mathf.Clamp01(value);
		LevelssScrollRect.verticalNormalizedPosition = value;
		return UniTask.CompletedTask;
	}
}

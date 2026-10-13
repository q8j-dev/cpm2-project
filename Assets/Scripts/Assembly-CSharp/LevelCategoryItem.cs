using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelCategoryItem : MonoBehaviour
{
	[SerializeField]
	private TextMeshProUGUI Title = default;

	[SerializeField]
	private Image BorderLine = default;

	[SerializeField]
	private CanvasGroup CanvasGroup = default;

	private Button _button;

	private LevelCategory _levelCategory;

	private LevelsMenuWindow _levelsMenuWindow;

	public void Init(LevelsMenuWindow levelsMenuWindow, LevelCategory levelCategory)
	{
		Title.text = levelCategory.name;
		if (!levelCategory.isActive)
		{
			CanvasGroup.alpha = 0.3f;
			CanvasGroup.interactable = false;
			return;
		}
		_levelsMenuWindow = levelsMenuWindow;
		_levelCategory = levelCategory;
		if (TryGetComponent<Button>(out _button))
		{
			_button.onClick.AddListener(OnClick);
		}
	}

	private void OnClick()
	{
		ParseCategoryAsync().Forget();
	}

	private async UniTask ParseCategoryAsync()
	{
		await _levelsMenuWindow.ParseCategory(_levelCategory.id);
	}

	public void ChangeState(bool state)
	{
		BorderLine.color = (state ? _levelsMenuWindow.SelectedColor : _levelsMenuWindow.NotSelectedColor);
	}
}

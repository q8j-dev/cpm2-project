using TMPro;
using UnityEngine;

public class TyreWidthUIElement : MonoBehaviour
{
	public RectTransform rect;

	[SerializeField]
	private new TextMeshProUGUI name = default;

	[SerializeField]
	private CanvasGroup canvasGroup;

	private static readonly Color WarningColor = new Color(1f, 0.83f, 0.15f);

	private string _baseText;

	private Color _defaultColor = Color.white;

	private bool _colorCached;

	public bool IsValid { get; private set; } = true;

	public bool IsWarning { get; private set; }

	private void Awake()
	{
		if (canvasGroup == null && !TryGetComponent<CanvasGroup>(out canvasGroup))
		{
			canvasGroup = base.gameObject.AddComponent<CanvasGroup>();
		}
		CacheDefaultColor();
	}

	private void CacheDefaultColor()
	{
		if (!_colorCached && !(name == null))
		{
			_defaultColor = name.color;
			_colorCached = true;
		}
	}

	public void Init(string text)
	{
		CacheDefaultColor();
		_baseText = text;
		if (name != null)
		{
			name.text = text;
		}
	}

	public void SetValid(bool valid)
	{
		IsValid = valid;
		IsWarning = false;
		if (canvasGroup != null)
		{
			canvasGroup.alpha = (valid ? 1f : 0.3f);
		}
		ApplyLabel(_baseText, _defaultColor);
	}

	public void SetWarning()
	{
		IsValid = false;
		IsWarning = true;
		if (canvasGroup != null)
		{
			canvasGroup.alpha = 1f;
		}
		ApplyLabel(_baseText, WarningColor);
	}

	private void ApplyLabel(string text, Color color)
	{
		if (!(name == null))
		{
			if (text != null)
			{
				name.text = text;
			}
			name.color = color;
		}
	}
}

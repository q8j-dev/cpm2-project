using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LockableUiToggleElement : Toggle
{
	public int Index;

	[SerializeField]
	private Image IconImage = default;

	[SerializeField]
	private TextMeshProUGUI TitleText = default;

	[Header("Lock")]
	[SerializeField]
	private float LockedAlphaValue = default;

	[SerializeField]
	private CanvasGroup PanelCanvasGroup = default;

	[SerializeField]
	private GameObject LockIcon = default;

	protected Action<int> _onTrue;

	protected Action<int> _onFalse;

	public void Init(int index, Action<int> onTrue, Action<int> onFalse = null)
	{
		Index = index;
		_onTrue = onTrue;
		_onFalse = onFalse;
		ReuseListeners();
	}

	protected void ReuseListeners()
	{
		onValueChanged.RemoveAllListeners();
		onValueChanged.AddListener(OnToggleChanged);
	}

	public void Lock(bool state)
	{
		base.interactable = !state;
		LockVisual(state);
	}

	public void LockVisual(bool state)
	{
		PanelCanvasGroup.alpha = (state ? LockedAlphaValue : 1f);
		LockIcon.SetActive(state);
	}

	public void SetSprite(Sprite sprite)
	{
		IconImage.sprite = sprite;
	}

	public void SetText(string text)
	{
		TitleText.text = text;
	}

	private void OnToggleChanged(bool isOn)
	{
		if (isOn)
		{
			_onTrue?.Invoke(Index);
		}
		else
		{
			_onFalse?.Invoke(Index);
		}
	}
}

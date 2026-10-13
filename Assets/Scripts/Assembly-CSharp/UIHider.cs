using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class UIHider : MonoBehaviour
{
	public const float FadeDuration = 0.5f;

	[SerializeField]
	private CanvasGroup[] CanvasGroups;

	private List<Tween> _tweens = new List<Tween>();

	private bool _tempCameraRotationDisablerPressed;

	private bool _buyCarBodyKitsUIPressed;

	private bool _oldFade;

	private void Start()
	{
		TempCameraRotationDisabler.OnPointerUpAction = (Action)Delegate.Combine(TempCameraRotationDisabler.OnPointerUpAction, new Action(OnBackgroundRelease));
		TempCameraRotationDisabler.OnPointerDownAction = (Action)Delegate.Combine(TempCameraRotationDisabler.OnPointerDownAction, new Action(OnBackgroundPress));
	}

	private void OnDestroy()
	{
		TempCameraRotationDisabler.OnPointerUpAction = (Action)Delegate.Remove(TempCameraRotationDisabler.OnPointerUpAction, new Action(OnBackgroundRelease));
		TempCameraRotationDisabler.OnPointerDownAction = (Action)Delegate.Remove(TempCameraRotationDisabler.OnPointerDownAction, new Action(OnBackgroundPress));
		for (int i = 0; i < _tweens.Count; i++)
		{
			_tweens[i]?.Kill();
		}
		_tweens.Clear();
	}

	private void OnBackgroundPress()
	{
		_tempCameraRotationDisablerPressed = true;
		UpdateFade();
	}

	private void OnBackgroundRelease()
	{
		_tempCameraRotationDisablerPressed = false;
		UpdateFade();
	}

	private void UpdateFade()
	{
		if (_tempCameraRotationDisablerPressed || _buyCarBodyKitsUIPressed)
		{
			SetFade(0f, 0.5f);
		}
		else
		{
			SetFade(1f, 0.5f);
		}
	}

	private void SetFade(float endValue, float duration)
	{
		bool flag = endValue < 0.5f;
		if (_oldFade == flag)
		{
			return;
		}
		_oldFade = flag;
		for (int i = 0; i < _tweens.Count; i++)
		{
			_tweens[i]?.Kill();
		}
		_tweens.Clear();
		CanvasGroup[] canvasGroups = CanvasGroups;
		foreach (CanvasGroup canvasGroup in canvasGroups)
		{
			if ((bool)canvasGroup)
			{
				canvasGroup.blocksRaycasts = endValue > 0.5f;
				_tweens.Add(canvasGroup.DOFade(endValue, duration));
			}
		}
	}
}

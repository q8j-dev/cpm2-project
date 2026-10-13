using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class AlphaHidableObject : MonoBehaviour
{
	public bool CantChangeAlpha;

	[SerializeField]
	private CanvasGroup CanvasGroup;

	private List<AlphaHidableObject> _dependObjectsCanvases = new List<AlphaHidableObject>();

	private Tween _tween;

	private void Start()
	{
		CanvasGroup = GetComponent<CanvasGroup>();
	}

	public void AddDependObject(GameObject dependObject)
	{
		if (dependObject.TryGetComponent<AlphaHidableObject>(out var component))
		{
			_dependObjectsCanvases.Add(component);
		}
	}

	public void ClearDependObjects()
	{
		_dependObjectsCanvases?.Clear();
	}

	public void SetAlphaWithoutFade(float alpha)
	{
		_tween?.Kill();
		CanvasGroup.alpha = alpha;
	}

	public void SetCanvasGroups(List<AlphaHidableObject> alphaHidableObject)
	{
		_dependObjectsCanvases = alphaHidableObject;
	}

	public void ChangeDependObjectsState(bool state, float value)
	{
		for (int i = 0; i < _dependObjectsCanvases.Count; i++)
		{
			_dependObjectsCanvases[i].CantChangeAlpha = state;
			_dependObjectsCanvases[i].SetAlphaWithoutFade(value);
		}
	}

	public void ChangeAlphaState(float value, float duration, float delay = 0f, bool notIgnoreDependObjects = false)
	{
		if (!CantChangeAlpha)
		{
			_tween?.Kill();
			ChangeDependObjectsState(notIgnoreDependObjects, value);
			_tween = CanvasGroup.DOFade(value, duration).SetDelay(delay).SetUpdate(isIndependentUpdate: true);
		}
	}

	private void OnDisable()
	{
		_tween?.Kill();
		_dependObjectsCanvases?.Clear();
		CantChangeAlpha = false;
	}
}

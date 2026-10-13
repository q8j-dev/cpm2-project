using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using _Car_Parking.Scripts.Racing_Scripts;
using _Car_Parking.Scripts.Rally.Data;

public class RallySequenceUIItem : MonoBehaviour
{
	public bool SkipHide;

	public RectTransform RectTransform;

	[SerializeField]
	private Image MainIcon;

	[SerializeField]
	private Image ProgressBar;

	[SerializeField]
	private Image LeftTopIcon;

	[SerializeField]
	private Image RightMiddleIcon;

	[SerializeField]
	private Image RightBottomIcon;

	[SerializeField]
	private Text DistanceText;

	[SerializeField]
	private Text DistanceMaskText;

	[SerializeField]
	private Image TextProgressBar;

	[SerializeField]
	private GameObject IconsParent;

	[SerializeField]
	private GameObject TextsParent;

	private Dictionary<Image, RallySequenceMark> _marksImageDictionary = new Dictionary<Image, RallySequenceMark>();

	private RallySeqenceTriggerLine _rallySeqenceTriggerLine;

	private RallySequenceItem _rallySequenceItem;

	private RallySequenceUIController _rallySequenceUIController;

	private Tween _tween;

	private float _currentFill = 1f;

	private CanvasGroup _canvasGroup;

	private RallyPaceNotesAssetsSO _rallyPaceNotesAssetsSo;

	public RallyLinkedSequenceParent RallyLinkedSequenceParent;

	public CanvasGroup CanvasGroup => _canvasGroup;

	public float CurrentFill => _currentFill;

	private void Start()
	{
		_canvasGroup = GetComponent<CanvasGroup>();
	}

	public void Init(RallySequenceItem rallySequenceItem, RallySeqenceTriggerLine rallySeqenceTriggerLine, RallySequenceUIController rallySequenceUIController, RallyPaceNotesAssetsSO rallyPaceNotesAssetsSo)
	{
		_rallySequenceItem = rallySequenceItem;
		_rallySeqenceTriggerLine = rallySeqenceTriggerLine;
		_rallySequenceUIController = rallySequenceUIController;
		_rallyPaceNotesAssetsSo = rallyPaceNotesAssetsSo;
		_rallySeqenceTriggerLine.ProgressTick += SetProgress;
		RallySeqenceTriggerLine rallySeqenceTriggerLine2 = _rallySeqenceTriggerLine;
		rallySeqenceTriggerLine2.OnActivated = (Action<bool>)Delegate.Combine(rallySeqenceTriggerLine2.OnActivated, new Action<bool>(OnSequenceActivated));
		ProgressBar.fillAmount = 0f;
		TextProgressBar.fillAmount = 0f;
		_currentFill = 0f;
		SetAssets().Forget();
		SetMarks().Forget();
	}

	private async UniTaskVoid SetAssets()
	{
		bool flag = _rallySequenceItem.SequenceTurnType == RallySequenceTurnType.None;
		TextsParent.SetActive(flag);
		IconsParent.SetActive(!flag);
		if (flag)
		{
			string text = ((int)(Math.Round((double)Mathf.Clamp(_rallySequenceItem.Distance, 20f, 250f) / 10.0) * 10.0)).ToString();
			DistanceText.text = text;
			DistanceMaskText.text = text;
		}
		else
		{
			Sprite turnIcon = _rallyPaceNotesAssetsSo.GetTurnIcon(_rallySequenceItem.SequenceTurnType);
			MainIcon.sprite = turnIcon;
			ProgressBar.sprite = turnIcon;
		}
	}

	private async UniTaskVoid SetMarks()
	{
		for (int i = 0; i < _rallySequenceItem.Mark.Length; i++)
		{
			Sprite markIcon = _rallyPaceNotesAssetsSo.GetMarkIcon(_rallySequenceItem.Mark[i]);
			if (!markIcon)
			{
				continue;
			}
			Image markPlace = GetMarkPlace(_rallySequenceItem.Mark[i]);
			if (_marksImageDictionary.ContainsKey(markPlace))
			{
				if (_marksImageDictionary[markPlace] > _rallySequenceItem.Mark[i])
				{
					_marksImageDictionary[markPlace] = _rallySequenceItem.Mark[i];
					markPlace.sprite = markIcon;
				}
			}
			else
			{
				_marksImageDictionary.Add(markPlace, _rallySequenceItem.Mark[i]);
				markPlace.sprite = markIcon;
			}
			markPlace.gameObject.SetActive(value: true);
		}
	}

	private Image GetMarkPlace(RallySequenceMark markType)
	{
		switch (markType)
		{
		case RallySequenceMark.Caution:
		case RallySequenceMark.DontCut:
		case RallySequenceMark.Maybe:
			return LeftTopIcon;
		case RallySequenceMark.Opens:
		case RallySequenceMark.Tightens:
			return RightMiddleIcon;
		default:
			return RightBottomIcon;
		}
	}

	public void ResetProgress()
	{
		_currentFill = 0f;
		if ((bool)ProgressBar)
		{
			ProgressBar.fillAmount = 0f;
		}
		if ((bool)TextProgressBar)
		{
			TextProgressBar.fillAmount = 0f;
		}
	}

	public void SetProgress(float progress)
	{
		if ((bool)ProgressBar && (bool)TextProgressBar)
		{
			float b = Mathf.Abs(progress);
			_currentFill = Mathf.Lerp(_currentFill, b, Time.deltaTime * 8f);
			ProgressBar.fillAmount = _currentFill;
			TextProgressBar.fillAmount = _currentFill;
		}
	}

	private void OnSequenceActivated(bool state)
	{
		if ((bool)this)
		{
			Snap(state);
			if (state)
			{
				_rallySequenceUIController.OnSequenceActivated(RectTransform, _rallySequenceItem);
			}
		}
	}

	private void Snap(bool snapState)
	{
		if (!this)
		{
			return;
		}
		_tween?.Kill();
		_tween = base.transform.DOScale(snapState ? 1.2f : 1f, 0.5f).SetLink(base.gameObject).OnComplete(() =>
		{
			if ((bool)this && !snapState)
			{
				if (!SkipHide)
				{
					_canvasGroup.alpha = 0f;
				}
				if ((bool)RallyLinkedSequenceParent)
				{
					RallyLinkedSequenceParent.CanvasGroup.alpha = 0f;
				}
			}
		});
	}

	private void OnDestroy()
	{
		_tween?.Kill();
		if ((bool)_rallySeqenceTriggerLine)
		{
			_rallySeqenceTriggerLine.ProgressTick -= SetProgress;
			RallySeqenceTriggerLine rallySeqenceTriggerLine = _rallySeqenceTriggerLine;
			rallySeqenceTriggerLine.OnActivated = (Action<bool>)Delegate.Remove(rallySeqenceTriggerLine.OnActivated, new Action<bool>(OnSequenceActivated));
		}
	}

	public void SetParentRectTransform()
	{
		RectTransform = base.transform.parent.GetComponent<RectTransform>();
	}
}

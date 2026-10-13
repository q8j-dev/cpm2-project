using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Rally.Data;
using _Car_Parking.Scripts.UIElement;

namespace _Car_Parking.Scripts.Racing_Scripts
{
	public class RallySequenceUIController : MonoBehaviour
	{
		[SerializeField]
		private RectTransform SequenceParent;

		[SerializeField]
		private RallySequenceUIItem RallySequenceItemPrefab;

		[SerializeField]
		private RallyLinkedSequenceParent SequenceLinkPrefab;

		[SerializeField]
		private ScrollRect SequenceParentScrollRect;

		[SerializeField]
		private RallySequenceUIItem StopIcon;

		[SerializeField]
		private RallyPaceNotesAssetsSO RallyPaceNotesAssetsSO;

		[SerializeField]
		private CanvasGroup CanvasGroup;

		private IPrefabFactory _prefabFactory;

		private RallyTriggersControl _triggersControl;

		private Tween _tween;

		private RallySequenceData _rallySequenceData;

		private bool _isDestroyed;

		private CancellationTokenSource _stopIconCts;

		private bool _isRaceStarted;

		private float _initialDistanceToMarshal;

		private Vector3 _marshalTargetPosition;

		private bool _stopIconTracking;

		public Action<RallySequenceItem> OnNoteActivated;

		public float StopIconFill
		{
			get
			{
				if (!_stopIconTracking || !(StopIcon != null))
				{
					return -1f;
				}
				return StopIcon.CurrentFill;
			}
		}

		[Inject]
		[UnityEngine.Scripting.Preserve]
		public void Construct(IPrefabFactory prefabFactory)
		{
			_prefabFactory = prefabFactory;
		}

		public void Init(RallyTriggersControl rallyTriggersControl, RallySequenceData rallySequenceData)
		{
			_triggersControl = rallyTriggersControl;
			_rallySequenceData = rallySequenceData;
			CreateSequence().Forget();
			ShowSequence(show: false);
		}

		public void Show(bool state)
		{
			CanvasGroup.alpha = (state ? 1f : 0f);
		}

		public void ScrollToStart()
		{
			SequenceParentScrollRect.horizontalNormalizedPosition = 0f;
		}

		public void ShowSequence(bool show)
		{
			SequenceParent.gameObject.SetActive(show);
		}

		public void OnSequenceActivated(RectTransform rectTransform, RallySequenceItem item)
		{
			OnNoteActivated?.Invoke(item);
			RectTransform target = rectTransform;
			if (rectTransform.parent != null && rectTransform.parent != SequenceParent.transform && rectTransform.parent is RectTransform rectTransform2)
			{
				target = rectTransform2;
			}
			ScrollToCenter(target).Forget();
		}

		private async UniTaskVoid ScrollToCenter(RectTransform target)
		{
			await UniTask.Yield();
			if (_isDestroyed || SequenceParentScrollRect == null)
			{
				return;
			}
			float width = SequenceParent.rect.width;
			float width2 = SequenceParentScrollRect.viewport.rect.width;
			if (width <= width2)
			{
				return;
			}
			Vector3 position = target.TransformPoint(new Vector3(target.rect.center.x, 0f, 0f));
			float num = SequenceParent.InverseTransformPoint(position).x - SequenceParent.rect.xMin;
			float targetNormalized = Mathf.Clamp01((num - width2 * 0.5f) / (width - width2));
			float startNormalized = SequenceParentScrollRect.horizontalNormalizedPosition;
			float elapsed = 0f;
			while (elapsed < 0.5f)
			{
				elapsed += Time.unscaledDeltaTime;
				SequenceParentScrollRect.horizontalNormalizedPosition = Mathf.Lerp(startNormalized, targetNormalized, elapsed / 0.5f);
				await UniTask.Yield();
				if (_isDestroyed || SequenceParentScrollRect == null)
				{
					return;
				}
			}
			if (!_isDestroyed && !(SequenceParentScrollRect == null))
			{
				SequenceParentScrollRect.horizontalNormalizedPosition = targetNormalized;
			}
		}

		private async UniTaskVoid CreateSequence()
		{
			if (_rallySequenceData == null)
			{
				return;
			}
			CancellationToken ct = this.GetCancellationTokenOnDestroy();
			await UniTask.WaitUntil(() => _triggersControl.TriggerList.Count >= _rallySequenceData.RallySequenceItems.Count, PlayerLoopTiming.Update, ct);
			List<RallySeqenceTriggerLine> triggerLines = _triggersControl.TriggerList;
			for (int i = 0; i < triggerLines.Count; i++)
			{
				RallySequenceUIItem item = _prefabFactory.Instantiate(RallySequenceItemPrefab, SequenceParent);
				item.Init(_rallySequenceData.RallySequenceItems[i], triggerLines[i], this, RallyPaceNotesAssetsSO);
				await UniTask.Yield(ct);
				if (_rallySequenceData.RallySequenceItems[i].Link != RallySequenceLink.None)
				{
					RallyLinkedSequenceParent rallyLinkedSequenceParent = _prefabFactory.Instantiate(SequenceLinkPrefab, SequenceParent);
					rallyLinkedSequenceParent.link.sprite = RallyPaceNotesAssetsSO.GetLinkIcon(_rallySequenceData.RallySequenceItems[i].Link);
					item.transform.SetParent(rallyLinkedSequenceParent.transform);
					item.SkipHide = true;
					item.transform.SetSiblingIndex(rallyLinkedSequenceParent.LeftPlace.GetSiblingIndex());
					item.SetParentRectTransform();
					item.RallyLinkedSequenceParent = rallyLinkedSequenceParent;
				}
			}
			await UIHelper.DelayedRebuildLayout(SequenceParentScrollRect.content);
			if (!_isDestroyed)
			{
				ScrollToStart();
			}
		}

		public void OnRaceStarted()
		{
			_isRaceStarted = true;
			_initialDistanceToMarshal = 0f;
			StopIcon.ResetProgress();
		}

		public void RestartStopIconProgress()
		{
			_initialDistanceToMarshal = 0f;
			StopIcon.ResetProgress();
		}

		public void ShowStopIcon(bool state)
		{
			_tween?.Kill();
			_tween = StopIcon.CanvasGroup.DOFade(state ? 1 : 0, 0.5f);
			_stopIconCts?.Cancel();
			_stopIconCts?.Dispose();
			_stopIconCts = null;
			_stopIconTracking = state;
			if (state)
			{
				if (_initialDistanceToMarshal <= 0f)
				{
					StopIcon.ResetProgress();
				}
				_stopIconCts = new CancellationTokenSource();
				UpdateStopIconProgress(_stopIconCts.Token).Forget();
			}
		}

		private static float FlatDistance(Vector3 a, Vector3 b)
		{
			float num = a.x - b.x;
			float num2 = a.z - b.z;
			return Mathf.Sqrt(num * num + num2 * num2);
		}

		private void CaptureInitialMarshalDistance()
		{
			if (!(V.Car == null) && !(_triggersControl == null))
			{
				Transform transform = ((!_isRaceStarted) ? _triggersControl.StartMarshall?.transform : _triggersControl.FinishMarshall?.transform);
				if (!(transform == null))
				{
					_marshalTargetPosition = transform.position;
					float a = FlatDistance(V.Car.transform.position, _marshalTargetPosition);
					_initialDistanceToMarshal = Mathf.Max(a, 1f);
				}
			}
		}

		private async UniTaskVoid UpdateStopIconProgress(CancellationToken ct)
		{
			while (!ct.IsCancellationRequested && !_isDestroyed)
			{
				if (V.Car != null && _triggersControl != null)
				{
					if (_initialDistanceToMarshal <= 0f)
					{
						CaptureInitialMarshalDistance();
					}
					if (((!_isRaceStarted) ? _triggersControl.StartMarshall?.transform : _triggersControl.FinishMarshall?.transform) != null && _initialDistanceToMarshal > 0f)
					{
						float num = FlatDistance(V.Car.transform.position, _marshalTargetPosition);
						float num2 = Mathf.Max(_initialDistanceToMarshal - 6f, 0.1f);
						float progress = 1f - Mathf.Clamp01((num - 6f) / num2);
						StopIcon.SetProgress(progress);
					}
				}
				await UniTask.Yield(PlayerLoopTiming.Update, ct);
			}
		}

		private void OnDestroy()
		{
			_isDestroyed = true;
			_stopIconCts?.Cancel();
			_stopIconCts?.Dispose();
		}
	}
}

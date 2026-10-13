using DG.Tweening;
using UnityEngine;

namespace NewUI.Scripts1.Elements
{
	public class CanvasGroupStateTweener : MonoBehaviour
	{
		public float DurationFadeEnd = 0.5f;

		public float DurationFadeStart = 0.1f;

		public bool UpdateCanvasSettings = true;

		private bool _state;

		private bool _startState;

		private CanvasGroup _canvasGroup;

		private Tween _tween;

		private void Awake()
		{
			TryGetComponent<CanvasGroup>(out _canvasGroup);
			_state = _canvasGroup.alpha == 1f;
			_startState = _canvasGroup.alpha == 1f;
			UpdateState();
		}

		private void OnDestroy()
		{
			_tween?.Kill();
		}

		public void OnSideStateChanging()
		{
		}

		public void ChangeState(bool state)
		{
			_state = state;
			UpdateState();
		}

		public void ChangeState()
		{
			_state = !_state;
			UpdateState();
		}

		private void UpdateState()
		{
			_tween?.Kill();
			_tween = _canvasGroup.DOFade(_state ? 1 : 0, _state ? DurationFadeStart : DurationFadeEnd).SetUpdate(isIndependentUpdate: true);
			if (UpdateCanvasSettings)
			{
				_canvasGroup.interactable = _state;
				_canvasGroup.blocksRaycasts = _state;
			}
		}

		private void OnDisable()
		{
			ResetTween();
		}

		public void ResetTween()
		{
			_tween?.Kill();
			_state = _startState;
			_canvasGroup.alpha = (_startState ? 1 : 0);
			_canvasGroup.interactable = _startState;
			_canvasGroup.blocksRaycasts = _startState;
		}
	}
}

using DG.Tweening;
using UnityEngine;

namespace _Car_Parking.Scripts.Other.Helpers
{
	public class MoveToTransformByTween : MonoBehaviour
	{
		private Tween _tween;

		private Vector3 _startPosition;

		private float _progress;

		public void Move(Transform target, float duration, Ease ease, TweenCallback onComplete = null)
		{
			_tween?.Kill();
			_progress = 0f;
			_startPosition = base.transform.position;
			if (duration > 0f)
			{
				_tween = DOTween.To(() => _progress, (float x) =>
				{
					_progress = x;
				}, 1f, duration).SetEase(ease).OnUpdate(() =>
				{
					if ((bool)target)
					{
						base.transform.position = Vector3.Lerp(_startPosition, target.position, _progress);
					}
				})
					.OnComplete(() =>
					{
						OnMoveCompleted(target, onComplete);
					});
			}
			else
			{
				OnMoveCompleted(target, onComplete);
			}
		}

		private void OnMoveCompleted(Transform target, TweenCallback onComplete)
		{
			if ((bool)target)
			{
				base.transform.position = target.position;
			}
			onComplete?.Invoke();
		}

		private void OnDestroy()
		{
			_tween?.Kill();
		}
	}
}

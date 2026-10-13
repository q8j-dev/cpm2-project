using DG.Tweening;
using UnityEngine;

namespace _Car_Parking.Scripts.Other.Helpers
{
	public class MoverByTween : MonoBehaviour
	{
		private Tween _tween;

		private Vector3 _defaultPosition;

		private void Awake()
		{
			_defaultPosition = base.transform.position;
		}

		public void MoveToDefault(float duration, Ease ease, bool isLocal)
		{
			Move(_defaultPosition, duration, ease, isLocal);
		}

		public void MoveToZero(float duration, Ease ease, bool isLocal)
		{
			Move(Vector3.zero, duration, ease, isLocal);
		}

		public void Move(Vector3 position, float duration, Ease ease, bool isLocal)
		{
			_tween?.Kill();
			if (isLocal)
			{
				_tween = base.transform.DOLocalMove(position, duration).SetEase(ease).SetUpdate(isIndependentUpdate: true);
			}
			else
			{
				_tween = base.transform.DOMove(position, duration).SetEase(ease).SetUpdate(isIndependentUpdate: true);
			}
		}

		private void OnDisable()
		{
			_tween?.Kill();
		}
	}
}

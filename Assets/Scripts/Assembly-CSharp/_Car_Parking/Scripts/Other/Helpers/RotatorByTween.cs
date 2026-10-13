using DG.Tweening;
using UnityEngine;

namespace _Car_Parking.Scripts.Other.Helpers
{
	public class RotatorByTween : MonoBehaviour
	{
		private Tween _tween;

		public void RotateToZero(float duration, Ease ease, bool isLocal)
		{
			Rotate(Vector3.zero, duration, ease, isLocal);
		}

		public void Rotate(Vector3 rotation, float duration, Ease ease, bool isLocal)
		{
			_tween?.Kill();
			if (isLocal)
			{
				_tween = base.transform.DOLocalRotate(rotation, duration).SetEase(ease).SetUpdate(isIndependentUpdate: true);
			}
			else
			{
				_tween = base.transform.DORotate(rotation, duration).SetEase(ease).SetUpdate(isIndependentUpdate: true);
			}
		}

		private void OnDisable()
		{
			_tween?.Kill();
		}
	}
}

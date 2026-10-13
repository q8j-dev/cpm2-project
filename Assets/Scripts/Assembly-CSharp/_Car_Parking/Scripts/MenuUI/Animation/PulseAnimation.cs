using DG.Tweening;
using UnityEngine;

namespace _Car_Parking.Scripts.MenuUI.Animation
{
	public class PulseAnimation : MonoBehaviour
	{
		[SerializeField]
		private float ScaleMultiplier = 1.2f;

		[SerializeField]
		private float Duration = 0.5f;

		private RectTransform _target;

		private Tween _pulseTween;

		private void Awake()
		{
			if (_target == null)
			{
				_target = GetComponent<RectTransform>();
			}
			Vector3 localScale = _target.localScale;
			_pulseTween = _target.DOScale(localScale * ScaleMultiplier, Duration).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
		}

		private void OnDestroy()
		{
			_pulseTween?.Kill();
		}
	}
}

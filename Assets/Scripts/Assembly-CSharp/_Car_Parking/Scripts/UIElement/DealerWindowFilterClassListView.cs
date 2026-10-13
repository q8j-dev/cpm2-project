using DG.Tweening;
using NewUI.Scripts1.Elements;
using UniRx;
using UnityEngine;
using _Car_Parking.Scripts.Other.Helpers;

namespace _Car_Parking.Scripts.UIElement
{
	public class DealerWindowFilterClassListView : MonoBehaviour
	{
		private const float k_AnimationOpenKof = 0.1f;

		private const float k_AnimationCloseKof = 1.5f;

		[SerializeField]
		private CanvasGroupStateTweener CanvasGroupStateTweener;

		[SerializeField]
		private Transform ClosePosition;

		[SerializeField]
		private MoverByTween[] Items;

		private void Start()
		{
			CanvasGroupStateTweener.DurationFadeStart = 0.05f;
			CanvasGroupStateTweener.DurationFadeEnd = 0.75f;
			Observable.NextFrame().Subscribe((Unit _) =>
			{
				UpdateVisible(isVisible: false);
			}).AddTo(base.gameObject);
		}

		public void UpdateVisible(bool isVisible)
		{
			CanvasGroupStateTweener.ChangeState(isVisible);
			for (int i = 0; i < Items.Length; i++)
			{
				float duration = Mathf.Lerp(0f, 0.5f, Mathf.InverseLerp(0f, Items.Length - 1, i));
				if (isVisible)
				{
					Items[i].MoveToZero(duration, Ease.OutSine, isLocal: true);
				}
				else
				{
					Items[i].Move(ClosePosition.position, duration, Ease.OutSine, isLocal: false);
				}
			}
		}
	}
}

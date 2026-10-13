using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using _Car_Parking.Scripts.Audio;

namespace _Car_Parking.Scripts.EventSystem.View
{
	public class ProgressSlider : MonoBehaviour
	{
		[SerializeField]
		private Ease LinesEase = Ease.OutQuad;

		[SerializeField]
		private List<Slider> Lines;

		[SerializeField]
		private List<Image> FillSlidersImages;

		[SerializeField]
		private List<Image> Knobs;

		[SerializeField]
		private Color CompleteColor;

		private Sequence _progressSeq;

		public async UniTask SetProgress(int from, int to, bool withAnimation)
		{
			if (!withAnimation)
			{
				SetWithoutAnimation(to);
				return;
			}
			SetWithoutAnimation(from);
			await Animate(from, to);
		}

		private async UniTask Animate(int from, int to)
		{
			_progressSeq?.Kill();
			Sequence sequence = (_progressSeq = DOTween.Sequence().SetLink(base.gameObject, LinkBehaviour.KillOnDestroy));
			for (int i = from; i < to; i++)
			{
				int index = i;
				float num = (float)(index + 1) * 2.4f;
				FillSlidersImages[index].color = CompleteColor;
				sequence.AppendCallback(() =>
				{
					SoundEventsDataHelper.PlayUISound(AudioWrapper.GetId("UI_Event_ProgressBar_" + (index + 1)), base.gameObject);
				});
				sequence.Append(Lines[index].DOValue(1f, 0.7f).SetEase(LinesEase));
				sequence.Append(DOTween.Sequence().SetLink(base.gameObject, LinkBehaviour.KillOnDestroy).Join(Knobs[index].DOColor(CompleteColor, 0.5f).SetEase(Ease.OutQuad))
					.Join(Knobs[index].transform.DOPunchScale(Vector3.one * (0.1f * num), 0.5f, 1, 0.5f)));
			}
			await sequence.AsyncWaitForCompletion();
		}

		private void OnDestroy()
		{
			_progressSeq?.Kill();
		}

		private void SetWithoutAnimation(int to)
		{
			for (int i = 0; i < to; i++)
			{
				FillSlidersImages[i].color = CompleteColor;
				Lines[i].value = Lines[i].maxValue;
				Knobs[i].color = CompleteColor;
			}
		}
	}
}

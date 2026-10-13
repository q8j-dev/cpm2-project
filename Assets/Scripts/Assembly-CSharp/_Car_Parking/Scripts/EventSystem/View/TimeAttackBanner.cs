using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Car_Parking.Scripts.EventSystem.View
{
	public class TimeAttackBanner : TimeAttackBannerBase
	{
		[SerializeField]
		private ProgressSlider ProgressSlider;

		protected override async UniTask SetProgress(int cachedProgress, CancellationToken token)
		{
			await base.SetProgress(cachedProgress, token);
			await ProgressSlider.SetProgress(cachedProgress, base.Event.Progress, cachedProgress != base.Event.Progress).AttachExternalCancellation(token);
		}
	}
}

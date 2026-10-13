using System;
using System.Threading;
using CarAnimationScripts;
using Cysharp.Threading.Tasks;

namespace CarInteractionAnimationsScripts
{
	public class PlayOpenOutsideAnimationAction : IAction, IDisposable
	{
		private PersonAnimationsController _personAnimationsController;

		private SitGetOutData _sitGetOutData;

		private CarInteractionAnimationsCache _animationCache;

		private AnimationService _animationService;

		public PlayOpenOutsideAnimationAction(PersonAnimationsController personAnimationsController)
		{
			_sitGetOutData = personAnimationsController.SitGetOutCarSystem.SitGetOutData;
			_animationCache = personAnimationsController.SitGetOutCarSystem.AnimationCache;
			_animationService = personAnimationsController.AnimationService;
			_personAnimationsController = personAnimationsController;
		}

		public void Dispose()
		{
		}

		public async UniTask Run(CarAnimationsController carAnimController, CancellationToken cancellationToken)
		{
			_sitGetOutData.IsKitPoseSuppressed = true;
			CheckerStopAction checkerStopAction = new CheckerStopAction();
			if (!_sitGetOutData.IsCurrentDoorWasOpened)
			{
				float animLength = await _animationService.PlayCarInteractionAnimation(CarAnimationType.OpenOutside, _sitGetOutData.CurrentPlace, carAnimController, _animationCache, cancellationToken);
				float delay = _animationService.GetDelayToOpenCloseDoor(carAnimController, CarAnimationType.OpenOutside, _sitGetOutData.CurrentPlace);
				delay = Math.Max(0f, delay);
				await UniTask.Delay(TimeSpan.FromSeconds(delay), ignoreTimeScale: false, PlayerLoopTiming.Update, cancellationToken).SuppressCancellationThrow();
				if (!checkerStopAction.CheckStop(_personAnimationsController, carAnimController, cancellationToken))
				{
					carAnimController.OpenCloseCarElement((CarDoorType)_sitGetOutData.CurrentPlace, isOpen: true);
					await UniTask.Delay(TimeSpan.FromSeconds(Math.Max(0f, animLength - delay)), ignoreTimeScale: false, PlayerLoopTiming.Update, cancellationToken).SuppressCancellationThrow();
				}
			}
		}
	}
}

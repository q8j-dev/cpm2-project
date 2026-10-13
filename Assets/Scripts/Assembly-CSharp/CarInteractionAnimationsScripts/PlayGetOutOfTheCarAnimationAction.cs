using System;
using System.Threading;
using CarAnimationScripts;
using Cysharp.Threading.Tasks;

namespace CarInteractionAnimationsScripts
{
	public class PlayGetOutOfTheCarAnimationAction : IAction, IDisposable
	{
		private PersonAnimationsController _personAnimationsController;

		private SitGetOutData _sitGetOutData;

		private CarInteractionAnimationsCache _animationCache;

		private AnimationService _animationService;

		public PlayGetOutOfTheCarAnimationAction(PersonAnimationsController personAnimationsController)
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
			CheckerStopAction checkerStopAction = new CheckerStopAction();
			_personAnimationsController.AnimationService.EnableDisableIk(isOn: false);
			float animLength = await _animationService.PlayCarInteractionAnimation(CarAnimationType.GetOutOfTheCar, _sitGetOutData.CurrentPlace, carAnimController, _animationCache, cancellationToken);
			if (checkerStopAction.CheckStop(_personAnimationsController, carAnimController, cancellationToken))
			{
				return;
			}
			if ((bool)carAnimController.CarAnimationReferences.CarWithoutDoorsBodyKit)
			{
				await UniTask.Delay(TimeSpan.FromSeconds(animLength), ignoreTimeScale: false, PlayerLoopTiming.Update, cancellationToken).SuppressCancellationThrow();
				return;
			}
			float delay = _animationService.GetDelayToOpenCloseDoor(carAnimController, CarAnimationType.GetOutOfTheCar, _sitGetOutData.CurrentPlace);
			delay = Math.Max(delay, 0f);
			await UniTask.Delay(TimeSpan.FromSeconds(delay), ignoreTimeScale: false, PlayerLoopTiming.Update, cancellationToken).SuppressCancellationThrow();
			if (!checkerStopAction.CheckStop(_personAnimationsController, carAnimController, cancellationToken))
			{
				carAnimController.OpenCloseCarElement((CarDoorType)_sitGetOutData.CurrentPlace, isOpen: false);
				await UniTask.Delay(TimeSpan.FromSeconds(Math.Max(animLength - delay, 0f)), ignoreTimeScale: false, PlayerLoopTiming.Update, cancellationToken).SuppressCancellationThrow();
			}
		}
	}
}

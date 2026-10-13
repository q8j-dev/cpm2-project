using System;
using System.Threading;
using CarAnimationScripts;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CarInteractionAnimationsScripts
{
	public class PlayOpenInsideAnimationAction : IAction, IDisposable
	{
		private PersonAnimationsController _personAnimationsController;

		private SitGetOutData _sitGetOutData;

		private CarInteractionAnimationsCache _animationCache;

		private GameObject _person;

		private AnimationService _animationService;

		public PlayOpenInsideAnimationAction(PersonAnimationsController personAnimationsController)
		{
			_sitGetOutData = personAnimationsController.SitGetOutCarSystem.SitGetOutData;
			_animationCache = personAnimationsController.SitGetOutCarSystem.AnimationCache;
			_animationService = personAnimationsController.AnimationService;
			_personAnimationsController = personAnimationsController;
			_person = personAnimationsController.transform.parent.gameObject;
		}

		public void Dispose()
		{
		}

		public async UniTask Run(CarAnimationsController carAnimController, CancellationToken cancellationToken)
		{
			CheckerStopAction checkerStopAction = new CheckerStopAction();
			float animLength = await _animationService.PlayCarInteractionAnimation(CarAnimationType.OpenInside, _sitGetOutData.CurrentPlace, carAnimController, _animationCache, cancellationToken);
			if (!checkerStopAction.CheckStop(_personAnimationsController, carAnimController, cancellationToken))
			{
				float delay = _animationService.GetDelayToOpenCloseDoor(carAnimController, CarAnimationType.OpenInside, _sitGetOutData.CurrentPlace);
				delay = Mathf.Max(delay, 0f);
				await UniTask.Delay(TimeSpan.FromSeconds(delay), ignoreTimeScale: false, PlayerLoopTiming.Update, cancellationToken).SuppressCancellationThrow();
				if (!checkerStopAction.CheckStop(_personAnimationsController, carAnimController, cancellationToken) && !_sitGetOutData.IsCurrentDoorWasOpened && !carAnimController.CarAnimationReferences.CarWithoutDoorsBodyKit)
				{
					carAnimController.OpenCloseCarElement((CarDoorType)_sitGetOutData.CurrentPlace, isOpen: true);
					_animationService.EnableDisableIk(isOn: false);
					delay = Mathf.Max(animLength - delay, 0f);
					await UniTask.Delay(TimeSpan.FromSeconds(delay), ignoreTimeScale: false, PlayerLoopTiming.Update, cancellationToken).SuppressCancellationThrow();
				}
			}
		}
	}
}

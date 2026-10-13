using System;
using System.Threading;
using CarAnimationScripts;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CarInteractionAnimationsScripts
{
	public class PlaySeatIntoCarAnimationAction : IAction, IDisposable
	{
		private PersonAnimationsController _personAnimationsController;

		private SitGetOutData _sitGetOutData;

		private CarInteractionAnimationsCache _animationCache;

		private GameObject _person;

		private AnimationService _animationService;

		private const float KitPoseSuppressLead = 0.2f;

		public PlaySeatIntoCarAnimationAction(PersonAnimationsController personAnimationsController)
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
			_sitGetOutData.IsKitPoseSuppressed = false;
			CheckerStopAction checkerStopAction = new CheckerStopAction();
			float animLength = await _animationService.PlayCarInteractionAnimation(CarAnimationType.SeatIntoCarOutside, _sitGetOutData.CurrentPlace, carAnimController, _animationCache, cancellationToken);
			if ((bool)carAnimController.CarAnimationReferences.CarWithoutDoorsBodyKit)
			{
				await UniTask.Delay(TimeSpan.FromSeconds(animLength), ignoreTimeScale: false, PlayerLoopTiming.Update, cancellationToken).SuppressCancellationThrow();
			}
			else
			{
				float delay = _animationService.GetDelayToOpenCloseDoor(carAnimController, CarAnimationType.SeatIntoCarOutside, _sitGetOutData.CurrentPlace);
				delay = Mathf.Max(delay, 0f);
				float beforeSuppress = Mathf.Max(delay - 0.2f, 0f);
				await UniTask.Delay(TimeSpan.FromSeconds(beforeSuppress), ignoreTimeScale: false, PlayerLoopTiming.Update, cancellationToken).SuppressCancellationThrow();
				_sitGetOutData.IsKitPoseSuppressed = true;
				await UniTask.Delay(TimeSpan.FromSeconds(delay - beforeSuppress), ignoreTimeScale: false, PlayerLoopTiming.Update, cancellationToken).SuppressCancellationThrow();
				if (checkerStopAction.CheckStop(_personAnimationsController, carAnimController, cancellationToken))
				{
					return;
				}
				carAnimController.OpenCloseCarElement((CarDoorType)_sitGetOutData.CurrentPlace, isOpen: false);
				await UniTask.Delay(TimeSpan.FromSeconds(Mathf.Max(animLength - delay, 0f)), ignoreTimeScale: false, PlayerLoopTiming.Update, cancellationToken).SuppressCancellationThrow();
			}
			if (!checkerStopAction.CheckStop(_personAnimationsController, carAnimController, cancellationToken))
			{
				if (carAnimController.TryGetComponent<DriverController>(out var component))
				{
					component.PreSetPerson(_person, _sitGetOutData.CurrentPlace);
				}
				_animationService.EnableDisableIk(isOn: true);
				await UniTask.Delay(TimeSpan.FromSeconds(0.20000000298023224), ignoreTimeScale: false, PlayerLoopTiming.Update, cancellationToken).SuppressCancellationThrow();
			}
		}
	}
}

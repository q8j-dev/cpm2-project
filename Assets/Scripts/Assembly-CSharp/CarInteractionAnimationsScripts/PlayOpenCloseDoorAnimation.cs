using System;
using System.Threading;
using CarAnimationScripts;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CarInteractionAnimationsScripts
{
	public class PlayOpenCloseDoorAnimation : IAction, IDisposable
	{
		private PersonAnimationsController _personAnimationsController;

		private SitGetOutData _sitGetOutData;

		private CarInteractionAnimationsCache _animationCache;

		private GameObject _person;

		private AnimationService _animationService;

		private CarAnimationType _carAnimationType;

		private CancellationTokenSource _personPositionUpdateCts;

		private CancellationTokenSource _fullPersonPositionUpdateCts;

		public PlayOpenCloseDoorAnimation(PersonAnimationsController personAnimationsController, CarAnimationType carAnimationType)
		{
			_sitGetOutData = personAnimationsController.SitGetOutCarSystem.SitGetOutData;
			_animationCache = personAnimationsController.SitGetOutCarSystem.AnimationCache;
			_animationService = personAnimationsController.AnimationService;
			_personAnimationsController = personAnimationsController;
			_carAnimationType = carAnimationType;
			_person = personAnimationsController.transform.parent.gameObject;
		}

		public void Dispose()
		{
			_personPositionUpdateCts?.Dispose();
			_fullPersonPositionUpdateCts?.Dispose();
		}

		public async UniTask Run(CarAnimationsController carAnimController, CancellationToken cancellationToken)
		{
			_sitGetOutData.IsKitPoseSuppressed = true;
			ClearCts();
			CheckerStopAction checkerStopAction = new CheckerStopAction();
			CarAnimationType carAnimationType = _carAnimationType;
			if ((carAnimationType == CarAnimationType.OpenInside || carAnimationType == CarAnimationType.CloseInside) && (bool)carAnimController.CarAnimationsPoints && carAnimController.CarAnimationsPoints.TryGetSeatPoint((CarDoorType)_sitGetOutData.CurrentPlace, out var seatPoint))
			{
				_person.transform.position = seatPoint.position;
				_personPositionUpdateCts = new CancellationTokenSource();
				_fullPersonPositionUpdateCts = CancellationTokenSource.CreateLinkedTokenSource(_personPositionUpdateCts.Token, cancellationToken);
				UpdatePersonPositionAsync(seatPoint, _fullPersonPositionUpdateCts.Token).Forget();
			}
			float animLength = await _animationService.PlayCarInteractionAnimation(_carAnimationType, _sitGetOutData.CurrentPlace, carAnimController, _animationCache, cancellationToken);
			if (checkerStopAction.CheckStop(_personAnimationsController, carAnimController, cancellationToken))
			{
				ClearCts();
				return;
			}
			_personAnimationsController.Animator.Update(0f);
			float delay = _animationService.GetDelayToOpenCloseDoor(carAnimController, _carAnimationType, _sitGetOutData.CurrentPlace);
			delay = Mathf.Max(delay, 0f);
			carAnimationType = _carAnimationType;
			bool inside = carAnimationType == CarAnimationType.OpenInside || carAnimationType == CarAnimationType.CloseInside;
			if (inside)
			{
				_personAnimationsController.AnimationService.EnableDisableIk(isOn: false);
			}
			await UniTask.Delay(TimeSpan.FromSeconds(delay), ignoreTimeScale: false, PlayerLoopTiming.Update, cancellationToken).SuppressCancellationThrow();
			if (checkerStopAction.CheckStop(_personAnimationsController, carAnimController, cancellationToken))
			{
				ClearCts();
				return;
			}
			carAnimationType = _carAnimationType;
			bool isOpen = carAnimationType == CarAnimationType.OpenInside || carAnimationType == CarAnimationType.OpenOutside;
			carAnimController.OpenCloseCarElement((CarDoorType)_sitGetOutData.CurrentPlace, isOpen);
			await UniTask.Delay(TimeSpan.FromSeconds(Mathf.Max(inside ? ((animLength - delay) / 2f) : (animLength - delay), 0f)), ignoreTimeScale: false, PlayerLoopTiming.Update, cancellationToken).SuppressCancellationThrow();
			if (checkerStopAction.CheckStop(_personAnimationsController, carAnimController, cancellationToken))
			{
				ClearCts();
				return;
			}
			if (inside)
			{
				await _animationService.EnableIkLerp(cancellationToken, checkerStopAction, carAnimController);
			}
			ClearCts();
		}

		private async UniTask UpdatePersonPositionAsync(Transform transformPosition, CancellationToken cancellationToken)
		{
			while (!cancellationToken.IsCancellationRequested && (bool)transformPosition && (bool)_person)
			{
				_person.transform.position = transformPosition.position;
				await UniTask.Yield(PlayerLoopTiming.Update);
			}
		}

		private void ClearCts()
		{
			CommonExtensions.CancelAndDisposeToken(ref _fullPersonPositionUpdateCts);
			CommonExtensions.CancelAndDisposeToken(ref _personPositionUpdateCts);
		}
	}
}

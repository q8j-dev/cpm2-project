using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using _Car_Parking.Scripts.Audio.SoundBanks;

namespace CarAnimationScripts
{
	public class OpenCloseLightsAnimationsController : MonoBehaviour
	{
		private const string OpenTrigger = "Open";

		private const string CloseTrigger = "Close";

		[SerializeField]
		private Animator LightAnimator;

		private LightController _lightController;

		private SoundBankLoadService _soundBankService;

		private bool _headlightsBankHeld;

		private bool _isOpened;

		private bool _lastCommand;

		private Stack<bool> _commands = new Stack<bool>();

		private IDisposable _animatingDisposable;

		private bool _isAnimating;

		private void Start()
		{
			Transform parent = base.transform.parent;
			while ((bool)parent)
			{
				if (parent.TryGetComponent<LightController>(out _lightController))
				{
					_lightController.OnSwitchLights += OpenCloseLights;
					break;
				}
				parent = parent.parent;
			}
			VehicleData vehicleData = (parent ? parent.GetComponent<VehicleData>() : null);
			if ((bool)vehicleData)
			{
				_soundBankService = vehicleData.SoundBankService;
			}
			if ((bool)LightAnimator && _soundBankService != null)
			{
				_soundBankService.TryLoadBank(SoundBankType.CarHeadlights).Forget();
				_headlightsBankHeld = true;
			}
		}

		private void OpenCloseLights(bool open)
		{
			if (!LightAnimator)
			{
				return;
			}
			_lastCommand = open;
			if (_isAnimating || _isOpened == _lastCommand)
			{
				return;
			}
			LightAnimator.SetTrigger(_lastCommand ? "Open" : "Close");
			if ((bool)_lightController)
			{
				_soundBankService?.PostEvent(_lastCommand ? LightController.Play_headlights_open : LightController.Play_headlights_close, SoundBankType.CarHeadlights, _lightController.gameObject).Forget();
			}
			_isAnimating = true;
			_isOpened = _lastCommand;
			_animatingDisposable?.Dispose();
			_animatingDisposable = null;
			_animatingDisposable = Observable.Timer(TimeSpan.FromSeconds(GetCurrentAnimationLength())).Subscribe((long _) =>
			{
				_isAnimating = false;
				if (_lastCommand != _isOpened)
				{
					OpenCloseLights(_lastCommand);
				}
			}).AddTo(this);
		}

		private float GetCurrentAnimationLength()
		{
			if (!LightAnimator || !LightAnimator.runtimeAnimatorController)
			{
				return 0f;
			}
			AnimatorClipInfo[] currentAnimatorClipInfo = LightAnimator.GetCurrentAnimatorClipInfo(0);
			if (currentAnimatorClipInfo.Length == 0)
			{
				return 0f;
			}
			return currentAnimatorClipInfo[0].clip.length;
		}

		private void OnDestroy()
		{
			if ((bool)_lightController)
			{
				_lightController.OnSwitchLights -= OpenCloseLights;
			}
			if (_headlightsBankHeld)
			{
				_soundBankService?.TryUnloadBank(SoundBankType.CarHeadlights);
			}
		}
	}
}

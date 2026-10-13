using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using _Car_Parking.Scripts.Audio.SoundBanks;
using _Car_Parking.Scripts.Other;

namespace _Car_Parking.Scripts.Police
{
	public class PoliceCarLampsController : MonoBehaviour
	{
		[SerializeField]
		private LampLightGroup LampLightGroup;

		[SerializeField]
		private PoliceCarLampsAnimationConfig AnimationConfig = default;

		private CancellationTokenSource _cancellationTokenSource;

		private SoundBankLoadService _soundBankService;

		private bool _sirenBankHeld;

		private void OnEnable()
		{
			SetSirenBank(needed: true);
		}

		private void OnDisable()
		{
			SetSirenBank(needed: false);
		}

		private void SetSirenBank(bool needed)
		{
			if (_soundBankService == null && (bool)V.CoreGameManager)
			{
				_soundBankService = V.CoreGameManager.SoundBankLoadService;
			}
			if (needed != _sirenBankHeld && _soundBankService != null)
			{
				_sirenBankHeld = needed;
				if (needed)
				{
					_soundBankService.TryLoadBank(SoundBankType.CarSirens).Forget();
				}
				else
				{
					_soundBankService.TryUnloadBank(SoundBankType.CarSirens).Forget();
				}
			}
		}

		public void StartAnimation()
		{
			CancelAnimation();
			_cancellationTokenSource = new CancellationTokenSource();
			AnimationAsync(_cancellationTokenSource.Token).Forget();
		}

		public void StopAnimation()
		{
			CancelAnimation();
			LampLightGroup.DisableAll(0.1f);
		}

		private void OnDestroy()
		{
			CancelAnimation();
		}

		private void CancelAnimation()
		{
			_cancellationTokenSource?.Cancel();
			_cancellationTokenSource?.Dispose();
			_cancellationTokenSource = null;
		}

		private async UniTaskVoid AnimationAsync(CancellationToken cancellationToken)
		{
			if (!AnimationConfig || AnimationConfig.AnimationOneLightDatas.Length == 0)
			{
				return;
			}
			do
			{
				for (int i = 0; i < AnimationConfig.AnimationOneLightDatas.Length; i++)
				{
					float delayToEnable = AnimationConfig.AnimationOneLightDatas[i].DelayToEnable;
					if (delayToEnable > 0f)
					{
						await UniTask.Delay(TimeSpan.FromSeconds(delayToEnable), ignoreTimeScale: false, PlayerLoopTiming.Update, cancellationToken);
						if (cancellationToken.IsCancellationRequested)
						{
							return;
						}
					}
					LampLightGroup.EnableDisableAnimation(AnimationConfig.AnimationOneLightDatas[i].Index, AnimationConfig.AnimationOneLightDatas[i].TimeEnable, AnimationConfig.AnimationOneLightDatas[i].Time, AnimationConfig.AnimationOneLightDatas[i].TimeDisable);
				}
				if (AnimationConfig.DelayToNextCycle > 0f)
				{
					await UniTask.Delay(TimeSpan.FromSeconds(AnimationConfig.DelayToNextCycle), ignoreTimeScale: false, PlayerLoopTiming.Update, cancellationToken);
				}
			}
			while (!cancellationToken.IsCancellationRequested);
		}

		private void SetRefs()
		{
			LampLightGroup = GetComponent<LampLightGroup>();
		}
	}
}

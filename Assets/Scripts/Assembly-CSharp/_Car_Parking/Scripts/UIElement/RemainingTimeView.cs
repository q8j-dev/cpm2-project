using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.Infrastructure.Start;

namespace _Car_Parking.Scripts.UIElement
{
	public class RemainingTimeView : MonoBehaviour
	{
		[SerializeField]
		private TMP_Text Text;

		private ServerTimeService _serverTimeService;

		private TimeUnitSuffixes _timeUnitSuffixes;

		private long _endUnixSec;

		private bool _ticking;

		[Inject]
		public void Construct(ServerTimeService serverTimeService)
		{
			_serverTimeService = serverTimeService;
		}

		private void Awake()
		{
			_timeUnitSuffixes = TextExtensions.CreateTimeUnitSuffixes();
		}

		public void Init(long endUnixSec)
		{
			_endUnixSec = endUnixSec;
			UpdateTime();
			if (!_ticking)
			{
				_ticking = true;
				Tick(base.destroyCancellationToken).Forget();
			}
		}

		private async UniTaskVoid Tick(CancellationToken token)
		{
			while (!(await UniTask.Delay(TimeSpan.FromSeconds(1.0), DelayType.UnscaledDeltaTime, PlayerLoopTiming.Update, token).SuppressCancellationThrow()))
			{
				UpdateTime();
			}
		}

		private void UpdateTime()
		{
			if ((bool)Text && _serverTimeService != null)
			{
				TimeSpan remaining = TimeFormatter.RemainingTime(_endUnixSec, _serverTimeService.CurrentServerUnixTimeSec);
				Text.text = TimeFormatter.FormatRemainingShort(remaining, in _timeUnitSuffixes);
			}
		}
	}
}

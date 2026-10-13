using System;
using System.Collections.Generic;
using System.Threading;
using CPM.Ads.Policy;
using Cysharp.Threading.Tasks;
using UnityEngine.Scripting;
using _Car_Parking.Scripts.Backend;

namespace _Car_Parking.Scripts.Drag
{
	[Preserve]
	public sealed class AdsDailyLimitsService : IAdsDailyLimitsService, IAdsDailyLimits
	{
		private readonly IAdsRewardsBackend _backend;

		private readonly IUtcClock _clock;

		private readonly Dictionary<AdPlacement, int> _spent = new Dictionary<AdPlacement, int>();

		private DateTime _fetchedDayUtc = DateTime.MinValue;

		private bool IsFresh => _fetchedDayUtc == _clock.UtcNow().Date;

		public AdsDailyLimitsService(IAdsRewardsBackend backend, IUtcClock clock)
		{
			_backend = backend;
			_clock = clock;
		}

		public async UniTask RefreshAsync(CancellationToken ct = default(CancellationToken))
		{
			try
			{
				Result<List<AdDailyLimit>> result = await _backend.GetLimits(ct);
				if (result.IsFail)
				{
					return;
				}
				_spent.Clear();
				_fetchedDayUtc = _clock.UtcNow().Date;
				foreach (AdDailyLimit item in result.Value)
				{
					if (item != null && Enum.TryParse<AdPlacement>(item.Id, out var result2) && Enum.IsDefined(typeof(AdPlacement), result2))
					{
						_spent[result2] = Math.Max(0, item.Spent);
					}
				}
			}
			catch (OperationCanceledException)
			{
			}
			catch (Exception)
			{
			}
		}

		public bool TryGetSpent(AdPlacement placement, out int spent)
		{
			spent = 0;
			if (!IsFresh)
			{
				return false;
			}
			return _spent.TryGetValue(placement, out spent);
		}

		public bool TryGetTotalSpent(out int totalSpent)
		{
			totalSpent = 0;
			if (!IsFresh)
			{
				return false;
			}
			foreach (int value in _spent.Values)
			{
				totalSpent += value;
			}
			return true;
		}

		public void RegisterSpent(AdPlacement placement)
		{
			if (IsFresh)
			{
				_spent[placement] = ((!_spent.TryGetValue(placement, out var value)) ? 1 : (value + 1));
			}
		}

		private string Describe()
		{
			List<string> list = new List<string>(_spent.Count);
			foreach (KeyValuePair<AdPlacement, int> item in _spent)
			{
				list.Add($"{item.Key}={item.Value}");
			}
			if (list.Count <= 0)
			{
				return "<empty>";
			}
			return string.Join(", ", list);
		}
	}
}

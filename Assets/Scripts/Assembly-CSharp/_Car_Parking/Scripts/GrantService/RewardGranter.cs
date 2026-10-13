using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Offers;

namespace _Car_Parking.Scripts.GrantService
{
	public sealed class RewardGranter : IDisposable
	{
		private readonly Dictionary<RewardType, IGrantHandler> _handlers;

		private readonly CommonProvider _commonProvider;

		private CancellationTokenSource _grantCts;

		public RewardGranter(IReadOnlyList<IGrantHandler> handlers, CommonProvider commonProvider)
		{
			_commonProvider = commonProvider;
			_handlers = new Dictionary<RewardType, IGrantHandler>();
			for (int i = 0; i < handlers.Count; i++)
			{
				_handlers[handlers[i].Type] = handlers[i];
			}
		}

		public async UniTask<bool> GrantAsync(RewardItemData[] rewards)
		{
			if (rewards == null)
			{
				return true;
			}
			BaseCommonExtensions.CancelAndDisposeToken(ref _grantCts);
			_grantCts = new CancellationTokenSource();
			CancellationToken token = _grantCts.Token;
			for (int i = 0; i < rewards.Length; i++)
			{
				if (token.IsCancellationRequested)
				{
					return false;
				}
				RewardItemData rewardItemData = rewards[i];
				if (rewardItemData != null && rewardItemData.Type != RewardType.Car && !(await GrantOneAsync(rewardItemData, token)))
				{
					return false;
				}
			}
			if (HasSuitReward(rewards))
			{
				_commonProvider.CharactersService?.Save();
			}
			for (int i = 0; i < rewards.Length; i++)
			{
				if (token.IsCancellationRequested)
				{
					return false;
				}
				RewardItemData rewardItemData2 = rewards[i];
				if (rewardItemData2 != null && rewardItemData2.Type == RewardType.Car)
				{
					if (!(await GrantOneAsync(rewardItemData2, token)))
					{
						return false;
					}
					if (token.IsCancellationRequested)
					{
						return false;
					}
				}
			}
			return true;
		}

		private UniTask<bool> GrantOneAsync(RewardItemData reward, CancellationToken token)
		{
			if (_handlers.TryGetValue(reward.Type, out var value))
			{
				return value.GrantAsync(reward, token);
			}
			return UniTask.FromResult(value: true);
		}

		private static bool HasSuitReward(RewardItemData[] rewards)
		{
			for (int i = 0; i < rewards.Length; i++)
			{
				if (rewards[i] != null && (rewards[i].Type == RewardType.Suit || rewards[i].Type == RewardType.SuitPack))
				{
					return true;
				}
			}
			return false;
		}

		public void Dispose()
		{
			BaseCommonExtensions.CancelAndDisposeToken(ref _grantCts);
		}
	}
}

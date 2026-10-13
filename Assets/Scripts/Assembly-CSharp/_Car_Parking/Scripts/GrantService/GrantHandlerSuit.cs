using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using _Car_Parking.Scripts.Notification;
using _Car_Parking.Scripts.Offers;
using _Car_Parking.Scripts.Offers.Reward;

namespace _Car_Parking.Scripts.GrantService
{
	public sealed class GrantHandlerSuit : IGrantHandler
	{
		private readonly ISuitInventory _suits;

		private readonly INotificationService _notification;

		public RewardType Type => RewardType.Suit;

		public GrantHandlerSuit(ISuitInventory suits, INotificationService notification)
		{
			_suits = suits;
			_notification = notification;
		}

		public UniTask<bool> GrantAsync(RewardItemData reward, CancellationToken token)
		{
			if (!(reward is RewardItemSuitData rewardItemSuitData))
			{
				return UniTask.FromResult(value: true);
			}
			try
			{
				if (!_suits.IsBought(rewardItemSuitData.BodyType, rewardItemSuitData.TypeId, rewardItemSuitData.ID))
				{
					_suits.Buy(rewardItemSuitData.BodyType, rewardItemSuitData.TypeId, rewardItemSuitData.ID);
					_notification?.SetNotice(rewardItemSuitData.TypeId, rewardItemSuitData.ID, rewardItemSuitData.BodyType);
				}
			}
			catch (Exception)
			{
			}
			return UniTask.FromResult(value: true);
		}
	}
}

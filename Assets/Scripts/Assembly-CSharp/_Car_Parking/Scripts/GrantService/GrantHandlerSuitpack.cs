using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using _Car_Parking.Scripts.Notification;
using _Car_Parking.Scripts.Offers;
using _Car_Parking.Scripts.Offers.Reward;

namespace _Car_Parking.Scripts.GrantService
{
	public sealed class GrantHandlerSuitpack : IGrantHandler
	{
		private readonly ISuitInventory _suits;

		private readonly ISuitPacksConfig _packs;

		private readonly INotificationService _notification;

		public RewardType Type => RewardType.SuitPack;

		public GrantHandlerSuitpack(ISuitInventory suits, ISuitPacksConfig packs, INotificationService notification)
		{
			_suits = suits;
			_packs = packs;
			_notification = notification;
		}

		public UniTask<bool> GrantAsync(RewardItemData reward, CancellationToken token)
		{
			int iD = reward.ID;
			if (_packs == null || !_packs.TryGetPackItems(iD, out var items) || items == null)
			{
				return UniTask.FromResult(value: true);
			}
			for (int i = 0; i < items.Count; i++)
			{
				SuitPackItem suitPackItem = items[i];
				try
				{
					if (!_suits.IsBought(suitPackItem.BodyType, suitPackItem.TypeId, suitPackItem.ItemId))
					{
						_suits.Buy(suitPackItem.BodyType, suitPackItem.TypeId, suitPackItem.ItemId);
						_notification?.SetNotice(suitPackItem.TypeId, suitPackItem.ItemId, suitPackItem.BodyType);
					}
				}
				catch (Exception)
				{
				}
			}
			return UniTask.FromResult(value: true);
		}
	}
}

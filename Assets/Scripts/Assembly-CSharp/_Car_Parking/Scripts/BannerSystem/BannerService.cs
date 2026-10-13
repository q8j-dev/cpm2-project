using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using VContainer;
using _Car_Parking.Scripts.BannerSystem.Rules;
using _Car_Parking.Scripts.EventSystem.EventService;
using _Car_Parking.Scripts.Offers;
using _Car_Parking.Scripts.Other.Helpers;

namespace _Car_Parking.Scripts.BannerSystem
{
	public class BannerService : IBannerService
	{
		private const string LastDayShowKey = "BigOfferLastDayShowKey4";

		private readonly List<BannerType> _bannersQueue = new List<BannerType>
		{
			BannerType.Event,
			BannerType.Offer
		};

		private readonly Dictionary<BannerType, IBannerRule> _rules;

		[Inject]
		public BannerService(IEventService eventService, IOfferSystem offerSystem, IPlayerStatusSystem playerStatusSystem)
		{
			_rules = new Dictionary<BannerType, IBannerRule>
			{
				{
					BannerType.Event,
					new EventsBannerRule(eventService)
				},
				{
					BannerType.Offer,
					new OfferBannerRule(offerSystem, playerStatusSystem)
				}
			};
		}

		public async UniTask<bool> ProvideShowBanner(BannerType typeForShow, Action showCallback, CancellationToken cancellationToken)
		{
			foreach (BannerType item in _bannersQueue)
			{
				IBannerRule bannerRule = _rules[item];
				if (item == typeForShow)
				{
					if (await bannerRule.CanShow())
					{
						if (cancellationToken.IsCancellationRequested)
						{
							return false;
						}
						showCallback?.Invoke();
						return true;
					}
					return false;
				}
				if (await bannerRule.CanShow())
				{
					return false;
				}
			}
			return false;
		}
	}
}

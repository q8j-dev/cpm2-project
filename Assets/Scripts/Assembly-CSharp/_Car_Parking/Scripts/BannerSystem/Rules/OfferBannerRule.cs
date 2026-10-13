using Cysharp.Threading.Tasks;
using _Car_Parking.Scripts.Offers;
using _Car_Parking.Scripts.Other.Helpers;

namespace _Car_Parking.Scripts.BannerSystem.Rules
{
	public class OfferBannerRule : IBannerRule
	{
		private readonly IOfferSystem _offerSystem;

		private IPlayerStatusSystem _playerStatusSystem;

		public OfferBannerRule(IOfferSystem offerSystem, IPlayerStatusSystem playerStatusSystem)
		{
			_offerSystem = offerSystem;
			_playerStatusSystem = playerStatusSystem;
		}

		public UniTask<bool> CanShow()
		{
			return new UniTask<bool>(_offerSystem.GetOfferForBigBanner() != null);
		}
	}
}

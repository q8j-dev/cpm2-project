using _Car_Parking.Scripts.MenuUI.Person;
using _Car_Parking.Scripts.Offers;

namespace _Car_Parking.Scripts.WindowSystem
{
	public class BigOfferAnimationWindow : BigOfferEmoteWindowBase
	{
		protected override AnimationShopTab TargetTab => AnimationShopTab.Animations;

		protected override RewardType BannerRewardType => RewardType.Animations;
	}
}

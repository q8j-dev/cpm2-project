using _Car_Parking.Scripts.MenuUI.Person;
using _Car_Parking.Scripts.Offers;

namespace _Car_Parking.Scripts.WindowSystem
{
	public class BigOfferPoseWindow : BigOfferEmoteWindowBase
	{
		protected override AnimationShopTab TargetTab => AnimationShopTab.Poses;

		protected override RewardType BannerRewardType => RewardType.Poses;
	}
}

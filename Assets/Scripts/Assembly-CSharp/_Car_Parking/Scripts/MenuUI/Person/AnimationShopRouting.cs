namespace _Car_Parking.Scripts.MenuUI.Person
{
	public static class AnimationShopRouting
	{
		public static PersonWindowCategoryType CategoryFor(AnimationShopTab tab)
		{
			if (tab != AnimationShopTab.Poses)
			{
				return PersonWindowCategoryType.Animation;
			}
			return PersonWindowCategoryType.Poses;
		}

		public static AnimationShopSection SectionFor(AnimationShopTab tab)
		{
			if (tab != AnimationShopTab.Poses)
			{
				return AnimationShopSection.Animations;
			}
			return AnimationShopSection.Poses;
		}
	}
}

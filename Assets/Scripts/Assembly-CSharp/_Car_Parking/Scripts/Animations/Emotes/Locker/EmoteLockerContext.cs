using System;
using _Car_Parking.Scripts.MenuUI.Person;

namespace _Car_Parking.Scripts.Animations.Emotes.Locker
{
	public sealed class EmoteLockerContext
	{
		public bool HideEmojiTab;

		public Func<bool> ExitWalkForBanner;

		public AnimationShopTab? OpenPickerTab;

		public int FocusBundleId = -1;
	}
}

namespace _Car_Parking.Scripts.MenuUI.Person
{
	public sealed class AnimationShopOpenRequest
	{
		private AnimationShopTab? _requestedTab;

		private int _focusBundleId = -1;

		public void Set(AnimationShopTab tab, int focusBundleId = -1)
		{
			_requestedTab = tab;
			_focusBundleId = focusBundleId;
		}

		public bool TryConsume(out AnimationShopTab tab)
		{
			int focusBundleId;
			return TryConsume(out tab, out focusBundleId);
		}

		public bool TryConsume(out AnimationShopTab tab, out int focusBundleId)
		{
			if (!_requestedTab.HasValue)
			{
				tab = AnimationShopTab.Animations;
				focusBundleId = -1;
				return false;
			}
			tab = _requestedTab.Value;
			focusBundleId = _focusBundleId;
			_requestedTab = null;
			_focusBundleId = -1;
			return true;
		}
	}
}

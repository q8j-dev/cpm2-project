using UnityEngine;
using _Car_Parking.Scripts.PrefsKeys;

namespace _Car_Parking.Scripts.EventSystem.View.Elements
{
	public class BannerSteamLinkElement : MonoBehaviour, IBannerElement
	{
		[SerializeField]
		private bool ShowWhenLinked = true;

		public void Bind(BannerContext context)
		{
			bool flag = BannerClaimStateElement.Resolve(context) == BannerClaimStateElement.ClaimState.Claimed;
			base.gameObject.SetActive(!flag && ManagerKeysForPrefs.IsSteamLinked == ShowWhenLinked);
		}
	}
}

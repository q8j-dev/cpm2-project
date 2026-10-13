using UnityEngine;
using _Car_Parking.Scripts.EventSystem.Data;
using _Car_Parking.Scripts.PrefsKeys;

namespace _Car_Parking.Scripts.EventSystem.View.Elements
{
	public class BannerClaimStateElement : MonoBehaviour, IBannerElement
	{
		public enum ClaimState
		{
			Claim = 0,
			Link = 1,
			Claimed = 2
		}

		[SerializeField]
		private ClaimState State;

		public void Bind(BannerContext context)
		{
			base.gameObject.SetActive(Resolve(context) == State);
		}

		public static ClaimState Resolve(BannerContext context)
		{
			if (context?.Event != null && context.Event.Status == EventStatus.Claimed)
			{
				return ClaimState.Claimed;
			}
			if (!ManagerKeysForPrefs.IsSteamLinked)
			{
				return ClaimState.Link;
			}
			return ClaimState.Claim;
		}
	}
}

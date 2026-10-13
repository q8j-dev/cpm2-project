using _Car_Parking.Scripts.EventSystem.Data;
using _Car_Parking.Scripts.PrefsKeys;

namespace _Car_Parking.Scripts.EventSystem.View
{
	public class SteamExclusiveCarEventView : EventView
	{
		public override void Init(Event @event)
		{
			base.Init(@event);
			ApplyClaimButtonGate();
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			ApplyClaimButtonGate();
		}

		private void ApplyClaimButtonGate()
		{
			if ((bool)base.ClaimButtonAnimationParent)
			{
				base.ClaimButtonAnimationParent.gameObject.SetActive(ManagerKeysForPrefs.IsSteamLinked);
			}
		}
	}
}

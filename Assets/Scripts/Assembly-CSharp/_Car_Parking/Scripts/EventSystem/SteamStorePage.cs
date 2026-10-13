using UnityEngine;
using UnityEngine.Scripting;
using _Car_Parking.Scripts.EventSystem.Data;
using _Car_Parking.Scripts.EventSystem.Data.Response;

namespace _Car_Parking.Scripts.EventSystem
{
	[Preserve]
	public class SteamStorePage
	{
		private const string StoreUrl = "https://store.steampowered.com/";

		public void Open(_Car_Parking.Scripts.EventSystem.Data.Event @event)
		{
			if (!(@event?.Data is PromoEventData promoEventData))
			{
				return;
			}
			string text = "https://store.steampowered.com/";
			if (promoEventData.AppId > 0)
			{
				text += $"app/{promoEventData.AppId}/";
				if (!string.IsNullOrEmpty(promoEventData.AppName))
				{
					text = text + promoEventData.AppName + "/";
				}
			}
			Application.OpenURL(text);
		}
	}
}

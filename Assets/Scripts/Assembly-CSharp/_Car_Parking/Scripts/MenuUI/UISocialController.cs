using UnityEngine;

namespace _Car_Parking.Scripts.MenuUI
{
	public class UISocialController : MonoBehaviour
	{
		public void OnSocialPressed(GameObject GO)
		{
			string text = "";
			text = ((GO.name == "TikTok") ? "http://tiktok.com/@olzhass_games_tiktok" : ((GO.name == "Telegram") ? "https://t.me/olzhass_games_tg" : ((GO.name == "Discord") ? "https://discord.com/invite/A5CgdeX5AS" : ((!(GO.name == "Youtube")) ? "https://www.instagram.com/olzhass_games/" : "https://www.youtube.com/watch?v=vgezX_37Ucs"))));
			Application.OpenURL(text);
		}
	}
}

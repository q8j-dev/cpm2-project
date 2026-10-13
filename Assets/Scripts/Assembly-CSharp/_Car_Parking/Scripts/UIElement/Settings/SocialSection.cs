using UnityEngine;
using UnityEngine.UI;

namespace _Car_Parking.Scripts.UIElement.Settings
{
	public class SocialSection : SettingsSectionBase
	{
		private const string TikTokLink = "https://www.tiktok.com/@olzhass_games_tiktok";

		private const string InstagramLink = "https://www.instagram.com/olzhass_games_cpm2";

		private const string TelegramLink = "https://t.me/olzhass_games_tg";

		private const string DiscordLink = "https://discord.gg/cpmtwo";

		private const string YouTubeLink = "https://www.youtube.com/@olzhass_games_youtube";

		private const string AndroidLink = "https://play.google.com/store/apps/details?id=com.olzhas.carparking.multyplayer2";

		private const string IOSLink = "https://apps.apple.com/app/car-parking-multiplayer-2/id6514315118";

		private const string SteamLink = "https://store.steampowered.com/";

		[SerializeField]
		private Button TikTokButton = default;

		[SerializeField]
		private Button InstagramButton = default;

		[SerializeField]
		private Button TelegramButton = default;

		[SerializeField]
		private Button DiscordButton = default;

		[SerializeField]
		private Button YouTubeButton = default;

		[SerializeField]
		private Button LeaveAReviewButton = default;

		[SerializeField]
		private GameObject TitleReview = default;

		private void Start()
		{
			TikTokButton.onClick.AddListener(OpenTikTok);
			InstagramButton.onClick.AddListener(OpenInstagram);
			TelegramButton.onClick.AddListener(OpenTelegram);
			DiscordButton.onClick.AddListener(OpenDiscord);
			YouTubeButton.onClick.AddListener(OpenYoutube);
			LeaveAReviewButton.gameObject.SetActive(value: false);
			if ((bool)TitleReview)
			{
				TitleReview.SetActive(value: false);
			}
		}

		private void OpenTikTok()
		{
			OpenLink("https://www.tiktok.com/@olzhass_games_tiktok");
		}

		private void OpenInstagram()
		{
			OpenLink("https://www.instagram.com/olzhass_games_cpm2");
		}

		private void OpenTelegram()
		{
			OpenLink("https://t.me/olzhass_games_tg");
		}

		private void OpenDiscord()
		{
			OpenLink("https://discord.gg/cpmtwo");
		}

		private void OpenYoutube()
		{
			OpenLink("https://www.youtube.com/@olzhass_games_youtube");
		}

		private void OpenLink(string link)
		{
			Application.OpenURL(link);
		}

		private void LeaveAReview()
		{
			OpenLink("https://store.steampowered.com/");
		}

		private void OnDestroy()
		{
			TikTokButton.onClick.RemoveListener(OpenTikTok);
			InstagramButton.onClick.RemoveListener(OpenInstagram);
			TelegramButton.onClick.RemoveListener(OpenTelegram);
			DiscordButton.onClick.RemoveListener(OpenDiscord);
			YouTubeButton.onClick.RemoveListener(OpenYoutube);
		}
	}
}

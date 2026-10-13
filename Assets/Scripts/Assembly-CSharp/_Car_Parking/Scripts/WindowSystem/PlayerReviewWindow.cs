using TMPro;
using UnityEngine;
using UnityEngine.UI;
using _Car_Parking.Scripts.Drag;

namespace _Car_Parking.Scripts.WindowSystem
{
	public class PlayerReviewWindow : WindowScript
	{
		private const string AndroidLink = "https://play.google.com/store/apps/details?id=com.olzhas.carparking.multyplayer2";

		private const string IOSLink = "https://apps.apple.com/app/car-parking-multiplayer-2/id6514315118";

		[SerializeField]
		private Button OpenMarketButton;

		[SerializeField]
		private Button LaterButton;

		[SerializeField]
		private Button CloseButton;

		[SerializeField]
		private TextMeshProUGUI Description;

		protected override void Start()
		{
			base.Start();
			OpenMarketButton.onClick.AddListener(OpenMarket);
			LaterButton.onClick.AddListener(ShowLater);
			CloseButton.onClick.AddListener(CloseWindow);
			SetDescription();
		}

		private void SetDescription()
		{
			string text = "";
			Description.text = text;
		}

		private void CloseWindow()
		{
			PlayerPrefs.SetInt("MarketOpened", 1);
			Close();
		}

		private void ShowLater()
		{
			int num = PlayerPrefs.GetInt("PlayerReviewShowTime");
			PlayerPrefs.SetInt("PlayerReviewShowTime", num + 60);
			Close();
		}

		private void OpenMarket()
		{
			PlayerPrefs.SetInt("MarketOpened", 1);
			Close();
		}

		private void OpenLink(string link)
		{
			Application.OpenURL(link);
		}
	}
}

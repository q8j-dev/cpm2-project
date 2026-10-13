using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace _Car_Parking.Scripts.Database.GameUpdater
{
	public class ButtonToMarket : MonoBehaviour
	{
		[FormerlySerializedAs("linksToMarkerConfig")]
		[SerializeField]
		private LinksToMarketConfig LinksToMarketConfig;

		private void Start()
		{
			GetComponent<Button>().onClick.AddListener(Show);
		}

		public void Show()
		{
			PlayerPrefs.SetInt("NeedToUpdateShow", 0);
			Application.OpenURL(LinksToMarketConfig.AndroidLink);
		}
	}
}

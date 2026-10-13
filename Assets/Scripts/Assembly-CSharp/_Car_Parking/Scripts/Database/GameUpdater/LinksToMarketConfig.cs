using UnityEngine;

namespace _Car_Parking.Scripts.Database.GameUpdater
{
	[CreateAssetMenu(fileName = "LinksToMarketConfig", menuName = "Game/LinksToMarketConfig", order = 0)]
	public class LinksToMarketConfig : ScriptableObject
	{
		public string AndroidLink = "https://play.google.com/store/apps/";

		public string IosLink = "https://apps.apple.com/";
	}
}

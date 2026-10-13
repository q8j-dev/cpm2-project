using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine.Scripting;

namespace _Car_Parking.Scripts.PlayerProfile
{
	[Preserve]
	public class ProfileFavouriteItemsData
	{
		[JsonProperty("FavouriteAnimations")]
		public List<int> FavouriteAnimations { get; set; }

		[JsonProperty("FavouriteEmojis")]
		public List<int> FavouriteEmojis { get; set; }
	}
}

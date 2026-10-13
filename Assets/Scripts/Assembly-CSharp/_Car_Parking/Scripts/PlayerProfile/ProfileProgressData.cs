using Newtonsoft.Json;
using UnityEngine.Scripting;

namespace _Car_Parking.Scripts.PlayerProfile
{
	[Preserve]
	public class ProfileProgressData
	{
		[JsonProperty("profile")]
		public ProfileVersionData Profile { get; set; }

		[JsonProperty("rating")]
		public ProfileRatingData Rating { get; set; }

		[JsonProperty("features")]
		public ProfileFeaturesData Features { get; set; }

		[JsonProperty("items")]
		public ProfileItemsData Items { get; set; }

		[JsonProperty("favouriteitems")]
		public ProfileFavouriteItemsData FavouriteItems { get; set; }
	}
}

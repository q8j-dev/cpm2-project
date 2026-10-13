using UnityEngine.Scripting;
using _Car_Parking.Scripts.EventSystem.View.Elements;

namespace _Car_Parking.Scripts.EventSystem.View.ButtonActions
{
	[Preserve]
	public class OpenStoreAction : IBannerButtonAction
	{
		private readonly SteamStorePage _steamStorePage;

		public BannerButtonKind Kind => BannerButtonKind.OpenStore;

		public OpenStoreAction(SteamStorePage steamStorePage)
		{
			_steamStorePage = steamStorePage;
		}

		public void Execute(BannerActionContext context)
		{
			_steamStorePage.Open(context?.Event);
		}
	}
}

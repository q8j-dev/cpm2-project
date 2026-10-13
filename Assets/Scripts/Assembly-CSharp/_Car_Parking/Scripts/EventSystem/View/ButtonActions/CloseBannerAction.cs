using UnityEngine.Scripting;
using _Car_Parking.Scripts.EventSystem.View.Elements;

namespace _Car_Parking.Scripts.EventSystem.View.ButtonActions
{
	[Preserve]
	public class CloseBannerAction : IBannerButtonAction
	{
		public BannerButtonKind Kind => BannerButtonKind.Close;

		public void Execute(BannerActionContext context)
		{
			if (context != null && context.CloseBanner != null)
			{
				context.CloseBanner();
			}
		}
	}
}

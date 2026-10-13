using _Car_Parking.Scripts.EventSystem.View.Elements;

namespace _Car_Parking.Scripts.EventSystem.View.ButtonActions
{
	public interface IBannerButtonAction
	{
		BannerButtonKind Kind { get; }

		void Execute(BannerActionContext context);
	}
}

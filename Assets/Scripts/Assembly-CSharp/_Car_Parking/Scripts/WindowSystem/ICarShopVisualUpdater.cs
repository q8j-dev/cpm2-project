using _Car_Parking.Scripts.UIElement;

namespace _Car_Parking.Scripts.WindowSystem
{
	public interface ICarShopVisualUpdater
	{
		void UpdateCard(DealerUIElement card, int carId);

		void UpdateBuyButton(ButtonBuyNewCar button, int carId);
	}
}

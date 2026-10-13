using System.Collections.Generic;
using UnityEngine.Scripting;
using _Car_Parking.Scripts.UIElement;

namespace _Car_Parking.Scripts.WindowSystem
{
	[Preserve]
	public class CarShopVisualService
	{
		private readonly List<ICarShopVisualUpdater> _updaters = new List<ICarShopVisualUpdater>();

		public void Register(ICarShopVisualUpdater updater)
		{
			if (updater != null && !_updaters.Contains(updater))
			{
				_updaters.Add(updater);
			}
		}

		public void Unregister(ICarShopVisualUpdater updater)
		{
			_updaters.Remove(updater);
		}

		public void ApplyCardVisual(DealerUIElement card)
		{
			if ((bool)card)
			{
				if ((bool)card.SideIcon)
				{
					card.SideIcon.gameObject.SetActive(value: false);
				}
				if ((bool)card.PriceRoot)
				{
					card.PriceRoot.SetActive(value: true);
				}
				for (int i = 0; i < _updaters.Count; i++)
				{
					_updaters[i].UpdateCard(card, card.CarId);
				}
			}
		}

		public void ApplyBuyButtonVisual(ButtonBuyNewCar button, int carId)
		{
			if ((bool)button)
			{
				if ((bool)button.PriceRoot)
				{
					button.PriceRoot.SetActive(value: true);
				}
				if ((bool)button.Label && !string.IsNullOrEmpty(button.DefaultLabelKey))
				{
					button.Label.SetKey(button.DefaultLabelKey);
				}
				for (int i = 0; i < _updaters.Count; i++)
				{
					_updaters[i].UpdateBuyButton(button, carId);
				}
			}
		}
	}
}

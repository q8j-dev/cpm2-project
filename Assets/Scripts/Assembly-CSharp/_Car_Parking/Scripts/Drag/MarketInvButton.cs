using System;
using DG.Tweening;
using UnityEngine.UI;

namespace _Car_Parking.Scripts.Drag
{
	public class MarketInvButton : Button
	{
		public int id;

		private MarketInventoryWindow window;

		protected override void OnEnable()
		{
			window = GetComponentInParent<MarketInventoryWindow>();
			MarketInventoryWindow marketInventoryWindow = window;
			marketInventoryWindow.OnPanelOpened = (VisualButtonHandler)Delegate.Combine(marketInventoryWindow.OnPanelOpened, new VisualButtonHandler(Visual));
		}

		protected override void OnDisable()
		{
			MarketInventoryWindow marketInventoryWindow = window;
			marketInventoryWindow.OnPanelOpened = (VisualButtonHandler)Delegate.Remove(marketInventoryWindow.OnPanelOpened, new VisualButtonHandler(Visual));
		}

		private void Visual(object sender)
		{
			if (id == (int)window.openPanel)
			{
				base.targetGraphic.DOFade(1f, 0.25f);
			}
			else
			{
				base.targetGraphic.DOFade(0f, 0.25f);
			}
		}
	}
}

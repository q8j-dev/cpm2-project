using System;
using UnityEngine;

namespace _Car_Parking.Scripts.VisualTuningFavourite
{
	public class FavouriteWheelsUI : VisualTuningFavouriteUI
	{
		[SerializeField]
		private GameObject FavouriteSwitchPanel = default;

		protected override void RegisterEvents()
		{
			base.RegisterEvents();
			VisualTuningFavouriteControllerBase visualTuningFavouriteController = VisualTuningFavouriteController;
			visualTuningFavouriteController.OnFavouriteSwitchPanelEnabled = (Action<bool>)Delegate.Combine(visualTuningFavouriteController.OnFavouriteSwitchPanelEnabled, new Action<bool>(EnableFavouriteSwitchPanel));
		}

		protected override void UnregisterEvents()
		{
			base.UnregisterEvents();
			VisualTuningFavouriteControllerBase visualTuningFavouriteController = VisualTuningFavouriteController;
			visualTuningFavouriteController.OnFavouriteSwitchPanelEnabled = (Action<bool>)Delegate.Remove(visualTuningFavouriteController.OnFavouriteSwitchPanelEnabled, new Action<bool>(EnableFavouriteSwitchPanel));
		}

		private void EnableFavouriteSwitchPanel(bool isEnable)
		{
			FavouriteSwitchPanel.SetActive(isEnable);
			AddRemoveToFavouriteToggle.SetActive(isEnable);
		}
	}
}

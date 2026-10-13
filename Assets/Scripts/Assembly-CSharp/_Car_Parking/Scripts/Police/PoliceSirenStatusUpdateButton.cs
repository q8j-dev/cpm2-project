using UnityEngine;
using UnityEngine.UI;

namespace _Car_Parking.Scripts.Police
{
	public class PoliceSirenStatusUpdateButton : PoliceButtonBase
	{
		[SerializeField]
		private GameObjectStateUpdater[] GameObjectStateUpdaters;

		protected override void OnEnable()
		{
			base.OnEnable();
			UpdateIcon();
		}

		protected override void ButtonOnClick()
		{
			base.ButtonOnClick();
			if ((bool)V.Car && V.Car.TryGetComponent<VehicleData>(out var component))
			{
				component.PoliceCarController.PoliceCarSirenControllerInGameplay.SwitchSiren();
				UpdateIcon();
			}
		}

		public void UpdateIcon()
		{
			if (!V.Car || !V.Car.TryGetComponent<VehicleData>(out var component))
			{
				return;
			}
			GameObjectStateUpdater[] gameObjectStateUpdaters = GameObjectStateUpdaters;
			foreach (GameObjectStateUpdater gameObjectStateUpdater in gameObjectStateUpdaters)
			{
				if ((bool)gameObjectStateUpdater)
				{
					gameObjectStateUpdater.UpdateState(component.PoliceCarController.PoliceCarSirenControllerInGameplay.SirenActive);
				}
			}
		}

		public void PoliceSirenStatus()
		{
			ButtonOnClick();
		}

		public void UpdatePoliceSirenStatusButtonImage(Image image)
		{
			image.sprite = Button.image.sprite;
		}
	}
}

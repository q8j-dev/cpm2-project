using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace _Car_Parking.Scripts.Police
{
	public class PoliceSirenSoundEnableButton : PoliceUIBase
	{
		[SerializeField]
		private Button Button;

		[SerializeField]
		private GameObjectStateUpdater[] GameObjectStateUpdaters;

		public IPoliceSystem _policeSystem;

		[Inject]
		public void Construct(IPoliceSystem policeSystem)
		{
			_policeSystem = policeSystem;
		}

		private void OnEnable()
		{
			Button.onClick.AddListener(OnButtonClick);
			UpdateUI();
		}

		private void OnDisable()
		{
			Button.onClick.RemoveListener(OnButtonClick);
		}

		private void OnButtonClick()
		{
			if ((bool)V.Car && V.Car.TryGetComponent<VehicleData>(out var component))
			{
				component.PoliceCarController.MuteController.SwitchMute();
				UpdateUI();
			}
		}

		private void UpdateUI()
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
					gameObjectStateUpdater.UpdateState(!component.PoliceCarController.MuteController.IsMute.Value);
				}
			}
		}

		public void PoliceSirenSound()
		{
			OnButtonClick();
		}

		public void UpdatePoliceSirenSoundButtonImage(Image image)
		{
			image.sprite = Button.image.sprite;
		}
	}
}

using System;
using Cysharp.Threading.Tasks;
using _Car_Parking.Scripts.Localization;

namespace _Car_Parking.Scripts.PlayerProperty
{
	public class PropertyTriggerService
	{
		private UIActionButton _uiActionButton;

		private Action _lastActiveActionButton;

		public void SetAction(Action activeActionButton)
		{
			_lastActiveActionButton = activeActionButton;
		}

		public async UniTaskVoid CreateButton()
		{
			if (_lastActiveActionButton != null)
			{
				string buttonName = "SpeedActionButton";
				if (FlagsStorage.OutOfCar)
				{
					buttonName = "UIActionButton";
				}
				if (!_uiActionButton)
				{
					_uiActionButton = await V.CoreGameManager.ManagerUI.CreateActionButton(buttonName, _lastActiveActionButton, CantStartRace);
				}
				if (FlagsStorage.OutOfCar)
				{
					_uiActionButton.SetInteractable(interactable: true, deactivateButtons: false);
				}
			}
		}

		public void CloseActionButton()
		{
			if ((bool)_uiActionButton)
			{
				_uiActionButton.Close();
			}
		}

		private void CantStartRace()
		{
			V.CoreGameManager.ManagerUI.ShowNotificationWindow(new LocalizedString("TimeAttack.CantStartRace"), 3f).Forget();
		}
	}
}

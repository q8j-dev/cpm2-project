using Cysharp.Threading.Tasks;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Other.Helpers;

namespace _Car_Parking.Scripts.WindowSystem
{
	public static class ManagerUIHelper
	{
		private const int MenuWaitFrames = 600;

		private static ManagerUI ManagerUI
		{
			get
			{
				if (!V.CoreGameManager)
				{
					return null;
				}
				return V.CoreGameManager.ManagerUI;
			}
		}

		public static async UniTask ShowWaitingWindow(bool set, bool backButtonState = false, bool alwaysOnTop = true, string additionalData = "", bool updateBackButton = true, bool waitFrameAfterShow = false)
		{
			if ((bool)ManagerUI)
			{
				await ManagerUI.ShowWaitingWindow(set, backButtonState, alwaysOnTop, additionalData, updateBackButton);
				if (waitFrameAfterShow)
				{
					await UniTask.Yield();
				}
			}
		}

		public static async UniTask CloseWaitingWindow(bool waitFrameAfterHide = false, bool canUseDelay = true)
		{
			if ((bool)ManagerUI)
			{
				ManagerUI.CloseWaitingWindow(canUseDelay);
				if (waitFrameAfterHide)
				{
					await UniTask.Yield();
				}
			}
		}

		public static async UniTask CloseNotTransparentWaitingWindow(bool waitFrameAfterHide = false)
		{
			if ((bool)ManagerUI)
			{
				ManagerUI.CloseNotTransparentWaitingWindow();
				if (waitFrameAfterHide)
				{
					await UniTask.Yield();
				}
			}
		}

		public static void SetMessage(string text, float duration, NotificationType type = NotificationType.Success)
		{
			if ((bool)ManagerUI)
			{
				ManagerUI.ShowNotificationWindow(text, duration, type).Forget();
			}
		}

		public static async UniTask SetMessageOnMenu(string text, float duration = 5f, NotificationType type = NotificationType.Success)
		{
			for (int frame = 0; frame < 600; frame++)
			{
				if (ScenesHelper.IsMenuSceneActive() && (bool)ManagerUI)
				{
					SetMessage(text, duration, type);
					break;
				}
				await UniTask.Yield();
			}
		}
	}
}

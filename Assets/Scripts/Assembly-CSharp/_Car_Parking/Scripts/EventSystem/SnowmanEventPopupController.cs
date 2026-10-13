using Cysharp.Threading.Tasks;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Localization;

namespace _Car_Parking.Scripts.EventSystem
{
	public class SnowmanEventPopupController : IEventUIWrapper
	{
		private TimedActivityZoneController _timedActivityZoneController;

		private UiNotificationsManager _uiNotificationsManager;

		private ManagerUI _managerUI;

		public SnowmanEventPopupController(UiNotificationsManager uiNotificationsManager, TimedActivityZoneController timedActivityZoneController, ManagerUI managerUI)
		{
			_uiNotificationsManager = uiNotificationsManager;
			_timedActivityZoneController = timedActivityZoneController;
			_managerUI = managerUI;
		}

		public void ShowHud(bool state)
		{
			if (state)
			{
				_managerUI.ShowWindow("SnowmanEventHudWindow", alwaysOnTop: false, show: true, null, fadeBlack: false, enableBackButton: false, cleanScreen: false).Forget();
			}
			else
			{
				_managerUI.CloseWindow("SnowmanEventHudWindow");
			}
		}

		public void ShowStartMessage()
		{
			PopupData popupData = GetPopupData("SnowManPopup", string.Format("{0}", new LocalizedString("Event.HitSnowmans")), "", SoundEventsDataHelper.UITimeAttackDefaultResultShowSwitch);
			popupData.BackgroundColor = GetPopupColorData("FFFFFF", useGradient: true, "#237EE0", "#69D2FF");
			_uiNotificationsManager.ShowPopup(popupData);
		}

		public void ShowMessage(bool pass, string message)
		{
			if (pass)
			{
				PopupData popupData = new PopupData
				{
					PopupKey = "FlashPopup"
				};
				_uiNotificationsManager.ShowPopup(popupData);
			}
			string colorKey = (pass ? "#FF9C00" : "#797979");
			string text = (pass ? "<color=green>" : "<color=red>");
			string title = (pass ? "" : ((string)new LocalizedString("Event.TimeOff")));
			uint audioKey = (pass ? SoundEventsDataHelper.UITimeAttackBestResultShowSwitch : SoundEventsDataHelper.UITimeAttackDefaultResultShowSwitch);
			PopupData popupData2 = GetPopupData("SnowManPopup", text + message, title, audioKey);
			popupData2.BackgroundColor = GetPopupColorData(colorKey);
			_uiNotificationsManager.ShowPopup(popupData2);
		}

		public void ShowResult(bool pass, int collectiblesCount, int targetCount)
		{
			string arg = (pass ? "<color=green>" : "<color=red>");
			string text = $"{arg} {collectiblesCount}</color>/{targetCount}";
			string title = new LocalizedString("Event.SnowmanHit");
			uint audioKey = (pass ? SoundEventsDataHelper.UITimeAttackBestResultShowSwitch : SoundEventsDataHelper.UITimeAttackDefaultResultShowSwitch);
			PopupData popupData = GetPopupData("SnowManPopup", text, title, audioKey);
			string colorKey = (pass ? "#FF9C00" : "#797979");
			popupData.BackgroundColor = GetPopupColorData(colorKey);
			_uiNotificationsManager.ShowPopup(popupData);
		}

		public void ShowEventProgressMessage(string message)
		{
			PopupData popupData = GetPopupData("SnowManPopup", message, new LocalizedString("Event.ZoneCleared"), SoundEventsDataHelper.UITimeAttackDefaultResultShowSwitch);
			popupData.BackgroundColor = GetPopupColorData("FFFFFF", useGradient: true, "#237EE0", "#69D2FF");
			_uiNotificationsManager.ShowPopup(popupData);
		}

		private PopupData GetPopupData(string prefabKey, string text, string title, uint audioKey)
		{
			return new PopupData
			{
				Title = title,
				PopupKey = prefabKey,
				Description = text,
				AudioKey = audioKey
			};
		}

		private PopupColorData GetPopupColorData(string colorKey, bool useGradient = false, string gradientStartColor = "", string gradientEndColor = "")
		{
			return new PopupColorData
			{
				ColorKey = colorKey,
				UseGradient = useGradient,
				GradientStartKey = gradientStartColor,
				GradientEndKey = gradientEndColor
			};
		}
	}
}

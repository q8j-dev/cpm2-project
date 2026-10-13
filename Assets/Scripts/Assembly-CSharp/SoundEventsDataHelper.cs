using UI;
using UnityEngine;
using _Car_Parking.Scripts.Audio;

public class SoundEventsDataHelper
{
	public static readonly uint InstallEngineDetailProcessSwitch = AudioWrapper.GetId("Install_Engine_Detail_Process");

	public static readonly uint InstallVisualTuningSwitch = AudioWrapper.GetId("Install_Visual_Tuning");

	public static readonly uint SmallClickSwitch = AudioWrapper.GetId("Small_Click");

	public static readonly uint ClickBackSwitch = AudioWrapper.GetId("Click_Back");

	public static readonly uint InstallPaintAcceptSwitch = AudioWrapper.GetId("Install_Paint_Accept");

	public static readonly uint TuningSelectRimSizeSwitch = AudioWrapper.GetId("Tuning_Select_Rim_Size");

	public static readonly uint TuningSelectCarbodySwitch = AudioWrapper.GetId("Tuning_Select_Carbody");

	public static readonly uint TuningSelectRimSwitch = AudioWrapper.GetId("Tuning_Select_Rim");

	public static readonly uint TuningSelectBrakesSwitch = AudioWrapper.GetId("Tuning_Select_Brakes");

	public static readonly uint TuningSelectVinylSwitch = AudioWrapper.GetId("Tuning_Select_Vinyl");

	public static readonly uint TuningSelectPaintSwitch = AudioWrapper.GetId("Tuning_Select_Paint");

	public static readonly uint GearShiftSwitch = AudioWrapper.GetId("Gear_Shift");

	public static readonly uint AchivementUnlockedSwitch = AudioWrapper.GetId("AchivementUnlocked");

	public static readonly uint PhoneNotifySwitch = AudioWrapper.GetId("Phone_Notify");

	public static readonly uint SpeedAchiveSwitch = AudioWrapper.GetId("Speed_Achive");

	public static readonly uint LevelNotCompleteSwitch = AudioWrapper.GetId("Level_NotComplete");

	public static readonly uint LevelCompleteSwitch = AudioWrapper.GetId("Level_Complete");

	public static readonly uint ConfirmationWindowSwitch = AudioWrapper.GetId("Confirmation_Window");

	public static readonly uint RequestRaceSwitch = AudioWrapper.GetId("Request_Race");

	public static readonly uint IndicatorSwitch = AudioWrapper.GetId("Indicator");

	public static readonly uint PhotoModeZoomSwitch = AudioWrapper.GetId("UI_PhotoMode_Zoom");

	public static readonly uint PhotoModeSettingValueSwitch = AudioWrapper.GetId("UI_PhotoMode_SettingValue");

	public static readonly uint PhotoModeSettingsSwitch = AudioWrapper.GetId("UI_PhotoMode_Settings");

	public static readonly uint PhotoModeInfoSwitch = AudioWrapper.GetId("UI_PhotoMode_Info");

	public static readonly uint PhotoModeShutterSwitch = AudioWrapper.GetId("UI_PhotoMode_Shutter");

	public static readonly uint ScrollSwitch = AudioWrapper.GetId("Scroll");

	public static readonly uint ShortNotifySwitchState = AudioWrapper.GetId("Short_Notification");

	public static readonly uint ShowUIMapPopupSwtch = AudioWrapper.GetId("UI_Map_Popup");

	public static readonly uint UITimeAttackBestResultShowSwitch = AudioWrapper.GetId("UI_TimeAttack_Best_Result");

	public static readonly uint UITimeAttackDefaultResultShowSwitch = AudioWrapper.GetId("UI_TimeAttack_Lap");

	public static readonly uint UITimeAttackEventCanClaimSwitch = AudioWrapper.GetId("UI_Event_Claim_Popup");

	public static readonly uint ChatMessageSwitch = AudioWrapper.GetId("Chat_Massege");

	public static readonly uint ChatSendingSwitch = AudioWrapper.GetId("Chat_Sending");

	public static readonly uint ChatFriendRequestSwitch = AudioWrapper.GetId("Chat_FriendRequest");

	public static readonly uint UITimeAttackFinishFlash = AudioWrapper.GetId("UI_TimeAttack_Finish_Flash");

	public static readonly uint ClaimAppearSwitchState = AudioWrapper.GetId("UI_Event_Claim_Appear");

	public static readonly uint ClaimRewardSwitchState = AudioWrapper.GetId("UI_Event_Claim_Reward");

	public static readonly string StateReverPause = "State_Rever_Pause";

	public static readonly string OnReverState = "on_rever";

	public static readonly string OffReverState = "off_rever";

	public static readonly uint AcceptSwitch = AudioWrapper.GetId("Accept");

	public static readonly uint UIEventDriftEnd = AudioWrapper.GetId("UI_Event_Drift_End");

	public static readonly uint UIEventDriftCount = AudioWrapper.GetId("UI_Event_Drift_Count");

	public static readonly uint ShortNotification = AudioWrapper.GetId("Short_Notification");

	public static readonly uint UIEventDriftBonus = AudioWrapper.GetId("UI_Event_Drift_Bonus");

	public static readonly uint PlaySfxScreenShotSwitch = AudioWrapper.GetId("Play_SFX_ScreenShot");

	public static void PlayUISound(uint key, GameObject gameObject)
	{
		AudioWrapper.SetSwitch(Scroll.UiSwitch, key, gameObject);
		AudioWrapper.PostEvent(Scroll.UiEvent, gameObject);
	}

	public static void SetGroupState(string groupName, string state)
	{
		AudioWrapper.SetState(groupName, state);
	}
}

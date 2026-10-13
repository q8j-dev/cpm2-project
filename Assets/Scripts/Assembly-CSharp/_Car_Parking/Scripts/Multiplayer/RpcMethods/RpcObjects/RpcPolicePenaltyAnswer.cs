using CodeStage.AntiCheat.ObscuredTypes;
using VContainer;
using _Car_Parking.Scripts.CustomDataTypes;
using _Car_Parking.Scripts.GooglePlayServices;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Steam.Base;

namespace _Car_Parking.Scripts.Multiplayer.RpcMethods.RpcObjects
{
	public class RpcPolicePenaltyAnswer : RpcObjectBase
	{
		public enum ResultType
		{
			None = 0,
			CarInRace = 1,
			JustPenalized = 2,
			NoMoney = 3,
			Success = 4
		}

		private ISteamAchievementService _steamAchievementService;

		public override byte Code => 103;

		[Inject]
		public void Construct(ISteamAchievementService steamAchievementService)
		{
			_steamAchievementService = steamAchievementService;
		}

		public override void Execute(MultiPlayerPosControl multiPlayerPosControl, string penaltyNickname, string resultString)
		{
			FreeDriveControl freeDriveControl = multiPlayerPosControl.FreeDriveProvider.FreeDriveControl;
			if (!string.IsNullOrEmpty(penaltyNickname) && !string.IsNullOrEmpty(resultString) && (bool)freeDriveControl)
			{
				switch ((ResultType)int.Parse(resultString))
				{
				case ResultType.Success:
				{
					string format4 = new LocalizedString("BuyPolice.YouFined");
					V.CoreGameManager.SetMessage(string.Format(format4, penaltyNickname, freeDriveControl.PolicePenalizeForSpeeding.PoliceConfig.PenaltySystem.MoneyPenalty), 3f);
					ObscuredInt value = freeDriveControl.PolicePenalizeForSpeeding.PoliceConfig.PenaltySystem.MoneyPenalty;
					freeDriveControl.CurrencyManagerController.AddMoney(new AdvancedInt(value));
					++multiPlayerPosControl.FreeDriveProvider.FreeDriveDB.police;
					GooglePlayAchievements.Unlock("CgkIuKqW0PcZEAIQKw", "police_penalty");
					_steamAchievementService?.Unlock("ACH_LIGHTS_AND_PAPERS");
					break;
				}
				case ResultType.CarInRace:
				{
					string format3 = new LocalizedString("BuyPolice.IsRacing");
					V.CoreGameManager.SetMessage(string.Format(format3, penaltyNickname), 3f);
					break;
				}
				case ResultType.JustPenalized:
				{
					string format2 = new LocalizedString("BuyPolice.WasAlreadyFined");
					V.CoreGameManager.SetMessage(string.Format(format2, penaltyNickname), 3f);
					break;
				}
				case ResultType.NoMoney:
				{
					string format = new LocalizedString("BuyPolice.NoMoney");
					V.CoreGameManager.SetMessage(string.Format(format, penaltyNickname), 3f);
					break;
				}
				}
			}
		}
	}
}

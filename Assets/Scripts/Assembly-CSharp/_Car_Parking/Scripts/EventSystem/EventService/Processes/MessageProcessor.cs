using Cysharp.Threading.Tasks;
using _Car_Parking.Scripts.Analytics.Analyzers;
using _Car_Parking.Scripts.EventSystem.Data.Response;
using _Car_Parking.Scripts.Localization;

namespace _Car_Parking.Scripts.EventSystem.EventService.Processes
{
	public class MessageProcessor : RewardProcessor
	{
		public MessageProcessor(EventsAnalytic eventsAnalytic)
			: base(eventsAnalytic)
		{
		}

		public override async UniTask Execute(RewardResponseBase responseBase, string eventId)
		{
			if (responseBase is RewardMessageResponse rewardMessageResponse)
			{
				V.CoreGameManager.SetMessage(new LocalizedString($"RewardMessage.{rewardMessageResponse.MessageId}"), 3f);
				EventsAnalytic.LogSnowmanRewardClaimed(eventId, rewardMessageResponse.MessageId.ToString(), rewardReceived: true);
			}
		}
	}
}

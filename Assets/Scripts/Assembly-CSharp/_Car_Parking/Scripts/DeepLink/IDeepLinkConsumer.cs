namespace _Car_Parking.Scripts.DeepLink
{
	public interface IDeepLinkConsumer
	{
		bool TryConsumeDeepLink(DeepLinkData linkData);
	}
}

using UnityEngine.Scripting;
using _Car_Parking.Scripts.Infrastructure;

namespace _Car_Parking.Scripts.Multiplayer.RpcMethods.RpcObjects
{
	[Preserve]
	public class RpcCarExitCarSellTrigger : RpcObjectBase
	{
		private readonly CommonProvider _commonProvider;

		public override byte Code => 43;

		public static void Send(MultiPlayer multiPlayer, int sellCarTriggerNumber)
		{
			multiPlayer.SendValueStr<RpcCarExitCarSellTrigger>("", sellCarTriggerNumber.ToString());
		}

		public RpcCarExitCarSellTrigger(CommonProvider commonProvider)
		{
			_commonProvider = commonProvider;
		}

		public override void Execute(MultiPlayerPosControl multiPlayerPosControl, string b, string name)
		{
			_commonProvider.SellExchangeFreeService.SellCarExit(int.Parse(name));
		}
	}
}

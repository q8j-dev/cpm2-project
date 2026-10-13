using Photon.Pun;
using UnityEngine.Scripting;
using VContainer;
using _Car_Parking.Scripts.Infrastructure;

namespace _Car_Parking.Scripts.Multiplayer.RpcMethods.RpcObjects
{
	[UnityEngine.Scripting.Preserve]
	public class RpcOtherPlayerBlockedYou : RpcObjectBase
	{
		private readonly CommonProvider _commonProvider;

		public override byte Code => 122;

		public static string GetRequestData(bool blocked)
		{
			return string.Format("{0}{1}{2}", PhotonNetwork.LocalPlayer.ActorNumber, "?", blocked ? "1" : "0");
		}

		[Inject]
		public RpcOtherPlayerBlockedYou(CommonProvider commonProvider)
		{
			_commonProvider = commonProvider;
		}

		public override void Execute(MultiPlayerPosControl multiPlayerPosControl, string b, string name)
		{
			if (!string.IsNullOrEmpty(b) && _commonProvider.ChatExtraDataController != null)
			{
				string[] array = b.Split("?");
				if (array != null && array.Length == 2 && int.TryParse(array[0], out var result) && int.TryParse(array[1], out var result2))
				{
					_commonProvider.ChatExtraDataController.SetBlocked(result, result2 == 1);
				}
			}
		}
	}
}

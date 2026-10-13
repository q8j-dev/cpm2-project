using UnityEngine.Scripting;
using _Car_Parking.Scripts.Providers;

namespace _Car_Parking.Scripts.Multiplayer.RpcMethods.RpcObjects
{
	[Preserve]
	public class RpcChatMessageReceived : RpcObjectBase
	{
		public override byte Code => 52;

		public override void Execute(MultiPlayerPosControl multiPlayerPosControl, string b, string name)
		{
			ExecuteOnPerson(multiPlayerPosControl.FreeDriveProvider, b, name);
		}

		public override void ExecuteOnPerson(FreeDriveProvider freeDriveProvider, string b, string name)
		{
			freeDriveProvider.FreeDriveControl.chatControl.OnChatMessageReceived(b);
		}
	}
}

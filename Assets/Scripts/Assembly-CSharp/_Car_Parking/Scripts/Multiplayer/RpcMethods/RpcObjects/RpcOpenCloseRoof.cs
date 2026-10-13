namespace _Car_Parking.Scripts.Multiplayer.RpcMethods.RpcObjects
{
	public class RpcOpenCloseRoof : RpcObjectBase
	{
		public override byte Code => 68;

		public override void Execute(MultiPlayerPosControl multiPlayerPosControl, string b, string data)
		{
			if (int.TryParse(b, out var result))
			{
				bool isOpen = data == "1";
				multiPlayerPosControl.FreeDriveProvider.MultiPlayer.OpenCloseRoof(result, isOpen);
			}
		}
	}
}

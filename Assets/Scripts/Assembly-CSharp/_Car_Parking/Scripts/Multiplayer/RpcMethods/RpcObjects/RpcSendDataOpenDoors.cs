namespace _Car_Parking.Scripts.Multiplayer.RpcMethods.RpcObjects
{
	public class RpcSendDataOpenDoors : RpcObjectBase
	{
		public override byte Code => 66;

		public override void Execute(MultiPlayerPosControl multiPlayerPosControl, string b, string openDoors)
		{
			if (int.TryParse(b, out var result))
			{
				multiPlayerPosControl.FreeDriveProvider.MultiPlayer.UpdateDataOpenDoors(result, openDoors);
			}
		}
	}
}

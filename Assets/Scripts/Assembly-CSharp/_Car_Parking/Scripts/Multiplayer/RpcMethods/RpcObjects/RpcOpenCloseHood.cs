using CarInteractionAnimationsScripts;

namespace _Car_Parking.Scripts.Multiplayer.RpcMethods.RpcObjects
{
	public class RpcOpenCloseHood : RpcObjectBase
	{
		public override byte Code => 57;

		public override void Execute(MultiPlayerPosControl multiPlayerPosControl, string b, string name)
		{
			if (int.TryParse(b, out var result))
			{
				bool isOpen = (multiPlayerPosControl.hoodState = name == "1");
				multiPlayerPosControl.FreeDriveProvider.MultiPlayer.OpenCloseCarElement(result, isOpen, CarDoorType.Hood);
			}
		}
	}
}

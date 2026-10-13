using CarInteractionAnimationsScripts;

namespace _Car_Parking.Scripts.Multiplayer.RpcMethods.RpcObjects
{
	public class RpcOpenCloseDoorCar : RpcObjectBase
	{
		public override byte Code => 67;

		public override void Execute(MultiPlayerPosControl multiPlayerPosControl, string b, string name)
		{
			if (int.TryParse(b, out var result))
			{
				string[] array = name.Split(',');
				int carDoorType = int.Parse(array[0]);
				bool isOpen = array[1] == "1";
				multiPlayerPosControl.FreeDriveProvider.MultiPlayer.OpenCloseCarElement(result, isOpen, (CarDoorType)carDoorType);
			}
		}
	}
}

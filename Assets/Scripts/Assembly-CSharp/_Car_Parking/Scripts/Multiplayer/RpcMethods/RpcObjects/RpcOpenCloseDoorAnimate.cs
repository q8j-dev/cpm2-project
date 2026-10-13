using CarInteractionAnimationsScripts;

namespace _Car_Parking.Scripts.Multiplayer.RpcMethods.RpcObjects
{
	public class RpcOpenCloseDoorAnimate : RpcObjectBase
	{
		public override byte Code => 65;

		public override void Execute(MultiPlayerPosControl multiPlayerPosControl, string b, string name)
		{
			string[] array = b.Split(',');
			string[] array2 = name.Split(',');
			string s = array[0];
			string s2 = array[1];
			int.TryParse(array2[0], out var result);
			int.TryParse(array2[1], out var result2);
			int place = result;
			CarAnimationType carAnimationType = (CarAnimationType)result2;
			if (int.TryParse(s, out var result3) && int.TryParse(s2, out var result4))
			{
				multiPlayerPosControl.FreeDriveProvider.MultiPlayer.OpenCloseCarDoor(result3, result4, place, carAnimationType);
			}
		}
	}
}

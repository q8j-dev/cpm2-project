using _Car_Parking.Scripts.Providers;

namespace _Car_Parking.Scripts.Multiplayer.RpcMethods.RpcObjects
{
	public class RpcGetOutCar : RpcObjectBase
	{
		public override byte Code => 63;

		public override void Execute(MultiPlayerPosControl multiPlayerPosControl, string b, string data)
		{
			ExecuteOnPerson(multiPlayerPosControl.FreeDriveProvider, b, data);
		}

		public override void ExecuteOnPerson(FreeDriveProvider freeDriveProvider, string b, string data)
		{
			string[] array = b.Split(',');
			string s = array[0];
			string s2 = array[1];
			string[] array2 = data.Split(',');
			int place = int.Parse(array2[0]);
			bool playAnimation = array2[1] == "1";
			if (int.TryParse(s, out var result) && int.TryParse(s2, out var result2))
			{
				freeDriveProvider.MultiPlayer.PlayerExitFromCar(result, result2, place, playAnimation);
			}
		}
	}
}

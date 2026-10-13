using _Car_Parking.Scripts.Providers;

namespace _Car_Parking.Scripts.Multiplayer.RpcMethods.RpcObjects
{
	public class RpcSitIntoCar : RpcObjectBase
	{
		public override byte Code => 62;

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
			string s3 = array2[0];
			bool playAnimation = array2.Length <= 1 || array2[1] == "1";
			bool playDefaultDoorSound = array2.Length <= 2 || array2[2] == "1";
			if (int.TryParse(s, out var result) && int.TryParse(s2, out var result2))
			{
				freeDriveProvider.MultiPlayer.PlayerEnteredToCar(result, result2, int.Parse(s3), playAnimation, playDefaultDoorSound);
			}
		}
	}
}

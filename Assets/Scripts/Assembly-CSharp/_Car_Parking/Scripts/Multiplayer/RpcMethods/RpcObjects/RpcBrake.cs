namespace _Car_Parking.Scripts.Multiplayer.RpcMethods.RpcObjects
{
	public class RpcBrake : RpcObjectBase
	{
		public override byte Code => 60;

		public override bool SendToInterestGroup => true;

		public override void Execute(MultiPlayerPosControl multiPlayerPosControl, string b, string name)
		{
			if (multiPlayerPosControl.IsThisCar(b))
			{
				multiPlayerPosControl.CarComponents.Axles.BrakeRPC(name);
			}
		}
	}
}

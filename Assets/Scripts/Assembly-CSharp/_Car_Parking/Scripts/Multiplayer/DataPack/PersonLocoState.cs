namespace _Car_Parking.Scripts.Multiplayer.DataPack
{
	public struct PersonLocoState
	{
		public float Yaw;

		public float MoveSpeed;

		public PersonNetFlags Flags;

		public int Emotion;

		public bool Has(PersonNetFlags flag)
		{
			return (Flags & flag) != 0;
		}
	}
}

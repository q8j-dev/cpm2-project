namespace _Car_Parking.Scripts.Multiplayer.DataPack
{
	public class PersonLocoStatePacker : PackerBase<PersonLocoState>
	{
		public const float MaxSyncedSpeed = 8f;

		private readonly RangedFloatPacker _yaw = new RangedFloatPacker(10, 0f, 360f);

		private readonly RangedFloatPacker _speed = new RangedFloatPacker(7, 0f, 8f);

		private readonly IntPacker _flags = new IntPacker(8, signed: false);

		private readonly IntPacker _emotion = new IntPacker(7, signed: false);

		public override void Pack(ref long current, PersonLocoState data)
		{
			_yaw.Pack(ref current, data.Yaw);
			_speed.Pack(ref current, data.MoveSpeed);
			_flags.Pack(ref current, (int)data.Flags);
			_emotion.Pack(ref current, data.Emotion);
		}

		public override PersonLocoState Unpack(ref long current)
		{
			int emotion = _emotion.Unpack(ref current);
			int num = _flags.Unpack(ref current);
			float moveSpeed = _speed.Unpack(ref current);
			float yaw = _yaw.Unpack(ref current);
			return new PersonLocoState
			{
				Yaw = yaw,
				MoveSpeed = moveSpeed,
				Flags = (PersonNetFlags)num,
				Emotion = emotion
			};
		}
	}
}

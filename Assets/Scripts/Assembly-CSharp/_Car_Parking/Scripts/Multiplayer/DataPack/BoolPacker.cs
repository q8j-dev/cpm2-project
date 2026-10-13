namespace _Car_Parking.Scripts.Multiplayer.DataPack
{
	public class BoolPacker : PackerBase<bool>
	{
		public override void Pack(ref long current, bool data)
		{
			current <<= 1;
			if (data)
			{
				current++;
			}
		}

		public override bool Unpack(ref long current)
		{
			long num = current & 1;
			current >>= 1;
			return num == 1;
		}
	}
}

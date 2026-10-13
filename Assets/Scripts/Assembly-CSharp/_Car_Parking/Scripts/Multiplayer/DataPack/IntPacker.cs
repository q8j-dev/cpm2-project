namespace _Car_Parking.Scripts.Multiplayer.DataPack
{
	public class IntPacker : PackerBase<int>
	{
		private readonly int _bits;

		private readonly BoolPacker _boolPacker;

		public IntPacker(int bits, bool signed)
		{
			_bits = bits;
			if (signed)
			{
				_boolPacker = new BoolPacker();
			}
		}

		public override void Pack(ref long current, int data)
		{
			_boolPacker?.Pack(ref current, data >= 0);
			current <<= _bits;
			current += data & ((1 << _bits) - 1);
		}

		public override int Unpack(ref long current)
		{
			long num = current & ((1 << _bits) - 1);
			current >>= _bits;
			bool? flag = _boolPacker?.Unpack(ref current);
			int num2 = (int)num;
			if (flag.HasValue && !flag.Value)
			{
				num2 = -num2;
			}
			return num2;
		}
	}
}

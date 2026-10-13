using UnityEngine;

namespace _Car_Parking.Scripts.Multiplayer.DataPack
{
	public class RangedFloatPacker : PackerBase<float>
	{
		private readonly Float01Packer _packer;

		private readonly Vector2 _range;

		public RangedFloatPacker(int precision, float min = 0f, float max = 1f)
		{
			_packer = new Float01Packer(precision);
			_range = new Vector2(min, max);
		}

		public override void Pack(ref long current, float data)
		{
			_packer.Pack(ref current, _range.InverseLerp(data));
		}

		public override float Unpack(ref long current)
		{
			return _range.Lerp(_packer.Unpack(ref current));
		}
	}
}

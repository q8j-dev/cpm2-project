using UnityEngine;

namespace _Car_Parking.Scripts.Multiplayer.DataPack
{
	public class RangedVectorPacker : PackerBase<Vector3>
	{
		private readonly RangedFloatPacker _x;

		private readonly RangedFloatPacker _y;

		private readonly RangedFloatPacker _z;

		public RangedVectorPacker((int Precision, float x, float y) x, (int Precision, float x, float y) y, (int Precision, float x, float y) z)
		{
			_x = new RangedFloatPacker(x.Precision, x.x, x.y);
			_y = new RangedFloatPacker(y.Precision, y.x, y.y);
			_z = new RangedFloatPacker(z.Precision, z.x, z.y);
		}

		public override void Pack(ref long current, Vector3 data)
		{
			_x.Pack(ref current, data.x);
			_y.Pack(ref current, data.y);
			_z.Pack(ref current, data.z);
		}

		public override Vector3 Unpack(ref long current)
		{
			float z = _z.Unpack(ref current);
			float y = _y.Unpack(ref current);
			return new Vector3(_x.Unpack(ref current), y, z);
		}
	}
}

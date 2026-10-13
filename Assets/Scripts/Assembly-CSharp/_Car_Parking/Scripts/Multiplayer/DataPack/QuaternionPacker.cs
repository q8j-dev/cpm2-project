using UnityEngine;

namespace _Car_Parking.Scripts.Multiplayer.DataPack
{
	public class QuaternionPacker : PackerBase<Quaternion>
	{
		private readonly Float01Packer _xPacker;

		private readonly Float01Packer _yPacker;

		private readonly Float01Packer _zPacker;

		private readonly Float01Packer _wPacker;

		public QuaternionPacker(int xPrecision, int yPrecision, int zPrecision, int wPrecision)
		{
			_xPacker = new Float01Packer(xPrecision);
			_yPacker = new Float01Packer(yPrecision);
			_zPacker = new Float01Packer(zPrecision);
			_wPacker = new Float01Packer(wPrecision);
		}

		public override void Pack(ref long current, Quaternion data)
		{
			_xPacker.Pack(ref current, Mathf.InverseLerp(-1f, 1f, data.x));
			_yPacker.Pack(ref current, Mathf.InverseLerp(-1f, 1f, data.y));
			_zPacker.Pack(ref current, Mathf.InverseLerp(-1f, 1f, data.z));
			_wPacker.Pack(ref current, Mathf.InverseLerp(-1f, 1f, data.w));
		}

		public override Quaternion Unpack(ref long current)
		{
			float w = Mathf.Lerp(-1f, 1f, _wPacker.Unpack(ref current));
			float z = Mathf.Lerp(-1f, 1f, _zPacker.Unpack(ref current));
			float y = Mathf.Lerp(-1f, 1f, _yPacker.Unpack(ref current));
			return new Quaternion(Mathf.Lerp(-1f, 1f, _xPacker.Unpack(ref current)), y, z, w);
		}
	}
}

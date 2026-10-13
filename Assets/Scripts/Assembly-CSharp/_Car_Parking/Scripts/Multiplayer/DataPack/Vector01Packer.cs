using UnityEngine;

namespace _Car_Parking.Scripts.Multiplayer.DataPack
{
	public class Vector01Packer : PackerBase<Vector3>
	{
		private readonly Float01Packer _packX;

		private readonly Float01Packer _packY;

		private readonly Float01Packer _packZ;

		public Vector01Packer(int precisionX, int precisionY, int precisionZ)
		{
			_packX = new Float01Packer(precisionX);
			_packY = new Float01Packer(precisionY);
			_packZ = new Float01Packer(precisionZ);
		}

		public override void Pack(ref long current, Vector3 data)
		{
			_packX.Pack(ref current, data.x);
			_packY.Pack(ref current, data.y);
			_packZ.Pack(ref current, data.z);
		}

		public override Vector3 Unpack(ref long current)
		{
			float z = _packZ.Unpack(ref current);
			float y = _packY.Unpack(ref current);
			return new Vector3(_packX.Unpack(ref current), y, z);
		}
	}
}

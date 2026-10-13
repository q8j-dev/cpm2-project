using UnityEngine;

namespace _Car_Parking.Scripts.Multiplayer.DataPack
{
	public class Float01Packer : PackerBase<float>
	{
		private readonly int _precision;

		private readonly int _allOnes;

		public Float01Packer(int precision)
		{
			_precision = precision;
			_allOnes = (1 << _precision) - 1;
		}

		public override void Pack(ref long current, float data)
		{
			current <<= _precision;
			current += (int)(Mathf.Clamp01(data) * (float)_allOnes) & _allOnes;
		}

		public override float Unpack(ref long current)
		{
			long num = current & _allOnes;
			current >>= _precision;
			return (float)num / (float)_allOnes;
		}
	}
}

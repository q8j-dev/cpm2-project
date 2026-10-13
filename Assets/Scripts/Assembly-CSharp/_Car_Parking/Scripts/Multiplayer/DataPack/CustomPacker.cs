using System;

namespace _Car_Parking.Scripts.Multiplayer.DataPack
{
	public class CustomPacker<T> : PackerBase<T>
	{
		private readonly Func<long, T, long> _pack;

		private readonly Func<long, (T, long)> _unpack;

		public CustomPacker(Func<long, T, long> pack, Func<long, (T, long)> unpack)
		{
			_pack = pack;
			_unpack = unpack;
		}

		public override void Pack(ref long current, T data)
		{
			current = _pack(current, data);
		}

		public override T Unpack(ref long current)
		{
			(T, long) tuple = _unpack(current);
			T item = tuple.Item1;
			long item2 = tuple.Item2;
			current = item2;
			return item;
		}
	}
}

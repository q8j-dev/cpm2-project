using System;

namespace _Car_Parking.Scripts.Multiplayer.MasterMigration
{
	public sealed class PingSampler
	{
		private readonly int[] _buffer;

		private readonly int _minSamples;

		private int _head;

		private int _count;

		private long _sum;

		public PingSampler(int capacity, int minSamples)
		{
			if (capacity <= 0)
			{
				throw new ArgumentOutOfRangeException("capacity");
			}
			if (minSamples <= 0)
			{
				throw new ArgumentOutOfRangeException("minSamples");
			}
			_buffer = new int[capacity];
			_minSamples = minSamples;
		}

		public void AddSample(int ping)
		{
			if (_count == _buffer.Length)
			{
				_sum -= _buffer[_head];
			}
			else
			{
				_count++;
			}
			_buffer[_head] = ping;
			_sum += ping;
			_head = (_head + 1) % _buffer.Length;
		}

		public int? GetAverage()
		{
			if (_count < _minSamples)
			{
				return null;
			}
			return (int)Math.Round((double)_sum / (double)_count);
		}

		public void Clear()
		{
			Array.Clear(_buffer, 0, _buffer.Length);
			_head = 0;
			_count = 0;
			_sum = 0L;
		}
	}
}

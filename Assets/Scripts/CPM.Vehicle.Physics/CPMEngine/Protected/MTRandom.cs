namespace CPMEngine.Protected
{
	internal class MTRandom
	{
		private const int N = 624;

		private const int M = 397;

		private const uint MatrixA = 2567483615u;

		private const uint UpperMask = 2147483648u;

		private const uint LowerMask = 2147483647u;

		private readonly uint[] _state = new uint[624];

		private readonly uint[] _mag01 = new uint[2] { 0u, 2567483615u };

		private int _index;

		public MTRandom()
		{
			Seed(5489u);
		}

		public MTRandom(uint seed)
		{
			Seed(seed);
		}

		public void Seed(uint seed)
		{
			_state[0] = seed;
			for (int i = 1; i < 624; i++)
			{
				_state[i] = 1812433253 * (_state[i - 1] ^ (_state[i - 1] >> 30)) + (uint)i;
			}
			_index = 624;
		}

		private void GenState()
		{
			int i;
			uint num;
			for (i = 0; i < 227; i++)
			{
				num = (_state[i] & 0x80000000u) | (_state[i + 1] & 0x7FFFFFFF);
				_state[i] = _state[i + 397] ^ (num >> 1) ^ _mag01[num & 1];
			}
			for (; i < 623; i++)
			{
				num = (_state[i] & 0x80000000u) | (_state[i + 1] & 0x7FFFFFFF);
				_state[i] = _state[i + -227] ^ (num >> 1) ^ _mag01[num & 1];
			}
			num = (_state[623] & 0x80000000u) | (_state[0] & 0x7FFFFFFF);
			_state[623] = _state[396] ^ (num >> 1) ^ _mag01[num & 1];
			_index = 0;
		}

		public uint Next()
		{
			if (_index >= 624)
			{
				GenState();
			}
			uint num = _state[_index++];
			uint num2 = num ^ (num >> 11);
			uint num3 = num2 ^ ((num2 << 7) & 0x9D2C5680u);
			uint num4 = num3 ^ ((num3 << 15) & 0xEFC60000u);
			return num4 ^ (num4 >> 18);
		}

		public float NextFloat()
		{
			return (float)Next() * 2.3283064E-10f;
		}

		public int NextInt(int max)
		{
			return (int)(Next() % (uint)max);
		}

		public int NextInt(int min, int max)
		{
			return min + (int)(Next() % (uint)(max - min));
		}
	}
}

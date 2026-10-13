using Unity.Mathematics;

namespace CPM.Vehicle.Damage
{
	public sealed class ImpactQueue
	{
		public const float MaxMergedRadius = 2.5f;

		private readonly ImpactEvent[] _items;

		private int _count;

		public int Count => _count;

		public int Capacity => _items.Length;

		public ImpactQueue(int capacity)
		{
			_items = new ImpactEvent[math.max(1, capacity)];
		}

		public void Enqueue(in ImpactEvent impact)
		{
			if (_count < _items.Length)
			{
				_items[_count++] = impact;
				return;
			}
			int num = 0;
			float num2 = float.MaxValue;
			for (int i = 0; i < _count; i++)
			{
				float num3 = math.distancesq(_items[i].WorldPoint, impact.WorldPoint);
				if (!(num3 >= num2))
				{
					num2 = num3;
					num = i;
				}
			}
			_items[num] = Merge(in _items[num], in impact);
		}

		public bool TryDequeue(out ImpactEvent impact)
		{
			if (_count == 0)
			{
				impact = default;
				return false;
			}
			impact = _items[0];
			for (int i = 1; i < _count; i++)
			{
				_items[i - 1] = _items[i];
			}
			_count--;
			return true;
		}

		public void Clear()
		{
			_count = 0;
		}

		private static ImpactEvent Merge(in ImpactEvent a, in ImpactEvent b)
		{
			float num = math.max(a.Depth, 1E-06f);
			float num2 = math.max(b.Depth, 1E-06f);
			float3 float5 = (a.WorldPoint * num + b.WorldPoint * num2) / (num + num2);
			float3 x = a.WorldDir * num + b.WorldDir * num2;
			x = ((math.lengthsq(x) > 1E-08f) ? math.normalize(x) : ((num >= num2) ? a.WorldDir : b.WorldDir));
			return new ImpactEvent
			{
				WorldPoint = float5,
				WorldDir = x,
				Depth = math.max(a.Depth, b.Depth),
				Radius = math.min(2.5f, math.max(math.distance(float5, a.WorldPoint) + a.Radius, math.distance(float5, b.WorldPoint) + b.Radius))
			};
		}
	}
}

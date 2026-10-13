namespace Gley.UrbanSystem.Internal
{
	public class Heap<T> where T : IHeapItem<T>
	{
		private readonly T[] _items;

		private int _currentItemCount;

		public int Count => _currentItemCount;

		public Heap(int maxHeapSize)
		{
			_items = new T[maxHeapSize];
		}

		public void Add(T item)
		{
			item.HeapIndex = _currentItemCount;
			_items[_currentItemCount] = item;
			SortUp(item);
			_currentItemCount++;
		}

		public T RemoveFirst()
		{
			T result = _items[0];
			_currentItemCount--;
			_items[0] = _items[_currentItemCount];
			_items[0].HeapIndex = 0;
			SortDown(_items[0]);
			return result;
		}

		public void UpdateItem(T item)
		{
			SortUp(item);
		}

		public bool Contains(T item)
		{
			return object.Equals(_items[item.HeapIndex], item);
		}

		private void SortDown(T item)
		{
			while (true)
			{
				int num = item.HeapIndex * 2 + 1;
				int num2 = item.HeapIndex * 2 + 2;
				int num3 = 0;
				if (num >= _currentItemCount)
				{
					break;
				}
				num3 = num;
				if (num2 < _currentItemCount)
				{
					ref readonly T reference = ref _items[num];
					T other = _items[num2];
					if (reference.CompareTo(other) < 0)
					{
						num3 = num2;
					}
				}
				T other2 = _items[num3];
				if (item.CompareTo(other2) < 0)
				{
					Swap(item, _items[num3]);
					continue;
				}
				break;
			}
		}

		private void SortUp(T item)
		{
			int num = (item.HeapIndex - 1) / 2;
			while (true)
			{
				T val = _items[num];
				if (item.CompareTo(val) > 0)
				{
					Swap(item, val);
					num = (item.HeapIndex - 1) / 2;
					continue;
				}
				break;
			}
		}

		private void Swap(T itemA, T itemB)
		{
			_items[itemA.HeapIndex] = itemB;
			_items[itemB.HeapIndex] = itemA;
			int heapIndex = itemA.HeapIndex;
			int heapIndex2 = itemB.HeapIndex;
			itemA.HeapIndex = heapIndex2;
			itemB.HeapIndex = heapIndex;
		}
	}
}

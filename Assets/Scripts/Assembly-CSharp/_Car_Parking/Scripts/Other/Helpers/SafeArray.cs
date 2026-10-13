namespace _Car_Parking.Scripts.Other.Helpers
{
	public class SafeArray<T>
	{
		private T[] array;

		public T this[int index]
		{
			get
			{
				if (index < 0 || index >= array.Length)
				{
					return default;
				}
				return array[index];
			}
		}

		public int Length => array.Length;

		public T[] RealArray => array;

		public SafeArray(T[] array)
		{
			this.array = array;
		}
	}
}

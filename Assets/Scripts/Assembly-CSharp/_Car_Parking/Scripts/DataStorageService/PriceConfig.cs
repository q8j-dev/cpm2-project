using UnityEngine;

namespace _Car_Parking.Scripts.DataStorageService
{
	public abstract class PriceConfig<T> : ScriptableObject where T : IPriced
	{
		public abstract T Get(int index);
	}
}

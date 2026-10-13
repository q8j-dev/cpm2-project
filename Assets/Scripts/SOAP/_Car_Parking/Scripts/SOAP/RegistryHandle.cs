using UnityEngine;

namespace _Car_Parking.Scripts.SOAP
{
	public struct RegistryHandle<T> where T : ScriptableObject
	{
		private readonly EventRegistryIDs _id;

		private T _cachedAsset;

		public RegistryHandle(EventRegistryIDs id)
		{
			_id = id;
			_cachedAsset = null;
		}

		public T Get(EventRegistry eventRegistry)
		{
			if ((bool)_cachedAsset)
			{
				return _cachedAsset;
			}
			if ((bool)eventRegistry && eventRegistry.TryGet<T>(_id, out var asset))
			{
				_cachedAsset = asset;
			}
			return _cachedAsset;
		}

		public static implicit operator bool(RegistryHandle<T> handle)
		{
			return handle._cachedAsset != null;
		}
	}
}

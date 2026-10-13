using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Car_Parking.Scripts.UIElement.Chat
{
	public class ObjectPool<T> where T : MonoBehaviour
	{
		private readonly Queue<T> _availableObjects = new Queue<T>();

		private readonly HashSet<T> _allObjects = new HashSet<T>();

		private readonly Func<T> _createFunc;

		public int AvailableCount => _availableObjects.Count;

		public int TotalCount => _allObjects.Count;

		public ObjectPool(Func<T> createFunc, int initialSize = 0)
		{
			_createFunc = createFunc;
			for (int i = 0; i < initialSize; i++)
			{
				T item = _createFunc();
				_allObjects.Add(item);
				_availableObjects.Enqueue(item);
			}
		}

		public T Get()
		{
			T val;
			if (_availableObjects.Count > 0)
			{
				val = _availableObjects.Dequeue();
			}
			else
			{
				val = _createFunc();
				_allObjects.Add(val);
			}
			return val;
		}

		public void Return(T obj)
		{
			if ((bool)obj && _allObjects.Contains(obj))
			{
				obj.gameObject.SetActive(value: false);
				if (!_availableObjects.Contains(obj))
				{
					_availableObjects.Enqueue(obj);
				}
			}
		}

		public void ReturnAll()
		{
			_availableObjects.Clear();
			foreach (T allObject in _allObjects)
			{
				if ((bool)allObject)
				{
					allObject.gameObject.SetActive(value: false);
					_availableObjects.Enqueue(allObject);
				}
			}
		}

		public bool Contains(T obj)
		{
			return _allObjects.Contains(obj);
		}
	}
}

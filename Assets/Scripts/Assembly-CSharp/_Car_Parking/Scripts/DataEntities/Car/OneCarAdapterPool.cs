using System;
using System.Collections.Generic;
using DataEntities.Car;
using UnityEngine;

namespace _Car_Parking.Scripts.DataEntities.Car
{
	public static class OneCarAdapterPool
	{
		private const int SizeOfPool = 3;

		private static Dictionary<int, IOneCarKeysToCollectionsAdapter> _pool = new Dictionary<int, IOneCarKeysToCollectionsAdapter>();

		private static DefaultOneCarKeysToCollectionsAdapter defaultAdapter;

		private static DefaultOneCarKeysToCollectionsAdapter DefaultAdapter
		{
			get
			{
				if (defaultAdapter == null)
				{
					defaultAdapter = new DefaultOneCarKeysToCollectionsAdapter();
				}
				return defaultAdapter;
			}
		}

		public static IOneCarKeysToCollectionsAdapter Get(int carID)
		{
			if (_pool.Count > 3)
			{
				Clear();
			}
			if (_pool.TryGetValue(carID, out var value))
			{
				return value;
			}
			return CreateAdapter(carID);
		}

		private static IOneCarKeysToCollectionsAdapter CreateAdapter(int carID)
		{
			try
			{
				OneCar oneCar = new OneCarFactory(carID).Create();
				if (oneCar == null)
				{
					return DefaultAdapter;
				}
				OneCarKeysToCollectionsAdapter oneCarKeysToCollectionsAdapter = new OneCarKeysToCollectionsAdapter(oneCar);
				_pool.Add(carID, oneCarKeysToCollectionsAdapter);
				return oneCarKeysToCollectionsAdapter;
			}
			catch (Exception ex)
			{
				Debug.LogError("Try get car error. Return default:" + ex);
				return DefaultAdapter;
			}
		}

		public static void Clear()
		{
			_pool.Clear();
			ES3File.cachedFiles.Clear();
		}

		public static IOneCarKeysToCollectionsAdapter GetDefault()
		{
			return new DefaultOneCarKeysToCollectionsAdapter();
		}
	}
}

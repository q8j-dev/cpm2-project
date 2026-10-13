using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

namespace _Car_Parking.Scripts.Database.PlayerData
{
	[Serializable]
	[Preserve]
	public class CarsHashesPool
	{
		public Dictionary<string, OneCarHashes> CarsHashes = new Dictionary<string, OneCarHashes>();

		public OneCarHashes Get(string carInstanceId)
		{
			if (CarsHashes.TryGetValue(carInstanceId, out var value))
			{
				return value;
			}
			OneCarHashes oneCarHashes = new OneCarHashes();
			CarsHashes[carInstanceId] = oneCarHashes;
			return oneCarHashes;
		}

		public bool Remove(string carInstanceId)
		{
			return CarsHashes.Remove(carInstanceId);
		}

		public bool Contains(string carInstanceId)
		{
			return CarsHashes.ContainsKey(carInstanceId);
		}
	}
}

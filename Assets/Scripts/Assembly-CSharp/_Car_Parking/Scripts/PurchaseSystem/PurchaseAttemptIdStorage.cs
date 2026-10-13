using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace _Car_Parking.Scripts.PurchaseSystem
{
	public class PurchaseAttemptIdStorage
	{
		private const string AttemptPrefsKey = "PurchaseAttemptIds";

		private const string ProcessCountPrefsKey = "PurchaseProcessCounts";

		private readonly Dictionary<string, string> _attemptIds;

		private readonly Dictionary<string, int> _processCounts;

		public PurchaseAttemptIdStorage()
		{
			_attemptIds = Load();
			_processCounts = LoadCounts();
		}

		public string GetOrCreate(string productId, out bool isNew)
		{
			if (_attemptIds.TryGetValue(productId, out var value))
			{
				isNew = false;
				return value;
			}
			string text = Guid.NewGuid().ToString();
			_attemptIds[productId] = text;
			Save("PurchaseAttemptIds", _attemptIds);
			isNew = true;
			return text;
		}

		public string Get(string productId)
		{
			if (!_attemptIds.TryGetValue(productId, out var value))
			{
				return "";
			}
			return value;
		}

		public void AddProcessPurchase(string attemptId)
		{
			_processCounts.TryGetValue(attemptId, out var value);
			value++;
			_processCounts[attemptId] = value;
			Save("PurchaseProcessCounts", _processCounts);
		}

		public bool IsRetry(string attemptId)
		{
			if (_processCounts.TryGetValue(attemptId, out var value))
			{
				return value > 1;
			}
			return false;
		}

		public void Remove(string productId)
		{
			if (_attemptIds.TryGetValue(productId, out var value))
			{
				_attemptIds.Remove(productId);
				_processCounts.Remove(value);
				Save("PurchaseAttemptIds", _attemptIds);
				Save("PurchaseProcessCounts", _processCounts);
			}
		}

		private void Save<T>(string key, T data)
		{
			string value = JsonConvert.SerializeObject(data);
			PlayerPrefs.SetString(key, value);
			PlayerPrefs.Save();
		}

		private Dictionary<string, string> Load()
		{
			string value = PlayerPrefs.GetString("PurchaseAttemptIds", "");
			if (string.IsNullOrEmpty(value))
			{
				return new Dictionary<string, string>();
			}
			try
			{
				return JsonConvert.DeserializeObject<Dictionary<string, string>>(value) ?? new Dictionary<string, string>();
			}
			catch
			{
				return new Dictionary<string, string>();
			}
		}

		private Dictionary<string, int> LoadCounts()
		{
			string value = PlayerPrefs.GetString("PurchaseProcessCounts", "");
			if (string.IsNullOrEmpty(value))
			{
				return new Dictionary<string, int>();
			}
			try
			{
				return JsonConvert.DeserializeObject<Dictionary<string, int>>(value) ?? new Dictionary<string, int>();
			}
			catch
			{
				return new Dictionary<string, int>();
			}
		}
	}
}

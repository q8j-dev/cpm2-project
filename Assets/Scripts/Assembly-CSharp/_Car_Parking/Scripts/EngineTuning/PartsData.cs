using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Car_Parking.Scripts.EngineTuning
{
	[Serializable]
	public class PartsData<TPart> where TPart : PartItem
	{
		public List<TPart> Models = new List<TPart>();

		private Dictionary<int, TPart> ModelsDictionary = new Dictionary<int, TPart>();

		private bool wasInitialized;

		private const int FamilyStep = 100000;

		private const int TypeStep = 1000;

		public TPart this[int index]
		{
			get
			{
				EnsureInitialized();
				if (Models.Count == 0)
				{
					Debug.LogWarning("Count:0 " + typeof(TPart));
					return null;
				}
				if (ModelsDictionary.TryGetValue(index, out var value))
				{
					return value;
				}
				TPart val = FindClosestInSameFamily(index);
				if (val != null)
				{
					Debug.LogWarning($"Деталь {index} ({typeof(TPart).Name}) не найдена — " + $"подставлен ближайший вариант той же семьи: {val.ModelId}.");
					return val;
				}
				Debug.LogError($"Деталь {index} ({typeof(TPart).Name}) не найдена, и замены " + $"в семье {FamilyOf(index)} нет — вернул null.");
				return null;
			}
		}

		public void Init()
		{
			if (Models == null || ModelsDictionary == null)
			{
				return;
			}
			foreach (TPart model in Models)
			{
				if (model == null)
				{
					continue;
				}
				if (ModelsDictionary.ContainsKey(model.ModelId))
				{
					int modelId = model.ModelId;
					while (ModelsDictionary.ContainsKey(model.ModelId) && model.ModelId < modelId + 50)
					{
						model.ModelId++;
					}
					Debug.LogError("____________");
					Debug.LogError($"New ID for {model.GetName}:{model.ModelId}", model);
					Debug.LogError($"Old part model id was in: {modelId}", ModelsDictionary[modelId]);
				}
				ModelsDictionary.Add(model.ModelId, model);
			}
		}

		private static int FamilyOf(int modelId)
		{
			return modelId / 100000;
		}

		private static int TypeCodeOf(int modelId)
		{
			return modelId / 1000 % 100;
		}

		private TPart FindClosestInSameFamily(int index)
		{
			int num = FamilyOf(index);
			int num2 = TypeCodeOf(index);
			TPart val = null;
			foreach (KeyValuePair<int, TPart> item in ModelsDictionary)
			{
				if (FamilyOf(item.Key) == num && TypeCodeOf(item.Key) == num2 && (val == null || item.Key < val.ModelId))
				{
					val = item.Value;
				}
			}
			return val;
		}

		private void EnsureInitialized()
		{
			try
			{
				if (!wasInitialized)
				{
					Init();
					wasInitialized = true;
				}
			}
			catch (Exception ex)
			{
				Debug.LogError("TPart:" + ex);
			}
		}

		public bool TryGetExact(int index, out TPart part)
		{
			EnsureInitialized();
			if (Models == null || Models.Count == 0)
			{
				part = null;
				return false;
			}
			return ModelsDictionary.TryGetValue(index, out part);
		}
	}
}

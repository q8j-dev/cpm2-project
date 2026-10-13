using System;
using System.Collections.Generic;
using CPM.Balance;
using UnityEngine;
using UnityEngine.Scripting;

namespace _Car_Parking.Scripts.EngineTuning.TuningTimer
{
	[Preserve]
	public sealed class BalanceInstallTimeCatalog : IInstallTimeCatalog
	{
		private readonly IBalanceDocumentStore _documents;

		private readonly Dictionary<PartType, float> _seconds = new Dictionary<PartType, float>();

		public BalanceInstallTimeCatalog(IBalanceDocumentStore documents)
		{
			_documents = documents;
			_documents.Changed += Rebuild;
			Rebuild();
		}

		public bool TryGet(PartType partType, out float seconds)
		{
			return _seconds.TryGetValue(partType, out seconds);
		}

		private void Rebuild()
		{
			_seconds.Clear();
			List<InstallTimeBalance> list = _documents.Current?.Tuning?.InstallTimes;
			if (list == null)
			{
				return;
			}
			foreach (InstallTimeBalance item in list)
			{
				if (item != null)
				{
					if (!Enum.TryParse<PartType>(item.PartType, out var result) || !Enum.IsDefined(typeof(PartType), result))
					{
						Debug.LogWarning("BalanceInstallTimeCatalog: \"" + item.PartType + "\" — не член PartType, запись игнорируется этой сборкой.");
					}
					else if (item.Seconds <= 0)
					{
						Debug.LogError($"BalanceInstallTimeCatalog: у \"{result}\" время {item.Seconds} — " + "запись игнорируется, длительность берётся из конфига сборки.");
					}
					else if (_seconds.ContainsKey(result))
					{
						Debug.LogWarning($"BalanceInstallTimeCatalog: \"{result}\" встречается в документе " + "дважды — в силе остаётся первое значение.");
					}
					else
					{
						_seconds[result] = item.Seconds;
					}
				}
			}
		}
	}
}

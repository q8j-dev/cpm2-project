using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Persistence.Systems;
using VContainer;

namespace _Car_Parking.Scripts.Persistence.Migration
{
	public class LocalDataMigrationManager
	{
		private class EntryPair
		{
			public DataVersionHistory.Entry From { get; private set; }

			public DataVersionHistory.Entry To { get; private set; }

			public string Key { get; private set; }

			public EntryPair(DataVersionHistory.Entry from, DataVersionHistory.Entry to, string key)
			{
				From = from;
				To = to;
				Key = key;
			}
		}

		private readonly RuntimePlayerDataHandler runtimePlayerDataHandler;

		private readonly VersionedDataClassesConfig versionedDataClassesConfig;

		private readonly IMigrationManager migrationManager;

		private Action<float> progressCallback;

		private Action MigrationCallback;

		[Inject]
		public LocalDataMigrationManager(RuntimePlayerDataHandler runtimePlayerDataHandler, VersionedDataClassesConfig versionedDataClassesConfig, IMigrationManager migrationManager)
		{
			this.runtimePlayerDataHandler = runtimePlayerDataHandler;
			this.versionedDataClassesConfig = versionedDataClassesConfig;
			this.migrationManager = migrationManager;
		}

		public void SubscribeToMigrationCompletion(Action callback)
		{
			MigrationCallback = (Action)Delegate.Combine(MigrationCallback, callback);
		}

		public async UniTask CheckLocalDataVersions(Action<float> progressCallback)
		{
			this.progressCallback = progressCallback;
			DataClassesVersionStorage versionsData = await runtimePlayerDataHandler.GetDataObject<DataClassesVersionStorage>();
			if (versionsData == null)
			{
				versionsData = new DataClassesVersionStorage();
			}
			versionsData.Initialize();
			HashSet<EntryPair> hashSet = ProcessDataClassesVersions(versionsData);
			if (hashSet.Count == 0)
			{
				return;
			}
			foreach (EntryPair pair in hashSet)
			{
				Type type = Type.GetType(pair.From.saveableType);
				Type type2 = Type.GetType(pair.To.saveableType);
				await migrationManager.MigrateData(type, type2, OnMigrationProgress, commit: true, deleteOld: true);
				versionsData.SetEntry(pair.Key, pair.To.version);
			}
			await runtimePlayerDataHandler.SaveDataObject(versionsData);
			MigrationCallback?.Invoke();
			MigrationCallback = null;
		}

		private void OnMigrationProgress(float progress)
		{
			progressCallback?.Invoke(progress);
		}

		private HashSet<EntryPair> ProcessDataClassesVersions(DataClassesVersionStorage versionsStorage)
		{
			HashSet<EntryPair> hashSet = new HashSet<EntryPair>();
			DataVersionHistory[] entryHistories = versionedDataClassesConfig.entryHistories;
			foreach (DataVersionHistory dataVersionHistory in entryHistories)
			{
				dataVersionHistory.Initialize();
				versionsStorage.VersionsDict.TryGetValue(dataVersionHistory.id, out var value);
				if (string.IsNullOrEmpty(value) && dataVersionHistory.entries.Length != 0)
				{
					value = dataVersionHistory.entries[0].version;
				}
				string version = dataVersionHistory.entries[dataVersionHistory.entries.Length - 1].version;
				if (value != version)
				{
					hashSet.Add(new EntryPair(dataVersionHistory.entriesDict[value], dataVersionHistory.entriesDict[version], dataVersionHistory.id));
				}
			}
			return hashSet;
		}
	}
}

using System;
using System.Collections.Generic;
using CPM.Common;
using Cysharp.Threading.Tasks;
using Persistence.Systems;
using UnityEngine.Scripting;
using VContainer;

namespace _Car_Parking.Scripts.Persistence.Migration
{
	[UnityEngine.Scripting.Preserve]
	public class MigrationManager : IMigrationManager
	{
		public const bool CommitExternal = false;

		public const bool DeleteOldData = false;

		private Dictionary<Type, IMigrator> migratorsDict = new Dictionary<Type, IMigrator>();

		private readonly VersionedDataClassesConfig versionedDataClassesConfig;

		[Inject]
		public MigrationManager(VersionedDataClassesConfig versionedDataClassesConfig)
		{
			this.versionedDataClassesConfig = versionedDataClassesConfig;
		}

		public void Initialize()
		{
		}

		private void ProcessEntryHistory(DataVersionHistory dataVersionHistory)
		{
			DataVersionHistory.Entry[] entries = dataVersionHistory.entries;
			foreach (DataVersionHistory.Entry entry in entries)
			{
				if (!string.IsNullOrEmpty(entry.migratorToType))
				{
					IMigrator migrator = ClassByStringUtility.Create<IMigrator>(entry.migratorToType);
					if (migrator != null)
					{
						migrator.SetPath(dataVersionHistory.path);
						migrator.SetMigrationManager(this);
						migratorsDict.Add(migrator.GetFirstType(), migrator);
					}
				}
			}
		}

		public async UniTask<object> MigrateData(Type from, Type to, Action<float> progressCallback, bool commit = false, bool deleteOld = false)
		{
			return await new MigrationChain(migratorsDict, from, to).GetData(commit, progressCallback, deleteOld);
		}

		public async UniTask<object> MigrateData(object data, Type from, Type to, object inputData = null, bool commit = false, bool deleteOld = false)
		{
			return await new MigrationChain(migratorsDict, from, to).GetData(data, inputData, commit, deleteOld);
		}
	}
}

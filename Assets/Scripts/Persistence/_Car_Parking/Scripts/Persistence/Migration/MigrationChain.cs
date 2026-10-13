using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Car_Parking.Scripts.Persistence.Migration
{
	public class MigrationChain
	{
		private Queue<IMigrator> chain = new Queue<IMigrator>();

		public MigrationChain(Dictionary<Type, IMigrator> migratorsDict, Type start, Type end)
		{
			Type type = start;
			while (type != end)
			{
				if (migratorsDict.TryGetValue(type, out var value))
				{
					chain.Enqueue(value);
					type = value.GetSecondType();
					continue;
				}
				Debug.LogError($"Cant find migrator from type {start} to {end}!");
				break;
			}
		}

		public async UniTask<object> GetData(object data = null, object inputData = null, bool commitAfter = false, bool deleteOld = false)
		{
			object currentData = data;
			try
			{
				IMigrator migrator = chain.Dequeue();
				do
				{
					currentData = await ProcessMigrator(inputData, migrator, currentData);
					await UniTask.Yield();
					if (chain.Count > 0)
					{
						chain.Dequeue();
					}
				}
				while (chain.Count > 0);
				if (commitAfter && migrator != null)
				{
					await migrator.Commit(currentData);
				}
				await UniTask.Yield();
				if (deleteOld)
				{
					migrator?.Delete(inputData);
				}
				await UniTask.Yield();
			}
			catch (Exception arg)
			{
				Debug.LogError($"Error processing migration chain: {arg}");
				return null;
			}
			return currentData;
		}

		public async UniTask<object[]> GetData(bool commitAfter = false, Action<float> progressCallback = null, bool deleteOld = false)
		{
			IMigrator migrator = chain.Dequeue();
			if (migrator == null)
			{
				return null;
			}
			List<object> inputData = migrator.GetMigrationProvider().GetMigrationInputData();
			if (inputData == null || inputData.Count == 0)
			{
				return null;
			}
			object[] currentData;
			do
			{
				currentData = await ProcessMigrator(migrator, inputData, progressCallback);
				if (chain.Count > 0)
				{
					migrator = chain.Dequeue();
				}
			}
			while (chain.Count > 0);
			if (commitAfter)
			{
				object[] array = currentData;
				foreach (object data in array)
				{
					await migrator.Commit(data);
					await UniTask.Yield();
				}
			}
			if (deleteOld)
			{
				foreach (object item in inputData)
				{
					await migrator.Delete(item);
					await UniTask.Yield();
				}
			}
			return currentData;
		}

		private async UniTask<object> ProcessMigrator(object inputData, IMigrator migrator, object currentData)
		{
			if (currentData == null)
			{
				currentData = await migrator.Migrate(inputData, DataSource.Internal);
			}
			currentData = await migrator.Migrate(currentData);
			return currentData;
		}

		private async UniTask<object[]> ProcessMigrator(IMigrator migrator, List<object> inputData, Action<float> progressCallback)
		{
			object[] currentData = new object[inputData.Count];
			for (int i = 0; i < inputData.Count; i++)
			{
				try
				{
					object[] array = currentData;
					int num = i;
					array[num] = await migrator.Migrate(inputData[i], DataSource.Internal);
					progressCallback?.Invoke((float)i / (float)inputData.Count);
					await UniTask.Yield();
				}
				catch (Exception arg)
				{
					Debug.LogError($"Migrator {migrator.GetType()} failed to migrate data at index{inputData[i]}: {arg}");
				}
			}
			return currentData;
		}
	}
}

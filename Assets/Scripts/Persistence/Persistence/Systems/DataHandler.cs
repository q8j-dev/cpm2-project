using System;
using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Persistence.Systems
{
	public static class DataHandler
	{
		private class AsyncReader
		{
			public static FileExistenceState FileExists<T>(string name)
			{
				return GetFileExistenceState<T>(PathUtility.GetPath<T>(name));
			}

			private static FileExistenceState GetFileExistenceState<T>(string path)
			{
				if (File.Exists(path + "_temp"))
				{
					return FileExistenceState.TempExists;
				}
				if (!File.Exists(path))
				{
					return FileExistenceState.NotExists;
				}
				return FileExistenceState.Exists;
			}

			public static FileExistenceState FileExists<T>()
			{
				return GetFileExistenceState<T>(PathUtility.GetPath<T>());
			}

			public static async UniTask<T> ReadAsync<T>(string path, bool useTemp = false)
			{
				try
				{
					if (info.dataPaths.ContainsKey(typeof(T)))
					{
						using (StreamReader sr = new StreamReader(path))
						{
							string encodedData = await sr.ReadToEndAsync();
							return JsonUtility.FromJson<T>(cipherStrategy.Decode(encodedData));
						}
					}
					return default;
				}
				catch (Exception message)
				{
					Debug.LogError(message);
					return default;
				}
			}
		}

		private static class AsyncWriter
		{
			public static async UniTask CreateDefaultDataFile(Type type)
			{
				string data = SyncReader.ReadFromSample(info.jsonResourcesPaths[type]);
				string path = PathUtility.GetPath(type);
				using FileStream fs = File.Create(path);
				using StreamWriter sw = new StreamWriter(fs);
				await sw.WriteAsync(cipherStrategy.Encode(data));
			}

			public static async UniTask<bool> WriteAsync(WriteData writeData)
			{
				try
				{
					using (FileStream fs = File.Create(writeData.path))
					{
						using StreamWriter sw = new StreamWriter(fs);
						await sw.WriteAsync(cipherStrategy.Encode(writeData.json));
					}
					return true;
				}
				catch (Exception message)
				{
					Debug.LogError(message);
					return false;
				}
			}
		}

		private class SyncReader
		{
			public static string ReadFromSample(string path)
			{
				TextAsset textAsset = (TextAsset)Resources.Load(path, typeof(TextAsset));
				if (textAsset == null)
				{
					return "{}";
				}
				return textAsset.text;
			}

			public static T Read<T>(T type = default(T))
			{
				if (info.dataPaths.ContainsKey(typeof(T)))
				{
					using (StreamReader streamReader = new StreamReader(Path.Combine(persistentPath, info.dataPaths[typeof(T)])))
					{
						string text = streamReader.ReadToEnd();
						Debug.Log(text);
						return JsonUtility.FromJson<T>(text);
					}
				}
				return default;
			}
		}

		private static class SyncWriter
		{
			[Obsolete("sync write is no longer supported and developed, please use async write")]
			public static void CreateDefaultDataFile(Type type)
			{
				string value = SyncReader.ReadFromSample(info.jsonResourcesPaths[type]);
				string path = info.dataPaths[type];
				using (File.Create(Path.Combine(persistentPath, path)))
				{
				}
				using StreamWriter streamWriter = new StreamWriter(Path.Combine(persistentPath, path));
				streamWriter.Write(value);
			}

			[Obsolete("sync write is no longer supported and developed, please use async write")]
			public static void RewriteDataFile<T>(string path, T data)
			{
				string value = JsonUtility.ToJson(data);
				using StreamWriter streamWriter = new StreamWriter(Path.Combine(persistentPath, path), append: false);
				streamWriter.Write(value);
			}
		}

		private class PathUtility
		{
			public static string GetPath<T>()
			{
				return Path.Combine(persistentPath, info.dataPaths[typeof(T)] ?? "");
			}

			public static string GetPath(Type type)
			{
				return Path.Combine(persistentPath, info.dataPaths[type] ?? "");
			}

			public static string GetPath<T>(string name)
			{
				return Path.Combine(persistentPath, info.dataPaths[typeof(T)] + "_" + name);
			}
		}

		private static string persistentPath;

		private static string namePrefix = "_";

		private static DataHandlerInfo info;

		private static string accountID;

		private static ICipherStrategy cipherStrategy;

		private static ICipherStrategy pathCipherStrategy;

		public static string AccountID
		{
			private get
			{
				return accountID;
			}
			set
			{
				accountID = cipherStrategy.Encode(value);
			}
		}

		public static async UniTask Init(DataClassesConfig config)
		{
			SetCipherStrategies(config);
			info = new DataHandlerFiller(config, pathCipherStrategy).GetDataHandlerInfo();
			Debug.Log("Data Handler static initialization running");
			persistentPath = Path.Combine(Application.persistentDataPath, "49183427851348");
			await InitDirectoryAndFilesAsync();
			Debug.Log("Initialization finished");
		}

		private static void SetCipherStrategies(DataClassesConfig config)
		{
			Type type = Type.GetType(config.cipherStrategy);
			if (type == null)
			{
				Debug.LogError("Could not get cipher strategy type from string " + config.cipherStrategy);
				cipherStrategy = new NoCipherStrategy();
			}
			else
			{
				cipherStrategy = (ICipherStrategy)Activator.CreateInstance(type);
			}
			Type type2 = Type.GetType(config.pathCipherStrategy);
			if (type2 == null)
			{
				Debug.LogError("Could not get cipher strategy type from string " + config.pathCipherStrategy);
				pathCipherStrategy = new NoCipherStrategy();
			}
			else
			{
				pathCipherStrategy = (ICipherStrategy)Activator.CreateInstance(type2);
			}
		}

		private static async UniTask InitDirectoryAndFilesAsync()
		{
			if (!Directory.Exists(persistentPath))
			{
				Directory.CreateDirectory(persistentPath);
			}
			foreach (KeyValuePair<Type, string> dataPath in info.dataPaths)
			{
				if (!File.Exists(Path.Combine(persistentPath, dataPath.Value)))
				{
					await AsyncWriter.CreateDefaultDataFile(dataPath.Key);
				}
			}
		}

		public static FileExistenceState GetFileExistence<T>(string name)
		{
			return AsyncReader.FileExists<T>(name);
		}

		public static void ClearAllData()
		{
			if (Directory.Exists(persistentPath))
			{
				string[] files = Directory.GetFiles(persistentPath);
				for (int i = 0; i < files.Length; i++)
				{
					File.Delete(files[i]);
				}
			}
		}

		public static FileExistenceState GetFileExistence<T>()
		{
			return AsyncReader.FileExists<T>();
		}

		public static async UniTask<bool> SaveNewDataInFileAsync<T>(T data)
		{
			string json = JsonUtility.ToJson(data);
			string path = PathUtility.GetPath<T>();
			string tempPath = path + "_temp";
			Debug.Log($"Saving {data} of type {typeof(T)} asynchronously");
			bool num = await AsyncWriter.WriteAsync(new WriteData(json, tempPath));
			if (num)
			{
				if (File.Exists(path))
				{
					File.Delete(path);
				}
				File.Move(tempPath, path);
			}
			return num;
		}

		public static async UniTask<bool> SaveNewDataInFileAsync<T>(T data, string subPath)
		{
			string json = JsonUtility.ToJson(data);
			string path = PathUtility.GetPath<T>(subPath);
			string tempPath = path + "_temp";
			Debug.Log($"Saving {data} of type {typeof(T)} asynchronously");
			bool num = await AsyncWriter.WriteAsync(new WriteData(json, tempPath));
			if (num)
			{
				if (File.Exists(path))
				{
					File.Delete(path);
				}
				File.Move(tempPath, path);
			}
			return num;
		}

		public static async UniTask<T> GetDataAsync<T>()
		{
			FileExistenceState fileExistence = GetFileExistence<T>();
			if (fileExistence == FileExistenceState.NotExists)
			{
				return default;
			}
			return await AsyncReader.ReadAsync<T>(PathUtility.GetPath<T>(), fileExistence == FileExistenceState.TempExists);
		}

		public static async UniTask<T> GetDataAsync<T>(string subPath)
		{
			FileExistenceState fileExistence = GetFileExistence<T>(subPath);
			if (fileExistence == FileExistenceState.NotExists)
			{
				return default;
			}
			return await AsyncReader.ReadAsync<T>(PathUtility.GetPath<T>(subPath), fileExistence == FileExistenceState.TempExists);
		}

		public static T GetDataSync<T>(T type = default(T))
		{
			return SyncReader.Read<T>();
		}
	}
}

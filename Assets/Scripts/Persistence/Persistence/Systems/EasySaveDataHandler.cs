using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using CodeStage.AntiCheat.ObscuredTypes;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Scripting;
using _Car_Parking.Scripts.PrefsKeys;

namespace Persistence.Systems
{
	[Preserve]
	public static class EasySaveDataHandler
	{
		private static ObscuredBool Encrypt = true;

		private static ObscuredBool EncryptPath = true;

		private static ICipherStrategy pathEncodingStrategy;

		private static ObscuredString Key { get; set; } = "";

		private static string path => "es3";

		private static string BasePath => Path.Combine(Application.persistentDataPath, path);

		private static string DefaultKey => SystemInfo.deviceUniqueIdentifier.Substring(0, 3) + "c";

		private static bool AccountKeyActive
		{
			get
			{
				if (!string.IsNullOrEmpty(Key))
				{
					return Key != (ObscuredString)DefaultKey;
				}
				return false;
			}
		}

		[Preserve]
		public static void Initialize()
		{
			CheckFolderExists();
			ResetCombinedKey();
			if ((bool)EncryptPath)
			{
				pathEncodingStrategy = new PaddedUTFEncodingStrategy(SystemInfo.deviceUniqueIdentifier.Substring(0, 3));
			}
			else
			{
				pathEncodingStrategy = new NoCipherStrategy();
			}
		}

		private static void CheckFolderExists()
		{
			if (!Directory.Exists(BasePath))
			{
				Directory.CreateDirectory(BasePath);
			}
		}

		public static void ResetCombinedKey()
		{
			string text = "0";
			string email = ManagerKeysForPrefs.Email;
			string accountId = ManagerKeysForPrefs.AccountId;
			if (email == text || email.Length < 1 || accountId.Length < 3)
			{
				Key = DefaultKey;
			}
			else
			{
				Key = SystemInfo.deviceUniqueIdentifier.Substring(0, 3) + accountId.Substring(0, 3);
			}
		}

		private static void DeleteOrphanedFile(ES3Settings settings, string filePath)
		{
			if (!AccountKeyActive)
			{
				return;
			}
			try
			{
				ES3.DeleteFile(settings);
				SaveIntegrity.Forget(FullPath(filePath));
				Debug.LogWarning("[EasySave] Deleted orphaned '" + filePath + "' (unreadable with current identity) — will be recreated.");
			}
			catch (Exception ex)
			{
				Debug.LogWarning("[EasySave] Failed to delete orphaned '" + filePath + "': " + ex.Message);
			}
		}

		private static string FullPath(string filePath)
		{
			return Path.Combine(path, pathEncodingStrategy.Encode(filePath));
		}

		public static bool TryGetFile(string filePath, out ES3File file)
		{
			string text = FullPath(filePath);
			if (ES3File.cachedFiles.TryGetValue(text, out file))
			{
				return true;
			}
			if (!ES3.FileExists(text))
			{
				return false;
			}
			LoadFile(filePath, out file);
			return file != null;
		}

		public static bool FileExists(string filePath)
		{
			string text = FullPath(filePath);
			if (ES3File.cachedFiles.ContainsKey(text))
			{
				return true;
			}
			return ES3.FileExists(text);
		}

		public static bool TryGetOrCreateFile(string filePath, out ES3File file, bool storeFile = false)
		{
			string text = FullPath(filePath);
			if (TryGetFile(filePath, out file))
			{
				return true;
			}
			ES3Settings settings = (Encrypt ? new ES3Settings(text, ES3.EncryptionType.AES, Key) : new ES3Settings(text));
			UnityEngine.Object value = new UnityEngine.Object();
			try
			{
				ES3.Save("temp", value, settings);
			}
			catch (Exception ex) when (ex is CryptographicException || ex is FormatException)
			{
				if (ex is CryptographicException && TryMigrateFileFromDefaultKey(text))
				{
					ES3.Save("temp", value, settings);
				}
				else
				{
					if (!AccountKeyActive)
					{
						Debug.LogError("[EasySaveDataHandler] Could not decrypt '" + filePath + "' (" + ex.GetType().Name + ") pre-auth. Skipped.");
						file = null;
						return false;
					}
					Debug.LogError("[EasySaveDataHandler] Could not decrypt '" + filePath + "' with current or default key (" + ex.GetType().Name + "). File will be recreated.");
					DeleteOrphanedFile(settings, filePath);
					ES3.Save("temp", value, settings);
				}
			}
			if (storeFile)
			{
				ES3.DeleteKey("temp", settings);
				ES3.CacheFile(settings);
				file = ES3File.cachedFiles[text];
			}
			else
			{
				ES3.CacheFile(settings);
				file = ES3File.cachedFiles[text];
				file.Sync();
				file.Clear();
				ES3.StoreCachedFile(text);
			}
			SaveIntegrity.OnFileWritten(text);
			return true;
		}

		public static T GetValue<T>(string filePath, string key)
		{
			if (TryGetFile(filePath, out var file))
			{
				return GetValue<T>(file, key);
			}
			return default;
		}

		public static T GetValue<T>(ES3File file, string key)
		{
			return file.Load<T>(key, default);
		}

		public static void SaveValue<T>(T value, string filePath, string key)
		{
			CheckFolderExists();
			string fullPath = FullPath(filePath);
			ES3Settings eS3Settings = GetES3Settings(fullPath);
			try
			{
				ES3.Save(key, value, eS3Settings);
				SaveIntegrity.OnFileWritten(fullPath);
			}
			catch (Exception ex) when (ex is CryptographicException || ex is FormatException)
			{
				if (AccountKeyActive)
				{
					DeleteOrphanedFile(eS3Settings, filePath);
					ES3.Save(key, value, eS3Settings);
					SaveIntegrity.OnFileWritten(fullPath);
					return;
				}
				Debug.LogError("[EasySaveDataHandler] Skipped save of '" + filePath + "' (" + ex.GetType().Name + ") pre-auth.");
			}
		}

		public static void SaveValue<T>(T value, ES3File file, string key)
		{
			CheckFolderExists();
			file.Save(key, value);
		}

		public static void SaveValues(Dictionary<string, object> values, string filePath)
		{
			CheckFolderExists();
			TryGetFile(filePath, out var file);
			foreach (KeyValuePair<string, object> value in values)
			{
				if (file == null)
				{
					SaveValue(value.Value, filePath, value.Key);
					TryGetFile(filePath, out file);
				}
				else
				{
					SaveValue(value.Value, file, value.Key);
				}
			}
			SaveFile(filePath);
		}

		private static void LoadFile(string filePath, out ES3File file)
		{
			string text = FullPath(filePath);
			ES3Settings eS3Settings = GetES3Settings(text);
			try
			{
				ES3.CacheFile(eS3Settings);
				SaveIntegrity.OnFileRead(text);
			}
			catch (Exception ex) when (ex is CryptographicException || ex is FormatException)
			{
				if (!(ex is CryptographicException) || !TryMigrateFileFromDefaultKey(text))
				{
					Debug.LogError("[EasySaveDataHandler] Could not decrypt '" + filePath + "' with current or default key (" + ex.GetType().Name + "). File will be skipped.");
					DeleteOrphanedFile(eS3Settings, filePath);
					file = null;
					return;
				}
				ES3.CacheFile(eS3Settings);
			}
			ES3File.cachedFiles.TryGetValue(text, out file);
		}

		private static bool TryMigrateFileFromDefaultKey(string fullPath)
		{
			try
			{
				byte[] array = ES3.LoadRawBytes(new ES3Settings(fullPath, ES3.EncryptionType.AES, DefaultKey));
				if (array == null || array.Length == 0)
				{
					return false;
				}
				byte[] bytes = ES3.DecryptBytes(array, DefaultKey);
				SaveIntegrity.OnFileRead(fullPath);
				ES3.SaveRaw(ES3.EncryptBytes(bytes, Key), fullPath);
				SaveIntegrity.OnFileWritten(fullPath);
				Debug.Log("[EasySaveDataHandler] Migrated '" + fullPath + "' from DefaultKey to current key.");
				return true;
			}
			catch (Exception ex)
			{
				Debug.LogWarning("[EasySaveDataHandler] Migration failed for '" + fullPath + "': " + ex.Message);
				return false;
			}
		}

		private static ES3Settings GetES3Settings(string fullPath)
		{
			if ((bool)Encrypt)
			{
				if (Key == (ObscuredString)"")
				{
					ResetCombinedKey();
				}
				return new ES3Settings(fullPath, ES3.EncryptionType.AES, Key);
			}
			return new ES3Settings(fullPath);
		}

		public static void ReleaseFile(string path)
		{
			string key = FullPath(path);
			if (ES3File.cachedFiles.ContainsKey(key))
			{
				ES3File.cachedFiles.Remove(key);
			}
		}

		public static void SaveFile(string filePath)
		{
			string text = FullPath(filePath);
			if (!ES3File.cachedFiles.ContainsKey(text))
			{
				return;
			}
			try
			{
				ES3.StoreCachedFile(GetES3Settings(text));
				SaveIntegrity.OnFileWritten(text);
			}
			catch (Exception ex)
			{
				Debug.LogWarning("[EasySave] StoreCachedFile skipped '" + filePath + "': " + ex.GetType().Name);
			}
		}

		public static void DeleteValue(ES3File file, string key)
		{
			file.DeleteKey(key);
		}

		public static void DeleteValue(string filePath, string key)
		{
			if (TryGetFile(filePath, out var file))
			{
				file.DeleteKey(key);
				SaveFile(filePath);
			}
		}

		public static async UniTask ReCipherToAuthorized()
		{
			if ((bool)Encrypt)
			{
				ResetCombinedKey();
				await ReCipherAllFiles(new EncryptionData
				{
					encrypt = true,
					key = DefaultKey
				}, new EncryptionData
				{
					encrypt = true,
					key = Key
				});
			}
		}

		private static async UniTask ReCipherAllFiles(EncryptionData oldData, EncryptionData newData)
		{
			if (!Directory.Exists(BasePath))
			{
				return;
			}
			foreach (string item in Directory.EnumerateFiles(BasePath, "*", SearchOption.AllDirectories))
			{
				try
				{
					byte[] oldDataBytes = GetOldDataBytes(oldData, item);
					if (oldDataBytes.Length != 0)
					{
						oldDataBytes = RecipherWithNewData(newData, oldDataBytes);
						ES3.SaveRaw(oldDataBytes, item);
						SaveIntegrity.OnFileWritten(item);
					}
				}
				catch (Exception ex)
				{
					Debug.LogWarning("[EasySave] ReCipher skipped '" + Path.GetFileName(item) + "': " + ex.GetType().Name);
				}
				await UniTask.Delay(1);
			}
		}

		private static byte[] RecipherWithNewData(EncryptionData newData, byte[] bytes)
		{
			if (newData.encrypt)
			{
				bytes = ES3.EncryptBytes(bytes, newData.key);
			}
			return bytes;
		}

		private static byte[] GetOldDataBytes(EncryptionData oldData, string filePath)
		{
			byte[] array = ES3.LoadRawBytes(new ES3Settings(filePath));
			if (oldData.encrypt && array.Length != 0)
			{
				array = ES3.DecryptBytes(array, oldData.key);
			}
			return array;
		}

		public static void DeleteDirectory(string directoryPath)
		{
			string dir = Path.Combine(Application.persistentDataPath, path, directoryPath);
			if (Directory.Exists(dir))
			{
				ForgetSignaturesUnder(dir);
				Directory.Delete(dir, recursive: true);
			}
			ClearAllCache();
		}

		public static void DeleteAll()
		{
			string dir = Path.Combine(Application.persistentDataPath, path);
			if (Directory.Exists(dir))
			{
				ForgetSignaturesUnder(dir);
				Directory.Delete(dir, recursive: true);
			}
			ES3File.cachedFiles.Clear();
		}

		public static void DeleteFile(string filePath)
		{
			ReleaseFile(filePath);
			ES3.DeleteFile(FullPath(filePath));
			SaveIntegrity.Forget(FullPath(filePath));
		}

		private static void ForgetSignaturesUnder(string dir)
		{
			foreach (string item in Directory.EnumerateFiles(dir, "*", SearchOption.AllDirectories))
			{
				SaveIntegrity.Forget(item);
			}
		}

		public static void ClearAllCache()
		{
			ES3File.cachedFiles.Clear();
		}
	}
}

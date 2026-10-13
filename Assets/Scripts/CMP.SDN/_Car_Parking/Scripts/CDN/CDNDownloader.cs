using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Scripting;
using VContainer.Unity;

namespace _Car_Parking.Scripts.CDN
{
	[Preserve]
	public class CDNDownloader : ICDNDownloader, IInitializable, IDisposable
	{
		private const string PrefixUrl = "https://olzhas1.b-cdn.net/";

		private const string SuffixMeta = ".meta";

		private const int DeleteFilesAfterDays = 10;

		private const float DelayToDeleteOldFiles = 10f;

		private static readonly string CacheDir = Path.Combine(Application.persistentDataPath, "cdn");

		public static bool LogsEnabled = true;

		private List<string> _downloadUrls = new List<string>();

		private Dictionary<string, Texture2D> _localCash = new Dictionary<string, Texture2D>();

		public void Initialize()
		{
			if (!Directory.Exists(CacheDir))
			{
				Directory.CreateDirectory(CacheDir);
			}
			DeleteOldFilesAsync().Forget();
		}

		public void Dispose()
		{
		}

		public async UniTask<Texture2D> LoadImageAsync(string url)
		{
			if (string.IsNullOrEmpty(url))
			{
				return null;
			}
			string urlFull = "https://olzhas1.b-cdn.net/" + url;
			string fileName = Path.GetFileName(urlFull);
			string filePath = Path.Combine(CacheDir, fileName);
			string metaPath = filePath + ".meta";
			while (_downloadUrls.Contains(url))
			{
				await UniTask.Yield();
			}
			if (_localCash.TryGetValue(url, out var texture) && (bool)texture)
			{
				return texture;
			}
			_downloadUrls.Add(url);
			try
			{
				if (File.Exists(filePath) && File.Exists(metaPath))
				{
					texture = CreateTexture(await File.ReadAllBytesAsync(filePath));
					if ((bool)texture)
					{
						await File.WriteAllTextAsync(metaPath, GetMetaData());
					}
					else
					{
						File.Delete(filePath);
						File.Delete(metaPath);
					}
				}
				if (!texture)
				{
					using UnityWebRequest request = UnityWebRequest.Get(urlFull);
					await request.SendWebRequest();
					byte[] data = request.downloadHandler.data;
					texture = CreateTexture(data);
					await File.WriteAllBytesAsync(filePath, data);
					await File.WriteAllTextAsync(metaPath, GetMetaData());
				}
			}
			catch (UnityWebRequestException ex)
			{
				UnityEngine.Debug.LogWarning($"[CDNDownloader] image not loaded: {url}. status: {ex.ResponseCode}. {ex.Error}");
			}
			catch (Exception exception)
			{
				UnityEngine.Debug.LogException(exception);
			}
			finally
			{
				_downloadUrls.Remove(url);
			}
			if ((bool)texture)
			{
				_localCash[url] = texture;
			}
			return texture;
		}

		public void ClearLocalCache()
		{
			foreach (KeyValuePair<string, Texture2D> item in _localCash)
			{
				if ((bool)item.Value)
				{
					UnityEngine.Object.Destroy(item.Value);
				}
			}
			_localCash.Clear();
		}

		public void ClearLocalCache(string url)
		{
			if (!string.IsNullOrEmpty(url) && _localCash.TryGetValue(url, out var value))
			{
				if ((bool)value)
				{
					UnityEngine.Object.Destroy(value);
				}
				_localCash.Remove(url);
			}
		}

		public bool HaveLocalCache(string url)
		{
			if (string.IsNullOrEmpty(url))
			{
				return false;
			}
			if (_localCash.TryGetValue(url, out var value))
			{
				return value;
			}
			return false;
		}

		private static Texture2D CreateTexture(byte[] bytes)
		{
			Texture2D texture2D = new Texture2D(2, 2, TextureFormat.RGBA32, mipChain: false);
			if (texture2D.LoadImage(bytes))
			{
				return texture2D;
			}
			UnityEngine.Object.Destroy(texture2D);
			return null;
		}

		private string GetMetaData()
		{
			return DateTime.UtcNow.ToString("o");
		}

		[Conditional("UNITY_EDITOR")]
		[Conditional("DEVELOPMENT_BUILD")]
		private static void Log(string message)
		{
			_ = LogsEnabled;
		}

		private async UniTaskVoid DeleteOldFilesAsync()
		{
			await UniTask.Delay(TimeSpan.FromSeconds(10.0));
			while (_downloadUrls.Count > 0)
			{
				await UniTask.Yield();
			}
			string searchPattern = "*.meta";
			string[] files = Directory.GetFiles(CacheDir, searchPattern);
			foreach (string text in files)
			{
				try
				{
					DateTime dateTime = DateTime.Parse(File.ReadAllText(text), CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
					if ((DateTime.UtcNow - dateTime).TotalDays > 10.0)
					{
						string path = text.Substring(0, text.Length - ".meta".Length);
						if (File.Exists(path))
						{
							File.Delete(path);
						}
						File.Delete(text);
					}
				}
				catch (Exception ex)
				{
					UnityEngine.Debug.LogWarning("Failed to clean " + text + ": " + ex.Message);
				}
			}
		}
	}
}

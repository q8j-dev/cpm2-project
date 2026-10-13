using System;
using System.IO;
using System.Text;

namespace CPM.Cdn
{
	public sealed class EtagFileCache
	{
		private readonly string _rootDirectory;

		private readonly string _dataPath;

		private readonly string _sidecarPath;

		public EtagFileCache(string rootDirectory, string fileName)
		{
			_rootDirectory = rootDirectory;
			_dataPath = Path.Combine(rootDirectory, fileName);
			_sidecarPath = _dataPath + ".meta";
		}

		public bool TryRead(string expectedKey, out byte[] bytes, out string etag)
		{
			bytes = null;
			etag = null;
			try
			{
				if (!File.Exists(_dataPath) || !File.Exists(_sidecarPath))
				{
					return false;
				}
				string[] array = File.ReadAllLines(_sidecarPath, Encoding.UTF8);
				if (array.Length < 2)
				{
					return false;
				}
				if (array[0] != expectedKey)
				{
					Invalidate();
					return false;
				}
				bytes = File.ReadAllBytes(_dataPath);
				etag = (string.IsNullOrEmpty(array[1]) ? null : array[1]);
				return true;
			}
			catch (Exception)
			{
				bytes = null;
				etag = null;
				return false;
			}
		}

		public void Write(string key, byte[] bytes, string etagOrNull)
		{
			Directory.CreateDirectory(_rootDirectory);
			File.WriteAllBytes(_dataPath, bytes);
			File.WriteAllLines(_sidecarPath, new string[2]
			{
				key,
				etagOrNull ?? string.Empty
			}, Encoding.UTF8);
		}

		public void Invalidate()
		{
			try
			{
				if (File.Exists(_dataPath))
				{
					File.Delete(_dataPath);
				}
				if (File.Exists(_sidecarPath))
				{
					File.Delete(_sidecarPath);
				}
			}
			catch (Exception)
			{
			}
		}
	}
}

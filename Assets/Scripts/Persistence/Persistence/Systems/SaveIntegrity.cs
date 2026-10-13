using System;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using CPM.Persistence.Integrity;
using CodeStage.AntiCheat.Storage;
using UnityEngine;

namespace Persistence.Systems
{
	public static class SaveIntegrity
	{
		private const bool Enabled = true;

		private const int MarkGeneration = 1;

		private static string _key;

		private static bool _disabled;

		private static bool _forgotOlderGenerations;

		public static bool ViolatedThisSession { get; private set; }

		public static bool MarkedPreviously => ObscuredPrefs.GetBool(MarkKey());

		private static string Key
		{
			get
			{
				if (_key != null)
				{
					return _key;
				}
				string forSaveFiles = NativeSalt.ForSaveFiles;
				if (forSaveFiles == null)
				{
					_disabled = true;
					return null;
				}
				_key = SystemInfo.deviceUniqueIdentifier + ":" + forSaveFiles;
				return _key;
			}
		}

		public static event Action ViolationDetected;

		private static string SignaturePrefix()
		{
			return FromCodes(new byte[3] { 115, 105, 95 });
		}

		private static string MarkKey()
		{
			return MarkKey(1);
		}

		private static string MarkKey(int generation)
		{
			string text = FromCodes(new byte[7] { 115, 105, 95, 109, 97, 114, 107 });
			if (generation <= 1)
			{
				return text;
			}
			return text + generation.ToString(CultureInfo.InvariantCulture);
		}

		public static void OnFileWritten(string fullPath)
		{
			if (_disabled)
			{
				return;
			}
			EnsureOlderGenerationsForgotten();
			try
			{
				byte[] array = ReadRaw(fullPath);
				if (array == null)
				{
					Forget(fullPath);
					return;
				}
				string key = Key;
				if (key != null)
				{
					ObscuredPrefs.SetString(PrefsKeyFor(fullPath), SaveFileSignature.Compute(key, array));
				}
			}
			catch (Exception e)
			{
				FailOpen(e);
			}
		}

		public static void Forget(string fullPath)
		{
			try
			{
				ObscuredPrefs.DeleteKey(PrefsKeyFor(fullPath));
			}
			catch (Exception e)
			{
				FailOpen(e);
			}
		}

		public static void OnFileRead(string fullPath)
		{
			if (_disabled)
			{
				return;
			}
			EnsureOlderGenerationsForgotten();
			try
			{
				byte[] array = ReadRaw(fullPath);
				if (array == null)
				{
					return;
				}
				string key = Key;
				if (key != null)
				{
					string key2 = PrefsKeyFor(fullPath);
					string text = ObscuredPrefs.GetString(key2, string.Empty);
					if (string.IsNullOrEmpty(text))
					{
						ObscuredPrefs.SetString(key2, SaveFileSignature.Compute(key, array));
					}
					else if (!SaveFileSignature.Verify(text, key, array))
					{
						Report(fullPath);
						ObscuredPrefs.SetString(key2, SaveFileSignature.Compute(key, array));
					}
				}
			}
			catch (Exception e)
			{
				FailOpen(e);
			}
		}

		public static void TrustCurrentState()
		{
			ViolatedThisSession = false;
		}

		private static void EnsureOlderGenerationsForgotten()
		{
			if (!_forgotOlderGenerations)
			{
				_forgotOlderGenerations = true;
				ForgetOlderGenerations(1);
			}
		}

		private static void ForgetOlderGenerations(int currentGeneration)
		{
			bool flag = false;
			for (int i = 1; i < currentGeneration; i++)
			{
				string key = MarkKey(i);
				if (ObscuredPrefs.HasKey(key))
				{
					ObscuredPrefs.DeleteKey(key);
					flag = true;
				}
			}
			if (flag)
			{
				ObscuredPrefs.Save();
			}
		}

		private static void Report(string fullPath)
		{
			if (!ViolatedThisSession)
			{
				ViolatedThisSession = true;
				ObscuredPrefs.SetBool(MarkKey(), value: true);
				ViolationDetected?.Invoke();
			}
			Debug.LogError("[SaveIntegrity] Signature mismatch for '" + fullPath + "'. Cloud sync paused for this session.");
		}

		private static void FailOpen(Exception e)
		{
			_disabled = true;
			Debug.LogWarning("[SaveIntegrity] disabled after an error: " + e.Message);
		}

		private static byte[] ReadRaw(string fullPath)
		{
			if (!ES3.FileExists(fullPath))
			{
				return null;
			}
			return ES3.LoadRawBytes(new ES3Settings(fullPath));
		}

		private static string FromCodes(byte[] codes)
		{
			StringBuilder stringBuilder = new StringBuilder(codes.Length);
			for (int i = 0; i < codes.Length; i++)
			{
				stringBuilder.Append((char)codes[i]);
			}
			return stringBuilder.ToString();
		}

		private static string PrefsKeyFor(string path)
		{
			using SHA256 sHA = SHA256.Create();
			byte[] inArray = sHA.ComputeHash(Encoding.UTF8.GetBytes(Normalize(path)));
			return SignaturePrefix() + Convert.ToBase64String(inArray).Substring(0, 22);
		}

		private static string Normalize(string path)
		{
			string text = path.Replace('\\', '/');
			string text2 = Application.persistentDataPath.Replace('\\', '/');
			if (text.StartsWith(text2, StringComparison.Ordinal))
			{
				text = text.Substring(text2.Length);
			}
			return text.TrimStart('/');
		}
	}
}

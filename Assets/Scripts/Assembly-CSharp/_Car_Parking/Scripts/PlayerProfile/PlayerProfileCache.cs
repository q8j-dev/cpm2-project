using System;
using Newtonsoft.Json;
using Persistence.Systems;
using UnityEngine.Scripting;
using VContainer;

namespace _Car_Parking.Scripts.PlayerProfile
{
	[UnityEngine.Scripting.Preserve]
	public class PlayerProfileCache : IPlayerProfileCache
	{
		private const string DefaultFileName = "PlayerProfile";

		private const string OwnerUidKey = "ownerUid";

		private const string ProfileJsonKey = "profileJson";

		private readonly string _fileName;

		[Inject]
		public PlayerProfileCache()
			: this("PlayerProfile")
		{
		}

		public PlayerProfileCache(string fileName)
		{
			_fileName = fileName;
		}

		public void Save(PlayerProfileData data)
		{
			if (data == null || string.IsNullOrEmpty(data.Id))
			{
				return;
			}
			try
			{
				WriteFile(data.Id, JsonConvert.SerializeObject(data));
			}
			catch (Exception)
			{
				try
				{
					EasySaveDataHandler.DeleteFile(_fileName);
					WriteFile(data.Id, JsonConvert.SerializeObject(data));
				}
				catch (Exception)
				{
				}
			}
		}

		private void WriteFile(string ownerUid, string json)
		{
			if (!EasySaveDataHandler.TryGetOrCreateFile(_fileName, out var file, storeFile: true))
			{
				throw new InvalidOperationException("Could not create cache file: " + _fileName);
			}
			EasySaveDataHandler.SaveValue(ownerUid, file, "ownerUid");
			EasySaveDataHandler.SaveValue(json, file, "profileJson");
			EasySaveDataHandler.SaveFile(_fileName);
		}

		public bool TryLoad(string currentUid, out PlayerProfileData data)
		{
			data = null;
			if (string.IsNullOrEmpty(currentUid))
			{
				return false;
			}
			try
			{
				if (!EasySaveDataHandler.FileExists(_fileName))
				{
					return false;
				}
				if (EasySaveDataHandler.GetValue<string>(_fileName, "ownerUid") != currentUid)
				{
					return false;
				}
				string value = EasySaveDataHandler.GetValue<string>(_fileName, "profileJson");
				if (string.IsNullOrEmpty(value))
				{
					return false;
				}
				data = JsonConvert.DeserializeObject<PlayerProfileData>(value);
				return data != null;
			}
			catch (Exception)
			{
				data = null;
				return false;
			}
		}
	}
}
